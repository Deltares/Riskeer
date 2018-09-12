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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.Revetment.IO.Configurations.Converters;

namespace Ringtoets.Revetment.IO.Test.Configurations.Converters
{
    [TestFixture]
    public class ConfigurationAssessmentSectionCategoryTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_AssessmentSectionCategoryType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(AssessmentSectionCategoryType));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }

        [Test]
        [TestCase(ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm, "Categoriegrens A+")]
        [TestCase(ConfigurationAssessmentSectionCategoryType.SignalingNorm, "Categoriegrens A")]
        [TestCase(ConfigurationAssessmentSectionCategoryType.LowerLimitNorm, "Categoriegrens B")]
        [TestCase(ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm, "Categoriegrens C")]
        public void ConvertTo_ValidConfigurationAssessmentSectionCategoryType_ReturnExpectedText(
            ConfigurationAssessmentSectionCategoryType value, string expectedText)
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            object convertTo = converter.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, convertTo);
        }

        [Test]
        [TestCase(ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm, AssessmentSectionCategoryType.FactorizedSignalingNorm)]
        [TestCase(ConfigurationAssessmentSectionCategoryType.SignalingNorm, AssessmentSectionCategoryType.SignalingNorm)]
        [TestCase(ConfigurationAssessmentSectionCategoryType.LowerLimitNorm, AssessmentSectionCategoryType.LowerLimitNorm)]
        [TestCase(ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm, AssessmentSectionCategoryType.FactorizedLowerLimitNorm)]
        public void ConvertTo_ValidConfigurationAssessmentSectionCategoryType_ReturnAssessmentSectionCategoryType(
            ConfigurationAssessmentSectionCategoryType originalValue, AssessmentSectionCategoryType expectedResult)
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            object categoryType = converter.ConvertTo(null, CultureInfo.CurrentCulture, originalValue, typeof(AssessmentSectionCategoryType));

            // Assert
            Assert.AreEqual(expectedResult, categoryType);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(AssessmentSectionCategoryType))]
        public void ConvertTo_InvalidConfigurationAssessmentSectionCategoryType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            const ConfigurationAssessmentSectionCategoryType invalidValue = (ConfigurationAssessmentSectionCategoryType) 99;
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationAssessmentSectionCategoryType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_AssessmentSectionCategoryType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(AssessmentSectionCategoryType));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        [TestCase("Categoriegrens A+", ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm)]
        [TestCase("Categoriegrens A", ConfigurationAssessmentSectionCategoryType.SignalingNorm)]
        [TestCase("Categoriegrens B", ConfigurationAssessmentSectionCategoryType.LowerLimitNorm)]
        [TestCase("Categoriegrens C", ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm)]
        public void ConvertFrom_ValidStringValue_ReturnConfigurationAssessmentSectionCategoryType(
            string value, ConfigurationAssessmentSectionCategoryType expectedResult)
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            object convertFrom = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, convertFrom);
        }

        [Test]
        public void ConvertFrom_InvalidText_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("1x");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        [TestCase(AssessmentSectionCategoryType.FactorizedSignalingNorm, ConfigurationAssessmentSectionCategoryType.FactorizedSignalingNorm)]
        [TestCase(AssessmentSectionCategoryType.SignalingNorm, ConfigurationAssessmentSectionCategoryType.SignalingNorm)]
        [TestCase(AssessmentSectionCategoryType.LowerLimitNorm, ConfigurationAssessmentSectionCategoryType.LowerLimitNorm)]
        [TestCase(AssessmentSectionCategoryType.FactorizedLowerLimitNorm, ConfigurationAssessmentSectionCategoryType.FactorizedLowerLimitNorm)]
        public void ConvertFrom_ValidAssessmentSectionCategoryType_ReturnConfigurationAssessmentSectionCategoryType(
            AssessmentSectionCategoryType originalValue, ConfigurationAssessmentSectionCategoryType expectedResult)
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            object categoryType = converter.ConvertFrom(originalValue);

            // Assert
            Assert.AreEqual(expectedResult, categoryType);
        }

        [Test]
        public void ConvertFrom_InvalidAssessmentSectionCategoryType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const AssessmentSectionCategoryType invalidValue = (AssessmentSectionCategoryType) 99;
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(AssessmentSectionCategoryType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ConvertFrom_Null_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationAssessmentSectionCategoryTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}