// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.WaveImpactAsphaltCover;

namespace Ringtoets.Storage.Core.Test.Read.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((WaveImpactAsphaltCoverFailureMechanismMetaEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_Always_ReturnGeneralWaveImpactAsphaltCoverInput()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverFailureMechanismMetaEntity
            {
                DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
            };

            // Call
            GeneralWaveImpactAsphaltCoverInput generalInput = entity.Read();

            // Assert
            Assert.AreEqual(entity.DeltaL, generalInput.DeltaL, generalInput.DeltaL.GetAccuracy());
        }
    }
}