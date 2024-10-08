﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using Core.Gui.Converters;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Primitives.TestUtil;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsWaternetPropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsWaternetProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidWaternet_ExpectedValues()
        {
            // Setup
            var waternet = new MacroStabilityInwardsWaternet(new[]
            {
                MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine()
            }, new[]
            {
                MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsWaternetLine()
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
                MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsPhreaticLine()
            }, new[]
            {
                MacroStabilityInwardsTestDataFactory.CreateMacroStabilityInwardsWaternetLine()
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
            var name = properties.ToString();

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