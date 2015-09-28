using System;
using DelftTools.Utils.Collections;

namespace DelftTools.Shell.Core.Workflow
{
    public class SequentialActivity : CompositeActivity
    {
        private IActivity currentActivity;
        private int currentActivityIndex;
        private bool currentActivityInitialized;

        protected override void OnInitialize()
        {
            currentActivityIndex = 0;
            currentActivity = Activities[0];
            currentActivityInitialized = false;

            Activities.ForEach(a =>
            {
                if (a is Activity)
                {
                    typeof(Activity).GetProperty("Status").SetValue(a, ActivityStatus.None, null);
                }
            });
        }

        protected override void OnExecute()
        {
            if (currentActivity == null)
            {
                return;
            }

            if (!currentActivityInitialized)
            {
                currentActivity.Initialize();

                if (currentActivity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("The initialization of activity {0} failed", currentActivity));
                }
                currentActivityInitialized = true;
            }

            if (currentActivity.Status != ActivityStatus.Done)
            {
                currentActivity.Execute();

                if (currentActivity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("The execution of activity {0} failed", currentActivity));
                }
            }
                
            if(currentActivity.Status == ActivityStatus.Done) // take the next activity
            {
                currentActivity.Finish();

                if (currentActivity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("The finishing of activity {0} failed", currentActivity));
                }

                if (++currentActivityIndex < Activities.Count)
                {
                    currentActivity = Activities[currentActivityIndex];
                    currentActivityInitialized = false;
                }
                else
                {
                    currentActivity = null; // all activities are finished
                    currentActivityIndex = -1;
                    Status = ActivityStatus.Done;
                }
            }
        }

        protected override void OnFinish()
        {
            //already done.
        }
    }
}