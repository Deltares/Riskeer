// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
        private Task task;
        private Activity runningActivity;

        /// <summary>
        /// Method prototype for updating the progress step visualization user controls.
        /// </summary>
        /// <param name="stepNumberForProgressNotification">The current step number.</param>
        /// <param name="activityCount">The total number of activities.</param>
        private delegate void UpdateActivityStepControlsDelegate(int stepNumberForProgressNotification, int activityCount);

        /// <summary>
        /// Method prototype for updating the progress bar control.
        /// </summary>
        /// <param name="progressBarValue">The new progress bar value.</param>
        private delegate void UpdateProgressBarDelegate(int progressBarValue);

        /// <summary>
        /// Method prototype for calling <see cref="Activity.Finish"/> and closing this dialog.
        /// </summary>
        private delegate void FinishActivitiesAndCloseDialogDelegate();

        /// <summary>
        /// Method prototype for updating controls visualizing the progress of the activities.
        /// </summary>
        /// <param name="activity">The activity currently raising <see cref="Activity.ProgressChanged"/>.</param>
        private delegate void UpdateProgressControlsDelegate(Activity activity);

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityProgressDialog"/> class.
        /// </summary>
        /// <param name="dialogParent">The dialog parent for which this dialog should be shown on top.</param>
        /// <param name="activities">The activities to be executed when the dialog is shown.</param>
        public ActivityProgressDialog(IWin32Window dialogParent, IEnumerable<Activity> activities) : base(dialogParent, Resources.Ringtoets, 520, 150)
        {
            InitializeComponent();

            this.activities = activities ?? Enumerable.Empty<Activity>();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            int activityCount = activities.Count();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            task = RunAllActivitiesAsTask(activityCount, cancellationToken);

            FinishAllActivitiesInThreadSafeManner();
        }

        protected override Button GetCancelButton()
        {
            return buttonCancel;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                CancelRunningActivities();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"><c>true</c> if managed resources should be disposed; otherwise, <c>false</c>.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                task?.Dispose();
            }

            cancellationTokenSource.Dispose();

            base.Dispose(disposing);
        }

        private Task RunAllActivitiesAsTask(int activityCount, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => RunAllActivities(activityCount, cancellationToken), cancellationToken);
        }

        private void RunAllActivities(int activityCount, CancellationToken cancellationToken)
        {
            for (var i = 0; i < activityCount; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                runningActivity = activities.ElementAt(i);
                int stepNumberForProgressNotification = i + 1;

                RunActivity(stepNumberForProgressNotification, activityCount);
            }
        }

        private void RunActivity(int stepNumberForProgressNotification, int activityCount)
        {
            if (InvokeRequired)
            {
                UpdateActivityStepControlsDelegate updateDelegate = UpdateProgressControls;
                Invoke(updateDelegate, stepNumberForProgressNotification, activityCount);
            }
            else
            {
                UpdateProgressControls(stepNumberForProgressNotification, activityCount);
            }

            try
            {
                runningActivity.ProgressChanged += ActivityOnProgressChanged;

                runningActivity.Run();
                runningActivity.LogState();
            }
            finally
            {
                runningActivity.ProgressChanged -= ActivityOnProgressChanged;
            }

            UpdateProgressBar(stepNumberForProgressNotification, activityCount);
        }

        private void FinishAllActivitiesInThreadSafeManner()
        {
            task.ContinueWith(t =>
            {
                if (InvokeRequired)
                {
                    FinishActivitiesAndCloseDialogDelegate wrappingUpDelegate = FinishAllActivitiesAndCloseDialog;
                    Invoke(wrappingUpDelegate);
                }
                else
                {
                    FinishAllActivitiesAndCloseDialog();
                }
            }, CancellationToken.None);
        }

        private void FinishAllActivitiesAndCloseDialog()
        {
            foreach (Activity activity in activities)
            {
                activity.Finish();
            }

            Close();
        }

        private void ActivityProgressDialogFormClosing(object sender, FormClosingEventArgs e)
        {
            if (task != null && task.Status == TaskStatus.Running)
            {
                CancelRunningActivities();

                e.Cancel = true;
            }
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            CancelRunningActivities();
        }

        private void CancelRunningActivities()
        {
            cancellationTokenSource.Cancel();
            runningActivity.Cancel();

            labelActivityCounter.Text = Resources.ActivityProgressDialog_CancelActivities_Quit_after_finishing_current_activity;
        }

        private void UpdateProgressBar(int stepNumberForProgressNotification, int activityCount)
        {
            var progressBarValue = (int) Math.Round(100.0 / activityCount * stepNumberForProgressNotification);
            if (InvokeRequired)
            {
                UpdateProgressBarDelegate updateDelegate = UpdateProgressBar;
                Invoke(updateDelegate, progressBarValue);
            }
            else
            {
                UpdateProgressBar(progressBarValue);
            }
        }

        private void UpdateProgressBar(int progressBarValue)
        {
            progressBar.Value = progressBarValue;
        }

        private void ActivityOnProgressChanged(object sender, EventArgs e)
        {
            var activity = sender as Activity;
            if (activity == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                UpdateProgressControlsDelegate updateDelegate = UpdateProgressControls;
                Invoke(updateDelegate, activity);
            }
            else
            {
                UpdateProgressControls(activity);
            }
        }

        private void UpdateProgressControls(int stepNumberForProgressNotification, int activityCount)
        {
            labelActivityDescription.Text = runningActivity.Description;
            pictureBoxActivityProgressText.Visible = false;
            labelActivityProgressText.Visible = false;
            labelActivityCounter.Text = string.Format(CultureInfo.CurrentCulture,
                                                      Resources.ActivityProgressDialog_OnShown_Executing_step_0_of_1,
                                                      stepNumberForProgressNotification, activityCount);
        }

        private void UpdateProgressControls(Activity activity)
        {
            bool progressTextNullOrEmpty = string.IsNullOrEmpty(activity.ProgressText);

            pictureBoxActivityProgressText.Visible = !progressTextNullOrEmpty;
            labelActivityProgressText.Visible = !progressTextNullOrEmpty;
            labelActivityProgressText.Text = progressTextNullOrEmpty ? string.Empty : activity.ProgressText;
        }
    }
}