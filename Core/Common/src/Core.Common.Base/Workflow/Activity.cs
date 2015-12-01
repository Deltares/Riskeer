using System;
using Core.Common.Base.Properties;
using log4net;

namespace Core.Common.Base.Workflow
{
    public abstract class Activity : IActivity
    {
        public event EventHandler ProgressChanged;
        private static readonly ILog log = LogManager.GetLogger(typeof(Activity));
        private string progressText;

        protected Activity()
        {
            Log = "";
        }

        public string Log { get; set; }

        public virtual string Name { get; set; }

        public virtual ActivityStatus Status { get; protected set; }

        public virtual string ProgressText
        {
            get
            {
                return progressText;
            }
            set
            {
                progressText = value;

                if (ProgressChanged != null)
                {
                    ProgressChanged(this, null);
                }
            }
        }

        public void Run()
        {
            try
            {
                Initialize();

                if (Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Initialization_of_0_has_failed, Name));
                }

                while (Status != ActivityStatus.Done)
                {
                    if (Status == ActivityStatus.Cancelled)
                    {
                        log.WarnFormat(Resources.ActivityRunner_RunActivity_Execution_of_0_has_been_canceled, Name);
                        break;
                    }

                    if (Status != ActivityStatus.WaitingForData)
                    {
                        Execute();
                    }

                    if (Status == ActivityStatus.Failed)
                    {
                        throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Execution_of_0_has_failed, Name));
                    }
                }

                if (Status != ActivityStatus.Cancelled)
                {
                    Finish();

                    if (Status == ActivityStatus.Failed)
                    {
                        throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Finishing_of_0_has_failed, Name));
                    }
                }

                Cleanup();

                if (Status == ActivityStatus.Failed)
                {
                    throw new Exception(string.Format(Resources.ActivityRunner_RunActivity_Clean_up_of_0_has_failed, Name));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message); //for build server debugging
                log.Error(exception.Message);
            }
            finally
            {
                try
                {
                    if (Status != ActivityStatus.Cleaned)
                    {
                        Cleanup();
                    }
                }
                catch (Exception)
                {
                    log.ErrorFormat(Resources.ActivityRunner_RunActivity_Clean_up_of_0_has_failed, Name);
                }
            }
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
                ChangeState(OnCleanUp, Status, Status);
            }
        }

        public virtual void Finish()
        {
            ChangeState(OnFinish, ActivityStatus.Finishing, ActivityStatus.Finished);
        }

        protected void OnProgressChanged()
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