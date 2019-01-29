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
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingSurfaceLinePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingSurfaceLineProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<PipingSurfaceLine>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some nice name>";
            var point1 = new Point3D(1.1, 2.2, 3.3);
            var point2 = new Point3D(2.1, 2.2, 3.3);

            var surfaceLine = new PipingSurfaceLine(expectedName);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2
            });
            surfaceLine.SetDikeToeAtRiverAt(point1);
            surfaceLine.SetDikeToeAtPolderAt(point2);
            surfaceLine.SetDitchDikeSideAt(point1);
            surfaceLine.SetBottomDitchDikeSideAt(point1);
            surfaceLine.SetBottomDitchPolderSideAt(point2);
            surfaceLine.SetDitchPolderSideAt(point2);

            var properties = new PipingSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Call & Assert
            Assert.AreEqual(expectedName, properties.Name);
            CollectionAssert.AreEqual(surfaceLine.Points, properties.Points);
            Assert.AreEqual(point1, properties.DikeToeAtRiver);
            Assert.AreEqual(point2, properties.DikeToeAtPolder);
            Assert.AreEqual(point1, properties.DitchDikeSide);
            Assert.AreEqual(point1, properties.BottomDitchDikeSide);
            Assert.AreEqual(point2, properties.BottomDitchPolderSide);
            Assert.AreEqual(point2, properties.DitchPolderSide);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);

            // Call
            var properties = new PipingSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

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
                "Teen dijk buitenwaarts",
                "De locatie van de teen van de dijk wanneer de dijk van buiten de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[3],
                charactersticPointsCategory,
                "Teen dijk binnenwaarts",
                "De locatie van de teen van de dijk wanneer de dijk van binnen de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[4],
                charactersticPointsCategory,
                "Insteek sloot dijkzijde",
                "De locatie van het begin van de sloot wanneer deze van de kant van de dijk wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[5],
                charactersticPointsCategory,
                "Slootbodem dijkzijde",
                "De locatie van het begin van de slootbodem wanneer deze van de kant van de dijk wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[6],
                charactersticPointsCategory,
                "Slootbodem polderzijde",
                "De locatie van het begin van de slootbodem wanneer deze van binnen de polder wordt benaderd.",
                true);

            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                dynamicProperties[7],
                charactersticPointsCategory,
                "Insteek sloot polderzijde",
                "De locatie van het begin van de sloot wanneer deze van binnen de polder wordt benaderd.",
                true);
        }
    }
}