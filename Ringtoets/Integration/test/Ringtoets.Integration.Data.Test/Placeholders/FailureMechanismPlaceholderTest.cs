using NUnit.Framework;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Placeholders;

namespace Ringtoets.Integration.Data.Test.Placeholders
{
    [TestFixture]
    public class FailureMechanismPlaceholderTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            const string expectedName = "test";

            // Call
            var failureMechanism = new FailureMechanismPlaceholder(expectedName);

            // Assert
            Assert.IsInstanceOf<BaseFailureMechanism>(failureMechanism);
            Assert.AreEqual(expectedName, failureMechanism.Name);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            Assert.AreEqual("Locaties", failureMechanism.Locations.Name);
            Assert.AreEqual("Randvoorwaarden", failureMechanism.BoundaryConditions.Name);
            Assert.AreEqual("Oordeel", failureMechanism.AssessmentResult.Name);
        }
    }
}