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
    public class ProjectEntityConverterTest
    {
        [Test]
        public void GetProject_ProjectEntity_Project()
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
            var project = ProjectEntityConverter.GetProject(projectEntities);

            // Assert
            Assert.IsInstanceOf<Project>(project);
            Assert.AreEqual(project.StorageId, projectId);
            Assert.That(project.Name == projectEntity.Name);
            Assert.That(project.Description == projectEntity.Description);
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
        public void UpdateProjectEntity_Project_ProjectEntity()
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
            var projectEntities = GetDbSetTest(projectEnties);

            // Call
            ProjectEntityConverter.UpdateProjectEntity(projectEntities, project);

            // Assert
            Assert.IsInstanceOf<Project>(project);
            var projectEntitiesArray = projectEntities.ToList();
            Assert.AreEqual(projectEntitiesArray.Count, 1);
            Assert.AreEqual(project.StorageId, projectEntitiesArray[0].ProjectEntityId);
            Assert.AreEqual(project.Name, projectEntitiesArray[0].Name);
            Assert.AreEqual(project.Description, projectEntitiesArray[0].Description);
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