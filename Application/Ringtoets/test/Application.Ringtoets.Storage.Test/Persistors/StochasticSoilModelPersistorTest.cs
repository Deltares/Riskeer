using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class StochasticSoilModelPersistorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_EmptyDataSet_NewInstance()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            // Call
            StochasticSoilModelPersistor persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            // Assert
            Assert.IsInstanceOf<StochasticSoilModelPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullDataSet_ThrowsAgrumentNullException()
        {
            // Call
            TestDelegate test = () => new StochasticSoilModelPersistor(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("ringtoetsContext", exception.ParamName);
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            // Call
            TestDelegate test = () => persistor.LoadModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entities", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_ValidEntityValidModel_EntityAsModel()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            const string name = "someName";
            const string segmentName = "someSegmentName";
            long storageId = new Random(21).Next();
            var entity = new StochasticSoilModelEntity
            {
                Name = name,
                SegmentName = segmentName,
                StochasticSoilModelEntityId = storageId
            };

            // Call
            List<StochasticSoilModel> models = persistor.LoadModel(new List<StochasticSoilModelEntity>
            {
                entity
            }).ToList();

            // Assert
            Assert.AreEqual(1, models.Count);
            var model = models[0];
            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(segmentName, model.SegmentName);
            Assert.AreEqual(storageId, model.StorageId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            // Call
            TestDelegate test = () => persistor.InsertModel(null, new [] { new StochasticSoilModel(-1, string.Empty, string.Empty) });

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentNavigationProperty", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullModel_DoesNotAddEntityInParentNavigationProperty()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            var parentNavigationProperty = new List<StochasticSoilModelEntity>();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsEmpty(parentNavigationProperty);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertyStochasticSoilModelWithSameStorageId_StochasticSoilModelAsEntityInParentNavigationProperty()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            const long storageId = 1234L;
            StochasticSoilModelEntity entityToDelete = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId
            };

            IList<StochasticSoilModelEntity> parentNavigationProperty = new List<StochasticSoilModelEntity>
            {
                entityToDelete
            };

            StochasticSoilModel model = new StochasticSoilModel(-1, string.Empty, string.Empty)
            {
                StorageId = storageId
            };

            // Call
            persistor.InsertModel(parentNavigationProperty, new [] { model });

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.StochasticSoilModelEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullDatasetValidModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            var soilModels = new[]
            {
                new StochasticSoilModel(-1, string.Empty, string.Empty)
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, soilModels);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentNavigationProperty", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDatasetNullModel_DoesNotThrow()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilModelEntity> parentNavigationProperty = new List<StochasticSoilModelEntity>();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void UpdateModel_EmptyDatasetNullEntryInModel_ArgumentException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilModelEntity> parentNavigationProperty = new List<StochasticSoilModelEntity>();

            var soilModels = new StochasticSoilModel[]
            {
                null
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, soilModels);

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            Assert.AreEqual("A null StochasticSoilModel cannot be added", message);
        }

        [Test]
        public void UpdateModel_EmptyDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilModelEntity> parentNavigationProperty = new List<StochasticSoilModelEntity>();

            var soilModels = new[]
            {
                new StochasticSoilModel(-1, string.Empty, string.Empty)
                {
                    StorageId = storageId
                }
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, soilModels);

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilModelEntity' met id '{0}' is niet gevonden.", storageId);
            var exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplicateEntityInDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilModelEntity> parentNavigationProperty = new List<StochasticSoilModelEntity>
            {
                new StochasticSoilModelEntity
                {
                    StochasticSoilModelEntityId = storageId
                },
                new StochasticSoilModelEntity
                {
                    StochasticSoilModelEntityId = storageId
                }
            };

            var soilModels = new[]
            {
                new StochasticSoilModel(-1, string.Empty, string.Empty)
                {
                    StorageId = storageId
                }
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, soilModels);

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilModelEntity' met id '{0}' is niet gevonden.", storageId);
            var exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleStochasticSoilModelWithStorageId_UpdatedStochasticSoilModelAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            var parentNavigationProperty = new []
            {
                new StochasticSoilModelEntity
                {
                    StochasticSoilModelEntityId = storageId
                }
            };

            var name = "someName";
            var segmentName = "someSegmentName";
            var soilModels = new[]
            {
                new StochasticSoilModel(-1, name, segmentName)
                {
                    StorageId = storageId
                }
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, soilModels);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Length);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.StochasticSoilModelEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(segmentName, entity.SegmentName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NoStorageIdSet_InsertNewEntity()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            IList<StochasticSoilModelEntity> parentNavigationProperty = new List<StochasticSoilModelEntity>();

            var name = "someName";
            var segmentName = "someSegmentName";
            var soilModels = new[]
            {
                new StochasticSoilModel(-1, name, segmentName)
                {
                    StorageId = 0
                }
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, soilModels);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleStochasticSoilModelWithoutStorageId_UpdatedStochasticSoilModelAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            StochasticSoilModelEntity entityToDelete = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = 4567L,
                Name = "Entity to delete"
            };

            ringtoetsEntitiesMock.StochasticSoilModelEntities.Add(entityToDelete);

            var parentNavigationProperty = new List<StochasticSoilModelEntity>
            {
                entityToDelete
            };

            mockRepository.ReplayAll();

            StochasticSoilModelPersistor persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);
            var name = "someName";
            var segmentName = "someSegmentName";
            var soilModels = new[]
            {
                new StochasticSoilModel(-1, name, segmentName)
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, soilModels);

            // Assert
            CollectionAssert.IsEmpty(ringtoetsEntitiesMock.StochasticSoilModelEntities);
            Assert.AreEqual(2, parentNavigationProperty.Count);
            StochasticSoilModelEntity entity = parentNavigationProperty.SingleOrDefault(x => x.StochasticSoilModelEntityId == storageId);
            Assert.IsNotNull(entity);
            Assert.AreEqual(storageId, entity.StochasticSoilModelEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(segmentName, entity.SegmentName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            // Call
            TestDelegate test = () => persistor.PerformPostSaveActions();

            // Assert
            Assert.DoesNotThrow(test);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void PerformPostSaveActions_MultipleModelsInsertedWithoutStorageId_ModelsWithStorageId(int numberOfInserts)
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var parentNavigationProperty = new List<StochasticSoilModelEntity>();

            IList<StochasticSoilModel> stochasticSoilModel = new List<StochasticSoilModel>();
            for (var i = 0; i < numberOfInserts; i++)
            {
                stochasticSoilModel.Add(new StochasticSoilModel(-1, string.Empty, string.Empty)
                {
                    StorageId = 0L
                });
            }

            var persistor = new StochasticSoilModelPersistor(ringtoetsEntitiesMock);

            try
            {
                persistor.UpdateModel(parentNavigationProperty, stochasticSoilModel);
            }
            catch (Exception)
            {
                Assert.Fail("Precondition failed: persistor.UpdateModel");
            }

            // Call
            for (var i = 0; i < parentNavigationProperty.Count; i++)
            {
                parentNavigationProperty[i].StochasticSoilModelEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(stochasticSoilModel.Count, parentNavigationProperty.Count);
            foreach (var entity in parentNavigationProperty)
            {
                StochasticSoilModel insertedModel = stochasticSoilModel.SingleOrDefault(x => x.StorageId == entity.StochasticSoilModelEntityId);
                Assert.IsNotNull(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}