﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Create.HeightStructures;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.HeightStructures.Data;

namespace Application.Ringtoets.Storage.Test.Create.HeightStructures
{
    [TestFixture]
    public class GeneralHeightStructuresInputCreateExtensionsTest
    {
        [Test]
        public void Create_ValidInput_ReturnMetaEntity()
        {
            // Setup
            var random = new Random(1);
            var generalinput = new GeneralHeightStructuresInput
            {
                N = random.Next(1, 20)
            };

            // Call
            HeightStructuresFailureMechanismMetaEntity entity = generalinput.Create();

            // Assert
            Assert.AreEqual(generalinput.N, entity.N);

            Assert.IsNull(entity.FailureMechanismEntity);
            Assert.AreEqual(0, entity.HeightStructuresFailureMechanismMetaEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
        }
    }
}