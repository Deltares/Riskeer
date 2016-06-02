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
        public void Run_InvalidPipingCalculationWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var originalOutput = new TestPipingOutput();

            var invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidPipingCalculation.Output = originalOutput;

            var activity = new PipingCalculationActivity(invalidPipingCalculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidPipingCalculation.Name), msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[2]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidPipingCalculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.AreEqual(originalOutput, invalidPipingCalculation.Output);
        }

        [Test]
        public void Run_ValidPipingCalculation_PerformPipingValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            var validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = null;

            var activity = new PipingCalculationActivity(validPipingCalculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);
            activity.Run();

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validPipingCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs[1]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", validPipingCalculation.Name), msgs[2]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", validPipingCalculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
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

            var activity = new PipingCalculationActivity(validPipingCalculation, new PipingProbabilityAssessmentInput(), int.MinValue, double.NaN);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}