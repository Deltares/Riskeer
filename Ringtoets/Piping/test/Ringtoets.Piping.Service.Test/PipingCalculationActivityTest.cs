// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput());

            // Call
            var activity = new PipingCalculationActivity(calculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual(calculation.Name, activity.Name);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void ParameteredConstructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculationActivity(null, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_PipingProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput());

            // Call
            TestDelegate call = () => new PipingCalculationActivity(calculation, null, int.MinValue, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("pipingProbabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        public void Run_InvalidPipingCalculationWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var originalOutput = new TestPipingOutput();
            var originalSemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput();

            PipingCalculationScenario invalidPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithInvalidInput();
            invalidPipingCalculation.Output = originalOutput;
            invalidPipingCalculation.SemiProbabilisticOutput = originalSemiProbabilisticOutput;

            var activity = new PipingCalculationActivity(invalidPipingCalculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", invalidPipingCalculation.Name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: ", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[2]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[3]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[4]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[5]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", invalidPipingCalculation.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.AreSame(originalOutput, invalidPipingCalculation.Output);
            Assert.AreSame(originalSemiProbabilisticOutput, invalidPipingCalculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Run_ValidPipingCalculation_PerformPipingValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            PipingCalculationScenario validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            validPipingCalculation.Output = null;
            validPipingCalculation.SemiProbabilisticOutput = null;

            double norm = new Random(21).NextDouble();
            var activity = new PipingCalculationActivity(validPipingCalculation, new PipingProbabilityAssessmentInput(), norm, double.NaN);
            activity.Run();

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", validPipingCalculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", validPipingCalculation.Name), msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.IsNotNull(validPipingCalculation.Output);
            Assert.IsNotNull(validPipingCalculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Finish_ValidPipingCalculationAndRan_NotifyObserversOfPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            PipingCalculationScenario validPipingCalculation = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            validPipingCalculation.Output = null;
            validPipingCalculation.SemiProbabilisticOutput = null;
            validPipingCalculation.Attach(observerMock);

            var activity = new PipingCalculationActivity(validPipingCalculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}