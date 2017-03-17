// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class WaveConditionsInputStepSizeTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherTypeThenString_ReturnFalse()
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            bool canConvertToNonString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToNonString);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(WaveConditionsInputStepSize.Half, "0,5")]
        [TestCase(WaveConditionsInputStepSize.One, "1,0")]
        [TestCase(WaveConditionsInputStepSize.Two, "2,0")]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedText(WaveConditionsInputStepSize value,
                                                                  string expectedText)
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void ConvertTo_Object_ThrowNotSupportedException()
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(WaveConditionsInputStepSize.Half, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_NonString_ReturnFalse()
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("0,5", WaveConditionsInputStepSize.Half)]
        [TestCase("1,0", WaveConditionsInputStepSize.One)]
        [TestCase("2,0", WaveConditionsInputStepSize.Two)]
        public void ConvertFrom_VariousCasesInCultureNL_ReturnWaveConditionsInputStepSize(string text,
                                                                                          WaveConditionsInputStepSize expectedResult)
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            object result = converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [SetCulture("en-US")]
        [TestCase("0.5", WaveConditionsInputStepSize.Half)]
        [TestCase("1.0", WaveConditionsInputStepSize.One)]
        [TestCase("2.0", WaveConditionsInputStepSize.Two)]
        public void ConvertFrom_VariousCasesInCultureEN_ReturnWaveConditionsInputStepSize(string text,
                                                                                          WaveConditionsInputStepSize expectedResult)
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            object result = converter.ConvertFrom(null, CultureInfo.CurrentCulture, text);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new WaveConditionsInputStepSizeTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("1");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}