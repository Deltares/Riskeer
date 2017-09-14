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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class SurfaceLineEntityReadExtensionsTest
    {
        [Test]
        public void ReadAsPipingSurfaceLine_CollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SurfaceLineEntity();

            // Call
            TestDelegate call = () => entity.ReadAsPipingSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void ReadAsPipingSurfaceLine_EntityNull_ThrowArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => ((SurfaceLineEntity) null).ReadAsPipingSurfaceLine(collector);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void ReadAsPipingSurfaceLine_PointsXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            var entity = new SurfaceLineEntity
            {
                Name = "surface line",
                PointsXml = xml
            };

            // Call
            TestDelegate call = () => entity.ReadAsPipingSurfaceLine(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void ReadAsPipingSurfaceLine_SurfaceLineEntityWithoutGeometryPointEntities_ReturnSurfaceLine()
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
            PipingSurfaceLine surfaceLine = entity.ReadAsPipingSurfaceLine(collector);

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
        public void ReadAsPipingSurfaceLine_SurfaceLineEntityWithGeometryPointEntitiesButNoCharacteristicPoints_ReturnSurfaceLineWithGeometry()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var random = new Random(31);

            var points = new[]
            {
                CreatePoint3D(random),
                CreatePoint3D(random),
                CreatePoint3D(random)
            };

            var entity = new SurfaceLineEntity
            {
                Name = nameof(SurfaceLineEntity),
                ReferenceLineIntersectionX = random.NextDouble(),
                ReferenceLineIntersectionY = random.NextDouble(),
                PointsXml = new Point3DXmlSerializer().ToXml(points)
            };

            // Call
            PipingSurfaceLine surfaceLine = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.AreEqual(points, surfaceLine.Points);

            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
        }

        [Test]
        public void ReadAsPipingSurfaceLine_SurfaceLineEntityWithGeometryPointEntitiesAndCharacteristicPoints_ReturnFullSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var random = new Random(31);

            var points = new[]
            {
                CreatePoint3D(random),
                CreatePoint3D(random),
                CreatePoint3D(random),
                CreatePoint3D(random),
                CreatePoint3D(random),
                CreatePoint3D(random),
                CreatePoint3D(random)
            };

            var entity = new SurfaceLineEntity
            {
                Name = "Better name.",
                ReferenceLineIntersectionX = random.NextDouble(),
                ReferenceLineIntersectionY = random.NextDouble(),
                PointsXml = new Point3DXmlSerializer().ToXml(points)
            };
            entity.PipingCharacteristicPointEntities.Add(CreatePipingCharacteristicPointEntity(points[1], PipingCharacteristicPointType.BottomDitchDikeSide));
            entity.PipingCharacteristicPointEntities.Add(CreatePipingCharacteristicPointEntity(points[2], PipingCharacteristicPointType.BottomDitchPolderSide));
            entity.PipingCharacteristicPointEntities.Add(CreatePipingCharacteristicPointEntity(points[3], PipingCharacteristicPointType.DikeToeAtPolder));
            entity.PipingCharacteristicPointEntities.Add(CreatePipingCharacteristicPointEntity(points[4], PipingCharacteristicPointType.DikeToeAtRiver));
            entity.PipingCharacteristicPointEntities.Add(CreatePipingCharacteristicPointEntity(points[5], PipingCharacteristicPointType.DitchDikeSide));
            entity.PipingCharacteristicPointEntities.Add(CreatePipingCharacteristicPointEntity(points[6], PipingCharacteristicPointType.DitchPolderSide));

            // Call
            PipingSurfaceLine surfaceLine = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.AreEqual(points, surfaceLine.Points);

            Assert.AreSame(surfaceLine.Points[1], surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(surfaceLine.Points[2], surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(surfaceLine.Points[3], surfaceLine.DikeToeAtPolder);
            Assert.AreSame(surfaceLine.Points[4], surfaceLine.DikeToeAtRiver);
            Assert.AreSame(surfaceLine.Points[5], surfaceLine.DitchDikeSide);
            Assert.AreSame(surfaceLine.Points[6], surfaceLine.DitchPolderSide);
        }

        [Test]
        public void ReadAsPipingSurfaceLine_FullSurfaceLineEntity_ReturnFullSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var random = new Random(31);

            var points = new[]
            {
                CreatePoint3D(random),
                CreatePoint3D(random)
            };

            var entity = new SurfaceLineEntity
            {
                Name = "Better name.",
                ReferenceLineIntersectionX = random.NextDouble(),
                ReferenceLineIntersectionY = random.NextDouble(),
                PointsXml = new Point3DXmlSerializer().ToXml(points),
                PipingCharacteristicPointEntities =
                {
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.BottomDitchDikeSide),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.BottomDitchPolderSide),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DikeToeAtPolder),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DikeToeAtRiver),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DitchDikeSide),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DitchPolderSide)
                }
            };

            // Call
            PipingSurfaceLine surfaceLine = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

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
        public void ReadAsPipingSurfaceLine_WithNullValues_ReturnsPipingSurfaceLineWithNaNValues()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var random = new Random(31);

            var point3D = new Point3D(double.NaN, double.NaN, double.NaN);
            var points = new[]
            {
                point3D,
                CreatePoint3D(random)
            };

            var entity = new SurfaceLineEntity
            {
                Name = "name",
                PointsXml = new Point3DXmlSerializer().ToXml(points),
                PipingCharacteristicPointEntities =
                {
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.BottomDitchDikeSide),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.BottomDitchPolderSide),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DikeToeAtPolder),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DikeToeAtRiver),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DitchDikeSide),
                    CreatePipingCharacteristicPointEntity(points[0], PipingCharacteristicPointType.DitchPolderSide)
                }
            };

            // Call
            PipingSurfaceLine surfaceLine = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.IsNaN(surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.IsNaN(surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

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
        public void ReadAsPipingSurfaceLine_SurfaceLineEntityReadMultipleTimes_ReturnSameSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new SurfaceLineEntity
            {
                Name = "surface line",
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            PipingSurfaceLine surfaceLine1 = entity.ReadAsPipingSurfaceLine(collector);
            PipingSurfaceLine surfaceLine2 = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreSame(surfaceLine1, surfaceLine2);
        }

        private static Point3D CreatePoint3D(Random random)
        {
            return new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }

        private static PipingCharacteristicPointEntity CreatePipingCharacteristicPointEntity(Point3D point,
                                                                                             PipingCharacteristicPointType type)
        {
            return new PipingCharacteristicPointEntity
            {
                Type = (byte) type,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
        }
    }
}