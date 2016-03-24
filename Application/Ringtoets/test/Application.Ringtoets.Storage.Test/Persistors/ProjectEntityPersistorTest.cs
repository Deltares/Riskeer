using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.Test.DbContext;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class ProjectEntityPersistorTest
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
            ProjectEntityPersistor p = new ProjectEntityPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();

            // Call
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<ProjectEntityPersistor>(persistor);
        }

        [Test]
        public void GetEntityAsModel_EmptyDataset_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.GetEntityAsModel();

            // Assert
            Assert.DoesNotThrow(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_SingleEntityInDataSet_ProjectEntityFromDataSetAsModel()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = description
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            Project model = persistor.GetEntityAsModel();

            // Assert
            Assert.IsInstanceOf<Project>(model);
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(description, model.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_NoProjectNameSet_ProjectEntityFromDataSetAsModelWithDefaultName()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            string defaultProjectName = new Project().Name;
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = description
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            Project model = persistor.GetEntityAsModel();

            // Assert
            Assert.IsInstanceOf<Project>(model);
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(defaultProjectName, model.Name);
            Assert.AreEqual(description, model.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_MultipleEntitiesInDataSet_ThrowsInvalidOperationException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = 1
                },
                new ProjectEntity
                {
                    ProjectEntityId = 2
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.GetEntityAsModel();

            // Assert
            Assert.Throws<InvalidOperationException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetEntityAsModel_SingleEntityWithChildrenInDataSet_ProjectEntityFromDataSetAsModel()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = description,
                    DikeAssessmentSectionEntities = new List<DikeAssessmentSectionEntity>
                    {
                        new DikeAssessmentSectionEntity
                        {
                            Norm = 1,
                            Order = 0
                        }
                    }
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            Project model = persistor.GetEntityAsModel();

            // Assert
            Assert.IsInstanceOf<Project>(model);
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(description, model.Description);
            Assert.AreEqual(1, model.Items.Count);
            Assert.AreEqual(1, model.Items.Count(i => i is DikeAssessmentSection));

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullData_ThrowsArgumentNullException()
        {
            // Setup
            var dbSetMethodAddWasHit = 0;
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            dbset.Stub(m => m.Add(new ProjectEntity())).IgnoreArguments().Return(new ProjectEntity())
                 .WhenCalled(invocation => dbSetMethodAddWasHit++);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.InsertModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual(0, dbSetMethodAddWasHit);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_ValidProject_UpdatedDataSet()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);

            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.InsertModel(project);

            // Assert
            var projectEntity = dbset.Local.First();
            Assert.AreNotEqual(project, projectEntity);
            Assert.AreEqual(storageId, projectEntity.ProjectEntityId);
            Assert.AreEqual(description, projectEntity.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_ValidProjectWithChildren_UpdatedProjectEntityWithChildren()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            Project project = new Project
            {
                StorageId = storageId,
                Description = description,
                Items =
                {
                    new DikeAssessmentSection()
                }
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            DatabaseSetHelper.AddSetExpectancy<HydraulicLocationEntity>(mockRepository, ringtoetsEntities);

            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.InsertModel(project);

            // Assert
            var projectEntity = dbset.Local.First();
            Assert.AreNotEqual(project, projectEntity);
            Assert.AreEqual(storageId, projectEntity.ProjectEntityId);
            Assert.AreEqual(description, projectEntity.Description);
            Assert.AreEqual(1, projectEntity.DikeAssessmentSectionEntities.Count);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullData_ThrowsArgumentNullException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_UnknownProject_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            Project project = new Project
            {
                StorageId = storageId
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = 2
                }
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateModel(project);

            // Assert
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_MultipleEqualEntitiesInDbSet_ThrownEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            var expectedInnerMessage = "Sequence contains more than one matching element";
            Project project = new Project
            {
                StorageId = storageId
            };
            var projectEntities = new ObservableCollection<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId
                },
                new ProjectEntity
                {
                    ProjectEntityId = storageId
                }
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, projectEntities);
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);

            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateModel(project);

            // Assert
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(expectedInnerMessage, exception.InnerException.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_ValidProject_UpdatedDataSet()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "<some description>";

            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };
            ProjectEntity entity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                entity
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);

            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.UpdateModel(project);

            // Assert
            Assert.IsInstanceOf<ProjectEntity>(entity);
            Assert.AreEqual(project.StorageId, entity.ProjectEntityId);
            Assert.AreEqual(project.Description, entity.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_ValidProjectWithChildren_UpdatedProjectEntityWithChildren()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";
            ProjectEntity projectEntity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };
            Project project = new Project
            {
                StorageId = storageId,
                Description = description,
                Items =
                {
                    new DikeAssessmentSection()
                }
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>
            {
                projectEntity
            });
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            DatabaseSetHelper.AddSetExpectancy<HydraulicLocationEntity>(mockRepository, ringtoetsEntities);

            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.UpdateModel(project);

            // Assert
            Assert.AreNotEqual(project, projectEntity);
            Assert.AreEqual(storageId, projectEntity.ProjectEntityId);
            Assert.AreEqual(description, projectEntity.Description);
            Assert.AreEqual(1, projectEntity.DikeAssessmentSectionEntities.Count);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.PerformPostSaveActions();

            // Assert
            Assert.DoesNotThrow(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_ModelInsertedWithoutStorageId_ModelWithStorageId()
        {
            // Setup
            const long storageId = 1234L;

            Project project = new Project
            {
                StorageId = 0L
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, new ObservableCollection<ProjectEntity>());

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset);

            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            TestDelegate insertTest = () => persistor.InsertModel(project);
            Assert.DoesNotThrow(insertTest, "Precondition failed: InsertModel failed");

            var insertedProjectEntity = dbset.Local.First();
            insertedProjectEntity.ProjectEntityId = storageId;
            Assert.AreEqual(0L, project.StorageId, "Precondition failed: Id should not have been set already");

            // Call
            persistor.PerformPostSaveActions();

            // Assert
            Assert.IsInstanceOf<Project>(project);
            Assert.AreEqual(storageId, project.StorageId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInDbSetSingleProjectWithoutStorageId_UpdatedProjectAsEntityInDbSetAndOthersDeletedInDbSet()
        {
            // Setup
            const string description = "test";
            const long storageId = 1L;
            ProjectEntity entityToDelete = new ProjectEntity
            {
                ProjectEntityId = 4567L,
                Description = "Entity to delete"
            };

            ObservableCollection<ProjectEntity> parentNavigationProperty = new ObservableCollection<ProjectEntity>
            {
                entityToDelete,
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = "Entity to delete"
                }
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset).Repeat.Twice();

            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            TestDelegate test = () => persistor.UpdateModel(project);
            Assert.DoesNotThrow(test, "Precondition failed: UpdateModel");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.ProjectEntityId == storageId);
            Assert.IsInstanceOf<ProjectEntity>(entity);
            Assert.AreEqual(storageId, entity.ProjectEntityId);
            Assert.AreEqual(storageId, entity.ProjectEntityId);
            Assert.AreEqual(description, entity.Description);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInDbSetEmptyProject_EmptyDbSet()
        {
            // Setup
            const long storageId = 1L;
            ProjectEntity firstEntityToDelete = new ProjectEntity
            {
                ProjectEntityId = 1234L,
                Description = "First entity to delete"
            };
            ProjectEntity secondEntityToDelete = new ProjectEntity
            {
                ProjectEntityId = 4567L,
                Description = "Second entity to delete"
            };
            ObservableCollection<ProjectEntity> parentNavigationProperty = new ObservableCollection<ProjectEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete,
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = "Entity to update"
                }
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(firstEntityToDelete)).Return(firstEntityToDelete);
            dbset.Expect(x => x.Remove(secondEntityToDelete)).Return(secondEntityToDelete);

            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntities.Expect(x => x.ProjectEntities).Return(dbset).Repeat.Times(3);

            Project project = new Project
            {
                StorageId = storageId
            };
            mockRepository.ReplayAll();
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            TestDelegate test = () => persistor.UpdateModel(project);
            Assert.DoesNotThrow(test, "Precondition failed: UpdateModel");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}