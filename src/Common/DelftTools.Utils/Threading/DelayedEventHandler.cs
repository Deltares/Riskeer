#define LOGGING_ENABLED

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using DelftTools.Utils.Collections;
using log4net;
using Timer = System.Timers.Timer;

namespace DelftTools.Utils.Threading
{
    /// <summary>
    /// Fires actions in a delayed way, in a separate thread. Action must contain logic which is thread-safe.
    /// 
    /// DelayedEventHandler watches DelayedEventHandlerController.FireEvents to see if events can be fired.
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    // TODO: refactor DelayedEventHandler to become ControlEventHandler
    public class DelayedEventHandler<TEventArgs> : IDisposable where TEventArgs : EventArgs
    {
        private static readonly ILog Log = LogManager.GetLogger("DelayedEventHandler<" + typeof(TEventArgs).Name + ">");
        
        private DateTime timeEndProcessingEvents;
        private bool enabled;
        private bool disposed;
        private int totalProcessedEvents; //for debugging

        private EventHandler<TEventArgs> handler;

        private readonly Queue<EventInfo> events = new Queue<EventInfo>();
        
        private readonly Timer timer;

        private readonly Stopwatch stopwatch = new Stopwatch();

        private int threadId;

        private class EventInfo
        {
            public object Sender;
            public TEventArgs Arguments;
        }

        public DelayedEventHandler(EventHandler<TEventArgs> handler)
        {
            threadId = Thread.CurrentThread.ManagedThreadId;

            this.handler = handler;

            DelayedEventHandlerController.FireEventsChanged += DelayedEventHandlerController_FireEventsChanged;
            FireLastEventOnly = true;
            Delay = 1;

            timer = new Timer { AutoReset = false };
            timer.Elapsed += TimerElapsed;

            Enabled = true;
        }

        public ISynchronizeInvoke SynchronizingObject
        {
            get { return timer.SynchronizingObject; }
            set { timer.SynchronizingObject = value; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;

                if (enabled && Delay == 0)
                {
                    if (FullRefreshEventHandler != null && SynchronizingObject != null)
                    {
                        SynchronizingObject.BeginInvoke((Action) (() => FullRefreshEventHandler(SynchronizingObject, EventArgs.Empty)), null);
                    }
                    return;
                }

                if (!enabled) // stopped
                {
                    lock (events)
                    {
                        events.Clear();
                        timer.Stop();
                        LogTimerStopped();
                    }
                }

                if (IsRunning)
                {
                    return;
                }

                lock (events)
                {
                    // start processing events
                    if (events.Count != 0)
                    {
                        // make sure that we don't call full refresh event handler too frequently if events are being processed already,
                        // this has to do with a duration of full event handler self.
                        //var interval = (DateTime.Now - timeEndProcessingEvents).TotalMilliseconds < FullRefreshDelay ? FullRefreshDelay : Delay;
                        IsRunning = true;
                        timer.Interval = Delay;
                        timer.Start();
                        LogTimerStarted();
                    }
                }
            }
        }

        /// <summary>
        /// When is set to true - only last events will be processed.
        /// </summary>
        [Obsolete("Use full refresh instead")]
        public bool FireLastEventOnly { get; set; }

        /// <summary>
        /// Time used as a criteria to schedule full refresh (if defined) or event handler.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// When multiple events are stacked and number of events exceeds this amount - full refresh is called and stack is cleared.
        /// Works only if FullRefreshEventHandler is not null.
        /// </summary>
        public int FullRefreshEventsCount { get; set; }

        /// <summary>
        /// If it took more than <see cref="FullRefreshTimeout"/> to process previous event
        /// </summary>
        public int FullRefreshTimeout { get; set; }

        /// <summary>
        /// Event handler used to to full refresh.
        /// </summary>
        public EventHandler FullRefreshEventHandler { get; set; }

        /// <summary>
        /// Fire only events where Filter is evaluated to true.
        /// </summary>
        public Func<object, TEventArgs, bool> Filter { get; set; }

