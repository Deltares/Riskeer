// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionConfigurationCreateExtensionsTest
    {
        [Test]
        public void Create_ConfigurationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsFailureMechanismSectionConfigurationCreateExtensions.Create(null);

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
            var configuration = new FailureMechanismSectionConfiguration(failureMechanismSection)
            {
                A = random.NextRoundedDouble()
            };

            // Call
            MacroStabilityInwardsFailureMechanismSectionConfigurationEntity entity = configuration.Create();

            // Assert
            Assert.AreEqual(configuration.A, entity.A, configuration.A.GetAccuracy());
        }
    }
}