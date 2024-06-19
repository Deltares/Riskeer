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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingFailureMechanismSectionConfigurationCreateExtensionsTest
    {
        [Test]
        public void Create_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismSectionConfigurationCreateExtensions.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("configuration", exception.ParamName);
        }

        [Test]
        public void Create_WithSectionConfiguration_ReturnsExpectedEntity()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var configuration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection)
            {
                ScenarioConfigurationType = random.NextEnumValue<PipingFailureMechanismSectionScenarioConfigurationType>(),
                A = random.NextRoundedDouble()
            };

            // Call
            PipingFailureMechanismSectionConfigurationEntity entity = configuration.Create();

            // Assert
            Assert.AreEqual(Convert.ToByte(configuration.ScenarioConfigurationType), entity.PipingScenarioConfigurationPerFailureMechanismSectionType);
            Assert.AreEqual(configuration.A, entity.A, configuration.A.GetAccuracy());
        }
    }
}