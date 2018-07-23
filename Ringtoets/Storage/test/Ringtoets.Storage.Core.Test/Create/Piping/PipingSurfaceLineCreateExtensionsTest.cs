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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.Create.Piping;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingSurfaceLineCreateExtensionsTest
    {
        [Test]
        public void Create_SurfaceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => ((PipingSurfaceLine) null).Create(registry, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);

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
            int order = random.Next();
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
            };

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, order);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            Assert.IsNull(entity.FailureMechanismEntity);
            IEnumerable<Point3D> points = new Point3D[0];
            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(points);
            Assert.AreEqual(expectedXml, entity.PointsXml);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine("Test");

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
            int order = random.Next();
            var registry = new PersistenceRegistry();
            var surfaceLine = new PipingSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(1.1, 2.2)
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble())
            });

            // Call
            SurfaceLineEntity entity = surfaceLine.Create(registry, order);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);
            Assert.AreEqual(order, entity.Order);

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(surfaceLine.Points);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.IsNull(entity.FailureMechanismEntity);
        }

        [Test]
        public void Create_SurfaceLineWithGeometryAndCharacteristicPoints_ReturnSurfaceLineEntityWithPointEntitiesAndCharactersisticPointReferences()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var random = new Random(31);
            var geometry = new[]
            {
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            var surfaceLine = new PipingSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
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

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(6, entity.PipingCharacteristicPointEntities.Count);
            foreach (PipingCharacteristicPointEntity characteristicPointEntity in entity.PipingCharacteristicPointEntities)
            {
                switch (characteristicPointEntity.Type)
                {
                    case (byte) PipingCharacteristicPointType.BottomDitchDikeSide:
                        Assert.AreEqual(geometry[bottomDitchDikeIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[bottomDitchDikeIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[bottomDitchDikeIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) PipingCharacteristicPointType.BottomDitchPolderSide:
                        Assert.AreEqual(geometry[bottomDitchPolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[bottomDitchPolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[bottomDitchPolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) PipingCharacteristicPointType.DikeToeAtPolder:
                        Assert.AreEqual(geometry[toePolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[toePolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[toePolderIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) PipingCharacteristicPointType.DikeToeAtRiver:
                        Assert.AreEqual(geometry[toeDikeIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[toeDikeIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[toeDikeIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) PipingCharacteristicPointType.DitchDikeSide:
                        Assert.AreEqual(geometry[ditchDikeIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[ditchDikeIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[ditchDikeIndex].Z, characteristicPointEntity.Z);
                        break;
                    case (byte) PipingCharacteristicPointType.DitchPolderSide:
                        Assert.AreEqual(geometry[ditchPolderIndex].X, characteristicPointEntity.X);
                        Assert.AreEqual(geometry[ditchPolderIndex].Y, characteristicPointEntity.Y);
                        Assert.AreEqual(geometry[ditchPolderIndex].Z, characteristicPointEntity.Z);
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
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            var surfaceLine = new PipingSurfaceLine("Test")
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(random.NextDouble(), random.NextDouble())
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

            string expectedXml = new Point3DCollectionXmlSerializer().ToXml(geometry);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            Assert.AreEqual(6, entity.PipingCharacteristicPointEntities.Count);

            CollectionAssert.AreEquivalent(new[]
            {
                (byte) PipingCharacteristicPointType.DikeToeAtRiver,
                (byte) PipingCharacteristicPointType.DikeToeAtPolder,
                (byte) PipingCharacteristicPointType.DitchDikeSide,
                (byte) PipingCharacteristicPointType.BottomDitchDikeSide,
                (byte) PipingCharacteristicPointType.BottomDitchPolderSide,
                (byte) PipingCharacteristicPointType.DitchPolderSide
            }, entity.PipingCharacteristicPointEntities
                     .Select(cpe => cpe.Type));

            foreach (PipingCharacteristicPointEntity characteristicPointEntity in entity.PipingCharacteristicPointEntities)
            {
                Assert.AreEqual(geometry[0].X, characteristicPointEntity.X);
                Assert.AreEqual(geometry[0].Y, characteristicPointEntity.Y);
                Assert.AreEqual(geometry[0].Z, characteristicPointEntity.Z);
            }
        }

        [Test]
        public void Create_CreatingEntityForSameSurfaceLine_ReturnSamenEntity()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            var registry = new PersistenceRegistry();

            // Call
            SurfaceLineEntity entity1 = surfaceLine.Create(registry, 0);
            SurfaceLineEntity entity2 = surfaceLine.Create(registry, 0);

            // Assert
            Assert.AreSame(entity1, entity2);
        }
    }
}