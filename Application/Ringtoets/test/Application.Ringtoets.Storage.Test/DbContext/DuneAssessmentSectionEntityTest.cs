using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    [TestFixture]
    public class DuneAssessmentSectionEntityTest
    {
        [Test]
        public void DefaultConstructor_Always_ExpectedValues()
        {
            // Call
            DuneAssessmentSectionEntity duneAssessmentSectionEntity = new DuneAssessmentSectionEntity();

            // Assert
            Assert.AreEqual(0, duneAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(0, duneAssessmentSectionEntity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(null, duneAssessmentSectionEntity.Name);
            Assert.AreEqual(0, duneAssessmentSectionEntity.Norm);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const long expectedParentId = 1L;
            const long expectedId = 1024L;
            const string someName = "<some name>";
            const int someNorm = 5000;

            var duneAssessmentSectionEntity = new DuneAssessmentSectionEntity
            {
                ProjectEntityId = expectedParentId,
                DuneAssessmentSectionEntityId = expectedId,
                Name = someName,
                Norm = someNorm
            };

            // Call & Assert
            Assert.AreEqual(expectedParentId, duneAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(expectedId, duneAssessmentSectionEntity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(someName, duneAssessmentSectionEntity.Name);
            Assert.AreEqual(someNorm, duneAssessmentSectionEntity.Norm);
        }
    }
}