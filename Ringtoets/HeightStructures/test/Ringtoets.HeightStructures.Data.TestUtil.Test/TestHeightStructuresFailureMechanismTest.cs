using NUnit.Framework;

namespace Ringtoets.HeightStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHeightStructuresFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new TestHeightStructuresFailureMechanism();

            // Assert
            Assert.AreEqual(24, failureMechanism.Contribution);
        }
    }
}
