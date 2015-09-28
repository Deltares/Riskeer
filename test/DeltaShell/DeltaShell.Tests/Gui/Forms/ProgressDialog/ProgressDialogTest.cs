using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using DelftTools.Shell.Core.Workflow;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms.ProgressDialog
{

    [TestFixture]
    public class ProgressDialogTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        [Category(TestCategory.Slow)]
        public void Show()
        {
            var dialog = new DeltaShell.Gui.Forms.ProgressDialog.ProgressDialog();
            IList<IActivity> activities = new List<IActivity>();
            var activity = new TestActivity(1, 1, "kees");
            activities.Add(activity);

            //dialog.Bind(activities);
            dialog.Show();
            new Thread(delegate()
            {
                activity.Initialize();
                activity.Execute();
            }).Start();
            //activity.Initialize).Start();

            //Application.DoEvents();
            //change an activity

            DateTime startTime = DateTime.Now;
            while (DateTime.Now - startTime < new TimeSpan(0, 0, 5))
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowCustomActivity()
        {
            var dialog = new DeltaShell.Gui.Forms.ProgressDialog.ProgressDialog();
            var activities = new EventedList<IActivity>();
            var activity = new TestStringBasedProgressingActivity() { Name = "Jetje" };

            activity.Initialize();

            activities.Add(activity);
            //activities.Add(new TestProgressReportingActivity());

            dialog.Data = activities;
            dialog.Show();
            new Thread(activity.Execute).Start();
            //activity.Initialize).Start();

            //Application.DoEvents();
            //change an activity

            //wait for the activity to finish
            while (activity.Status != ActivityStatus.Done)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }

        
    }
}