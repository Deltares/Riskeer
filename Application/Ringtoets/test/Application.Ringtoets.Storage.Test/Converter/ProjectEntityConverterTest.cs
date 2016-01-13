using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converter;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.Test.Converter
{
    [TestFixture]
    class ProjectEntityConverterTest
    {
        [Test]
        public void ProjectEntityMapping_ProjectEntity_Project()
        {
            // Setup
            const long projectId = 1;
            var projectEntity = new ProjectEntity()
            {
                ProjectEntityId = projectId,
                Name = "test",
                Description = "description"
            };

            var projectEntities = GetDbSetTest(new List<ProjectEntity> { projectEntity });

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
        public void ProjectEntityMapping_Project_ProjectEntity()
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
