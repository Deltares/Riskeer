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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Structures
{
    [TestFixture]
    public class StructuresFailureMechanismSectionResultInitialFailureMechanismResultExtensionsTest
    {
        [Test]
        public void GetInitialFailureMechanismResultProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StructuresFailureMechanismSectionResultInitialFailureMechanismResultExtensions.GetInitialFailureMechanismResultProbability(
                null, Enumerable.Empty<TestStructuresCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetInitialFailureMechanismResultProbability<TestStructuresInput>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

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

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculations);

            // Assert
            Assert.AreEqual(0.039607535209180436, initialFailureMechanismResultProbability, 1e-8);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                Enumerable.Empty<TestStructuresCalculationScenario>());

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            var calculationScenario = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                IsRelevant = false,
                Output = new TestStructuresOutput()
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(new[]
            {
                calculationScenario
            });

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void GetInitialFailureMechanismResultProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            var calculationScenario = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                }
            };

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
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            var calculationScenario1 = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                IsRelevant = true,
                Contribution = (RoundedDouble) 0.2,
                Output = new TestStructuresOutput(double.NaN)
            };
            var calculationScenario2 = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                IsRelevant = true,
                Contribution = (RoundedDouble) 0.8,
                Output = new TestStructuresOutput(0.1)
            };

            TestStructuresCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculations);

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
            var result = new FailureMechanismSectionResult(section);

            var calculationScenarioA = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                Contribution = (RoundedDouble) contributionA,
                Output = new TestStructuresOutput(0.1)
            };
            var calculationScenarioB = new TestStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = new TestStructure(section.StartPoint)
                },
                Contribution = (RoundedDouble) contributionB,
                Output = new TestStructuresOutput(0.2)
            };

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