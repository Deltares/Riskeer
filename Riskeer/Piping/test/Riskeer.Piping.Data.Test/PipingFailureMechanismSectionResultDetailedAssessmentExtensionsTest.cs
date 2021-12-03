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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultDetailedAssessmentExtensionsTest
    {
        [Test]
        public void GetDetailedAssessmentProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionResultDetailedAssessmentExtensions.GetDetailedAssessmentProbability(
                null, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(), 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetDetailedAssessmentProbability(null, 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenario2 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenario3 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble) 0.2111;
            pipingCalculationScenario1.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(1.1, 2.2, 3.3);

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble) 0.7889;
            pipingCalculationScenario2.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(4.4, 5.5, 6.6);

            pipingCalculationScenario3.IsRelevant = false;

            SemiProbabilisticPipingCalculationScenario[] calculations =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2,
                pipingCalculationScenario3
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculations, 0.1);

            // Assert
            Assert.AreEqual(4.2467174336864661e-7, detailedAssessmentProbability);
        }

        [Test]
        public void GetDetailedAssessmentProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(), 0.1);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
        }

        [Test]
        public void GetDetailedAssessmentProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            SemiProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                CreateIrrelevantPipingCalculationScenario(section)
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculationScenarios, 0.1);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
        }

        [Test]
        public void GetDetailedAssessmentProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(new[]
            {
                pipingCalculationScenario
            }, 0.1);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
        }

        [Test]
        public void GetDetailedAssessmentProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            var pipingCalculationScenario1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenario2 =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble) contribution1;

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble) contribution2;
            pipingCalculationScenario2.Output = new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties());

            SemiProbabilisticPipingCalculationScenario[] calculations =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculations, 0.1);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        public void GetDetailedAssessmentProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingCalculationScenarioA =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenarioB =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenarioA.Contribution = (RoundedDouble) contributionA;
            pipingCalculationScenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            double detailedAssessmentProbability = result.GetDetailedAssessmentProbability(new[]
            {
                pipingCalculationScenarioA,
                pipingCalculationScenarioB
            }, 0.1);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
        }

        [Test]
        public void GetTotalContribution_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((PipingFailureMechanismSectionResult) null).GetTotalContribution(Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => sectionResult.GetTotalContribution(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_WithScenarios_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario1.Contribution = (RoundedDouble) 0.3211;

            var pipingCalculationScenario2 =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble) 0.5435;

            SemiProbabilisticPipingCalculationScenario pipingCalculationScenario3 = CreateIrrelevantPipingCalculationScenario(section);

            SemiProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2,
                pipingCalculationScenario3
            };

            // Call
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution(calculationScenarios);

            // Assert
            Assert.AreEqual((RoundedDouble) 0.8646, totalContribution);
        }

        [Test]
        public void GetCalculationScenarios_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((PipingFailureMechanismSectionResult) null).GetCalculationScenarios(Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarios_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => sectionResult.GetCalculationScenarios(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarios_WithRelevantAndIrrelevantScenarios_ReturnsRelevantCalculationScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);
            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            SemiProbabilisticPipingCalculationScenario calculationScenario2 = CreateIrrelevantPipingCalculationScenario(section);

            // Call
            IEnumerable<SemiProbabilisticPipingCalculationScenario> relevantScenarios = sectionResult.GetCalculationScenarios(new[]
            {
                calculationScenario,
                calculationScenario2
            });

            // Assert
            Assert.AreEqual(calculationScenario, relevantScenarios.Single());
        }

        [Test]
        public void GetCalculationScenarios_WithoutScenarioIntersectingSection_ReturnsNoCalculationScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(999, 999),
                new Point2D(998, 998)
            });
            var sectionResult = new PipingFailureMechanismSectionResult(section);
            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            IEnumerable<SemiProbabilisticPipingCalculationScenario> relevantScenarios = sectionResult.GetCalculationScenarios(new[]
            {
                calculationScenario
            });

            // Assert
            CollectionAssert.IsEmpty(relevantScenarios);
        }

        private static SemiProbabilisticPipingCalculationScenario CreateIrrelevantPipingCalculationScenario(FailureMechanismSection section)
        {
            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            calculationScenario.IsRelevant = false;

            return calculationScenario;
        }
    }
}