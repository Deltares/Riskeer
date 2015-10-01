using System;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Utils.Aop;

namespace DelftTools.Shell.Core.Workflow
{
    /// <summary>
    /// Used to wrap existing activities which are defined somewhere else, propagates all calls to the underlying acitivity.
    /// </summary>
    public class ActivityWrapper : Activity
    {
        private IActivity activity;

        public ActivityWrapper(IActivity activity)
        {
            this.activity = activity;
        }

        public ActivityWrapper()
        {
        }

        /// <summary>
        /// Activity being wrapped as an aggregation.
        /// </summary>
        [Aggregation]
        public IActivity Activity
        {
            get { return activity; }
            set
            {
                if (activity != null)
                {
                    activity.StatusChanged -= ActivityOnStatusChanged;
                }

                activity = value;

                if (activity != null)
                {
                    activity.StatusChanged += ActivityOnStatusChanged;
                }
            }
        }

        public override string Name
        {
            get { return Activity.Name; }
            set { Activity.Name = value; }
        }

        public override ActivityStatus Status
        {
            get { return Activity.Status; }
        }

        protected void ActivityOnStatusChanged(object sender, ActivityStatusChangedEventArgs e)
        {
            Status = e.NewStatus;
        }

        public override void Initialize()
        {
            Activity.Initialize();
        }

        public override void Execute()
        {
            Activity.Execute();
        }

        public override void Cancel()
        {
            Activity.Cancel();
        }

        public override void Cleanup()
        {
            Activity.Cleanup();
        }

        public override void Finish()
        {
            Activity.Finish();
        }

        protected override void OnInitialize()
        {
            throw new NotImplementedException();
        }

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnCancel()
        {
            throw new NotImplementedException();
        }

        protected override void OnCleanUp()
        {
            throw new NotImplementedException();
        }

        protected override void OnFinish()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Activity.ToString();
        }

        public override IEnumerable<object> GetDirectChildren()
        {
            return Enumerable.Empty<object>();
        }

        public override object DeepClone()
        {
            return new ActivityWrapper { Activity = Activity };
        }
    }
}