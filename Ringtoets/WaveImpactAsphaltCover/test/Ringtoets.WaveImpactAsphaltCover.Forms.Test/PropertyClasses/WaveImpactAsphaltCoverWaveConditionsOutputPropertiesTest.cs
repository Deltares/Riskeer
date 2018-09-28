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

using System.ComponentModel;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Data.TestUtil;
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
                new TestWaveConditionsOutput()
            };

            var waveImpactAsphaltCoverWaveConditionsOutput = new WaveImpactAsphaltCoverWaveConditionsOutput(items);

            // Call
            var properties = new WaveImpactAsphaltCoverWaveConditionsOutputProperties
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
            Assert.AreEqual(expectedOutputProperty.WaveDirection, firstOutputProperties.WaveDirection);
            Assert.AreEqual(expectedOutputProperty.TargetProbability, firstOutputProperties.TargetProbability);
            Assert.AreEqual(expectedOutputProperty.TargetReliability, firstOutputProperties.TargetReliability,
                            firstOutputProperties.TargetReliability.GetAccuracy());
            Assert.AreEqual(expectedOutputProperty.TargetProbability, firstOutputProperties.TargetProbability);
            Assert.AreEqual(expectedOutputProperty.TargetReliability, firstOutputProperties.TargetReliability,
                            firstOutputProperties.TargetReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(expectedOutputProperty.CalculationConvergence).DisplayName;
            Assert.AreEqual(convergenceValue, firstOutputProperties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var items = new[]
            {
                new TestWaveConditionsOutput()
            };

            var waveImpactAsphaltCoverWaveConditionsOutput = new WaveImpactAsphaltCoverWaveConditionsOutput(items);

            // Call
            var properties = new WaveImpactAsphaltCoverWaveConditionsOutputProperties
            {
                Data = waveImpactAsphaltCoverWaveConditionsOutput
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor itemsProperty = dynamicProperties[0];
            Assert.IsNotNull(itemsProperty);
            Assert.IsTrue(itemsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(itemsProperty.Converter);
            Assert.AreEqual("Resultaat", itemsProperty.Category);
            Assert.AreEqual("Hydraulische belastingen voor asfalt", itemsProperty.DisplayName);
            Assert.AreEqual("Berekende hydraulische belastingen voor asfalt.", itemsProperty.Description);
        }
    }
}