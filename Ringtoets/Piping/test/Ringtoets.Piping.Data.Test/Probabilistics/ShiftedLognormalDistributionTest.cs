using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class ShiftedLognormalDistributionTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new ShiftedLognormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<LognormalDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Shift);
        }
    }
}