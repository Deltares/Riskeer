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
        /// This method runs the <see cref="Activity"/>.
        /// </summary>
        public void Run()
        {
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

            if (Status == ActivityStatus.Failed)
            {
                log.ErrorFormat(Resources.Activity_Run_Execution_of_0_has_failed, Name);

                return;
            }

            Status = ActivityStatus.Executed;
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