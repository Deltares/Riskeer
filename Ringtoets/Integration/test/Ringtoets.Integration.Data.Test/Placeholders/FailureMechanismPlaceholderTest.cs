using NUnit.Framework;
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
            Assert.AreEqual(expectedName, failureMechanism.Name);
            Assert.AreEqual("Vakindeling", failureMechanism.SectionDivisions.Name);
            Assert.AreEqual("Locaties", failureMechanism.Locations.Name);
            Assert.AreEqual("Randvoorwaarden", failureMechanism.BoundaryConditions.Name);
            Assert.AreEqual("Oordeel", failureMechanism.AssessmentResult.Name);
        }
    }
}