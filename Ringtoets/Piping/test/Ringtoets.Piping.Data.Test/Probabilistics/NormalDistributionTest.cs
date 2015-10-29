using System;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class NomalDistributionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new NormalDistribution();

            // Assert
            Assert.AreEqual(0.0, distribution.Mean);
            Assert.AreEqual(1.0, distribution.StandardDeviation);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-4)]
        public void StandardDeviation_SettingNotGreaterThan0_ThrowArgumentException(double newStd)
        {
            // Setup
            var distribution = new NormalDistribution();

            // Call
            TestDelegate call = () => distribution.StandardDeviation = newStd;

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("Standaard afwijking (\u03C3) moet groter zijn dan 0.", exception.Message);
        }
    }
}