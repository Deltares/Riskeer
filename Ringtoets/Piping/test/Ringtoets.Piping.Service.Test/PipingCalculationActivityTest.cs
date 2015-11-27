using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Workflow;
using Core.Common.TestUtils;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var calculation = new PipingCalculation();

            // Call
            var activity = new PipingCalculationActivity(calculation);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual(calculation.Name, activity.Name);
            CollectionAssert.IsEmpty(activity.DependsOn);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityStatus.None, activity.Status);
        }

        [Test]
        public void Initialize_ValidPipingCalculationWithOutput_LogValidationStartAndEndedAndClearOutput()
        {
            // Setup
            var validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = new TestPipingOutput();

            var activity = new PipingCalculationActivity(validPipingCalculation);

            // Call
            Action call = () => activity.Initialize();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validPipingCalculation.Name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Initialized, activity.Status);
            Assert.IsNull(validPipingCalculation.Output);
        }

        [Test]
        public void Initialize_InvalidPipingCalculationWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var originalOutput = new TestPipingOutput();

            var invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidPipingCalculation.Output = originalOutput;

            var activity = new PipingCalculationActivity(invalidPipingCalculation);

            // Call
            Action call = () => activity.Initialize();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.Greater(msgs.Length, 2, "Expecting more than 2 messages");

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidPipingCalculation.Name), msgs.First());
                foreach (var expectedValidationErrorMessage in msgs.Skip(1).Take(msgs.Length-2))
                {
                    StringAssert.StartsWith("Validatie mislukt: ", expectedValidationErrorMessage);
                }
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidPipingCalculation.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Failed, activity.Status);
            Assert.AreEqual(originalOutput, invalidPipingCalculation.Output);
        }

        [Test]
        public void Execute_ValidPipingCalculationAndInitialized_PerformPipingCalculationAndLogStartAndEnd()
        {
            // Setup
            var validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = null;

            var activity = new PipingCalculationActivity(validPipingCalculation);
            activity.Initialize();

            // Call
            Action call = () => activity.Execute();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", validPipingCalculation.Name), msgs.First());
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityStatus.Done, activity.Status);
            Assert.IsNotNull(validPipingCalculation.Output);
        }

        [Test]
        public void OnFinish_ValidPipingCalculationAndExecuted_NotifyObserversOfPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = null;
            validPipingCalculation.Attach(observerMock);

            var activity = new PipingCalculationActivity(validPipingCalculation);
            activity.Initialize();
            activity.Execute();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}