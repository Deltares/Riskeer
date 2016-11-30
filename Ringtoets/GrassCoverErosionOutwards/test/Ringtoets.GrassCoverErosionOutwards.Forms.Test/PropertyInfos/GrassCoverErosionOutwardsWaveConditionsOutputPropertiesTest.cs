﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.PropertyInfos
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
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties()
            {
                Data = grassCoverErosionOutwardsWaveConditionsOutput
            };

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Items, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(items.Length, properties.Items.Length);

            var firstOutput = items[0];
            var firstOutputProperties = properties.Items[0];
            Assert.AreEqual(firstOutput.WaterLevel, firstOutputProperties.WaterLevel);
            Assert.AreEqual(firstOutput.WaveHeight, firstOutputProperties.WaveHeight);
            Assert.AreEqual(firstOutput.WavePeakPeriod, firstOutputProperties.WavePeakPeriod);
            Assert.AreEqual(firstOutput.WaveAngle, firstOutputProperties.WaveAngle);
            Assert.AreEqual(firstOutput.WaveDirection, firstOutputProperties.WaveDirection);

            double expectedTargetReliability = firstOutput.TargetReliability;
            double expectedTargetProbability = StatisticsConverter.ReliabilityToProbability(expectedTargetReliability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedTargetProbability), firstOutputProperties.TargetProbability);
            Assert.AreEqual(expectedTargetReliability, firstOutputProperties.TargetReliability,
                            firstOutputProperties.TargetReliability.GetAccuracy());

            double expectedCalculatedReliability = firstOutput.TargetReliability;
            double expectedCalculatedProbability = StatisticsConverter.ReliabilityToProbability(expectedCalculatedReliability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedCalculatedProbability), firstOutputProperties.TargetProbability);
            Assert.AreEqual(expectedTargetReliability, firstOutputProperties.TargetReliability,
                            firstOutputProperties.TargetReliability.GetAccuracy());
            Assert.AreEqual(string.Empty, firstOutputProperties.Convergence);
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
            var properties = new GrassCoverErosionOutwardsWaveConditionsOutputProperties()
            {
                Data = grassCoverErosionOutwardsOutputProperties
            };

            // Assert
            var propertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor itemsProperty = dynamicProperties[0];
            Assert.IsNotNull(itemsProperty);
            Assert.IsTrue(itemsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(itemsProperty.Converter);
            Assert.AreEqual("Resultaat", itemsProperty.Category);
            Assert.AreEqual("Hydraulische randvoorwaarden voor gras", itemsProperty.DisplayName);
            Assert.AreEqual("Berekende resultaten voor de hydraulische randvoorwaarden voor gras.", itemsProperty.Description);
        }
    }
}