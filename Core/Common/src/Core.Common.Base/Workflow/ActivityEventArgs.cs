using System;

namespace Core.Common.Base.Workflow
{
    public class ActivityEventArgs : EventArgs
    {
        public ActivityEventArgs(IActivity activity)
        {
            Activity = activity;
        }

        public IActivity Activity { get; private set; }
    }
}