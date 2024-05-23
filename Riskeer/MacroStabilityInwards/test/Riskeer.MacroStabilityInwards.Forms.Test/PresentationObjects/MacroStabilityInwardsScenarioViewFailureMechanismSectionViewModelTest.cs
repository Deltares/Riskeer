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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityInwardsScenarioViewFailureMechanismSectionViewModelTest
    {
        [Test]
        public void Constructor_FailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new MacroStabilityInwardsScenarioViewFailureMechanismSectionViewModel(
                null, new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ScenarioConfigurationPerSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            void Call() => new MacroStabilityInwardsScenarioViewFailureMechanismSectionViewModel(section, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("scenarioConfigurationPerSection", exception.ParamName);
        }


        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            // Call
            var viewModel = new MacroStabilityInwardsScenarioViewFailureMechanismSectionViewModel(section, scenarioConfiguration);

            // Assert
            Assert.AreSame(section, viewModel.Section);
            Assert.AreSame(scenarioConfiguration, viewModel.ScenarioConfigurationPerSection);
        }
        
        [Test]
        public void ToString_FailureMechanismScenarioConfigurationTypeNotPerSection_ReturnsExpectedName()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var scenarioConfiguration = new MacroStabilityInwardsScenarioConfigurationPerFailureMechanismSection(section);

            var viewModel = new MacroStabilityInwardsScenarioViewFailureMechanismSectionViewModel(section, scenarioConfiguration);

            // Call
            var toString = viewModel.ToString();

            // Assert
            Assert.AreEqual(section.Name, toString);
        }
    }
}