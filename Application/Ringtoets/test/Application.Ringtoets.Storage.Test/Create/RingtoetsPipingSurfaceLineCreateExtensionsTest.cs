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
        public void Create_PersistenceRegistryIsNull_ThrowArgumentNullException()
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
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test",
                ReferenceLineIntersectionWorldPoint = new Point2D(1235.439, 49308.346)
            };

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
            CollectionAssert.IsEmpty(entity.SurfaceLinePointEntities);
        }

        [Test]
        public void Create_SurfaceLineWithGeometryWithoutCharacteristicPoints_ReturnSurfaceLineEntityWithPointEntities()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var geometry = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6),
                new Point3D(7.7, 8.8, 9.9)
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test",
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            };
            surfaceLine.SetGeometry(geometry);

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

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
            var registry = new PersistenceRegistry();
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
                Name = "Test",
                ReferenceLineIntersectionWorldPoint = new Point2D(3.3, 4.4)
            };
            surfaceLine.SetGeometry(geometry);
            const int bottomDitchDikeIndex = 1;
            surfaceLine.SetBottomDitchDikeSideAt(geometry[bottomDitchDikeIndex]);
            const int bottomDitchPolderIndex = 2;
            surfaceLine.SetBottomDitchPolderSideAt(geometry[bottomDitchPolderIndex]);
            const int toePolderIndex = 3;
            surfaceLine.SetDikeToeAtPolderAt(geometry[toePolderIndex]);
            const int toeDikeIndex = 4;
            surfaceLine.SetDikeToeAtRiverAt(geometry[toeDikeIndex]);
            const int ditchDikeIndex = 5;
            surfaceLine.SetDitchDikeSideAt(geometry[ditchDikeIndex]);
            const int ditchPolderIndex = 6;
            surfaceLine.SetDitchPolderSideAt(geometry[ditchPolderIndex]);

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

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

            SurfaceLinePointEntity bottomDitchDikeEntity = pointEntities[bottomDitchDikeIndex];
            Assert.AreEqual(1, bottomDitchDikeEntity.CharacteristicPointEntities.Count);
            Assert.AreEqual((short)CharacteristicPointType.BottomDitchDikeSide, bottomDitchDikeEntity.CharacteristicPointEntities.First().CharacteristicPointType);

            SurfaceLinePointEntity bottomDitchPolderEntity = pointEntities[bottomDitchPolderIndex];
            Assert.AreEqual(1, bottomDitchPolderEntity.CharacteristicPointEntities.Count);
            Assert.AreEqual((short)CharacteristicPointType.BottomDitchPolderSide, bottomDitchPolderEntity.CharacteristicPointEntities.First().CharacteristicPointType);

            SurfaceLinePointEntity dikeToePolderEntity = pointEntities[toePolderIndex];
            Assert.AreEqual(1, dikeToePolderEntity.CharacteristicPointEntities.Count);
            Assert.AreEqual((short)CharacteristicPointType.DikeToeAtPolder, dikeToePolderEntity.CharacteristicPointEntities.First().CharacteristicPointType);

            SurfaceLinePointEntity dikeToeRiverEntity = pointEntities[toeDikeIndex];
            Assert.AreEqual(1, dikeToeRiverEntity.CharacteristicPointEntities.Count);
            Assert.AreEqual((short)CharacteristicPointType.DikeToeAtRiver, dikeToeRiverEntity.CharacteristicPointEntities.First().CharacteristicPointType);

            SurfaceLinePointEntity ditchDikeEntity = pointEntities[ditchDikeIndex];
            Assert.AreEqual(1, ditchDikeEntity.CharacteristicPointEntities.Count);
            Assert.AreEqual((short)CharacteristicPointType.DitchDikeSide, ditchDikeEntity.CharacteristicPointEntities.First().CharacteristicPointType);

            SurfaceLinePointEntity ditchPolderEntity = pointEntities[ditchPolderIndex];
            Assert.AreEqual(1, ditchPolderEntity.CharacteristicPointEntities.Count);
            Assert.AreEqual((short)CharacteristicPointType.DitchPolderSide, ditchPolderEntity.CharacteristicPointEntities.First().CharacteristicPointType);
            
            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
        }

        [Test]
        public void Create_SurfaceLineWithCharacteristicPointsOnSameGeometryPoint_ReturnSurfaceLineEntityWithPointEntitiesAndCharactersisticPointReferences()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var geometry = new[]
            {
                new Point3D(1.1, 2.2, 3.3)
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test",
                ReferenceLineIntersectionWorldPoint = new Point2D(3.3, 4.4)
            };
            surfaceLine.SetGeometry(geometry);
            surfaceLine.SetBottomDitchDikeSideAt(geometry[0]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[0]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[0]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[0]);
            surfaceLine.SetDitchDikeSideAt(geometry[0]);
            surfaceLine.SetDitchPolderSideAt(geometry[0]);

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

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

            SurfaceLinePointEntity characteristicGeometryPointEntity = pointEntities[0];
            Assert.AreEqual(6, characteristicGeometryPointEntity.CharacteristicPointEntities.Count);
            short[] characteristicPointTypeValues = characteristicGeometryPointEntity.CharacteristicPointEntities
                                                                                     .Select(cpe => cpe.CharacteristicPointType)
                                                                                     .ToArray();
            CollectionAssert.Contains(characteristicPointTypeValues, (short)CharacteristicPointType.DikeToeAtRiver);
            CollectionAssert.Contains(characteristicPointTypeValues, (short)CharacteristicPointType.DikeToeAtPolder);
            CollectionAssert.Contains(characteristicPointTypeValues, (short)CharacteristicPointType.DitchDikeSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (short)CharacteristicPointType.BottomDitchDikeSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (short)CharacteristicPointType.BottomDitchPolderSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (short)CharacteristicPointType.DitchPolderSide);

            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
        }

        [Test]
        public void Create_SurfaceLine_RegisterNewEntityToPersistenceRegistry()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(0.0, 0.0)
            };

            // Call
            surfaceLine.Create(registry);

            // Assert
            Assert.IsTrue(registry.Contains(surfaceLine));
        }

        [Test]
        public void Create_CreatingEntityForSameSurfaceLine_ReturnSamenEntity()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            };

            var registry = new PersistenceRegistry();

            // Call
            SurfaceLineEntity entity1 = surfaceLine.Create(registry);
            SurfaceLineEntity entity2 = surfaceLine.Create(registry);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}