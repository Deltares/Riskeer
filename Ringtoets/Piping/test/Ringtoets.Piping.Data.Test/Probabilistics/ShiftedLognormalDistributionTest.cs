using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class ShiftedLognormalDistributionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new ShiftedLognormalDistribution();

            // Assert
            Assert.IsInstanceOf<LognormalDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Shift);
        }
    }
}