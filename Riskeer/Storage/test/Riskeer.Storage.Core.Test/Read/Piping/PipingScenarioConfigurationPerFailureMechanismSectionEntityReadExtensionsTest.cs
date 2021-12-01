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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.Piping;

namespace Riskeer.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingScenarioConfigurationPerFailureMechanismSectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((PipingScenarioConfigurationPerFailureMechanismSectionEntity) null).Read(
                new PipingScenarioConfigurationPerFailureMechanismSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ScenarioConfigurationsPerFailureMechanismSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingScenarioConfigurationPerFailureMechanismSectionEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("scenarioConfigurationsPerFailureMechanismSection", paramName);
        }

        [Test]
        public void Read_ParameterValues_SectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(21);
            var configurationType = random.NextEnumValue<PipingScenarioConfigurationPerFailureMechanismSectionType>();

            var entity = new PipingScenarioConfigurationPerFailureMechanismSectionEntity
            {
                PipingScenarioConfigurationPerFailureMechanismSectionType = Convert.ToByte(configurationType)
            };

            var configurationPerSection =
                new PipingScenarioConfigurationPerFailureMechanismSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(configurationPerSection);

            // Assert
            Assert.AreEqual(configurationType, configurationPerSection.ScenarioConfigurationType);
        }
    }
}