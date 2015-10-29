using System;
using System.Linq;

namespace Core.Common.BaseDelftTools.Workflow
{
    public class ParallelActivity : CompositeActivity
    {
        protected override void OnInitialize()
        {
            foreach (var activity in Activities)
            {
                activity.Initialize();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("The initialization of activity {0} failed", activity));
                }
            }
        }

        protected override void OnExecute()
        {
            foreach (var activity in Activities)
            {
                if (activity.Status == ActivityStatus.Done)
                {
                    continue;
                }

                activity.Execute();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("The execution of activity {0} failed", activity));
                }
            }

            if (Activities.All(a => a.Status == ActivityStatus.Done))
            {
                Status = ActivityStatus.Done;
            }
        }
    }
}