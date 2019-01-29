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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.IO.Configurations;
using Riskeer.ClosingStructures.IO.Configurations.Helpers;

namespace Riskeer.ClosingStructures.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationClosingStructureInflowModelTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_ClosingStructureInflowModelType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(ClosingStructureInflowModelType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherThanStringOrClosingStructureInflowModelType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToString);
        }

        [Test]
        [TestCase(ConfigurationClosingStructureInflowModelType.FloodedCulvert, ClosingStructuresConfigurationSchemaIdentifiers.FloodedCulvert)]
        [TestCase(ConfigurationClosingStructureInflowModelType.LowSill, ClosingStructuresConfigurationSchemaIdentifiers.LowSill)]
        [TestCase(ConfigurationClosingStructureInflowModelType.VerticalWall, ClosingStructuresConfigurationSchemaIdentifiers.VerticalWall)]
        public void ConvertTo_VariousCases_ReturnExpectedText(ConfigurationClosingStructureInflowModelType value,
                                                              string expectedResult)
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(string));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(typeof(ClosingStructureInflowModelType))]
        [TestCase(typeof(string))]
        public void ConvertTo_InvalidClosingStructureInflowModelType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();
            const ConfigurationClosingStructureInflowModelType invalidValue = (ConfigurationClosingStructureInflowModelType) 99999999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationClosingStructureInflowModelType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        public void ConvertTo_InvalidDestinationType_ThrowsNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(random.NextEnumValue<ConfigurationClosingStructureInflowModelType>(), typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(ConfigurationClosingStructureInflowModelType.FloodedCulvert, ClosingStructureInflowModelType.FloodedCulvert)]
        [TestCase(ConfigurationClosingStructureInflowModelType.LowSill, ClosingStructureInflowModelType.LowSill)]
        [TestCase(ConfigurationClosingStructureInflowModelType.VerticalWall, ClosingStructureInflowModelType.VerticalWall)]
        public void ConvertTo_VariousCases_ReturnExpectedText(ConfigurationClosingStructureInflowModelType value,
                                                              ClosingStructureInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(ClosingStructureInflowModelType));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_ClosingStructureInflowModelType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(ClosingStructureInflowModelType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanStringOrClosingStructureInflowModelType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(ClosingStructuresConfigurationSchemaIdentifiers.FloodedCulvert, ConfigurationClosingStructureInflowModelType.FloodedCulvert)]
        [TestCase(ClosingStructuresConfigurationSchemaIdentifiers.LowSill, ConfigurationClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructuresConfigurationSchemaIdentifiers.VerticalWall, ConfigurationClosingStructureInflowModelType.VerticalWall)]
        public void ConvertFrom_Text_ReturnExpectedClosingStructureInflowModelType(string value,
                                                                                   ConfigurationClosingStructureInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("A");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(ClosingStructureInflowModelType.FloodedCulvert, ConfigurationClosingStructureInflowModelType.FloodedCulvert)]
        [TestCase(ClosingStructureInflowModelType.LowSill, ConfigurationClosingStructureInflowModelType.LowSill)]
        [TestCase(ClosingStructureInflowModelType.VerticalWall, ConfigurationClosingStructureInflowModelType.VerticalWall)]
        public void ConvertFrom_ClosingStructureInflowModelType_ReturnExpectedClosingStructureInflowModelType(ClosingStructureInflowModelType value,
                                                                                                              ConfigurationClosingStructureInflowModelType expectedResult)
        {
            // Setup
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidClosingStructureInflowModelType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = -1;
            var converter = new ConfigurationClosingStructureInflowModelTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom((ClosingStructureInflowModelType) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ClosingStructureInflowModelType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }
    }
}