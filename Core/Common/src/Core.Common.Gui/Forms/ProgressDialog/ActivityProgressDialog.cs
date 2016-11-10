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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Common.Base.Service;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui.Appenders;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    /// <summary>
    /// Dialog that runs a sequence of activities, showing their progress during the execution,
    /// when shown.
    /// </summary>
    public partial class ActivityProgressDialog : DialogBase
    {
        private readonly IEnumerable<Activity> activities;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ProgressReporter progressReporter = new ProgressReporter();
        private Task task;
        private Activity runningActivity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityProgressDialog"/> class.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which this dialog should be shown on top.</param>
        /// <param name="activities">The activities to be executed when the dialog is shown.</param>
        public ActivityProgressDialog(IWin32Window dialogParent, IEnumerable<Activity> activities) : base(dialogParent, Resources.Ringtoets, 520, 150)
        {
            InitializeComponent();

            MinimizeBox = true; // Allows for minimizing the dialog parent (in case of long-lasting activities)

            this.activities = activities ?? Enumerable.Empty<Activity>();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var activityCount = activities.Count();
            var cancellationToken = cancellationTokenSource.Token;

            // Run all activities as part of a task
            task = Task.Factory.StartNew(() =>
            {
                for (var i = 0; i < activityCount; i++)
                {
                    // Check for cancellation
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    runningActivity = activities.ElementAt(i);

                    int stepNumberForProgressNotification = i + 1;
                    progressReporter.ReportProgress(() =>
                    {
                        // Update the activity description label
                        labelActivityDescription.Text = runningActivity.Name;

                        // Update the visibility of the activity progress text related controls
                        pictureBoxActivityProgressText.Visible = false;
                        labelActivityProgressText.Visible = false;

                        // Update the activity counter label
                        labelActivityCounter.Text = string.Format(CultureInfo.CurrentCulture,
                                                                  Resources.ActivityProgressDialog_OnShown_Executing_step_0_of_1,
                                                                  stepNumberForProgressNotification, activityCount);
                    });

                    try
                    {
                        if (RenderedMessageLogAppender.Instance != null)
                        {
                            RenderedMessageLogAppender.Instance.AppendMessageLineAction = message => runningActivity.LogMessages.Add(message);
                        }

                        runningActivity.ProgressChanged += ActivityOnProgressChanged;

                        // Run the activity
                        runningActivity.Run();
                    }
                    finally
                    {
                        if (RenderedMessageLogAppender.Instance != null)
                        {
                            RenderedMessageLogAppender.Instance.AppendMessageLineAction = null;
                        }

                        runningActivity.ProgressChanged -= ActivityOnProgressChanged;
                    }

                    progressReporter.ReportProgress(() =>
                    {
                        // Update the progress bar
                        progressBar.Value = (int) Math.Round(100.0/activityCount*stepNumberForProgressNotification);
                    });
                }
            }, cancellationToken);

            // Afterwards, perform actions that (might) affect the UI thread
            progressReporter.RegisterContinuation(task, () =>
            {
                // Finish all activities
                foreach (var activity in activities)
                {
                    activity.Finish();
                }

                // Close the dialog
                Close();
            });
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                CancelActivities();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            CancelActivities();
        }

        private void ActivityProgressDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            if (task != null && task.Status == TaskStatus.Running)
            {
                // Perform a cancel action when the activities are still running
                CancelActivities();

                // Cancel the close action
                e.Cancel = true;
            }
        }

        private void CancelActivities()
        {
            // Cancel all activities in the queue
            cancellationTokenSource.Cancel();

            // Cancel the currently running activity
            runningActivity.Cancel();

            // Update the activity counter label
            labelActivityCounter.Text = Resources.ActivityProgressDialog_CancelActivities_Quit_after_finishing_current_activity;
        }

        private void ActivityOnProgressChanged(object sender, EventArgs e)
        {
            var activity = sender as Activity;
            if (activity == null)
            {
                return;
            }

            progressReporter.ReportProgress(() =>
            {
                var progressTextNullOrEmpty = string.IsNullOrEmpty(activity.ProgressText);

                // Update the visibility of the activity progress text related controls
                pictureBoxActivityProgressText.Visible = !progressTextNullOrEmpty;
                labelActivityProgressText.Visible = !progressTextNullOrEmpty;

                // Update the activity progress text label
                labelActivityProgressText.Text = progressTextNullOrEmpty ? string.Empty : activity.ProgressText;
            });
        }
    }
}