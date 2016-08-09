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
using System.Collections.Generic;
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update.Piping;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update.Piping
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            // Call
            TestDelegate call = () => surfaceLine.Update(null, context);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            // Call
            TestDelegate call = () => surfaceLine.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_SurfaceLineNotSaved_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();

            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Precondition
            const long unsavedObjectId = 0;
            Assert.AreEqual(unsavedObjectId, surfaceLine.StorageId);

            // Call
            TestDelegate call = () => surfaceLine.Update(registry, context);

            // Assert
            var expectedMessage = String.Format("Het object 'SurfaceLineEntity' met id '{0}' is niet gevonden.", unsavedObjectId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithoutGeometry_SurfaceLineEntityUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithoutGeometry();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                Name = "<originalName>",
                ReferenceLineIntersectionX = 9876.5432,
                ReferenceLineIntersectionY = 9182.8374,
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = new Point3DXmlSerializer().ToXml(new Point3D[0]);
            Assert.AreEqual(expectedXml, entity.PointsXml);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithNewGeometry_SurfaceLineEntityUpdatedAndGeometrySaved()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();

            var serializer = new Point3DXmlSerializer();
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                Name = "<name>",
                ReferenceLineIntersectionX = 91.28,
                ReferenceLineIntersectionY = 37.46,
                PointsXml = serializer.ToXml(new Point3D[0])
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = serializer.ToXml(surfaceLine.Points);
            Assert.AreEqual(expectedXml, entity.PointsXml);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithUpdatedGeometry_SurfaceLineEntityUpdatedAndGeometryChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();

            var serializer = new Point3DXmlSerializer();
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                Name = "<name>",
                ReferenceLineIntersectionX = 91.28,
                ReferenceLineIntersectionY = 37.46,
                PointsXml = serializer.ToXml(new[]
                {
                    new Point3D(1.0, 2.0, 3.0), 
                    new Point3D(5.0, 2.0, 4.0)
                })
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            string expectedXml = serializer.ToXml(surfaceLine.Points);
            Assert.AreEqual(expectedXml, entity.PointsXml);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineIdenticalGeometry_SurfaceLineEntityUpdatedAndGeometrySaved()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();
            
            string pointsXml = new Point3DXmlSerializer().ToXml(surfaceLine.Points);
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                PointsXml = pointsXml
            };
            context.SurfaceLineEntities.Add(entity);
            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            Assert.AreSame(pointsXml, entity.PointsXml);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithNewGeometryAndCharacteristicPoints_SurfaceLineEntityUpdatedAndGeometryPlusCharacteristicPointsSaved()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            string expectedXml = new Point3DXmlSerializer().ToXml(surfaceLine.Points);
            Assert.AreEqual(expectedXml, entity.PointsXml);

            var characteristicPointAssociations = new[]
            {
                Tuple.Create(surfaceLine.BottomDitchDikeSide, CharacteristicPointType.BottomDitchDikeSide),
                Tuple.Create(surfaceLine.BottomDitchPolderSide, CharacteristicPointType.BottomDitchPolderSide),
                Tuple.Create(surfaceLine.DikeToeAtPolder, CharacteristicPointType.DikeToeAtPolder),
                Tuple.Create(surfaceLine.DikeToeAtRiver, CharacteristicPointType.DikeToeAtRiver),
                Tuple.Create(surfaceLine.DitchDikeSide, CharacteristicPointType.DitchDikeSide),
                Tuple.Create(surfaceLine.DitchPolderSide, CharacteristicPointType.DitchPolderSide)
            };
            foreach (Tuple<Point3D, CharacteristicPointType> characteristicPointAssociation in characteristicPointAssociations)
            {
                CharacteristicPointEntity characteristicPointEntity = entity.CharacteristicPointEntities.First(cpe => cpe.Type == (short)characteristicPointAssociation.Item2);
                Assert.AreEqual(characteristicPointAssociation.Item1.X, characteristicPointEntity.X);
                Assert.AreEqual(characteristicPointAssociation.Item1.Y, characteristicPointEntity.Y);
                Assert.AreEqual(characteristicPointAssociation.Item1.Z, characteristicPointEntity.Z);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithCharacteristicPoints_CharacteristicPointEntitiesRegistered()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                PointsXml = new Point3DXmlSerializer().ToXml(new Point3D[0])
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            CharacteristicPointEntity[] characteristicPointEntities = entity.CharacteristicPointEntities
                                                                            .ToArray();
            foreach (CharacteristicPointEntity characteristicPointEntity in characteristicPointEntities)
            {
                context.CharacteristicPointEntities.Add(characteristicPointEntity);
            }
            registry.RemoveUntouched(context);
            Assert.AreEqual(characteristicPointEntities.Length, context.CharacteristicPointEntities.Count());

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(CharacteristicPointType.BottomDitchDikeSide)]
        [TestCase(CharacteristicPointType.BottomDitchPolderSide)]
        [TestCase(CharacteristicPointType.DikeToeAtPolder)]
        [TestCase(CharacteristicPointType.DikeToeAtRiver)]
        [TestCase(CharacteristicPointType.DitchDikeSide)]
        [TestCase(CharacteristicPointType.DitchPolderSide)]
        public void Update_SurfaceLineRemovedCharacteristicPoint_CharacteristicPointEntityRemoved(CharacteristicPointType type)
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();

            var characteristicPointEntity = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 1,
                Type = (short)type,
                X = surfaceLine.Points[0].X,
                Y = surfaceLine.Points[0].Y,
                Z = surfaceLine.Points[0].Z
            };
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                PointsXml = new Point3DXmlSerializer().ToXml(surfaceLine.Points),
                CharacteristicPointEntities =
                {
                    characteristicPointEntity
                }
            };

            context.SurfaceLineEntities.Add(entity);
            context.CharacteristicPointEntities.Add(characteristicPointEntity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            CollectionAssert.DoesNotContain(entity.CharacteristicPointEntities, characteristicPointEntity);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithChangedCharacteristicPoints_CharacteristicPointEntitiesUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            var originalGeometryPointMarkedCharacteristic = surfaceLine.Points[1];
            CharacteristicPointEntity[] characteristicPointEntities = CreateCharacteristicPointEntities(originalGeometryPointMarkedCharacteristic);

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                PointsXml = new Point3DXmlSerializer().ToXml(surfaceLine.Points)
            };
            foreach (CharacteristicPointEntity characteristicPointEntity in characteristicPointEntities)
            {
                entity.CharacteristicPointEntities.Add(characteristicPointEntity);
                context.CharacteristicPointEntities.Add(characteristicPointEntity);
            }
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            foreach (CharacteristicPointEntity characteristicPointEntity in characteristicPointEntities)
            {
                Assert.AreNotEqual(originalGeometryPointMarkedCharacteristic.X, characteristicPointEntity.X);
                Assert.AreNotEqual(originalGeometryPointMarkedCharacteristic.Y, characteristicPointEntity.Y);
                Assert.AreNotEqual(originalGeometryPointMarkedCharacteristic.Z, characteristicPointEntity.Z);
            }
            mocks.VerifyAll();
        }

        private CharacteristicPointEntity[] CreateCharacteristicPointEntities(Point3D point)
        {
            var characteristicPointEntities = new CharacteristicPointEntity[6];

            characteristicPointEntities[0] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 1,
                Type = (short)CharacteristicPointType.DikeToeAtRiver,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
            characteristicPointEntities[1] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 2,
                Type = (short)CharacteristicPointType.DikeToeAtPolder,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
            characteristicPointEntities[2] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 3,
                Type = (short)CharacteristicPointType.DitchDikeSide,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
            characteristicPointEntities[3] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 4,
                Type = (short)CharacteristicPointType.BottomDitchDikeSide,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
            characteristicPointEntities[4] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 5,
                Type = (short)CharacteristicPointType.BottomDitchPolderSide,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };
            characteristicPointEntities[5] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 6,
                Type = (short)CharacteristicPointType.DitchPolderSide,
                X = point.X,
                Y = point.Y,
                Z = point.Z
            };

            return characteristicPointEntities;
        }

        private static RingtoetsPipingSurfaceLine CreateSavedSurfaceLineWithGeometry()
        {
            var geometryPoints = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6)
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                StorageId = 14843246,
                Name = "<surfaceline with geometry data but no characteristic points>",
                ReferenceLineIntersectionWorldPoint = new Point2D(08.05, 20.16)
            };
            surfaceLine.SetGeometry(geometryPoints);
            return surfaceLine;
        }

        private static RingtoetsPipingSurfaceLine CreateSavedSurfaceLineWithData()
        {
            var geometryPoints = new[]
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
                StorageId = 4256,
                Name = "<surfaceline with all data set>",
                ReferenceLineIntersectionWorldPoint = new Point2D(07.11, 19.87)
            };
            surfaceLine.SetGeometry(geometryPoints);
            surfaceLine.SetBottomDitchDikeSideAt(geometryPoints[0]);
            surfaceLine.SetBottomDitchPolderSideAt(geometryPoints[2]);
            surfaceLine.SetDikeToeAtPolderAt(geometryPoints[3]);
            surfaceLine.SetDikeToeAtRiverAt(geometryPoints[4]);
            surfaceLine.SetDitchDikeSideAt(geometryPoints[5]);
            surfaceLine.SetDitchPolderSideAt(geometryPoints[7]);
            return surfaceLine;
        }

        private RingtoetsPipingSurfaceLine CreateSavedSurfaceLineWithoutGeometry()
        {
            return new RingtoetsPipingSurfaceLine
            {
                StorageId = 93584793,
                Name = "<surfaceline without geometry>",
                ReferenceLineIntersectionWorldPoint = new Point2D(1.2, 3.4)
            };
        }
    }
}