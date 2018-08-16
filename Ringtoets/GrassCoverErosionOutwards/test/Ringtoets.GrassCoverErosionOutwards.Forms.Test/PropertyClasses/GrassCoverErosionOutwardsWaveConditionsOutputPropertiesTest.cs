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

using System.ComponentModel;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsOutputPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<GrassCoverErosionOutwardsWaveConditionsOutput>>(properties);
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

            var grassCoverErosionOutwardsWaveConditionsOutput = new GrassCoverErosionOutwardsWaveConditionsOutput(items);

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties
            {
                Data = grassCoverErosionOutwardsWaveConditionsOutput
            };

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Items, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(items.Length, properties.Items.Length);

            TestWaveConditionsOutput firstOutput = items[0];
            WaveConditionsOutputProperties firstOutputProperties = properties.Items[0];
            Assert.AreEqual(firstOutput.WaterLevel, firstOutputProperties.WaterLevel);
            Assert.AreEqual(firstOutput.WaveHeight, firstOutputProperties.WaveHeight);
            Assert.AreEqual(firstOutput.WavePeakPeriod, firstOutputProperties.WavePeakPeriod);
            Assert.AreEqual(firstOutput.WaveAngle, firstOutputProperties.WaveAngle);
            Assert.AreEqual(firstOutput.WaveDirection, firstOutputProperties.WaveDirection);
            Assert.AreEqual(firstOutput.TargetProbability, firstOutputProperties.TargetProbability);
            Assert.AreEqual(firstOutput.TargetReliability, firstOutputProperties.TargetReliability,
                            firstOutputProperties.TargetReliability.GetAccuracy());
            Assert.AreEqual(firstOutput.CalculatedProbability, firstOutputProperties.CalculatedProbability);
            Assert.AreEqual(firstOutput.TargetReliability, firstOutputProperties.TargetReliability,
                            firstOutputProperties.TargetReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(firstOutput.CalculationConvergence).DisplayName;
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

            var grassCoverErosionOutwardsOutputProperties = new GrassCoverErosionOutwardsWaveConditionsOutput(items);

            // Call
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties
            {
                Data = grassCoverErosionOutwardsOutputProperties
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor itemsProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(itemsProperty,
                                                                            "Resultaat",
                                                                            "Hydraulische belastingen voor gras",
                                                                            "Berekende hydraulische belastingen voor gras.",
                                                                            true);
        }
    }
}