        public bool IsRunning { get; private set; }

        public bool HasEventsToProcess
        {
            get
            {
                lock (events)
                {
                    var hasEvents = events.Count > 0;

                    if (hasEvents && !IsRunning && Enabled)
                    {
                        Log.Error("Events need to be processed but timer has been stopped."); // it is actually a bug - review logic
                        timer.Start();
                    }

                    return hasEvents;
                }
            }
        }

        public static implicit operator PropertyChangedEventHandler(DelayedEventHandler<TEventArgs> handler)
        {
            return handler.FirePropertyChangedEventHandler;
        }

        public static implicit operator NotifyCollectionChangedEventHandler(DelayedEventHandler<TEventArgs> handler)
        {
            return handler.FireCollectionChangedHandler;
        }

        public static implicit operator EventHandler<TEventArgs>(DelayedEventHandler<TEventArgs> handler)
        {
            return handler.FireHandler;
        }

        public void FireHandler(object sender, TEventArgs e)
        {
            if (Filter != null && !Filter(sender, e)) // filtered
            {
                return;
            }

            int timerDelay = Delay;

            if ((DateTime.Now - lastHandlerStartTime).TotalMilliseconds < Delay) // interval between events is too small
            {
                fireFullRefresh = true;
            }
            else if (lastHandlerElapsedMilliseconds > Delay) // check if last event handler took too long to run
            {
                fireFullRefresh = true;
                timerDelay = Delay * 5;
            }
            else if (Thread.CurrentThread.ManagedThreadId == threadId) // no delay or same thread - send immediately
            {
                IsRunning = true;
                try
                {
                    if (Enabled)
                    {
                        OnHandler(sender, e);
                    }
                }
                finally
                {
                    IsRunning = false;
                }
                return;
            }
            
            lock (events)
            {
                if (events.Count == 0)
                {
                    totalProcessedEvents = 1;
                }
                else
                {
                    totalProcessedEvents++;
                }

                // stop enqueuing when already above refresh count:
                if (FireLastEventOnly || events.Count <= FullRefreshEventsCount)
                {
                    events.Enqueue(new EventInfo {Sender = sender, Arguments = e});
                }
            }

            // schedule refresh
            if (IsRunning || !Enabled)
            {
                return;
            }

            IsRunning = true;

            timer.Interval = timerDelay;
            timer.Start();
            timer.Enabled = true;
            LogTimerStarted();
        }

        private double lastHandlerElapsedMilliseconds;
        private DateTime lastHandlerStartTime = DateTime.Now;

        protected virtual void OnHandler(object sender, TEventArgs e)
        {
            if(!enabled)
            {
                return;
            }

            lastHandlerStartTime = DateTime.Now;
            stopwatch.Reset();
            stopwatch.Start();

            handler(sender, e);

            stopwatch.Stop();
            lastHandlerElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        }

        void DelayedEventHandlerController_FireEventsChanged(object sender, EventArgs e)
        {
            // HACK: disable all delayed event handlers (static event!)
            Enabled = DelayedEventHandlerController.FireEvents;
        }

        private void FireCollectionChangedHandler(object sender, NotifyCollectionChangingEventArgs e)
        {
            FireHandler(sender, (TEventArgs)(EventArgs)e);
        }

        private void FirePropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            FireHandler(sender, (TEventArgs)(EventArgs)e);
        }

        private bool fireFullRefresh;

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (disposed)
            {
                return;
            }

