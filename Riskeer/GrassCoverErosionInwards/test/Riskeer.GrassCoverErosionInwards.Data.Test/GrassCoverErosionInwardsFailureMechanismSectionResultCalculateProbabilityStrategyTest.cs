// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategyTest
    {
        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                null, Enumerable.Empty<GrassCoverErosionInwardsCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var strategy = new GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), Enumerable.Empty<GrassCoverErosionInwardsCalculationScenario>());

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
        }

        [Test]
        public void CalculateSectionProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            GrassCoverErosionInwardsCalculationScenario calculationScenario1 = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            GrassCoverErosionInwardsCalculationScenario calculationScenario2 = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            GrassCoverErosionInwardsCalculationScenario calculationScenario3 = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) 0.2111;
            calculationScenario1.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(1.1), null, null);

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) 0.7889;
            calculationScenario1.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(2.2), null, null);

            calculationScenario3.IsRelevant = false;

            GrassCoverErosionInwardsCalculationScenario[] calculationScenarios =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            var strategy = new GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculationScenarios);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.AreEqual(0.3973850177700996, sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var strategy = new GrassCoverErosionInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<GrassCoverErosionInwardsCalculationScenario>());

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }
    }
}