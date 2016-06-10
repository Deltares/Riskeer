using System;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class DecimalConversionExtensionsTests
    {
        [Test]
        public void ToNanableDouble_Null_ReturnNaN()
        {
            // Setup
            decimal? value = null;

            // Call
            double result = value.ToNanableDouble();

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void ToNanableDouble_Number_ReturnNumberAsDouble(
            [Random(-9999.9999, 9999.9999, 1)]double expectedValue)
        {
            // Setup
            decimal? value = Convert.ToDecimal(expectedValue);

            // Call
            double result = value.ToNanableDouble();

            // Assert
            Assert.AreEqual(expectedValue, result, 1e-6);
        }
    }
}