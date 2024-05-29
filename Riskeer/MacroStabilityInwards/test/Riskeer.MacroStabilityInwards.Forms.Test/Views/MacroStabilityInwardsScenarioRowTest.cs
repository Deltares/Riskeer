// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
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

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            // Call
            void Call() => new MacroStabilityInwardsScenarioRow(calculation, null, sectionConfiguration);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            void Call() => new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, sectionConfiguration);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<MacroStabilityInwardsCalculationScenario>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityInwardsScenarioRow.SectionFailureProbability));
        }

        [Test]
        [TestCase(1)]
        [TestCase(100)]
        public void Constructor_WithCalculationWithOutput_PropertiesFromCalculation(double sectionLength)
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties
                {
                    FactorOfStability = 0.1
                })
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0),
                new Point2D(sectionLength, 0)
            });
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, sectionConfiguration);

            // Assert
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
            Assert.AreEqual(Math.Min(1.0, expectedDerivedOutput.MacroStabilityInwardsProbability * sectionConfiguration.GetN(failureMechanism.GeneralInput.B)),
                            row.SectionFailureProbability);
        }

        [Test]
        public void Constructor_WithCalculationWithoutOutput_PropertiesFromCalculation()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, sectionConfiguration);

            // Assert
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.SectionFailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, sectionConfiguration);

            // Precondition
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.SectionFailureProbability);

            // When
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();
            row.Update();

            // Then
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability * sectionConfiguration.GetN(failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput()
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, sectionConfiguration);

            // Precondition
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability * sectionConfiguration.GetN(failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.SectionFailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputChangedAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput()
            };
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, sectionConfiguration);

            // Precondition
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(calculation.Output, failureMechanism.GeneralInput.ModelFactor);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.MacroStabilityInwardsProbability * sectionConfiguration.GetN(failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);

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
            Assert.AreEqual(newExpectedDerivedOutput.MacroStabilityInwardsProbability * sectionConfiguration.GetN(failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);
        }
    }
}