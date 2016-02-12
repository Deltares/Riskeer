using System;
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.Test.DbContext;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class DuneAssessmentSectionEntityPersistorTest
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
            DuneAssessmentSectionEntityPersistor p = new DuneAssessmentSectionEntityPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            mockRepository.ReplayAll();

            // Call
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<DuneAssessmentSectionEntityPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_ValidEntity_EntityAsModel()
        {
            // Setup
            const long storageId = 1234L;
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            var entity = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = storageId,
                Name = name,
                Norm = norm
            };
            mockRepository.ReplayAll();

            // Call
            DuneAssessmentSection section = persistor.LoadModel(entity);

            // Assert
            Assert.AreEqual(storageId, section.StorageId);
            Assert.AreEqual(name, section.Name);
            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_MultipleEntitiesInDataset_EntitiesAsModel()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = 1, Name = "test1", Norm = 12
                },
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = 2, Name = "test2", Norm = 22
                }
            };
            mockRepository.ReplayAll();

            // Call
            var loadedModels = parentNavigationProperty.Select(entity => persistor.LoadModel(entity));

            // Assert
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var loadedModelsList = loadedModels.ToList();
            Assert.AreEqual(parentNavigationPropertyList.Count, loadedModelsList.Count);
            for (var i = 0; i < loadedModelsList.Count; i++)
            {
                Assert.AreEqual(parentNavigationPropertyList[i].DuneAssessmentSectionEntityId, loadedModelsList[i].StorageId);
                Assert.AreEqual(parentNavigationPropertyList[i].Name, loadedModelsList[i].Name);
                Assert.AreEqual(parentNavigationPropertyList[i].Norm, loadedModelsList[i].FailureMechanismContribution.Norm);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            var duneAssessmentSection = new DuneAssessmentSection();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(null, duneAssessmentSection, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void InsertModel_NullDuneAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<DuneAssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyParentNavigationPropertySingleDuneAssessmentSectionWithoutStorageId_DuneAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>();
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.InsertModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertySingleDuneAssessmentSectionWithSameStorageId_DuneAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;

            DuneAssessmentSectionEntity entityToDelete = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = storageId,
                Name = "Entity to delete"
            };
            IList<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                entityToDelete
            };
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();

            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);

            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.InsertModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            var duneAssessmentSection = new DuneAssessmentSection();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, duneAssessmentSection, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateModel_NullDuneAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<DuneAssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyParentNavigationPropertySingleDuneAssessmentSectionWithoutStorageId_DuneAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>();
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            }
                ;
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyParentNavigationPropertySingleDuneAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>();
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleDuneAssessmentSectionWithUnknownStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = 4567L
                }
            };
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplucateEntityInParentNavigationPropertySingleDuneAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = storageId
                },
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = storageId
                }
            };
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleDuneAssessmentSectionWithStorageId_UpdatedDuneAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = storageId,
                    Name = "old name",
                    Norm = 1
                }
            };
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_MultipleEntitiesInParentNavigationPropertySingleDuneAssessmentSectionWithStorageId_UpdatedDuneAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "UpdatedName";
            const long storageId = 1234L;
            const int norm = 30000;
            DuneAssessmentSectionEntity entityToDelete = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };
            IList<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                entityToDelete,
                new DuneAssessmentSectionEntity
                {
                    DuneAssessmentSectionEntityId = storageId,
                    Name = "Entity to update",
                    Norm = 1
                }
            };

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);

            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.DuneAssessmentSectionEntityId == storageId);
            Assert.IsInstanceOf<DuneAssessmentSectionEntity>(entity);
            Assert.AreEqual(storageId, entity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInParentNavigationPropertySingleDuneAssessmentSectionWithoutStorageId_UpdatedDuneAssessmentSectionAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved
            const int norm = 30000;
            DuneAssessmentSectionEntity entityToDelete = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };

            IList<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                entityToDelete
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DuneAssessmentSectionEntities).Return(dbset);

            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.DuneAssessmentSectionEntityId == storageId);
            Assert.IsInstanceOf<DuneAssessmentSectionEntity>(entity);
            Assert.AreEqual(storageId, entity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertySingleModelStorageId_UpdatedDuneAssessmentSectionAsEntityAndOtherDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            DuneAssessmentSectionEntity entityToUpdate = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = storageId,
                Name = "Entity to update"
            };
            DuneAssessmentSectionEntity entityToDelete = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = 4567L,
                Name = "First entity to delete"
            };

            IList<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                entityToDelete,
                entityToUpdate
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DuneAssessmentSectionEntities).Return(dbset);

            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                StorageId = storageId
            };
            mockRepository.ReplayAll();

            TestDelegate updateTest = () => persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);
            Assert.DoesNotThrow(updateTest, "Precondition failed: Update should not throw exception.");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            Assert.IsInstanceOf<DuneAssessmentSectionEntity>(entityToUpdate);
            Assert.AreEqual(storageId, entityToUpdate.DuneAssessmentSectionEntityId);
            Assert.AreEqual(name, entityToUpdate.Name);
            Assert.AreEqual(norm, entityToUpdate.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertyEmptyDuneAssessmentSection_EmptyDatabaseSet()
        {
            // Setup
            DuneAssessmentSectionEntity firstEntityToDelete = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = 1234L,
                Name = "First entity to delete"
            };
            DuneAssessmentSectionEntity secondEntityToDelete = new DuneAssessmentSectionEntity
            {
                DuneAssessmentSectionEntityId = 4567L,
                Name = "Second entity to delete"
            };
            IList<DuneAssessmentSectionEntity> parentNavigationProperty = new List<DuneAssessmentSectionEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(firstEntityToDelete)).Return(firstEntityToDelete);
            dbset.Expect(x => x.Remove(secondEntityToDelete)).Return(secondEntityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DuneAssessmentSectionEntities).Return(dbset).Repeat.Twice();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);

            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection();
            mockRepository.ReplayAll();

            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, duneAssessmentSection, 0);
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
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
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
            var insertedDuneAssessmentSectionEntities = new List<DuneAssessmentSectionEntity>();
            var parentNavigationPropertyMock = mockRepository.StrictMock<ICollection<DuneAssessmentSectionEntity>>();
            parentNavigationPropertyMock.Expect(m => m.Add(null)).IgnoreArguments().WhenCalled(x =>
            {
                var insertedDuneAssessmentSectionEntity = x.Arguments.GetValue(0);
                Assert.IsInstanceOf<DuneAssessmentSectionEntity>(insertedDuneAssessmentSectionEntity);
                insertedDuneAssessmentSectionEntities.Add((DuneAssessmentSectionEntity) insertedDuneAssessmentSectionEntity);
            }).Repeat.Times(numberOfInserts);

            IList<DuneAssessmentSection> duneAssessmentSections = new List<DuneAssessmentSection>();
            for (var i = 0; i < numberOfInserts; i++)
            {
                duneAssessmentSections.Add(new DuneAssessmentSection
                {
                    StorageId = 0L
                });
            }

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DuneAssessmentSectionEntityPersistor persistor = new DuneAssessmentSectionEntityPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            foreach (var duneAssessmentSection in duneAssessmentSections)
            {
                try
                {
                    persistor.UpdateModel(parentNavigationPropertyMock, duneAssessmentSection, 0);
                }
                catch (Exception)
                {
                    Assert.Fail("Precondition failed: persistor.UpdateModel");
                }
            }

            // Call
            for (var i = 0; i < insertedDuneAssessmentSectionEntities.Count; i++)
            {
                insertedDuneAssessmentSectionEntities[i].DuneAssessmentSectionEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(duneAssessmentSections.Count, insertedDuneAssessmentSectionEntities.Count);
            foreach (var entity in insertedDuneAssessmentSectionEntities)
            {
                var insertedModel = duneAssessmentSections.SingleOrDefault(x => x.StorageId == entity.DuneAssessmentSectionEntityId);
                Assert.IsInstanceOf<DuneAssessmentSection>(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}