using System;

namespace DelftTools.Shell.Core.Workflow
{
    public class ActivityStatusChangedEventArgs : EventArgs
    {
        public ActivityStatusChangedEventArgs(ActivityStatus oldStatus, ActivityStatus newStatus)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public ActivityStatus OldStatus { get; private set; }
        public ActivityStatus NewStatus { get; private set; }
    }
}