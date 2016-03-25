using System;
using NUnit.Framework;

using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingSemiProbabilisticOutputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble();
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety, 
                upliftReliability,
                upliftProbability,
                heaveFactorOfSafety, 
                heaveReliability,
                heaveProbability,
                sellmeijerFactorOfSafety, 
                sellmeijerReliability,
                sellmeijerProbability,
                requiredProbability,
                requiredReliability,
                pipingProbability,
                pipingReliability,
                pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety, output.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(upliftReliability, output.UpliftReliability, output.UpliftReliability.GetAccuracy());
            Assert.AreEqual(upliftProbability, output.UpliftProbability, output.UpliftProbability.GetAccuracy());
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety, output.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, output.HeaveReliability, output.HeaveReliability.GetAccuracy());
            Assert.AreEqual(heaveProbability, output.HeaveProbability, output.HeaveProbability.GetAccuracy());
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, output.SellmeijerReliability, output.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability, output.SellmeijerProbability.GetAccuracy());
            Assert.AreEqual(requiredProbability, output.RequiredProbability, output.RequiredProbability.GetAccuracy());
            Assert.AreEqual(requiredReliability, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(pipingProbability, output.PipingProbability, output.PipingProbability.GetAccuracy());
            Assert.AreEqual(pipingReliability, output.PipingReliability, output.PipingReliability.GetAccuracy());
            Assert.AreEqual(pipingFactorOfSafety, output.PipingFactorOfSafety, output.PipingFactorOfSafety.GetAccuracy());
        }
    }
}