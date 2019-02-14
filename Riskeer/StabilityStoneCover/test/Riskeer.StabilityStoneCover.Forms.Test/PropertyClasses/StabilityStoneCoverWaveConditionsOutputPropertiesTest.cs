// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Data.TestUtil;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;

namespace Riskeer.StabilityStoneCover.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputPropertiesTest
    {
        private const int blockPropertyIndex = 0;
        private const int columnPropertyIndex = 1;

        [Test]
        public void Constructor_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsOutputProperties(null, new StabilityStoneCoverWaveConditionsInput());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Constructor_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            StabilityStoneCoverWaveConditionsOutput output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create();

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsOutputProperties(output, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            StabilityStoneCoverWaveConditionsOutput output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create();

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties(output, new StabilityStoneCoverWaveConditionsInput());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityStoneCoverWaveConditionsOutput>>(properties);
            Assert.AreSame(output, properties.Data);
        }

        [Test]
        public void Constructor_WithData_ReturnsExpectedValues()
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

            StabilityStoneCoverWaveConditionsOutput stabilityStoneCoverWaveConditionsOutput = StabilityStoneCoverWaveConditionsOutputTestFactory.Create(
                columnsOutput, blocksOutput);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties(
                stabilityStoneCoverWaveConditionsOutput, new StabilityStoneCoverWaveConditionsInput());

            // Assert 
            CollectionAssert.AllItemsAreInstancesOfType(properties.Blocks, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(blocksOutput.Length, properties.Blocks.Length);

            TestWaveConditionsOutput firstBlocksOutput = blocksOutput[0];
            WaveConditionsOutputProperties firstBlocksProperties = properties.Blocks[0];
            Assert.AreSame(firstBlocksOutput, firstBlocksProperties.Data);

            string convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(firstBlocksOutput.CalculationConvergence).DisplayName;
            Assert.AreEqual(convergenceValue, firstBlocksProperties.Convergence);

            CollectionAssert.AllItemsAreInstancesOfType(properties.Columns, typeof(WaveConditionsOutputProperties));
            Assert.AreEqual(columnsOutput.Length, properties.Columns.Length);

            TestWaveConditionsOutput firstColumnsOutput = columnsOutput[0];
            WaveConditionsOutputProperties firstColumnsProperties = properties.Columns[0];
            Assert.AreSame(firstColumnsOutput, firstColumnsProperties.Data);

            convergenceValue = new EnumDisplayWrapper<CalculationConvergence>(firstColumnsOutput.CalculationConvergence).DisplayName;
            Assert.AreEqual(convergenceValue, firstColumnsProperties.Convergence);
        }

        [Test]
        public void Constructor_CalculationTypeBoth_PropertiesHaveExpectedAttributesValues()
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

            StabilityStoneCoverWaveConditionsOutput stabilityStoneCoverWaveConditionsOutput = StabilityStoneCoverWaveConditionsOutputTestFactory.Create(
                columnsOutput, blocksOutput);

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties(
                stabilityStoneCoverWaveConditionsOutput, new StabilityStoneCoverWaveConditionsInput());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string resultCategory = "Resultaat";

            PropertyDescriptor blocksProperty = dynamicProperties[blockPropertyIndex];
            Assert.IsInstanceOf<ExpandableArrayConverter>(blocksProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(blocksProperty,
                                                                            resultCategory,
                                                                            "Hydraulische belastingen voor blokken",
                                                                            "Berekende hydraulische belastingen voor blokken.",
                                                                            true);

            PropertyDescriptor columnsProperty = dynamicProperties[columnPropertyIndex];
            Assert.IsInstanceOf<ExpandableArrayConverter>(columnsProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(columnsProperty,
                                                                            resultCategory,
                                                                            "Hydraulische belastingen voor zuilen",
                                                                            "Berekende hydraulische belastingen voor zuilen.",
                                                                            true);
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Blocks)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Columns)]
        public void Constructor_CalculationTypeBlocksOrColumns_PropertiesHaveExpectedAttributesValues(StabilityStoneCoverWaveConditionsCalculationType calculationType)
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

            StabilityStoneCoverWaveConditionsOutput stabilityStoneCoverWaveConditionsOutput = StabilityStoneCoverWaveConditionsOutputTestFactory.Create(
                columnsOutput, blocksOutput);
            var input = new StabilityStoneCoverWaveConditionsInput
            {
                CalculationType = calculationType
            };

            // Call
            var properties = new StabilityStoneCoverWaveConditionsOutputProperties(
                stabilityStoneCoverWaveConditionsOutput, input);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            const string resultCategory = "Resultaat";

            string revetmentType = null;
            if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Blocks)
            {
                revetmentType = "blokken";
            }

            if (calculationType == StabilityStoneCoverWaveConditionsCalculationType.Columns)
            {
                revetmentType = "zuilen";
            }

            PropertyDescriptor outputProperty = dynamicProperties[0];
            Assert.IsInstanceOf<ExpandableArrayConverter>(outputProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(outputProperty,
                                                                            resultCategory,
                                                                            $"Hydraulische belastingen voor {revetmentType}",
                                                                            $"Berekende hydraulische belastingen voor {revetmentType}.",
                                                                            true);
        }

        [Test]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Both, true, true)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Blocks, true, false)]
        [TestCase(StabilityStoneCoverWaveConditionsCalculationType.Columns, false, true)]
        public void DynamicVisibleValidationMethod_DependingOnRelevancy_ReturnExpectedVisibility(
            StabilityStoneCoverWaveConditionsCalculationType calculationType, bool blocksVisible, bool columnsVisible)
        {
            // Setup
            StabilityStoneCoverWaveConditionsOutput output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create();
            var input = new StabilityStoneCoverWaveConditionsInput
            {
                CalculationType = calculationType
            };

            var properties = new StabilityStoneCoverWaveConditionsOutputProperties(output, input);

            // Call & Assert
            Assert.AreEqual(blocksVisible, properties.DynamicVisibleValidationMethod(nameof(properties.Blocks)));
            Assert.AreEqual(columnsVisible, properties.DynamicVisibleValidationMethod(nameof(properties.Columns)));
            Assert.IsTrue(properties.DynamicVisibleValidationMethod(null));
        }
    }
}