            try
            {
                if (FireLastEventOnly)
                {
                    // remove all event args and use the only fire timer one more time with the latest one
                    
                    EventInfo eventInfo = null;
                    lock (events)
                    {
                        if (events.Count > 0)
                        {
                            LogBeforeFireLastEvent();
                            eventInfo = events.Last();
                            events.Clear();
                        }
                    }

                    if (eventInfo != null)
                    {
                        OnHandler(eventInfo.Sender, eventInfo.Arguments);
                        LogAfterFireLastEvent();
                    }
                }
                else
                {
                    // check if FullRefresh is configured
                    lock (events)
                    {
                        if (FullRefreshEventsCount != 0 && events.Count > FullRefreshEventsCount)
                        {
                            LogBeforeOnFullRefresh();
                            events.Clear();
                            fireFullRefresh = true;
                        }
                    }

                    if (fireFullRefresh)
                    {
                        try
                        {
                            FullRefreshEventHandler(this, null);
                            lastHandlerElapsedMilliseconds = 0;
                            LogAfterOnFullRefresh();
                        }
                        finally
                        {
                            fireFullRefresh = false;
                        }
                    }
                    else
                    {
                        EventInfo eventInfo = null;

                        LogProcessOneEvent();
                        lock (events)
                        {
                            if (events.Count > 0) // process event after event
                            {
                                eventInfo = events.Dequeue();
                            }
                        }
                        if (eventInfo != null)
                        {
                            InHandler = true;
                            OnHandler(eventInfo.Sender, eventInfo.Arguments);
                        }
                    }
                }
            }
            finally
            {
                InHandler = false;
                lock (events)
                {
                    if (events.Count != 0) // reschedule
                    {
                        IsRunning = true;

                        timer.Interval = Delay;
                        timer.Start();
                        LogTimerStarted();
                    }
                    else
                    {
                        timeEndProcessingEvents = DateTime.Now;
                        LogFinishedProcessing();
                        IsRunning = false;
                    }
                }
            }
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DelayedEventHandlerController.FireEventsChanged -= DelayedEventHandlerController_FireEventsChanged;
                    events.Clear();
                    IsRunning = false;
                    handler = null;

                    if (InHandler)
                    {
                        Console.WriteLine("Warning: hanging delayed event handler");
                    }

                    timer.Stop();
                    timer.Close();
                    timer.SynchronizingObject = null;
                    timer.Elapsed -= TimerElapsed;
                    LogTimerStopped();
                }
            }
            disposed = true;
        }
        
        ~DelayedEventHandler()
        {
            Dispose(true);
        }

        #endregion

        public bool InHandler { get; private set; }

        #region Logging

        private int eventsCount; //for logging
        
        [Conditional("TRACE")]
        private void LogFinishedProcessing()
        {
            Log.DebugFormat("Finished processing {0} events, target: {1} - {2}",
                            totalProcessedEvents, handler.Target.GetType().Name, handler.Method.Name);
        }
        
        [Conditional("TRACE")]
        private void LogProcessOneEvent()
        {
            Log.DebugFormat("Number of events in the queue is {0}, processing ... ", events.Count);
        }

        [Conditional("TRACE")]
        private void LogAfterOnFullRefresh()
        {
            stopwatch.Stop();
            Log.DebugFormat("Clearing {0} events and full refresh completed in {1} ms", eventsCount,
                            stopwatch.ElapsedMilliseconds);
        }

        [Conditional("TRACE")]
        private void LogBeforeOnFullRefresh()
        {
            Log.DebugFormat("Number of events exceeded {0}, clearing all events and calling full refresh",
                            FullRefreshEventsCount);
            stopwatch.Start();

            eventsCount = events.Count;
        }

        [Conditional("TRACE")]
        private void LogBeforeFireLastEvent()
        {
            Log.DebugFormat("Skipping {0} events and firing last event", events.Count);
            stopwatch.Start();
        }

        [Conditional("TRACE")]
        private void LogAfterFireLastEvent()
        {
            stopwatch.Stop();
            Log.DebugFormat("Firing event completed in {0} ms", stopwatch.ElapsedMilliseconds);
        }

        [Conditional("TRACE")]
        private void LogTimerStarted()
        {
            Log.DebugFormat("Started timer, event count: {0}", events.Count);
        }

        [Conditional("TRACE")]
        private void LogTimerStopped()
        {
            Log.DebugFormat("Stopped timer, event count: {0}", events.Count);
        }
        #endregion
    }
}