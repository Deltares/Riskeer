using System;

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class LognormalDistributionTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new LognormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<IDistribution>(distribution);
            Assert.AreEqual(Math.Exp(-0.5), distribution.Mean, Math.Pow(10.0, -numberOfDecimalPlaces));
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(Math.Sqrt((Math.Exp(1)-1)*Math.Exp(1)), distribution.StandardDeviation);
        }

        [Test]
        [TestCase(0)]
        public void Constructor_InvalidNumberOfDecimalPlaces_ThrowArgumentOutOfRangeException()
        {
            // Setup

            // Call
            TestDelegate call = () => new LognormalDistribution(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-123.45)]
        public void Mean_SettingToLessThanOrEqualTo0_ThrowArgumentOutOfRangeException(double newMean)
        {
            // Setup
            var distribution = new LognormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.Mean = (RoundedDouble)newMean;

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            var customMessagePart = exception.Message.Split(new[]{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Gemiddelde moet groter zijn dan 0.", customMessagePart);
        }

        [Test]
        [TestCase(0 + 1e-4)]
        [TestCase(156.23)]
        public void Mean_SettingValidValue_ValueIsSet(double newMean)
        {
            // Setup
            var distribution = new LognormalDistribution(4);

            // Call
            distribution.Mean = (RoundedDouble)newMean;

            // Assert
            Assert.AreEqual(newMean, distribution.Mean, 1e-4);
        }

        [Test]
        [TestCase(0 - 1e-6)]
        [TestCase(-4)]
        public void StandardDeviation_SettingNotGreaterThan0_ThrowArgumentOutOfRangeException(double newStd)
        {
            // Setup
            var distribution = new LognormalDistribution(4);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = newStd;

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            string customMessagePart = exception.Message.Split(new []{Environment.NewLine}, StringSplitOptions.None)[0];
            Assert.AreEqual("Standaard afwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.", customMessagePart);
        }
    }
}