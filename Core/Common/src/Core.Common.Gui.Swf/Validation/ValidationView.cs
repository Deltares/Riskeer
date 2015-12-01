using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.Controls;
using Core.Common.Utils.Validation;

namespace Core.Common.Gui.Swf.Validation
{
    public partial class ValidationView : UserControl, IAdditionalView
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private Func<object, ValidationReport> onValidate;
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

                RefreshReport();
            }
        }

        public ViewInfo ViewInfo { get; set; }

        public void EnsureVisible(object item) {}

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