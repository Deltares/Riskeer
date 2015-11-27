using System;
using Core.Common.Base.Workflow;
using Core.Common.TestUtils;
using NUnit.Framework;

namespace Core.Common.Base.Test.Shell.Core.WorkFlow
{
    [TestFixture]
    public class ActivityTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var activity = new SimpleActivity();

            // assert
            Assert.IsInstanceOf<IActivity>(activity);
            Assert.IsNull(activity.Name);
            CollectionAssert.IsEmpty(activity.DependsOn);
            Assert.AreEqual(ActivityStatus.None, activity.Status);
            Assert.IsNull(activity.ProgressText);
        }

        [Test]
        public void Name_SetAndGetValue_ReturnSetValue()
        {
            // setup & call
            const string someName = "Some name";
            var activity = new SimpleActivity
            {
                Name = someName
            };

            // assert
            Assert.AreEqual(someName, activity.Name);
        }

        [Test]
        public void Status_SetNewValue_ChangeStatusAndBroadcastEvent()
        {
            // setup
            int callCount = 0;
            const ActivityStatus newStatus = ActivityStatus.Done;

            var activity = new SimpleActivity();
            ActivityStatus originalStatus = activity.Status;
            activity.StatusChanged += (sender, args) =>
            {
                callCount++;
                Assert.AreEqual(originalStatus, args.OldStatus);
                Assert.AreEqual(newStatus, args.NewStatus);
                Assert.AreSame(activity, sender);
            };

            // call
            activity.SetStatus(newStatus);

            // assert
            Assert.AreEqual(newStatus, activity.Status);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Status_SetSameValue_DoNothing()
        {
            // setup
            int callCount = 0;

            var activity = new SimpleActivity();
            ActivityStatus originalStatus = activity.Status;
            activity.StatusChanged += (sender, args) => { callCount++; };

            // call
            activity.SetStatus(originalStatus);

            // assert
            Assert.AreEqual(originalStatus, activity.Status);
            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Initialize_ActivityNotYetInitialized_UpdateStatusAndPerformInitialization()
        {
            // setup
            int statusChangeCount = 0;
            int onInitializeCallCount = 0;

            var activity = new SimpleActivity
            {
                OnInitializeInjection = () => onInitializeCallCount++
            };
            ActivityStatus originalStatus = activity.Status;
            activity.StatusChanged += (sender, args) =>
            {
                if (statusChangeCount == 0)
                {
                    // 1st transition
                    Assert.AreEqual(originalStatus, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Initializing, args.NewStatus);
                }
                else if (statusChangeCount == 1)
                {
                    // 2nd transition
                    Assert.AreEqual(ActivityStatus.Initializing, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Initialized, args.NewStatus);
                }
                else
                {
                    Assert.Fail("Expect 2 status updates.");
                }
                Assert.AreSame(activity, sender);

                statusChangeCount++;
            };

            // call
            activity.Initialize();

            // assert
            Assert.AreEqual(2, statusChangeCount);
            Assert.AreEqual(1, onInitializeCallCount);
        }

        [Test]
        public void Initialize_ActivityNotYetInitializedAndInitalizationCausesException_CatchExceptionAndUpdateStatusToFailedAndLogException()
        {
            // setup
            int statusChangeCount = 0;
            const string someErrorMessage = "Some error message";

            var activity = new SimpleActivity
            {
                OnInitializeInjection = () => { throw new Exception(someErrorMessage); }
            };
            ActivityStatus originalStatus = activity.Status;
            activity.StatusChanged += (sender, args) =>
            {
                if (statusChangeCount == 0)
                {
                    // 1st transition
                    Assert.AreEqual(originalStatus, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Initializing, args.NewStatus);
                }
                else if (statusChangeCount == 1)
                {
                    // 2nd transition
                    Assert.AreEqual(ActivityStatus.Initializing, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Failed, args.NewStatus);
                }
                else
                {
                    Assert.Fail("Expect 2 status updates.");
                }
                Assert.AreSame(activity, sender);

                statusChangeCount++;
            };

            // call
            Action call = () => activity.Initialize();

            // assert
            TestHelper.AssertLogMessageIsGenerated(call, someErrorMessage, 1);
            Assert.AreEqual(2, statusChangeCount);
        }

        [Test]
        public void Initialize_ActivityNotYetInitializedAndInitializationEncountersError_UpdateStatusAndEarlyExit()
        {
            // setup
            int statusChangeCount = 0;

            var activity = new SimpleActivity();

            activity.OnInitializeInjection = () => activity.SetStatus(ActivityStatus.Failed);
            ActivityStatus originalStatus = activity.Status;

            activity.StatusChanged += (sender, args) =>
            {
                if (statusChangeCount == 0)
                {
                    // 1st transition
                    Assert.AreEqual(originalStatus, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Initializing, args.NewStatus);
                }
                else if (statusChangeCount == 1)
                {
                    // 2nd transition
                    Assert.AreEqual(ActivityStatus.Initializing, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Failed, args.NewStatus);
                }
                else
                {
                    Assert.Fail("Expect 2 status updates.");
                }
                Assert.AreSame(activity, sender);

                statusChangeCount++;
            };

            // call
            activity.Initialize();

            // assert
            Assert.AreEqual(2, statusChangeCount);
        }

        [Test]
        public void Cancel_ActivityNotYetCancelled_UpdateStatusAndPerformCancel()
        {
            // setup
            int statusChangeCount = 0;
            int onCancelCallCount = 0;

            var activity = new SimpleActivity
            {
                OnCancelInjection = () => onCancelCallCount++
            };
            ActivityStatus originalStatus = activity.Status;
            activity.StatusChanged += (sender, args) =>
            {
                if (statusChangeCount == 0)
                {
                    // 1st transition
                    Assert.AreEqual(originalStatus, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Cancelling, args.NewStatus);
                }
                else if (statusChangeCount == 1)
                {
                    // 2nd transition
                    Assert.AreEqual(ActivityStatus.Cancelling, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Cancelled, args.NewStatus);
                }
                else
                {
                    Assert.Fail("Expect 2 status updates.");
                }
                Assert.AreSame(activity, sender);

                statusChangeCount++;
            };

            // call
            activity.Cancel();

            // assert
            Assert.AreEqual(2, statusChangeCount);
            Assert.AreEqual(1, onCancelCallCount);
        }

        [Test]
        public void Cancel_ActivityNotYetCancelledAndCancellingCausesException_CatchExceptionAndUpdateStatusToFailedAndLogException()
        {
            // setup
            int statusChangeCount = 0;
            const string someErrorMessage = "Some error message";

            var activity = new SimpleActivity
            {
                OnCancelInjection = () => { throw new Exception(someErrorMessage); }
            };
            ActivityStatus originalStatus = activity.Status;
            activity.StatusChanged += (sender, args) =>
            {
                if (statusChangeCount == 0)
                {
                    // 1st transition
                    Assert.AreEqual(originalStatus, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Cancelling, args.NewStatus);
                }
                else if (statusChangeCount == 1)
                {
                    // 2nd transition
                    Assert.AreEqual(ActivityStatus.Cancelling, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Failed, args.NewStatus);
                }
                else
                {
                    Assert.Fail("Expect 2 status updates.");
                }
                Assert.AreSame(activity, sender);

                statusChangeCount++;
            };

            // call
            Action call = () => activity.Cancel();

            // assert
            TestHelper.AssertLogMessageIsGenerated(call, someErrorMessage, 1);
            Assert.AreEqual(2, statusChangeCount);
        }

        [Test]
        public void Cancel_ActivityNotYetCancelledAndCancellingEncountersError_UpdateStatusAndEarlyExit()
        {
            // setup
            int statusChangeCount = 0;

            var activity = new SimpleActivity();

            activity.OnCancelInjection = () => activity.SetStatus(ActivityStatus.Failed);
            ActivityStatus originalStatus = activity.Status;

            activity.StatusChanged += (sender, args) =>
            {
                if (statusChangeCount == 0)
                {
                    // 1st transition
                    Assert.AreEqual(originalStatus, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Cancelling, args.NewStatus);
                }
                else if (statusChangeCount == 1)
                {
                    // 2nd transition
                    Assert.AreEqual(ActivityStatus.Cancelling, args.OldStatus);
                    Assert.AreEqual(ActivityStatus.Failed, args.NewStatus);
                }
                else
                {
                    Assert.Fail("Expect 2 status updates.");
                }
                Assert.AreSame(activity, sender);

                statusChangeCount++;
            };

            // call
            activity.Cancel();

            // assert
            Assert.AreEqual(2, statusChangeCount);
        }

        private class SimpleActivity : Activity
        {
            /// <summary>
            /// Sets the implementation of <see cref="OnInitialize"/>.
            /// </summary>
            public Action OnInitializeInjection { private get; set; }

            /// <summary>
            /// Sets the implementation of <see cref="OnCancel"/>.
            /// </summary>
            public Action OnCancelInjection { private get; set; }

            public void SetStatus(ActivityStatus newStatus)
            {
                Status = newStatus;
            }

            protected override void OnInitialize()
            {
                OnInitializeInjection();
            }

            protected override void OnExecute()
            {
                throw new NotImplementedException();
            }

            protected override void OnCancel()
            {
                OnCancelInjection();
            }

            protected override void OnCleanUp()
            {
                throw new NotImplementedException();
            }

            protected override void OnFinish()
            {
                throw new NotImplementedException();
            }
        }
    }
}