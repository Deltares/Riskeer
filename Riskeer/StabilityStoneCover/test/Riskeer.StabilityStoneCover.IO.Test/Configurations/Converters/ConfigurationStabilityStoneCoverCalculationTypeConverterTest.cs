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
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.IO.Configurations;
using Riskeer.StabilityStoneCover.IO.Configurations.Converters;

namespace Riskeer.StabilityStoneCover.IO.Test.Configurations.Converters
{
    [TestFixture]
    public class ConfigurationStabilityStoneCoverCalculationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_StabilityStoneCoverWaveConditionsCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(StabilityStoneCoverWaveConditionsCalculationType));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(StabilityStoneCoverWaveConditionsCalculationType))]
        public void ConvertTo_InvalidConfigurationStabilityStoneCoverCalculationType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            const StabilityStoneCoverWaveConditionsCalculationType invalidValue = (StabilityStoneCoverWaveConditionsCalculationType) 99;
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationStabilityStoneCoverCalculationType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(ConfigurationStabilityStoneCoverCalculationType.Both, StabilityStoneCoverWaveConditionsCalculationType.Both)]
        [TestCase(ConfigurationStabilityStoneCoverCalculationType.Blocks, StabilityStoneCoverWaveConditionsCalculationType.Blocks)]
        [TestCase(ConfigurationStabilityStoneCoverCalculationType.Columns, StabilityStoneCoverWaveConditionsCalculationType.Columns)]
        public void ConvertTo_ValidConfigurationStabilityStoneCoverCalculationType_ReturnStabilityStoneCoverWaveConditionsCalculationType(
            ConfigurationStabilityStoneCoverCalculationType originalValue, StabilityStoneCoverWaveConditionsCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            object calculationType = converter.ConvertTo(null, CultureInfo.CurrentCulture, originalValue, typeof(StabilityStoneCoverWaveConditionsCalculationType));

            // Assert
            Assert.AreEqual(expectedResult, calculationType);
        }

        [Test]
        [TestCase(ConfigurationStabilityStoneCoverCalculationType.Both, "Steen (blokken en zuilen)")]
        [TestCase(ConfigurationStabilityStoneCoverCalculationType.Blocks, "Steen (blokken)")]
        [TestCase(ConfigurationStabilityStoneCoverCalculationType.Columns, "Steen (zuilen)")]
        public void ConvertTo_ValidConfigurationStabilityStoneCoverCalculationType_ReturnExpectedText(
            ConfigurationStabilityStoneCoverCalculationType originalValue, string expectedText)
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            object calculationType = converter.ConvertTo(null, CultureInfo.CurrentCulture, originalValue, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, calculationType);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_StabilityStoneCoverWaveConditionsCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(StabilityStoneCoverWaveConditionsCalculationType));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        public void ConvertFrom_InvalidStabilityStoneCoverWaveConditionsCalculationType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const StabilityStoneCoverWaveConditionsCalculationType invalidValue = (StabilityStoneCoverWaveConditionsCalculationType)99;
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(StabilityStoneCoverWaveConditionsCalculationType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase("Steen (blokken en zuilen)", ConfigurationStabilityStoneCoverCalculationType.Both)]
        [TestCase("Steen (blokken)", ConfigurationStabilityStoneCoverCalculationType.Blocks)]
        [TestCase("Steen (zuilen)", ConfigurationStabilityStoneCoverCalculationType.Columns)]
        public void ConvertFrom_ValidStringValue_ReturnConfigurationStabilityStoneCoverCalculationType(
            string originalValue, ConfigurationStabilityStoneCoverCalculationType expectedValue)
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            object convertedValue = converter.ConvertFrom(originalValue);

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Both, ConfigurationStabilityStoneCoverCalculationType.Both)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Blocks, ConfigurationStabilityStoneCoverCalculationType.Blocks)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Columns, ConfigurationStabilityStoneCoverCalculationType.Columns)]
        public void ConvertFrom_ValidStabilityStoneCoverWaveConditionsCalculationType_ReturnConfigurationStabilityStoneCoverCalculationType(
            StabilityStoneCoverWaveConditionsCalculationType originalValue, ConfigurationStabilityStoneCoverCalculationType expectedValue)
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            object convertedValue = converter.ConvertFrom(originalValue);

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        public void ConvertFrom_Null_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationStabilityStoneCoverCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}