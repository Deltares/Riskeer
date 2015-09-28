using System.Threading;
using DelftTools.Shell.Core.Workflow;
using NUnit.Framework;
using Rhino.Mocks;

namespace DelftTools.Tests.Shell.Core.WorkFlow
{
    [TestFixture]
    public class ActivityRunnerTest
    {
        private static readonly MockRepository mocks = new MockRepository();

        [Test]
        public void ActivityIsRemovedAfterItIsFinished()
        {
            var activity = new SimpleActivity();

            var runner = new ActivityRunner();
            runner.Enqueue(activity);

            while (runner.IsRunning)
            {
                Thread.Sleep(0);
            }

            Assert.AreEqual(0, runner.Activities.Count, "Activity is removed from the list of running activities after it is finished");
        }

        public class SimpleActivity : Activity
        {
            public SimpleActivity()
            {
                Name = "simple activity";
            }

            protected override void OnInitialize()
            {
            }

            protected override void OnExecute()
            {
                Status = ActivityStatus.Done;
            }

            protected override void OnCancel()
            {
            }

            protected override void OnCleanUp()
            {
            }

            protected override void OnFinish()
            {

            }
        }

        [Test]
        public void CancelActivityRemovesItFromActivityRunner()
        {
            var activity = new InfiniteActivity();

            var runner = new ActivityRunner();
            runner.Enqueue(activity);
            
            while(activity.Status != ActivityStatus.Executing)
            {
                Thread.Sleep(1);
            }

            runner.Cancel(activity);

            Thread.Sleep(200); // HACK: API should allow some other way

            Assert.AreEqual(0, runner.Activities.Count);
        }

        [Test]
        public void CancelAllActivitiesRemovesThemFromActivityRunner()
        {
            var activity = new InfiniteActivity();
            var activity2 = new InfiniteActivity();

            var runner = new ActivityRunner();
            runner.Enqueue(activity);
            runner.Enqueue(activity2);

            while (activity.Status != ActivityStatus.Executing) { Thread.Sleep(1); }

            runner.CancelAll();

            Thread.Sleep(200); // HACK: API should allow some other way

            Assert.AreEqual(0, runner.Activities.Count);
        }
    }

    public class InfiniteActivity : Activity
    {
        private bool shouldCancel;


        protected override void OnInitialize()
        {
        }

        protected override void OnExecute()
        {
            Thread.Sleep(100);
            Status = ActivityStatus.Done;
        }

        protected override void OnCancel()
        {
        }

        protected override void OnCleanUp()
        {
        }

        protected override void OnFinish()
        {

        }
    }
}