using System;

namespace DelftTools.Shell.Core.Workflow
{
    public class ActivityEventArgs:EventArgs
    {
        public IActivity Activity { get;private set; }
        public ActivityEventArgs(IActivity activity)
        {
            Activity = activity;
        }
    }
}