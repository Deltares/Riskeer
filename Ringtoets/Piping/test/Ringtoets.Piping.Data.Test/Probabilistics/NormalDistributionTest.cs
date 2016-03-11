using System;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class NomalDistributionTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Mean, Math.Pow(10.0, -numberOfDecimalPlaces));
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.StandardDeviation);
        }

        [Test]
        [TestCase(0 - 1e-6)]
        [TestCase(-4)]
        public void StandardDeviation_SettingNotGreaterThan0_ThrowArgumentOutOfRangeException(double newStd)
        {
            // Setup
            var distribution = new NormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = newStd;

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Standaard afwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.", customMessagePart);
        }
    }
}