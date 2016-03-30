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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class ReferenceLinePersistorTest
    {
        [Test]
        public void Constructor_WithoutContext_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReferenceLinePersistor(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("ringtoetsEntities", parameter);
        }

        [Test]
        public void Constructor_WithContext_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new ReferenceLinePersistor(context);

            // Assert
            Assert.DoesNotThrow(call);
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_WithoutEntityCollection_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            TestDelegate call = () => persistor.InsertModel(null, null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entityCollection", parameter);
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_WithEmptyEntityCollectionWithoutReferenceLine_NoChange()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            var entities = new List<ReferenceLinePointEntity>();
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            persistor.InsertModel(entities, null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_WithEmptyEntityCollectionWithReferenceLine_AddsNewEntitiesToContext()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            var entities = new List<ReferenceLinePointEntity>();
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(3, 2),
                new Point2D(1, 3)
            });

            // Call
            persistor.InsertModel(entities, referenceLine);

            // Assert
            AssertCreatedEntities(entities, referenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_WithNonEmptyEntityCollectionWithReferenceLine_EntityCollectionClearedAddsNewEntitiesToContext()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var entities = new List<ReferenceLinePointEntity>();
            for (int i = 0; i < 3; i++)
            {
                entities.Add(new ReferenceLinePointEntity
                {
                    Order = i
                });
            }

            context.ReferenceLinePointEntities.Add(entities[0]);
            context.ReferenceLinePointEntities.Add(entities[1]);
            context.ReferenceLinePointEntities.Add(new ReferenceLinePointEntity());

            var persistor = new ReferenceLinePersistor(context);
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(3, 2),
                new Point2D(1, 3)
            });

            // Call
            persistor.InsertModel(entities, referenceLine);

            // Assert
            AssertCreatedEntities(entities, referenceLine);
            Assert.AreEqual(1, context.ReferenceLinePointEntities.Count());
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyCollectionNullReferenceLine_ShouldNotClearCollection()
        {
            // Setup
            var backingList = new List<ReferenceLinePointEntity>();

            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            var entities = mocks.StrictMock<ICollection<ReferenceLinePointEntity>>();
            entities.Expect(e => e.GetEnumerator()).Return(backingList.GetEnumerator());
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            persistor.InsertModel(entities, null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyCollectionReferenceLineWithPoint_ShouldAddPointsToCollection()
        {
            // Setup
            var entities = new List<ReferenceLinePointEntity>();

            var referenceLine = new ReferenceLine();
            Point2D point = new Point2D(1.2, 3.5);
            referenceLine.SetGeometry(new[]
            {
                point
            });

            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            persistor.InsertModel(entities, referenceLine);

            // Assert
            Assert.AreEqual(1, entities.Count);
            Assert.AreEqual(point.X, entities.First().X);
            Assert.AreEqual(point.Y, entities.First().Y);
            Assert.AreEqual(0, entities.First().Order);
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_CollectionWithPointReferenceLineWithPoint_DoesNotChangeCollection()
        {
            // Setup
            var referenceLinePointEntity = new ReferenceLinePointEntity
            {
                X = 0, Y = 0, Order = 0
            };
            var entities = new List<ReferenceLinePointEntity>(new[]
            {
                referenceLinePointEntity
            });
            var referenceLine = new ReferenceLine();
            Point2D point = new Point2D(0, 0);
            referenceLine.SetGeometry(new[]
            {
                point
            });

            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            persistor.InsertModel(entities, referenceLine);

            // Assert
            Assert.AreEqual(new[]
            {
                referenceLinePointEntity
            }, entities);
            mocks.VerifyAll();
        }

        [Test]
        public void InsertModel_CollectionWithPointReferenceLineNull_ClearsCollection()
        {
            // Setup
            var referenceLinePointEntity = new ReferenceLinePointEntity
            {
                X = 0,
                Y = 0,
                Order = 0
            };
            var entities = new List<ReferenceLinePointEntity>(new[]
            {
                referenceLinePointEntity
            });

            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            persistor.InsertModel(entities, null);

            // Assert
            Assert.AreEqual(0, entities.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_WithoutEntityCollection_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var persistor = new ReferenceLinePersistor(context);

            // Call
            TestDelegate call = () => persistor.LoadModel(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("entityCollection", parameter);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_EmptyEntityCollection_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var entities = new List<ReferenceLinePointEntity>();
            var persistor = new ReferenceLinePersistor(context);

            // Call
            var referenceLine = persistor.LoadModel(entities);

            // Assert
            Assert.IsNull(referenceLine);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_EntityCollectionWithOrderedElements_ReturnsEqualReferenceLineInstance()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var entities = new List<ReferenceLinePointEntity>
            {
                new ReferenceLinePointEntity
                {
                    Order = 0,
                    X = 1,
                    Y = 2
                },
                new ReferenceLinePointEntity
                {
                    Order = 1,
                    X = 3,
                    Y = 2
                },
                new ReferenceLinePointEntity
                {
                    Order = 2,
                    X = 5,
                    Y = 3
                }
            };
            var persistor = new ReferenceLinePersistor(context);

            // Call
            var referenceLine = persistor.LoadModel(entities);

            // Assert
            AssertCreatedReferenceLine(referenceLine, entities);
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_EntityCollectionWithUnorderedElements_ReturnsEqualReferenceLineInstance()
        {
            // Setup
            var mocks = new MockRepository();
            var context = RingtoetsEntitiesHelper.Create(mocks);
            mocks.ReplayAll();

            var entities = new List<ReferenceLinePointEntity>
            {
                new ReferenceLinePointEntity
                {
                    Order = 0,
                    X = 1,
                    Y = 2
                },
                new ReferenceLinePointEntity
                {
                    Order = 2,
                    X = 5,
                    Y = 3
                },
                new ReferenceLinePointEntity
                {
                    Order = 1,
                    X = 3,
                    Y = 2
                }
            };
            var persistor = new ReferenceLinePersistor(context);

            // Call
            var referenceLine = persistor.LoadModel(entities);

            // Assert
            AssertCreatedReferenceLine(referenceLine, entities);
            mocks.VerifyAll();
        }

        private void AssertCreatedEntities(IList<ReferenceLinePointEntity> entities, ReferenceLine referenceLine)
        {
            Assert.AreEqual(entities.Count, referenceLine.Points.Count());
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                var point = referenceLine.Points.ElementAt(i);

                Assert.AreEqual(point.X, entity.X);
                Assert.AreEqual(referenceLine.Points.ElementAt(i).Y, entity.Y);
                Assert.AreEqual(i, entity.Order);
                Assert.AreEqual(0, entity.ReferenceLinePointEntityId);
            }
        }

        private void AssertCreatedReferenceLine(ReferenceLine referenceLine, List<ReferenceLinePointEntity> entities)
        {
            Assert.AreEqual(entities.Count, referenceLine.Points.Count());
            for (int i = 0; i < referenceLine.Points.Count(); i++)
            {
                var point = referenceLine.Points.ElementAt(i);
                var entity = entities.First(e => e.Order == i);

                Assert.AreEqual(entity.X, point.X);
                Assert.AreEqual(entity.Y, point.Y);
            }
        }
    }
}