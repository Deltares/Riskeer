using NUnit.Framework;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class TestPipingFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new TestPipingFailureMechanism();

            // Assert
            Assert.AreEqual(24, failureMechanism.Contribution);
        }
    }
}
