using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Workflow;
using Core.Common.Gui.Forms.ProgressDialog;
using NUnit.Framework;

namespace Core.Common.Tests.Gui.Forms
{
    [TestFixture]
    public class ProgressDialogTest
    {
        private ProgressDialog progressDlg;
        private ActivityRunner activityRunner;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            progressDlg = new ProgressDialog();
            activityRunner = new ActivityRunner();
            progressDlg.Data = activityRunner.Activities;
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            progressDlg = null;
            activityRunner = null;
        }

        [Test]
        public void ShowProgressFor2Activities()
        {
            activityRunner.Enqueue(new TimedActivity(3));
            activityRunner.Enqueue(new TimedActivity(3));

            progressDlg.Show();
            
            var grid = progressDlg.Controls.OfType<DataGridView>().First();

            Assert.AreEqual(2, grid.RowCount);

            while (activityRunner.IsRunning)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            Assert.AreEqual(0, grid.RowCount);
        }

        private class TimedActivity: ParallelActivity
        {
            private readonly int hundredsOfMilliseconds;

            public TimedActivity(int hundredsOfMilliseconds)
            {
                this.hundredsOfMilliseconds = hundredsOfMilliseconds;
            }

            protected override void OnInitialize()
            {
                Name = GetType().Name;
            }

            protected override void OnExecute()
            {
                for (var i = 0; i < hundredsOfMilliseconds; i++)
                {
                    SetProgressText(i.ToString());

                    Thread.Sleep(100);
                }

                Status = ActivityStatus.Done;
            }

            /// <summary>
            /// Cancels current run. Changes status to <see cref="ActivityStatus.Cancelled"/>.
            /// </summary>
            /// <returns>True when finished.</returns>
            protected override void OnCancel()
            {
                Status = ActivityStatus.Cancelled; 
            }

            /// <summary>
            /// Performs clean-up of all internal resources.
            /// </summary>
            /// <returns>True when finished.</returns>
            protected override void OnCleanUp()
            {
//                throw new Exception("Tried to cleanup, but not implemented");
            }

            /// <summary>
            /// Activity has finished successfully.
            /// </summary>
            protected override void OnFinish()
            {
//                throw new Exception("Tried to finish, but not implemented");
            }
        }
    }
}
