using System;
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
            double heaveFactorOfSafety = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();
            double pipingFactorOfSafety = random.NextDouble();

            var semiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                upliftFactorOfSafety,
                heaveFactorOfSafety,
                sellmeijerFactorOfSafety,
                pipingFactorOfSafety);

            var properties = new PipingSemiProbabilisticOutputProperties
            {
                Data = semiProbabilisticOutput
            };

            // Call & Assert
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety);
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety);
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety);
            Assert.AreEqual(pipingFactorOfSafety, properties.PipingFactorOfSafety);
        }
    }
}