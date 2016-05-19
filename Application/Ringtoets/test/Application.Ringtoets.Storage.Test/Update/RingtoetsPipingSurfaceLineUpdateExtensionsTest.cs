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
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineUpdateExtensionsTest
    {
        [Test]
        public void Update_UpdateConversionCollectorIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            // Call
            TestDelegate call = () => surfaceLine.Update(null, context);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("collector", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var collector = new UpdateConversionCollector();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            // Call
            TestDelegate call = () => surfaceLine.Update(collector, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_SurfaceLineNotSaved_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var collector = new UpdateConversionCollector();

            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Precondition
            const long unsavedObjectId = 0;
            Assert.AreEqual(unsavedObjectId, surfaceLine.StorageId);

            // Call
            TestDelegate call = () => surfaceLine.Update(collector, context);

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
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithoutGeometry();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                Name = "<originalName>",
                ReferenceLineIntersectionX = 9876.5432m,
                ReferenceLineIntersectionY = 9182.8374m
            };
            context.SurfaceLineEntities.Add(entity);

            var collector = new UpdateConversionCollector();

            // Call
            surfaceLine.Update(collector, context);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            CollectionAssert.IsEmpty(entity.SurfaceLinePointEntities);
            CollectionAssert.IsEmpty(context.SurfaceLinePointEntities);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithNewGeometry_SurfaceLineEntityUpdatedAndGeometrySaved()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                Name = "<name>",
                ReferenceLineIntersectionX = 91.28m,
                ReferenceLineIntersectionY = 37.46m
            };
            context.SurfaceLineEntities.Add(entity);

            // Precondition
            CollectionAssert.IsEmpty(context.SurfaceLinePointEntities);

            var collector = new UpdateConversionCollector();

            // Call
            surfaceLine.Update(collector, context);

            // Assert
            Assert.AreEqual(surfaceLine.Name, entity.Name);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.X, entity.ReferenceLineIntersectionX);
            Assert.AreEqual(surfaceLine.ReferenceLineIntersectionWorldPoint.Y, entity.ReferenceLineIntersectionY);

            Assert.AreEqual(surfaceLine.Points.Length, entity.SurfaceLinePointEntities.Count);
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                Point3D geometryPoint = surfaceLine.Points[i];
                SurfaceLinePointEntity pointEntity = entity.SurfaceLinePointEntities.ElementAt(i);

                Assert.AreEqual(geometryPoint.X, pointEntity.X);
                Assert.AreEqual(geometryPoint.Y, pointEntity.Y);
                Assert.AreEqual(geometryPoint.Z, pointEntity.Z);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineIdenticalGeometry_SurfaceLineEntityUpdatedAndGeometrySaved()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();
            
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId
            };
            context.SurfaceLineEntities.Add(entity);
            var createCollector = new CreateConversionCollector();
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                var geometryPoint = surfaceLine.Points[i];
                SurfaceLinePointEntity pointEntity = geometryPoint.CreateSurfaceLinePoint(createCollector, i);

                geometryPoint.StorageId = i + 1;
                pointEntity.SurfaceLinePointEntityId = geometryPoint.StorageId;

                entity.SurfaceLinePointEntities.Add(pointEntity);
                context.SurfaceLinePointEntities.Add(pointEntity);
            }

            var updateCollector = new UpdateConversionCollector();

            // Call
            surfaceLine.Update(updateCollector, context);

            // Assert
            Assert.AreEqual(surfaceLine.Points.Length, entity.SurfaceLinePointEntities.Count);
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                Point3D geometryPoint = surfaceLine.Points[i];
                SurfaceLinePointEntity pointEntity = entity.SurfaceLinePointEntities.ElementAt(i);

                Assert.AreEqual(geometryPoint.X, pointEntity.X);
                Assert.AreEqual(geometryPoint.Y, pointEntity.Y);
                Assert.AreEqual(geometryPoint.Z, pointEntity.Z);
                Assert.AreEqual(geometryPoint.StorageId, pointEntity.SurfaceLinePointEntityId);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Update_SurfaceLineWithNewGeometryAndCharacteristicPoints_SurfaceLineEntityUpdatedAndGeometryPlusCharacteristicPointsSaved()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId
            };
            context.SurfaceLineEntities.Add(entity);

            var collector = new UpdateConversionCollector();

            // Call
            surfaceLine.Update(collector, context);

            // Assert
            Assert.AreEqual(surfaceLine.Points.Length, entity.SurfaceLinePointEntities.Count);
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                Point3D geometryPoint = surfaceLine.Points[i];
                SurfaceLinePointEntity pointEntity = entity.SurfaceLinePointEntities.ElementAt(i);

                Assert.AreEqual(geometryPoint.X, pointEntity.X);
                Assert.AreEqual(geometryPoint.Y, pointEntity.Y);
                Assert.AreEqual(geometryPoint.Z, pointEntity.Z);
            }

            Assert.AreEqual(surfaceLine.BottomDitchDikeSide.X, entity.BottomDitchDikeSidePointEntity.X);
            Assert.AreEqual(surfaceLine.BottomDitchDikeSide.Y, entity.BottomDitchDikeSidePointEntity.Y);
            Assert.AreEqual(surfaceLine.BottomDitchDikeSide.Z, entity.BottomDitchDikeSidePointEntity.Z);
            Assert.AreEqual(GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.BottomDitchDikeSide), entity.BottomDitchDikeSidePointEntity.Order);
            Assert.AreEqual(surfaceLine.BottomDitchPolderSide.X, entity.BottomDitchPolderSidePointEntity.X);
            Assert.AreEqual(surfaceLine.BottomDitchPolderSide.Y, entity.BottomDitchPolderSidePointEntity.Y);
            Assert.AreEqual(surfaceLine.BottomDitchPolderSide.Z, entity.BottomDitchPolderSidePointEntity.Z);
            Assert.AreEqual(GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.BottomDitchPolderSide), entity.BottomDitchPolderSidePointEntity.Order);
            Assert.AreEqual(surfaceLine.DikeToeAtPolder.X, entity.DikeToeAtPolderPointEntity.X);
            Assert.AreEqual(surfaceLine.DikeToeAtPolder.Y, entity.DikeToeAtPolderPointEntity.Y);
            Assert.AreEqual(surfaceLine.DikeToeAtPolder.Z, entity.DikeToeAtPolderPointEntity.Z);
            Assert.AreEqual(GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DikeToeAtPolder), entity.DikeToeAtPolderPointEntity.Order);
            Assert.AreEqual(surfaceLine.DikeToeAtRiver.X, entity.DikeToeAtRiverPointEntity.X);
            Assert.AreEqual(surfaceLine.DikeToeAtRiver.Y, entity.DikeToeAtRiverPointEntity.Y);
            Assert.AreEqual(surfaceLine.DikeToeAtRiver.Z, entity.DikeToeAtRiverPointEntity.Z);
            Assert.AreEqual(GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DikeToeAtRiver), entity.DikeToeAtRiverPointEntity.Order);
            Assert.AreEqual(surfaceLine.DitchDikeSide.X, entity.DitchDikeSidePointEntity.X);
            Assert.AreEqual(surfaceLine.DitchDikeSide.Y, entity.DitchDikeSidePointEntity.Y);
            Assert.AreEqual(surfaceLine.DitchDikeSide.Z, entity.DitchDikeSidePointEntity.Z);
            Assert.AreEqual(GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DitchDikeSide), entity.DitchDikeSidePointEntity.Order);
            Assert.AreEqual(surfaceLine.DitchPolderSide.X, entity.DitchPolderSidePointEntity.X);
            Assert.AreEqual(surfaceLine.DitchPolderSide.Y, entity.DitchPolderSidePointEntity.Y);
            Assert.AreEqual(surfaceLine.DitchPolderSide.Z, entity.DitchPolderSidePointEntity.Z);
            Assert.AreEqual(GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DitchPolderSide), entity.DitchPolderSidePointEntity.Order);
            mocks.VerifyAll();
        }

        private static int GetGeometryPointIndexForCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D characteristicPoint)
        {
            int index = -1;
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                if (surfaceLine.Points[i].Equals(characteristicPoint))
                {
                    return i;
                }
            }
            return index;
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