using System;
using System.Collections.Generic;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using log4net;

namespace DelftTools.Shell.Core.Workflow
{
    public abstract class CompositeActivity : Activity, ICompositeActivity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CompositeActivity));

        public CompositeActivity()
        {
            Activities = new EventedList<IActivity>();
        }

        public virtual IEventedList<IActivity> Activities { get; set; }

        public bool ReadOnly { get; set; }

        [Aggregation]
        public virtual ICompositeActivity CurrentWorkflow
        {
            get
            {
                return this;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override IEnumerable<object> GetDirectChildren()
        {
            foreach (var activity in Activities)
            {
                yield return activity;
            }
        }

        public virtual object DeepClone()
        {
            var clone = (ICompositeActivity) Activator.CreateInstance(GetType());

            clone.Name = Name;

            foreach (var activity in Activities)
            {
                clone.Activities.Add((IActivity) activity.DeepClone());
            }

            return clone;
        }

        protected override void OnCleanUp()
        {
            foreach (var activity in Activities)
            {
                activity.Cleanup();
            }
        }

        protected override void OnCancel()
        {
            foreach (var activity in Activities)
            {
                if (activity.Status != ActivityStatus.Initialized && activity.Status != ActivityStatus.Executed && activity.Status != ActivityStatus.WaitingForData)
                {
                    continue; // finished or failed
                }

                activity.Cancel();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception("Activity failed: " + activity);
                }
            }
        }

        protected override void OnFinish()
        {
            foreach (var activity in Activities)
            {
                activity.Finish();

                if (activity.Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format("The finishing of activity {0} failed", activity));
                }
            }
        }
    }
}