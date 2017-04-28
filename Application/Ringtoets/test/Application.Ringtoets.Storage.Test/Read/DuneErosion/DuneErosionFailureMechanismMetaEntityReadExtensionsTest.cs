// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.DuneErosion;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;

namespace Application.Ringtoets.Storage.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneErosionFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void Read_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new DuneErosionFailureMechanismMetaEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("input", paramName);
        }

        [Test]
        public void Read_ValidInput_UpdateDuneErosionGeneralInput()
        {
            // Setup
            var generalInput = new GeneralDuneErosionInput();
            double nValue = new Random(31).GetFromRange(1, 20);
            var entity = new DuneErosionFailureMechanismMetaEntity
            {
                N = nValue
            };

            // Call
            entity.Read(generalInput);

            // Assert
            Assert.AreEqual(nValue, generalInput.N, generalInput.N.GetAccuracy());
        }
    }
}