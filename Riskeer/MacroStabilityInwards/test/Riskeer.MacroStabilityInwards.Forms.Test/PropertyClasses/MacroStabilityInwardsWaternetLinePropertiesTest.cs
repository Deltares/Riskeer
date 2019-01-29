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
using Core.Common.Base.Geometry;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Primitives.TestUtil;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsWaternetLinePropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsWaternetLineProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidWaternetLine_ExpectedValues()
        {
            // Setup
            var waternetLine = new TestMacroStabilityInwardsWaternetLine();

            // Call
            var properties = new MacroStabilityInwardsWaternetLineProperties(waternetLine);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsWaternetLine>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsWaternetLineProperties, ExpandableReadOnlyArrayConverter>(
                nameof(MacroStabilityInwardsWaternetLineProperties.Geometry));
            Assert.AreSame(waternetLine, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var waternetLine = new MacroStabilityInwardsWaternetLine("Zone A",
                                                                     new[]
                                                                     {
                                                                         new Point2D(random.NextDouble(), random.NextDouble()),
                                                                         new Point2D(random.NextDouble(), random.NextDouble())
                                                                     },
                                                                     new MacroStabilityInwardsPhreaticLine("PL1", Enumerable.Empty<Point2D>()));

            // Call
            var properties = new MacroStabilityInwardsWaternetLineProperties(waternetLine);

            // Assert
            Assert.AreEqual(waternetLine.Name, properties.Name);
            CollectionAssert.AreEqual(waternetLine.Geometry, properties.Geometry);
            Assert.AreEqual(waternetLine.PhreaticLine.Name, properties.PhreaticLineName);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            const string expectedName = "PL2";
            var waternetLine = new MacroStabilityInwardsWaternetLine(expectedName,
                                                                     Enumerable.Empty<Point2D>(),
                                                                     new TestMacroStabilityInwardsPhreaticLine());
            var properties = new MacroStabilityInwardsWaternetLineProperties(waternetLine);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual(name, expectedName);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var waternetLine = new TestMacroStabilityInwardsWaternetLine();

            // Call
            var properties = new MacroStabilityInwardsWaternetLineProperties(waternetLine);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string waterStressesCategoryName = "Waterspanningen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            waterStressesCategoryName,
                                                                            "Naam",
                                                                            "De naam van de zone.",
                                                                            true);

            PropertyDescriptor geometryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(geometryProperty,
                                                                            waterStressesCategoryName,
                                                                            "Geometrie",
                                                                            "De geometrie van de zone.",
                                                                            true);

            PropertyDescriptor phreaticLineNameProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(phreaticLineNameProperty,
                                                                            waterStressesCategoryName,
                                                                            "Stijghoogtelijn",
                                                                            "De stijghoogtelijn behorend bij de zone.",
                                                                            true);
        }
    }
}