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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.IO.Configurations;
using Riskeer.GrassCoverErosionInwards.IO.Configurations.Helpers;

namespace Riskeer.GrassCoverErosionInwards.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class ConfigurationHydraulicLoadsCalculationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_DikeHeightCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(DikeHeightCalculationType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OvertoppingRateCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertToString = converter.CanConvertTo(typeof(OvertoppingRateCalculationType));

            // Assert
            Assert.IsTrue(canConvertToString);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvert = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        [TestCase(ConfigurationHydraulicLoadsCalculationType.NoCalculation, "niet")]
        [TestCase(ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm, "norm")]
        [TestCase(ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability, "doorsnede")]
        public void ConvertTo_StringVariousCases_ReturnExpectedValues(ConfigurationHydraulicLoadsCalculationType value, string expectedResult)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(string));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(
            ConfigurationHydraulicLoadsCalculationType.NoCalculation,
            DikeHeightCalculationType.NoCalculation)]
        [TestCase(
            ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm,
            DikeHeightCalculationType.CalculateByAssessmentSectionNorm)]
        [TestCase(
            ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability,
            DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability)]
        public void ConvertTo_DikeHeightCalculationTypeVariousCases_ReturnExpectedValues(
            ConfigurationHydraulicLoadsCalculationType value, DikeHeightCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(DikeHeightCalculationType));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(
            ConfigurationHydraulicLoadsCalculationType.NoCalculation,
            OvertoppingRateCalculationType.NoCalculation)]
        [TestCase(
            ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm,
            OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm)]
        [TestCase(
            ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability,
            OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability)]
        public void ConvertTo_OvertoppingRateCalculationTypeVariousCases_ReturnExpectedValues(
            ConfigurationHydraulicLoadsCalculationType value, OvertoppingRateCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertTo(value, typeof(OvertoppingRateCalculationType));

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(DikeHeightCalculationType))]
        [TestCase(typeof(OvertoppingRateCalculationType))]
        public void ConvertTo_InvalidReadHydraulicLoadsCalculationTypeValue_ThrowInvalidEnumArgumentException(Type type)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            const ConfigurationHydraulicLoadsCalculationType invalidValue = (ConfigurationHydraulicLoadsCalculationType) 9999999;

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, type);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationHydraulicLoadsCalculationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        public void ConvertTo_Object_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(ConfigurationHydraulicLoadsCalculationType.NoCalculation, typeof(object));

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_DikeHeightCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(DikeHeightCalculationType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OvertoppingRateCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvertFromString = converter.CanConvertFrom(typeof(OvertoppingRateCalculationType));

            // Assert
            Assert.IsTrue(canConvertFromString);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            bool canConvert = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        [TestCase("niet", ConfigurationHydraulicLoadsCalculationType.NoCalculation)]
        [TestCase("norm", ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm)]
        [TestCase("doorsnede", ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability)]
        public void ConvertFrom_StringVariousCases_ReturnExpectedValue(string value, ConfigurationHydraulicLoadsCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(
            DikeHeightCalculationType.NoCalculation,
            ConfigurationHydraulicLoadsCalculationType.NoCalculation)]
        [TestCase(
            DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
            ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm)]
        [TestCase(
            DikeHeightCalculationType.CalculateByProfileSpecificRequiredProbability,
            ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability)]
        public void ConvertFrom_DikeHeightCalculationTypeVariousCases_ReturnExpectedValue(
            DikeHeightCalculationType value, ConfigurationHydraulicLoadsCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        [TestCase(
            OvertoppingRateCalculationType.NoCalculation,
            ConfigurationHydraulicLoadsCalculationType.NoCalculation)]
        [TestCase(
            OvertoppingRateCalculationType.CalculateByAssessmentSectionNorm,
            ConfigurationHydraulicLoadsCalculationType.CalculateByAssessmentSectionNorm)]
        [TestCase(
            OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
            ConfigurationHydraulicLoadsCalculationType.CalculateByProfileSpecificRequiredProbability)]
        public void ConvertFrom_OvertoppingRateCalculationTypeVariousCases_ReturnExpectedValue(
            OvertoppingRateCalculationType value, ConfigurationHydraulicLoadsCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            object result = converter.ConvertFrom(value);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void ConvertFrom_UnsupportedString_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom("<unsupported string value>");

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }

        [Test]
        public void ConvertFrom_UnsupportedDikeHeightCalculationType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const DikeHeightCalculationType invalidValue = (DikeHeightCalculationType) 9999;
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(DikeHeightCalculationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }

        [Test]
        public void ConvertFrom_UnsupportedOvertoppingRateCalculationType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const OvertoppingRateCalculationType invalidValue = (OvertoppingRateCalculationType) 9999;
            var converter = new ConfigurationHydraulicLoadsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(OvertoppingRateCalculationType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("value", parameterName);
        }
    }
}