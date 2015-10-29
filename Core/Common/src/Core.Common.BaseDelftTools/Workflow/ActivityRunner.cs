using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using log4net;

namespace Core.Common.BaseDelftTools.Workflow
{
    /// <summary>
    /// Class fires activities asynch and generates state changed
    /// </summary>
    //TODO: increase coverage and simplity this class. It is on the verge of unmanagable
    //TODO: make this stuff (lists) threadsafe damnit!
    public class ActivityRunner : IActivityRunner
    {
        public event EventHandler IsRunningChanged;

        public event EventHandler ActivitiesCollectionChanged;

        private const int maxRunningTaskCount = 1;
        private static readonly ILog log = LogManager.GetLogger(typeof(ActivityRunner));

        private readonly IList<AsyncActivityRunner> runningTasks = new List<AsyncActivityRunner>();
        private readonly IList<AsyncActivityRunner> todoTasks = new List<AsyncActivityRunner>();
        private readonly IEventedList<IActivity> activities;

        private bool running; // synchronization flag

        private bool wasRunning;

        public ActivityRunner()
        {
            activities = new EventedList<IActivity>();
            activities.CollectionChanged += HandleActivitiesCollectionChanged;
        }

        /// <summary>
        /// Provides an evented summary of the current activities (running and todo). 
        /// DO NOT ADD TO THIS LIST useEnqueue instead
        /// </summary>
        public IEventedList<IActivity> Activities
        {
            get
            {
                return activities;
            }
        }

