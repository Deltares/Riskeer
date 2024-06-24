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
        public void Constructor_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingScenariosViewFailureMechanismSectionViewModel(null, new PipingFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new PipingScenariosViewFailureMechanismSectionViewModel(new PipingFailureMechanismSectionConfiguration(section), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(section);

            // Call
            var viewModel = new PipingScenariosViewFailureMechanismSectionViewModel(sectionConfiguration, new PipingFailureMechanism());

            // Assert
            Assert.AreSame(sectionConfiguration, viewModel.SectionConfiguration);
        }

        [Test]
        [TestCase(PipingFailureMechanismScenarioConfigurationType.SemiProbabilistic)]
        [TestCase(PipingFailureMechanismScenarioConfigurationType.Probabilistic)]
        public void ToString_FailureMechanismScenarioConfigurationTypeNotPerSection_ReturnsExpectedName(PipingFailureMechanismScenarioConfigurationType scenarioConfigurationType)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingFailureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(section);

            var viewModel = new PipingScenariosViewFailureMechanismSectionViewModel(sectionConfiguration, pipingFailureMechanism);

            // Call
            var toString = viewModel.ToString();

            // Assert
            Assert.AreEqual(section.Name, toString);
        }

        [Test]
        [TestCase(PipingFailureMechanismSectionScenarioConfigurationType.SemiProbabilistic, "semi-probabilistisch")]
        [TestCase(PipingFailureMechanismSectionScenarioConfigurationType.Probabilistic, "probabilistisch")]
        public void ToString_FailureMechanismSectionScenarioConfigurationType_ReturnsExpectedName(
            PipingFailureMechanismSectionScenarioConfigurationType scenarioConfigurationType, string displayNameSuffix)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var pipingFailureMechanism = new PipingFailureMechanism
            {
                ScenarioConfigurationType = PipingFailureMechanismScenarioConfigurationType.PerFailureMechanismSection
            };
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(section)
            {
                ScenarioConfigurationType = scenarioConfigurationType
            };

            var viewModel = new PipingScenariosViewFailureMechanismSectionViewModel(sectionConfiguration, pipingFailureMechanism);

            // Call
            var toString = viewModel.ToString();

            // Assert
            Assert.AreEqual($"{section.Name} ({displayNameSuffix})", toString);
        }
    }
}