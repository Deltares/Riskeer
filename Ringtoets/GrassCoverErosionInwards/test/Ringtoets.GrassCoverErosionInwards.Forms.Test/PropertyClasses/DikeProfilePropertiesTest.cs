﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DikeProfilePropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int worldReferencePointPropertyIndex = 1;
        private const int orientationPropertyIndex = 2;
        private const int breakWaterPropertyIndex = 3;
        private const int foreshorePropertyIndex = 4;
        private const int dikeGeometryPropertyIndex = 5;
        private const int dikeHeightPropertyIndex = 6;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var properties = new DikeProfileProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DikeProfile>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void Data_SetNewDikeProfileInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            const string name = "Dijkprofiel";
            var dikeProfile = new DikeProfile(new Point2D(12.34, 56.78), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties
                                              {
                                                  Name = name
                                              });

            // Call
            var properties = new DikeProfileProperties
            {
                Data = dikeProfile
            };

            // Assert
            Assert.AreEqual(new Point2D(12, 57), properties.WorldReferencePoint);
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.AreSame(dikeProfile.ForeshoreProfile, properties.BreakWater.Data);
            Assert.AreSame(dikeProfile.ForeshoreProfile, properties.Foreshore.Data);
            Assert.AreSame(dikeProfile, properties.DikeGeometry.Data);
            Assert.AreEqual(2, properties.DikeHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.DikeHeight.Value);
        }

        [Test]
        public void PropertyAttributes_ReturnExpectedValues()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());

            // Call
            var properties = new DikeProfileProperties
            {
                Data = dikeProfile
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("Naam van het dijkprofiel.", nameProperty.Description);

            PropertyDescriptor worldReferencePointProperty = dynamicProperties[worldReferencePointPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(worldReferencePointProperty,
                                                                            "Schematisatie",
                                                                            "Locatie (RD) [m]",
                                                                            "De coördinaten van de locatie van de dijk in het Rijksdriehoeksstelsel.",
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

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(foreshoreProperty,
                                                                            "Schematisatie",
                                                                            "Voorlandgeometrie",
                                                                            "Eigenschappen van de voorlandgeometrie.",
                                                                            true);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsInstanceOf<ExpandableObjectConverter>(dikeGeometryProperty.Converter);
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeGeometryProperty,
                                                                            "Schematisatie",
                                                                            "Dijkgeometrie",
                                                                            "Eigenschappen van de dijkgeometrie.",
                                                                            true);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(dikeHeightProperty,
                                                                            "Schematisatie",
                                                                            "Dijkhoogte [m+NAP]",
                                                                            "De hoogte van de dijk.",
                                                                            true);
        }
    }
}