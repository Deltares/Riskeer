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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Geometry;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLinePointCreateExtensionsTest
    {
        [Test]
        public void CreateSurfaceLinePoint_NoCollector_ThrowArgumentNullException()
        {
            // Setup
            var geometryPoint = new Point3D(1.1, 2.2, 3.3);

            // Call
            TestDelegate call = () => geometryPoint.CreateSurfaceLinePoint(null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void CreateSurfaceLinePoint_ValidArguments_CreateSurfaceLinePointEntity()
        {
            // Setup
            var collector = new CreateConversionCollector();
            var geometryPoint = new Point3D(2.3, 4.5, 6.7);

            // Call
            const int expectedOrder = 3;
            SurfaceLinePointEntity entity = geometryPoint.CreateSurfaceLinePoint(collector, expectedOrder);

            // Assert
            Assert.AreEqual(geometryPoint.X, entity.X);
            Assert.AreEqual(geometryPoint.Y, entity.Y);
            Assert.AreEqual(geometryPoint.Z, entity.Z);
            Assert.AreEqual(expectedOrder, entity.Order);

            Assert.AreEqual(0, entity.SurfaceLinePointEntityId);

            CollectionAssert.IsEmpty(entity.BottomDitchDikeSidePointParentEntity);
            CollectionAssert.IsEmpty(entity.BottomDitchPolderSidePointParentEntity);
            CollectionAssert.IsEmpty(entity.DikeToeAtPolderPointParentEntity);
            CollectionAssert.IsEmpty(entity.DikeToeAtRiverPointParentEntity);
            CollectionAssert.IsEmpty(entity.DitchDikeSidePointParentEntity);
            CollectionAssert.IsEmpty(entity.DitchPolderSidePointParentEntity);
            Assert.IsNull(entity.SurfaceLineEntity);
            Assert.AreEqual(0, entity.SurfaceLineEntityId);
        }

        [Test]
        public void CreateSurfaceLinePoint_ValidArguments_NewEntityIsRegisteredToCreateConversionCollector()
        {
            // Setup
            var collector = new CreateConversionCollector();
            var geometryPoint = new Point3D(2.3, 4.5, 6.7);

            // Call
            const int expectedOrder = 3;
            SurfaceLinePointEntity entity = geometryPoint.CreateSurfaceLinePoint(collector, expectedOrder);

            // Assert
            SurfaceLinePointEntity retrievedEntity = collector.GetSurfaceLinePoint(geometryPoint);
            Assert.AreSame(entity, retrievedEntity);
        }
    }
}