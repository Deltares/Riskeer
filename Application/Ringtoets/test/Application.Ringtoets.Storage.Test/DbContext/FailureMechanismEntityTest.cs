using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.DbContext
{
    [TestFixture]
    public class FailureMechanismEntityTest
    {
        [Test]
        public void DefaultConstructor_Always_ExpectedValues()
        {
            // Call
            FailureMechanismEntity failureMechanismEntity = new FailureMechanismEntity();

            // Assert
            Assert.AreEqual(0, failureMechanismEntity.FailureMechanismEntityId);
            Assert.AreEqual(0, failureMechanismEntity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(0, failureMechanismEntity.FailureMechanismType);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const long expectedParentId = 1L;
            const long expectedId = 1024L;
            int failureMechanismType = (int) FailureMechanismType.DikesPipingFailureMechanism;

            // Call
            FailureMechanismEntity failureMechanismEntity = new FailureMechanismEntity
            {
                DikeAssessmentSectionEntityId = expectedParentId,
                FailureMechanismEntityId = expectedId,
                FailureMechanismType = failureMechanismType
            };

            // Assert
            Assert.AreEqual(expectedParentId, failureMechanismEntity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(expectedId, failureMechanismEntity.FailureMechanismEntityId);
            Assert.AreEqual(failureMechanismType, failureMechanismEntity.FailureMechanismType);
        }
    }
}