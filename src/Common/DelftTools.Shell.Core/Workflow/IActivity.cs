using System;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Shell.Core.Workflow
{
    /// <summary>
    /// Defines basic activity which can be executed as part of the workflow.
    /// </summary>
    public interface IActivity : IProjectItem /* TODO: wrap it with the ProjectItem instead */
    {
        /// <summary>
        /// Activity may depend on other activities. Used within <see cref="CompositeActivity"/>.
        /// </summary>
        IEventedList<IActivity> DependsOn { get; set; }

        /// <summary>
        /// Returns current status of the activity (executing, cancelling, etc.)
        /// </summary>
        ActivityStatus Status { get; }

        /// <summary>
        /// Text to describe the current progress of the activity. Most often a percentage.
        /// </summary>
        string ProgressText { get; }

        /// <summary>
        /// Initializes activity. If initialization step is successful, <see cref="Status"/> will change to <see cref="ActivityStatus.Initialized"/>.
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
        void Cleanup();

        /// <summary>
        /// Event to be fired when we want to publish changes in <see cref="ProgressText"/>.
        /// </summary>
        event EventHandler ProgressChanged;

        /// <summary>
        /// Event to be fired on every <see cref="Status"/> change.
        /// </summary>
        event EventHandler<ActivityStatusChangedEventArgs> StatusChanged;
    }
}