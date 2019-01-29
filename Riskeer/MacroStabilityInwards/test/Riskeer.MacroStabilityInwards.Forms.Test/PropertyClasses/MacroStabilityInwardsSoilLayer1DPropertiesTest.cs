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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    public class MacroStabilityInwardsSoilLayer1DPropertiesTest
    {
        [Test]
        public void Constructor_MacroStabilityInwardsSoilLayer1DNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer1DProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidMacroStabilityInwardsSoilLayer1D_ExpectedValues()
        {
            // Setup
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(2.0);

            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties(soilLayer);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsSoilLayer1D>>(properties);
            Assert.AreSame(soilLayer, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double topLevel = random.NextDouble();

            var layer = new MacroStabilityInwardsSoilLayer1D(topLevel)
            {
                Data =
                {
                    MaterialName = "Test Name",
                    IsAquifer = true
                }
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties(layer);

            // Assert
            Assert.AreEqual(layer.Data.MaterialName, properties.Name);
            Assert.AreEqual(layer.Data.IsAquifer, properties.IsAquifer);
            Assert.AreEqual(2, properties.TopLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(topLevel, properties.TopLevel, properties.TopLevel.GetAccuracy());
        }

        [Test]
        public void GetProperties_WithDataEmptyName_ReturnExpectedValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9)
            {
                Data =
                {
                    MaterialName = string.Empty
                }
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties(layer);

            // Assert
            Assert.AreEqual("Onbekend", properties.Name);
        }

        [Test]
        public void ToString_ValidName_ReturnsMaterialName()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9)
            {
                Data =
                {
                    MaterialName = "Layer A"
                }
            };

            var properties = new MacroStabilityInwardsSoilLayer1DProperties(layer);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual(layer.Data.MaterialName, name);
        }

        [Test]
        public void ToString_EmptyName_ReturnsDefaultName()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9)
            {
                Data =
                {
                    MaterialName = string.Empty
                }
            };

            var properties = new MacroStabilityInwardsSoilLayer1DProperties(layer);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual("Onbekend", name);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer1D(-2.9);

            // Call
            var properties = new MacroStabilityInwardsSoilLayer1DProperties(layer);

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