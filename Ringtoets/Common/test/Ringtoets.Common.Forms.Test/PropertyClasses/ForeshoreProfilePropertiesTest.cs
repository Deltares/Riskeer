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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ForeshoreProfilePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int worldReferencePointPropertyIndex = 1;
        private const int orientationPropertyIndex = 2;
        private const int breakWaterPropertyIndex = 3;
        private const int foreshoreGeometryPropertyIndex = 4;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new ForeshoreProfileProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ForeshoreProfile>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewForeshoreProfileInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            const string name = "Voorland";
            var foreshoreProfile = new ForeshoreProfile(new Point2D(12.34, 56.78), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Name = name
                                                        });

            // Call
            var properties = new ForeshoreProfileProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.AreSame(foreshoreProfile, properties.BreakWater.Data);
            Assert.AreEqual(foreshoreProfile, properties.Foreshore.Data);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties());

            // Call
            var properties = new ForeshoreProfileProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(6, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("Naam van het voorland.", nameProperty.Description);

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            Assert.IsNotNull(worldReferencePointProperty);
            Assert.IsTrue(worldReferencePointProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", worldReferencePointProperty.Category);
            Assert.AreEqual("Locatie (RD) [m]", worldReferencePointProperty.DisplayName);
            Assert.AreEqual("De coördinaten van de locatie van het voorland in het Rijksdriehoeksstelsel.", worldReferencePointProperty.Description);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            Assert.IsNotNull(orientationProperty);
            Assert.IsTrue(orientationProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", orientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", orientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van het voorland.", orientationProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsNotNull(breakWaterProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", breakWaterProperty.Category);
            Assert.AreEqual("Dam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dam.", breakWaterProperty.Description);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshoreGeometryPropertyIndex];
            Assert.IsNotNull(foreshoreProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            Assert.IsTrue(foreshoreProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", foreshoreProperty.Category);
            Assert.AreEqual("Voorlandgeometrie", foreshoreProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de voorlandgeometrie.", foreshoreProperty.Description);
        }
    }
}