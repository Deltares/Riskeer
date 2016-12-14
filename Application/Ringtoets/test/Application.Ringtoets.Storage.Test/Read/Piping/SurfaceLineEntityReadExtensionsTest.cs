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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Read.Piping;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class SurfaceLineEntityReadExtensionsTest
    {
        [Test]
        public void Read_ReadConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SurfaceLineEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Read_EntityNotReadBefore_EntityRegistered()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new SurfaceLineEntity
            {
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };

            // Precondition
            Assert.IsFalse(collector.Contains(entity));

            // Call
            entity.Read(collector);

            // Assert
            Assert.IsTrue(collector.Contains(entity));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Read_PointsXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            var entity = new SurfaceLineEntity
            {
                PointsXml = xml
            };

            // Call
            TestDelegate call = () => entity.Read(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void Read_SurfaceLineEntityWithoutGeometryPointEntities_ReturnSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();

            const string name = "nice name!";
            const double intersectionX = 1.1;
            const double intersectionY = 2.2;

            var entity = new SurfaceLineEntity
            {
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY,
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, surfaceLine.Name);
            Assert.AreEqual(intersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(intersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.IsEmpty(surfaceLine.Points);

            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
        }

        [Test]
        public void Read_SurfaceLineEntityWithGeometryPointEntitiesButNoCharacteristicPoints_ReturnSurfaceLineGeometry()
        {
            // Setup
            var collector = new ReadConversionCollector();

            const string name = "Better name.";
            const double intersectionX = 3.4;
            const double intersectionY = 7.5;

            var points = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6),
                new Point3D(7.7, 8.8, 9.9)
            };

            var entity = new SurfaceLineEntity
            {
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY,
                PointsXml = new Point3DXmlSerializer().ToXml(points)
            };

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, surfaceLine.Name);
            Assert.AreEqual(intersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(intersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.AreEqual(points, surfaceLine.Points);

            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
        }

        [Test]
        public void Read_SurfaceLineEntityWithGeometryPointEntitiesAndCharacteristicPoints_ReturnFullSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();

            const string name = "Better name.";
            const double intersectionX = 3.4;
            const double intersectionY = 7.5;

            var points = new[]
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

            var entity = new SurfaceLineEntity
            {
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY,
                PointsXml = new Point3DXmlSerializer().ToXml(points)
            };
            entity.CharacteristicPointEntities.Add(CreateCharacteristicPointEntity(points[1], CharacteristicPointType.BottomDitchDikeSide));
            entity.CharacteristicPointEntities.Add(CreateCharacteristicPointEntity(points[2], CharacteristicPointType.BottomDitchPolderSide));
            entity.CharacteristicPointEntities.Add(CreateCharacteristicPointEntity(points[3], CharacteristicPointType.DikeToeAtPolder));
            entity.CharacteristicPointEntities.Add(CreateCharacteristicPointEntity(points[4], CharacteristicPointType.DikeToeAtRiver));
            entity.CharacteristicPointEntities.Add(CreateCharacteristicPointEntity(points[5], CharacteristicPointType.DitchDikeSide));
            entity.CharacteristicPointEntities.Add(CreateCharacteristicPointEntity(points[6], CharacteristicPointType.DitchPolderSide));

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, surfaceLine.Name);
            Assert.AreEqual(intersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(intersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.AreEqual(points, surfaceLine.Points);

            Assert.AreSame(surfaceLine.Points[1], surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(surfaceLine.Points[2], surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(surfaceLine.Points[3], surfaceLine.DikeToeAtPolder);
            Assert.AreSame(surfaceLine.Points[4], surfaceLine.DikeToeAtRiver);
            Assert.AreSame(surfaceLine.Points[5], surfaceLine.DitchDikeSide);
            Assert.AreSame(surfaceLine.Points[6], surfaceLine.DitchPolderSide);
        }

        [Test]
        public void Read_SurfaceLineEntityWithGeometryPointEntityMarkedForAllCharacteristicPoints_ReturnFullSurfaceLineWithCharacteristicPointsToOneGeometryPoint()
        {
            // Setup
            var collector = new ReadConversionCollector();

            const string name = "Better name.";
            const double intersectionX = 3.4;
            const double intersectionY = 7.5;

            const double x = 1.0;
            const double y = 2.0;
            const double z = 3.0;
            var points = new[]
            {
                new Point3D(x, y, z),
                new Point3D(5.0, 6.0, 7.0)
            };

            var entity = new SurfaceLineEntity
            {
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY,
                PointsXml = new Point3DXmlSerializer().ToXml(points),
                CharacteristicPointEntities =
                {
                    new CharacteristicPointEntity
                    {
                        X = x, Y = y, Z = z, Type = (byte) CharacteristicPointType.BottomDitchDikeSide
                    },
                    new CharacteristicPointEntity
                    {
                        X = x, Y = y, Z = z, Type = (byte) CharacteristicPointType.BottomDitchPolderSide
                    },
                    new CharacteristicPointEntity
                    {
                        X = x, Y = y, Z = z, Type = (byte) CharacteristicPointType.DikeToeAtPolder
                    },
                    new CharacteristicPointEntity
                    {
                        X = x, Y = y, Z = z, Type = (byte) CharacteristicPointType.DikeToeAtRiver
                    },
                    new CharacteristicPointEntity
                    {
                        X = x, Y = y, Z = z, Type = (byte) CharacteristicPointType.DitchDikeSide
                    },
                    new CharacteristicPointEntity
                    {
                        X = x, Y = y, Z = z, Type = (byte) CharacteristicPointType.DitchPolderSide
                    }
                }
            };

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(name, surfaceLine.Name);
            Assert.AreEqual(intersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(intersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            Point3D[] geometry = surfaceLine.Points.ToArray();
            Assert.AreEqual(2, geometry.Length);
            Point3D geometryPoint = geometry[0];

            Assert.AreSame(geometryPoint, surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(geometryPoint, surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(geometryPoint, surfaceLine.DikeToeAtPolder);
            Assert.AreSame(geometryPoint, surfaceLine.DikeToeAtRiver);
            Assert.AreSame(geometryPoint, surfaceLine.DitchDikeSide);
            Assert.AreSame(geometryPoint, surfaceLine.DitchPolderSide);
        }

        [Test]
        public void Read_SurfaceLineEntityReadMultipleTimes_ReturnSameSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new SurfaceLineEntity
            {
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            RingtoetsPipingSurfaceLine surfaceLine1 = entity.Read(collector);
            RingtoetsPipingSurfaceLine surfaceLine2 = entity.Read(collector);

            // Assert
            Assert.AreSame(surfaceLine1, surfaceLine2);
        }

        private static CharacteristicPointEntity CreateCharacteristicPointEntity(Point3D point, CharacteristicPointType type)
        {
            return new CharacteristicPointEntity
            {
                Type = (byte)type,
                X = point.X, Y = point.Y, Z = point.Z
            };
        }
    }
}