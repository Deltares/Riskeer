using System;

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class LognormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameterdConstructor_ValidLognormalDistribution_ExpectedValues()
        {
            // Setup
            var lognormalDistribution = new LognormalDistribution(2);

            // Call
            var designValue = new LognormalDistributionDesignVariable(lognormalDistribution);

            // Assert
            Assert.AreSame(lognormalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
        }

        /// <summary>
        /// Tests the <see cref="LognormalDistributionDesignVariable.GetDesignValue"/>
        /// against the values calculated with the excel sheet in WTI-33 (timestamp: 27-11-2015 10:27).
        /// </summary>
        /// <param name="expectedValue">MEAN.</param>
        /// <param name="variance">VARIANCE.</param>
        /// <param name="percentile">Percentile.</param>
        /// <param name="expectedResult">Rekenwaarde.</param>
        [Test]
        [TestCase(75, 70, 0.95, 89.49908018)]
        [TestCase(75, 70, 0.5, 74.53764421)]
        [TestCase(75, 70, 0.05, 62.07729055)]
        [TestCase(75, 123.45, 0.95, 94.5366392)]
        [TestCase(75, 1.2345, 0.95, 76.84147913)]
        [TestCase(123.45, 70, 0.95, 137.6747689)]
        [TestCase(1.2345, 70, 0.95, 4.541270837)]
        public void GetDesignVariable_ValidLognormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double percentile,
            double expectedResult)
        {
            // Setup
            var lognormalDistribution = new LognormalDistribution(4)
            {
                Mean = (RoundedDouble)expectedValue,
                StandardDeviation = (RoundedDouble)Math.Sqrt(variance)
            };

            var designVariable = new LognormalDistributionDesignVariable(lognormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            double result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-4);
        }
    }
}