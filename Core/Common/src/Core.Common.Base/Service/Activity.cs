using System;
using Core.Common.Base.Properties;
using log4net;

namespace Core.Common.Base.Service
{
    /// <summary>
    /// Defines basic activity which can be executed as part of the workflow.
    /// </summary>
    /// <example>
    /// <para>Regular workflow for an activity is: Initialize -> Execute -> Finish -> Cleanup.</para>
    /// <para>Regular workflow for an activity with an error occurring: [Initialize/Execute/Finish] -> ! Exception / Error ! -> Cleanup.</para> 
    /// <para>Regular workflow for an activity being cancelled: [Initialize/Execute/Finish] -> ! Cancel ! -> Cleanup.</para>
    /// </example>
    public abstract class Activity
    {
        public event EventHandler ProgressChanged;
        private readonly ILog log = LogManager.GetLogger(typeof(Activity));
        private string progressText;

        protected Activity()
        {
            Log = "";
        }

        public string Log { get; set; }

        public virtual string Name { get; set; }

        public ActivityStatus Status { get; protected set; }

        public string ProgressText
        {
            get
            {
                return progressText;
            }
            protected set
            {
                progressText = value;

                OnProgressChanged();
            }
        }

        public void Run()
        {
            try
            {
                Initialize();

                if (Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format(Resources.Activity_Run_Initialization_of_0_has_failed, Name));
                }

                if (Status == ActivityStatus.Cancelled)
                {
                    log.WarnFormat(Resources.Activity_Run_Execution_of_0_has_been_canceled, Name);
                    return;
                }

                Execute();

                if (Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format(Resources.Activity_Run_Execution_of_0_has_failed, Name));
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.Message);
            }
        }

        public void Cancel()
        {
            ChangeState(OnCancel, ActivityStatus.Cancelling, ActivityStatus.Cancelled);
        }

        public void Finish()
        {
            if (Status != ActivityStatus.Failed && Status != ActivityStatus.Cancelled)
            {
                ChangeState(OnFinish, ActivityStatus.Finishing, ActivityStatus.Finished);
            }
        }

        protected void Initialize()
        {
            ChangeState(OnInitialize, ActivityStatus.Initializing, ActivityStatus.Initialized);
        }

        protected void Execute()
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

                if (Status == ActivityStatus.Failed ||
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

        private void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, EventArgs.Empty);
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