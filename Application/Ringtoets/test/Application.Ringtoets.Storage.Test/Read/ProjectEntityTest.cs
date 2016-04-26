using System;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ProjectEntityTest
    {
        [Test]
        public void Read_Always_ReturnsNewProjectWithPropertiesSet()
        {
            // Setup
            var testDescription = "testName";
            var entityId = new Random(21).Next(1,502);
            var entity = new ProjectEntity
            {
                ProjectEntityId = entityId,
                Description = testDescription
            };

            // Call
            var project = entity.Read();

            // Assert
            Assert.IsNotNull(project);
            Assert.AreEqual(entityId, project.StorageId);
            Assert.AreEqual(testDescription, project.Description);
        }    

        [Test]
        public void Read_WithAssessmentSection_ReturnsNewProjectWithAssessmentSections()
        {
            // Setup
            var entity = new ProjectEntity
            {
                Description = "testName",
                AssessmentSectionEntities =
                {
                    new AssessmentSectionEntity(),
                    new AssessmentSectionEntity()
                }
            };

            // Call
            var project = entity.Read();

            // Assert
            Assert.AreEqual(2, project.Items.Count);
        }    
    }
}