using System;
using Core.Common.Base.Data;
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
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety, GetAccuracy(output.UpliftFactorOfSafety));
            Assert.AreEqual(upliftReliability, output.UpliftReliability, GetAccuracy(output.UpliftReliability));
            Assert.AreEqual(upliftProbability, output.UpliftProbability, GetAccuracy(output.UpliftProbability));
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety, GetAccuracy(output.HeaveFactorOfSafety));
            Assert.AreEqual(heaveReliability, output.HeaveReliability, GetAccuracy(output.HeaveReliability));
            Assert.AreEqual(heaveProbability, output.HeaveProbability, GetAccuracy(output.HeaveProbability));
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety, GetAccuracy(output.SellmeijerFactorOfSafety));
            Assert.AreEqual(sellmeijerReliability, output.SellmeijerReliability, GetAccuracy(output.SellmeijerReliability));
            Assert.AreEqual(sellmeijerProbability, output.SellmeijerProbability, GetAccuracy(output.SellmeijerProbability));
            Assert.AreEqual(requiredProbability, output.RequiredProbability, GetAccuracy(output.RequiredProbability));
            Assert.AreEqual(requiredReliability, output.RequiredReliability, GetAccuracy(output.RequiredReliability));
            Assert.AreEqual(pipingProbability, output.PipingProbability, GetAccuracy(output.PipingProbability));
            Assert.AreEqual(pipingReliability, output.PipingReliability, GetAccuracy(output.PipingReliability));
            Assert.AreEqual(pipingFactorOfSafety, output.PipingFactorOfSafety, GetAccuracy(output.PipingFactorOfSafety));
        }


        private static double GetAccuracy(RoundedDouble d)
        {
            return Math.Pow(10.0, -d.NumberOfDecimalPlaces);
        }
    }
}