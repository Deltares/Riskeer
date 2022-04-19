﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingCalculationScenarioTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var scenario = new SemiProbabilisticPipingCalculationScenario();

            // Assert
            Assert.IsInstanceOf<SemiProbabilisticPipingCalculation>(scenario);
            Assert.IsInstanceOf<IPipingCalculationScenario<SemiProbabilisticPipingInput>>(scenario);

            Assert.IsTrue(scenario.IsRelevant);
            Assert.AreEqual(4, scenario.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, scenario.Contribution, scenario.Contribution.GetAccuracy());
        }

        [Test]
        [TestCaseSource(typeof(CalculationScenarioTestHelper), nameof(CalculationScenarioTestHelper.GetInvalidScenarioContributionValues))]
        public void Contribution_SetInvalidValue_ThrowArgumentException(double newValue)
        {
            // Setup
            var calculationScenario = new SemiProbabilisticPipingCalculationScenario();

            // Call
            void Call() => calculationScenario.Contribution = (RoundedDouble) newValue;

            // Assert
            const string expectedMessage = "De waarde voor de bijdrage moet binnen het bereik [0, 100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCaseSource(typeof(CalculationScenarioTestHelper), nameof(CalculationScenarioTestHelper.GetValidScenarioContributionValues))]
        public void Contribution_SetValidValue_ValueSet(double newValue)
        {
            // Setup
            var calculationScenario = new SemiProbabilisticPipingCalculationScenario();

            // Call
            calculationScenario.Contribution = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(4, calculationScenario.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(newValue, calculationScenario.Contribution, calculationScenario.Contribution.GetAccuracy());
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            SemiProbabilisticPipingCalculationScenario original = CreateRandomCalculationScenarioWithoutOutput();

            original.Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            SemiProbabilisticPipingCalculationScenario original = CreateRandomCalculationScenarioWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, PipingCloneAssert.AreClones);
        }

        private static SemiProbabilisticPipingCalculationScenario CreateRandomCalculationScenarioWithoutOutput()
        {
            var random = new Random(21);

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                Comments =
                {
                    Body = "Random body"
                },
                Name = "Random name",
                IsRelevant = random.NextBoolean(),
                Contribution = random.NextRoundedDouble()
            };

            PipingTestDataGenerator.SetRandomDataToPipingInput(calculation.InputParameters);

            return calculation;
        }
    }
}