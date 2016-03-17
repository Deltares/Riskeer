using System;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingSemiProbabilisticOutputPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingSemiProbabilisticOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingSemiProbabilisticOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);
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
            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
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

            var properties = new PipingSemiProbabilisticOutputProperties
            {
                Data = semiProbabilisticOutput
            };

            // Call & Assert
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety, GetAccuracy(properties.UpliftFactorOfSafety));
            Assert.AreEqual(upliftReliability, properties.UpliftReliability, GetAccuracy(properties.UpliftReliability));
            Assert.AreEqual(upliftProbability, properties.UpliftProbability, GetAccuracy(properties.UpliftProbability));
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety, GetAccuracy(properties.HeaveFactorOfSafety));
            Assert.AreEqual(heaveReliability, properties.HeaveReliability, GetAccuracy(properties.HeaveReliability));
            Assert.AreEqual(heaveProbability, properties.HeaveProbability, GetAccuracy(properties.HeaveProbability));
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety, GetAccuracy(properties.SellmeijerFactorOfSafety));
            Assert.AreEqual(sellmeijerReliability, properties.SellmeijerReliability, GetAccuracy(properties.SellmeijerReliability));
            Assert.AreEqual(sellmeijerProbability, properties.SellmeijerProbability, GetAccuracy(properties.SellmeijerProbability));
            Assert.AreEqual(requiredProbability, properties.RequiredProbability, GetAccuracy(properties.RequiredProbability));
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, GetAccuracy(properties.RequiredReliability));
            Assert.AreEqual(pipingProbability, properties.PipingProbability, GetAccuracy(properties.PipingProbability));
            Assert.AreEqual(pipingReliability, properties.PipingReliability, GetAccuracy(properties.PipingReliability));
            Assert.AreEqual(pipingFactorOfSafety, properties.PipingFactorOfSafety, GetAccuracy(properties.PipingFactorOfSafety));
        }


        private static double GetAccuracy(RoundedDouble d)
        {
            return Math.Pow(10.0, -d.NumberOfDecimalPlaces);
        }
    }
}