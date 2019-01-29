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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Riskeer.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveConditionsOutputPropertiesTest
    {
        private const int requiredWaterLevelPropertyIndex = 0;
        private const int requiredWaveHeightPropertyIndex = 1;
        private const int requiredWavePeakPeriodPropertyIndex = 2;
        private const int requiredWaveDirectionPropertyIndex = 3;
        private const int requiredWaveAnglePropertyIndex = 4;
        private const int requiredTargetProbabilityPropertyIndex = 5;
        private const int requiredTargetReliabilityPropertyIndex = 6;
        private const int requiredCalculatedProbabilityPropertyIndex = 7;
        private const int requiredCalculatedReliabilityPropertyIndex = 8;
        private const int requiredConvergencePropertyIndex = 9;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new WaveConditionsOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WaveConditionsOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_ValuesNaN_ReturnsExpectedValues()
        {
            // Setup
            var convergence = new Random().NextEnumValue<CalculationConvergence>();

            // Call
            var properties = new WaveConditionsOutputProperties
            {
                Data = new WaveConditionsOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN,
                                                double.NaN, double.NaN, double.NaN, convergence)
            };

            // Assert
            Assert.IsNaN(properties.WaterLevel);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(WaveConditionsOutputProperties.WaterLevel));

            Assert.IsNaN(properties.WaveHeight);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(WaveConditionsOutputProperties.WaveHeight));

            Assert.IsNaN(properties.WavePeakPeriod);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(WaveConditionsOutputProperties.WavePeakPeriod));

            Assert.IsNaN(properties.WaveDirection);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(WaveConditionsOutputProperties.WaveDirection));

            Assert.IsNaN(properties.WaveAngle);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(WaveConditionsOutputProperties.WaveAngle));

            Assert.IsNaN(properties.TargetProbability);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(WaveConditionsOutputProperties.TargetProbability));

            Assert.IsNaN(properties.TargetReliability);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>
                (nameof(WaveConditionsOutputProperties.TargetReliability));

            Assert.IsNaN(properties.CalculatedProbability);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoProbabilityValueDoubleConverter>(
                nameof(WaveConditionsOutputProperties.CalculatedProbability));

            Assert.IsNaN(properties.CalculatedReliability);
            TestHelper.AssertTypeConverter<WaveConditionsOutputProperties, NoValueRoundedDoubleConverter>(
                nameof(WaveConditionsOutputProperties.CalculatedReliability));

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(convergence).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);
        }

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const double waterLevel = 3.09378;
            const double waveHeight = 4.29884;
            const double wavePeakPeriod = 0.19435;
            const double waveAngle = 180.62353;
            const double waveDirection = 230.5326;
            const double targetProbability = 0.5;
            const double targetReliability = 3000;
            const double calculatedProbability = 0.4;
            const double calculatedReliability = 6000;
            var convergence = new Random().NextEnumValue<CalculationConvergence>();

            // Call
            var properties = new WaveConditionsOutputProperties
            {
                Data = new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle, waveDirection, targetProbability,
                                                targetReliability, calculatedProbability, calculatedReliability, convergence)
            };

            // Assert
            Assert.AreEqual(waterLevel, properties.WaterLevel, properties.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, properties.WavePeakPeriod, properties.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveDirection, properties.WaveDirection, properties.WaveDirection.GetAccuracy());
            Assert.AreEqual(waveAngle, properties.WaveAngle, properties.WaveAngle.GetAccuracy());
            Assert.AreEqual(targetProbability, properties.TargetProbability);
            Assert.AreEqual(targetReliability, properties.TargetReliability, properties.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, properties.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, properties.CalculatedReliability, properties.CalculatedReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(convergence).DisplayName;
            Assert.AreEqual(convergenceValue, properties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            const double waterLevel = 3.09378;
            const double waveHeight = 4.29884;
            const double wavePeakPeriod = 0.19435;
            const double waveAngle = 180.62353;
            const double waveDirection = 230.5326;
            const double targetProbability = 0.5;
            const double targetReliability = 3000;
            const double calculatedProbability = 0.4;
            const double calculatedReliability = 6000;

            // Call
            var properties = new WaveConditionsOutputProperties
            {
                Data = new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle, waveDirection, targetProbability,
                                                targetReliability, calculatedProbability, calculatedReliability, CalculationConvergence.NotCalculated)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(10, dynamicProperties.Count);

            PropertyDescriptor waterLevelProperty = dynamicProperties[requiredWaterLevelPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waterLevelProperty,
                                                                            "Algemeen",
                                                                            "Waterstand [m+NAP]",
                                                                            "De waterstand waarvoor de golfhoogte, -periode en -richting zijn berekend.",
                                                                            true);

            PropertyDescriptor waveHeightProperty = dynamicProperties[requiredWaveHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveHeightProperty,
                                                                            "Algemeen",
                                                                            "Golfhoogte (Hs) [m]",
                                                                            "Berekende golfhoogte.",
                                                                            true);

            PropertyDescriptor wavePeakPeriodProperty = dynamicProperties[requiredWavePeakPeriodPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(wavePeakPeriodProperty,
                                                                            "Algemeen",
                                                                            "Golfperiode (Tp) [s]",
                                                                            "Berekende golfperiode.",
                                                                            true);
            PropertyDescriptor waveDirectionProperty = dynamicProperties[requiredWaveDirectionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveDirectionProperty,
                                                                            "Algemeen",
                                                                            "Golfrichting t.o.v. Noord [°]",
                                                                            "Berekende maatgevende golfrichting ten opzichte van het noorden.",
                                                                            true);

            PropertyDescriptor waveAngleProperty = dynamicProperties[requiredWaveAnglePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waveAngleProperty,
                                                                            "Algemeen",
                                                                            "Golfrichting t.o.v. dijknormaal [°]",
                                                                            "Berekende maatgevende golfrichting ten opzichte van de dijknormaal.",
                                                                            true);

            PropertyDescriptor targetProbabilityProperty = dynamicProperties[requiredTargetProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetProbabilityProperty,
                                                                            "Algemeen",
                                                                            "Doelkans [1/jaar]",
                                                                            "De opgegeven kans waarvoor het resultaat is berekend.",
                                                                            true);

            PropertyDescriptor targetReliabilityProperty = dynamicProperties[requiredTargetReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(targetReliabilityProperty,
                                                                            "Algemeen",
                                                                            "Betrouwbaarheidsindex doelkans [-]",
                                                                            "Betrouwbaarheidsindex van de opgegeven kans waarvoor het resultaat is berekend.",
                                                                            true);

            PropertyDescriptor calculatedProbabilityProperty = dynamicProperties[requiredCalculatedProbabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedProbabilityProperty,
                                                                            "Algemeen",
                                                                            "Berekende kans [1/jaar]",
                                                                            "De berekende kans van voorkomen van het resultaat.",
                                                                            true);

            PropertyDescriptor calculatedReliabilityProperty = dynamicProperties[requiredCalculatedReliabilityPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculatedReliabilityProperty,
                                                                            "Algemeen",
                                                                            "Betrouwbaarheidsindex berekende kans [-]",
                                                                            "Betrouwbaarheidsindex van de berekende kans van voorkomen van het resultaat.",
                                                                            true);

            PropertyDescriptor calculationConvergenceProperty = dynamicProperties[requiredConvergencePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(calculationConvergenceProperty,
                                                                            "Algemeen",
                                                                            "Convergentie",
                                                                            "Is convergentie bereikt voor de berekening?",
                                                                            true);
        }
    }
}