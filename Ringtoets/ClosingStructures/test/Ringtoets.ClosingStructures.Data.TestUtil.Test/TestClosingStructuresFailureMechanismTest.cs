using NUnit.Framework;

namespace Ringtoets.ClosingStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestClosingStructuresFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new TestClosingStructuresFailureMechanism();

            // Assert
            Assert.AreEqual(4, failureMechanism.Contribution);
        }
    }
}
