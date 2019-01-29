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
using Core.Common.Base.Data;
using Core.Common.Base.TypeConverters;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Forms.TypeConverters;

namespace Riskeer.Common.Forms.Test.TypeConverters
{
    [TestFixture]
    public class NoValueRoundedDoubleConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new NoValueRoundedDoubleConverter();

            // Assert
            Assert.IsInstanceOf<RoundedDoubleConverter>(converter);
        }

        [Test]
        [TestCase("")]
        [TestCase("     ")]
        [TestCase("-")]
        [TestCase("NaN")]
        public void ConvertFrom_NoValueText_ReturnNaN(string text)
        {
            // Setup
            var converter = new NoValueRoundedDoubleConverter();

            // Call
            var result = (RoundedDouble) converter.ConvertFrom(text);

            // Assert
            Assert.IsNaN(result.Value);
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, result.NumberOfDecimalPlaces);
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
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            const string text = "I'm not a number!";

            var converter = new NoValueRoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(context, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst moet een getal zijn.", message);
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

            var converter = new NoValueRoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(context, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst is een getal dat te groot of te klein is om gerepresenteerd te worden.", message);
            mocks.VerifyAll();
        }

        [Test]
        public void CanConvertTo_ToString_ReturnTrue()
        {
            // Setup
            var converter = new NoValueRoundedDoubleConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void ConvertTo_NaNToString_ReturnHyphen()
        {
            // Setup
            var converter = new NoValueRoundedDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(RoundedDouble.NaN, typeof(string));

            // Assert
            Assert.AreEqual("-", text);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-12.345)]
        [TestCase(56.967)]
        public void ConvertTo_NumberToString_ReturnStringInLocalDutchCulture(double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(2, value);
            var converter = new NoValueRoundedDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(roundedDouble, typeof(string));

            // Assert
            Assert.AreEqual(roundedDouble.ToString(), text);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase(-67.891)]
        [TestCase(23.346)]
        public void ConvertTo_NumberToString_ReturnStringInLocalEnglishCulture(double value)
        {
            // Setup
            var roundedDouble = new RoundedDouble(2, value);
            var converter = new NoValueRoundedDoubleConverter();

            // Call
            var text = (string) converter.ConvertTo(roundedDouble, typeof(string));

            // Assert
            Assert.AreEqual(roundedDouble.ToString(), text);
        }

        private static void DoConvertFrom_SomeNumericalTextInCurrentCulture_ReturnConvertedRoundedDouble(double input)
        {
            // Setup
            var mocks = new MockRepository();
            var context = mocks.Stub<ITypeDescriptorContext>();
            mocks.ReplayAll();

            string text = input.ToString(CultureInfo.CurrentCulture);

            var converter = new NoValueRoundedDoubleConverter();

            // Call
            var conversionResult = (RoundedDouble) converter.ConvertFrom(context, CultureInfo.CurrentCulture, text);

            // Assert
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, conversionResult.NumberOfDecimalPlaces);
            Assert.AreEqual(input, conversionResult.Value);
            mocks.VerifyAll();
        }
    }
}