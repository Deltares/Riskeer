using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    /// <summary>
    /// A class used by <see cref="Task"/> to report progress or completion updates back to the UI.
    /// </summary>
    public class ProgressReporter
    {
        /// <summary>
        /// The underlying scheduler for the UI's synchronization context.
        /// </summary>
        private readonly TaskScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReporter"/> class.
        /// This should be run on a UI thread.
        /// </summary>
        public ProgressReporter()
        {
            scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        /// <summary>
        /// Reports the progress to the UI thread, and waits for the UI thread to process
        /// the update before returning. This method should be called from the task.
        /// </summary>
        /// <param name="action">The action to perform in the context of the UI thread.</param>
        public void ReportProgress(Action action)
        {
            var task = Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, scheduler);

            task.Wait();
        }

        /// <summary>
        /// Registers a UI thread handler for when the specified task finishes execution,
        /// whether it finishes with success, failure, or cancellation.
        /// </summary>
        /// <param name="task">The task to monitor for completion.</param>
        /// <param name="action">The action to take when the task has completed, in the context of the UI thread.</param>
        /// <returns>The continuation created to handle completion. This is normally ignored.</returns>
        public void RegisterContinuation(Task task, Action action)
        {
            task.ContinueWith(_ => action(), CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }
    }
}