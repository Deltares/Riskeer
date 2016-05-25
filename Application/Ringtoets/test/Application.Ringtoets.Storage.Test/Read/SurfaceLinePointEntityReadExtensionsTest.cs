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

using Core.Common.Base.Geometry;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class SurfaceLinePointEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SurfaceLinePointEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Read_ValidEntity_ReturnPoint3D()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new SurfaceLinePointEntity
            {
                X = 1.1m,
                Y = 3.3m,
                Z = 5.5m,
                SurfaceLinePointEntityId = 538246839
            };

            // Call
            Point3D geometryPoint = entity.Read(collector);

            // Assert
            Assert.AreEqual(entity.X, geometryPoint.X);
            Assert.AreEqual(entity.Y, geometryPoint.Y);
            Assert.AreEqual(entity.Z, geometryPoint.Z);
            Assert.AreEqual(entity.SurfaceLinePointEntityId, geometryPoint.StorageId);
        }

        [Test]
        public void Read_ReadingSameEntityTwice_ReturnedInstanceAreIdentical()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new SurfaceLinePointEntity
            {
                X = 1.1m,
                Y = 3.3m,
                Z = 5.5m,
                SurfaceLinePointEntityId = 538246839
            };

            // Call
            Point3D geometryPoint1 = entity.Read(collector);
            Point3D geometryPoint2 = entity.Read(collector);

            // Assert
            Assert.AreSame(geometryPoint1, geometryPoint2);
        }
    }
}