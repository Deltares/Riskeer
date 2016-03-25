using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.ProjectEntities.Add(
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Description = description
                });
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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.ProjectEntities.Add(
            new ProjectEntity
            {
                ProjectEntityId = storageId,
                Description = description
            });

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.ProjectEntities.Add(
                new ProjectEntity
                {
                    ProjectEntityId = 1
                });
            ringtoetsEntities.ProjectEntities.Add(
                new ProjectEntity
                {
                    ProjectEntityId = 2
                });
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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.ProjectEntities.Add(
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
                });

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.InsertModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual(0, ringtoetsEntities.ProjectEntities.Count());

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_ValidProject_UpdatedDataSet()
        {
            // Setup
            const long storageId = 1234L;
            const string description = "description";

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };

            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.InsertModel(project);

            // Assert
            var projectEntity = ringtoetsEntities.ProjectEntities.First();
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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            Project project = new Project
            {
                StorageId = storageId,
                Description = description,
                Items =
                {
                    new DikeAssessmentSection()
                }
            };

            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            persistor.InsertModel(project);

            // Assert
            var projectEntity = ringtoetsEntities.ProjectEntities.First();
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            Project project = new Project
            {
                StorageId = storageId
            };
            ringtoetsEntities.ProjectEntities.Add(
                new ProjectEntity
                {
                    ProjectEntityId = 2
                });
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateModel(project);

            // Assert
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_MultipleEqualEntitiesInDbSet_ThrownEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            Project project = new Project
            {
                StorageId = storageId
            };
            ringtoetsEntities.ProjectEntities.Add(
                new ProjectEntity
                {
                    ProjectEntityId = storageId
                });
            ringtoetsEntities.ProjectEntities.Add(
                new ProjectEntity
                {
                    ProjectEntityId = storageId
                });

            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.UpdateModel(project);

            // Assert
            var expectedMessage = String.Format("Het object 'ProjectEntity' met id '{0}' is niet gevonden.", storageId);
            var expectedInnerMessage = "Sequence contains more than one matching element";

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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();


            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };
            ProjectEntity entity = new ProjectEntity
            {
                ProjectEntityId = storageId
            };

            ringtoetsEntities.ProjectEntities.Add(entity);

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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

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

            ringtoetsEntities.ProjectEntities.Add(projectEntity);
            
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            Project project = new Project
            {
                StorageId = 0L
            };

            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            TestDelegate insertTest = () => persistor.InsertModel(project);
            Assert.DoesNotThrow(insertTest, "Precondition failed: InsertModel failed");

            var insertedProjectEntity = ringtoetsEntities.ProjectEntities.First();
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

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ProjectEntity entityToDelete = new ProjectEntity
            {
                ProjectEntityId = 4567L,
                Description = "Entity to delete"
            };

            var entityToUpdate = new ProjectEntity
            {
                ProjectEntityId = storageId,
                Description = "Entity to update"
            };

            ringtoetsEntities.ProjectEntities.Add(entityToDelete);
            ringtoetsEntities.ProjectEntities.Add(entityToUpdate);

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
            persistor.RemoveUnModifiedEntries();

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.ProjectEntities.Count());
            CollectionAssert.AreEqual(new[] { entityToUpdate }, ringtoetsEntities.ProjectEntities);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInDbSetEmptyProject_EmptyDbSet()
        {
            // Setup
            const long storageId = 1L;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

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
            ProjectEntity entityToUpdate = new ProjectEntity
            {
                ProjectEntityId = storageId,
                Description = "Entity to update"
            };

            ringtoetsEntities.ProjectEntities.Add(firstEntityToDelete);
            ringtoetsEntities.ProjectEntities.Add(secondEntityToDelete);
            ringtoetsEntities.ProjectEntities.Add(entityToUpdate);

            Project project = new Project
            {
                StorageId = storageId
            };

            ProjectEntityPersistor persistor = new ProjectEntityPersistor(ringtoetsEntities);

            TestDelegate test = () => persistor.UpdateModel(project);
            Assert.DoesNotThrow(test, "Precondition failed: UpdateModel");

            // Call
            persistor.RemoveUnModifiedEntries();

            // Assert
            CollectionAssert.AreEqual(new[] { entityToUpdate }, ringtoetsEntities.ProjectEntities);
            mockRepository.VerifyAll();
        }
    }
}