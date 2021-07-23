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
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Forms.Views;

namespace Riskeer.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresScenarioRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call
            var row = new StabilityPointStructuresScenarioRow(calculation);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<StructuresCalculationScenario<StabilityPointStructuresInput>>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);
        }

        [Test]
        public void Constructor_WithCalculationWithOutput_PropertiesFromCalculation()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            // Call
            var row = new StabilityPointStructuresScenarioRow(calculation);

            // Assert
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);
        }

        [Test]
        public void Constructor_WithCalculationWithoutOutput_PropertiesFromCalculation()
        {
            // Setup
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            // Call
            var row = new StabilityPointStructuresScenarioRow(calculation);

            // Assert
            Assert.IsNaN(row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>();

            var row = new StabilityPointStructuresScenarioRow(calculation);

            // Precondition
            Assert.IsNaN(row.FailureProbability);

            // When
            calculation.Output = new TestStructuresOutput();
            row.Update();

            // Then
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var row = new StabilityPointStructuresScenarioRow(calculation);

            // Precondition
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);

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
            var calculation = new StructuresCalculationScenario<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var row = new StabilityPointStructuresScenarioRow(calculation);

            // Precondition
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);

            var random = new Random(11);

            // When
            calculation.Output = new TestStructuresOutput(random.NextDouble());
            row.Update();

            // Then
            ProbabilityAssessmentOutput newExpectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.Reliability);
            Assert.AreEqual(newExpectedDerivedOutput.Probability, row.FailureProbability);
        }
    }
}