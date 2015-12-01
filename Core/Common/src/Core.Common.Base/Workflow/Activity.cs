using System;
using System.Collections.Generic;
using Core.Common.Base.Properties;
using Core.Common.Utils.Collections.Generic;
using log4net;

namespace Core.Common.Base.Workflow
{
    public abstract class Activity : IActivity
    {
        public virtual event EventHandler<ActivityStatusChangedEventArgs> StatusChanged;

        public string Log { get; set; }

        public event EventHandler ProgressChanged;
        private static readonly ILog log = LogManager.GetLogger(typeof(Activity));
        private string progressText;
        private ActivityStatus status;

        protected Activity()
        {
            Log = "";
            DependsOn = new EventedList<IActivity>();
        }

        public virtual string Name { get; set; }

        public virtual IEventedList<IActivity> DependsOn { get; set; }

        public virtual ActivityStatus Status
        {
            get
            {
                return status;
            }
            protected set
            {
                if (value == status)
                {
                    return;
                }

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
            get
            {
                return progressText;
            }
        }

        public virtual IEnumerable<object> GetDirectChildren()
        {
            throw new NotImplementedException();
        }

        public virtual void Initialize()
        {
            ChangeState(OnInitialize, ActivityStatus.Initializing, ActivityStatus.Initialized);
        }

        public virtual void Execute()
        {
            if (Status == ActivityStatus.Finished || Status == ActivityStatus.Cancelled || Status == ActivityStatus.Failed || Status == ActivityStatus.None)
            {
                throw new InvalidOperationException(string.Format(Resources.Activity_Execute_Activity_is_0_Initialize_must_be_called_before_Execute, Status));
            }

            if (Status != ActivityStatus.Executed && Status != ActivityStatus.Initialized)
            {
                throw new InvalidOperationException(string.Format(Resources.Activity_Execute_Can_t_call_Execute_for_activity_in_0_state, Status));
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

        public virtual object DeepClone()
        {
            throw new NotImplementedException();
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

            if (ProgressChanged != null)
            {
                ProgressChanged(this, null);
            }
        }

        /// <summary>
        /// Initializes internal state to prepare for execution. After calling this method,
        /// <see cref="Status"/> will be set to <see cref="ActivityStatus.Initialized"/> if
        /// no error has occurred.
        /// </summary>
        /// <remarks>Set <see cref="Status"/> to <see cref="ActivityStatus.Failed"/>
        /// when an error has occurred while performing the initialization.</remarks>
        protected abstract void OnInitialize();

        /// <summary>
        /// Executes one step. This method will be called multiple times to allow for multiple
        /// execution steps. After calling this method, <see cref="Status"/> will be set to 
        /// <see cref="ActivityStatus.Executed"/> if no error has occurred.
        /// </summary>
        /// <remarks>
        /// <para>Set <see cref="Status"/> to <see cref="ActivityStatus.Failed"/>
        /// when an error has occurred while executing.</para>
        /// <para>Set <see cref="Status"/> to <see cref="ActivityStatus.Done"/>
        /// when execution has finished without errors.</para>
        /// </remarks>
        protected abstract void OnExecute();

        /// <summary>
        /// Activity has finished successfully. After calling this method, <see cref="Status"/> 
        /// will be set to <see cref="ActivityStatus.Finished"/> if no error has occurred.
        /// </summary>
        protected abstract void OnFinish();

        /// <summary>
        /// Cancels current run. <see cref="Status"/> will change to <see cref="ActivityStatus.Cancelled"/>
        /// after this method has been called.
        /// </summary>
        protected abstract void OnCancel();

        /// <summary>
        /// Performs clean-up of all internal resources.
        /// </summary>
        protected abstract void OnCleanUp();

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