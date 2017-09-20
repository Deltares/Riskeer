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
using Ringtoets.MacroStabilityInwards.Primitives;
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
            var random = new Random(31);

            var entity = new SurfaceLineEntity
            {
                Name = "nice name!",
                ReferenceLineIntersectionX = random.NextDouble(),
                ReferenceLineIntersectionY = random.NextDouble(),
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            PipingSurfaceLine surfaceLine = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points)
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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points),
                PipingCharacteristicPointEntities =
                {
                    CreatePipingCharacteristicPointEntity(points[1], PipingCharacteristicPointType.BottomDitchDikeSide),
                    CreatePipingCharacteristicPointEntity(points[2], PipingCharacteristicPointType.BottomDitchPolderSide),
                    CreatePipingCharacteristicPointEntity(points[3], PipingCharacteristicPointType.DikeToeAtPolder),
                    CreatePipingCharacteristicPointEntity(points[4], PipingCharacteristicPointType.DikeToeAtRiver),
                    CreatePipingCharacteristicPointEntity(points[5], PipingCharacteristicPointType.DitchDikeSide),
                    CreatePipingCharacteristicPointEntity(points[6], PipingCharacteristicPointType.DitchPolderSide)
                }
            };

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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points),
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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points),
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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            PipingSurfaceLine surfaceLine1 = entity.ReadAsPipingSurfaceLine(collector);
            PipingSurfaceLine surfaceLine2 = entity.ReadAsPipingSurfaceLine(collector);

            // Assert
            Assert.AreSame(surfaceLine1, surfaceLine2);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_CollectorNull_ThrowArgumentNullException()
        {
            // Setup
            var entity = new SurfaceLineEntity();

            // Call
            TestDelegate call = () => entity.ReadAsMacroStabilityInwardsSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_EntityNull_ThrowArgumentNullException()
        {
            // Setup
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate call = () => ((SurfaceLineEntity) null).ReadAsMacroStabilityInwardsSurfaceLine(collector);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void ReadAsMacroStabilityInwardsSurfaceLine_PointsXmlNullOrEmpty_ThrowsArgumentException(string xml)
        {
            var entity = new SurfaceLineEntity
            {
                Name = "surface line",
                PointsXml = xml
            };

            // Call
            TestDelegate call = () => entity.ReadAsMacroStabilityInwardsSurfaceLine(new ReadConversionCollector());

            // Assert
            string paramName = Assert.Throws<ArgumentException>(call).ParamName;
            Assert.AreEqual("xml", paramName);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_SurfaceLineEntityWithoutGeometryPointEntities_ReturnSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var random = new Random(31);

            var entity = new SurfaceLineEntity
            {
                Name = "nice name!",
                ReferenceLineIntersectionX = random.NextDouble(),
                ReferenceLineIntersectionY = random.NextDouble(),
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            MacroStabilityInwardsSurfaceLine surfaceLine = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.IsEmpty(surfaceLine.Points);

            Assert.IsNull(surfaceLine.SurfaceLevelOutside);
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.TrafficLoadOutside);
            Assert.IsNull(surfaceLine.TrafficLoadInside);
            Assert.IsNull(surfaceLine.DikeTopAtPolder);
            Assert.IsNull(surfaceLine.DikeTopAtRiver);
            Assert.IsNull(surfaceLine.ShoulderBaseInside);
            Assert.IsNull(surfaceLine.ShoulderTopInside);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
            Assert.IsNull(surfaceLine.SurfaceLevelInside);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_SurfaceLineEntityWithGeometryPointEntitiesButNoCharacteristicPoints_ReturnSurfaceLineWithGeometry()
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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points)
            };

            // Call
            MacroStabilityInwardsSurfaceLine surfaceLine = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.AreEqual(points, surfaceLine.Points);

            Assert.IsNull(surfaceLine.SurfaceLevelOutside);
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.TrafficLoadOutside);
            Assert.IsNull(surfaceLine.TrafficLoadInside);
            Assert.IsNull(surfaceLine.DikeTopAtPolder);
            Assert.IsNull(surfaceLine.DikeTopAtRiver);
            Assert.IsNull(surfaceLine.ShoulderBaseInside);
            Assert.IsNull(surfaceLine.ShoulderTopInside);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
            Assert.IsNull(surfaceLine.SurfaceLevelInside);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_SurfaceLineEntityWithGeometryPointEntitiesAndCharacteristicPoints_ReturnFullSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();
            var random = new Random(31);

            Point3D[] points = Array.ConvertAll(new Point3D[14], p => CreatePoint3D(random));

            var entity = new SurfaceLineEntity
            {
                Name = "Better name.",
                ReferenceLineIntersectionX = random.NextDouble(),
                ReferenceLineIntersectionY = random.NextDouble(),
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points),
                MacroStabilityInwardsCharacteristicPointEntities =
                {
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[1], MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[2], MacroStabilityInwardsCharacteristicPointType.TrafficLoadOutside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[3], MacroStabilityInwardsCharacteristicPointType.TrafficLoadInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[4], MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[5], MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[6], MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[7], MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[8], MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[9], MacroStabilityInwardsCharacteristicPointType.DitchDikeSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[10], MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[11], MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[12], MacroStabilityInwardsCharacteristicPointType.DitchPolderSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[13], MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside)
                }
            };

            // Call
            MacroStabilityInwardsSurfaceLine surfaceLine = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            CollectionAssert.AreEqual(points, surfaceLine.Points);

            Assert.AreSame(surfaceLine.Points[0], surfaceLine.SurfaceLevelOutside);
            Assert.AreSame(surfaceLine.Points[1], surfaceLine.DikeToeAtRiver);
            Assert.AreSame(surfaceLine.Points[2], surfaceLine.TrafficLoadOutside);
            Assert.AreSame(surfaceLine.Points[3], surfaceLine.TrafficLoadInside);
            Assert.AreSame(surfaceLine.Points[4], surfaceLine.DikeTopAtPolder);
            Assert.AreSame(surfaceLine.Points[5], surfaceLine.DikeTopAtRiver);
            Assert.AreSame(surfaceLine.Points[6], surfaceLine.ShoulderBaseInside);
            Assert.AreSame(surfaceLine.Points[7], surfaceLine.ShoulderTopInside);
            Assert.AreSame(surfaceLine.Points[8], surfaceLine.DikeToeAtPolder);
            Assert.AreSame(surfaceLine.Points[9], surfaceLine.DitchDikeSide);
            Assert.AreSame(surfaceLine.Points[10], surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(surfaceLine.Points[11], surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(surfaceLine.Points[12], surfaceLine.DitchPolderSide);
            Assert.AreSame(surfaceLine.Points[13], surfaceLine.SurfaceLevelInside);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_FullSurfaceLineEntity_ReturnFullSurfaceLine()
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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points),
                MacroStabilityInwardsCharacteristicPointEntities =
                {
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.TrafficLoadOutside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.TrafficLoadInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DitchDikeSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DitchPolderSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside)
                }
            };

            // Call
            MacroStabilityInwardsSurfaceLine surfaceLine = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.AreEqual(entity.ReferenceLineIntersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(entity.ReferenceLineIntersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            Point3D[] geometry = surfaceLine.Points.ToArray();
            Assert.AreEqual(2, geometry.Length);
            Point3D geometryPoint = geometry[0];

            Assert.AreSame(geometryPoint, surfaceLine.SurfaceLevelOutside);
            Assert.AreSame(geometryPoint, surfaceLine.DikeToeAtRiver);
            Assert.AreSame(geometryPoint, surfaceLine.TrafficLoadOutside);
            Assert.AreSame(geometryPoint, surfaceLine.TrafficLoadInside);
            Assert.AreSame(geometryPoint, surfaceLine.DikeTopAtPolder);
            Assert.AreSame(geometryPoint, surfaceLine.DikeTopAtRiver);
            Assert.AreSame(geometryPoint, surfaceLine.ShoulderBaseInside);
            Assert.AreSame(geometryPoint, surfaceLine.ShoulderTopInside);
            Assert.AreSame(geometryPoint, surfaceLine.DikeToeAtPolder);
            Assert.AreSame(geometryPoint, surfaceLine.DitchDikeSide);
            Assert.AreSame(geometryPoint, surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(geometryPoint, surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(geometryPoint, surfaceLine.DitchPolderSide);
            Assert.AreSame(geometryPoint, surfaceLine.SurfaceLevelInside);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_WithNullValues_ReturnsMacroStabilityInwardsSurfaceLineWithNaNValues()
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
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(points),
                MacroStabilityInwardsCharacteristicPointEntities =
                {
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.TrafficLoadOutside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.TrafficLoadInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DitchDikeSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.DitchPolderSide),
                    CreateMacroStabilityInwardsCharacteristicPointEntity(points[0], MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside)
                }
            };

            // Call
            MacroStabilityInwardsSurfaceLine surfaceLine = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);

            // Assert
            Assert.AreEqual(entity.Name, surfaceLine.Name);
            Assert.IsNaN(surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.IsNaN(surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            Point3D[] geometry = surfaceLine.Points.ToArray();
            Assert.AreEqual(2, geometry.Length);
            Point3D geometryPoint = geometry[0];

            Assert.AreSame(geometryPoint, surfaceLine.SurfaceLevelOutside);
            Assert.AreSame(geometryPoint, surfaceLine.DikeToeAtRiver);
            Assert.AreSame(geometryPoint, surfaceLine.TrafficLoadOutside);
            Assert.AreSame(geometryPoint, surfaceLine.TrafficLoadInside);
            Assert.AreSame(geometryPoint, surfaceLine.DikeTopAtPolder);
            Assert.AreSame(geometryPoint, surfaceLine.DikeTopAtRiver);
            Assert.AreSame(geometryPoint, surfaceLine.ShoulderBaseInside);
            Assert.AreSame(geometryPoint, surfaceLine.ShoulderTopInside);
            Assert.AreSame(geometryPoint, surfaceLine.DikeToeAtPolder);
            Assert.AreSame(geometryPoint, surfaceLine.DitchDikeSide);
            Assert.AreSame(geometryPoint, surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(geometryPoint, surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(geometryPoint, surfaceLine.DitchPolderSide);
            Assert.AreSame(geometryPoint, surfaceLine.SurfaceLevelInside);
        }

        [Test]
        public void ReadAsMacroStabilityInwardsSurfaceLine_SurfaceLineEntityReadMultipleTimes_ReturnSameSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();

            var entity = new SurfaceLineEntity
            {
                Name = "surface line",
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0])
            };

            // Call
            MacroStabilityInwardsSurfaceLine surfaceLine1 = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);
            MacroStabilityInwardsSurfaceLine surfaceLine2 = entity.ReadAsMacroStabilityInwardsSurfaceLine(collector);

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

        private static MacroStabilityInwardsCharacteristicPointEntity CreateMacroStabilityInwardsCharacteristicPointEntity(
            Point3D point,
            MacroStabilityInwardsCharacteristicPointType type)
        {
            return new MacroStabilityInwardsCharacteristicPointEntity
            {
                Type = (byte) type,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
        }
    }
}