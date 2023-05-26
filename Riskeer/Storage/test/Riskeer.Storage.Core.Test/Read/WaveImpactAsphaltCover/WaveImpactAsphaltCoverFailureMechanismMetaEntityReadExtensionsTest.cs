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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.WaveImpactAsphaltCover;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Test.Read.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((WaveImpactAsphaltCoverFailureMechanismMetaEntity) null)
                .Read(new GeneralWaveImpactAsphaltCoverInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_GeneralInputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverFailureMechanismMetaEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void Read_WithAllData_SetsGeneralInputProperties()
        {
            // Setup
            var random = new Random();
            var entity = new WaveImpactAsphaltCoverFailureMechanismMetaEntity
            {
                DeltaL = random.NextDouble(1, 2000),
                ApplyLengthEffectInSection = Convert.ToByte(random.NextBoolean())
            };

            var generalInput = new GeneralWaveImpactAsphaltCoverInput();

            // Call
            entity.Read(generalInput);

            // Assert
            Assert.AreEqual(entity.DeltaL, generalInput.DeltaL, generalInput.DeltaL.GetAccuracy());
            Assert.AreEqual(Convert.ToBoolean(entity.ApplyLengthEffectInSection), generalInput.ApplyLengthEffectInSection);
        }
    }
}