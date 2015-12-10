using System;
using System.Collections.Generic;
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
    public partial class ActivityProgressDialog : DialogBase
    {
        private Task task;
        private Activity runningActivity;
        private readonly IEnumerable<Activity> activities;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ProgressReporter progressReporter = new ProgressReporter();

        public ActivityProgressDialog(IWin32Window owner, IEnumerable<Activity> activities) : base(owner, Resources.Ringtoets)
        {
            InitializeComponent();

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
                    cancellationToken.ThrowIfCancellationRequested();

                    runningActivity = activities.ElementAt(i);

                    progressReporter.ReportProgress(() =>
                    {
                        // Update the activity description label
                        labelActivityDescription.Text = string.Format(runningActivity.Name);

                        // Update the visibility of the activity progress text related controls
                        pictureBoxActivityProgressText.Visible = false;
                        labelActivityProgressText.Visible = false;

                        // Update the activity counter label
                        labelActivityCounter.Text = string.Format(Resources.ActivityProgressDialog_OnShown_Executing_step_0_of_1, i + 1, activityCount);
                    });

                    try
                    {
                        if (RunReportLogAppender.Instance != null)
                        {
                            RunReportLogAppender.Instance.AppendMessageLineAction = message => runningActivity.LogMessages.Add(message);
                        }

                        runningActivity.ProgressChanged += ActivityOnProgressChanged;

                        // Run the activity
                        runningActivity.Run();
                    }
                    finally
                    {
                        if (RunReportLogAppender.Instance != null)
                        {
                            RunReportLogAppender.Instance.AppendMessageLineAction = null;
                        }

                        runningActivity.ProgressChanged -= ActivityOnProgressChanged;
                    }

                    progressReporter.ReportProgress(() =>
                    {
                        // Update the progress bar
                        progressBar.Value = (int) Math.Round(100.0 / activityCount * (i + 1));
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
            if (task.Status == TaskStatus.Running)
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
                labelActivityProgressText.Text = !progressTextNullOrEmpty
                                                     ? activity.ProgressText.Length <= 75
                                                           ? activity.ProgressText
                                                           : activity.ProgressText.Take(75) + "..."
                                                     : "";
            });
        }
    }
}