using System;

using Core.Common.Base.Data;
using Core.Common.TestUtil;

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
            Assert.AreEqual(0.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.StandardDeviation.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(0, 1.0)]
        [TestCase(3, 1.235)]
        [TestCase(4, 1.2345)]
        [TestCase(15, 1.234500000000000)]
        public void Mean_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.Mean = new RoundedDouble(4, 1.2345);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.Mean.Value);
        }

        [Test]
        [TestCase(0 - 1e-2)]
        [TestCase(-4)]
        public void StandardDeviation_SettingToLessThan0_ThrowArgumentOutOfRangeException(double newStd)
        {
            // Setup
            var distribution = new NormalDistribution(2);

            // Call
            TestDelegate call = () => distribution.StandardDeviation = (RoundedDouble)newStd;

            // Assert
            const string expectedMessage = "Standaard afwijking (\u03C3) moet groter zijn dan of gelijk zijn aan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(0, 6.0)]
        [TestCase(2, 5.68)]
        [TestCase(3, 5.678)]
        [TestCase(15, 5.678000000000000)]
        public void StandardDeviation_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces(int numberOfDecimalPlaces, double expectedStandardDeviation)
        {
            // Setup
            var distribution = new NormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.StandardDeviation = new RoundedDouble(3, 5.678);

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedStandardDeviation, distribution.StandardDeviation.Value);
        }
    }
}