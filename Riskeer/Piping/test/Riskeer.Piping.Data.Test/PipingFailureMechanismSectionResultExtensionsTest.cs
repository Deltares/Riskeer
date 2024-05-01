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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultExtensionsTest
    {
        #region Probabilistic

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionResultExtensions.GetInitialFailureMechanismResultProbability(
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
            var failureMechanismSectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            // Call
            double initialFailureMechanismResultProbability = failureMechanismSectionResult.GetInitialFailureMechanismResultProbability(
                Enumerable.Empty<ProbabilisticPipingCalculationScenario>(), scenario => scenario.Output.ProfileSpecificOutput);

            // Assert
            Assert.IsNaN(initialFailureMechanismResultProbability);
        }

        [Test]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            var calculationScenario = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            calculationScenario.IsRelevant = false;

            ProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                calculationScenario
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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
        [TestCase(0.3, 0.7 + 1e-4)]
        public void ProbabilisticGetInitialFailureMechanismResultProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingCalculationScenarioA =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenarioB =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenarioA.Contribution = (RoundedDouble) contributionA;
            pipingCalculationScenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new AdoptableFailureMechanismSectionResult(section);

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
            void Call() => PipingFailureMechanismSectionResultExtensions.GetInitialFailureMechanismResultProbability(
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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

            var calculationScenario = SemiProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            calculationScenario.IsRelevant = false;

            SemiProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                calculationScenario
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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
            var failureMechanismSectionResult = new AdoptableFailureMechanismSectionResult(section);

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
        [TestCase(0.3, 0.7 + 1e-4)]
        public void SemiProbabilisticGetInitialFailureMechanismResultProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingCalculationScenarioA =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var pipingCalculationScenarioB =
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            pipingCalculationScenarioA.Contribution = (RoundedDouble) contributionA;
            pipingCalculationScenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new AdoptableFailureMechanismSectionResult(section);

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