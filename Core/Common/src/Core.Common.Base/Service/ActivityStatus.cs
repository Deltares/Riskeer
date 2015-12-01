namespace Core.Common.Base.Service
{
    /// <summary>
    /// Defines possible states of the activity.
    /// </summary>
    public enum ActivityStatus
    {
        /// <summary>
        /// Activity has been just created and not used yet.
        /// </summary>
        None,

        /// <summary>
        /// Activity is being initialized.
        /// </summary>
        Initializing,

        /// <summary>
        /// Activity has been initialized and ready for execution.
        /// </summary>
        Initialized,

        /// <summary>
        /// Activity is currently executing.
        /// </summary>
        Executing,

        /// <summary>
        /// Activity has executed.
        /// </summary>
        Executed,

        /// <summary>
        /// Activity is finishing.
        /// </summary>
        Finishing,

        /// <summary>
        /// Activity has finished successfully.
        /// </summary>
        Finished,

        /// <summary>
        /// Activity has run but failed to complete.
        /// </summary>
        Failed,

        /// <summary>
        /// Activite execution is being cancelled.
        /// </summary>
        Cancelling,

        /// <summary>
        /// Activity execution has been cancelled.
        /// </summary>
        Cancelled
    }
}