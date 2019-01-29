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

using System.ComponentModel;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsSurfaceLinePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new MacroStabilityInwardsSurfaceLineProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsSurfaceLine>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some nice name>";
            var point1 = new Point3D(1.1, 2.2, 3.3);
            var point2 = new Point3D(2.1, 2.3, 3.3);
            var point3 = new Point3D(3.8, 2.4, 5.5);
            var point4 = new Point3D(4.4, 2.5, 4.4);
            var point5 = new Point3D(5.1, 2.6, 3.3);
            var point6 = new Point3D(6.1, 2.7, 3.3);
            var point7 = new Point3D(7.4, 2.8, 1.2);
            var point8 = new Point3D(8.2, 2.9, 1.2);
            var point9 = new Point3D(9.3, 3.2, 1.0);
            var point10 = new Point3D(10.1, 3.5, 0.5);
            var point11 = new Point3D(12.2, 3.8, 0.5);
            var point12 = new Point3D(13.1, 4.2, 1.0);

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(expectedName);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
                point3,
                point4,
                point5,
                point6,
                point7,
                point8,
                point9,
                point10,
                point11,
                point12
            });

            surfaceLine.SetSurfaceLevelOutsideAt(point1);
            surfaceLine.SetDikeToeAtRiverAt(point2);
            surfaceLine.SetDikeTopAtPolderAt(point3);
            surfaceLine.SetShoulderBaseInsideAt(point4);
            surfaceLine.SetShoulderTopInsideAt(point5);
            surfaceLine.SetDikeToeAtPolderAt(point6);
            surfaceLine.SetDitchDikeSideAt(point7);
            surfaceLine.SetBottomDitchDikeSideAt(point8);
            surfaceLine.SetBottomDitchPolderSideAt(point9);
            surfaceLine.SetDitchPolderSideAt(point10);
            surfaceLine.SetSurfaceLevelInsideAt(point11);
            surfaceLine.SetDikeTopAtRiverAt(point12);

            var properties = new MacroStabilityInwardsSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Call & Assert
            Assert.AreEqual(expectedName, properties.Name);
            CollectionAssert.AreEqual(surfaceLine.Points, properties.Points);
            Assert.AreEqual(surfaceLine.SurfaceLevelOutside, properties.SurfaceLevelOutside);
            Assert.AreEqual(surfaceLine.DikeToeAtRiver, properties.DikeToeAtRiver);
            Assert.AreEqual(surfaceLine.DikeTopAtRiver, properties.DikeTopAtRiver);
            Assert.AreEqual(surfaceLine.DikeTopAtPolder, properties.DikeTopAtPolder);
            Assert.AreEqual(surfaceLine.ShoulderBaseInside, properties.ShoulderBaseInside);
            Assert.AreEqual(surfaceLine.ShoulderTopInside, properties.ShoulderTopInside);
            Assert.AreEqual(surfaceLine.DikeToeAtPolder, properties.DikeToeAtPolder);
            Assert.AreEqual(surfaceLine.DitchDikeSide, properties.DitchDikeSide);
            Assert.AreEqual(surfaceLine.BottomDitchDikeSide, properties.BottomDitchDikeSide);
            Assert.AreEqual(surfaceLine.BottomDitchPolderSide, properties.BottomDitchPolderSide);
            Assert.AreEqual(surfaceLine.DitchPolderSide, properties.DitchPolderSide);
            Assert.AreEqual(surfaceLine.SurfaceLevelInside, properties.SurfaceLevelInside);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            // Call
            var properties = new MacroStabilityInwardsSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(14, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string charactersticPointsCategory = "Karakteristieke punten";

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[0],
                generalCategory,
                "Naam",
                "Naam van de profielschematisatie.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[1],
                generalCategory,
                "Geometriepunten",
                "De punten die de geometrie van de profielschematisatie definiëren.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[2],
                charactersticPointsCategory,
                "Maaiveld buitenwaarts",
                "De locatie van het maaiveld buiten de polder.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[3],
                charactersticPointsCategory,
                "Teen dijk buitenwaarts",
                "De locatie van de teen van de dijk wanneer de dijk van buiten de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[4],
                charactersticPointsCategory,
                "Kruin buitentalud",
                "De kruin van het talud wanneer de dijk van buiten de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[5],
                charactersticPointsCategory,
                "Kruin binnentalud",
                "De kruin van het talud wanneer de dijk van binnen de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[6],
                charactersticPointsCategory,
                "Insteek binnenberm",
                "De locatie van de insteek van de binnenberm.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[7],
                charactersticPointsCategory,
                "Kruin binnenberm",
                "De locatie van de kruin van de binnenberm.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[8],
                charactersticPointsCategory,
                "Teen dijk binnenwaarts",
                "De locatie van de teen van de dijk wanneer de dijk van binnen de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[9],
                charactersticPointsCategory,
                "Insteek sloot dijkzijde",
                "De locatie van het begin van de sloot wanneer deze van de kant van de dijk wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[10],
                charactersticPointsCategory,
                "Slootbodem dijkzijde",
                "De locatie van het begin van de slootbodem wanneer deze van de kant van de dijk wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[11],
                charactersticPointsCategory,
                "Slootbodem polderzijde",
                "De locatie van het begin van de slootbodem wanneer deze van binnen de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[12],
                charactersticPointsCategory,
                "Insteek sloot polderzijde",
                "De locatie van het begin van de sloot wanneer deze van binnen de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[13],
                charactersticPointsCategory,
                "Maaiveld binnenwaarts",
                "De locatie van het maaiveld binnen de polder.",
                true);
        }
    }
}