using System;
using System.Collections.Generic;
using System.Threading;

namespace DelftTools.Utils.Threading
{
    /// <summary>
    /// QueueState can be one of the following: Idle, Running, Pausing
    /// Paused, Stopping.
    /// </summary>
    public enum QueueState
    {
        /// <summary>
        /// Nothing to do.
        /// </summary>
        Idle,
        /// <summary>
        /// Running items in the queue.
        /// </summary>
        Running,
        /// <summary>
        /// Processing of items in the queue is Paused, 
        /// Item(s) currently running will finish their run. 
        /// </summary>
        Pausing,
        /// <summary>
        /// Processing of items in the queue is Paused.
        /// </summary>
        Paused,
        /// <summary>
        /// Unprocessed items are removed from the queue
        /// Running itmes will finish their run.
        /// </summary>
        Stopping,
       
        Aborting
    } ;


    /// <summary>
    /// Subscribe to this if you want to be notified in case items are added/removed
    /// from the queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    public delegate void QueueOperationHandler<T>(T item);

    /// <summary>
    /// http://www.codeproject.com/useritems/NotifyingThreadQueue.asp
    /// 
    /// Questions:
    ///  
    /// Why to store running activities as KeyValuePair
    /// It is better to look for some workflow library or use WPF when it will be in Mono (Seems to be a task for Google Summer of Code 2007)
    /// Overall class looks tooooooo complicated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NotifyingThreadQueue<T> //:IDisposable 
    {
        #region my events

        /// <summary>
        /// Subscribe to this in case you want to monitor the state of the queue.
        /// </summary>
        public event GenericEventHandler<QueueState> QueueStateChanged;

        /// <summary>
        /// Subscribe to this in case you want to know when the threadqueue
        /// finishes on of the items in the queue.
        /// </summary>
        public event GenericEventHandler<T> ThreadFinished;

        /// <summary>
        /// Subscribe to this to be informed about errors taking place
        /// while calculating one of the items in the queue.
        /// </summary>
        public event GenericEventHandler<T, Exception> ThreadError;

        /// <summary>
        /// Subscribe to this in case you want to know when the threadqueue
        /// starts running one of the items in the queue
        /// </summary>
        public event GenericEventHandler<T> ThreadStarted;

        #endregion

        #region my vars

        private object syncobj;
        private int maxthreads;
        private int currentthreads;
        private QueueState qs;
        private Queue<KeyValuePair<T, QueueOperationHandler<T>>> queue;
        private readonly QueueOperationHandler<T> defaultop;
        //private AsyncOperation asyncOperation;
        private Dictionary<KeyValuePair<T, QueueOperationHandler<T>>, Thread> threadDictionary =
            new Dictionary<KeyValuePair<T, QueueOperationHandler<T>>, Thread>();

        #endregion

        #region constructors

        /// <summary>
        /// Constructs the NotifyingThreadQueue. sets the state to QueueState.Idle.
        /// </summary>
        /// <param name="defaultoperation">The default operation to perform on an enqueued item.</param>
        /// <exception cref="System.ArgumentNullException">defaultoperation is null</exception>
        public NotifyingThreadQueue(QueueOperationHandler<T> defaultoperation)
            : this(int.MaxValue, defaultoperation)
        {
        }

        /// <summary>
        /// Constructs the NotifyingThreadQueue. sets the state to QueueState.Idle.
        /// </summary>
        /// <param name="maxthreads">Sets the maximum number of simultaneous operations</param>
        /// <param name="defaultoperation">The default operation to perform on an enqueued item</param>
        /// <exception cref="System.ArgumentException">maxthreads is less than or equal to 0</exception>
        /// <exception cref="System.ArgumentNullException">defaultoperation is null</exception>
        public NotifyingThreadQueue(int maxthreads, QueueOperationHandler<T> defaultoperation)
        {
            if (maxthreads <= 0)
                throw new ArgumentException("maxthreads can not be <= 0");
            if (defaultoperation == null)
                throw new ArgumentNullException("defaultoperation", "defaultoperation can not be null");

            qs = QueueState.Idle;
            syncobj = new object();
            currentthreads = 0;
            this.maxthreads = maxthreads;
            queue = new Queue<KeyValuePair<T, QueueOperationHandler<T>>>();
            defaultop = defaultoperation;
        }

        #endregion

        #region control ops

        /// <summary>
        /// Pauses the execution of future operations. the current operations are allowed to finish.
        /// </summary>
        public void Pause()
        {
            lock (syncobj)
            {
                if (qs == QueueState.Idle)
                {
                    /// this is a judgment call if you pause this when you 
                    /// don’t have any elements in it then you can go directly 
                    /// to paused and this means that you basically want to 
                    /// keep queuing until something happens                    
                    qs = QueueState.Paused;
                    QueueStateChangedInternal(qs);
                }
                else if (qs == QueueState.Running)
                {
                    qs = QueueState.Pausing;
                    QueueStateChangedInternal(qs);

                    /// running means you had some active threads so you couldn’t 
                    /// get to paused right away
                }
                else if (qs == QueueState.Stopping || qs == QueueState.Aborting)
                {
                    ThreadErrorInternal(default(T),
                                        new ThreadStateException("Once the queue is stopping  its done processing"));
                }
                /// if we are already paused or pausing we dont need to do anything
            }
        }

        /// <summary>
        /// Stops the execution of future operations. clears out all pending operations. 
        /// No further operations are allowed to be enqueued. the current operations are 
        /// allowed to finish.
        /// </summary>
        public void Stop()
        {
            lock (syncobj)
            {
                if ((qs == QueueState.Idle) || (qs == QueueState.Stopping) || (qs == QueueState.Aborting))
                {
                    /// do nothing idle has nothing to stop and stopping  
                    /// is already working on stopping 
                    return;
                }
                else if (qs == QueueState.Paused)
                {
                    qs = QueueState.Stopping;
                    QueueStateChangedInternal(qs);

                    /// if we are already paused then we have no threads running 
                    /// so just drop all the extra items in the queue
                    while (queue.Count != 0)
                        ThreadErrorInternal(queue.Dequeue().Key,
                                            new ThreadStateException("the Queue is stopping . no processing done"));

                    /// ensure proper event flow paused-> stopping -> idle
                    qs = QueueState.Idle;
                    QueueStateChangedInternal(qs);
                }
                else
                {
                    qs = QueueState.Stopping;
                    QueueStateChangedInternal(qs);

                    /// why are we not dequeuing everything? that’s b/c if we have threads 
                    /// left they have to finish in their own good time so they can go 
                    /// through the process of getting rid of all the others. both ways work
                    if (currentthreads == 0)
                    {
                        qs = QueueState.Idle;
                        QueueStateChangedInternal(qs);
                    }
                }
            }
        }

        /// <summary>
        /// Abort running item and empty queue.
        /// </summary>
        public void Abort()
        {
            lock (syncobj)
            {
                if ((qs == QueueState.Idle) || (qs == QueueState.Stopping) || (qs == QueueState.Aborting))
                {
                    /// do nothing idle has nothing to stop and stopping  
                    /// is already working on stopping 
                    return;
                }
                else if (qs == QueueState.Paused)
                {
                    qs = QueueState.Aborting;
                    QueueStateChangedInternal(qs);

                    // if we are already paused then we have no threads running 
                    /// so just drop all the extra items in the queue
                    while (queue.Count != 0)
                    {
                        queue.Dequeue();
                        //ThreadErrorInternal(queue.Dequeue().Key, new ThreadStateException("the Queue is stopping . no processing done"));
                    }

                    // ensure proper event flow paused-> stopping -> idle
                    qs = QueueState.Idle;
                    QueueStateChangedInternal(qs);
                }
                else
                {
                    qs = QueueState.Aborting;
                    QueueStateChangedInternal(qs);

                    /// why are we not dequeuing everything? that’s b/c if we have threads 
                    /// left they have to finish in their own good time so they can go 
                    /// through the process of getting rid of all the others. both ways work

                    //abort running threads
                    KeyValuePair<T, QueueOperationHandler<T>>[] kvpArr =
                        new KeyValuePair<T, QueueOperationHandler<T>>[threadDictionary.Keys.Count];
                    threadDictionary.Keys.CopyTo(kvpArr, 0);
                    int kvpCount = threadDictionary.Keys.Count;
                    for (int i = 0; i < kvpCount; i++)
                    {
                        if (threadDictionary.ContainsKey(kvpArr[i]))
                        {
                            threadDictionary[kvpArr[i]].Abort();
                            //ThreadErrorInternal(kvpArr[i].Key,new Exception("Aborted thread"));
                        }
                    }

                    //call to ensure the list is cleared 
                    threadDictionary.Clear();

                    if (currentthreads == 0)
                    {
                        qs = QueueState.Idle;
                        QueueStateChangedInternal(qs);
                    }
                }
            }
        }

        /// <summary>
        /// Abort thread for a specific item / default operation.
        /// </summary>
        /// <param name="Item"></param>
        public void Abort(T Item)
        {
            KeyValuePair<T, QueueOperationHandler<T>> kvp =
                new KeyValuePair<T, QueueOperationHandler<T>>(Item, defaultop);
            Abort(kvp);
        }

        /// <summary>
        /// Abort thread for a specific item/operation combination.
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="operation"></param>
        public void Abort(T Item, QueueOperationHandler<T> operation)
        {
            KeyValuePair<T, QueueOperationHandler<T>> kvp =
                new KeyValuePair<T, QueueOperationHandler<T>>(Item, operation);
            Abort(kvp);
        }

        /// <summary>
        /// Abort thread for key-value pair.
        /// </summary>
        /// <param name="kvp"></param>
        private void Abort(KeyValuePair<T, QueueOperationHandler<T>> kvp)
        {
            lock (syncobj)
            {
                if (qs == QueueState.Idle)
                {
                    return;
                }
                else if (qs == QueueState.Paused)
                {
                    /// if we are already paused then we have no threads running 
                    /// remove item from the queue
                    RemoveQueueKvp(kvp);

                    if (queue.Count == 0)
                    {
                        qs = QueueState.Stopping;
                        QueueStateChangedInternal(qs);
                        /// ensure proper event flow paused-> stopping -> idle
                        qs = QueueState.Idle;
                        QueueStateChangedInternal(qs);
                    }
                    return;
                }
                else //running, pausing or stopping
                    //check first if item is in the queue
                {
                    RemoveQueueKvp(kvp);

                    if (threadDictionary.ContainsKey(kvp))
                    {
                        threadDictionary[kvp].Abort();
                        threadDictionary.Remove(kvp);
                        ThreadErrorInternal(kvp.Key, new Exception("Aborted process"));
                    }
                }
            }
        }

        /// <summary>
        /// Remove specific item from the queue.
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns></returns>
        private void RemoveQueueKvp(KeyValuePair<T, QueueOperationHandler<T>> kvp)
        {
            lock (syncobj)
            {
                if (!queue.Contains(kvp))
                {
                    return;
                }

                //QueueState presentState = qs;
                //qs = QueueState.Pausing;
                Queue<KeyValuePair<T, QueueOperationHandler<T>>> copyOfQueue =
                    new Queue<KeyValuePair<T, QueueOperationHandler<T>>>();
                //enqueue all items of the present queue in a copy
                while (queue.Count != 0)
                {
                    KeyValuePair<T, QueueOperationHandler<T>> item = queue.Dequeue();
                    if (!item.Equals(kvp))
                    {
                        copyOfQueue.Enqueue(queue.Dequeue());
                    }
                    else
                    {
                        ThreadErrorInternal(kvp.Key, new Exception("Item removed from queue"));
                    }
                }
                while (copyOfQueue.Count != 0)
                {
                    queue.Enqueue(copyOfQueue.Dequeue());
                }

                //qs = presentState;
                return;
            }
        }

        /// <summary>
        /// Continues the execution of enqueued operations after a pause.
        /// </summary>
        public void Continue()
        {
            lock (syncobj)
            {
                if ((qs == QueueState.Pausing) || (qs == QueueState.Paused))
                {
                    qs = QueueState.Running;
                    QueueStateChangedInternal(qs);

                    while (currentthreads < maxthreads)
                        TryNewThread();
                }
                else if ((qs == QueueState.Idle) || (qs == QueueState.Running))
                {
                    /// Continuing to process while the queue is idle is meaning 
                    /// less just ignore the command
                    return;
                }
                else if (qs == QueueState.Stopping)
                {
                    ThreadErrorInternal(default(T),
                                        new ThreadStateException("Once the queue is stopping  its done processing"));
                }
            }
        }

        #endregion

        #region data accessors

        /// <summary>
        /// Gets the current QueueState.
        /// </summary>
        public QueueState QueueState
        {
            get { return qs; }
        }

        /// <summary>
        /// Gets the maximum number of operations that can be executed at once.
        /// </summary>
        public int MaxThreads
        {
            get { return maxthreads; }
        }

        /// <summary>
        /// Gets the current number of current ongoing operations.
        /// </summary>
        public int CurrentRunningThreads
        {
            get
            {
                lock (syncobj)
                {
                    return currentthreads;
                }
            }
        }

        /// <summary>
        /// Return all running items and all items queued for running.
        /// </summary>
        public int Count
        {
            get { return queue.Count + threadDictionary.Count; }
        }

        #endregion

        #region enque ops

        /// <summary>
        /// Adds the item to the queue to process asynchronously.
        /// </summary>
        /// <param name="item">the item to enqueue</param>
        public void EnQueue(T item)
        {
            EnQueue(item, defaultop);
        }

        /// <summary>
        /// Adds the item to the queue to process asynchronously and 
        /// uses the different operation instead  of the default.
        /// </summary>
        /// <param name="item">the item to enqueue</param>
        /// <param name="opp">the new operation that overrides the default</param>
        /// <exception cref="System.ArgumentNullException">opp is null</exception>
        public void EnQueue(T item, QueueOperationHandler<T> opp)
        {
            if (opp == null)
                throw new ArgumentNullException("opp", "operation can not be null");

            lock (syncobj)
            {
                if (qs == QueueState.Idle)
                {
                    #region idle

                    qs = QueueState.Running;
                    QueueStateChangedInternal(qs);

                    /// the problem with generics is that sometimes the fully 
                    /// descriptive name goes on for a while
                    KeyValuePair<T, QueueOperationHandler<T>> kvp =
                        new KeyValuePair<T, QueueOperationHandler<T>>(item, opp);

                    /// thread demands that its ParameterizedThreadStart take an object not a generic type
                    /// one might have resonably thought that there would be a generic constructor that 
                    /// took a strongly typed value but there is not one
                    currentthreads++;

                    ParameterizedThreadStart threadStart = RunOpp;
                    Thread thread = new Thread(threadStart);

                    //store reference for this thread
                    threadDictionary.Add(kvp, thread);

                    //trigger event : started proces for this item
                    ThreadStartedInternal(kvp.Key);

                    thread.Start(kvp);

                    #endregion
                }
                else if ((qs == QueueState.Paused) || (qs == QueueState.Pausing))
                {
                    #region pause

                    KeyValuePair<T, QueueOperationHandler<T>> kvp =
                        new KeyValuePair<T, QueueOperationHandler<T>>(item, opp);
                    if (queue.Contains(kvp))
                    {
                        ThreadErrorInternal(kvp.Key, new Exception("This item is in the queue already"));
                        return;
                    }
                    /// in the case that we are pausing or currently paused we just add the value to the
                    /// queue we dont try to run the process 
                    queue.Enqueue(kvp);

                    #endregion
                }
                else if (qs == QueueState.Running)
                {
                    #region running

                    KeyValuePair<T, QueueOperationHandler<T>> kvp =
                        new KeyValuePair<T, QueueOperationHandler<T>>(item, opp);
                    if ((threadDictionary.ContainsKey(kvp)) || queue.Contains(kvp))
                    {
                        ThreadErrorInternal(kvp.Key, new Exception("This item is in the queue already"));
                        return;
                    }

                    /// you have to enqueue the item then try to execute the first item in the process
                    /// always enqueue first as this ensures that you get the oldest item first since 
                    /// that is what you wanted to do you did not want a stack
                    queue.Enqueue(kvp);
                    TryNewThread();

                    #endregion
                }
                else if (qs == QueueState.Stopping)
                {
                    #region stopping

                    /// when you are stopping the queue i assume that you wanted to stop it not pause it this 
                    /// means that if you try to enqueue something it will throw an exception since you 
                    /// shouldnt be enqueueing anything since when the queue gets done all its current 
                    /// threads it clears the rest out so why bother enqueueing it. at this point we have 
                    /// a choice we can make the notifyer die or we can use the error event we already 
                    /// have built in to tell the sender. i chose the later. also try to pick an appropriate 
                    /// exception not just the base
                    ThreadErrorInternal(item, new ThreadStateException("the Queue is stopping . no processing done"));

                    #endregion
                }
            }
        }

        /// <summary>
        /// If the queue contains a specific item this method will return true.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Contains(T obj)
        {
            foreach (KeyValuePair<T, QueueOperationHandler<T>> kvp in threadDictionary.Keys)
            {
                if (kvp.Key.Equals(obj))
                {
                    return true;
                }
            }
            foreach (KeyValuePair<T, QueueOperationHandler<T>> kvp in queue)
            {
                if (kvp.Key.Equals(obj))
                {
                    return true;
                }
            }

            return false;
        }

        #region tools

        private void RunOpp(object o)
        {
            KeyValuePair<T, QueueOperationHandler<T>> kvp = (KeyValuePair<T, QueueOperationHandler<T>>) o;

            try
            {
                kvp.Value(kvp.Key);
                ThreadFinishedInternal(kvp);
            }
            catch (Exception ex)
            {
                ThreadErrorInternal(kvp.Key, new ThreadStateException("error processing. partial processing done.", ex));
            }
            finally
            {
                lock (syncobj)
                {
                    currentthreads--;
                }
                TryNewThread();
            }
        }

        private void TryNewThread()
        {
            lock (syncobj)
            {
                if (qs == QueueState.Running)
                {
                    #region Running

                    if (queue.Count != 0)
                    {
                        if (currentthreads < maxthreads)
                        {
                            currentthreads++;

                            //trigger event: started process for item
                            ThreadStartedInternal(queue.Peek().Key);
                            ParameterizedThreadStart threadStart = RunOpp;
                            Thread thread = new Thread(threadStart);
                            //store a reference for this thread in the dictionary
                            threadDictionary.Add(queue.Peek(), thread);
                            thread.Start(queue.Dequeue());
                        }
                    }
                    else
                    {
                        if (currentthreads == 0)
                        {
                            qs = QueueState.Idle;
                            QueueStateChangedInternal(qs);
                        }
                    }

                    #endregion
                }
                else if (qs == QueueState.Stopping || qs == QueueState.Aborting)
                {
                    #region stopping 

                    /// normally when we stop a queue we can just clear out the remaining 
                    /// values and let the threads peter out. however, we made the decision 
                    /// to throw an exception by way of our exception handler. it is therefore 
                    /// important to keep with that and get rid of all the queue items in 
                    /// that same way
                    while (queue.Count != 0)
                        ThreadErrorInternal(queue.Dequeue().Key,
                                            new ThreadStateException("the Queue is stopping . no processing done"));

                    /// all threads come through here so its up to us to single the change 
                    /// from stopping to idle
                    if (currentthreads == 0)
                    {
                        qs = QueueState.Idle;
                        QueueStateChangedInternal(qs);
                    }

                    #endregion
                }
                else if (qs == QueueState.Pausing)
                {
                    #region Pausing

                    if (currentthreads == 0)
                    {
                        qs = QueueState.Paused;
                        QueueStateChangedInternal(qs);
                    }

                    #endregion
                }
                else
                {
                    #region Idle / Paused

                    /// there should be no way to got in here while your idle or paused
                    /// this is just an error check
                    ThreadErrorInternal(default(T), new Exception("internal state bad"));

                    #endregion
                }
            }
        }

        #endregion

        #endregion

        #region event forwarders

        private void QueueStateChangedInternal(QueueState qs)
        {
            EventsHelper.UnsafeFire(QueueStateChanged, qs);
        }


        private void ThreadFinishedInternal(KeyValuePair<T, QueueOperationHandler<T>> finisheditem)
        {
            {
                threadDictionary.Remove(finisheditem);
            }
            EventsHelper.UnsafeFire(ThreadFinished, finisheditem.Key);
        }

        private void ThreadErrorInternal(T unfinisheditem, Exception ex)
        {
            lock (syncobj)
            {
                foreach (KeyValuePair<T, QueueOperationHandler<T>> item in threadDictionary.Keys)
                {
                    if (item.Key.Equals(unfinisheditem))
                    {
                        threadDictionary.Remove(item);
                        break;
                    }
                }
            }
            EventsHelper.UnsafeFire(ThreadError, unfinisheditem, ex);
        }

        private void ThreadStartedInternal(T startedItem)
        {
            EventsHelper.UnsafeFire(ThreadStarted, startedItem);
        }

        #endregion
    }
}