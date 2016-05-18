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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineCreateExtensionsTest
    {
        [Test]
        public void Create_CreateConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate call = () => surfaceLine.Create(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Create_SurfaceLineWithoutGeometry_ReturnSurfaceLineEntityWithoutAddingPointEntities()
        {
            // Setup
            var collector = new CreateConversionCollector();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test"
            };

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(collector);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);

            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
            CollectionAssert.IsEmpty(entity.SurfaceLinePointEntities);
        }

        [Test]
        public void Create_SurfaceLineWithGeometryWithoutCharacteristicPoints_ReturnSurfaceLineEntityWithPointEntities()
        {
            // Setup
            var collector = new CreateConversionCollector();
            var geometry = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6),
                new Point3D(7.7, 8.8, 9.9)
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test"
            };
            surfaceLine.SetGeometry(geometry);

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(collector);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);

            Assert.AreEqual(geometry.Length, entity.SurfaceLinePointEntities.Count);
            SurfaceLinePointEntity[] pointEntities = entity.SurfaceLinePointEntities.ToArray();
            for (int i = 0; i < geometry.Length; i++)
            {
                SurfaceLinePointEntity pointEntity = pointEntities[i];
                Assert.AreEqual(i, pointEntity.Order);

                Point3D expectedMatchingGeometryPoint = geometry[i];
                Assert.AreEqual(expectedMatchingGeometryPoint.X, pointEntity.X);
                Assert.AreEqual(expectedMatchingGeometryPoint.Y, pointEntity.Y);
                Assert.AreEqual(expectedMatchingGeometryPoint.Z, pointEntity.Z);
            }

            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
        }

        [Test]
        public void Create_SurfaceLineWithAllData_ReturnSurfaceLineEntityWithPointEntitiesAndCharactersisticPointReferences()
        {
            // Setup
            var collector = new CreateConversionCollector();
            var geometry = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6),
                new Point3D(7.7, 8.8, 9.9),
                new Point3D(10.10, 11.11, 12.12),
                new Point3D(13.13, 14.14, 15.15),
                new Point3D(16.16, 17.17, 18.18),
                new Point3D(19.19, 20.20, 21.21),
                new Point3D(22.22, 23.23, 24.24),
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test"
            };
            surfaceLine.SetGeometry(geometry);
            surfaceLine.SetBottomDitchDikeSideAt(geometry[1]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[2]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[3]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[4]);
            surfaceLine.SetDitchDikeSideAt(geometry[5]);
            surfaceLine.SetDitchPolderSideAt(geometry[6]);

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(collector);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);

            Assert.AreEqual(geometry.Length, entity.SurfaceLinePointEntities.Count);
            SurfaceLinePointEntity[] pointEntities = entity.SurfaceLinePointEntities.ToArray();
            for (int i = 0; i < geometry.Length; i++)
            {
                SurfaceLinePointEntity pointEntity = pointEntities[i];
                Assert.AreEqual(i, pointEntity.Order);

                Point3D expectedMatchingGeometryPoint = geometry[i];
                Assert.AreEqual(expectedMatchingGeometryPoint.X, pointEntity.X);
                Assert.AreEqual(expectedMatchingGeometryPoint.Y, pointEntity.Y);
                Assert.AreEqual(expectedMatchingGeometryPoint.Z, pointEntity.Z);
            }

            Assert.AreEqual(geometry[1].X, entity.BottomDitchDikeSidePointEntity.X);
            Assert.AreEqual(geometry[1].Y, entity.BottomDitchDikeSidePointEntity.Y);
            Assert.AreEqual(geometry[1].Z, entity.BottomDitchDikeSidePointEntity.Z);

            Assert.AreEqual(geometry[2].X, entity.BottomDitchPolderSidePointEntity.X);
            Assert.AreEqual(geometry[2].Y, entity.BottomDitchPolderSidePointEntity.Y);
            Assert.AreEqual(geometry[2].Z, entity.BottomDitchPolderSidePointEntity.Z);

            Assert.AreEqual(geometry[3].X, entity.DikeToeAtPolderPointEntity.X);
            Assert.AreEqual(geometry[3].Y, entity.DikeToeAtPolderPointEntity.Y);
            Assert.AreEqual(geometry[3].Z, entity.DikeToeAtPolderPointEntity.Z);

            Assert.AreEqual(geometry[4].X, entity.DikeToeAtRiverPointEntity.X);
            Assert.AreEqual(geometry[4].Y, entity.DikeToeAtRiverPointEntity.Y);
            Assert.AreEqual(geometry[4].Z, entity.DikeToeAtRiverPointEntity.Z);

            Assert.AreEqual(geometry[5].X, entity.DitchDikeSidePointEntity.X);
            Assert.AreEqual(geometry[5].Y, entity.DitchDikeSidePointEntity.Y);
            Assert.AreEqual(geometry[5].Z, entity.DitchDikeSidePointEntity.Z);

            Assert.AreEqual(geometry[6].X, entity.DitchPolderSidePointEntity.X);
            Assert.AreEqual(geometry[6].Y, entity.DitchPolderSidePointEntity.Y);
            Assert.AreEqual(geometry[6].Z, entity.DitchPolderSidePointEntity.Z);
            
            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
        }
    }
}