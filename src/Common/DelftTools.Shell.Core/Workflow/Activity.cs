using System;
using System.Collections.Generic;
using DelftTools.Utils.Collections.Generic;
using log4net;

namespace DelftTools.Shell.Core.Workflow
{
    public abstract class Activity : IActivity
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Activity));
        private string progressText;
        private ActivityStatus status;

        public virtual string Name { get; set; }

        public virtual IEventedList<IActivity> DependsOn { get; set; }

        public virtual ActivityStatus Status
        {
            get { return status; }
            protected set
            {
                if (value == status)
                    return;

                var oldStatus = status;
                status = value;

                if (StatusChanged != null)
                {
                    StatusChanged(this, new ActivityStatusChangedEventArgs(oldStatus, status));
                }
            }
        }

        public virtual string ProgressText
        {
            get { return progressText; }
        }

        protected Activity()
        {
            DependsOn = new EventedList<IActivity>();
        }

        public virtual void Initialize()
        {
            ChangeState(OnInitialize, ActivityStatus.Initializing, ActivityStatus.Initialized);
        }

        public virtual void Execute()
        {
            if (Status == ActivityStatus.Finished || Status == ActivityStatus.Cancelled || Status == ActivityStatus.Failed || Status == ActivityStatus.None)
            {
                throw new InvalidOperationException(string.Format("Activity is {0}, Initialize() must be called before Execute()", Status));
            }

            if (Status != ActivityStatus.Executed && Status != ActivityStatus.Initialized)
            {
                throw new InvalidOperationException(string.Format("Can't call Execute() for activity in {0} state.", Status));
            }

            try
            {
                Status = ActivityStatus.Executing;

                OnExecute();
                OnProgressChanged();

                if (Status == ActivityStatus.Failed ||
                    Status == ActivityStatus.Done ||
                    Status == ActivityStatus.Cancelled)
                {
                    // keep this status
                    return;
                }
            }
            catch (Exception e)
            {
                Status = ActivityStatus.Failed;
                log.Error(e.Message);
                return;
            }

            Status = ActivityStatus.Executed;
        }


        public virtual void Cancel()
        {
            ChangeState(OnCancel, ActivityStatus.Cancelling, ActivityStatus.Cancelled);
        }

        public virtual void Cleanup()
        {
            if (Status != ActivityStatus.Cancelled || Status != ActivityStatus.Failed)
            {
                ChangeState(OnCleanUp, ActivityStatus.Cleaning, ActivityStatus.Cleaned);
            }
            else
            {
                ChangeState(OnCleanUp, status, status);
            }
        }

        public virtual void Finish()
        {
            ChangeState(OnFinish, ActivityStatus.Finishing, ActivityStatus.Finished);
        }

        protected virtual void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, EventArgs.Empty);
            }
        }

        protected void SetProgressText(string progressText)
        {
            this.progressText = progressText;

            if(ProgressChanged != null)
            {
                ProgressChanged(this, null);
            }
        }

        /// <summary>
        /// Initializes internal state. Changes status to <see cref="ActivityStatus.Initialized"/>
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// Executes one step. Changes status to <see cref="ActivityStatus.Executed"/> or <see cref="ActivityStatus.Finished"/>.
        /// </summary>
        protected abstract void OnExecute();

        /// <summary>
        /// Cancels current run. Changes status to <see cref="ActivityStatus.Cancelled"/>.
        /// </summary>
        /// <returns>True when finished.</returns>
        protected abstract void OnCancel();

        /// <summary>
        /// Performs clean-up of all internal resources.
        /// </summary>
        /// <returns>True when finished.</returns>
        protected abstract void OnCleanUp();

        /// <summary>
        /// Activity has finished successfully.
        /// </summary>
        protected abstract void OnFinish();

        public virtual IEnumerable<object> GetDirectChildren()
        {
            throw new NotImplementedException();
        }

        public virtual object DeepClone()
        {
            throw new NotImplementedException();
        }

        public virtual event EventHandler<ActivityStatusChangedEventArgs> StatusChanged;

        public event EventHandler ProgressChanged;

        private void ChangeState(Action transitionAction, ActivityStatus statusBefore, ActivityStatus statusAfter)
        {
            try
            {
                Status = statusBefore;
                transitionAction();

                if (Status == ActivityStatus.Failed)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Status = ActivityStatus.Failed;
                log.Error(e.Message);
                return;
            }

            Status = statusAfter;
        }
    }
}