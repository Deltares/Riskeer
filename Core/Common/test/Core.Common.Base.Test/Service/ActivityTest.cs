// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
            Assert.AreEqual("", activity.Name);
            Assert.AreEqual(ActivityState.None, activity.State);
            Assert.IsNull(activity.ProgressText);
            CollectionAssert.IsEmpty(activity.LogMessages);
        }

        [Test]
        public void ProgressText_SetValue_ProgressChangedListenersAreNotified()
        {
            // Setup
            const string someProgress = "Some progress";

            var counter = 0;
            var activity = new SimpleActivity(false, false, false);

            activity.ProgressChanged += (sender, args) => counter++;

            // Call
            activity.SetProgressText(someProgress);

            // Assert
            Assert.AreEqual(1, counter);
            Assert.AreEqual(someProgress, activity.ProgressText);
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
        public void Cancel_ActivityWithSuccessfulCancel_StateIsChangedToCanceled()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false);

            // Call
            activity.Cancel();

            // Assert
            Assert.AreEqual(ActivityState.Canceled, activity.State);
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
            var activity = new SimpleActivity(false, false, false);

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van '' is gelukt."
            });

            Assert.AreEqual(ActivityState.Finished, activity.State);
        }

        [Test]
        public void Finish_ActivityWithFailingFinish_MessageIsSendToLogAndStateIsChangedToFailed()
        {
            // Setup
            var activity = new SimpleActivity(false, false, true);

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Executed, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van '' is mislukt."
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
        public void Finish_CanceledActivityWithSuccessfulFinish_MessageIsSendToLogAndPreviousStateIsPreserved()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false);

            activity.Cancel();

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van '' is geannuleerd."
            });

            Assert.AreEqual(ActivityState.Canceled, activity.State);
        }

        [Test]
        public void Finish_FailedActivityWithSuccessfulFinish_MessageIsSendToLogAndPreviousStateIsPreserved()
        {
            // Setup
            var activity = new SimpleActivity(true, false, false);

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Failed, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van '' is mislukt."
            });

            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Finish_SkippedActivityWithcSuccessfulFinish_MessageIsSendToLogAndPreviousStateIsPreserved()
        {
            // Setup
            var activity = new SimpleActivity(false, false, false, true);

            activity.Run();

            // Precondition
            Assert.AreEqual(ActivityState.Skipped, activity.State);

            // Call / Assert
            TestHelper.AssertLogMessagesAreGenerated(() => activity.Finish(), new[]
            {
                "Uitvoeren van '' is overgeslagen."
            });

            Assert.AreEqual(ActivityState.Skipped, activity.State);
        }

        private class SimpleActivity : Activity
        {
            private readonly bool throwOnRun;
            private readonly bool throwOnCancel;
            private readonly bool throwOnFinish;
            private readonly bool skipped;

            public SimpleActivity(bool throwOnRun, bool throwOnCancel, bool throwOnFinish, bool skipped = false)
            {
                this.throwOnRun = throwOnRun;
                this.throwOnCancel = throwOnCancel;
                this.throwOnFinish = throwOnFinish;
                this.skipped = skipped;
            }

            public void SetProgressText(string progressText)
            {
                ProgressText = progressText;
            }

            protected override void OnRun()
            {
                if (skipped)
                {
                    State = ActivityState.Skipped;
                    return;
                }

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
        }
    }
}