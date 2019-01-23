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
using Ringtoets.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.StabilityPointStructures;

namespace Ringtoets.Storage.Core.Test.Read.StabilityPointStructures
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((StabilityPointStructuresFailureMechanismMetaEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_Always_ReturnGeneralStabilityPointStructuresInput()
        {
            // Setup
            var entity = new StabilityPointStructuresFailureMechanismMetaEntity
            {
                N = new Random(39).NextRoundedDouble(1.0, 20.0)
            };

            // Call
            GeneralStabilityPointStructuresInput generalInput = entity.Read();

            // Assert
            Assert.AreEqual(entity.N, generalInput.N, generalInput.N.GetAccuracy());
        }
    }
}