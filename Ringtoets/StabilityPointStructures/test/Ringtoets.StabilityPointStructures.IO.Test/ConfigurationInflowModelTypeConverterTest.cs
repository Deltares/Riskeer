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
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.IO.Test
{
    [TestFixture]
    public class ConfigurationInflowModelTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationInflowModelTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_StabilityPointStructureInflowModelType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(StabilityPointStructureInflowModelType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherTypeThanStringOrStabilityPointStructureInflowModelType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            bool canConvertToNonString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToNonString);
        }

        [Test]
        [TestCase(ConfigurationInflowModelType.LowSill, StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure)]
        [TestCase(ConfigurationInflowModelType.FloodedCulvert, StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure)]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedText(ConfigurationInflowModelType value,
                                                                  string expectedText)
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void ConvertTo_InvalidType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();
            const ConfigurationInflowModelType invalidValue = (ConfigurationInflowModelType) 99999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertTo_ConfigurationInflowModelTypeToString_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();
            const ConfigurationInflowModelType invalidValue = (ConfigurationInflowModelType) 99999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(string));

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Value '{invalidValue}' is not supported.", message);
        }

        [Test]
        public void ConvertTo_InvalidConfigurationInflowModelTypeToStabilityPointStructureInflowModelType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();
            const ConfigurationInflowModelType invalidValue = (ConfigurationInflowModelType) 99999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(StabilityPointStructureInflowModelType));

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Value '{invalidValue}' is not supported.", message);
        }

        [Test]
        [TestCase(ConfigurationInflowModelType.LowSill, StabilityPointStructureInflowModelType.LowSill)]
        [TestCase(ConfigurationInflowModelType.FloodedCulvert, StabilityPointStructureInflowModelType.FloodedCulvert)]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedType(ConfigurationInflowModelType value,
                                                                  StabilityPointStructureInflowModelType expectedText)
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(StabilityPointStructureInflowModelType));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_StabilityPointStructureInflowModelType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(StabilityPointStructureInflowModelType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanStringOrStabilityPointStructureInflowModelType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelLowSillStructure, ConfigurationInflowModelType.LowSill)]
        [TestCase(StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelFloodedCulvertStructure, ConfigurationInflowModelType.FloodedCulvert)]
        public void ConvertFrom_Text_ReturnExpectedConfigurationInflowModelType(string value,
                                                                                ConfigurationInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(new object());

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();
            const string invalidValue = "some text";

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Value '{invalidValue}' is not supported.", message);
        }

        [Test]
        public void ConvertFrom_InvalidStabilityPointStructureInflowModelType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();
            const StabilityPointStructureInflowModelType invalidValue = (StabilityPointStructureInflowModelType) 983;

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Value '{invalidValue}' is not supported.", message);
        }

        [Test]
        [TestCase(StabilityPointStructureInflowModelType.FloodedCulvert, ConfigurationInflowModelType.FloodedCulvert)]
        [TestCase(StabilityPointStructureInflowModelType.LowSill, ConfigurationInflowModelType.LowSill)]
        public void ConvertFrom_StabilityPointStructureInflowModelType_ReturnExpectedConfigurationInflowModelType(StabilityPointStructureInflowModelType value,
                                                                                                                  ConfigurationInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationInflowModelTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}