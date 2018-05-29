﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations.Helpers;

namespace Ringtoets.Revetment.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationFailureMechanismCategoryTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_FailureMechanismCategoryType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(FailureMechanismCategoryType));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }

        [Test]
        [TestCase(ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm, "Iv-IIv")]
        [TestCase(ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm, "IIv-IIIv")]
        [TestCase(ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm, "IIIv-IVv")]
        [TestCase(ConfigurationFailureMechanismCategoryType.LowerLimitNorm, "IVv-Vv")]
        [TestCase(ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm, "Vv-VIv")]
        public void ConvertTo_ValidConfigurationFailureMechanismCategoryType_ReturnExpectedText(
            ConfigurationFailureMechanismCategoryType value, string expectedText)
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            object convertTo = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, convertTo);
        }

        [Test]
        [TestCase(ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm)]
        [TestCase(ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm, FailureMechanismCategoryType.MechanismSpecificSignalingNorm)]
        [TestCase(ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm)]
        [TestCase(ConfigurationFailureMechanismCategoryType.LowerLimitNorm, FailureMechanismCategoryType.LowerLimitNorm)]
        [TestCase(ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm, FailureMechanismCategoryType.FactorizedLowerLimitNorm)]
        public void ConvertTo_ValidConfigurationFailureMechanismCategoryType_ReturnFailureMechanismCategoryType(
            ConfigurationFailureMechanismCategoryType originalValue, FailureMechanismCategoryType expectedResult)
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            object categoryType = converter.ConvertTo(null, CultureInfo.CurrentCulture, originalValue, typeof(FailureMechanismCategoryType));

            // Assert
            Assert.AreEqual(expectedResult, categoryType);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(FailureMechanismCategoryType))]
        public void ConvertTo_InvalidConfigurationFailureMechanismCategoryType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            const ConfigurationFailureMechanismCategoryType invalidValue = (ConfigurationFailureMechanismCategoryType) 99;
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationFailureMechanismCategoryType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_FailureMechanismCategoryType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(FailureMechanismCategoryType));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        [TestCase("Iv-IIv", ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm)]
        [TestCase("IIv-IIIv", ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm)]
        [TestCase("IIIv-IVv", ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm)]
        [TestCase("IVv-Vv", ConfigurationFailureMechanismCategoryType.LowerLimitNorm)]
        [TestCase("Vv-VIv", ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm)]
        public void ConvertFrom_ValidStringValue_ReturnConfigurationFailureMechanismCategoryType(
            string value, ConfigurationFailureMechanismCategoryType expectedResult)
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            object convertFrom = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, convertFrom);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("1x");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm, ConfigurationFailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm)]
        [TestCase(FailureMechanismCategoryType.MechanismSpecificSignalingNorm, ConfigurationFailureMechanismCategoryType.MechanismSpecificSignalingNorm)]
        [TestCase(FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm, ConfigurationFailureMechanismCategoryType.MechanismSpecificLowerLimitNorm)]
        [TestCase(FailureMechanismCategoryType.LowerLimitNorm, ConfigurationFailureMechanismCategoryType.LowerLimitNorm)]
        [TestCase(FailureMechanismCategoryType.FactorizedLowerLimitNorm, ConfigurationFailureMechanismCategoryType.FactorizedLowerLimitNorm)]
        public void ConvertFrom_ValidFailureMechanismCategoryType_ReturnConfigurationFailureMechanismCategoryType(
            FailureMechanismCategoryType originalValue, ConfigurationFailureMechanismCategoryType expectedResult)
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            object categoryType = converter.ConvertFrom(originalValue);

            // Assert
            Assert.AreEqual(expectedResult, categoryType);
        }

        [Test]
        public void ConvertFrom_InvalidFailureMechanismCategoryType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const FailureMechanismCategoryType invalidValue = (FailureMechanismCategoryType) 99;
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(FailureMechanismCategoryType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ConvertFrom_Null_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationFailureMechanismCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}