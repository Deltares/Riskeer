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
    public class MacroStabilityInwardsPhreaticLinePropertiesTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsPhreaticLineProperties(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidPhreaticLine_ExpectedValues()
        {
            // Setup
            var phreaticLine = new TestMacroStabilityInwardsPhreaticLine();

            // Call
            var properties = new MacroStabilityInwardsPhreaticLineProperties(phreaticLine);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsPhreaticLine>>(properties);
            TestHelper.AssertTypeConverter<MacroStabilityInwardsPhreaticLineProperties, ExpandableReadOnlyArrayConverter>(
                nameof(MacroStabilityInwardsPhreaticLineProperties.Geometry));
            Assert.AreSame(phreaticLine, properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var phreaticLine = new MacroStabilityInwardsPhreaticLine("PL1",
                                                                     new[]
                                                                     {
                                                                         new Point2D(random.NextDouble(), random.NextDouble()),
                                                                         new Point2D(random.NextDouble(), random.NextDouble())
                                                                     });

            // Call
            var properties = new MacroStabilityInwardsPhreaticLineProperties(phreaticLine);

            // Assert
            Assert.AreEqual(phreaticLine.Name, properties.Name);
            CollectionAssert.AreEqual(phreaticLine.Geometry, properties.Geometry);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            const string expectedName = "PL2";
            var phreaticLine = new MacroStabilityInwardsPhreaticLine(expectedName, Enumerable.Empty<Point2D>());
            var properties = new MacroStabilityInwardsPhreaticLineProperties(phreaticLine);

            // Call
            string name = properties.ToString();

            // Assert
            Assert.AreEqual(name, expectedName);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributeValues()
        {
            // Setup
            var phreaticLine = new TestMacroStabilityInwardsPhreaticLine();

            // Call
            var properties = new MacroStabilityInwardsPhreaticLineProperties(phreaticLine);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string waterStressesCategoryName = "Waterspanningen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            waterStressesCategoryName,
                                                                            "Naam",
                                                                            "De naam van de lijn.",
                                                                            true);

            PropertyDescriptor geometryProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(geometryProperty,
                                                                            waterStressesCategoryName,
                                                                            "Geometrie",
                                                                            "De geometrie van de lijn.",
                                                                            true);
        }
    }
}