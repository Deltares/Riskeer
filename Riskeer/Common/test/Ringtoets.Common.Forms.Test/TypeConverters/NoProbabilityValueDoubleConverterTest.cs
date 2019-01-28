// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Globalization;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.Test.TypeConverters
{
    [TestFixture]
    public class NoProbabilityValueDoubleConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new NoProbabilityValueDoubleConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanString_ReturnFalse()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        public void ConvertFrom_Object_ThrowNotSupportedException()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(new object());

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("-")]
        [TestCase("NaN")]
        public void ConvertFrom_NoValueText_ReturnNaN(string text)
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var result = (double) converter.ConvertFrom(text);

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(123.4567)]
        [TestCase(-9.87654321)]
        public void ConvertFrom_SomeNumericalTextInDutchCulture_ReturnConvertedDouble(double input)
        {
            DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedDouble(input);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(12.34)]
        [TestCase(-0.96834715)]
        public void ConvertFrom_SomeNumericalTextInEnglishCulture_ReturnConvertedDouble(double input)
        {
            DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedDouble(input);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("1/25", 0.04)]
        [TestCase("1/2,500", 0.4)]
        [TestCase("1/2.500", 0.0004)]
        [TestCase("1e-3", 0.001)]
        [TestCase("1/1.000", 0.001)]
        [TestCase("1/0", double.PositiveInfinity)]
        [TestCase("1/Oneindig", 0.0)]
        [TestCase("1/oneINdig", 0.0)]
        [TestCase("1/-1.000", -0.001)]
        [TestCase("1/-10", -0.1)]
        [TestCase("1/-2,5", -0.4)]
        [TestCase("-0,5", -0.5)]
        [TestCase("-1", -1.0)]
        [TestCase("-1e-2", -0.01)]
        public void ConvertFrom_StringInDutchCulture_ReturnExpectedConvertedDouble(string input, double expectedOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var conversionResult = (double) converter.ConvertFrom(context, CultureInfo.CurrentCulture, input);

            // Assert
            Assert.AreEqual(expectedOutput, conversionResult);
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase("1/25", 0.04)]
        [TestCase("1/2.5", 0.4)]
        [TestCase("1e-3", 0.001)]
        [TestCase("1/1,000", 0.001)]
        [TestCase("1/0", double.PositiveInfinity)]
        [TestCase("1/Oneindig", 0.0)]
        [TestCase("1/oneINdig", 0.0)]
        [TestCase("1/-1,000", -0.001)]
        [TestCase("1/-10", -0.1)]
        [TestCase("1/-2.5", -0.4)]
        [TestCase("-0.5", -0.5)]
        [TestCase("-1", -1.0)]
        [TestCase("-1e-2", -0.01)]
        public void ConvertFrom_StringInEnglishCulture_ReturnExpectedConvertedDouble(string input, double expectedOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var conversionResult = (double) converter.ConvertFrom(context, CultureInfo.CurrentCulture, input);

            // Assert
            Assert.AreEqual(expectedOutput, conversionResult);
            mocks.VerifyAll();
        }

        [Test]
        public void ConvertFrom_TextDoesNotRepresentNumber_ThrowNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            const string text = "I'm not a number!";

            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(context, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De waarde kon niet geïnterpreteerd worden als een kans.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void ConvertFrom_TextTooLongToStoreInDouble_ThrowNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            string text = "1" + double.MaxValue.ToString(CultureInfo.CurrentCulture);

            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(context, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De waarde is te groot of te klein.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void CanConvertTo_ToString_ReturnTrue()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_ToObject_ReturnFalse()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            bool canConvertToObject = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToObject);
        }

        [Test]
        public void ConvertTo_Object_ThrowNotSupportedException()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(1.1, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertTo_NaNToString_ReturnHyphen()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(double.NaN, typeof(string));

            // Assert
            Assert.AreEqual("-", text);
        }

        [Test]
        public void ConvertTo_PositiveInfinityToString_ReturnInfinity()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(double.PositiveInfinity, typeof(string));

            // Assert
            Assert.AreEqual("Oneindig", text);
        }

        [Test]
        public void ConvertTo_NegativeInfinityToString_ReturnNegativeInfinity()
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(double.NegativeInfinity, typeof(string));

            // Assert
            Assert.AreEqual("-Oneindig", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-0.000235)]
        [TestCase(0.0069)]
        [TestCase(0.000000000000000069)]
        public void ConvertTo_NumberToString_ReturnStringInLocalDutchCulture(double value)
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(value, typeof(string));

            // Assert
            string expectedText = ProbabilityFormattingHelper.Format(value);
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(-0.0000658)]
        [TestCase(0.000006788)]
        [TestCase(-0.000000000000000069)]
        public void ConvertTo_NumberToString_ReturnStringInLocalEnglishCulture(double value)
        {
            // Setup
            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(value, typeof(string));

            // Assert
            string expectedText = ProbabilityFormattingHelper.Format(value);
            Assert.AreEqual(expectedText, text);
        }

        private static void DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedDouble(double input)
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            string text = input.ToString(CultureInfo.CurrentCulture);

            var converter = new NoProbabilityValueDoubleConverter();

            // Call
            var conversionResult = (double) converter.ConvertFrom(context, CultureInfo.CurrentCulture, text);

            // Assert
            Assert.AreEqual(input, conversionResult);
            mocks.VerifyAll();
        }
    }
}