using System;
using Core.Common.Base.Workflow;
using NUnit.Framework;
using SharpTestsEx;

namespace Core.Common.Base.Tests.Shell.Core.WorkFlow
{
    [TestFixture]
    public class CompositeActivityTest
    {
        [Test]
        public void ExecuteInParallelModeUsingSimpleActivities()
        {
            var a1 = new SimpleActivity
            {
                Input = 2
            };
            var a2 = new SimpleActivity
            {
                Input = 3
            };

            var workflow = new ParallelActivity
            {
                Activities =
                {
                    a1, a2
                }
            };

            workflow.Initialize();
            workflow.Execute();

            a1.Output.Should().Be.EqualTo(4);
            a2.Output.Should().Be.EqualTo(6);
            a1.Status.Should().Be.EqualTo(ActivityStatus.Done);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Done);
        }

        [Test]
        public void ExecuteInSequentialModeUsingSimpleActivities()
        {
            var a1 = new SimpleActivity
            {
                Input = 2
            };
            var a2 = new SimpleActivity
            {
                Input = 3
            };
            var workflow = new SequentialActivity
            {
                Activities =
                {
                    a1, a2
                }
            };

            workflow.Initialize();
            workflow.Execute(); // initializes 1st activity
            workflow.Execute();

            workflow.Finish();

            a1.Output.Should().Be.EqualTo(4);
            a2.Output.Should().Be.EqualTo(6);
            a1.Status.Should().Be.EqualTo(ActivityStatus.Finished);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Finished);

            workflow.Cleanup();

            a1.Status.Should().Be.EqualTo(ActivityStatus.Cleaned);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Cleaned);
        }

        [Test]
        public void ExecuteInSequentialModeUsingSimpleTwoStepActivities()
        {
            var a1 = new SimpleTwoStepActivity
            {
                Input = 2
            };
            var a2 = new SimpleTwoStepActivity
            {
                Input = 3
            };
            var workflow = new SequentialActivity
            {
                Activities =
                {
                    a1, a2
                }
            };

            workflow.Initialize();
            while (workflow.Status != ActivityStatus.Done)
            {
                workflow.Execute();

                if (workflow.Status == ActivityStatus.Failed)
                {
                    Assert.Fail("failed!");
                }
            }

            workflow.Finish();

            a1.Output.Should().Be.EqualTo(16);
            a2.Output.Should().Be.EqualTo(12);
            a1.Status.Should().Be.EqualTo(ActivityStatus.Finished);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Finished);
            a1.Initialized.Should().Be.True();
            a2.Initialized.Should().Be.True();

            workflow.Cleanup();

            a1.Status.Should().Be.EqualTo(ActivityStatus.Cleaned);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Cleaned);
        }

        [Test]
        public void ParallelAndSequentialActivityExample()
        {
            var a1 = new SimpleActivity();
            var a2 = new SimpleActivity();
            var a3 = new SimpleActivity();
            var a4 = new SimpleActivity();

            var workflow = new ParallelActivity
            {
                Activities =
                {
                    a1,
                    new SequentialActivity
                    {
                        Activities =
                        {
                            a2, a3
                        }
                    },
                    a4
                }
            };

            workflow.Initialize();
            workflow.Execute();
            workflow.Execute();

            workflow.Finish();

            a1.Status.Should().Be.EqualTo(ActivityStatus.Finished);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Finished);

            workflow.Cleanup();

            a1.Status.Should().Be.EqualTo(ActivityStatus.Cleaned);
            a2.Status.Should().Be.EqualTo(ActivityStatus.Cleaned);
        }

        private class SimpleActivity : Activity
        {
            public int Input { get; set; }

            public int Output { get; set; }

            protected override void OnInitialize()
            {
                Output = 0;
            }

            protected override void OnExecute()
            {
                Output = Input*2;
                Status = ActivityStatus.Done;
            }

            protected override void OnCancel() {}

            protected override void OnCleanUp() {}

            protected override void OnFinish() {}
        }

        private class SimpleTwoStepActivity : Activity
        {
            public int Input { get; set; }

            public int Output { get; set; }

            public bool Initialized { get; set; }

            protected override void OnInitialize()
            {
                if (Initialized)
                {
                    throw new InvalidOperationException();
                }
                Output = Input;

                Initialized = true;
            }

            protected override void OnExecute()
            {
                Output *= 2;

                if (Output > 10)
                {
                    Status = ActivityStatus.Done;
                }
            }

            protected override void OnCancel() {}

            protected override void OnCleanUp() {}

            protected override void OnFinish() {}
        }
    }
}