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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Configurations;
using Riskeer.MacroStabilityInwards.IO.Configurations.Helpers;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationDikeSoilScenarioTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

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
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(supportedType);

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }

        [Test]
        [TestCaseSource(nameof(SupportedConvertions), new object[]
        {
            "ConvertTo_{1}_ReturnsExpectedResult"
        })]
        public void ConvertTo_VariousCases_ReturnExpectedResult(ConfigurationDikeSoilScenario value,
                                                                object expectedResult)
        {
            // Setup
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();
            Type convertToType = expectedResult.GetType();

            // Call
            object result = converter.ConvertTo(value, convertToType);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(MacroStabilityInwardsDikeSoilScenario))]
        public void ConvertTo_InvalidConfigurationDikeSoilScenario_ThrowsInvalidEnumArgumentException(Type convertToType)
        {
            // Setup
            const ConfigurationDikeSoilScenario invalidValue = (ConfigurationDikeSoilScenario) 9999;
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, convertToType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationDikeSoilScenario)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        public void ConvertTo_InvalidDestinationType_ThrowsNotSupportedException()
        {
            // Setup
            var random = new Random(21);
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(random.NextEnumValue<ConfigurationDikeSoilScenario>(), typeof(object));

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
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(supportedType);

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        [TestCaseSource(nameof(SupportedConvertions), new object[]
        {
            "ConvertFrom_{1}_ReturnExpectedConfigurationDikeSoilScenario"
        })]
        public void ConvertFrom_VariousCases_ReturnExpectedConfigurationDikeSoilScenario(ConfigurationDikeSoilScenario expectedResult,
                                                                                         object value)
        {
            // Setup
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

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
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

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
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_InvalidMacroStabilityInwardsDikeSoilScenario_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const MacroStabilityInwardsDikeSoilScenario invalidValue = (MacroStabilityInwardsDikeSoilScenario) 9999;
            var converter = new ConfigurationDikeSoilScenarioTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(MacroStabilityInwardsDikeSoilScenario)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        private static IEnumerable<TestCaseData> GetSupportedTypes(string testName)
        {
            yield return new TestCaseData(typeof(string))
                .SetName(testName);
            yield return new TestCaseData(typeof(MacroStabilityInwardsDikeSoilScenario))
                .SetName(testName);
        }

        private static IEnumerable<TestCaseData> SupportedConvertions(string testName)
        {
            yield return new TestCaseData(ConfigurationDikeSoilScenario.ClayDikeOnClay,
                                          MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnClay)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationDikeSoilScenario.SandDikeOnClay,
                                          MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnClay)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationDikeSoilScenario.ClayDikeOnSand,
                                          MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioClayDikeOnSand)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationDikeSoilScenario.SandDikeOnSand,
                                          MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers.DikeSoilScenarioSandDikeOnSand)
                .SetName(testName);

            yield return new TestCaseData(ConfigurationDikeSoilScenario.ClayDikeOnClay,
                                          MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationDikeSoilScenario.SandDikeOnClay,
                                          MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationDikeSoilScenario.ClayDikeOnSand,
                                          MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand)
                .SetName(testName);
            yield return new TestCaseData(ConfigurationDikeSoilScenario.SandDikeOnSand,
                                          MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand)
                .SetName(testName);
        }
    }
}