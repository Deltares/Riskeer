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
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputPropertiesTest
    {
        private readonly int requiredBlockPropertyIndex = 0;
        private readonly int requiredColumnPropertyIndex = 1;

        [Test]
        public void Data_WithBlocksAndColumns_ReturnsExpectedValues()
        {
            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties();

            //Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverWaveConditionsOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetColumnProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var blocksOutput = new[]
            {
                new WaveConditionsOutput(6, 2, 9, 4),
            };

            var columnsOutput = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
            };

            var stabilityStoneCoverWaveConditionsOutput = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties()
            {
                Data = stabilityStoneCoverWaveConditionsOutput
            };

            //Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Blocks, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(blocksOutput.Length, properties.Blocks.Length);
            Assert.AreEqual(blocksOutput[0].WaterLevel, properties.Blocks[0].WaterLevel);
            Assert.AreEqual(blocksOutput[0].WaveHeight, properties.Blocks[0].WaveHeight);
            Assert.AreEqual(blocksOutput[0].WavePeakPeriod, properties.Blocks[0].WavePeakPeriod);
            Assert.AreEqual(blocksOutput[0].WaveAngle, properties.Blocks[0].WaveAngle);

            CollectionAssert.AllItemsAreInstancesOfType(properties.Columns, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(columnsOutput.Length, properties.Columns.Length);
            Assert.AreEqual(columnsOutput[0].WaterLevel, properties.Columns[0].WaterLevel);
            Assert.AreEqual(columnsOutput[0].WaveHeight, properties.Columns[0].WaveHeight);
            Assert.AreEqual(columnsOutput[0].WavePeakPeriod, properties.Columns[0].WavePeakPeriod);
            Assert.AreEqual(columnsOutput[0].WaveAngle, properties.Columns[0].WaveAngle);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var blocksOutput = new[]
            {
                new WaveConditionsOutput(6, 2, 9, 4),
            };

            var columnsOutput = new[]
            {
                new WaveConditionsOutput(1, 0, 3, 5),
            };

            const string expectedColumnDisplayName = "Zuilen";
            const string expectedBlockDisplayName = "Blokken";
            const string expectedColumnDescription = "Berekende resultaten voor zuilen";
            const string expectedBlockDescription = "Berekende resultaten voor blokken";
            const string expectedCategory = "Resultaat";

            var stabilityStoneCoverWaveConditionsOutput = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties()
            {
                Data = stabilityStoneCoverWaveConditionsOutput
            };

            // Call
            TypeConverter classTypeConverter = TypeDescriptor.GetConverter(properties, true);
            Assert.IsInstanceOf<ExpandableObjectConverter>(classTypeConverter);

            // Asssert
            var propertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor blocksProperty = dynamicProperties[requiredBlockPropertyIndex];
            Assert.IsNotNull(expectedBlockDisplayName);
            Assert.IsTrue(blocksProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(blocksProperty.Converter);
            Assert.AreEqual(expectedCategory, blocksProperty.Category);
            Assert.AreEqual(expectedBlockDisplayName, blocksProperty.DisplayName);
            Assert.AreEqual(expectedBlockDescription, blocksProperty.Description);

            PropertyDescriptor columnsProperty = dynamicProperties[requiredColumnPropertyIndex];
            Assert.IsNotNull(columnsProperty);
            Assert.IsTrue(columnsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(columnsProperty.Converter);
            Assert.AreEqual(expectedCategory, columnsProperty.Category);
            Assert.AreEqual(expectedColumnDisplayName, columnsProperty.DisplayName);
            Assert.AreEqual(expectedColumnDescription, columnsProperty.Description);
        }
    }
}