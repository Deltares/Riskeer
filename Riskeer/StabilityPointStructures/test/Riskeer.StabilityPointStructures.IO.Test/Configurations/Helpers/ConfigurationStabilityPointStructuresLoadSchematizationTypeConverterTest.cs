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
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.IO.Configurations;
using Ringtoets.StabilityPointStructures.IO.Configurations.Helpers;

namespace Riskeer.StabilityPointStructures.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationStabilityPointStructuresConfigurationStabilityPointStructuresLoadSchematizationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_LoadSchematizationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(LoadSchematizationType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherTypeThanStringOrLoadSchematizationType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            bool canConvertToNonString = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertToNonString);
        }

        [Test]
        [TestCase(ConfigurationStabilityPointStructuresLoadSchematizationType.Linear, StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationLinearStructure,
            TestName = "ConvertTo_ForLinearConfigurationConfigurationStabilityPointStructuresLoadSchematizationType_ReturnExpectedText(Linear)")]
        [TestCase(ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic, StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationQuadraticStructure,
            TestName = "ConvertTo_ForQuadraticConfigurationConfigurationStabilityPointStructuresLoadSchematizationType_ReturnExpectedText(Quadratic)")]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedText(ConfigurationStabilityPointStructuresLoadSchematizationType value,
                                                                  string expectedText)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

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
            var invalidValue = random.NextEnumValue<ConfigurationStabilityPointStructuresLoadSchematizationType>();
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(LoadSchematizationType))]
        public void ConvertTo_ConfigurationStabilityPointStructuresLoadSchematizationType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();
            const ConfigurationStabilityPointStructuresLoadSchematizationType invalidValue = (ConfigurationStabilityPointStructuresLoadSchematizationType) 99999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type " +
                                     $"'{nameof(ConfigurationStabilityPointStructuresLoadSchematizationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        [TestCase(ConfigurationStabilityPointStructuresLoadSchematizationType.Linear, LoadSchematizationType.Linear,
            TestName = "ConvertTo_LinearConfigurationStabilityPointStructuresLoadSchematizationType_ReturnLinearLoadSchematizationType")]
        [TestCase(ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic, LoadSchematizationType.Quadratic,
            TestName = "ConvertTo_QuadraticConfigurationStabilityPointStructuresLoadSchematizationType_ReturnQuadraticLoadSchematizationType")]
        public void ConvertTo_ForAllEnumValues_ReturnExpectedType(ConfigurationStabilityPointStructuresLoadSchematizationType value,
                                                                  LoadSchematizationType expectedText)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            object result = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(LoadSchematizationType));

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_LoadSchematizationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(LoadSchematizationType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherThanStringOrLoadSchematizationType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFromString);
        }

        [Test]
        [TestCase(StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationLinearStructure, ConfigurationStabilityPointStructuresLoadSchematizationType.Linear,
            TestName = "ConvertFrom_LinearText_ReturnLinearConfigurationStabilityPointStructuresLoadSchematizationType")]
        [TestCase(StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationQuadraticStructure, ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic,
            TestName = "ConvertFrom_QuadraticText_ReturnQuadraticConfigurationStabilityPointStructuresLoadSchematizationType")]
        public void ConvertFrom_Text_ReturnExpectedConfigurationStabilityPointStructuresLoadSchematizationType(string value,
                                                                                                               ConfigurationStabilityPointStructuresLoadSchematizationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_InvalidType_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(new object());

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();
            const string invalidValue = "some text";

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string message = Assert.Throws<NotSupportedException>(call).Message;
            Assert.AreEqual($"Value '{invalidValue}' is not supported.", message);
        }

        [Test]
        public void ConvertFrom_InvalidLoadSchematizationType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();
            const LoadSchematizationType invalidValue = (LoadSchematizationType) 983;

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(LoadSchematizationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        [TestCase(LoadSchematizationType.Linear, ConfigurationStabilityPointStructuresLoadSchematizationType.Linear,
            TestName = "ConvertFrom_ForLinearLoadSchematizationType_ReturnLinearConfigurationStabilityPointStructuresLoadSchematizationType")]
        [TestCase(LoadSchematizationType.Quadratic, ConfigurationStabilityPointStructuresLoadSchematizationType.Quadratic,
            TestName = "ConvertFrom_ForQuadraticLoadSchematizationType_ReturnQuadraticConfigurationStabilityPointStructuresLoadSchematizationType")]
        public void ConvertFrom_StabilityPointStructureInflowModelType_ReturnExpectedConfigurationInflowModelType(
            LoadSchematizationType value,
            ConfigurationStabilityPointStructuresLoadSchematizationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}