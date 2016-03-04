using System;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class LognormalDistributionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new LognormalDistribution();

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);
            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1)-1)*Math.Exp(1)), distribution.StandardDeviation);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-123.45)]
        public void Mean_SettingToLessThanOrEqualTo0_ThrowArgumentOutOfRangeException(double newMean)
        {
            // Setup
            var distribution = new LognormalDistribution();

            // Call
            TestDelegate call = () => distribution.Mean = newMean;

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            var customMessagePart = exception.Message.Split(new[]{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Gemiddelde moet groter zijn dan 0.", customMessagePart);
        }

        [Test]
        [TestCase(0 + 1e-6)]
        [TestCase(156.23)]
        public void Mean_SettingValidValue_ValueIsSet(double newMean)
        {
            // Setup
            var distribution = new LognormalDistribution();

            // Call
            distribution.Mean = newMean;

            // Assert
            Assert.AreEqual(newMean, distribution.Mean);
        }

        [Test]
        [TestCase(0 - 1e-6)]
        [TestCase(-4)]
        public void StandardDeviation_SettingNotGreaterThan0_ThrowArgumentOutOfRangeException(double newStd)
        {
            // Setup
            var distribution = new LognormalDistribution();

            // Call
            TestDelegate call = () => distribution.StandardDeviation = newStd;

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Standaard afwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.", customMessagePart);
        }
    }
}