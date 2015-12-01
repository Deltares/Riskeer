using System;

namespace Core.Common.Base.Workflow
{
    /// <summary>
    /// Defines basic activity which can be executed as part of the workflow.
    /// </summary>
    /// <example>
    /// <para>Regular workflow for an activity is: Initialize -> Execute -> Finish -> Cleanup.</para>
    /// <para>Regular workflow for an activity with an error occurring: [Initialize/Execute/Finish] -> ! Exception / Error ! -> Cleanup.</para> 
    /// <para>Regular workflow for an activity being cancelled: [Initialize/Execute/Finish] -> ! Cancel ! -> Cleanup.</para>
    /// </example>
    public interface IActivity
    {
        /// <summary>
        /// Name of the activity
        /// </summary>
        string Name { get; }

        string Log { get; set; }

        /// <summary>
        /// Event to be fired when we want to publish changes in <see cref="ProgressText"/>.
        /// </summary>
        event EventHandler ProgressChanged;

        /// <summary>
        /// Returns current status of the activity (executing, cancelling, etc.)
        /// </summary>
        ActivityStatus Status { get; }

        /// <summary>
        /// Text to describe the current progress of the activity. Most often a percentage.
        /// </summary>
        string ProgressText { get; }

        /// <summary>
        /// Runs the activity.
        /// </summary>
        void Run();

        /// <summary>
        /// Initializes activity. If initialization step is successful, <see cref="Status"/> 
        /// will change to <see cref="ActivityStatus.Initialized"/>.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Executes activity. Depending on status of the activity execution may need to be repeated.
        /// </summary>
        void Execute();

        /// <summary>
        /// Signal the activity to cancel execution. Activity cancels on next execute.
        /// 
        /// This method must be implemented as thread-safe.
        /// </summary>
        void Cancel();

        /// <summary>
        /// The activity has finished executing.
        /// </summary>
        void Finish();

        /// <summary>
        /// Cleans all resources required during execution.
        /// </summary>
        /// <remarks>This method will always be called, even when an exception occurs. Take
        /// this into account when implementing this method.</remarks>
        void Cleanup();
    }
}