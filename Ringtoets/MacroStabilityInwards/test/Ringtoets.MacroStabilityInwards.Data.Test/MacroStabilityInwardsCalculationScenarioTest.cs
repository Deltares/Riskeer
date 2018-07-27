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
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var scenario = new MacroStabilityInwardsCalculationScenario();

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsCalculation>(scenario);
            Assert.IsInstanceOf<ICalculationScenario>(scenario);

            Assert.IsTrue(scenario.IsRelevant);
            Assert.AreEqual(4, scenario.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual((RoundedDouble) 1.0, scenario.Contribution);
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, scenario.Status);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsRelevant_Always_ReturnsSetValue(bool isRelevant)
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario();

            // Call
            scenario.IsRelevant = isRelevant;

            // Assert
            Assert.AreEqual(isRelevant, scenario.IsRelevant);
        }

        [Test]
        public void Contribution_Always_ReturnsSetValue()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble roundedDouble = random.NextRoundedDouble();

            var scenario = new MacroStabilityInwardsCalculationScenario();

            // Call
            scenario.Contribution = roundedDouble;

            // Assert
            Assert.AreEqual(4, scenario.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(roundedDouble, scenario.Contribution, scenario.Contribution.GetAccuracy());
        }

        [Test]
        public void CalculationScenarioStatus_OutputNull_ReturnsStatusNotCalculated()
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario();

            // Call
            CalculationScenarioStatus status = scenario.Status;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void CalculationScenarioStatus_OutputSet_ReturnsStatusDone()
        {
            // Setup
            var scenario = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            // Call
            CalculationScenarioStatus status = scenario.Status;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnsCopiedInstanceWithPropertiesSet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario original = CreateRandomCalculationScenarioWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnsCopiedInstanceWithPropertiesSet()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario original = CreateRandomCalculationScenarioWithoutOutput();
            original.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        private static MacroStabilityInwardsCalculationScenario CreateRandomCalculationScenarioWithoutOutput()
        {
            var random = new Random(21);

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "A Name",
                Comments =
                {
                    Body = "A comment"
                },
                IsRelevant = random.NextBoolean(),
                Contribution = random.NextRoundedDouble()
            };

            MacroStabilityInwardsTestDataGenerator.SetRandomMacroStabilityInwardsInput(calculation.InputParameters);

            return calculation;
        }
    }
}