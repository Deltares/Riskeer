using System;
using System.Collections.Generic;
using Core.Common.Base.Properties;
using log4net;

namespace Core.Common.Base.Service
{
    /// <summary>
    /// Abstract class that can be used for performing activies (like calculations, data imports, data exports, etc.).
    /// The regular workflow for completely performing an <see cref="Activity"/> is: <see cref="Execute"/> -> <see cref="Finish"/>.
    /// <see cref="Cancel"/> can be called for cancelling a running <see cref="Activity"/>.
    /// </summary>
    public abstract class Activity
    {
        public event EventHandler ProgressChanged;

        private readonly ILog log = LogManager.GetLogger(typeof(Activity));
        private string progressText;

        /// <summary>
        /// Constructs a new <see cref="Activity"/>.
        /// </summary>
        protected Activity()
        {
            LogMessages = new List<string>();
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="Activity"/>.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ActivityStatus"/> of the <see cref="Activity"/>.
        /// </summary>
        public ActivityStatus Status { get; protected set; }

        /// <summary>
        /// Gets or sets the progress text of the <see cref="Activity"/>.
        /// <see cref="ProgressChanged"/> listeners are notified when the progress text is set.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the collection of log messages of the <see cref="Activity"/> (which is appended while performing the <see cref="Activity"/>).
        /// </summary>
        public IList<string> LogMessages { get; private set; }

        /// <summary>
        /// This method runs the <see cref="Activity"/> by sequentially making calls to <see cref="Initialize"/> and <see cref="Execute"/>.
        /// </summary>
        public void Run()
        {
            Initialize();

            if (Status == ActivityStatus.Failed)
            {
                log.ErrorFormat(Resources.Activity_Run_Initialization_of_0_has_failed, Name);
                return;
            }

            if (Status == ActivityStatus.Cancelled)
            {
                log.WarnFormat(Resources.Activity_Run_Execution_of_0_has_been_canceled, Name);
                return;
            }

            Execute();

            if (Status == ActivityStatus.Failed)
            {
                log.ErrorFormat(Resources.Activity_Run_Execution_of_0_has_failed, Name);
            }
        }

        /// <summary>
        /// This method cancels a running <see cref="Activity"/>.
        /// </summary>
        public void Cancel()
        {
            ChangeState(OnCancel, ActivityStatus.Cancelling, ActivityStatus.Cancelled);
        }

        /// <summary>
        /// This method finishes an <see cref="Activity"/> that successfully ran.
        /// Successfully ran activities can be identified by a <see cref="Status"/> not equal to <see cref="ActivityStatus.Failed"/> or <see cref="ActivityStatus.Cancelled"/>.
        /// </summary>
        public void Finish()
        {
            if (Status != ActivityStatus.Failed && Status != ActivityStatus.Cancelled)
            {
                ChangeState(OnFinish, ActivityStatus.Finishing, ActivityStatus.Finished);
            }
        }

        /// <summary>
        /// This method initializes an <see cref="Activity"/>.
        /// </summary>
        protected void Initialize()
        {
            ChangeState(OnInitialize, ActivityStatus.Initializing, ActivityStatus.Initialized);
        }

        /// <summary>
        /// This method executes an <see cref="Activity"/> that successfully initialized.
        /// Successfully ran activities can be identified by a <see cref="Status"/> not equal to <see cref="ActivityStatus.Failed"/> or <see cref="ActivityStatus.Cancelled"/>.
        /// </summary>
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

        private void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, EventArgs.Empty);
            }
        }

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