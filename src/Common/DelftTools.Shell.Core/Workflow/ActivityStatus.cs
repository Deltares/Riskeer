namespace DelftTools.Shell.Core.Workflow
{
    /// <summary>
    /// Defines possible states of the activity.
    /// 
    /// TODO: migrate to WWF-based implementaiton in .NET 3.5
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
        /// Activity has executed. A possible next step could be another execute or finish 
        /// </summary>
        Executed,

        /// <summary>
        /// Activity has executed and done its last execute step
        /// </summary>
        Done,

        /// <summary>
        /// Activity is finishing. 
        /// </summary>
        Finishing,

        /// <summary>
        /// Activity has finished successfully.
        /// </summary>
        Finished,

        /// <summary>
        /// Cleaning all resources.
        /// </summary>
        Cleaning,

        /// <summary>
        /// Execution and cleaning resources are finished.
        /// </summary>
        Cleaned,

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
        Cancelled,

        /// <summary>
        /// Activity can't progress yet.
        /// </summary>
        WaitingForData
    }

    /* 
    WWF defines the following statuses for Activity:
    
    public enum ActivityStatus
    {
        Initialized, // Represents the status when an activity is being initialized. 
        Executing, // Represents the status when an activity is executing. 
        Canceling, // Represents the status when an activity is in the process of being canceled. 
        Closed, // Represents the status when an activity is closed. 
        Compensating, // Represents the status when an activity is compensating. 
        Faulting // Represents the status when an activity is faulting. 
    } 
     */
}