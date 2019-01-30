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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    public class MacroStabilityInwardsSoilLayer2DBasePropertiesTest
    {
        [Test]
        public void Constructor_MacroStabilityInwardsSoilLayer2DNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2DBaseProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidMacroStabilityInwardsSoilLayer2D_ExpectedValues()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D soilLayer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();

            // Call
            var properties = new MacroStabilityInwardsSoilLayer2DBaseProperties(soilLayer);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsSoilLayer2D>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsSoilLayer2DBaseProperties, ExpandableObjectConverter>();
            TestHelper.AssertTypeConverter<MacroStabilityInwardsSoilLayer2DBaseProperties,
                ExpandableArrayConverter>(nameof(MacroStabilityInwardsSoilLayer2DBaseProperties.Geometry));
            Assert.AreSame(soilLayer, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var layer = new MacroStabilityInwardsSoilLayer2D(
                new Ring(new[]
                {
                    new Point2D(20.210230, 26.00001),
                    new Point2D(3.830, 1.040506)
                }),
                new MacroStabilityInwardsSoilLayerData
                {
                    MaterialName = "Test Name",
                    IsAquifer = true
                },
                new[]
                {
                    new MacroStabilityInwardsSoilLayer2D(
                        new Ring(new[]
                        {
                            new Point2D(12.987, 12.821),
                            new Point2D(4.23, 1.02)
                        }))
                });

            // Call
            var properties = new MacroStabilityInwardsSoilLayer2DBaseProperties(layer);

            // Assert
            Assert.AreEqual(layer.Data.MaterialName, properties.Name);
            CollectionAssert.AreEqual(layer.OuterRing.Points, properties.Geometry);
        }

        [Test]
        public void GetProperties_WithDataEmptyName_ReturnExpectedValues()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();
            layer.Data.MaterialName = string.Empty;

            // Call
            var properties = new MacroStabilityInwardsSoilLayer2DBaseProperties(layer);

            // Assert
            Assert.AreEqual("Onbekend", properties.Name);
        }

        [Test]
        public void ToString_ValidName_ReturnsMaterialName()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();
            layer.Data.MaterialName = "Layer A 2D";

            var properties = new MacroStabilityInwardsSoilLayer2DBaseProperties(layer);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual(layer.Data.MaterialName, name);
        }

        [Test]
        public void ToString_EmptyName_ReturnsDefaultName()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();
            layer.Data.MaterialName = string.Empty;

            var properties = new MacroStabilityInwardsSoilLayer2DBaseProperties(layer);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual("Onbekend", name);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D layer = MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D();

            // Call
            var properties = new MacroStabilityInwardsSoilLayer2DBaseProperties(layer);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategoryName = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            generalCategoryName,
                                                                            "Naam",
                                                                            "De naam van de grondlaag.",
                                                                            true);

            PropertyDescriptor outerRingProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(outerRingProperty,
                                                                            generalCategoryName,
                                                                            "Geometrie",
                                                                            "De geometrie van de buitenring van deze grondlaag.",
                                                                            true);
        }
    }
}