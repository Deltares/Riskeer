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
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultInitialFailureMechanismResultExtensionsTest
    {
        [Test]
        public void GetInitialFailureMechanismResultProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => GrassCoverErosionInwardsFailureMechanismSectionResultInitialFailureMechanismResultExtensions.GetInitialFailureMechanismResultProbability(
                null, Enumerable.Empty<GrassCoverErosionInwardsCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

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

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios);

            // Assert
            Assert.AreEqual(0.3973850177700996, initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                Enumerable.Empty<GrassCoverErosionInwardsCalculationScenario>());

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            GrassCoverErosionInwardsCalculationScenario calculationScenario = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            calculationScenario.IsRelevant = false;

            GrassCoverErosionInwardsCalculationScenario[] calculationScenarios =
            {
                calculationScenario
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            GrassCoverErosionInwardsCalculationScenario calculationScenario = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateNotCalculatedGrassCoverErosionInwardsCalculationScenario(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(new[]
            {
                calculationScenario
            });

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            GrassCoverErosionInwardsCalculationScenario calculationScenario1 = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateGrassCoverErosionInwardsCalculationScenario(section);
            GrassCoverErosionInwardsCalculationScenario calculationScenario2 = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateNotCalculatedGrassCoverErosionInwardsCalculationScenario(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) contribution1;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;
            calculationScenario2.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN), null, null);

            GrassCoverErosionInwardsCalculationScenario[] calculationScenarios =
            {
                calculationScenario1,
                calculationScenario2
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        public void GetInitialFailureMechanismResultProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            GrassCoverErosionInwardsCalculationScenario calculationScenarioA = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateNotCalculatedGrassCoverErosionInwardsCalculationScenario(section);
            GrassCoverErosionInwardsCalculationScenario calculationScenarioB = GrassCoverErosionInwardsCalculationScenarioTestFactory.CreateNotCalculatedGrassCoverErosionInwardsCalculationScenario(section);
            calculationScenarioA.Contribution = (RoundedDouble) contributionA;
            calculationScenarioB.Contribution = (RoundedDouble) contributionB;

            // Call
            double initialFailureMechanismResultProbability = result.GetInitialFailureMechanismResultProbability(new[]
            {
                calculationScenarioA,
                calculationScenarioB
            });

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }
    }
}