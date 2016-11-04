// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="task"/> is <c>null</c>.</exception>
        public void RegisterContinuation(Task task, Action action)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            task.ContinueWith(_ => action(), CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }
    }
}