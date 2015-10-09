﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Utils;
using DelftTools.Utils.Validation;

namespace DelftTools.Shell.Gui.Swf.Validation
{
    public partial class ValidationView : UserControl, ISuspendibleView, IAdditionalView
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private Func<object, ValidationReport> onValidate;
        private bool suspend;
        private object data;

        public ValidationView()
        {
            InitializeComponent();
            validationReportControl.OnOpenViewForIssue = OpenViewForIssue;
        }

        public Func<object, ValidationReport> OnValidate
        {
            get
            {
                return onValidate;
            }
            set
            {
                onValidate = value;
                RefreshReport();
            }
        }

        public IGui Gui { get; set; }
        public Image Image { get; set; }

        public object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;

                if (data == null)
                {
                    Gui = null;
                }

                SetViewText();
                RefreshReport();
            }
        }

        public ViewInfo ViewInfo { get; set; }

        public void EnsureVisible(object item) {}

        public void SuspendUpdates()
        {
            suspend = true;
        }

        public void ResumeUpdates()
        {
            suspend = false;
        }

        private void SetViewText()
        {
            if (data == null)
            {
                return;
            }

            var dataName = (data is INameable) ? ((INameable) data).Name : data.ToString();
            Text = dataName + " Validation Report";
        }

        private bool RefreshReport()
        {
            if (Data == null || OnValidate == null)
            {
                return true;
            }

            stopwatch.Restart();

            var validationReport = OnValidate(Data);
            var oldValidationReport = validationReportControl.Data;

            if (!validationReport.Equals(oldValidationReport)) //only set if changed
            {
                validationReportControl.Data = validationReport;
                Image = ValidationReportControl.GetImageForSeverity(false, validationReport.Severity);

                // TextChanged triggers avalondock to update the image ;-)
                Text = "Refreshing...";
                SetViewText();
                // end TextChanged
            }

            stopwatch.Stop();

            // check the speed:
            var millisecondsPerRefresh = stopwatch.ElapsedMilliseconds;
            if (millisecondsPerRefresh < 500) // fast
            {
                if (manualRefreshPanel.Visible)
                {
                    manualRefreshPanel.Visible = false;
                }
            }
            else // slow
            {
                if (!manualRefreshPanel.Visible)
                {
                    manualRefreshPanel.Visible = true;
                }
                return false; //don't restart the timer
            }
            return true;
        }

        private void RefreshTimerTick(object sender, EventArgs e)
        {
            if (suspend)
            {
                return;
            }

            refreshTimer.Stop();
            if (RefreshReport())
            {
                refreshTimer.Start();
            }
        }

        private void OpenViewForIssue(ValidationIssue issue)
        {
            if (Gui == null || issue.ViewData == null)
            {
                return;
            }

            var viewOpen = Gui.DocumentViewsResolver.OpenViewForData(issue.ViewData);

            if (viewOpen)
            {
                var views = Gui.DocumentViewsResolver.GetViewsForData(issue.ViewData);
                foreach (var view in views)
                {
                    try
                    {
                        view.EnsureVisible(issue.Subject);
                    }
                    catch (Exception) {} //gulp
                }
                return;
            }

            var fileImporter = issue.ViewData as IFileImporter;
            if (fileImporter != null && fileImporter.CanImportOn(issue.Subject))
            {
                Gui.CommandHandler.ImportOn(issue.Subject, fileImporter);
            }
        }

        private void manualRefreshButton_Click(object sender, EventArgs e)
        {
            if (RefreshReport())
            {
                refreshTimer.Start();
            }
        }
    }
}