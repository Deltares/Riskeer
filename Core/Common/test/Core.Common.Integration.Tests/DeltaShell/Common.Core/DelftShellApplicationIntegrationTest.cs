using System.Threading;
using Core.Common.Base;
using Core.Common.Base.Workflow;
using NUnit.Framework;

namespace Core.Common.Integration.Tests.DeltaShell.Common.Core
{
    [TestFixture]
    public class DeltaShellApplicationIntegrationTest
    {
        [Test]
        public void RunManyActivitiesCheckForThreadingIssues()
        {
            var smallActivity = new SmallActivity();
            using (var app = new DeltaShellApplication
            {
                WaitMethod = System.Windows.Forms.Application.DoEvents
            })
            {
                for (int i = 0; i < 500; i++)
                {
                    app.RunActivity(smallActivity);
                }
            }
        }

        private class SmallActivity : Activity
        {
            protected override void OnInitialize() {}

            protected override void OnExecute()
            {
                Thread.Sleep(1);
                Status = ActivityStatus.Done;
            }

            protected override void OnCancel() {}

            protected override void OnCleanUp() {}

            protected override void OnFinish() {}
        }
    }
}