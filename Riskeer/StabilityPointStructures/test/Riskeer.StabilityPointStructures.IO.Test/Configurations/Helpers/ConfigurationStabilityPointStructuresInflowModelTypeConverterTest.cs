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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.IO.Configurations;
using Riskeer.StabilityPointStructures.IO.Configurations.Helpers;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationStabilityPointStructuresInflowModelTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_StabilityPointStructureInflowModelType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(StabilityPointStructureInflowModelType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherTypeThanStringOrStabilityPointStructureInflowModelType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            bool canConvertToNonString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToNonString);
        }

        [Test]
        [TestCase(ConfigurationStabilityPointStructuresInflowModelType.LowSill, StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure)]
        [TestCase(ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert, StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure)]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedText(ConfigurationStabilityPointStructuresInflowModelType value,
                                                                  string expectedText)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void ConvertTo_InvalidType_ThrowNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            var invalidValue = random.NextEnumValue<ConfigurationStabilityPointStructuresInflowModelType>();
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(StabilityPointStructureInflowModelType))]
        public void ConvertTo_InvalidConfigurationInflowModelType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();
            const ConfigurationStabilityPointStructuresInflowModelType invalidValue = (ConfigurationStabilityPointStructuresInflowModelType) 99999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationStabilityPointStructuresInflowModelType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        [TestCase(ConfigurationStabilityPointStructuresInflowModelType.LowSill, StabilityPointStructureInflowModelType.LowSill)]
        [TestCase(ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.FloodedCulvert)]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedType(ConfigurationStabilityPointStructuresInflowModelType value,
                                                                  StabilityPointStructureInflowModelType expectedText)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(StabilityPointStructureInflowModelType));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_StabilityPointStructureInflowModelType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(StabilityPointStructureInflowModelType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanStringOrStabilityPointStructureInflowModelType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure, ConfigurationStabilityPointStructuresInflowModelType.LowSill)]
        [TestCase(StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure, ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert)]
        public void ConvertFrom_Text_ReturnExpectedConfigurationInflowModelType(string value,
                                                                                ConfigurationStabilityPointStructuresInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(new object());

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();
            const string invalidValue = "some text";

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Value '{invalidValue}' is not supported.", message);
        }

        [Test]
        public void ConvertFrom_InvalidStabilityPointStructureInflowModelType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();
            const StabilityPointStructureInflowModelType invalidValue = (StabilityPointStructureInflowModelType) 983;

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(StabilityPointStructureInflowModelType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert, ConfigurationStabilityPointStructuresInflowModelType.FloodedCulvert,
            TestName = "ConvertFrom_StabilityPointStructureInflowModelType_ReturnFloodedCulvertConfigurationInflowModelType(FloodedCulvert)")]
        [TestCase(StabilityPointStructureInflowModelType.LowSill, ConfigurationStabilityPointStructuresInflowModelType.LowSill,
            TestName = "ConvertFrom_StabilityPointStructureInflowModelType_ReturnLowSillConfigurationInflowModelType(LowSill)")]
        public void ConvertFrom_StabilityPointStructureInflowModelType_ReturnExpectedConfigurationInflowModelType(StabilityPointStructureInflowModelType value,
                                                                                                                  ConfigurationStabilityPointStructuresInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresInflowModelTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}