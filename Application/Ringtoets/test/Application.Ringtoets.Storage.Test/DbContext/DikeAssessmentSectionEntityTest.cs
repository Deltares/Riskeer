using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    [TestFixture]
    public class DikeAssessmentSectionEntityTest
    {
        [Test]
        public void DefaultConstructor_Always_ExpectedValues()
        {
            // Call
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity();

            // Assert
            Assert.AreEqual(0, dikeAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(0, dikeAssessmentSectionEntity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(null, dikeAssessmentSectionEntity.Name);
            Assert.AreEqual(0, dikeAssessmentSectionEntity.Norm);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const long expectedParentId = 1L;
            const long expectedId = 1024L;
            const string someName = "<some name>";
            const int someNorm = 5000;

            var dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity
            {
                ProjectEntityId = expectedParentId,
                DikeAssessmentSectionEntityId = expectedId,
                Name = someName,
                Norm = someNorm
            };

            // Call & Assert
            Assert.AreEqual(expectedParentId, dikeAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(expectedId, dikeAssessmentSectionEntity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(someName, dikeAssessmentSectionEntity.Name);
            Assert.AreEqual(someNorm, dikeAssessmentSectionEntity.Norm);
        }
    }
}