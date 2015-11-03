using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Common.Base.Workflow;
using log4net.Appender;
using log4net.Core;

namespace Core.Common.Base
{
    /// <summary>
    /// Class add logmessage to models when they are 'done'.
    /// Class works by catch logAppender message durring running states of activity
    /// Running state is a when the activity is active : 
    /// Initializing, Executing, Cancelling
    /// </summary>
    public class RunningActivityLogAppender : IAppender
    {
        private readonly IList<ActivityStatus> doneStates = new[]
        {
            ActivityStatus.Cleaned,
            ActivityStatus.Failed,
            ActivityStatus.Cancelled
        };

        private readonly IList<ActivityStatus> runningStates = new[]
        {
            ActivityStatus.Executing,
            ActivityStatus.Initializing,
            ActivityStatus.Cancelling,
            ActivityStatus.Finishing,
            ActivityStatus.Cleaning
        };

        /// shouldnt it be a dictionary of stringbuilders?
        private readonly IDictionary<IActivity, IList<string>> activityLogs;

        // this is ThreadLocal as a hack to ensure we can keep logfiles from several models (eg. threads) seperate.
        private readonly ThreadLocal<HashSet<IActivity>> runningActivities = new ThreadLocal<HashSet<IActivity>>(() => new HashSet<IActivity>());

        private readonly object runningActivitiesLock = new object();
        private readonly object activityLogsLock = new object();

        private IActivityRunner activityRunner;

        //THIS constructor is used by Log4Net instantiation..
        //to make the appender reachable from the app we use a singleton construction
        public RunningActivityLogAppender()
        {
            activityLogs = new Dictionary<IActivity, IList<string>>();
            Instance = this;
        }

        public static RunningActivityLogAppender Instance { get; set; }

        public IActivityRunner ActivityRunner
        {
            get
            {
                return activityRunner;
            }
            set
            {
                if (activityRunner != null)
                {
                    activityRunner.ActivityStatusChanged -= ActivityRunnerActivityStatusChanged;
                }
                activityRunner = value;
                if (activityRunner != null)
                {
                    activityRunner.ActivityStatusChanged += ActivityRunnerActivityStatusChanged;
                }
            }
        }

        public string Name { get; set; }

        public void Close() {}

        public void DoAppend(LoggingEvent loggingEvent)
        {
            //loggingEvent.RenderedMessage;
            foreach (var act in GetRunningActivitiesThreadSafe())
            {
                var message = "[" + loggingEvent.TimeStamp.ToString("HH:mm:ss") + "]:" + loggingEvent.RenderedMessage + Environment.NewLine;

                using (new TryLock(activityLogsLock))
                {
                    if (!activityLogs.ContainsKey(act))
                    {
                        activityLogs[act] = new List<string>();
                    }
                    activityLogs[act].Add(message);
                }
            }
        }

        private void ActivityRunnerActivityStatusChanged(object sender, ActivityStatusChangedEventArgs e)
        {
            //if an activity goes into a running state i want to know it...
            if (runningStates.Contains(e.NewStatus))
            {
                using (new TryLock(runningActivitiesLock))
                {
                    runningActivities.Value.Add((IActivity) sender);
                }
            }
            //it is done...commit the log
            else if (doneStates.Contains(e.NewStatus))
            {
                var activity = (IActivity) sender;
                SendLogToActivity(activity);

                using (new TryLock(runningActivitiesLock))
                {
                    runningActivities.Value.Remove(activity);
                }
                using (new TryLock(activityLogsLock))
                {
                    activityLogs.Remove(activity); // DON'T forget to remove, otherwise memory leak!
                }
            }
            //it changed state into a non running state...suspend logging
            else
            {
                var activity = (IActivity) sender;
                using (new TryLock(runningActivitiesLock))
                {
                    runningActivities.Value.Remove(activity);
                }
            }

            //TODO: when do we commit the log the activity?
        }

        private void SendLogToActivity(IActivity activity)
        {
            if (!GetActivityLogActivitiesThreadSafe().Contains(activity))
            {
                return;
            }

            using (new TryLock(activityLogsLock))
            {
                // TODO: Add log item and/or append log text
            }
        }

        private ICollection<IActivity> GetActivityLogActivitiesThreadSafe()
        {
            using (new TryLock(activityLogsLock))
            {
                return activityLogs.Keys.ToList();
            }
        }

        private IEnumerable<IActivity> GetRunningActivitiesThreadSafe()
        {
            using (new TryLock(runningActivitiesLock))
            {
                return runningActivities.Value.ToList();
            }
        }
    }
}