﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.Read.GrassCoverErosionInwards;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Read.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismMetaEntityReadExtensionsTest
    {
        [Test]
        public void ReadGrassCoverErosionInwardsFailureMechanism_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => ((GrassCoverErosionInwardsFailureMechanismMetaEntity) null).Read(generalInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_GeneralInputIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new GrassCoverErosionInwardsFailureMechanismMetaEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("input", paramName);
        }

        [Test]
        public void Read_ValidEntity_ReturnGeneralGrassCoverErosionInwardsInput()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble n = random.NextRoundedDouble(1.0, 20.0);

            var entity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                N = n
            };
            var inputToUpdate = new GeneralGrassCoverErosionInwardsInput();

            // Call
            entity.Read(inputToUpdate);

            // Assert
            Assert.AreEqual(entity.N, inputToUpdate.N, inputToUpdate.N.GetAccuracy());
        }
    }
}