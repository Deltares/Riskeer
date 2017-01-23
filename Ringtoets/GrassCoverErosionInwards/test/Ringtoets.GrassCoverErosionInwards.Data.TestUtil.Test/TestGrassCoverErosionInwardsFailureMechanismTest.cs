using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class TestGrassCoverErosionInwardsFailureMechanismTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var failureMechanism = new TestGrassCoverErosionInwardsFailureMechanism();

            // Assert
            Assert.AreEqual(24, failureMechanism.Contribution);
        }
    }
}