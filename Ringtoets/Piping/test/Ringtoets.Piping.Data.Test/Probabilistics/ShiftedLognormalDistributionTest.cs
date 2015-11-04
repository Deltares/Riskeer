using System;

using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test.Probabilistics
{
    [TestFixture]
    public class ShiftedLognormalDistributionTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var distribution = new ShiftedLognormalDistribution();

            // Assert
            Assert.IsInstanceOf<LognormalDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Shift);
        }

        [Test]
        [TestCase(-26749.34)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(5678.896432)]
        public void InverseCDF_InvalidProbability_ThrowArgumentOutOfRangeException(double invalidProbability)
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution();

            // Call
            TestDelegate call = () => distribution.InverseCDF(invalidProbability);

            // Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(call);
            var customMessagePart = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)[0];
            Assert.AreEqual("Kans moet in het bereik van [0, 1] opgegeven worden.", customMessagePart);
        }

        /// <summary>
        /// Test oracle has been generated using EXCEL 2010, using the LOGNORM.INV function,
        /// then adding the shift to the result.
        /// </summary>
        [Test]
        [TestCase(0, 3.3)]
        [TestCase(0.025, 3.3402791721)]
        [TestCase(0.25, 3.9812149047)]
        [TestCase(0.5, 6.3041660239)]
        [TestCase(0.75, 16.5484087440)]
        [TestCase(0.975, 227.3615444190)]
        [TestCase(1, double.PositiveInfinity)]
        public void InverseCDF_ValidProbability_ReturnExpectedRealization(double probability, double expectedRealization)
        {
            // Setup
            var distribution = new ShiftedLognormalDistribution
            {
                Mean = 1.1,
                StandardDeviation = 2.2,
                Shift = 3.3
            };

            // Call
            var realization = distribution.InverseCDF(probability);

            // Assert
            Assert.AreEqual(expectedRealization, realization, 1e-6);
        }
    }
}