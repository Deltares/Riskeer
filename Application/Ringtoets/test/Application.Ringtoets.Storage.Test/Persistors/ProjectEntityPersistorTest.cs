using System;
using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.Test.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class ProjectEntityPersistorTest
    {
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
            // Call
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>());
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Assert
            Assert.IsInstanceOf<IEntityPersistor<Project, ProjectEntity>>(persistor);
        }

        [Test]
        public void GetEntityAsModel_EmptyDataset_DoesNotThrowException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>());
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Call
            TestDelegate test = () => persistor.GetEntityAsModel();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetEntityAsModel_SingleEntityInDataSet_ProjectEntityFromDataSet()
        {
            // Setup
            const long storageId = 1234L;
            const string name = "test";
            const string description = "description";
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId,
                    Name = name,
                    Description = description
                }
            });
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Call
            Project model = persistor.GetEntityAsModel();

            // Assert
            Assert.IsInstanceOf<Project>(model);
            Assert.AreEqual(storageId, model.StorageId);
            Assert.AreEqual(name, model.Name);
            Assert.AreEqual(description, model.Description);
        }

        [Test]
        public void GetEntityAsModel_MultipleEntitiesInDataSet_ThrowsInvalidOperationException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>
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
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Call
            TestDelegate test = () => persistor.GetEntityAsModel();

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void AddEntity_NullData_ThrowsArgumentNullException()
        {
            // Setup
            var dbSetMethodAddWasHit = 0;
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>());
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);
            dbset.Stub(m => m.Add(new ProjectEntity())).IgnoreArguments().Return(new ProjectEntity())
                 .WhenCalled(invocation => dbSetMethodAddWasHit++);

            // Call
            TestDelegate test = () => persistor.AddEntity(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual(0, dbSetMethodAddWasHit);
        }

        [Test]
        public void AddEntity_ValidProject_UpdatedDataSet()
        {
            // Setup
            var dbSetMethodAddWasHit = 0;
            const long storageId = 1234L;
            const string name = "test";
            const string description = "description";
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>());
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);
            ProjectEntity projectEntity = new ProjectEntity();
            Project project = new Project
            {
                StorageId = storageId,
                Name = name,
                Description = description
            };
            dbset.Stub(m => m.Add(projectEntity)).IgnoreArguments().Return(projectEntity)
                 .WhenCalled(invocation => dbSetMethodAddWasHit++);

            // Call
            persistor.AddEntity(project);

            // Assert
            Assert.AreEqual(1, dbSetMethodAddWasHit);
        }

        [Test]
        public void UpdateEntity_NullData_ThrowsArgumentNullException()
        {
            // Setup
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>());
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Call
            TestDelegate test = () => persistor.UpdateEntity(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateEntity_UnknownProject_ThrowsException()
        {
            // Setup
            Project project = new Project
            {
                StorageId = 1
            };
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = 2
                }
            });
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Call
            TestDelegate test = () => persistor.UpdateEntity(project);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void UpdateEntity_ValidProject_ReturnsTheProjectAsProjectEntity()
        {
            // Setup
            const long storageId = 1234L;
            Project project = new Project
            {
                StorageId = storageId
            };
            var dbset = DbTestSet.GetDbTestSet(new List<ProjectEntity>
            {
                new ProjectEntity
                {
                    ProjectEntityId = storageId
                }
            });
            ProjectEntityPersistor persistor = new ProjectEntityPersistor(dbset);

            // Call
            ProjectEntity entity = persistor.UpdateEntity(project);

            // Assert
            Assert.IsInstanceOf<ProjectEntity>(entity);
            Assert.AreEqual(project.StorageId, entity.ProjectEntityId);
        }
    }
}