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
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultInitialFailureMechanismResultExtensionsTest
    {
        [Test]
        public void GetTotalContribution_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionResultInitialFailureMechanismResultExtensions.GetTotalContribution<IPipingCalculationScenario<PipingInput>>(
                null, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>());

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
            void Call() => sectionResult.GetTotalContribution<IPipingCalculationScenario<PipingInput>>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GivenSectionResultWithScenarios_WhenGetTotalContributionForSemiProbabilistic_ThenReturnsTotalRelevantSemiProbabilisticScenarioContribution()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario1.Contribution = (RoundedDouble) 0.3211;

            var pipingCalculationScenario2 =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble) 0.5435;

            SemiProbabilisticPipingCalculationScenario pipingCalculationScenario3 = CreateIrrelevantSemiProbabilisticPipingCalculationScenario(section);
            var pipingCalculationScenario4 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario4.Contribution = (RoundedDouble) 0.1;

            IPipingCalculationScenario<PipingInput>[] calculationScenarios =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2,
                pipingCalculationScenario3,
                pipingCalculationScenario4
            };

            // When
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution<SemiProbabilisticPipingCalculationScenario>(calculationScenarios);

            // Then
            Assert.AreEqual((RoundedDouble) 0.8646, totalContribution);
        }

        [Test]
        public void GivenSectionResultWithScenarios_WhenGetTotalContributionForProbabilistic_ThenReturnsTotalRelevantProbabilisticScenarioContribution()
        {
            // Given
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario1 =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario1.Contribution = (RoundedDouble) 0.3211;

            var pipingCalculationScenario2 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble) 0.1;

            var pipingCalculationScenario3 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenario3.Contribution = (RoundedDouble) 0.4651;

            ProbabilisticPipingCalculationScenario pipingCalculationScenario4 = CreateIrrelevantProbabilisticPipingCalculationScenario(section);

            IPipingCalculationScenario<PipingInput>[] calculationScenarios =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2,
                pipingCalculationScenario3,
                pipingCalculationScenario4
            };

            // When
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution<ProbabilisticPipingCalculationScenario>(calculationScenarios);

            // Then
            Assert.AreEqual((RoundedDouble) 0.5651, totalContribution);
        }

        [Test]
        public void GetCalculationScenarios_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionResultInitialFailureMechanismResultExtensions.GetCalculationScenarios<IPipingCalculationScenario<PipingInput>>(
                null, Enumerable.Empty<IPipingCalculationScenario<PipingInput>>());

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
            void Call() => sectionResult.GetCalculationScenarios<IPipingCalculationScenario<PipingInput>>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationScenariosTestCases))]
        public void GetCalculationScenarios_WithRelevantAndIrrelevantScenarios_ReturnsRelevantCalculationScenarios(
            FailureMechanismSection section,
            IEnumerable<IPipingCalculationScenario<PipingInput>> allScenarios,
            Func<PipingFailureMechanismSectionResult, IEnumerable<IPipingCalculationScenario<PipingInput>>, IEnumerable<IPipingCalculationScenario<PipingInput>>> getRelevantScenariosFunc,
            IPipingCalculationScenario<PipingInput> expectedRelevantScenario)
        {
            // Setup
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            IEnumerable<IPipingCalculationScenario<PipingInput>> relevantScenarios = getRelevantScenariosFunc(sectionResult, allScenarios);

            // Assert
            Assert.AreEqual(expectedRelevantScenario, relevantScenarios.Single());
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
            IEnumerable<IPipingCalculationScenario<PipingInput>> relevantScenarios = sectionResult.GetCalculationScenarios<IPipingCalculationScenario<PipingInput>>(new[]
            {
                calculationScenario
            });

            // Assert
            CollectionAssert.IsEmpty(relevantScenarios);
        }

        private static IEnumerable<TestCaseData> GetCalculationScenariosTestCases()
        {
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            var semiProbabilisticCalculationScenario1 = SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            SemiProbabilisticPipingCalculationScenario semiProbabilisticCalculationScenario2 = CreateIrrelevantSemiProbabilisticPipingCalculationScenario(section);

            var probabilisticCalculationScenario1 = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            ProbabilisticPipingCalculationScenario probabilisticCalculationScenario2 = CreateIrrelevantProbabilisticPipingCalculationScenario(section);

            var allScenarios = new IPipingCalculationScenario<PipingInput>[]
            {
                semiProbabilisticCalculationScenario1,
                semiProbabilisticCalculationScenario2,
                probabilisticCalculationScenario1,
                probabilisticCalculationScenario2
            };

            yield return new TestCaseData(section, allScenarios,
                                          new Func<PipingFailureMechanismSectionResult, IEnumerable<IPipingCalculationScenario<PipingInput>>, IEnumerable<IPipingCalculationScenario<PipingInput>>>(
                                              (result, scenarios) => result.GetCalculationScenarios<SemiProbabilisticPipingCalculationScenario>(scenarios)),
                                          semiProbabilisticCalculationScenario1);
            yield return new TestCaseData(section, allScenarios,
                                          new Func<PipingFailureMechanismSectionResult, IEnumerable<IPipingCalculationScenario<PipingInput>>, IEnumerable<IPipingCalculationScenario<PipingInput>>>(
                                              (result, scenarios) => result.GetCalculationScenarios<ProbabilisticPipingCalculationScenario>(scenarios)),
                                          probabilisticCalculationScenario1);
        }

        private static ProbabilisticPipingCalculationScenario CreateIrrelevantProbabilisticPipingCalculationScenario(FailureMechanismSection section)
        {
            var calculationScenario =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);

            calculationScenario.IsRelevant = false;

            return calculationScenario;
        }

        private static SemiProbabilisticPipingCalculationScenario CreateIrrelevantSemiProbabilisticPipingCalculationScenario(FailureMechanismSection section)
        {
            var calculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            calculationScenario.IsRelevant = false;

            return calculationScenario;
        }

        #region Probabilistic

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionResultInitialFailureMechanismResultExtensions.GetInitialFailureMechanismResultProbability(
                null, Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), scenario => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(null, scenario => null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_GetOutputFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getOutputFunc", exception.ParamName);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario1 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenario2 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenario3 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble) 0.2111;

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble) 0.7889;

            pipingCalculationScenario3.IsRelevant = false;

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2,
                pipingCalculationScenario3
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                calculations, scenario => scenario.Output.ProfileSpecificOutput);

            // Assert
            Assert.AreEqual(0.24284668249632746, initialFailureMechanismResultProbability);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), scenario => scenario.Output.ProfileSpecificOutput);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            ProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                CreateIrrelevantProbabilisticPipingCalculationScenario(section)
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                calculationScenarios, scenario => scenario.Output.ProfileSpecificOutput);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(new[]
            {
                pipingCalculationScenario
            }, scenario => scenario.Output.ProfileSpecificOutput);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            var pipingCalculationScenario1 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenario2 =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble) contribution1;

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble) contribution2;
            pipingCalculationScenario2.Output = new ProbabilisticPipingOutput(new TestPartialProbabilisticPipingOutput(double.NaN, null),
                                                                              new TestPartialProbabilisticPipingOutput(double.NaN, null));

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                calculations, scenario => scenario.Output.ProfileSpecificOutput);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingCalculationScenarioA =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenarioB =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenarioA.Contribution = (RoundedDouble) contributionA;
            pipingCalculationScenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = result.GetInitialFailureMechanismResultProbability(new[]
            {
                pipingCalculationScenarioA,
                pipingCalculationScenarioB
            }, scenario => null);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        #endregion

        #region Semi-probabilistic

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionResultInitialFailureMechanismResultExtensions.GetInitialFailureMechanismResultProbability(
                null, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(), 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(null, 0.1);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
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
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculations, 0.1);

            // Assert
            Assert.AreEqual(4.2467174336864661e-7, initialFailureMechanismResultProbability);
        }

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(), 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            SemiProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                CreateIrrelevantSemiProbabilisticPipingCalculationScenario(section)
            };

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculationScenarios, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario =
                SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(new[]
            {
                pipingCalculationScenario
            }, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_ScenarioWithNaNResults_ReturnsNaN()
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
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(calculations, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
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
            double initialFailureMechanismResultProbability = result.GetInitialFailureMechanismResultProbability(new[]
            {
                pipingCalculationScenarioA,
                pipingCalculationScenarioB
            }, 0.1);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        #endregion
    }
}