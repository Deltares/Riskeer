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
using Riskeer.Common.Forms.Views;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.Views;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsScenarioRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            var row = new GrassCoverErosionInwardsScenarioRow(calculation);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<GrassCoverErosionInwardsCalculationScenario>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);
        }

        [Test]
        public void Constructor_WithCalculationWithOutput_PropertiesFromCalculation()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call
            var row = new GrassCoverErosionInwardsScenarioRow(calculation);

            // Assert
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.OvertoppingOutput.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);
        }

        [Test]
        public void Constructor_WithCalculationWithoutOutput_PropertiesFromCalculation()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            var row = new GrassCoverErosionInwardsScenarioRow(calculation);

            // Assert
            Assert.IsNaN(row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new GrassCoverErosionInwardsCalculationScenario();

            var row = new GrassCoverErosionInwardsScenarioRow(calculation);

            // Precondition
            Assert.IsNaN(row.FailureProbability);

            // When
            calculation.Output = new TestGrassCoverErosionInwardsOutput();
            row.Update();

            // Then
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.OvertoppingOutput.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var row = new GrassCoverErosionInwardsScenarioRow(calculation);

            // Precondition
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.OvertoppingOutput.Reliability);
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
            var calculation = new GrassCoverErosionInwardsCalculationScenario
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            var row = new GrassCoverErosionInwardsScenarioRow(calculation);

            // Precondition
            ProbabilityAssessmentOutput expectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.OvertoppingOutput.Reliability);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);

            var random = new Random(11);

            // When
            calculation.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()), null, null);
            row.Update();

            // Then
            ProbabilityAssessmentOutput newExpectedDerivedOutput = ProbabilityAssessmentOutputFactory.Create(calculation.Output.OvertoppingOutput.Reliability);
            Assert.AreEqual(newExpectedDerivedOutput.Probability, row.FailureProbability);
        }
    }
}