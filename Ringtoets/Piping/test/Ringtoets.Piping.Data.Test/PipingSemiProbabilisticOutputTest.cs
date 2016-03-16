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
            double heaveFactorOfSafety = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            // Call
            var output = new PipingSemiProbabilisticOutput(upliftFactorOfSafety, heaveFactorOfSafety, sellmeijerFactorOfSafety, pipingFactorOfSafety);

            // Assert
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety);
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety);
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety);
            Assert.AreEqual(pipingFactorOfSafety, output.PipingFactorOfSafety);
        } 
    }
}