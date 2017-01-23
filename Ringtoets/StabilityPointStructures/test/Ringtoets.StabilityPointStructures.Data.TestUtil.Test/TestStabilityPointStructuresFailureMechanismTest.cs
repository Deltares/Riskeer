using NUnit.Framework;

namespace Ringtoets.StabilityPointStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestStabilityPointStructuresFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();

            // Assert
            Assert.AreEqual(2, failureMechanism.Contribution);
        }
    }
}
