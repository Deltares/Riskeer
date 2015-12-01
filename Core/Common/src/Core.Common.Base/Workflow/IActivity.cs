using System;

namespace Core.Common.Base.Workflow
{
    /// <summary>
    /// Defines basic activity which can be executed as part of the workflow.
    /// </summary>
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
        /// Signal the activity to cancel execution. Activity cancels on next execute.
        /// This method must be implemented as thread-safe.
        /// </summary>
        void Cancel();
    }
}