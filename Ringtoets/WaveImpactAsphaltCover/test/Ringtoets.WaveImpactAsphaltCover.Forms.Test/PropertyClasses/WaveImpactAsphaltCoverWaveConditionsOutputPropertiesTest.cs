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
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PropertyClasses;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsOutputPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new WaveImpactAsphaltCoverWaveConditionsOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WaveImpactAsphaltCoverWaveConditionsOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_WithCalculationOutput_ReturnsExpectedValues()
        {
            // Setup
            var items = new[]
            {
                new WaveConditionsOutput(6, 2, 9, 4),
            };

            var waveImpactAsphaltCoverWaveConditionsOutput = new WaveImpactAsphaltCoverWaveConditionsOutput(items);

            // Call
            var properties = new WaveImpactAsphaltCoverWaveConditionsOutputProperties()
            {
                Data = waveImpactAsphaltCoverWaveConditionsOutput
            };

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Items, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(items.Length, properties.Items.Length);

            WaveConditionsOutput expectedOutputProperty = items[0];
            WaveConditionsOutputProperties firstOutputProperties = properties.Items[0];
            Assert.AreEqual(expectedOutputProperty.WaterLevel, firstOutputProperties.WaterLevel);
            Assert.AreEqual(expectedOutputProperty.WaveHeight, firstOutputProperties.WaveHeight);
            Assert.AreEqual(expectedOutputProperty.WavePeakPeriod, firstOutputProperties.WavePeakPeriod);
            Assert.AreEqual(expectedOutputProperty.WaveAngle, firstOutputProperties.WaveAngle);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var items = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
            };

            var waveImpactAsphaltCoverWaveConditionsOutput = new WaveImpactAsphaltCoverWaveConditionsOutput(items);
            var properties = new WaveImpactAsphaltCoverWaveConditionsOutputProperties()
            {
                Data = waveImpactAsphaltCoverWaveConditionsOutput
            };

            // Call
            var propertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor itemsProperty = dynamicProperties[0];
            Assert.IsNotNull(itemsProperty);
            Assert.IsTrue(itemsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(itemsProperty.Converter);
            Assert.AreEqual("Resultaat", itemsProperty.Category);
            Assert.AreEqual("Hydraulische randvoorwaarden voor asfalt", itemsProperty.DisplayName);
            Assert.AreEqual("Berekende resultaten voor de hydraulische randvoorwaarden voor asfalt.", itemsProperty.Description);
        }
    }
}