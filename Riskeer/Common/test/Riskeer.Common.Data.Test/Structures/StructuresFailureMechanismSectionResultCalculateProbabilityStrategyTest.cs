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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresFailureMechanismSectionResultCalculateProbabilityStrategyTest
    {
        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TestStructuresInput>(
                null, Enumerable.Empty<TestStructuresCalculationScenario>());

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
            void Call() => new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TestStructuresInput>(
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
            var strategy = new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TestStructuresInput>(
                new AdoptableFailureMechanismSectionResult(section), Enumerable.Empty<TestStructuresCalculationScenario>());

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
        }

        [Test]
        public void CalculateSectionProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            var calculationScenario1 = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                Contribution = (RoundedDouble) 0.2111,
                Output = new TestStructuresOutput(1.1)
            };
            var calculationScenario2 = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                Contribution = (RoundedDouble) 0.7889,
                Output = new TestStructuresOutput(2.2)
            };
            var calculationScenario3 = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                IsRelevant = false
            };

            TestStructuresCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            var strategy = new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TestStructuresInput>(
                failureMechanismSectionResult, calculations);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.AreEqual(0.039607535209180436, sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var strategy = new StructuresFailureMechanismSectionResultCalculateProbabilityStrategy<TestStructuresInput>(
                sectionResult, Enumerable.Empty<TestStructuresCalculationScenario>());

            // Call
            double profileProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }
    }
}