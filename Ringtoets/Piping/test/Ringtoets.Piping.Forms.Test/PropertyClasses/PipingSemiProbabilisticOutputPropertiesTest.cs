using System;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
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
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety, properties.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(upliftReliability, properties.UpliftReliability, properties.UpliftReliability.GetAccuracy());
            Assert.AreEqual(upliftProbability, properties.UpliftProbability, properties.UpliftProbability.GetAccuracy());
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety, properties.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, properties.HeaveReliability, properties.HeaveReliability.GetAccuracy());
            Assert.AreEqual(heaveProbability, properties.HeaveProbability, properties.HeaveProbability.GetAccuracy());
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, properties.SellmeijerReliability, properties.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(sellmeijerProbability, properties.SellmeijerProbability, properties.SellmeijerProbability.GetAccuracy());
            Assert.AreEqual(requiredProbability, properties.RequiredProbability, properties.RequiredProbability.GetAccuracy());
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(pipingProbability, properties.PipingProbability, properties.PipingProbability.GetAccuracy());
            Assert.AreEqual(pipingReliability, properties.PipingReliability, properties.PipingReliability.GetAccuracy());
            Assert.AreEqual(pipingFactorOfSafety, properties.PipingFactorOfSafety, properties.PipingFactorOfSafety.GetAccuracy());
        }
    }
}