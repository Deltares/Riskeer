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

using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class ProbabilisticPipingScenarioRowTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario();

            // Call
            var row = new ProbabilisticPipingScenarioRow(calculation);

            // Assert
            Assert.IsInstanceOf<PipingScenarioRow<ProbabilisticPipingCalculationScenario>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);
        }

        [Test]
        public void Constructor_CalculationWithOutput_ExpectedValues()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
            };

            // Call
            var row = new ProbabilisticPipingScenarioRow(calculation);

            // Assert
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculation.Output.ProfileSpecificOutput.Reliability), row.FailureProbability);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculation.Output.SectionSpecificOutput.Reliability), row.SectionFailureProbability);
        }

        [Test]
        public void Constructor_CalculationWithoutOutput_ExpectedValues()
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario();

            // Call
            var row = new ProbabilisticPipingScenarioRow(calculation);

            // Assert
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.SectionFailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new ProbabilisticPipingCalculationScenario();
            var row = new ProbabilisticPipingScenarioRow(calculation);

            // Precondition
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.SectionFailureProbability);

            // When
            calculation.Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints();

            // Then
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculation.Output.ProfileSpecificOutput.Reliability), row.FailureProbability);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculation.Output.SectionSpecificOutput.Reliability), row.SectionFailureProbability);
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
            };
            var row = new ProbabilisticPipingScenarioRow(calculation);

            // Precondition
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculation.Output.ProfileSpecificOutput.Reliability), row.FailureProbability);
            Assert.AreEqual(StatisticsConverter.ReliabilityToProbability(calculation.Output.SectionSpecificOutput.Reliability), row.SectionFailureProbability);

            // When
            calculation.Output = null;

            // Then
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.SectionFailureProbability);
        }
    }
}