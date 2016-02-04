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
    public class DikeAssessmentSectionEntityPersistorTest
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
            DikeAssessmentSectionEntityPersistor p = new DikeAssessmentSectionEntityPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            mockRepository.ReplayAll();

            // Call
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<DikeAssessmentSectionEntityPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModels_NullDataset_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModels(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModels_EmptyDataset_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModels(parentNavigationProperty);

            // Assert
            Assert.DoesNotThrow(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModels_SingleEntityInDataset_EntityAsModelInList()
        {
            // Setup
            const long storageId = 1234L;
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId, Name = name, Norm = norm
                }
            };
            mockRepository.ReplayAll();

            // Call
            IEnumerable<DikeAssessmentSection> loadedModels = persistor.LoadModels(parentNavigationProperty);

            // Assert
            var loadedModelsList = loadedModels.ToList();
            Assert.AreEqual(parentNavigationProperty.Count, loadedModelsList.Count);
            DikeAssessmentSection section = loadedModelsList[0];
            Assert.AreEqual(storageId, section.StorageId);
            Assert.AreEqual(name, section.Name);
            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModels_MultipleEntitiesInDataset_EntitiesAsModelInList()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = 1, Name = "test1", Norm = 12
                },
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = 2, Name = "test2", Norm = 22
                }
            };
            mockRepository.ReplayAll();

            // Call
            IEnumerable<DikeAssessmentSection> loadedModels = persistor.LoadModels(parentNavigationProperty);

            // Assert
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var loadedModelsList = loadedModels.ToList();
            Assert.AreEqual(parentNavigationPropertyList.Count, loadedModelsList.Count);
            for (var i = 0; i < loadedModelsList.Count; i++)
            {
                Assert.AreEqual(parentNavigationPropertyList[i].DikeAssessmentSectionEntityId, loadedModelsList[i].StorageId);
                Assert.AreEqual(parentNavigationPropertyList[i].Name, loadedModelsList[i].Name);
                Assert.AreEqual(parentNavigationPropertyList[i].Norm, loadedModelsList[i].FailureMechanismContribution.Norm);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModels_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var listOfDikeAssessmentSections = mockRepository.StrictMock<IEnumerable<DikeAssessmentSection>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModels(null, listOfDikeAssessmentSections);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void InsertModels_NullListOfDikeAssessmentSections_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<DikeAssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModels(parentNavigationProperty, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModels_EmptyParentNavigationPropertySingleDikeAssessmentSectionWithoutStorageId_DikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.InsertModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModels_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithSameStorageId_DikeAssessmentSectionAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;

            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = storageId,
                Name = "Entity to delete"
            };
            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                entityToDelete
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DikeAssessmentSectionEntities).Return(dbset);

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.InsertModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var listOfDikeAssessmentSections = mockRepository.StrictMock<IEnumerable<DikeAssessmentSection>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModels(null, listOfDikeAssessmentSections);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateModels_NullListOfDikeAssessmentSections_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<DikeAssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModels(parentNavigationProperty, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_EmptyParentNavigationPropertySingleDikeAssessmentSectionWithoutStorageId_DikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_EmptyParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithUnknownStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = 4567L
                }
            };
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    StorageId = storageId
                }
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_DuplucateEntityInParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId
                },
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId
                }
            };
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    StorageId = storageId
                }
            };
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_UpdatedDikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId,
                    Name = "old name",
                    Norm = 1
                }
            };
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithoutStorageId_UpdatedDikeAssessmentSectionAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved
            const int norm = 30000;
            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };

            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                entityToDelete
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DikeAssessmentSectionEntities).Return(dbset);

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.DikeAssessmentSectionEntityId == storageId);
            Assert.IsInstanceOf<DikeAssessmentSectionEntity>(entity);
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_MultipleEntitiesInParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_UpdatedDikeAssessmentSectionAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const string name = "UpdatedName";
            const long storageId = 1234L;
            const int norm = 30000;
            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };
            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                entityToDelete,
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId,
                    Name = "Entity to update",
                    Norm = 1
                }
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DikeAssessmentSectionEntities).Return(dbset);

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>
            {
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.DikeAssessmentSectionEntityId == storageId);
            Assert.IsInstanceOf<DikeAssessmentSectionEntity>(entity);
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModels_MultipleEntitiesInParentNavigationPropertyEmptyDikeAssessmentSection_EmptyDatabaseSet()
        {
            // Setup
            DikeAssessmentSectionEntity firstEntityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 1234L,
                Name = "First entity to delete"
            };
            DikeAssessmentSectionEntity secondEntityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "Second entity to delete"
            };
            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(firstEntityToDelete)).Return(firstEntityToDelete);
            dbset.Expect(x => x.Remove(secondEntityToDelete)).Return(secondEntityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.DikeAssessmentSectionEntities).Return(dbset).Repeat.Twice();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            IEnumerable<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModels(parentNavigationProperty, listOfDikeAssessmentSections);

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
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
            var insertedDikeAssessmentSectionEntities = new List<DikeAssessmentSectionEntity>();
            var parentNavigationPropertyMock = mockRepository.StrictMock<ICollection<DikeAssessmentSectionEntity>>();
            parentNavigationPropertyMock.Expect(m => m.Count).Return(0).Repeat.Twice();
            parentNavigationPropertyMock.Expect(m => m.Add(null)).IgnoreArguments().WhenCalled(x =>
            {
                var insertedDikeAssessmentSectionEntity = x.Arguments.GetValue(0);
                Assert.IsInstanceOf<DikeAssessmentSectionEntity>(insertedDikeAssessmentSectionEntity);
                insertedDikeAssessmentSectionEntities.Add((DikeAssessmentSectionEntity) insertedDikeAssessmentSectionEntity);
            }).Repeat.Times(numberOfInserts);

            IList<DikeAssessmentSection> listOfDikeAssessmentSections = new List<DikeAssessmentSection>();
            for (var i = 0; i < numberOfInserts; i++)
            {
                listOfDikeAssessmentSections.Add(new DikeAssessmentSection
                {
                    StorageId = 0L
                });
            }

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            TestDelegate updateTest = () => persistor.UpdateModels(parentNavigationPropertyMock, listOfDikeAssessmentSections);
            Assert.DoesNotThrow(updateTest, "Precondition failed: persistor.UpdateModels");

            // Call
            for (var i = 0; i < insertedDikeAssessmentSectionEntities.Count; i++)
            {
                insertedDikeAssessmentSectionEntities[i].DikeAssessmentSectionEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(listOfDikeAssessmentSections.Count, insertedDikeAssessmentSectionEntities.Count);
            foreach (var entity in insertedDikeAssessmentSectionEntities)
            {
                var insertedModel = listOfDikeAssessmentSections.SingleOrDefault(x => x.StorageId == entity.DikeAssessmentSectionEntityId);
                Assert.IsInstanceOf<DikeAssessmentSection>(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}