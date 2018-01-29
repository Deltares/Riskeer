﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Ringtoets.MacroStabilityInwards.Service.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationActivityTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation();

            // Call
            var activity = new MacroStabilityInwardsCalculationActivity(calculation, RoundedDouble.NaN);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}'", activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculationActivity(null, RoundedDouble.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Run_InvalidMacroStabilityInwardsCalculationWithOutput_LogValidationStartAndEndWithErrors()
        {
            // Setup
            MacroStabilityInwardsOutput originalOutput = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            MacroStabilityInwardsCalculationScenario invalidMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();
            invalidMacroStabilityInwardsCalculation.Output = originalOutput;

            var activity = new MacroStabilityInwardsCalculationActivity(invalidMacroStabilityInwardsCalculation, GetTestNormativeAssessmentLevel());

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
            {
                Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                Assert.AreEqual(6, msgs.Length);
                Assert.AreEqual($"Uitvoeren van berekening '{invalidMacroStabilityInwardsCalculation.Name}' is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                Assert.AreEqual(Level.Error, tupleArray[2].Item2);
                Assert.AreEqual(Level.Error, tupleArray[3].Item2);
                Assert.AreEqual(Level.Error, tupleArray[4].Item2);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[5]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.AreSame(originalOutput, invalidMacroStabilityInwardsCalculation.Output);
        }

        [Test]
        public void Run_ValidMacroStabilityInwardsCalculation_PerformMacroStabilityInwardsValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario validMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            validMacroStabilityInwardsCalculation.Output = null;

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                var activity = new MacroStabilityInwardsCalculationActivity(validMacroStabilityInwardsCalculation, GetTestNormativeAssessmentLevel());
                activity.Run();

                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(5, msgs.Length);
                    Assert.AreEqual($"Uitvoeren van berekening '{validMacroStabilityInwardsCalculation.Name}' is gestart.", msgs[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[4]);
                });
                Assert.AreEqual(ActivityState.Executed, activity.State);
                Assert.IsNotNull(validMacroStabilityInwardsCalculation.Output);
            }
        }

        [Test]
        public void Finish_ValidMacroStabilityInwardsCalculationAndRan_NotifyObserversOfMacroStabilityInwardsCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            MacroStabilityInwardsCalculationScenario validMacroStabilityInwardsCalculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            validMacroStabilityInwardsCalculation.Output = null;
            validMacroStabilityInwardsCalculation.Attach(observer);

            var activity = new MacroStabilityInwardsCalculationActivity(validMacroStabilityInwardsCalculation, GetTestNormativeAssessmentLevel());

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            mocks.VerifyAll();
        }

        private static RoundedDouble GetTestNormativeAssessmentLevel()
        {
            return (RoundedDouble) 1.1;
        }
    }
}