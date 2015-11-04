﻿using System;

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
            Assert.IsInstanceOf<IDistribution>(distribution);
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

        [Test]
        [TestCase(-26749.34)]
        [TestCase(0.0 - 1e-6)]
        [TestCase(1.0 + 1e-6)]
        [TestCase(5678.896432)]
        public void InverseCDF_InvalidProbability_ThrowArgumentOutOfRangeException(double invalidProbability)
        {
            // Setup
            var distribution = new NormalDistribution();

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
        /// Test oracle has been generated using EXCEL 2010, using the NORM.INV function.
        /// </summary>
        [Test]
        [TestCase(0, double.NegativeInfinity)]
        [TestCase(0.025, -3.2119207660)]
        [TestCase(0.25, -0.3838774504)]
        [TestCase(0.5, 1.1000000000)]
        [TestCase(0.75, 2.5838774504)]
        [TestCase(0.975, 5.4119207660)]
        [TestCase(1, double.PositiveInfinity)]
        public void InverseCDF_ValidProbability_ReturnExpectedRealization(double probability, double expectedRealization)
        {
            // Setup
            var distribution = new NormalDistribution
            {
                Mean = 1.1,
                StandardDeviation = 2.2
            };

            // Call
            var realization = distribution.InverseCDF(probability);

            // Assert
            Assert.AreEqual(expectedRealization, realization, 1e-6);
        }
    }
}