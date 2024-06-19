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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismExtensionsTest
    {
        [Test]
        public void ScenarioConfigurationTypeIsSemiProbabilistic_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(section);

            // Call
            void Call() => PipingFailureMechanismExtensions.ScenarioConfigurationTypeIsSemiProbabilistic(null, sectionConfiguration);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ScenarioConfigurationTypeIsSemiProbabilistic_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingFailureMechanism().ScenarioConfigurationTypeIsSemiProbabilistic(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetScenarioConfigurations))]
        public void ScenarioConfigurationTypeIsSemiProbabilistic_VariousConfigurations_ReturnsExpectedResult(
            PipingFailureMechanism failureMechanism, PipingFailureMechanismSectionScenarioConfigurationType configurationType,
            bool expectedResult)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            failureMechanism.SetSections(new[]
            {
                section
            }, "APath");

            PipingFailureMechanismSectionConfiguration sectionConfiguration = failureMechanism.SectionConfigurations.Single();
            sectionConfiguration.ScenarioConfigurationType = configurationType;

            // Call
            bool isSemiProbabilistic = failureMechanism.ScenarioConfigurationTypeIsSemiProbabilistic(sectionConfiguration);

            // Assert
            Assert.AreEqual(expectedResult, isSemiProbabilistic);
        }

        [Test]
        public void GetScenarioConfigurationForSection_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            void Call() => PipingFailureMechanismExtensions.GetSectionConfiguration(null, sectionResult);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void GetSectionConfiguration_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            void Call() => failureMechanism.GetSectionConfiguration(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetSectionConfiguration_WithSectionResult_ReturnsCorrespondingConfiguration()
        {
            // Setup
            var sections = new[]
            {
                new FailureMechanismSection("section 1", new[]
                {
                    new Point2D(0, 2),
                    new Point2D(2, 3)
                }),
                new FailureMechanismSection("section 2", new[]
                {
                    new Point2D(2, 3),
                    new Point2D(4, 5)
                }),
                new FailureMechanismSection("section 3", new[]
                {
                    new Point2D(4, 5),
                    new Point2D(2, 3)
                })
            };
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SetSections(sections, "APath");

            var sectionResult = new TestFailureMechanismSectionResult(sections[1]);

            // Call
            PipingFailureMechanismSectionConfiguration sectionConfiguration =
                failureMechanism.GetSectionConfiguration(sectionResult);

            // Assert
            Assert.AreSame(sectionConfiguration.Section, sectionResult.Section);
        }

        private static IEnumerable<TestCaseData> GetScenarioConfigurations()
        {
            var random = new Random(21);
            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.SemiProbabilistic
            }, random.NextEnumValue<PipingFailureMechanismSectionScenarioConfigurationType>(), true);

            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            }, PipingFailureMechanismSectionScenarioConfigurationType.SemiProbabilistic, true);

            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.Probabilistic
            }, random.NextEnumValue<PipingFailureMechanismSectionScenarioConfigurationType>(), false);

            yield return new TestCaseData(new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            }, PipingFailureMechanismSectionScenarioConfigurationType.Probabilistic, false);
        }
    }
}