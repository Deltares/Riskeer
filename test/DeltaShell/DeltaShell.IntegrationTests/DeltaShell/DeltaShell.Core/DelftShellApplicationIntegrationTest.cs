using System.Threading;
using System.Windows.Forms;
using DelftTools.Shell.Core.Workflow;
using DelftTools.TestUtils;
using DeltaShell.Core;
using NUnit.Framework;

namespace DeltaShell.IntegrationTests.DeltaShell.DeltaShell.Core
{
    [TestFixture]
    public class DeltaShellApplicationIntegrationTest
    {
        [Test]
        public void RunManyActivitiesCheckForThreadingIssues()
        {
            var smallActivity = new SmallActivity();
            using (var app = new DeltaShellApplication {WaitMethod = Application.DoEvents})
            {
                for (int i = 0; i < 2000; i++)
                    app.RunActivity(smallActivity);
            }
        }

        private class SmallActivity : Activity
        {
            protected override void OnInitialize() { }

            protected override void OnExecute()
            {
                Thread.Sleep(2);
                Status = ActivityStatus.Done;
            }

            protected override void OnCancel() { }

            protected override void OnCleanUp() { }

            protected override void OnFinish() { }
        }
    }
}