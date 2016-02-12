using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.Test.DbContext;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class PipingFailureMechanismPersistorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullDataSet_ThrowsArgumentNullException()
        {
            // Call
            PipingFailureMechanismPersistor p = new PipingFailureMechanismPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            mockRepository.ReplayAll();

            // Call
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanismPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntityValidModel_ThrowsArgumentNullException()
        {
            // Setup
            PipingFailureMechanism model = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => PipingFailureMechanismPersistor.LoadModel(model, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void LoadModel_ValidEntityNullModel_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismEntity entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => PipingFailureMechanismPersistor.LoadModel(null, entity);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void LoadModel_EntityWithIncorrectTypeValidModel_UpdatedModel()
        {
            // Setup
            const long storageId = 1234L;
            PipingFailureMechanism model = new PipingFailureMechanism();
            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.StoneRevetmentFailureMechanism,
            };

            // Call
            TestDelegate test = () => PipingFailureMechanismPersistor.LoadModel(model, entity);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void LoadModel_ValidEntityValidModel_UpdatedModel()
        {
            // Setup
            const long storageId = 1234L;
            PipingFailureMechanism model = new PipingFailureMechanism();
            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism,
            };

            // Call
            PipingFailureMechanismPersistor.LoadModel(model, entity);

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanism>(model);
            Assert.AreEqual(model.StorageId, entity.FailureMechanismEntityId);
        }

        [Test]
        public void InsertModel_NullDatasetValidModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism model = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(null, model);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyDatasetNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyDatasetValidModel_ValidEntityInDataModel()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();
            PipingFailureMechanism model = new PipingFailureMechanism();

            mockRepository.ReplayAll();

            // Call
            persistor.InsertModel(parentNavigationProperty, model);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var entity = parentNavigationProperty[0];
            Assert.AreNotEqual(model, entity);
            Assert.AreEqual((int) FailureMechanismType.PipingFailureMechanism, entity.FailureMechanismType);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertySinglePipingFailureMechanismWithSameStorageId_PipingFailureMechanismAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;
            FailureMechanismEntity entityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                entityToDelete
            };
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            persistor.InsertModel(parentNavigationProperty, pipingFailureMechanism);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullDataSetValidModel_ThrowsArgumentNullException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, model);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDataSetNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplicateEntityInDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                new FailureMechanismEntity
                {
                    FailureMechanismEntityId = storageId,
                    FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
                },
                new FailureMechanismEntity
                {
                    FailureMechanismEntityId = storageId,
                    FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
                }
            };
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySinglePipingFailureMechanismWithStorageId_UpdatedPipingFailureMechanismAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            ICollection<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                new FailureMechanismEntity
                {
                    FailureMechanismEntityId = storageId,
                    FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
                }
            };
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInParentNavigationPropertySinglePipingFailureMechanismWithoutStorageId_UpdatedPipingFailureMechanismAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved
            FailureMechanismEntity entityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4567L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };

            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                entityToDelete
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.FailureMechanismEntities).Return(dbset);

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, pipingFailureMechanism);
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.FailureMechanismEntityId == storageId);
            Assert.IsInstanceOf<FailureMechanismEntity>(entity);
            Assert.AreEqual(storageId, entity.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInnParentNavigationPropertySingleModelStorageId_UpdatedPipingFailureMechanismAsEntityAndOtherDeletedInDbSet()
        {
            // Setup
            const long storageId = 1234L;
            FailureMechanismEntity entityToUpdate = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            FailureMechanismEntity entityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4567L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };

            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                entityToDelete,
                entityToUpdate
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.FailureMechanismEntities).Return(dbset);

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            TestDelegate updateTest = () => persistor.UpdateModel(parentNavigationProperty, pipingFailureMechanism);
            Assert.DoesNotThrow(updateTest, "Precondition failed: Update should not throw exception.");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            Assert.IsInstanceOf<FailureMechanismEntity>(entityToUpdate);
            Assert.AreEqual(storageId, entityToUpdate.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertyEmptyPipingFailureMechanism_EmptyDatabaseSet()
        {
            // Setup
            FailureMechanismEntity firstEntityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1234L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            FailureMechanismEntity secondEntityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4567L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(firstEntityToDelete)).Return(firstEntityToDelete);
            dbset.Expect(x => x.Remove(secondEntityToDelete)).Return(secondEntityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.FailureMechanismEntities).Return(dbset).Repeat.Twice();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism();
            mockRepository.ReplayAll();

            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, pipingFailureMechanism);
            Assert.DoesNotThrow(test, "Precondition failed: UpdateModel");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

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
            var insertedFailureMechanismEntities = new List<FailureMechanismEntity>();
            var parentNavigationPropertyMock = mockRepository.StrictMock<ICollection<FailureMechanismEntity>>();
            parentNavigationPropertyMock.Expect(m => m.Add(null)).IgnoreArguments().WhenCalled(x =>
            {
                var insertedDikeAssessmentSectionEntity = x.Arguments.GetValue(0);
                Assert.IsInstanceOf<FailureMechanismEntity>(insertedDikeAssessmentSectionEntity);
                insertedFailureMechanismEntities.Add((FailureMechanismEntity) insertedDikeAssessmentSectionEntity);
            }).Repeat.Times(numberOfInserts);

            IList<PipingFailureMechanism> pipingFailureMechanisms = new List<PipingFailureMechanism>();
            for (var i = 0; i < numberOfInserts; i++)
            {
                pipingFailureMechanisms.Add(new PipingFailureMechanism
                {
                    StorageId = 0L
                });
            }

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            foreach (var pipingFailureMechanism in pipingFailureMechanisms)
            {
                try
                {
                    persistor.UpdateModel(parentNavigationPropertyMock, pipingFailureMechanism);
                }
                catch (Exception)
                {
                    Assert.Fail("Precondition failed: persistor.UpdateModel");
                }
            }

            // Call
            for (var i = 0; i < insertedFailureMechanismEntities.Count; i++)
            {
                insertedFailureMechanismEntities[i].FailureMechanismEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(pipingFailureMechanisms.Count, insertedFailureMechanismEntities.Count);
            foreach (var entity in insertedFailureMechanismEntities)
            {
                var insertedModel = pipingFailureMechanisms.SingleOrDefault(x => x.StorageId == entity.FailureMechanismEntityId);
                Assert.IsInstanceOf<PipingFailureMechanism>(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}