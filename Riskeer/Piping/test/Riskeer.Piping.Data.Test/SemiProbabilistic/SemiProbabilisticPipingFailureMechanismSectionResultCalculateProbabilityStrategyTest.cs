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
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test.SemiProbabilistic
{
    [TestFixture]
    public class SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategyTest
    {
        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                null, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(), new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section), null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var strategy = new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                new AdoptableFailureMechanismSectionResult(section), Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(), assessmentSection);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateProfileProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var calculationScenario1 = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var calculationScenario3 = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) 0.2111;
            calculationScenario1.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(1.1, 2.2, 3.3);

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) 0.7889;
            calculationScenario2.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(4.4, 5.5, 6.6);

            calculationScenario3.IsRelevant = false;

            SemiProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            var strategy = new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations, new PipingFailureMechanism(), new AssessmentSectionStub());

            // Call
            double profileProbability = strategy.CalculateProfileProbability();

            // Assert
            Assert.AreEqual(9.2969543564512289E-16, profileProbability);
        }

        [Test]
        public void CalculateProfileProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var strategy = new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(), new AssessmentSectionStub());

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

            SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy strategy = CreateStrategyForMultipleScenarios(section);

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.AreEqual(0.98862377, sectionProbability, 1e-8);
        }

        [Test]
        public void CalculateSectionProbability_MultipleScenariosForSectionWithLargeLength_ReturnsProbabilityEqualToOne()
        {
            // Setup
            var section = new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0),
                new Point2D(10000, 0)
            });

            SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy strategy = CreateStrategyForMultipleScenarios(section);

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

            var strategy = new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(), new AssessmentSectionStub());

            // Call
            double sectionProbability = strategy.CalculateSectionProbability();

            // Assert
            Assert.IsNaN(sectionProbability);
        }

        private static SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy CreateStrategyForMultipleScenarios(FailureMechanismSection section)
        {
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            var calculationScenario1 = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var calculationScenario2 = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);
            var calculationScenario3 = SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) 0.2111;
            calculationScenario1.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(0.01, 0.02, 0.03);

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) 0.7889;
            calculationScenario2.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(0.04, 0.05, 0.06);

            calculationScenario3.IsRelevant = false;

            SemiProbabilisticPipingCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            return new SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy(
                sectionResult, calculations, new PipingFailureMechanism(), new AssessmentSectionStub());
        }
    }
}