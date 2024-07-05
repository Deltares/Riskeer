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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.Piping;

namespace Riskeer.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingFailureMechanismSectionConfigurationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((PipingFailureMechanismSectionConfigurationEntity) null).Read(
                new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_FailureMechanismSectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new PipingFailureMechanismSectionConfigurationEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanismSectionConfiguration", paramName);
        }

        [Test]
        public void Read_ParameterValues_SectionResultWithParameterValues()
        {
            // Setup
            var random = new Random(21);
            var configurationType = random.NextEnumValue<PipingFailureMechanismSectionScenarioConfigurationType>();

            var entity = new PipingFailureMechanismSectionConfigurationEntity
            {
                ScenarioConfigurationType = Convert.ToByte(configurationType),
                A = random.NextRoundedDouble()
            };

            var sectionConfiguration =
                new PipingFailureMechanismSectionConfiguration(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            entity.Read(sectionConfiguration);

            // Assert
            Assert.AreEqual(configurationType, sectionConfiguration.ScenarioConfigurationType);
            Assert.AreEqual(entity.A, sectionConfiguration.A, sectionConfiguration.A.GetAccuracy());
        }
    }
}