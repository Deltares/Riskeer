using System;
using System.ComponentModel;
using System.Globalization;

using Core.Common.Gui.Converters;
using Core.Common.TestUtil;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class RoundDoubleToDecimalPlacesConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Call
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        [TestCase(-46)]
        [TestCase(-1)]
        [TestCase(29)]
        [TestCase(23456789)]
        public void Constructor_InvalidNumberOfPlaces_ThrowArgumentOutOfRangeException(int invalidNumberOfPlaces)
        {
            // Call
            TestDelegate call = () => new RoundDoubleToDecimalPlacesConverter(invalidNumberOfPlaces);

            // Assert
            string expectedMessage = "Value must be in range [0, 28].";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void CanConvertFrom_SourceIsStringType_ReturnTrue(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            var isConversionPossible = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(isConversionPossible);
        }

        [Test]
        public void CanConvertFrom_SourceIsDoubleType_ReturnTrue(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            var isConversionPossible = converter.CanConvertFrom(typeof(double));

            // Assert
            Assert.IsTrue(isConversionPossible);
        }

        [Test]
        public void CanConvertFrom_SourceIsNotDoubleNorStringType_ReturnFalse(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            var isConversionPossible = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(isConversionPossible);
        }

        [Test]
        public void CanConvertTo_TargetIsStringType_ReturnTrue(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            var isConversionPossible = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(isConversionPossible);
        }

        [Test]
        public void CanConvertTo_TargetIsDoubleType_ReturnTrue(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            var isConversionPossible = converter.CanConvertTo(typeof(double));

            // Assert
            Assert.IsTrue(isConversionPossible);
        }

        [Test]
        public void CanConvertTo_TargetIsNotDoubleNorStringType_ReturnFalse(
            [Random(0, 28, 1)]int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            var isConversionPossible = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(isConversionPossible);
        }

        [Test]
        [TestCase(1.0, 2, 1.00)]
        [TestCase(123456789.0, 3, 123456789.000)]
        [TestCase(12345678.90, 2, 12345678.90)]
        [TestCase(12345678.90, 3, 12345678.900)]
        [TestCase(1234567.890, 2, 1234567.89)]
        [TestCase(1234567.890, 3, 1234567.890)]
        [TestCase(123456.7890, 2, 123456.79)]
        [TestCase(123456.7890, 3, 123456.789)]
        [TestCase(12345.67890, 2, 12345.68)]
        [TestCase(12345.67890, 3, 12345.679)]
        [TestCase(1234.567890, 2, 1234.57)]
        [TestCase(1234.567890, 3, 1234.568)]
        [TestCase(123.4567890, 2, 123.46)]
        [TestCase(123.4567890, 3, 123.457)]
        [TestCase(12.34567890, 2, 12.35)]
        [TestCase(12.34567890, 3, 12.346)]
        [TestCase(1.234567890, 2, 1.23)]
        [TestCase(1.234567890, 3, 1.235)]
        [TestCase(0.1234567890, 2, 0.12)]
        [TestCase(0.1234567890, 3, 0.123)]
        [TestCase(0.01234567890, 2, 0.01)]
        [TestCase(0.01234567890, 3, 0.012)]
        [TestCase(0.001234567890, 2, 0.00)]
        [TestCase(0.001234567890, 3, 0.001)]
        [TestCase(0.0001234567890, 2, 0.00)]
        [TestCase(0.0001234567890, 3, 0.000)]
        public void ConvertTo_FromDoubleToDouble_ReturnRoundedDouble(
            double input, int numberOfPlaces, double expectedOutput)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            double result = (double)converter.ConvertTo(input, typeof(double));

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCase(1.0, 2, 1.00)]
        [TestCase(123456789.0, 3, 123456789.000)]
        [TestCase(12345678.90, 2, 12345678.90)]
        [TestCase(12345678.90, 3, 12345678.900)]
        [TestCase(1234567.890, 2, 1234567.89)]
        [TestCase(1234567.890, 3, 1234567.890)]
        [TestCase(123456.7890, 2, 123456.79)]
        [TestCase(123456.7890, 3, 123456.789)]
        [TestCase(12345.67890, 2, 12345.68)]
        [TestCase(12345.67890, 3, 12345.679)]
        [TestCase(1234.567890, 2, 1234.57)]
        [TestCase(1234.567890, 3, 1234.568)]
        [TestCase(123.4567890, 2, 123.46)]
        [TestCase(123.4567890, 3, 123.457)]
        [TestCase(12.34567890, 2, 12.35)]
        [TestCase(12.34567890, 3, 12.346)]
        [TestCase(1.234567890, 2, 1.23)]
        [TestCase(1.234567890, 3, 1.235)]
        [TestCase(0.1234567890, 2, 0.12)]
        [TestCase(0.1234567890, 3, 0.123)]
        [TestCase(0.01234567890, 2, 0.01)]
        [TestCase(0.01234567890, 3, 0.012)]
        [TestCase(0.001234567890, 2, 0.00)]
        [TestCase(0.001234567890, 3, 0.001)]
        [TestCase(0.0001234567890, 2, 0.00)]
        [TestCase(0.0001234567890, 3, 0.000)]
        public void ConvertTo_FromStringToDouble_ReturnRoundedDouble(
            double input, int numberOfPlaces, double expectedOutput)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string stringInput = input.ToString(currentCulture);

            // Call
            double result = (double)converter.ConvertTo(null, currentCulture, stringInput, typeof(double));

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(1.0, 2, "1.00")]
        [TestCase(123456789.0, 3, "123456789.000")]
        [TestCase(12345678.90, 2, "12345678.90")]
        [TestCase(12345678.90, 3, "12345678.900")]
        [TestCase(1234567.890, 2, "1234567.89")]
        [TestCase(1234567.890, 3, "1234567.890")]
        [TestCase(123456.7890, 2, "123456.79")]
        [TestCase(123456.7890, 3, "123456.789")]
        [TestCase(12345.67890, 2, "12345.68")]
        [TestCase(12345.67890, 3, "12345.679")]
        [TestCase(1234.567890, 2, "1234.57")]
        [TestCase(1234.567890, 3, "1234.568")]
        [TestCase(123.4567890, 2, "123.46")]
        [TestCase(123.4567890, 3, "123.457")]
        [TestCase(12.34567890, 2, "12.35")]
        [TestCase(12.34567890, 3, "12.346")]
        [TestCase(1.234567890, 2, "1.23")]
        [TestCase(1.234567890, 3, "1.235")]
        [TestCase(0.1234567890, 2, "0.12")]
        [TestCase(0.1234567890, 3, "0.123")]
        [TestCase(0.01234567890, 2, "0.01")]
        [TestCase(0.01234567890, 3, "0.012")]
        [TestCase(0.001234567890, 2, "0.00")]
        [TestCase(0.001234567890, 3, "0.001")]
        [TestCase(0.0001234567890, 2, "0.00")]
        [TestCase(0.0001234567890, 3, "0.000")]
        public void ConvertTo_FromDoubleToString_ReturnRoundedDouble(
            double input, int numberOfPlaces, string expectedOutput)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            // Call
            string result = (string)converter.ConvertTo(null, CultureInfo.CurrentCulture, input, typeof(string));

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1.0, 2, "1,00")]
        [TestCase(123456789.0, 3, "123456789,000")]
        [TestCase(12345678.90, 2, "12345678,90")]
        [TestCase(12345678.90, 3, "12345678,900")]
        [TestCase(1234567.890, 2, "1234567,89")]
        [TestCase(1234567.890, 3, "1234567,890")]
        [TestCase(123456.7890, 2, "123456,79")]
        [TestCase(123456.7890, 3, "123456,789")]
        [TestCase(12345.67890, 2, "12345,68")]
        [TestCase(12345.67890, 3, "12345,679")]
        [TestCase(1234.567890, 2, "1234,57")]
        [TestCase(1234.567890, 3, "1234,568")]
        [TestCase(123.4567890, 2, "123,46")]
        [TestCase(123.4567890, 3, "123,457")]
        [TestCase(12.34567890, 2, "12,35")]
        [TestCase(12.34567890, 3, "12,346")]
        [TestCase(1.234567890, 2, "1,23")]
        [TestCase(1.234567890, 3, "1,235")]
        [TestCase(0.1234567890, 2, "0,12")]
        [TestCase(0.1234567890, 3, "0,123")]
        [TestCase(0.01234567890, 2, "0,01")]
        [TestCase(0.01234567890, 3, "0,012")]
        [TestCase(0.001234567890, 2, "0,00")]
        [TestCase(0.001234567890, 3, "0,001")]
        [TestCase(0.0001234567890, 2, "0,00")]
        [TestCase(0.0001234567890, 3, "0,000")]
        public void ConvertTo_FromStringToString_ReturnRoundedDouble(
            double input, int numberOfPlaces, string expectedOutput)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            string stringInput = input.ToString(CultureInfo.CurrentCulture);

            // Call
            string result = (string)converter.ConvertTo(null, CultureInfo.CurrentCulture, stringInput, typeof(string));

            // Assert
            Assert.AreEqual(expectedOutput, result);
        }

        [Test]
        [TestCase("I'm really not a number", 2)]
        [TestCase("This value also does not represent a number!", 3)]
        public void ConvertTo_FromNotNumberStringToDouble_ThrowNowSupportedException(
            string stringInput, int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Call
            TestDelegate call = () => converter.ConvertTo(null, currentCulture, stringInput, typeof(double));

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual(string.Format("De waarde '{0}' is geen getal.", stringInput), message);
        }

        [Test]
        [TestCase(79228162514264337593543950335.0 + 1e-6, 2)]
        [TestCase(79228162514264337593543950335.0 + 1e-6, 3)]
        [TestCase("79228162514264337593543950336", 2)]
        [TestCase("79228162514264337593543950336", 3)]
        [TestCase(99999999999999999999999999999.0, 2)]
        [TestCase(99999999999999999999999999999.0, 3)]
        [TestCase("99999999999999999999999999999", 2)]
        [TestCase("99999999999999999999999999999", 3)]
        [TestCase(-79228162514264337593543950335.0 - 1e-6, 2)]
        [TestCase(-79228162514264337593543950335.0 - 1e-6, 3)]
        [TestCase("-79228162514264337593543950336", 2)]
        [TestCase("-79228162514264337593543950336", 3)]
        [TestCase(-99999999999999999999999999999.0, 2)]
        [TestCase(-99999999999999999999999999999.0, 3)]
        [TestCase("-99999999999999999999999999999", 2)]
        [TestCase("-99999999999999999999999999999", 3)]
        public void ConvertTo_FromTooLargeOrTooSmallNumberToDouble_ThrowNowSupportedException(
            object input, int numberOfPlaces)
        {
            // Setup
            var converter = new RoundDoubleToDecimalPlacesConverter(numberOfPlaces);

            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Call
            TestDelegate call = () => converter.ConvertTo(null, currentCulture, input, typeof(double));

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual(string.Format("De waarde '{0}' is te groot of te klein om te kunnen verwerken.", input), message);
        }
    }
}