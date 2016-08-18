﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
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

namespace Core.Common.Base.Test.TypeConverters
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
        public void ConvertFrom_TextIsEmpty_ThrowNotSupportedException()
        {
            // Setup
            string text = string.Empty;

            var converter = new RoundedDoubleConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual("De tekst mag niet leeg zijn.", message);
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