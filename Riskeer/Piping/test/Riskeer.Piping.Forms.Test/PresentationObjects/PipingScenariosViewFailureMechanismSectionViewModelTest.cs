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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.PresentationObjects;

namespace Riskeer.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingScenariosViewFailureMechanismSectionViewModelTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingScenariosViewFailureMechanismSectionViewModel(
                null, new PipingFailureMechanism(), new PipingScenarioConfigurationPerFailureMechanismSection(
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new PipingScenariosViewFailureMechanismSectionViewModel(section, null, new PipingScenarioConfigurationPerFailureMechanismSection(section));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ScenarioConfigurationPerSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new PipingScenariosViewFailureMechanismSectionViewModel(section, new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("scenarioConfigurationPerSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new PipingScenarioConfigurationPerFailureMechanismSection(section);

            // Call
            var viewModel = new PipingScenariosViewFailureMechanismSectionViewModel(section, new PipingFailureMechanism(), scenarioConfiguration);

            // Assert
            Assert.AreSame(section, viewModel.Section);
            Assert.AreSame(scenarioConfiguration, viewModel.ScenarioConfigurationPerSection);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingScenarioConfigurationType.Probabilistic)]
        public void DisplayName_FailureMechanismScenarioConfigurationTypeNotPerSection_ReturnsExpectedName(PipingScenarioConfigurationType scenarioConfigurationType)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingFailureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };
            var scenarioConfiguration = new PipingScenarioConfigurationPerFailureMechanismSection(section);

            var viewModel = new PipingScenariosViewFailureMechanismSectionViewModel(section, pipingFailureMechanism, scenarioConfiguration);

            // Call
            string displayName = viewModel.DisplayName;

            // Assert
            Assert.AreEqual(section.Name, displayName);
        }

        [Test]
        [TestCase(PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic, "semi-probabilistisch")]
        [TestCase(PipingScenarioConfigurationPerFailureMechanismSectionType.Probabilistic, "probabilistisch")]
        public void DisplayName_FailureMechanismScenarioConfigurationTypePerSection_ReturnsExpectedName(
            PipingScenarioConfigurationPerFailureMechanismSectionType scenarioConfigurationType, string displayNameSuffix)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingFailureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingScenarioConfigurationType.PerFailureMechanismSection
            };
            var scenarioConfiguration = new PipingScenarioConfigurationPerFailureMechanismSection(section)
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            var viewModel = new PipingScenariosViewFailureMechanismSectionViewModel(section, pipingFailureMechanism, scenarioConfiguration);

            // Call
            string displayName = viewModel.DisplayName;

            // Assert
            Assert.AreEqual($"{section.Name} ({displayNameSuffix})", displayName);
        }
    }
}