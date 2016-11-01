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
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveConditionsOutputPropertiesTest
    {
        private const int requiredWaterLevelPropertyIndex = 0;
        private const int requiredWaveHeightPropertyIndex = 1;
        private const int requiredWavePeakPeriodPropertyIndex = 2;
        private const int requiredWaveAnglePropertyIndex = 3;

        [Test]
        public void GetProperties_ValidData_ReturnsExpectedValues()
        {
            // Setup
            const double waterLevel = 3.09378;
            const double waveHeight = 4.29884;
            const double wavePeakPeriod = 0.19435;
            const double waveAngle = 180.62353;

            // Call
            var properties = new WaveConditionsOutputProperties
            {
                Data = new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle)
            };

            // Assert
            Assert.AreEqual(waterLevel, properties.WaterLevel, properties.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, properties.WaveHeight, properties.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeakPeriod, properties.WavePeakPeriod, properties.WavePeakPeriod.GetAccuracy());
            Assert.AreEqual(waveAngle, properties.WaveAngle, properties.WaveAngle.GetAccuracy());
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            const double waterLevel = 3.09378;
            const double waveHeight = 4.29884;
            const double wavePeakPeriod = 0.19435;
            const double waveAngle = 180.62353;

            const string expectedCategory = "Algemeen";
            const string expectedWaterLevelDisplayName = "Waterstand [m+NAP]";
            const string expectedWaveHeightDisplayName = "Golfhoogte (Hs) [m]";
            const string expectedWavePeakPeriodDisplayName = "Golfperiode (Tp) [s]";
            const string expectedWaveAngleDisplayName = "Golfrichting [°]";

            const string expectedWaterLevelDescription = "De waterstand waarvoor de golfhoogte, -periode en -richting zijn berekend.";
            const string expectedWaveHeightDescription = "Berekende golfhoogte.";
            const string expectedWavePeakPeriodDescription = "Berekende golfperiode.";
            const string expectedWaveAngleDescription = "Berekende golfrichting ten opzichte van dijknormaal.";

            // Call
            var properties = new WaveConditionsOutputProperties
            {
                Data = new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle)
            };

            // Assert
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            var propertyBag = new DynamicPropertyBag(properties);

            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(4, dynamicProperties.Count);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            PropertyDescriptor waterLevelProperty = dynamicProperties[requiredWaterLevelPropertyIndex];
            Assert.IsNotNull(waterLevelProperty);
            Assert.IsTrue(waterLevelProperty.IsReadOnly);
            Assert.AreEqual(expectedCategory, waterLevelProperty.Category);
            Assert.AreEqual(expectedWaterLevelDisplayName, waterLevelProperty.DisplayName);
            Assert.AreEqual(expectedWaterLevelDescription, waterLevelProperty.Description);

            PropertyDescriptor waveHeightProperty = dynamicProperties[requiredWaveHeightPropertyIndex];
            Assert.IsNotNull(waveHeightProperty);
            Assert.IsTrue(waveHeightProperty.IsReadOnly);
            Assert.AreEqual(expectedCategory, waveHeightProperty.Category);
            Assert.AreEqual(expectedWaveHeightDisplayName, waveHeightProperty.DisplayName);
            Assert.AreEqual(expectedWaveHeightDescription, waveHeightProperty.Description);

            PropertyDescriptor wavePeakPeriodProperty = dynamicProperties[requiredWavePeakPeriodPropertyIndex];
            Assert.IsNotNull(wavePeakPeriodProperty);
            Assert.IsTrue(wavePeakPeriodProperty.IsReadOnly);
            Assert.AreEqual(expectedCategory, wavePeakPeriodProperty.Category);
            Assert.AreEqual(expectedWavePeakPeriodDisplayName, wavePeakPeriodProperty.DisplayName);
            Assert.AreEqual(expectedWavePeakPeriodDescription, wavePeakPeriodProperty.Description);

            PropertyDescriptor waveAngleProperty = dynamicProperties[requiredWaveAnglePropertyIndex];
            Assert.IsNotNull(waveAngleProperty);
            Assert.IsTrue(waveAngleProperty.IsReadOnly);
            Assert.AreEqual(expectedCategory, waveAngleProperty.Category);
            Assert.AreEqual(expectedWaveAngleDisplayName, waveAngleProperty.DisplayName);
            Assert.AreEqual(expectedWaveAngleDescription, waveAngleProperty.Description);
        }
    }
}