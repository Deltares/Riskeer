// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationActivityTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput());

            // Call
            var activity = new MacroStabilityInwardsCalculationActivity(calculation, new MacroStabilityInwardsProbabilityAssessmentInput(), int.MinValue, double.NaN);

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
            TestDelegate call = () => new MacroStabilityInwardsCalculationActivity(null, new MacroStabilityInwardsProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_MacroStabilityInwardsProbabilityAssessmentInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput());

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculationActivity(calculation, null, int.MinValue, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("macroStabilityInwardsProbabilityAssessmentInput", exception.ParamName);
        }

        [Test]
        public void Run_InvalidMacroStabilityInwardsCalculationWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            var originalOutput = new TestMacroStabilityInwardsOutput();
            var originalSemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput();

            MacroStabilityInwardsCalculationScenario invalidMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
            invalidMacroStabilityInwardsCalculation.Output = originalOutput;
            invalidMacroStabilityInwardsCalculation.SemiProbabilisticOutput = originalSemiProbabilisticOutput;

            var activity = new MacroStabilityInwardsCalculationActivity(invalidMacroStabilityInwardsCalculation, new MacroStabilityInwardsProbabilityAssessmentInput(), int.MinValue, double.NaN);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(5, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", invalidMacroStabilityInwardsCalculation.Name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: ", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[2]);
                StringAssert.StartsWith("Validatie mislukt: ", msgs[3]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", invalidMacroStabilityInwardsCalculation.Name), msgs.Last());
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.AreSame(originalOutput, invalidMacroStabilityInwardsCalculation.Output);
            Assert.AreSame(originalSemiProbabilisticOutput, invalidMacroStabilityInwardsCalculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Run_ValidMacroStabilityInwardsCalculation_PerformMacroStabilityInwardsValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario validMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            validMacroStabilityInwardsCalculation.Output = null;
            validMacroStabilityInwardsCalculation.SemiProbabilisticOutput = null;

            double norm = new Random(21).NextDouble();
            var activity = new MacroStabilityInwardsCalculationActivity(validMacroStabilityInwardsCalculation, new MacroStabilityInwardsProbabilityAssessmentInput(), norm, double.NaN);
            activity.Run();

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", validMacroStabilityInwardsCalculation.Name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", validMacroStabilityInwardsCalculation.Name), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", validMacroStabilityInwardsCalculation.Name), msgs[2]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", validMacroStabilityInwardsCalculation.Name), msgs[3]);
            });
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.IsNotNull(validMacroStabilityInwardsCalculation.Output);
            Assert.IsNotNull(validMacroStabilityInwardsCalculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Finish_ValidMacroStabilityInwardsCalculationAndRan_NotifyObserversOfMacroStabilityInwardsCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            MacroStabilityInwardsCalculationScenario validMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            validMacroStabilityInwardsCalculation.Output = null;
            validMacroStabilityInwardsCalculation.SemiProbabilisticOutput = null;
            validMacroStabilityInwardsCalculation.Attach(observerMock);

            var activity = new MacroStabilityInwardsCalculationActivity(validMacroStabilityInwardsCalculation, new MacroStabilityInwardsProbabilityAssessmentInput(), int.MinValue, double.NaN);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }
    }
}