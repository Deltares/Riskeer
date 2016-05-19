using System;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
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
            var bigFactor = 1000000;
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double upliftProbability = random.NextDouble()*bigFactor;
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double heaveProbability = random.NextDouble()*bigFactor;
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double sellmeijerProbability = random.NextDouble()*bigFactor;
            double requiredProbability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingProbability = random.NextDouble()*bigFactor;
            double pipingReliability = random.NextDouble()*bigFactor;
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
            var probabilityFormat = "1/{0:n0}";
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety, properties.UpliftFactorOfSafety.GetAccuracy());
            Assert.AreEqual(upliftReliability, properties.UpliftReliability, properties.UpliftReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, upliftProbability), properties.UpliftProbability);
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety, properties.HeaveFactorOfSafety.GetAccuracy());
            Assert.AreEqual(heaveReliability, properties.HeaveReliability, properties.HeaveReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, heaveProbability), properties.HeaveProbability);
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety.GetAccuracy());
            Assert.AreEqual(sellmeijerReliability, properties.SellmeijerReliability, properties.SellmeijerReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, sellmeijerProbability), properties.SellmeijerProbability);
            Assert.AreEqual(string.Format(probabilityFormat, requiredProbability), properties.RequiredProbability);
            Assert.AreEqual(requiredReliability, properties.RequiredReliability, properties.RequiredReliability.GetAccuracy());
            Assert.AreEqual(string.Format(probabilityFormat, pipingProbability), properties.PipingProbability);
            Assert.AreEqual(pipingReliability, properties.PipingReliability, properties.PipingReliability.GetAccuracy());
            Assert.AreEqual(pipingFactorOfSafety, properties.PipingFactorOfSafety, properties.PipingFactorOfSafety.GetAccuracy());
        }

        [Test]
        public void GetProperties_WithInfinityValues_ReturnTranslatedFormat()
        {
            // Setup
            var random = new Random(22);
            double upliftFactorOfSafety = random.NextDouble();
            double upliftReliability = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double heaveReliability = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double sellmeijerReliability = random.NextDouble();
            double requiredReliability = random.NextDouble();
            double pipingReliability = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            double upliftProbability = double.PositiveInfinity;
            double heaveProbability = double.PositiveInfinity;
            double sellmeijerProbability = double.PositiveInfinity;
            double requiredProbability = double.PositiveInfinity;
            double pipingProbability = double.PositiveInfinity;

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
            var probability = "1/Oneindig";
            Assert.AreEqual(probability, properties.UpliftProbability);
            Assert.AreEqual(probability, properties.HeaveProbability);
            Assert.AreEqual(probability, properties.SellmeijerProbability);
            Assert.AreEqual(probability, properties.RequiredProbability);
            Assert.AreEqual(probability, properties.PipingProbability);
        }
    }
}