using System;

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
            var lognormalDistribution = new LognormalDistribution();

            // Call
            var designValue = new LognormalDistributionDesignVariable(lognormalDistribution);

            // Assert
            Assert.AreSame(lognormalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
        }

        [Test]
        [TestCase(75, 1, 0.05, 73.36660252)]
        [TestCase(75, 1, 0.95, 76.65613487)]
        [TestCase(75, 5, 0.95, 78.73383874)]
        [TestCase(75, 5, 0.05, 71.37978448)]
        [TestCase(1, 5, 0.5, 0.40824829)]
        public void GetDesignVariable_ValidLognormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double percentile,
            double expectedResult)
        {
            // Setup
            var lognormalDistribution = new LognormalDistribution
            {
                Mean = expectedValue,
                StandardDeviation = Math.Sqrt(variance)
            };

            var designVariable = new LognormalDistributionDesignVariable(lognormalDistribution)
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