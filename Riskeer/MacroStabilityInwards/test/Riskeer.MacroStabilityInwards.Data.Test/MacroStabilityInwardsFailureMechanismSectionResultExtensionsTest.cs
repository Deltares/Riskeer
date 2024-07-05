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
using Riskeer.MacroStabilityInwards.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionResultExtensionsTest
    {
        [Test]
        public void GetInitialFailureMechanismResultProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsFailureMechanismSectionResultExtensions.GetInitialFailureMechanismResultProbability(
                null, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(), 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(null, 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            const double factorOfStability1 = 1.0 / 10.0;
            const double factorOfStability2 = 1.0 / 20.0;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario1 =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factorOfStability1, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factorOfStability2, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            macroStabilityInwardsCalculationScenario1.Contribution = (RoundedDouble) 0.2111;
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) 0.7889;

            MacroStabilityInwardsCalculationScenario[] calculations =
            {
                macroStabilityInwardsCalculationScenario1,
                macroStabilityInwardsCalculationScenario2,
                macroStabilityInwardsCalculationScenario3
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculations, 1.1);

            // Assert
            Assert.AreEqual(0.99052414832077185, initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(), 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario calculationScenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
            {
                calculationScenario
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(new[]
            {
                macroStabilityInwardsCalculationScenario
            }, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario1 =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section);

            macroStabilityInwardsCalculationScenario1.IsRelevant = true;
            macroStabilityInwardsCalculationScenario1.Contribution = (RoundedDouble) contribution1;

            macroStabilityInwardsCalculationScenario2.IsRelevant = true;
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) contribution2;

            MacroStabilityInwardsCalculationScenario[] calculations =
            {
                macroStabilityInwardsCalculationScenario1,
                macroStabilityInwardsCalculationScenario2
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculations, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-4)]
        public void GetInitialFailureMechanismResultProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenarioA =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenarioB =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section);

            macroStabilityInwardsCalculationScenarioA.Contribution = (RoundedDouble) contributionA;
            macroStabilityInwardsCalculationScenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new AdoptableFailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = result.GetInitialFailureMechanismResultProbability(new[]
            {
                macroStabilityInwardsCalculationScenarioA,
                macroStabilityInwardsCalculationScenarioB
            }, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }
    }
}