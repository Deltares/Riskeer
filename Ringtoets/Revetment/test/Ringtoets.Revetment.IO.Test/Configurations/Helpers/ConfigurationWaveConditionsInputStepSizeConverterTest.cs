// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations.Helpers;

namespace Ringtoets.Revetment.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationWaveConditionsInputStepSizeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherTypeThanString_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertToNonString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToNonString);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.Half, "0,5")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.One, "1,0")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.Two, "2,0")]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedText(ConfigurationWaveConditionsInputStepSize value,
                                                                  string expectedText)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void ConvertTo_Object_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(ConfigurationWaveConditionsInputStepSize.Half, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void CanConvertFrom_NullableDouble_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertFromNullableDouble = converter.CanConvertFrom(typeof(double?));

            // Assert
            Assert.IsTrue(canConvertFromNullableDouble);
        }

        [Test]
        public void CanConvertFrom_NonNullableDouble_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(0.5, ConfigurationWaveConditionsInputStepSize.Half)]
        [TestCase(1, ConfigurationWaveConditionsInputStepSize.One)]
        [TestCase(2, ConfigurationWaveConditionsInputStepSize.Two)]
        public void ConvertFrom_VariousDoubles_ReturnWaveConditionsInputStepSize(double value,
                                                                                 ConfigurationWaveConditionsInputStepSize expectedResult)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            object result = converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("1x");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(2.001)]
        [TestCase(double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.NaN)]
        [TestCase(-0.5)]
        public void ConvertFrom_InvalidDoubleValue_ThrowNotSupportedException(double value)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(value);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_Null_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null);

            // Assert        
            Assert.Throws<NotSupportedException>(call);
        }
    }
}