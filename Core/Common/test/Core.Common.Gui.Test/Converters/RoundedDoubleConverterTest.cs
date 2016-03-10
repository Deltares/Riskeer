using System;
using System.ComponentModel;
using System.Globalization;

using Core.Common.Base.Data;
using Core.Common.Gui.Converters;

using NUnit.Framework;

namespace Core.Common.Gui.Test.Converters
{
    [TestFixture]
    public class RoundedDoubleConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new RoundedDoubleConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertFrom_StringType_ReturnTrue()
        {
            // Setup
            var converter = new RoundedDoubleConverter();

            // Call
            bool conversionIsPossible = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(conversionIsPossible);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new RoundedDoubleConverter();

            // Call
            bool conversionIsPossible = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(conversionIsPossible);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(123.4567)]
        [TestCase(-9.87654321)]
        public void ConvertFrom_SomeNumericalTextInDutchCulture_ReturnConvertedRoundedDouble(double input)
        {
            DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(input);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(12.34)]
        [TestCase(-0.96834715)]
        public void ConvertFrom_SomeNumericalTextInEnglishCulture_ReturnConvertedRoundedDouble(double input)
        {
            DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(input);
        }

        [Test]
        public void ConvertFrom_TextDoesNotRepresentNumber_ThrowNotSupportedException()
        {
            // Setup
            string text = "I'm not a number!";

            var converter = new RoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst moet een getal zijn.", message);
        }

        [Test]
        public void ConvertFrom_TextTooLongToStoreInDouble_ThrowNotSupportedException()
        {
            // Setup
            string text = "1" + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            var converter = new RoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst is een getal dat te groot of te klein is om gerepresenteerd te worden.", message);
        }

        private static void DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(double input)
        {
            // Setup
            string text = input.ToString(CultureInfo.CurrentCulture);

            var converter = new RoundedDoubleConverter();

            // Call
            RoundedDouble conversionResult = (RoundedDouble)converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            Assert.IsNotNull(conversionResult);
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, conversionResult.NumberOfDecimalPlaces);
            Assert.AreEqual(input, conversionResult.Value);
        }
    }
}