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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLineCreateExtensionsTest
    {
        [Test]
        public void Create_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => ((MacroStabilityInwardsSurfaceLine) null).Create(registry, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            // Call
            TestDelegate call = () => surfaceLine.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_SurfaceLineWithoutGeometry_ReturnSurfaceLineEntityWithoutAddingPointEntities()
        {
            // Setup
            var random = new Random(31);
            var registry = new PersistenceRegistry();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = GetRandomPoint2D(random)
            };
            int order = random.Next();

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, order);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            string expectedXml = new Point3DXmlSerializer().ToXml(new Point3D[0]);
            Assert.AreEqual(expectedXml, entity.PointsXml);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var random = new Random(31);
            var registry = new PersistenceRegistry();
            const string name = "Test";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name)
            {
                ReferenceLineIntersectionWorldPoint = GetRandomPoint2D(random)
            };

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(name, entity.Name);
        }

        [Test]
        public void Create_SurfaceLineWithGeometryWithoutCharacteristicPoints_ReturnSurfaceLineEntityWithPointEntities()
        {
            // Setup
            var random = new Random(31);
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = GetRandomPoint2D(random)
            };
            surfaceLine.SetGeometry(new[]
            {
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random)
            });

            var registry = new PersistenceRegistry();
            int order = random.Next();

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, order);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            string expectedXml = new Point3DXmlSerializer().ToXml(surfaceLine.Points);
            Assert.AreEqual(expectedXml, entity.PointsXml);
        }

        [Test]
        public void Create_SurfaceLineWithAllData_ReturnSurfaceLineEntityWithPointEntitiesAndCharactersisticPointReferences()
        {
            // Setup
            var random = new Random(31);
            var geometry = new[]
            {
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random),
                GetRandomPoint3D(random)
            };
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = GetRandomPoint2D(random)
            };
            surfaceLine.SetGeometry(geometry);

            const int surfaceLevelOutsideIndex = 1;
            surfaceLine.SetSurfaceLevelOutsideAt(geometry[surfaceLevelOutsideIndex]);
            const int trafficLoadOutsideIndex = 2;
            surfaceLine.SetTrafficLoadOutsideAt(geometry[trafficLoadOutsideIndex]);
            const int trafficLoadInsideIndex = 3;
            surfaceLine.SetTrafficLoadInsideAt(geometry[trafficLoadInsideIndex]);
            const int dikeTopAtPolderIndex = 4;
            surfaceLine.SetDikeTopAtPolderAt(geometry[dikeTopAtPolderIndex]);
            const int dikeTopAtRiverIndex = 5;
            surfaceLine.SetDikeTopAtRiverAt(geometry[dikeTopAtRiverIndex]);
            const int shoulderBaseInsideIndex = 6;
            surfaceLine.SetShoulderBaseInsideAt(geometry[shoulderBaseInsideIndex]);
            const int shoulderTopInsideIndex = 7;
            surfaceLine.SetShoulderTopInsideAt(geometry[shoulderTopInsideIndex]);
            const int bottomDitchDikeSideIndex = 8;
            surfaceLine.SetBottomDitchDikeSideAt(geometry[bottomDitchDikeSideIndex]);
            const int bottomDitchPolderSideIndex = 9;
            surfaceLine.SetBottomDitchPolderSideAt(geometry[bottomDitchPolderSideIndex]);
            const int dikeToeAtPolderIndex = 10;
            surfaceLine.SetDikeToeAtPolderAt(geometry[dikeToeAtPolderIndex]);
            const int dikeToeAtRiverIndex = 11;
            surfaceLine.SetDikeToeAtRiverAt(geometry[dikeToeAtRiverIndex]);
            const int ditchDikeSideIndex = 12;
            surfaceLine.SetDitchDikeSideAt(geometry[ditchDikeSideIndex]);
            const int ditchPolderSideIndex = 13;
            surfaceLine.SetDitchPolderSideAt(geometry[ditchPolderSideIndex]);
            const int surfaceLevelInsideIndex = 14;
            surfaceLine.SetSurfaceLevelInsideAt(geometry[surfaceLevelInsideIndex]);

            var registry = new PersistenceRegistry();

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = new Point3DXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(14, entity.MacroStabilityInwardsCharacteristicPointEntities.Count);
            foreach (MacroStabilityInwardsCharacteristicPointEntity characteristicPointEntity in entity.MacroStabilityInwardsCharacteristicPointEntities)
            {
                switch (characteristicPointEntity.Type)
                {
                    case (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside:
                        Assert.AreEqual(geometry[surfaceLevelOutsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[surfaceLevelOutsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[surfaceLevelOutsideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.TrafficLoadOutside:
                        Assert.AreEqual(geometry[trafficLoadOutsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[trafficLoadOutsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[trafficLoadOutsideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.TrafficLoadInside:
                        Assert.AreEqual(geometry[trafficLoadInsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[trafficLoadInsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[trafficLoadInsideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder:
                        Assert.AreEqual(geometry[dikeTopAtPolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[dikeTopAtPolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[dikeTopAtPolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver:
                        Assert.AreEqual(geometry[dikeTopAtRiverIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[dikeTopAtRiverIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[dikeTopAtRiverIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside:
                        Assert.AreEqual(geometry[shoulderBaseInsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[shoulderBaseInsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[shoulderBaseInsideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside:
                        Assert.AreEqual(geometry[shoulderTopInsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[shoulderTopInsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[shoulderTopInsideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide:
                        Assert.AreEqual(geometry[bottomDitchDikeSideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[bottomDitchDikeSideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[bottomDitchDikeSideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide:
                        Assert.AreEqual(geometry[bottomDitchPolderSideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[bottomDitchPolderSideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[bottomDitchPolderSideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder:
                        Assert.AreEqual(geometry[dikeToeAtPolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[dikeToeAtPolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[dikeToeAtPolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver:
                        Assert.AreEqual(geometry[dikeToeAtRiverIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[dikeToeAtRiverIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[dikeToeAtRiverIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.DitchDikeSide:
                        Assert.AreEqual(geometry[ditchDikeSideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[ditchDikeSideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[ditchDikeSideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.DitchPolderSide:
                        Assert.AreEqual(geometry[ditchPolderSideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[ditchPolderSideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[ditchPolderSideIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside:
                        Assert.AreEqual(geometry[surfaceLevelInsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[surfaceLevelInsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[surfaceLevelInsideIndex].Z, characteristicPointEntity.Z);
                        break;
                    default:
                        Assert.Fail("Invalid characteristic point type found: {0}", characteristicPointEntity.Type);
                        break;
                }
            }
        }

        [Test]
        public void Create_SurfaceLineWithCharacteristicPointsOnSameGeometryPoint_ReturnSurfaceLineEntityWithPointEntitiesAndCharactersisticPointReferences()
        {
            // Setup
            var random = new Random(31);
            var registry = new PersistenceRegistry();
            var geometry = new[]
            {
                GetRandomPoint3D(random)
            };
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };
            surfaceLine.SetGeometry(geometry);
            surfaceLine.SetSurfaceLevelOutsideAt(geometry[0]);
            surfaceLine.SetTrafficLoadOutsideAt(geometry[0]);
            surfaceLine.SetTrafficLoadInsideAt(geometry[0]);
            surfaceLine.SetDikeTopAtPolderAt(geometry[0]);
            surfaceLine.SetDikeTopAtRiverAt(geometry[0]);
            surfaceLine.SetShoulderBaseInsideAt(geometry[0]);
            surfaceLine.SetShoulderTopInsideAt(geometry[0]);
            surfaceLine.SetBottomDitchDikeSideAt(geometry[0]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[0]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[0]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[0]);
            surfaceLine.SetDitchDikeSideAt(geometry[0]);
            surfaceLine.SetDitchPolderSideAt(geometry[0]);
            surfaceLine.SetSurfaceLevelInsideAt(geometry[0]);

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = new Point3DXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(14, entity.MacroStabilityInwardsCharacteristicPointEntities.Count);
            short[] characteristicPointTypeValues = entity.MacroStabilityInwardsCharacteristicPointEntities
                                                          .Select(cpe => cpe.Type)
                                                          .ToArray();
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.TrafficLoadOutside);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.TrafficLoadInside);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.DitchDikeSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.DitchPolderSide);
            CollectionAssert.Contains(characteristicPointTypeValues, (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside);
        }

        [Test]
        public void Create_CreatingEntityForSameSurfaceLine_ReturnSameEntity()
        {
            // Setup
            var random = new Random(31);
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty)
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };

            var registry = new PersistenceRegistry();

            // Call
            SurfaceLineEntity entity1 = surfaceLine.Create(registry, 0);
            SurfaceLineEntity entity2 = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }

        private static Point3D GetRandomPoint3D(Random random)
        {
            return new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
        }

        private static Point2D GetRandomPoint2D(Random random)
        {
            return new Point2D(random.NextDouble(), random.NextDouble());
        }
    }
}