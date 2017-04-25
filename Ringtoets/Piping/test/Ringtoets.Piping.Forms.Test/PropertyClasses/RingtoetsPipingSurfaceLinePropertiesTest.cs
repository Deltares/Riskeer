// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLinePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new RingtoetsPipingSurfaceLineProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<RingtoetsPipingSurfaceLine>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some nice name>";
            var point1 = new Point3D(1.1, 2.2, 3.3);
            var point2 = new Point3D(2.1, 2.2, 3.3);

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = expectedName
            };
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

            var properties = new RingtoetsPipingSurfaceLineProperties
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            var properties = new RingtoetsPipingSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(8, dynamicProperties.Count);

            const string generalCategory = "Algemeen";
            const string charactersticPointsCategory = "Karakteristieke punten";

            PropertyDescriptor nameProperty = dynamicProperties[0];
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("Naam van de profielschematisatie.", nameProperty.Description);

            PropertyDescriptor dikeToeAtRiverProperty = dynamicProperties[1];
            Assert.IsTrue(dikeToeAtRiverProperty.IsReadOnly);
            Assert.AreEqual(charactersticPointsCategory, dikeToeAtRiverProperty.Category);
            Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverProperty.DisplayName);
            Assert.AreEqual("De locatie van de teen van de dijk wanneer de dijk van buiten de polder wordt benaderd.", dikeToeAtRiverProperty.Description);

            PropertyDescriptor dikeToeAtPolderProperty = dynamicProperties[2];
            Assert.IsTrue(dikeToeAtPolderProperty.IsReadOnly);
            Assert.AreEqual(charactersticPointsCategory, dikeToeAtPolderProperty.Category);
            Assert.AreEqual("Teen dijk binnenwaarts", dikeToeAtPolderProperty.DisplayName);
            Assert.AreEqual("De locatie van de teen van de dijk wanneer de dijk van binnen de polder wordt benaderd.", dikeToeAtPolderProperty.Description);

            PropertyDescriptor ditchDikeSideProperty = dynamicProperties[3];
            Assert.IsTrue(ditchDikeSideProperty.IsReadOnly);
            Assert.AreEqual(charactersticPointsCategory, ditchDikeSideProperty.Category);
            Assert.AreEqual("Insteek sloot dijkzijde", ditchDikeSideProperty.DisplayName);
            Assert.AreEqual("De locatie van het begin van de sloot wanneer deze van de kant van de dijk wordt benaderd.", ditchDikeSideProperty.Description);

            PropertyDescriptor bottomDitchDikeSideProperty = dynamicProperties[4];
            Assert.IsTrue(bottomDitchDikeSideProperty.IsReadOnly);
            Assert.AreEqual(charactersticPointsCategory, bottomDitchDikeSideProperty.Category);
            Assert.AreEqual("Slootbodem dijkzijde", bottomDitchDikeSideProperty.DisplayName);
            Assert.AreEqual("De locatie van het begin van de slootbodem wanneer deze van de kant van de dijk wordt benaderd.", bottomDitchDikeSideProperty.Description);

            PropertyDescriptor bottomDitchPolderSideProperty = dynamicProperties[5];
            Assert.IsTrue(bottomDitchPolderSideProperty.IsReadOnly);
            Assert.AreEqual(charactersticPointsCategory, bottomDitchPolderSideProperty.Category);
            Assert.AreEqual("Slootbodem polderzijde", bottomDitchPolderSideProperty.DisplayName);
            Assert.AreEqual("De locatie van het begin van de slootbodem wanneer deze van binnen de polder wordt benaderd.", bottomDitchPolderSideProperty.Description);

            PropertyDescriptor ditchPolderSideProperty = dynamicProperties[6];
            Assert.IsTrue(ditchPolderSideProperty.IsReadOnly);
            Assert.AreEqual(charactersticPointsCategory, ditchPolderSideProperty.Category);
            Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideProperty.DisplayName);
            Assert.AreEqual("De locatie van het begin van de sloot wanneer deze van binnen de polder wordt benaderd.", ditchPolderSideProperty.Description);

            PropertyDescriptor pointsProperty = dynamicProperties[7];
            Assert.IsTrue(pointsProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, pointsProperty.Category);
            Assert.AreEqual("Geometriepunten", pointsProperty.DisplayName);
            Assert.AreEqual("De punten die de geometrie van de profielschematisatie definiëren.", pointsProperty.Description);
        }
    }
}