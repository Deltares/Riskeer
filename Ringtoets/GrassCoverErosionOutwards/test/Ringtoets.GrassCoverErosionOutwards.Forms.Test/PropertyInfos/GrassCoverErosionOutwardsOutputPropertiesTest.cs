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
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsOutputPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties();

            //Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsWaveConditionsOutput>>(properties);
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

            var grassCoverErosionOutwardsWaveConditionsOutput = new GrassCoverErosionOutwardsWaveConditionsOutput(items);

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties()
            {
                Data = grassCoverErosionOutwardsWaveConditionsOutput
            };

            //Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Items, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(items.Length, properties.Items.Length);
            Assert.AreEqual(items[0].WaterLevel, properties.Items[0].WaterLevel);
            Assert.AreEqual(items[0].WaveHeight, properties.Items[0].WaveHeight);
            Assert.AreEqual(items[0].WavePeakPeriod, properties.Items[0].WavePeakPeriod);
            Assert.AreEqual(items[0].WaveAngle, properties.Items[0].WaveAngle);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var items = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
            };

            const string expectedDisplayName = "Hydraulische randvoorwaarden";
            const string expectedDescription = "Berekende resultaten voor de hydraulische randvoorwaarden.";
            const string expectedCategory = "Resultaat";

            var grassCoverErosionOutwardsOutputProperties = new GrassCoverErosionOutwardsWaveConditionsOutput(items);
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties()
            {
                Data = grassCoverErosionOutwardsOutputProperties
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
            Assert.AreEqual(expectedCategory, itemsProperty.Category);
            Assert.AreEqual(expectedDisplayName, itemsProperty.DisplayName);
            Assert.AreEqual(expectedDescription, itemsProperty.Description);
        }
    }
}