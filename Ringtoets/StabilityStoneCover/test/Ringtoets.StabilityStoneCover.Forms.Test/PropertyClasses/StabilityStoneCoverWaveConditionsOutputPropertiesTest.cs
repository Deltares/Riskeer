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
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.Revetment.TestUtil;
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
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverWaveConditionsOutput>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_WithBlocksAndColumns_ReturnsExpectedValues()
        {
            // Setup
            var blocksOutput = new[]
            {
                new TestWaveConditionsOutput(6, 2, 9, 4)
            };

            var columnsOutput = new[]
            {
                new TestWaveConditionsOutput(1, 0, 3, 5)
            };

            var stabilityStoneCoverWaveConditionsOutput = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties()
            {
                Data = stabilityStoneCoverWaveConditionsOutput
            };

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Blocks, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(blocksOutput.Length, properties.Blocks.Length);

            var firstBlocksOutput = blocksOutput[0];
            var firstBlocksProperties = properties.Blocks[0];
            Assert.AreEqual(firstBlocksOutput.WaterLevel, firstBlocksProperties.WaterLevel);
            Assert.AreEqual(firstBlocksOutput.WaveHeight, firstBlocksProperties.WaveHeight);
            Assert.AreEqual(firstBlocksOutput.WavePeakPeriod, firstBlocksProperties.WavePeakPeriod);
            Assert.AreEqual(firstBlocksOutput.WaveAngle, firstBlocksProperties.WaveAngle);
            Assert.AreEqual(firstBlocksOutput.WaveDirection, firstBlocksProperties.WaveDirection);

            double expectedBlocksTargerReliability = firstBlocksOutput.TargetReliability;
            double expectedBlocksTargetProbability = StatisticsConverter.ReliabilityToProbability(expectedBlocksTargerReliability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedBlocksTargetProbability), firstBlocksProperties.TargetProbability);
            Assert.AreEqual(expectedBlocksTargerReliability, firstBlocksProperties.TargetReliability,
                            firstBlocksProperties.TargetReliability.GetAccuracy());

            double expectedBlocksCalculatedReliability = firstBlocksOutput.TargetReliability;
            double expectedBlocksCalculatedProbability = StatisticsConverter.ReliabilityToProbability(expectedBlocksCalculatedReliability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedBlocksCalculatedProbability), firstBlocksProperties.TargetProbability);
            Assert.AreEqual(expectedBlocksTargerReliability, firstBlocksProperties.TargetReliability,
                            firstBlocksProperties.TargetReliability.GetAccuracy());

            Assert.AreEqual(string.Empty, firstBlocksProperties.Convergence);
            
            CollectionAssert.AllItemsAreInstancesOfType(properties.Columns, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(columnsOutput.Length, properties.Columns.Length);

            var firstColumnsOutput = columnsOutput[0];
            var firstColumnsProperties = properties.Columns[0];
            Assert.AreEqual(firstColumnsOutput.WaterLevel, firstColumnsProperties.WaterLevel);
            Assert.AreEqual(firstColumnsOutput.WaveHeight, firstColumnsProperties.WaveHeight);
            Assert.AreEqual(firstColumnsOutput.WavePeakPeriod, firstColumnsProperties.WavePeakPeriod);
            Assert.AreEqual(firstColumnsOutput.WaveAngle, firstColumnsProperties.WaveAngle);
            Assert.AreEqual(firstColumnsOutput.WaveDirection, firstColumnsProperties.WaveDirection);

            double expectedColumnsTargerReliability = firstBlocksOutput.TargetReliability;
            double expectedColumnsTargetProbability = StatisticsConverter.ReliabilityToProbability(expectedColumnsTargerReliability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedColumnsTargetProbability), firstBlocksProperties.TargetProbability);
            Assert.AreEqual(expectedColumnsTargerReliability, firstBlocksProperties.TargetReliability,
                            firstBlocksProperties.TargetReliability.GetAccuracy());

            double expectedColumnsCalculatedReliability = firstBlocksOutput.TargetReliability;
            double expectedColumnsCalculatedProbability = StatisticsConverter.ReliabilityToProbability(expectedColumnsCalculatedReliability);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedColumnsCalculatedProbability), firstBlocksProperties.TargetProbability);
            Assert.AreEqual(expectedColumnsTargerReliability, firstBlocksProperties.TargetReliability,
                            firstBlocksProperties.TargetReliability.GetAccuracy());

            Assert.AreEqual(string.Empty, firstBlocksProperties.Convergence);

        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var blocksOutput = new[]
            {
                new TestWaveConditionsOutput(6, 2, 9, 4),
            };

            var columnsOutput = new[]
            {
                new TestWaveConditionsOutput(1, 0, 3, 5),
            };

            var stabilityStoneCoverWaveConditionsOutput = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties()
            {
                Data = stabilityStoneCoverWaveConditionsOutput
            };

            // Call
            var propertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = propertyBag.GetProperties(new Attribute[]
            {
                new BrowsableAttribute(true)
            });

            // Assert
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor blocksProperty = dynamicProperties[requiredBlockPropertyIndex];
            Assert.IsNotNull(blocksProperty);
            Assert.IsTrue(blocksProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(blocksProperty.Converter);
            Assert.AreEqual("Resultaat", blocksProperty.Category);
            Assert.AreEqual("Hydraulische randvoorwaarden voor blokken", blocksProperty.DisplayName);
            Assert.AreEqual("Berekende resultaten voor de hydraulische randvoorwaarden voor blokken.", blocksProperty.Description);

            PropertyDescriptor columnsProperty = dynamicProperties[requiredColumnPropertyIndex];
            Assert.IsNotNull(columnsProperty);
            Assert.IsTrue(columnsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(columnsProperty.Converter);
            Assert.AreEqual("Resultaat", columnsProperty.Category);
            Assert.AreEqual("Hydraulische randvoorwaarden voor zuilen", columnsProperty.DisplayName);
            Assert.AreEqual("Berekende resultaten voor de hydraulische randvoorwaarden voor zuilen.", columnsProperty.Description);
        }
    }
}