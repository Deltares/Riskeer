using System;
using NUnit.Framework;

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
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety);
            Assert.AreEqual(upliftReliability, output.UpliftReliability);
            Assert.AreEqual(upliftProbability, output.UpliftProbability);
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety);
            Assert.AreEqual(heaveReliability, output.HeaveReliability);
            Assert.AreEqual(heaveProbability, output.HeaveProbability);
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety);
            Assert.AreEqual(sellmeijerReliability, output.SellmeijerReliability);
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability);
            Assert.AreEqual(requiredProbability, output.RequiredProbability);
            Assert.AreEqual(requiredReliability, output.RequiredReliability);
            Assert.AreEqual(pipingProbability, output.PipingProbability);
            Assert.AreEqual(pipingReliability, output.PipingReliability);
            Assert.AreEqual(pipingFactorOfSafety, output.PipingFactorOfSafety);
        } 
    }
}