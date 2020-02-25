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
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.IO.Configurations;
using Riskeer.GrassCoverErosionOutwards.IO.Configurations.Converters;

namespace Riskeer.GrassCoverErosionOutwards.IO.Test.Configurations.Converters
{
    [TestFixture]
    public class ConfigurationGrassCoverErosionOutwardsCalculationTypeConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Assert
            Assert.IsInstanceOf<TypeConverter>(converter);
        }

        [Test]
        public void CanConvertTo_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(string));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_GrassCoverErosionOutwardsWaveConditionsCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType));

            // Assert
            Assert.IsTrue(canConvertTo);
        }

        [Test]
        public void CanConvertTo_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            bool canConvertTo = converter.CanConvertTo(typeof(object));

            // Assert
            Assert.IsFalse(canConvertTo);
        }

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType))]
        public void ConvertTo_InvalidConfigurationGrassCoverErosionOutwardsCalculationType_ThrowInvalidEnumArgumentException(Type destinationType)
        {
            // Setup
            const GrassCoverErosionOutwardsWaveConditionsCalculationType invalidValue = (GrassCoverErosionOutwardsWaveConditionsCalculationType) 99;
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertTo(invalidValue, destinationType);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(ConfigurationGrassCoverErosionOutwardsCalculationType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact, GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact)]
        [TestCase(ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp, GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp)]
        [TestCase(ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact, GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact)]
        public void ConvertTo_ValidConfigurationGrassCoverErosionOutwardsCalculationType_ReturnGrassCoverErosionOutwardsWaveConditionsCalculationType(
            ConfigurationGrassCoverErosionOutwardsCalculationType originalValue, GrassCoverErosionOutwardsWaveConditionsCalculationType expectedResult)
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            object calculationType = converter.ConvertTo(null, CultureInfo.CurrentCulture, originalValue, typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType));

            // Assert
            Assert.AreEqual(expectedResult, calculationType);
        }

        [Test]
        [TestCase(ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact, "Gras (golfoploop en golfklap)")]
        [TestCase(ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp, "Gras (golfoploop)")]
        [TestCase(ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact, "Gras (golfklap)")]
        public void ConvertTo_ValidConfigurationGrassCoverErosionOutwardsCalculationType_ReturnExpectedText(
            ConfigurationGrassCoverErosionOutwardsCalculationType originalValue, string expectedText)
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            object calculationType = converter.ConvertTo(null, CultureInfo.CurrentCulture, originalValue, typeof(string));

            // Assert
            Assert.AreEqual(expectedText, calculationType);
        }

        [Test]
        public void CanConvertFrom_String_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(string));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_GrassCoverErosionOutwardsWaveConditionsCalculationType_ReturnTrue()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationType));

            // Assert
            Assert.IsTrue(canConvertFrom);
        }

        [Test]
        public void CanConvertFrom_OtherType_ReturnFalse()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            bool canConvertFrom = converter.CanConvertFrom(typeof(object));

            // Assert
            Assert.IsFalse(canConvertFrom);
        }

        [Test]
        public void ConvertFrom_InvalidGrassCoverErosionOutwardsWaveConditionsCalculationType_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const GrassCoverErosionOutwardsWaveConditionsCalculationType invalidValue = (GrassCoverErosionOutwardsWaveConditionsCalculationType) 99;
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'value' ({invalidValue}) is invalid for Enum type '{nameof(GrassCoverErosionOutwardsWaveConditionsCalculationType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase("Gras (golfoploop en golfklap)", ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact)]
        [TestCase("Gras (golfoploop)", ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp)]
        [TestCase("Gras (golfklap)", ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact)]
        [TestCase("Gras (golfklap toets op maat)", ConfigurationGrassCoverErosionOutwardsCalculationType.TailorMadeWaveImpact)]
        [TestCase("Gras (golfoploop en golfklap toets op maat)", ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndTailorMadeWaveImpact)]
        [TestCase("Gras (golfoploop, golfklap en golfklap op maat)", ConfigurationGrassCoverErosionOutwardsCalculationType.All)]
        public void ConvertFrom_ValidStringValue_ReturnConfigurationGrassCoverErosionOutwardsCalculationType(
            string originalValue, ConfigurationGrassCoverErosionOutwardsCalculationType expectedValue)
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            object convertedValue = converter.ConvertFrom(originalValue);

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUpAndWaveImpact, ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUpAndWaveImpact)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveRunUp, ConfigurationGrassCoverErosionOutwardsCalculationType.WaveRunUp)]
        [TestCase(GrassCoverErosionOutwardsWaveConditionsCalculationType.WaveImpact, ConfigurationGrassCoverErosionOutwardsCalculationType.WaveImpact)]
        public void ConvertFrom_ValidGrassCoverErosionOutwardsWaveConditionsCalculationType_ReturnConfigurationGrassCoverErosionOutwardsCalculationType(
            GrassCoverErosionOutwardsWaveConditionsCalculationType originalValue, ConfigurationGrassCoverErosionOutwardsCalculationType expectedValue)
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            object convertedValue = converter.ConvertFrom(originalValue);

            // Assert
            Assert.AreEqual(expectedValue, convertedValue);
        }

        [Test]
        public void ConvertFrom_Null_ThrowNotSupportedException()
        {
            // Setup
            var converter = new ConfigurationGrassCoverErosionOutwardsCalculationTypeConverter();

            // Call
            TestDelegate call = () => converter.ConvertFrom(null);

            // Assert
            Assert.Throws<NotSupportedException>(call);
        }
    }
}