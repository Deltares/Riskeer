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
using System.Globalization;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    public class MacroStabilityInwardsSoilLayer1DPropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsSoilLayer1D>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9)
            {
                Properties =
                {
                    MaterialName = "Test Name",
                    IsAquifer = true
                }
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties
            {
                Data = layer
            };

            // Assert
            Assert.AreEqual("Test Name", properties.Name);
            Assert.IsTrue(properties.IsAquifer);
            Assert.AreEqual(layer.Top.ToString(CultureInfo.CurrentCulture), properties.TopLevel);
        }

        [Test]
        public void ToString_Always_ReturnName()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9)
            {
                Properties =
                {
                    MaterialName = "Layer A"
                }
            };

            var properties = new MacroStabilityInwardsSoilLayer1DProperties
            {
                Data = layer
            };
            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual("Layer A", name);
        }

        [Test]
        public void Constructor_ValidData_PropertieshaveExpectedAttributeValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9)
            {
                Properties =
                {
                    MaterialName = "Test Name",
                    IsAquifer = true
                }
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties
            {
                Data = layer
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "De naam van de grondlaag.",
                                                                            true);
            PropertyDescriptor topLevelProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(topLevelProperty,
                                                                            generalCategoryName,
                                                                            "Topniveau",
                                                                            "Het niveau van de bovenkant van deze grondlaag binnen de ondergrondschematisatie.",
                                                                            true);
            PropertyDescriptor isAquiferProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isAquiferProperty,
                                                                            generalCategoryName,
                                                                            "Is aquifer",
                                                                            "Geeft aan of deze grondlaag een watervoerende laag betreft.",
                                                                            true);
        }
    }
}