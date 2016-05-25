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
                ReferenceLineIntersectionX = 9876.5432m,
                ReferenceLineIntersectionY = 9182.8374m
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

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
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
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

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

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
        public void Update_SurfaceLineWithUpdatedGeometry_SurfaceLineEntityUpdatedAndGeometryChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();

            var pointEntity1 = new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = 1,
                Order = 0,
                X = 1.0m,
                Y = 2.0m,
                Z = 3.0m
            };
            var pointEntity2 = new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = 2,
                Order = 1,
                X = 5.0m,
                Y = 2.0m,
                Z = 4.0m
            };

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
                Name = "<name>",
                ReferenceLineIntersectionX = 91.28m,
                ReferenceLineIntersectionY = 37.46m,
                SurfaceLinePointEntities =
                {
                    pointEntity1,
                    pointEntity2
                }
            };
            context.SurfaceLineEntities.Add(entity);
            context.SurfaceLinePointEntities.Add(pointEntity1);
            context.SurfaceLinePointEntities.Add(pointEntity2);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

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
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithGeometry();
            
            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId
            };
            context.SurfaceLineEntities.Add(entity);
            var createCollector = new PersistenceRegistry();
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                var geometryPoint = surfaceLine.Points[i];
                SurfaceLinePointEntity pointEntity = geometryPoint.CreateSurfaceLinePointEntity(createCollector, i);

                geometryPoint.StorageId = i + 1;
                pointEntity.SurfaceLinePointEntityId = geometryPoint.StorageId;

                entity.SurfaceLinePointEntities.Add(pointEntity);
                context.SurfaceLinePointEntities.Add(pointEntity);
            }

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

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
            var context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine surfaceLine = CreateSavedSurfaceLineWithData();

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId
            };
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            Assert.AreEqual(surfaceLine.Points.Length, entity.SurfaceLinePointEntities.Count);

            int dikeToeRiverIndex = GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DikeToeAtRiver);
            int dikeToePolderIndex = GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DikeToeAtPolder);
            int ditchDikeSideIndex = GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DitchDikeSide);
            int bottomDitchDikeSideIndex = GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.BottomDitchDikeSide);
            int buttomDitchPolderSideIndex = GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.BottomDitchPolderSide);
            int ditchPolderSide = GetGeometryPointIndexForCharacteristicPoint(surfaceLine, surfaceLine.DitchPolderSide);

            var surfaceLinePointEntities = entity.SurfaceLinePointEntities.OrderBy(pe => pe.Order).ToArray();
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                Point3D geometryPoint = surfaceLine.Points[i];
                SurfaceLinePointEntity pointEntity = surfaceLinePointEntities[i];

                Assert.AreEqual(geometryPoint.X, pointEntity.X);
                Assert.AreEqual(geometryPoint.Y, pointEntity.Y);
                Assert.AreEqual(geometryPoint.Z, pointEntity.Z);

                if (i == dikeToeRiverIndex)
                {
                    Assert.AreEqual(surfaceLine.DikeToeAtRiver.X, pointEntity.X);
                    Assert.AreEqual(surfaceLine.DikeToeAtRiver.Y, pointEntity.Y);
                    Assert.AreEqual(surfaceLine.DikeToeAtRiver.Z, pointEntity.Z);
                    CollectionAssert.Contains(pointEntity.CharacteristicPointEntities.Select(cpe => cpe.CharacteristicPointType),
                                              (short)CharacteristicPointType.DikeToeAtRiver);
                }
                if (i == dikeToePolderIndex)
                {
                    Assert.AreEqual(surfaceLine.DikeToeAtPolder.X, pointEntity.X);
                    Assert.AreEqual(surfaceLine.DikeToeAtPolder.Y, pointEntity.Y);
                    Assert.AreEqual(surfaceLine.DikeToeAtPolder.Z, pointEntity.Z);
                    CollectionAssert.Contains(pointEntity.CharacteristicPointEntities.Select(cpe => cpe.CharacteristicPointType),
                                              (short)CharacteristicPointType.DikeToeAtPolder);
                }
                if (i == ditchDikeSideIndex)
                {
                    Assert.AreEqual(surfaceLine.DitchDikeSide.X, pointEntity.X);
                    Assert.AreEqual(surfaceLine.DitchDikeSide.Y, pointEntity.Y);
                    Assert.AreEqual(surfaceLine.DitchDikeSide.Z, pointEntity.Z);
                    CollectionAssert.Contains(pointEntity.CharacteristicPointEntities.Select(cpe => cpe.CharacteristicPointType),
                                              (short)CharacteristicPointType.DitchDikeSide);
                }
                if (i == bottomDitchDikeSideIndex)
                {
                    Assert.AreEqual(surfaceLine.BottomDitchDikeSide.X, pointEntity.X);
                    Assert.AreEqual(surfaceLine.BottomDitchDikeSide.Y, pointEntity.Y);
                    Assert.AreEqual(surfaceLine.BottomDitchDikeSide.Z, pointEntity.Z);
                    CollectionAssert.Contains(pointEntity.CharacteristicPointEntities.Select(cpe => cpe.CharacteristicPointType),
                                              (short)CharacteristicPointType.BottomDitchDikeSide);
                }
                if (i == buttomDitchPolderSideIndex)
                {
                    Assert.AreEqual(surfaceLine.BottomDitchPolderSide.X, pointEntity.X);
                    Assert.AreEqual(surfaceLine.BottomDitchPolderSide.Y, pointEntity.Y);
                    Assert.AreEqual(surfaceLine.BottomDitchPolderSide.Z, pointEntity.Z);
                    CollectionAssert.Contains(pointEntity.CharacteristicPointEntities.Select(cpe => cpe.CharacteristicPointType),
                                              (short)CharacteristicPointType.BottomDitchPolderSide);
                }
                if (i == ditchPolderSide)
                {
                    Assert.AreEqual(surfaceLine.DitchPolderSide.X, pointEntity.X);
                    Assert.AreEqual(surfaceLine.DitchPolderSide.Y, pointEntity.Y);
                    Assert.AreEqual(surfaceLine.DitchPolderSide.Z, pointEntity.Z);
                    CollectionAssert.Contains(pointEntity.CharacteristicPointEntities.Select(cpe => cpe.CharacteristicPointType),
                                              (short)CharacteristicPointType.DitchPolderSide);
                }
            }
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

            int index = 0;
            var surfaceLinePointEntities = surfaceLine.Points.Select(p => new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = p.StorageId,
                Order = index++,
                X = Convert.ToDecimal(p.X),
                Y = Convert.ToDecimal(p.Y),
                Z = Convert.ToDecimal(p.Z),
            }).ToArray();

            var characteristicPointEntity = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 1,
                CharacteristicPointType = (short)type,
                SurfaceLinePointEntity = surfaceLinePointEntities[0]
            };
            surfaceLinePointEntities[0].CharacteristicPointEntities.Add(characteristicPointEntity);

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId,
            };
            foreach (SurfaceLinePointEntity pointEntity in surfaceLinePointEntities)
            {
                entity.SurfaceLinePointEntities.Add(pointEntity);
                context.SurfaceLinePointEntities.Add(pointEntity);
            }

            context.SurfaceLineEntities.Add(entity);
            context.CharacteristicPointEntities.Add(characteristicPointEntity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            Assert.IsNull(characteristicPointEntity.SurfaceLinePointEntity);

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

            int index = 0;
            var surfaceLinePointEntities = surfaceLine.Points.Select(p => new SurfaceLinePointEntity
            {
                SurfaceLinePointEntityId = p.StorageId,
                Order = index++,
                X = Convert.ToDecimal(p.X),
                Y = Convert.ToDecimal(p.Y),
                Z = Convert.ToDecimal(p.Z),
            }).ToArray();

            SurfaceLinePointEntity originalGeometryPointMarkedCharacteristic = surfaceLinePointEntities[1];
            var characteristicPointEntities = CreateCharacteristicPointEntities(originalGeometryPointMarkedCharacteristic);

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = surfaceLine.StorageId
            };
            foreach (SurfaceLinePointEntity pointEntity in surfaceLinePointEntities)
            {
                entity.SurfaceLinePointEntities.Add(pointEntity);
                context.SurfaceLinePointEntities.Add(pointEntity);
            }
            foreach (CharacteristicPointEntity characteristicPointEntity in characteristicPointEntities)
            {
                characteristicPointEntity.SurfaceLinePointEntity.CharacteristicPointEntities.Add(characteristicPointEntity);
                context.CharacteristicPointEntities.Add(characteristicPointEntity);
            }
            context.SurfaceLineEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            surfaceLine.Update(registry, context);

            // Assert
            foreach (CharacteristicPointEntity characteristicPointEntity in characteristicPointEntities)
            {
                Assert.AreNotEqual(originalGeometryPointMarkedCharacteristic, characteristicPointEntity.SurfaceLinePointEntity);
            }
            mocks.VerifyAll();
        }

        private CharacteristicPointEntity[] CreateCharacteristicPointEntities(SurfaceLinePointEntity pointEntity)
        {
            var characteristicPointEntities = new CharacteristicPointEntity[6];

            characteristicPointEntities[0] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 1,
                CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtRiver,
                SurfaceLinePointEntity = pointEntity
            };
            characteristicPointEntities[1] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 2,
                CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtPolder,
                SurfaceLinePointEntity = pointEntity
            };
            characteristicPointEntities[2] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 3,
                CharacteristicPointType = (short)CharacteristicPointType.DitchDikeSide,
                SurfaceLinePointEntity = pointEntity
            };
            characteristicPointEntities[3] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 4,
                CharacteristicPointType = (short)CharacteristicPointType.BottomDitchDikeSide,
                SurfaceLinePointEntity = pointEntity
            };
            characteristicPointEntities[4] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 5,
                CharacteristicPointType = (short)CharacteristicPointType.BottomDitchPolderSide,
                SurfaceLinePointEntity = pointEntity
            };
            characteristicPointEntities[5] = new CharacteristicPointEntity
            {
                CharacteristicPointEntityId = 6,
                CharacteristicPointType = (short)CharacteristicPointType.DitchPolderSide,
                SurfaceLinePointEntity = pointEntity
            };

            return characteristicPointEntities;
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
                new Point3D(1.1, 2.2, 3.3)
                {
                    StorageId = 38472
                },
                new Point3D(4.4, 5.5, 6.6)
                {
                    StorageId = 4095
                }
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
                new Point3D(1.1, 2.2, 3.3)
                {
                    StorageId = 340985
                },
                new Point3D(4.4, 5.5, 6.6)
                {
                    StorageId = 4058
                },
                new Point3D(7.7, 8.8, 9.9)
                {
                    StorageId = 39485
                },
                new Point3D(10.10, 11.11, 12.12)
                {
                    StorageId = 280347
                },
                new Point3D(13.13, 14.14, 15.15)
                {
                    StorageId = 43059
                },
                new Point3D(16.16, 17.17, 18.18)
                {
                    StorageId = 2390
                },
                new Point3D(19.19, 20.20, 21.21)
                {
                    StorageId = 43
                },
                new Point3D(22.22, 23.23, 24.24)
                {
                    StorageId = 325
                }
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