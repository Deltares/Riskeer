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
            ProjectEntity projectEntity = new ProjectEntity();

            // Assert
            Assert.AreEqual(0, projectEntity.ProjectEntityId);
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
        [TestCase(null)]
        [TestCase(0L)]
        [TestCase(1234L)]
        public void LastUpdated_DifferentValues_ReturnExpectedValues(long? someTimestamp)
        {
            // Setup
            var projectEntity = new ProjectEntity
            {
                LastUpdated = someTimestamp
            };

            // Call & Assert
            Assert.AreEqual(someTimestamp, projectEntity.LastUpdated);
        }
    }
}