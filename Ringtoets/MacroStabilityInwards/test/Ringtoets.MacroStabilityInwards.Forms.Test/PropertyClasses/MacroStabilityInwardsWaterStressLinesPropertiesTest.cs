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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsWaterStressLinesPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsWaterStressLinesProperties(null, GetTestNormativeAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidWaternet_ExpectedValues()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input, GetTestNormativeAssessmentLevel());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsInput>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressLinesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressLinesProperties.WaternetDaily));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaterStressLinesProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsWaterStressLinesProperties.WaternetExtreme));
            Assert.AreSame(input, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input, GetTestNormativeAssessmentLevel());

            // Assert
            CollectionAssert.IsEmpty(properties.WaternetExtreme.PhreaticLines);
            CollectionAssert.IsEmpty(properties.WaternetExtreme.WaternetLines);
            CollectionAssert.IsEmpty(properties.WaternetDaily.PhreaticLines);
            CollectionAssert.IsEmpty(properties.WaternetDaily.WaternetLines);
        }

        [Test]
        public void ToString_Always_ReturnEmpty()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input, GetTestNormativeAssessmentLevel());

            // Call
            string name = properties.ToString();

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsWaterStressLinesProperties(input, GetTestNormativeAssessmentLevel());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string waterStressesCategoryName = "Waterspanningen";

            PropertyDescriptor waternetExtremeProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waternetExtremeProperty,
                                                                            waterStressesCategoryName,
                                                                            "Extreme omstandigheden",
                                                                            "Eigenschappen van de waterspanningslijnen bij extreme omstandigheden.",
                                                                            true);

            PropertyDescriptor waternetDailyProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waternetDailyProperty,
                                                                            waterStressesCategoryName,
                                                                            "Dagelijkse omstandigheden",
                                                                            "Eigenschappen van de waterspanningslijnen bij dagelijkse omstandigheden.",
                                                                            true);
        }

        private static RoundedDouble GetTestNormativeAssessmentLevel()
        {
            return (RoundedDouble) 1.1;
        }
    }
}