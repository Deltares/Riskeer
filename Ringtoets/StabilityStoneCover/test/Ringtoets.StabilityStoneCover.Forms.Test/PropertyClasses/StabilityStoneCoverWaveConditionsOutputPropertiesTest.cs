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
using Ringtoets.Revetment.Data.TestUtil;
using Ringtoets.Revetment.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputPropertiesTest
    {
        private const int requiredBlockPropertyIndex = 0;
        private const int requiredColumnPropertyIndex = 1;

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
                new TestWaveConditionsOutput()
            };

            var columnsOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            var stabilityStoneCoverWaveConditionsOutput = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties
            {
                Data = stabilityStoneCoverWaveConditionsOutput
            };

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Blocks, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(blocksOutput.Length, properties.Blocks.Length);

            TestWaveConditionsOutput firstBlocksOutput = blocksOutput[0];
            WaveConditionsOutputProperties firstBlocksProperties = properties.Blocks[0];
            Assert.AreEqual(firstBlocksOutput.WaterLevel, firstBlocksProperties.WaterLevel);
            Assert.AreEqual(firstBlocksOutput.WaveHeight, firstBlocksProperties.WaveHeight);
            Assert.AreEqual(firstBlocksOutput.WavePeakPeriod, firstBlocksProperties.WavePeakPeriod);
            Assert.AreEqual(firstBlocksOutput.WaveAngle, firstBlocksProperties.WaveAngle);
            Assert.AreEqual(firstBlocksOutput.WaveDirection, firstBlocksProperties.WaveDirection);
            Assert.AreEqual(firstBlocksOutput.TargetProbability, firstBlocksProperties.TargetProbability);
            Assert.AreEqual(firstBlocksOutput.TargetReliability, firstBlocksProperties.TargetReliability,
                            firstBlocksProperties.TargetReliability.GetAccuracy());
            Assert.AreEqual(firstBlocksOutput.CalculatedProbability, firstBlocksProperties.CalculatedProbability);
            Assert.AreEqual(firstBlocksOutput.TargetReliability, firstBlocksProperties.TargetReliability,
                            firstBlocksProperties.TargetReliability.GetAccuracy());

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(firstBlocksOutput.CalculationConvergence).DisplayName;
            Assert.AreEqual(convergenceValue, firstBlocksProperties.Convergence);

            CollectionAssert.AllItemsAreInstancesOfType(properties.Columns, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(columnsOutput.Length, properties.Columns.Length);

            TestWaveConditionsOutput firstColumnsOutput = columnsOutput[0];
            WaveConditionsOutputProperties firstColumnsProperties = properties.Columns[0];
            Assert.AreEqual(firstColumnsOutput.WaterLevel, firstColumnsProperties.WaterLevel);
            Assert.AreEqual(firstColumnsOutput.WaveHeight, firstColumnsProperties.WaveHeight);
            Assert.AreEqual(firstColumnsOutput.WavePeakPeriod, firstColumnsProperties.WavePeakPeriod);
            Assert.AreEqual(firstColumnsOutput.WaveAngle, firstColumnsProperties.WaveAngle);
            Assert.AreEqual(firstColumnsOutput.WaveDirection, firstColumnsProperties.WaveDirection);
            Assert.AreEqual(firstColumnsOutput.TargetProbability, firstColumnsProperties.TargetProbability);
            Assert.AreEqual(firstColumnsOutput.TargetReliability, firstColumnsProperties.TargetReliability,
                            firstColumnsProperties.TargetReliability.GetAccuracy());
            Assert.AreEqual(firstBlocksOutput.CalculatedProbability, firstColumnsProperties.CalculatedProbability);
            Assert.AreEqual(firstColumnsOutput.TargetReliability, firstColumnsProperties.TargetReliability,
                            firstColumnsProperties.TargetReliability.GetAccuracy());

            convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(firstColumnsOutput.CalculationConvergence).DisplayName;
            Assert.AreEqual(convergenceValue, firstColumnsProperties.Convergence);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var blocksOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            var columnsOutput = new[]
            {
                new TestWaveConditionsOutput()
            };

            var stabilityStoneCoverWaveConditionsOutput = new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties
            {
                Data = stabilityStoneCoverWaveConditionsOutput
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor blocksProperty = dynamicProperties[requiredBlockPropertyIndex];
            Assert.IsNotNull(blocksProperty);
            Assert.IsTrue(blocksProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(blocksProperty.Converter);
            Assert.AreEqual("Resultaat", blocksProperty.Category);
            Assert.AreEqual("Hydraulische belastingen voor blokken", blocksProperty.DisplayName);
            Assert.AreEqual("Berekende hydraulische belastingen voor blokken.", blocksProperty.Description);

            PropertyDescriptor columnsProperty = dynamicProperties[requiredColumnPropertyIndex];
            Assert.IsNotNull(columnsProperty);
            Assert.IsTrue(columnsProperty.IsReadOnly);
            Assert.IsInstanceOf<ExpandableArrayConverter>(columnsProperty.Converter);
            Assert.AreEqual("Resultaat", columnsProperty.Category);
            Assert.AreEqual("Hydraulische belastingen voor zuilen", columnsProperty.DisplayName);
            Assert.AreEqual("Berekende hydraulische belastingen voor zuilen.", columnsProperty.Description);
        }
    }
}