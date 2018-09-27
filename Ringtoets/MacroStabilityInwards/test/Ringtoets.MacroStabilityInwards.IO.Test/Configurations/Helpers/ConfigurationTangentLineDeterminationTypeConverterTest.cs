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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.IO.Configurations.Helpers;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationTangentLineDeterminationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        [TestCaseSource(nameof(GetSupportedTypes), new object[]
        {
            "CanConvertTo_{0}_ReturnTrue"
        })]
        public void CanConvertTo_SupportedType_ReturnTrue(Type supportedType)
        {
            // Setup
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(supportedType);

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            bool convertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(convertTo);
        }

        [Test]
        [TestCaseSource(nameof(SupportedConvertions), new object[]
        {
            "ConvertTo_{1}_ReturnsExpectedResult"
        })]
        public void ConvertTo_VariousCases_ReturnExpectedResult(ConfigurationTangentLineDeterminationType value,
                                                                object expectedResult)
        {
            // Setup
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();
            Type convertToType = expectedResult.GetType();

            // Call
            object result = converter.ConvertTo(value, convertToType);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(MacroStabilityInwardsTangentLineDeterminationType))]
        public void ConvertTo_InvalidConfigurationTangentLineDeterminationType_ThrowsInvalidEnumArgumentException(Type convertToType)
        {
            // Setup
            const ConfigurationTangentLineDeterminationType invalidValue = (ConfigurationTangentLineDeterminationType) 9999;
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, convertToType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationTangentLineDeterminationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        public void ConvertTo_InvalidDestinationType_ThrowsNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(random.NextEnumValue<ConfigurationTangentLineDeterminationType>(), typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCaseSource(nameof(GetSupportedTypes), new object[]
        {
            "CanConvertFrom_{0}_ReturnTrue"
        })]
        public void CanConvertFrom_SupportedType_ReturnTrue(Type supportedType)
        {
            // Setup
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(supportedType);

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        [TestCaseSource(nameof(SupportedConvertions), new object[]
        {
            "ConvertFrom_{1}_ReturnExpectedConfigurationTangentLineDeterminationType"
        })]
        public void ConvertFrom_VariousCases_ReturnExpectedConfigurationTangentLineDeterminationType(ConfigurationTangentLineDeterminationType expectedResult,
                                                                                                     object value)
        {
            // Setup
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFom_InvalidStringValue_ThrowsNotSupportedException()
        {
            // Setup
            const string invalidValue = "invalid value";
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_InvalidValueType_ThrowsNotSupportedException()
        {
            // Setup
            var invalidValue = new object();
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_InvalidMacroStabilityInwardsTangentLineDeterminationType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const MacroStabilityInwardsTangentLineDeterminationType invalidValue = (MacroStabilityInwardsTangentLineDeterminationType) 9999;
            var converter = new ConfigurationTangentLineDeterminationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(MacroStabilityInwardsTangentLineDeterminationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        private static IEnumerable<TestCaseData> GetSupportedTypes(string testName)
        {
            yield return new TestCaseData(typeof(string))
                .SetName(testName);
            yield return new TestCaseData(typeof(MacroStabilityInwardsTangentLineDeterminationType))
                .SetName(testName);
        }

        private static IEnumerable<TestCaseData> SupportedConvertions(string testName)
        {
            yield return new TestCaseData(ConfigurationTangentLineDeterminationType.LayerSeparated,
                                          MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeLayerSeparated)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationTangentLineDeterminationType.Specified,
                                          MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.TangentLineDeterminationTypeSpecified)
                .SetName(testName);

            yield return new TestCaseData(ConfigurationTangentLineDeterminationType.LayerSeparated,
                                          MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationTangentLineDeterminationType.Specified,
                                          MacroStabilityInwardsTangentLineDeterminationType.Specified)
                .SetName(testName);
        }
    }
}