using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    [TestFixture]
    public class ProjectEntityTest
    {
        [Test]
        public void DefaultConstructor_Always_ExpectedValues()
        {
            // Call
            var projectEntity = new ProjectEntity();

            // Assert
            Assert.AreEqual(new long(), projectEntity.ProjectEntityId);
            Assert.IsNull(projectEntity.LastUpdated);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const long expectedId = 1024;
            const string expectedName = "<some name>";
            const string someDescription = "<some description>";
            const long someTimestamp = 123456789;

            var projectEntity = new ProjectEntity
            {
                Name = expectedName,
                ProjectEntityId = expectedId,
                Description = someDescription,
                LastUpdated = someTimestamp
            };

            // Call & Assert
            Assert.AreEqual(expectedName, projectEntity.Name);
            Assert.AreEqual(someDescription, projectEntity.Description);
            Assert.AreEqual(expectedId, projectEntity.ProjectEntityId);
            Assert.AreEqual(someTimestamp, projectEntity.LastUpdated);
        }

        [Test]
        public void GetProperties_WithNullableData_ReturnExpectedValues()
        {
            // Setup
            long? someTimestamp = null;
            var projectEntity = new ProjectEntity
            {
                LastUpdated = someTimestamp
            };

            // Call & Assert
            Assert.AreEqual(someTimestamp, projectEntity.LastUpdated);
        }
    }
}