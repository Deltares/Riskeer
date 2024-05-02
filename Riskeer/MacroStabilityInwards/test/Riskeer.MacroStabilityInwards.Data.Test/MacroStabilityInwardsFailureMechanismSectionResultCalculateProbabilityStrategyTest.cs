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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategyTest
    {
        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                null, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(), new MacroStabilityInwardsFailureMechanism());

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
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), null, new MacroStabilityInwardsFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section),
                Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var strategy = new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
        }

        [Test]
        public void CalculateProfileProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

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

            var strategy = new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations, new MacroStabilityInwardsFailureMechanism());

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.AreEqual(0.99012835, profileProbability, 1e-8);
        }

        [Test]
        public void CalculateProfileProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var strategy = new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                new MacroStabilityInwardsFailureMechanism());

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }

        [Test]
        public void CalculateSectionProbability_MultipleScenariosForSectionWithSmallLength_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            });

            MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy strategy = CreateStrategyForMultipleScenarios(section);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.AreEqual(0.99078184, sectionProbability, 1e-8);
        }

        [Test]
        public void CalculateSectionProbability_MultipleScenariosForSectionWithLargeLength_ReturnsProbabilityEqualToOne()
        {
            // Setup
            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0),
                new Point2D(100, 0)
            });

            MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy strategy = CreateStrategyForMultipleScenarios(section);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.AreEqual(1.0, sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var strategy = new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                new MacroStabilityInwardsFailureMechanism());

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }

        private static MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy CreateStrategyForMultipleScenarios(FailureMechanismSection section)
        {
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

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

            return new MacroStabilityInwardsFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations, new MacroStabilityInwardsFailureMechanism());
        }
    }
}