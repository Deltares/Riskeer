using System;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Service
{
    [TestFixture]
    public class ActivityTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var activity = new SimpleActivity(false, false, false);

            // Assert
            Assert.IsNull(activity.Name);
            Assert.AreEqual(ActivityState.None, activity.State);
            Assert.IsNull(activity.ProgressText);
            CollectionAssert.IsEmpty(activity.LogMessages);
        }

        [Test]
        public void Name_SetAndGetValue_ReturnSetValue()
        {
            // Setup / Call
            const string someName = "Some name";

            var activity = new SimpleActivity(false, false, false)
            {
                Name = someName
            };

            // Assert
            Assert.AreEqual(someName, activity.Name);
        }

        [Test]
        public void ProgressText_SetValue_ProgressChangedListenersAreNotified()
        {
            // Setup
            var counter = 0;
            var activity = new SimpleActivity(false, false, false);

            activity.ProgressChanged += (sender, args) => counter++;

            // Call
            activity.SetProgressText("Some progress");

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void Run_ActivityWithSuccessfulRun_StateIsChangedToExecuted()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Executed, activity.State);
        }

        [Test]
        public void Run_ActivityWithFailingRun_StateIsChangedToFailed()
        {
            // Setup
            var activity = new SimpleActivity(true, false, false);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Cancel_ActivityWithSuccessfulCancel_StateIsChangedToCancelled()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false);

            // Call
            activity.Cancel();

            // Assert
            Assert.AreEqual(ActivityState.Cancelled, activity.State);
        }

        [Test]
        public void Cancel_ActivityWithFailingCancel_StateIsChangedToFailed()
        {
            // Setup
            var activity = new SimpleActivity(false, true, false);

            // Call
            activity.Cancel();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Finish_ActivityWithSuccessfulFinish_MessageIsSendToLogAndStateIsChangedToFinished()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false) { Name = "Activity" };

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van 'Activity' is gelukt."
            });

            Assert.AreEqual(ActivityState.Finished, activity.State);
        }

        [Test]
        public void Finish_ActivityWithFailingFinish_MessageIsSendToLogAndStateIsChangedToFailed()
        {
            // Setup
            var activity = new SimpleActivity(false, false, true) { Name = "Activity" };

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van 'Activity' is mislukt."
            });

            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Finish_NotExecutedActivityWithSuccessfulFinish_MessageIsSendToLogAndPreviousStateIsPreserved()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false);

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Finish_CancelledActivityWithSuccessfulFinish_MessageIsSendToLogAndPreviousStateIsPreserved()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false) { Name = "Activity" };

            activity.Cancel();

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van 'Activity' is geannuleerd."
            });

            Assert.AreEqual(ActivityState.Cancelled, activity.State);
        }

        [Test]
        public void Finish_FailedActivityWithSuccessfulFinish_MessageIsSendToLogAndPreviousStateIsPreserved()
        {
            // Setup
            var activity = new SimpleActivity(true, false, false) { Name = "Activity" };

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Failed, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van 'Activity' is mislukt."
            });

            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        private class SimpleActivity : Activity
        {
            private readonly bool throwOnRun;
            private readonly bool throwOnCancel;
            private readonly bool throwOnFinish;

            public SimpleActivity(bool throwOnRun, bool throwOnCancel, bool throwOnFinish)
            {
                this.throwOnRun = throwOnRun;
                this.throwOnCancel = throwOnCancel;
                this.throwOnFinish = throwOnFinish;
            }

            protected override void OnRun()
            {
                if (throwOnRun)
                {
                    throw new Exception("Error during run");
                }
            }

            protected override void OnCancel()
            {
                if (throwOnCancel)
                {
                    throw new Exception("Error during cancel");
                }
            }

            protected override void OnFinish()
            {
                if (throwOnFinish)
                {
                    throw new Exception("Error during finish");
                }
            }

            public void SetProgressText(string progressText)
            {
                ProgressText = progressText;
            }
        }
    }
}