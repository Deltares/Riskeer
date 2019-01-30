// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.Revetment.IO.Configurations.Converters;

namespace Riskeer.Revetment.IO.Test.Configurations.Converters
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
            bool canConvertTo = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_WaveConditionsInputStepSize_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(WaveConditionsInputStepSize));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.Half, "0,5")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.One, "1,0")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.Two, "2,0")]
        public void ConvertTo_ValidConfigurationWaveConditionsInputStepSize_ReturnExpectedText(
            ConfigurationWaveConditionsInputStepSize value, string expectedText)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            object convertTo = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, convertTo);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(WaveConditionsInputStepSize))]
        public void ConvertTo_InvalidConfigurationWaveConditionsInputStepSize_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            const ConfigurationWaveConditionsInputStepSize invalidValue = (ConfigurationWaveConditionsInputStepSize) 9999;
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationWaveConditionsInputStepSize)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(ConfigurationWaveConditionsInputStepSize.Half, WaveConditionsInputStepSize.Half)]
        [TestCase(ConfigurationWaveConditionsInputStepSize.One, WaveConditionsInputStepSize.One)]
        [TestCase(ConfigurationWaveConditionsInputStepSize.Two, WaveConditionsInputStepSize.Two)]
        public void ConvertTo_ValidConfigurationWaveConditionsInputStepSize_ReturnWaveConditionsInputStepSize(
            ConfigurationWaveConditionsInputStepSize value, WaveConditionsInputStepSize expectedResult)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            object convertTo = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(WaveConditionsInputStepSize));

            // Assert
            Assert.AreEqual(expectedResult, convertTo);
        }

        [Test]
        public void ConvertTo_Object_ThrowNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(random.NextEnumValue<ConfigurationWaveConditionsInputStepSize>(), typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void CanConvertFrom_NullableDouble_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(double?));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_WaveConditionsInputStepSize_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(WaveConditionsInputStepSize));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        [TestCase(0.5, ConfigurationWaveConditionsInputStepSize.Half)]
        [TestCase(1, ConfigurationWaveConditionsInputStepSize.One)]
        [TestCase(2, ConfigurationWaveConditionsInputStepSize.Two)]
        public void ConvertFrom_ValidDoubleValue_ReturnWaveConditionsInputStepSize(
            double value, ConfigurationWaveConditionsInputStepSize expectedResult)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            object convertFrom = converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);

            // Assert
            Assert.AreEqual(expectedResult, convertFrom);
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
        [TestCase(WaveConditionsInputStepSize.Half, ConfigurationWaveConditionsInputStepSize.Half)]
        [TestCase(WaveConditionsInputStepSize.One, ConfigurationWaveConditionsInputStepSize.One)]
        [TestCase(WaveConditionsInputStepSize.Two, ConfigurationWaveConditionsInputStepSize.Two)]
        public void ConvertFrom_ValidWaveConditionsInputStepSize_ReturnConfigurationWaveConditionsInputStepSize(
            WaveConditionsInputStepSize value, ConfigurationWaveConditionsInputStepSize expectedResult)
        {
            // Setup
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            object convertFrom = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, convertFrom);
        }

        [Test]
        public void ConvertFrom_InvalidWaveConditionsInputStepSize_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const WaveConditionsInputStepSize invalidValue = (WaveConditionsInputStepSize) 9999;
            var converter = new ConfigurationWaveConditionsInputStepSizeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(WaveConditionsInputStepSize)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
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