using System;
using DelftTools.Shell.Gui;
using NUnit.Framework;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingOutputPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(22);

            double upliftZValue = random.NextDouble();
            double upliftFactorOfSafety = random.NextDouble();
            double heaveZValue = random.NextDouble();
            double heaveFactorOfSafety = random.NextDouble();
            double sellmeijerZValue = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();

            var pipingOutput = new PipingOutput(
                upliftZValue,
                upliftFactorOfSafety,
                heaveZValue,
                heaveFactorOfSafety,
                sellmeijerZValue,
                sellmeijerFactorOfSafety);

            var properties = new PipingOutputProperties
            {
                Data = pipingOutput
            };

            // Call & Assert
            Assert.AreEqual(upliftZValue, properties.UpliftZValue);
            Assert.AreEqual(upliftFactorOfSafety, properties.UpliftFactorOfSafety);
            Assert.AreEqual(heaveZValue, properties.HeaveZValue);
            Assert.AreEqual(heaveFactorOfSafety, properties.HeaveFactorOfSafety);
            Assert.AreEqual(sellmeijerZValue, properties.SellmeijerZValue);
            Assert.AreEqual(sellmeijerFactorOfSafety, properties.SellmeijerFactorOfSafety);
        }
    }
}