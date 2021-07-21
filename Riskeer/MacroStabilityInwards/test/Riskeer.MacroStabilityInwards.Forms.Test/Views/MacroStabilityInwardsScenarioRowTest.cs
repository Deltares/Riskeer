// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.Views;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsScenarioRowTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call
            void Call() => new MacroStabilityInwardsScenarioRow(calculation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<MacroStabilityInwardsCalculationScenario>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);
        }

        [Test]
        public void Constructor_WithCalculationWithOutput_PropertiesFromCalculation()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism);

            // Assert
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
        }

        [Test]
        public void Constructor_WithCalculationWithoutOutput_PropertiesFromCalculation()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism);

            // Assert
            Assert.IsNaN(row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism);

            // Precondition
            Assert.IsNaN(row.FailureProbability);

            // When
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();
            row.Update();

            // Then
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput()
            };

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism);

            // Precondition
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.IsNaN(row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputChangedAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput()
            };

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism);

            // Precondition
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);

            var random = new Random(11);

            // When
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = random.NextDouble()
            });
            row.Update();

            // Then
            DerivedMacroStabilityInwardsOutput newExpectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(newExpectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
        }
    }
}