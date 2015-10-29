using System;
using System.Linq;
using Core.Common.BaseDelftTools;
using Core.Common.BaseDelftTools.Workflow;
using Core.Common.TestUtils;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service.Test.TestHelpers;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var pipingData = new PipingData();

            // Call
            var activity = new PipingCalculationActivity(pipingData);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual(pipingData.Name, activity.Name);
            CollectionAssert.IsEmpty(activity.DependsOn);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityStatus.None, activity.Status);
        }

        [Test]
        public void Initialize_ValidPipingDataWithOutput_LogValidationStartAndEndedAndClearOutput()
        {
            // Setup
            var validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
            validPipingData.Output = new TestPipingOutput();

            var activity = new PipingCalculationActivity(validPipingData);

            // Call
            Action call = () => activity.Initialize();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validPipingData.Name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validPipingData.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Initialized, activity.Status);
            Assert.IsNull(validPipingData.Output);
        }

        [Test]
        public void Initialize_InvalidPipingDataWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var originalOutput = new TestPipingOutput();

            var invalidPipingData = PipingDataFactory.CreateCalculationWithInvalidData();
            invalidPipingData.Output = originalOutput;

            var activity = new PipingCalculationActivity(invalidPipingData);

            // Call
            Action call = () => activity.Initialize();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.Greater(msgs.Length, 2, "Expecting more than 2 messages");

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidPipingData.Name), msgs.First());
                foreach (var expectedValidationErrorMessage in msgs.Skip(1).Take(msgs.Length-2))
                {
                    StringAssert.StartsWith("Validatie mislukt: ", expectedValidationErrorMessage);
                }
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidPipingData.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Failed, activity.Status);
            Assert.AreEqual(originalOutput, invalidPipingData.Output);
        }

        [Test]
        public void Execute_InvalidPipingDataAndInitialized_ErrorLoggedAndOutputNullWithStartAndEnd()
        {
            // Setup
            var validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
            validPipingData.Diameter70.Mean = 0;
            validPipingData.DarcyPermeability.Mean = 0;
            validPipingData.Output = new TestPipingOutput();

            var activity = new PipingCalculationActivity(validPipingData);
            activity.Initialize();

            // Call
            Action call = () => activity.Execute();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", validPipingData.Name), msgs.First());
                StringAssert.StartsWith("Piping berekening niet gelukt: ", msgs[1]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", validPipingData.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Done, activity.Status);
            Assert.IsNull(validPipingData.Output);
        }

        [Test]
        public void Execute_ValidPipingDataAndInitialized_PerformPipingCalculationAndLogStartAndEnd()
        {
            // Setup
            var validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
            validPipingData.Output = null;

            var activity = new PipingCalculationActivity(validPipingData);
            activity.Initialize();

            // Call
            Action call = () => activity.Execute();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", validPipingData.Name), msgs.First());
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", validPipingData.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Done, activity.Status);
            Assert.IsNotNull(validPipingData.Output);
        }

        [Test]
        public void OnFinish_ValidPipingDataAndExecuted_NotifyObserversOfPipingData()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
            validPipingData.Output = null;
            validPipingData.Attach(observerMock);

            var activity = new PipingCalculationActivity(validPipingData);
            activity.Initialize();
            activity.Execute();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}