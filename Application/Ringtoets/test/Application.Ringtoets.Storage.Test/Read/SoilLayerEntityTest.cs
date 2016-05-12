// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class SoilLayerEntityTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_Always_NewPoint(bool isAquifer)
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double top = random.NextDouble();
            var entity = new SoilLayerEntity
            {
                SoilLayerEntityId = entityId,
                Top = Convert.ToDecimal(top),
                IsAquifer = Convert.ToByte(isAquifer)
            };

            // Call
            var layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            Assert.AreEqual(entityId, layer.StorageId);
            Assert.AreEqual(top, layer.Top, 1e-6);
            Assert.AreEqual(isAquifer, layer.IsAquifer);
        }     
    }
}