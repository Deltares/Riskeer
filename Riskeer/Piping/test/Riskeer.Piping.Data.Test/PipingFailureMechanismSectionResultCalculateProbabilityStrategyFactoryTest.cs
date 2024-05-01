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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactoryTest
    {
        [Test]
        public void CreateCalculateStrategy_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(
                null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculateStrategy_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableFailureMechanismSectionResult(section);

            // Call
            void Call() => PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(
                sectionResult, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateCalculateStrategy_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(
                sectionResult, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetSemiProbabilisticConfigurations))]
        public void CreateCalculateStrategy_WithSemiProbabilisticConfigurations_ReturnsExpectedStrategy(
            PipingFailureMechanism failureMechanism, PipingScenarioConfigurationPerFailureMechanismSectionType configurationType)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            failureMechanism.SetSections(new[]
            {
                section
            }, "APath");

            PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection =
                failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.Single();
            scenarioConfigurationPerFailureMechanismSection.ScenarioConfigurationType = configurationType;

            // Call
            IFailureMechanismSectionResultCalculateProbabilityStrategy strategy =
                PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(
                    failureMechanism.SectionResults.Single(), failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<SemiProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
        }

        [Test]
        [TestCaseSource(nameof(GetProbabilisticConfigurations))]
        public void CreateCalculateStrategy_WithProbabilisticConfigurations_ReturnsExpectedStrategy(
            PipingFailureMechanism failureMechanism, PipingScenarioConfigurationPerFailureMechanismSectionType configurationType)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            failureMechanism.SetSections(new[]
            {
                section
            }, "APath");

            PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerFailureMechanismSection =
                failureMechanism.ScenarioConfigurationsPerFailureMechanismSection.Single();
            scenarioConfigurationPerFailureMechanismSection.ScenarioConfigurationType = configurationType;

            // Call
            IFailureMechanismSectionResultCalculateProbabilityStrategy strategy =
                PipingFailureMechanismSectionResultCalculateProbabilityStrategyFactory.CreateCalculateStrategy(
                    failureMechanism.SectionResults.Single(), failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ProbabilisticPipingFailureMechanismSectionResultCalculateProbabilityStrategy>(strategy);
        }

        private static IEnumerable<TestCaseData> GetSemiProbabilisticConfigurations()
        {
            var random = new Random(21);
            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.SemiProbabilistic
            }, random.NextEnumValue<PipingScenarioConfigurationPerFailureMechanismSectionType>());

            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            }, PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic);
        }

        private static IEnumerable<TestCaseData> GetProbabilisticConfigurations()
        {
            var random = new Random(21);
            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.Probabilistic
            }, random.NextEnumValue<PipingScenarioConfigurationPerFailureMechanismSectionType>());

            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            }, PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic);
        }
    }
}