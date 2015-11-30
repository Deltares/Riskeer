using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core.Common.Base.Workflow;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.ProgressDialog
{
    public partial class ActivityProgressDialog : Form
    {
        private Task task;
        private IActivity runningActivity;
        private readonly IEnumerable<IActivity> activities;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ProgressReporter progressReporter = new ProgressReporter();

        public ActivityProgressDialog(IEnumerable<IActivity> activities)
        {
            InitializeComponent();

            this.activities = activities ?? Enumerable.Empty<IActivity>();
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
                        labelActivityDescription.Text = string.Format(Resources.ActivityProgressDialog_OnShown_Executing_activity_with_name_0, runningActivity.Name);

                        // Update the visibility of the activity progress text related controls
                        pictureBoxActivityProgressText.Visible = false;
                        labelActivityProgressText.Visible = false;

                        // Update the activity counter label
                        labelActivityCounter.Text = string.Format(Resources.ActivityProgressDialog_OnShown_Executing_step_0_of_1, i + 1, activityCount);
                    });

                    try
                    {
                        runningActivity.ProgressChanged += ActivityOnProgressChanged;

                        // Run the activity
                        ActivityRunner.RunActivity(runningActivity);
                    }
                    finally
                    {
                        runningActivity.ProgressChanged -= ActivityOnProgressChanged;
                    }

                    progressReporter.ReportProgress(() =>
                    {
                        // Update the progress bar
                        progressBar.Value = 100 / activityCount * (i + 1);
                    });
                }
            }, cancellationToken);

            // Close the dialog when all activities are ran
            progressReporter.RegisterContinuation(task, Close);
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
            cancellationTokenSource.Cancel();

            // Update the activity counter label
            labelActivityCounter.Text = Resources.ActivityProgressDialog_CancelActivities_Quit_after_finishing_current_activity;
        }

        private void ActivityOnProgressChanged(object sender, EventArgs e)
        {
            progressReporter.ReportProgress(() =>
            {
                // Update the visibility of the activity progress text related controls
                pictureBoxActivityProgressText.Visible = true;
                labelActivityProgressText.Visible = true;

                // Update the activity progress text label
                labelActivityProgressText.Text = string.Format(((IActivity) sender).ProgressText);
            });
        }
    }
}