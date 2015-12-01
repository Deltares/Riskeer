using System;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class NormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameterdConstructor_ValidLognormalDistribution_ExpectedValues()
        {
            // Setup
            var normalDistribution = new NormalDistribution();

            // Call
            var designValue = new NormalDistributionDesignVariable(normalDistribution);

            // Assert
            Assert.AreSame(normalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
        }

        /// <summary>
        /// Tests the <see cref="NormalDistributionDesignVariable.GetDesignValue"/>
        /// against the values calculated with the excel sheet in WTI-33 (timestamp: 27-11-2015 10:27).
        /// </summary>
        /// <param name="expectedValue">MEAN.</param>
        /// <param name="variance">VARIANCE.</param>
        /// <param name="percentile">Percentile.</param>
        /// <param name="expectedResult">Rekenwaarde.</param>
        [Test]
        [TestCase(75, 70, 0.95, 88.76183279)]
        [TestCase(75, 70, 0.5, 75)]
        [TestCase(75, 70, 0.05, 61.23816721)]
        [TestCase(75, 123.45, 0.95, 93.27564881)]
        [TestCase(75, 1.2345, 0.95, 76.82756488)]
        [TestCase(123.45, 70, 0.95, 137.2118328)]
        [TestCase(1.2345, 70, 0.95, 14.99633279)]
        public void GetDesignVariable_ValidNormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double percentile,
            double expectedResult)
        {
            // Setup
            var normalDistribution = new NormalDistribution
            {
                Mean = expectedValue,
                StandardDeviation = Math.Sqrt(variance)
            };

            var designVariable = new NormalDistributionDesignVariable(normalDistribution)
            {
                Percentile = percentile
            };

            // Call
            double result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-6);
        }
    }
}