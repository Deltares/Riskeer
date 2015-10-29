using System;

namespace Core.Common.BaseDelftTools.Workflow
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