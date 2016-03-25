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
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const long expectedId = 1024L;
            const string someDescription = "<some description>";
            const long someTimestamp = 123456789L;

            var projectEntity = new ProjectEntity
            {
                ProjectEntityId = expectedId,
                Description = someDescription,
            };

            // Call & Assert
            Assert.AreEqual(someDescription, projectEntity.Description);
            Assert.AreEqual(expectedId, projectEntity.ProjectEntityId);
        }
    }
}