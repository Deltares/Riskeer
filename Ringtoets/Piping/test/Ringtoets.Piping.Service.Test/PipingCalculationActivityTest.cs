﻿using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Service;
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
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityStatus.None, activity.Status);
        }

        [Test]
        public void Run_InvalidPipingCalculationWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var originalOutput = new TestPipingOutput();

            var invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidPipingCalculation.Output = originalOutput;

            var activity = new PipingCalculationActivity(invalidPipingCalculation);

            // Call
            Action call = () => activity.Run();

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
        public void Run_ValidPipingCalculation_PerformPipingValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = null;

            var activity = new PipingCalculationActivity(validPipingCalculation);
            activity.Run();

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToList();
                Assert.AreEqual(4, msgs.Count);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validPipingCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs[1]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", validPipingCalculation.Name), msgs[2]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityStatus.Executed, activity.Status);
            Assert.IsNotNull(validPipingCalculation.Output);
        }

        [Test]
        public void Finish_ValidPipingCalculationAndRan_NotifyObserversOfPipingCalculation()
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

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}