        private void HandleActivitiesCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            //only local changes...get this in the EventedList...
            if (sender != activities)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangeAction.Add)
            {
                ((IActivity) e.Item).StatusChanged += HandleActivityStatusChanged;
            }
            else if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                ((IActivity) e.Item).StatusChanged -= HandleActivityStatusChanged;
            }
        }

        private void HandleActivityStatusChanged(object sender, ActivityStatusChangedEventArgs e)
        {
            if (e.NewStatus == ActivityStatus.Cancelled)
            {
                var task = GetRunningTasksThreadSafe().First(t => Equals(t.Activity, sender));

                if (ActivityStatusChanged != null)
                {
                    //bubble the activity status change..
                    ActivityStatusChanged(sender, e);
                }

                running = true;

                //done running,
                using (new TryLock(runningTasks))
                {
                    runningTasks.Remove(task);
                    CleanUp(task);
                    activities.Remove(task.Activity);
                }

                OnIsRunningChanged();

                return;
            }

            if (ActivityStatusChanged != null)
            {
                //bubble the activity status change..
                ActivityStatusChanged(sender, e);
            }

            OnIsRunningChanged();
        }

        private void StartTaskIfPossible(Action beforeActualRun = null)
        {
            //we can run if we are not busy and have something todo ;)
            while (true)
            {
                AsyncActivityRunner taskToRun;

                using (new TryLock(runningTasks))
                {
                    if (runningTasks.Count >= maxRunningTaskCount || (todoTasks.Count <= 0))
                    {
                        break;
                    }

                    taskToRun = todoTasks[0];
                    runningTasks.Add(taskToRun);
                    todoTasks.RemoveAt(0);
                }

                Debug.WriteLine("Run activity {0}", (taskToRun.Activity.Name));

                if (beforeActualRun != null)
                {
                    beforeActualRun();
                }
                taskToRun.Run();
                //'pop' the first task (FIFO)
            }
        }

        private void Completed(object sender, EventArgs e)
        {
            var task = (AsyncActivityRunner) sender;

            Debug.WriteLine(task.Activity.Status == ActivityStatus.Cancelled
                ? string.Format("Cancelled activity {0}", task.Activity.Name)
                : string.Format("Finished activity {0}", task.Activity.Name));

            try
            {
                OnTaskCompleted(task);
            }
            finally
            {
                running = true;

                using (new TryLock(runningTasks))
                {
                    runningTasks.Remove(task);
                    CleanUp(task);
                }

                // continue with the next activity            
                StartTaskIfPossible();

                OnIsRunningChanged();
            }
        }

        private void CleanUp(AsyncActivityRunner task)
        {
            task.Completed -= Completed;
            activities.Remove(task.Activity);
        }

        private void OnIsRunningChanged()
        {
            running = false;

            var isRunning = IsRunning;
            if (wasRunning != isRunning)
            {
                //TODO: get some logic to determine whether it really changed. (P.Changed?)
                if (IsRunningChanged != null)
                {
                    IsRunningChanged(this, EventArgs.Empty);
                }
            }
            wasRunning = isRunning;
        }

        private void OnTaskCompleted(AsyncActivityRunner sender)
        {
            if (!sender.CompletedSuccesfully)
            {
                log.Error(String.IsNullOrEmpty(sender.Activity.Name) 
                    ? "An error occured while running a background activity: " 
                    : String.Format("An error occured while running activity {0}: ", sender.Activity.Name), sender.Exception);
            }

            if (ActivityCompleted != null)
            {
                ActivityCompleted(this, new ActivityEventArgs(sender.Activity));
            }

            OnIsRunningChanged();
        }

        #region IActivityRunner Members

        public bool IsRunning
        {
            get
            {
                using (new TryLock(runningTasks)) //prevent deadlock
                {
                    return runningTasks.Count > 0 || activities.Count > 0 || running;
                }
            }
        }

        public bool IsRunningActivity(IActivity activity)
        {
            if (activity == null)
            {
                return false;
            }

            return GetRunningTasksThreadSafe().Any(task => task.Activity == activity) && !running;
        }

        private IEnumerable<AsyncActivityRunner> GetRunningTasksThreadSafe()
        {
            using (new TryLock(runningTasks)) // lock modifications
            {
                return runningTasks.ToList(); // return a local copy
            }
        }

        public void Enqueue(IActivity activity)
        {
            var task = new AsyncActivityRunner(activity, RunActivity);
            task.Completed += Completed;

            using (new TryLock(runningTasks))
            {
                todoTasks.Add(task);
                activities.Add(activity);
            }

            Debug.WriteLine(string.Format("Enqueued activity {0}", activity.Name));

            //TODO: it might already be running so running would not be changed.
            //fix and review
            StartTaskIfPossible(OnIsRunningChanged);
        }

        public static void RunActivity(IActivity activity)
        {
            try
            {
                activity.Initialize();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("Initialization of {0} has failed.", activity.Name));
                }

                while (activity.Status != ActivityStatus.Done)
                {
                    if (activity.Status == ActivityStatus.Cancelled)
                    {
                        log.WarnFormat("Execution of {0} has been canceled.", activity.Name);
                        break;
                    }

                    if (activity.Status != ActivityStatus.WaitingForData)
                    {
                        activity.Execute();
                    }

                    if (activity.Status == ActivityStatus.Failed)
                    {
                        throw new Exception(string.Format("Execution of {0} has failed.", activity.Name));
                    }
                }

                if (activity.Status != ActivityStatus.Cancelled)
                {
                    activity.Finish();

                    if (activity.Status == ActivityStatus.Failed)
                    {
                        throw new Exception(string.Format("Finishing of {0} has failed.", activity.Name));
                    }
                }

                activity.Cleanup();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("Clean up of {0} has failed.", activity.Name));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message); //for build server debugging
                log.Error(exception.Message);
            }
            finally
            {
                try
                {
                    if (activity.Status != ActivityStatus.Cleaned)
                    {
                        activity.Cleanup();
                    }
                }
                catch (Exception)
                {
                    log.ErrorFormat("Clean up of {0} has failed.", activity.Name);
                }
            }
        }

        public void Cancel(IActivity activity)
        {
            var task = GetRunningTasksThreadSafe().FirstOrDefault(t => t.Activity == activity);

            if (task != null)
            {
                //TODO: let the task cancel and complete.cleanup should be in Completed
                task.Cancel();
                running = true;
                return;
            }

            //or remove it from todo
            using (new TryLock(runningTasks))
            {
                task = todoTasks.ToList().FirstOrDefault(t => t.Activity == activity);
                if (task != null)
                {
                    todoTasks.Remove(task);
                    CleanUp(task);
                }
            }

            OnIsRunningChanged();
        }

        //TODO: make cancelAll use cancel for each activity.
        public void CancelAll()
        {
            foreach (var task in GetRunningTasksThreadSafe())
            {
                running = true;
                task.Cancel();
            }

            //empty the todo on a cancel
            using (new TryLock(runningTasks))
            {
                var currentTodoTasks = todoTasks.ToList();
                foreach (var task in currentTodoTasks)
                {
                    CleanUp(task);

                    if (activities.Contains(task.Activity))
                    {
                        activities.Remove(task.Activity);
                    }
                }

                todoTasks.Clear();
            }

            OnIsRunningChanged();
        }

        public event EventHandler<ActivityEventArgs> ActivityCompleted;

        public event EventHandler<ActivityStatusChangedEventArgs> ActivityStatusChanged;

        #endregion
    }

    /// <summary>
    /// Poor man's locking mechanism under danger of deadlock due to invokes &amp; events :-(
    /// </summary>
    public class TryLock : IDisposable
    {
        private readonly bool hasLock;
        private object lockObject;

        public TryLock(object lockObject, int timeOut = 100)
        {
            this.lockObject = lockObject;
            hasLock = Monitor.TryEnter(lockObject, timeOut);
        }

        public void Dispose()
        {
            if (hasLock)
            {
                Monitor.Exit(lockObject);
            }
            lockObject = null;
        }
    }
}