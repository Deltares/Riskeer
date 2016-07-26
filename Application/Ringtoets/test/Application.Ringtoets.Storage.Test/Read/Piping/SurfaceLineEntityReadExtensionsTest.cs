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
        public void Read_SurfaceLineEntityWithoutGeometryPointEntities_ReturnSurfaceLine()
        {
            // Setup
            var collector = new ReadConversionCollector();

            const long id = 5317846874;
            const string name = "nice name!";
            const double intersectionX = 1.1;
            const double intersectionY = 2.2;

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = id,
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY
            };

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(id, surfaceLine.StorageId);
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

            const long id = 489357;
            const string name = "Better name.";
            const double intersectionX = 3.4;
            const double intersectionY = 7.5;
            
            var point1Entity = new SurfaceLinePointEntity
            {
                X = 1.1,
                Y = 2.2,
                Z = 3.3,
                Order = 0,
                SurfaceLinePointEntityId = 1
            };
            var point2Entity = new SurfaceLinePointEntity
            {
                X = 4.4,
                Y = 5.5,
                Z = 6.6,
                Order = 1,
                SurfaceLinePointEntityId = 2
            };
            var point3Entity = new SurfaceLinePointEntity
            {
                X = 7.7,
                Y = 8.8,
                Z = 9.9,
                Order = 2,
                SurfaceLinePointEntityId = 3
            };
            var sourceCollection = new[]
            {
                point1Entity,
                point2Entity,
                point3Entity
            };

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = id,
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY
            };
            foreach (SurfaceLinePointEntity pointEntity in sourceCollection)
            {
                entity.SurfaceLinePointEntities.Add(pointEntity);
            }

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(id, surfaceLine.StorageId);
            Assert.AreEqual(name, surfaceLine.Name);
            Assert.AreEqual(intersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(intersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            Point3D[] geometry = surfaceLine.Points.ToArray();
            Assert.AreEqual(sourceCollection.Length, geometry.Length);
            for (int i = 0; i < sourceCollection.Length; i++)
            {
                SurfaceLinePointEntity sourceEntity = sourceCollection[i];
                Point3D geometryPoint = geometry[i];

                Assert.AreEqual(sourceEntity.X, geometryPoint.X);
                Assert.AreEqual(sourceEntity.Y, geometryPoint.Y);
                Assert.AreEqual(sourceEntity.Z, geometryPoint.Z);
                Assert.AreEqual(sourceEntity.SurfaceLinePointEntityId, geometryPoint.StorageId);
            }

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

            const long id = 489357;
            const string name = "Better name.";
            const double intersectionX = 3.4;
            const double intersectionY = 7.5;

            var point1Entity = new SurfaceLinePointEntity
            {
                X = 1.1,
                Y = 2.2,
                Z = 3.3,
                Order = 0,
                SurfaceLinePointEntityId = 1
            };
            var point2Entity = new SurfaceLinePointEntity
            {
                X = 4.4,
                Y = 5.5,
                Z = 6.6,
                Order = 1,
                SurfaceLinePointEntityId = 2
            };
            var point3Entity = new SurfaceLinePointEntity
            {
                X = 7.7,
                Y = 8.8,
                Z = 9.9,
                Order = 2,
                SurfaceLinePointEntityId = 3
            };
            var point4Entity = new SurfaceLinePointEntity
            {
                X = 10.10,
                Y = 11.11,
                Z = 12.12,
                Order = 3,
                SurfaceLinePointEntityId = 4
            };
            var point5Entity = new SurfaceLinePointEntity
            {
                X = 13.13,
                Y = 14.14,
                Z = 15.15,
                Order = 4,
                SurfaceLinePointEntityId = 5
            };
            var point6Entity = new SurfaceLinePointEntity
            {
                X = 16.16,
                Y = 17.17,
                Z = 18.18,
                Order = 5,
                SurfaceLinePointEntityId = 6
            };
            var point7Entity = new SurfaceLinePointEntity
            {
                X = 19.19,
                Y = 20.20,
                Z = 21.21,
                Order = 6,
                SurfaceLinePointEntityId = 7
            };
            var point8Entity = new SurfaceLinePointEntity
            {
                X = 22.22,
                Y = 23.23,
                Z = 24.24,
                Order = 7,
                SurfaceLinePointEntityId = 8
            };
            var sourceCollection = new[]
            {
                point1Entity,
                point2Entity,
                point3Entity,
                point4Entity,
                point5Entity,
                point6Entity,
                point7Entity,
                point8Entity
            };

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = id,
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY
            };
            foreach (SurfaceLinePointEntity pointEntity in sourceCollection)
            {
                entity.SurfaceLinePointEntities.Add(pointEntity);
            }
            point2Entity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
            {
                CharacteristicPointType = (short)CharacteristicPointType.BottomDitchDikeSide,
                SurfaceLinePointEntity = point2Entity
            });
            point3Entity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
            {
                CharacteristicPointType = (short)CharacteristicPointType.BottomDitchPolderSide,
                SurfaceLinePointEntity = point3Entity
            });
            point4Entity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
            {
                CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtPolder,
                SurfaceLinePointEntity = point4Entity
            });
            point5Entity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
            {
                CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtRiver,
                SurfaceLinePointEntity = point5Entity
            });
            point6Entity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
            {
                CharacteristicPointType = (short)CharacteristicPointType.DitchDikeSide,
                SurfaceLinePointEntity = point6Entity
            });
            point7Entity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
            {
                CharacteristicPointType = (short)CharacteristicPointType.DitchPolderSide,
                SurfaceLinePointEntity = point7Entity
            });

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(id, surfaceLine.StorageId);
            Assert.AreEqual(name, surfaceLine.Name);
            Assert.AreEqual(intersectionX, surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            Assert.AreEqual(intersectionY, surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            Point3D[] geometry = surfaceLine.Points.ToArray();
            Assert.AreEqual(sourceCollection.Length, geometry.Length);
            for (int i = 0; i < sourceCollection.Length; i++)
            {
                SurfaceLinePointEntity sourceEntity = sourceCollection[i];
                Point3D geometryPoint = geometry[i];

                Assert.AreEqual(sourceEntity.X, geometryPoint.X);
                Assert.AreEqual(sourceEntity.Y, geometryPoint.Y);
                Assert.AreEqual(sourceEntity.Z, geometryPoint.Z);
                Assert.AreEqual(sourceEntity.SurfaceLinePointEntityId, geometryPoint.StorageId);
            }

            Assert.AreSame(geometry[1], surfaceLine.BottomDitchDikeSide);
            Assert.AreSame(geometry[2], surfaceLine.BottomDitchPolderSide);
            Assert.AreSame(geometry[3], surfaceLine.DikeToeAtPolder);
            Assert.AreSame(geometry[4], surfaceLine.DikeToeAtRiver);
            Assert.AreSame(geometry[5], surfaceLine.DitchDikeSide);
            Assert.AreSame(geometry[6], surfaceLine.DitchPolderSide);
        }

        [Test]
        public void Read_SurfaceLineEntityWithGeometryPointEntityMarkedForAllCharacteristicPoints_ReturnFullSurfaceLineWithCharacteristicPointsToOneGeometryPoint()
        {
            // Setup
            var collector = new ReadConversionCollector();

            const long id = 489357;
            const string name = "Better name.";
            const double intersectionX = 3.4;
            const double intersectionY = 7.5;

            var surfaceLinePointEntity1 = new SurfaceLinePointEntity
            {
                X = 1.0,
                Y = 2.0,
                Z = 3.0,
                Order = 0,
                SurfaceLinePointEntityId = 1,
                CharacteristicPointEntities =
                {
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.BottomDitchDikeSide
                    },
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.BottomDitchPolderSide
                    },
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtPolder
                    },
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtPolder
                    },
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtRiver
                    },
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.DitchDikeSide
                    },
                    new CharacteristicPointEntity
                    {
                        CharacteristicPointType = (short)CharacteristicPointType.DitchPolderSide
                    }
                }
            };
            foreach (CharacteristicPointEntity characteristicPointEntity in surfaceLinePointEntity1.CharacteristicPointEntities)
            {
                characteristicPointEntity.SurfaceLinePointEntity = surfaceLinePointEntity1;
            }
            var surfaceLinePointEntity2 = new SurfaceLinePointEntity
            {
                X = 5.0,
                Y = 6.0,
                Z = 7.0,
                Order = 1,
                SurfaceLinePointEntityId = 2
            };

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = id,
                Name = name,
                ReferenceLineIntersectionX = intersectionX,
                ReferenceLineIntersectionY = intersectionY
            };
            entity.SurfaceLinePointEntities.Add(surfaceLinePointEntity1);
            entity.SurfaceLinePointEntities.Add(surfaceLinePointEntity2);

            // Call
            RingtoetsPipingSurfaceLine surfaceLine = entity.Read(collector);

            // Assert
            Assert.AreEqual(id, surfaceLine.StorageId);
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

            const long id = 9348765;

            var entity = new SurfaceLineEntity
            {
                SurfaceLineEntityId = id
            };

            // Call
            RingtoetsPipingSurfaceLine surfaceLine1 = entity.Read(collector);
            RingtoetsPipingSurfaceLine surfaceLine2 = entity.Read(collector);

            // Assert
            Assert.AreSame(surfaceLine1, surfaceLine2);
        }
    }
}