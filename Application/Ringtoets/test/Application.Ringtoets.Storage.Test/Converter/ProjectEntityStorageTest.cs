using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Converter
{
    [TestFixture]
    public class ProjectEntityStorageTest
    {
        [Test]
        public void GetProject_DbSetWithValidProjectEntity_ReturnsTheProjectEntityAsProject()
        {
            // Setup
            const long projectId = 1;
            var projectEntity = new ProjectEntity()
            {
                ProjectEntityId = projectId,
                Name = "test",
                Description = "description"
            };

            var projectEntities = GetDbSetTest(new List<ProjectEntity>
            {
                projectEntity
            });

            // Call
            Project project = ProjectEntityConverter.GetProject(projectEntities);

            // Assert
            Assert.IsInstanceOf<Project>(project);
            Assert.AreEqual(project.StorageId, projectId);
            Assert.AreEqual(project.Name, projectEntity.Name);
            Assert.AreEqual(project.Description, projectEntity.Description);
            Assert.IsEmpty(project.Items);
        }

        [Test]
        public void GetProject_NullDataSet_ThrowsArgumentNullException()
        {
            // Setup
            TestDelegate test = () => ProjectEntityConverter.GetProject(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateProjectEntity_NullDataValidProject_ThrowsArgumentNullException()
        {
            // Setup
            var project = new Project();
            TestDelegate test = () => ProjectEntityConverter.UpdateProjectEntity(null, project);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateProjectEntity_ValidDataSetNullProject_ThrowsArgumentNullException()
        {
            // Setup
            var projectEntities = GetDbSetTest(new List<ProjectEntity>());
            TestDelegate test = () => ProjectEntityConverter.UpdateProjectEntity(projectEntities, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateProjectEntity_DuplicateProjectEntityId_ThrowsInvalidOperationException()
        {
            // Setup
            const long projectId = 1;
            var project = new Project
            {
                StorageId = projectId
            };
            var projectEnties = new List<ProjectEntity>
            {
                new ProjectEntity()
                {
                    ProjectEntityId = projectId
                },
                new ProjectEntity()
                {
                    ProjectEntityId = projectId
                }
            };
            var projectEntities = GetDbSetTest(projectEnties);

            // Call
            TestDelegate test = () => ProjectEntityConverter.UpdateProjectEntity(projectEntities, project);

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void UpdateProjectEntity_UnknownProjectEntityId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long projectId = 1;
            const long projectEntityId = 2;
            var project = new Project
            {
                StorageId = projectId
            };
            var projectEnties = new List<ProjectEntity>
            {
                new ProjectEntity()
                {
                    ProjectEntityId = projectEntityId
                }
            };
            var projectEntities = GetDbSetTest(projectEnties);

            // Call
            TestDelegate test = () => ProjectEntityConverter.UpdateProjectEntity(projectEntities, project);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);
        }

        [Test]
        public void UpdateProjectEntity_WithSingleProject_ProjectEntriesEqualToProject()
        {
            // Setup
            const long projectId = 1;
            var project = new Project
            {
                StorageId = projectId,
                Name = "test",
                Description = "description"
            };
            var projectEnties = new List<ProjectEntity>
            {
                new ProjectEntity()
                {
                    ProjectEntityId = projectId
                }
            };
            IDbSet<ProjectEntity> projectEntities = GetDbSetTest(projectEnties);

            // Call
            ProjectEntityConverter.UpdateProjectEntity(projectEntities, project);

            // Assert
            Assert.IsInstanceOf<Project>(project);
            List<ProjectEntity> projectEntitiesArray = projectEntities.ToList();
            Assert.AreEqual(projectEntitiesArray.Count, 1);

            ProjectEntity projectEntity = projectEntitiesArray[0];

            Assert.AreEqual(project.StorageId, projectEntity.ProjectEntityId);
            Assert.AreEqual(project.Name, projectEntity.Name);
            Assert.AreEqual(project.Description, projectEntity.Description);
            Assert.AreNotEqual(project, projectEntity);
        }

        [Test]
        public void InsertProjectEntity_NullDataValidProject_ThrowsArgumentNullException()
        {
            // Setup
            var project = new Project();
            TestDelegate test = () => ProjectEntityConverter.InsertProjectEntity(null, project);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void InsertProjectEntity_ValidDataSetNullProject_ThrowsArgumentNullException()
        {
            // Setup
            var projectEntities = GetDbSetTest(new List<ProjectEntity>());
            TestDelegate test = () => ProjectEntityConverter.InsertProjectEntity(projectEntities, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void InsertProjectEntity_ValidProject_ReturnsTheProjectAsProjectEntity()
        {
            // Setup
            const long projectId = 1;
            var project = new Project
            {
                StorageId = projectId,
                Name = "test",
                Description = "description"
            };
            var projectEnties = new List<ProjectEntity>();
            IDbSet<ProjectEntity> projectEntities = GetDbSetTest(projectEnties);

            // Call
            ProjectEntity projectEntity = ProjectEntityConverter.InsertProjectEntity(projectEntities, project);

            // Assert
            Assert.AreNotEqual(project.StorageId, projectEntity.ProjectEntityId); // Insert will decide the id of the entity
            Assert.AreEqual(project.Name, projectEntity.Name);
            Assert.AreEqual(project.Description, projectEntity.Description);
            Assert.AreNotEqual(project, projectEntity);
        }

        private static IDbSet<T> GetDbSetTest<T>(IList<T> data) where T : class
        {
            var queryable = data.AsQueryable();

            var dbSet = MockRepository.GenerateMock<IDbSet<T>, IQueryable>();

            dbSet.Stub(m => m.Provider).Return(queryable.Provider);
            dbSet.Stub(m => m.Expression).Return(queryable.Expression);
            dbSet.Stub(m => m.ElementType).Return(queryable.ElementType);
            dbSet.Stub(m => m.GetEnumerator()).Return(queryable.GetEnumerator());
            return dbSet;
        }
    }
}