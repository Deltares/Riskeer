using System;

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class ShiftedLognormalDistributionDesignVariableTest
    {
        [Test]
        public void ParameterdConstructor_ValidLognormalDistribution_ExpectedValues()
        {
            // Setup
            var shiftedLognormalDistribution = new ShiftedLognormalDistribution(1);

            // Call
            var designValue = new ShiftedLognormalDistributionDesignVariable(shiftedLognormalDistribution);

            // Assert
            Assert.AreSame(shiftedLognormalDistribution, designValue.Distribution);
            Assert.AreEqual(0.5, designValue.Percentile);
        }

        /// <summary>
        /// Tests the <see cref="ShiftedLognormalDistributionDesignVariable.GetDesignValue"/>
        /// against the values calculated with the excel sheet in WTI-33 (timestamp: 27-11-2015 10:27).
        /// </summary>
        /// <param name="expectedValue">MEAN.</param>
        /// <param name="variance">VARIANCE.</param>
        /// <param name="shift">SHIFT</param>
        /// <param name="percentile">Percentile.</param>
        /// <param name="expectedResult">Rekenwaarde.</param>
        [Test]
        [TestCase(75, 70, 10, 0.5, 84.53764421)]
        [TestCase(75, 70, 10, 0.95, 99.49908018)]
        [TestCase(75, 70, 10, 0.05, 72.07729055)]
        [TestCase(75, 70, -30, 0.95, 59.49908018)]
        [TestCase(75, 70, 123.45, 0.95, 212.9490802)]
        [TestCase(75, 123.45, 10, 0.95, 104.5366392)]
        [TestCase(75, 1.2345, 10, 0.95, 86.84147913)]
        [TestCase(123.45, 70, 10, 0.95, 147.6747689)]
        [TestCase(1.2345, 70, 10, 0.95, 14.54127084)]
        public void GetDesignVariable_ValidShiftedLognormalDistribution_ReturnExpectedValue(
            double expectedValue, double variance, double shift, double percentile,
            double expectedResult)
        {
            // Setup
            var shiftedLognormalDistribution = new ShiftedLognormalDistribution(4)
            {
                Mean = (RoundedDouble)expectedValue,
                StandardDeviation = (RoundedDouble)Math.Sqrt(variance),
                Shift = (RoundedDouble)shift
            };

            var designVariable = new ShiftedLognormalDistributionDesignVariable(shiftedLognormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            double result = designVariable.GetDesignValue();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-4);
        }

        [Test]
        [TestCase(75, 70, 0.5)]
        [TestCase(75, 70, 0.95)]
        [TestCase(75, 70, 0.05)]
        [TestCase(75, 123.45, 0.95)]
        [TestCase(75, 1.2345, 0.95)]
        [TestCase(123.45, 70, 0.95)]
        [TestCase(1.2345, 70, 0.95)]
        public void GetDesignVariable_ShiftIsZero_ReturnIdenticalValueAsLognormalDistributionDesignVariable(
            double expectedValue, double variance, double percentile)
        {
            // Setup
            var shiftedLognormalDistribution = new ShiftedLognormalDistribution(6)
            {
                Mean = (RoundedDouble)expectedValue,
                StandardDeviation = (RoundedDouble)Math.Sqrt(variance),
                Shift = (RoundedDouble)0.0
            };

            var designVariable = new ShiftedLognormalDistributionDesignVariable(shiftedLognormalDistribution)
            {
                Percentile = percentile
            };

            // Call
            double result = designVariable.GetDesignValue();

            // Assert
            var expectedResult = new LognormalDistributionDesignVariable(shiftedLognormalDistribution)
            {
                Percentile = percentile
            }.GetDesignValue();
            Assert.AreEqual(expectedResult, result);
        }
    }
}