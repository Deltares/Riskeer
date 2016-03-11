using Core.Common.Base.Data;

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
            Assert.IsInstanceOf<RoundedDouble>(distribution.Shift);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.Shift.Value);
        }

        [Test]
        [TestCase(1, 5.6)]
        [TestCase(3, 5.647)]
        [TestCase(4, 5.6473)]
        [TestCase(15, 5.647300000000000)]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Shift = new RoundedDouble(4, 5.6473);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.Shift.Value);
        }
    }
}