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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class StabilityPointStructurePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int locationPropertyIndex = 1;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new StabilityPointStructureProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<StabilityPointStructure>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewStabilityPointStructureInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            StabilityPointStructure structure = CreateSimpleStabilityPointStructure();
            var properties = new StabilityPointStructureProperties();

            // Call
            properties.Data = structure;

            // Assert
            Assert.AreEqual(structure.Name, properties.Name);
            var expectedLocation = new Point2D(new RoundedDouble(0, structure.Location.X),
                                   new RoundedDouble(0, structure.Location.Y));
            Assert.AreEqual(expectedLocation, properties.Location);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            StabilityPointStructure structure = CreateSimpleStabilityPointStructure();

            // Call
            var properties = new StabilityPointStructureProperties
            {
                Data = structure
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(2, dynamicProperties.Count);

            const string generalCategory = "Algemeen";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("De naam van het kunstwerk.", nameProperty.Description);

            PropertyDescriptor locationProperty = dynamicProperties[locationPropertyIndex];
            Assert.IsTrue(locationProperty.IsReadOnly);
            Assert.AreEqual(generalCategory, locationProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", locationProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van het kunstwerk in het Rijksdriehoeksstelsel.", locationProperty.Description);

        }

        private static StabilityPointStructure CreateSimpleStabilityPointStructure()
        {
            return new StabilityPointStructure("Name", "Id", new Point2D(1.234, 2.3456),
                                               123.456,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               456.789, 0.456,
                                               567.890, 0.567,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               123.546, 0.123,
                                               234.567, 0.234,
                                               345.678, 0.345,
                                               555.555,
                                               456.789, 0.456,
                                               555.55,
                                               0.55,
                                               567.890, 0.567,
                                               7777777.777, 0.777,
                                               567.890, 0.567,
                                               42,
                                               0.55,
                                               678.901, 0.678,
                                               789.012, 0.789,
                                               890.123, 0.890,
                                               901.234, 0.901,
                                               StabilityPointStructureInflowModelType.FloodedCulvert
                );
        }
    }
}