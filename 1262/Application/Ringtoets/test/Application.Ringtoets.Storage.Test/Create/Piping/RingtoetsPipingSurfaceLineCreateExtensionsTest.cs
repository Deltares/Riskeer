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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.Piping
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
            TestDelegate call = () => surfaceLine.Create(null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Create_SurfaceLineWithoutGeometry_ReturnSurfaceLineEntityWithoutAddingPointEntities()
        {
            // Setup
            int order = new Random(96).Next();
            var registry = new PersistenceRegistry();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Test",
                ReferenceLineIntersectionWorldPoint = new Point2D(1235.439, 49308.346)
            };

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, order);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            Assert.AreEqual(0, entity.SurfaceLineEntityId);
            Assert.AreEqual(0, entity.FailureMechanismEntityId);
            Assert.IsNull(entity.FailureMechanismEntity);
            IEnumerable<Point3D> points = new Point3D[0];
            string expectedXml = new Point3DXmlSerializer().ToXml(points);
            Assert.AreEqual(expectedXml, entity.PointsXml);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var registry = new PersistenceRegistry();
            const string originalName = "Test";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = originalName,
                ReferenceLineIntersectionWorldPoint = new Point2D(1235.439, 49308.346)
            };

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreNotSame(originalName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreEqual(originalName, entity.Name);
        }

        [Test]
        public void Create_SurfaceLineWithGeometryWithoutCharacteristicPoints_ReturnSurfaceLineEntityWithPointEntities()
        {
            // Setup
            int order = new Random(21).Next();
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
            SurfaceLineEntity entity = surfaceLine.Create(registry, order);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            string expectedXml = new Point3DXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

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
                new Point3D(22.22, 23.23, 24.24)
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
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = new Point3DXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(6, entity.CharacteristicPointEntities.Count);
            foreach (CharacteristicPointEntity characteristicPointEntity in entity.CharacteristicPointEntities)
            {
                switch (characteristicPointEntity.Type)
                {
                    case (byte) CharacteristicPointType.BottomDitchDikeSide:
                        Assert.AreEqual(geometry[bottomDitchDikeIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[bottomDitchDikeIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[bottomDitchDikeIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) CharacteristicPointType.BottomDitchPolderSide:
                        Assert.AreEqual(geometry[bottomDitchPolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[bottomDitchPolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[bottomDitchPolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) CharacteristicPointType.DikeToeAtPolder:
                        Assert.AreEqual(geometry[toePolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[toePolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[toePolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) CharacteristicPointType.DikeToeAtRiver:
                        Assert.AreEqual(geometry[toeDikeIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[toeDikeIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[toeDikeIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) CharacteristicPointType.DitchDikeSide:
                        Assert.AreEqual(geometry[ditchDikeIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[ditchDikeIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[ditchDikeIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) CharacteristicPointType.DitchPolderSide:
                        Assert.AreEqual(geometry[ditchPolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[ditchPolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[ditchPolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    default:
                        Assert.Fail("Invalid characteristic point type found: {0}", characteristicPointEntity.Type);
                        break;
                }
            }

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
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = new Point3DXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(6, entity.CharacteristicPointEntities.Count);
            byte[] characteristicPointTypeValues = entity.CharacteristicPointEntities
                                                         .Select(cpe => cpe.Type)
                                                         .ToArray();
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) CharacteristicPointType.DikeToeAtRiver);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) CharacteristicPointType.DikeToeAtPolder);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) CharacteristicPointType.DitchDikeSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) CharacteristicPointType.BottomDitchDikeSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) CharacteristicPointType.BottomDitchPolderSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) CharacteristicPointType.DitchPolderSide);

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
            surfaceLine.Create(registry, 0);

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
            SurfaceLineEntity entity1 = surfaceLine.Create(registry, 0);
            SurfaceLineEntity entity2 = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}