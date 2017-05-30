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
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class RingtoetsMacroStabilityInwardsSurfaceLinePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new RingtoetsMacroStabilityInwardsSurfaceLineProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<RingtoetsMacroStabilityInwardsSurfaceLine>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some nice name>";
            var point1 = new Point3D(1.1, 2.2, 3.3);
            var point2 = new Point3D(2.1, 2.2, 3.3);

            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = expectedName
            };
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2
            });

            var properties = new RingtoetsMacroStabilityInwardsSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Call & Assert
            Assert.AreEqual(expectedName, properties.Name);
            CollectionAssert.AreEqual(surfaceLine.Points, properties.Points);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

            // Call
            var properties = new RingtoetsMacroStabilityInwardsSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("Naam van de profielschematisatie.", nameProperty.Description);

            PropertyDescriptor pointsProperty = dynamicProperties[1];
            Assert.IsTrue(pointsProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, pointsProperty.Category);
            Assert.AreEqual("Geometriepunten", pointsProperty.DisplayName);
            Assert.AreEqual("De punten die de geometrie van de profielschematisatie definiëren.", pointsProperty.Description);
        }
    }
}