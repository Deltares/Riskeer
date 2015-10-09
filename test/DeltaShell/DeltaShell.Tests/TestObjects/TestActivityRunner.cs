using System;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Utils.Collections.Generic;

namespace DeltaShell.Tests.TestObjects
{
    public class TestActivityRunner : IActivityRunner
    {
        public event EventHandler<ActivityEventArgs> ActivityCompleted;
        public event EventHandler IsRunningChanged;
        public event EventHandler ActivitiesCollectionChanged;
        public event EventHandler<ActivityStatusChangedEventArgs> ActivityStatusChanged;

        public IEventedList<IActivity> Activities
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void RunActivity(IActivity activity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fire activity status changed for the sender
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newStatus"></param>
        public void FireActivityStatusChanged(IActivity sender, ActivityStatus newStatus)
        {
            if (ActivityStatusChanged != null)
            {
                ActivityStatusChanged(sender, new ActivityStatusChangedEventArgs(ActivityStatus.None, newStatus));
            }
        }

        public bool IsRunningActivity(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public void Enqueue(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public void Cancel(IActivity activity)
        {
            throw new NotImplementedException();
        }

        public void CancelAll()
        {
            throw new NotImplementedException();
        }
    }
}