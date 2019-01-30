// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Create.MacroStabilityInwards
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
            Assert.IsNotNull(entity);
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0]);
            Assert.AreEqual(expectedXml, entity.PointsXml);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(surfaceLine.Name, entity.Name);
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

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(surfaceLine.Points);
            Assert.AreEqual(expectedXml, entity.PointsXml);
        }

        [Test]
        public void Create_SurfaceLineWithGeometryAndCharacteristicPoints_ReturnSurfaceLineEntityWithPointEntitiesAndCharactersisticPointReferences()
        {
            // Setup
            var random = new Random(31);
            Point3D[] geometry =
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
            const int dikeTopAtPolderIndex = 2;
            surfaceLine.SetDikeTopAtPolderAt(geometry[dikeTopAtPolderIndex]);
            const int dikeTopAtRiverIndex = 3;
            surfaceLine.SetDikeTopAtRiverAt(geometry[dikeTopAtRiverIndex]);
            const int shoulderBaseInsideIndex = 4;
            surfaceLine.SetShoulderBaseInsideAt(geometry[shoulderBaseInsideIndex]);
            const int shoulderTopInsideIndex = 5;
            surfaceLine.SetShoulderTopInsideAt(geometry[shoulderTopInsideIndex]);
            const int bottomDitchDikeSideIndex = 6;
            surfaceLine.SetBottomDitchDikeSideAt(geometry[bottomDitchDikeSideIndex]);
            const int bottomDitchPolderSideIndex = 7;
            surfaceLine.SetBottomDitchPolderSideAt(geometry[bottomDitchPolderSideIndex]);
            const int dikeToeAtPolderIndex = 8;
            surfaceLine.SetDikeToeAtPolderAt(geometry[dikeToeAtPolderIndex]);
            const int dikeToeAtRiverIndex = 9;
            surfaceLine.SetDikeToeAtRiverAt(geometry[dikeToeAtRiverIndex]);
            const int ditchDikeSideIndex = 10;
            surfaceLine.SetDitchDikeSideAt(geometry[ditchDikeSideIndex]);
            const int ditchPolderSideIndex = 11;
            surfaceLine.SetDitchPolderSideAt(geometry[ditchPolderSideIndex]);
            const int surfaceLevelInsideIndex = 12;
            surfaceLine.SetSurfaceLevelInsideAt(geometry[surfaceLevelInsideIndex]);

            var registry = new PersistenceRegistry();

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(12, entity.MacroStabilityInwardsCharacteristicPointEntities.Count);
            foreach (MacroStabilityInwardsCharacteristicPointEntity characteristicPointEntity in entity.MacroStabilityInwardsCharacteristicPointEntities)
            {
                switch (characteristicPointEntity.Type)
                {
                    case (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside:
                        Assert.AreEqual(geometry[surfaceLevelOutsideIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[surfaceLevelOutsideIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[surfaceLevelOutsideIndex].Z, characteristicPointEntity.Z);
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
            Point3D[] geometry =
            {
                GetRandomPoint3D(random)
            };
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };
            surfaceLine.SetGeometry(geometry);
            surfaceLine.SetSurfaceLevelOutsideAt(geometry[0]);
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

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(12, entity.MacroStabilityInwardsCharacteristicPointEntities.Count);
            CollectionAssert.AreEquivalent(new[]
            {
                (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside,
                (byte) MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder,
                (byte) MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver,
                (byte) MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside,
                (byte) MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside,
                (byte) MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide,
                (byte) MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide,
                (byte) MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder,
                (byte) MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver,
                (byte) MacroStabilityInwardsCharacteristicPointType.DitchDikeSide,
                (byte) MacroStabilityInwardsCharacteristicPointType.DitchPolderSide,
                (byte) MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside
            }, entity.MacroStabilityInwardsCharacteristicPointEntities
                     .Select(cpe => cpe.Type));

            foreach (MacroStabilityInwardsCharacteristicPointEntity characteristicPointEntity in entity.MacroStabilityInwardsCharacteristicPointEntities)
            {
                Assert.AreEqual(geometry[0].X, characteristicPointEntity.X);
                Assert.AreEqual(geometry[0].Y, characteristicPointEntity.Y);
                Assert.AreEqual(geometry[0].Z, characteristicPointEntity.Z);
            }
        }

        [Test]
        public void Create_CreatingEntityForSameSurfaceLine_ReturnSameEntity()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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