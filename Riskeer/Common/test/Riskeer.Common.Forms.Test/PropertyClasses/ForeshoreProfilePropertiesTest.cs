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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PropertyClasses;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class ForeshoreProfilePropertiesTest
    {
        private const int idPropertyIndex = 0;
        private const int namePropertyIndex = 1;
        private const int worldReferencePointPropertyIndex = 2;
        private const int orientationPropertyIndex = 3;
        private const int breakWaterPropertyIndex = 4;
        private const int foreshoreGeometryPropertyIndex = 5;

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
            const string id = "VP";
            const string name = "Voorlandprofiel";
            var foreshoreProfile = new ForeshoreProfile(new Point2D(12.34, 56.78), new Point2D[0],
                                                        null, new ForeshoreProfile.ConstructionProperties
                                                        {
                                                            Id = id,
                                                            Name = name
                                                        });

            // Call
            var properties = new ForeshoreProfileProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            Assert.AreEqual(id, properties.Id);
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.AreSame(foreshoreProfile, properties.BreakWater.Data);
            Assert.AreEqual(foreshoreProfile, properties.Foreshore.Data);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            // Call
            var properties = new ForeshoreProfileProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(6, dynamicProperties.Count);

            PropertyDescriptor idProperty = dynamicProperties[idPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(idProperty,
                                                                            "Algemeen",
                                                                            "ID",
                                                                            "ID van het voorlandprofiel.",
                                                                            true);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            "Algemeen",
                                                                            "Naam",
                                                                            "Naam van het voorlandprofiel.",
                                                                            true);

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(worldReferencePointProperty,
                                                                            "Schematisatie",
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van het voorlandprofiel in het Rijksdriehoeksstelsel.",
                                                                            true);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(orientationProperty,
                                                                            "Schematisatie",
                                                                            "Oriëntatie [°]",
                                                                            "Oriëntatie van de dijknormaal ten opzichte van het noorden.",
                                                                            true);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterProperty,
                                                                            "Schematisatie",
                                                                            "Dam",
                                                                            "Eigenschappen van de dam.",
                                                                            true);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshoreGeometryPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProperty,
                                                                            "Schematisatie",
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);
        }
    }
}