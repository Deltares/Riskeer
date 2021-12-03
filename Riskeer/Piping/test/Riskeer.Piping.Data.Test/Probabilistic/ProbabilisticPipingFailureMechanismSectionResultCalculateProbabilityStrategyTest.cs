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
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;

namespace Riskeer.Piping.Data.Test.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategyTest
    {
        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                null, Enumerable.Empty<ProbabilisticPipingCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                new PipingFailureMechanismSectionResult(section), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                new PipingFailureMechanismSectionResult(section), Enumerable.Empty<ProbabilisticPipingCalculationScenario>());

            // Assert
            Assert.IsInstanceOf<IPipingFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
        }
        
        #region CalculateProfileProbability

        [Test]
        public void CalculateProfileProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario1 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario3 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) 0.2111;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) 0.7889;

            calculationScenario3.IsRelevant = false;

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(sectionResult, calculations);

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.AreEqual(0.24284668249632746, profileProbability);
        }

        [Test]
        public void CalculateProfileProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<ProbabilisticPipingCalculationScenario>());

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }

        [Test]
        public void CalculateProfileProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            calculationScenario.IsRelevant = false;

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, new[]
                {
                    calculationScenario
                });

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }

        [Test]
        public void CalculateProfileProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(
                section);

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, new[]
                {
                    calculationScenario
                });

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }

        [Test]
        public void CalculateProfileProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            var calculationScenario1 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) contribution1;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;
            calculationScenario2.Output = new ProbabilisticPipingOutput(new TestPartialProbabilisticPipingOutput(double.NaN, null),
                                                                        new TestPartialProbabilisticPipingOutput(double.NaN, null));

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2
            };

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations);

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        public void CalculateProfileProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contribution1, double contribution2)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario1 = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            calculationScenario1.Contribution = (RoundedDouble) contribution1;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2
            };

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations);

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.IsNaN(profileProbability);
        }
        
        #endregion
        
        #region CalculateSectionProbability

        [Test]
        public void CalculateSectionProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario1 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario3 = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) 0.2111;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) 0.7889;

            calculationScenario3.IsRelevant = false;

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(sectionResult, calculations);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.AreEqual(0.24284668249632746, sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<ProbabilisticPipingCalculationScenario>());

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario = ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            calculationScenario.IsRelevant = false;

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, new[]
                {
                    calculationScenario
                });

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(
                section);

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, new[]
                {
                    calculationScenario
                });

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }

        [Test]
        public void CalculateSectionProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            var calculationScenario1 =
                ProbabilisticPipingCalculationTestFactory.CreateCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 =
                ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) contribution1;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;
            calculationScenario2.Output = new ProbabilisticPipingOutput(new TestPartialProbabilisticPipingOutput(double.NaN, null),
                                                                        new TestPartialProbabilisticPipingOutput(double.NaN, null));

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2
            };

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }

        [Test]
        [TestCase(0.0, 0.0)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        public void CalculateSectionProbability_RelevantScenarioContributionsDoNotAddUpTo1_ReturnNaN(double contribution1, double contribution2)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenario1 = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 = ProbabilisticPipingCalculationTestFactory.CreateNotCalculatedCalculation<ProbabilisticPipingCalculationScenario>(section);
            calculationScenario1.Contribution = (RoundedDouble) contribution1;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;

            ProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2
            };

            var strategy = new ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }
        
        #endregion
    }
}