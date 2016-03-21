using System;
using System.Collections.Generic;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class ProjectEntityConverterTest
    {
        private static IEnumerable<Func<Project>> TestCases
        {
            get
            {
                yield return () => null;
                yield return null;
            }
        }

        [Test]
        public void DefaultConstructor_Always_NewProjectEntityConverter()
        {
            // Call
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<Project, ProjectEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null, () => new Project());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void ConvertEntityToModel_ValidProjectEntityNullModel_ThrowsArgumentNullException(Func<Project> func)
        {
            // SetUp
            const long storageId = 1234L;
            const string description = "Description";
            ProjectEntity projectEntity = new ProjectEntity()
            {
                ProjectEntityId = storageId,
                Description = description
            };

            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(projectEntity, func);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertEntityToModel_ValidProjectEntity_ReturnsTheProjectEntityAsProject()
        {
            // SetUp
            const long storageId = 1234L;
            const string description = "Description";
            ProjectEntity projectEntity = new ProjectEntity()
            {
                ProjectEntityId = storageId,
                Description = description
            };
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Call
            Project project = converter.ConvertEntityToModel(projectEntity, () => new Project());

            // Assert
            Assert.AreNotEqual(projectEntity, project);
            Assert.AreEqual(storageId, project.StorageId);
            Assert.AreEqual(description, project.Description);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            ProjectEntityConverter converter = new ProjectEntityConverter();
            Project project = new Project();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(project, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // SetUp
            ProjectEntityConverter converter = new ProjectEntityConverter();
            ProjectEntity projectEntity = new ProjectEntity();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, projectEntity);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_ValidProject_UpdatesTheProjectAsProjectEntity()
        {
            // SetUp
            const long storageId = 1234L;
            const string description = "Description";
            Project project = new Project
            {
                StorageId = storageId,
                Description = description
            };
            ProjectEntity projectEntity = new ProjectEntity();
            ProjectEntityConverter converter = new ProjectEntityConverter();

            // Call
            converter.ConvertModelToEntity(project, projectEntity);

            // Assert
            Assert.AreNotEqual(projectEntity, project);
            Assert.AreEqual(storageId, projectEntity.ProjectEntityId);
            Assert.AreEqual(description, projectEntity.Description);
            Assert.IsNotNull(projectEntity.LastUpdated);
        }
    }
}