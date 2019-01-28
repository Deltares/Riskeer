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

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsWaternetPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsWaternetProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidWaternet_ExpectedValues()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new[]
            {
                new TestMacroStabilityInwardsPhreaticLine()
            }, new[]
            {
                new TestMacroStabilityInwardsWaternetLine()
            });

            // Call
            var properties = new MacroStabilityInwardsWaternetProperties(waternet);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsWaternet>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaternetProperties, ExpandableReadOnlyArrayConverter>(
                nameof(MacroStabilityInwardsWaternetProperties.PhreaticLines));
            Assert.AreSame(waternet, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new[]
            {
                new TestMacroStabilityInwardsPhreaticLine()
            }, new[]
            {
                new TestMacroStabilityInwardsWaternetLine()
            });

            // Call
            var properties = new MacroStabilityInwardsWaternetProperties(waternet);

            // Assert
            Assert.AreSame(waternet.PhreaticLines.Single(), properties.PhreaticLines.Single().Data);
            Assert.AreSame(waternet.WaternetLines.Single(), properties.WaternetLines.Single().Data);
        }

        [Test]
        public void ToString_Always_ReturnEmpty()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(Enumerable.Empty<MacroStabilityInwardsPhreaticLine>(),
                                                             Enumerable.Empty<MacroStabilityInwardsWaternetLine>());
            var properties = new MacroStabilityInwardsWaternetProperties(waternet);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.IsEmpty(name);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(Enumerable.Empty<MacroStabilityInwardsPhreaticLine>(),
                                                             Enumerable.Empty<MacroStabilityInwardsWaternetLine>());

            // Call
            var properties = new MacroStabilityInwardsWaternetProperties(waternet);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string waterStressesCategoryName = "Waterspanningen";

            PropertyDescriptor phreaticLinesProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(phreaticLinesProperty,
                                                                            waterStressesCategoryName,
                                                                            "Stijghoogtelijnen",
                                                                            "Eigenschappen van de stijghoogtelijnen.",
                                                                            true);

            PropertyDescriptor waternetLinesProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(waternetLinesProperty,
                                                                            waterStressesCategoryName,
                                                                            "Zones",
                                                                            "Eigenschappen van de zones.",
                                                                            true);
        }
    }
}