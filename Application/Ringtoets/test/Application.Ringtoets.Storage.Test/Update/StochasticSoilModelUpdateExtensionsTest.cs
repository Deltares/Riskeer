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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class StochasticSoilModelUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var soilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => soilModel.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ArgumentNullException()
        {
            // Setup
            var soilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    soilModel.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoStochasticSoilModel_EntityNotFoundException()
        {
            // Setup
            var soilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    soilModel.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilModelEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoStochasticSoilModelWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var soilModel = new StochasticSoilModel(-storageId, "name", "segment name")
            {
                StorageId = storageId,
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 2,
                Name = string.Empty,
                SegmentName = string.Empty
            });

            // Call
            TestDelegate test = () => soilModel.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilModelEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithStochasticSoilModel_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            string newName = "new name";
            string newSegmentName = "new segment name";
            var soilModel = new StochasticSoilModel(-1, newName, newSegmentName)
            {
                StorageId = 1,
            };

            var modelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1,
                Name = string.Empty,
                SegmentName = string.Empty
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(modelEntity);

            // Call
            soilModel.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, modelEntity.Name);
            Assert.AreEqual(newSegmentName, modelEntity.SegmentName);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithNewStochasticSoilProfile_StochasticSoilProfileAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();
            
            var soilModel = new StochasticSoilModel(-1, string.Empty, string.Empty)
            {
                StorageId = 1,
                StochasticSoilProfiles = 
                {
                    new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -1)
                    {
                        SoilProfile = new TestPipingSoilProfile()
                    }
                }
            };

            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(soilModelEntity);

            // Call
            soilModel.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, soilModelEntity.StochasticSoilProfileEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithUpdatedStochasticSoilProfile_NoNewStochasticSoilProfileAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var soilModel = new StochasticSoilModel(-1, string.Empty, string.Empty)
            {
                StorageId = 1,
                StochasticSoilProfiles = 
                {
                    new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, -1)
                    {
                        StorageId = 1,
                        SoilProfile = new TestPipingSoilProfile()
                    }
                }
            };

            var soilProfileEntity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 1
            };
            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 1,
                StochasticSoilProfileEntities = 
                {
                    soilProfileEntity
                }
            };

            ringtoetsEntities.StochasticSoilModelEntities.Add(soilModelEntity);
            ringtoetsEntities.StochasticSoilProfileEntities.Add(soilProfileEntity);

            // Call
            soilModel.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new [] {soilProfileEntity}, soilModelEntity.StochasticSoilProfileEntities);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithDifferentGeometry_StochasticSoilModelSegmentPointEntitiesReplacesWithNewEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            const long soilModelId = 1;
            var soilModel = new StochasticSoilModel(soilModelId, "A", "B")
            {
                StorageId = soilModelId
            };
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var originalPointEntity = new StochasticSoilModelSegmentPointEntity
            {
                StochasticSoilModelSegmentPointEntityId = 1,
                Order = 0,
                X = 5.5m,
                Y = 6.6m
            };
            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = soilModelId,
                StochasticSoilModelSegmentPointEntities =
                {
                    originalPointEntity
                }
            };
            context.StochasticSoilModelEntities.Add(soilModelEntity);
            context.StochasticSoilModelSegmentPointEntities.Add(originalPointEntity);
            
            var registry = new PersistenceRegistry();

            // Call
            soilModel.Update(registry, context);

            // Assert
            Assert.AreEqual(2, soilModelEntity.StochasticSoilModelSegmentPointEntities.Count);

            StochasticSoilModelSegmentPointEntity[] pointEntities = soilModelEntity.StochasticSoilModelSegmentPointEntities.ToArray();
            StochasticSoilModelSegmentPointEntity point1Entity = pointEntities[0];
            Assert.AreEqual(soilModel.Geometry[0].X, point1Entity.X);
            Assert.AreEqual(soilModel.Geometry[0].Y, point1Entity.Y);
            Assert.AreEqual(0, point1Entity.Order);

            StochasticSoilModelSegmentPointEntity point2Entity = pointEntities[1];
            Assert.AreEqual(soilModel.Geometry[1].X, point2Entity.X);
            Assert.AreEqual(soilModel.Geometry[1].Y, point2Entity.Y);
            Assert.AreEqual(1, point2Entity.Order);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithDifferentGeometry2_StochasticSoilModelSegmentPointEntitiesReplacesWithNewEntities()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            const long soilModelId = 1;
            var soilModel = new StochasticSoilModel(soilModelId, "A", "B")
            {
                StorageId = soilModelId
            };
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var originalPointEntity1 = new StochasticSoilModelSegmentPointEntity
            {
                StochasticSoilModelSegmentPointEntityId = 1,
                Order = 0,
                X = 5.5m,
                Y = 6.6m
            };
            var originalPointEntity2 = new StochasticSoilModelSegmentPointEntity
            {
                StochasticSoilModelSegmentPointEntityId = 2,
                Order = 1,
                X = 7.7m,
                Y = 8.8m
            };
            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = soilModelId,
                StochasticSoilModelSegmentPointEntities =
                {
                    originalPointEntity1,
                    originalPointEntity2
                }
            };
            context.StochasticSoilModelEntities.Add(soilModelEntity);
            context.StochasticSoilModelSegmentPointEntities.Add(originalPointEntity1);
            context.StochasticSoilModelSegmentPointEntities.Add(originalPointEntity2);

            var registry = new PersistenceRegistry();

            // Call
            soilModel.Update(registry, context);

            // Assert
            Assert.AreEqual(2, soilModelEntity.StochasticSoilModelSegmentPointEntities.Count);

            StochasticSoilModelSegmentPointEntity[] pointEntities = soilModelEntity.StochasticSoilModelSegmentPointEntities.ToArray();
            StochasticSoilModelSegmentPointEntity point1Entity = pointEntities[0];
            Assert.AreEqual(soilModel.Geometry[0].X, point1Entity.X);
            Assert.AreEqual(soilModel.Geometry[0].Y, point1Entity.Y);
            Assert.AreEqual(0, point1Entity.Order);

            StochasticSoilModelSegmentPointEntity point2Entity = pointEntities[1];
            Assert.AreEqual(soilModel.Geometry[1].X, point2Entity.X);
            Assert.AreEqual(soilModel.Geometry[1].Y, point2Entity.Y);
            Assert.AreEqual(1, point2Entity.Order);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithSameGeometry_NoStochasticSoilModelSegmentPointEntitiesAreReplaced()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities context = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            const long soilModelId = 1;
            var soilModel = new StochasticSoilModel(soilModelId, "A", "B")
            {
                StorageId = soilModelId
            };
            soilModel.Geometry.AddRange(new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            });

            var originalPointEntity1 = new StochasticSoilModelSegmentPointEntity
            {
                StochasticSoilModelSegmentPointEntityId = 1,
                Order = 0,
                X = Convert.ToDecimal(soilModel.Geometry[0].X),
                Y = Convert.ToDecimal(soilModel.Geometry[0].Y)
            };
            var originalPointEntity2 = new StochasticSoilModelSegmentPointEntity
            {
                StochasticSoilModelSegmentPointEntityId = 2,
                Order = 1,
                X = Convert.ToDecimal(soilModel.Geometry[1].X),
                Y = Convert.ToDecimal(soilModel.Geometry[1].Y)
            };
            var soilModelEntity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = soilModelId,
                StochasticSoilModelSegmentPointEntities =
                {
                    originalPointEntity1,
                    originalPointEntity2
                }
            };
            context.StochasticSoilModelEntities.Add(soilModelEntity);
            context.StochasticSoilModelSegmentPointEntities.Add(originalPointEntity1);
            context.StochasticSoilModelSegmentPointEntities.Add(originalPointEntity2);

            var registry = new PersistenceRegistry();

            // Call
            soilModel.Update(registry, context);

            // Assert
            StochasticSoilModelSegmentPointEntity[] segmentPointEntites = context.StochasticSoilModelSegmentPointEntities.ToArray();
            CollectionAssert.Contains(segmentPointEntites, originalPointEntity1);
            CollectionAssert.Contains(segmentPointEntites, originalPointEntity2);
            Assert.AreEqual(2, segmentPointEntites.Length);

            mocks.VerifyAll();
        }
    }
}