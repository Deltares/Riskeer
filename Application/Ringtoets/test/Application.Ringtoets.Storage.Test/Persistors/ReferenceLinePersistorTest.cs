using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
            referenceLine.SetGeometry(new []
            {
                new Point2D(1,1),
                new Point2D(3,2),
                new Point2D(1,3)
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
                new Point2D(1,1),
                new Point2D(3,2),
                new Point2D(1,3)
            });

            // Call
            persistor.InsertModel(entities, referenceLine);

            // Assert
            AssertCreatedEntities(entities, referenceLine);
            Assert.AreEqual(1, context.ReferenceLinePointEntities.Count());
            mocks.VerifyAll();
        }

        [Test]
        public void LoadModel_WithoutEntityCollection_ThrowsArgumentException()
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