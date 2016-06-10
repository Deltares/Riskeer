using System;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class DoubleConversionExtensionsTest
    {
        [Test]
        public void ToNullableDecimal_NaN_ReturnNull()
        {
            // Call
            decimal? result = double.NaN.ToNullableDecimal();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ToNullableDecimal_Number_ReturnThatNumberAsDecimal(
            [Random(-9999.9999, 9999.9999, 1)] double value)
        {
            // Call
            decimal? result = value.ToNullableDecimal();

            // Assert
            Assert.AreEqual(value, Convert.ToDouble(result), 1e-6);
        }
    }
}