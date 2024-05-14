﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingScenarioConfigurationPerFailureMechanismSectionTest
    {
        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingScenarioConfigurationPerFailureMechanismSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            var scenarioConfigurationPerFailureMechanismSection = new PipingScenarioConfigurationPerFailureMechanismSection(section);

            // Assert
            Assert.IsInstanceOf<ScenarioConfigurationPerFailureMechanismSection>(scenarioConfigurationPerFailureMechanismSection);
            Assert.AreEqual(0.4, scenarioConfigurationPerFailureMechanismSection.A);
            Assert.AreSame(section, scenarioConfigurationPerFailureMechanismSection.Section);
            Assert.AreEqual(PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic,
                            scenarioConfigurationPerFailureMechanismSection.ScenarioConfigurationType);
        }
    }
}