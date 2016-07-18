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
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DikeProfilePropertiesTest
    {
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
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
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
            Assert.AreEqual(name, properties.Name);
            Assert.AreEqual(2, properties.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, properties.Orientation.Value);
            Assert.AreSame(dikeProfile, properties.BreakWater.Data);
            Assert.AreSame(dikeProfile, properties.Foreshore.Data);
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
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(7, dynamicProperties.Count);

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            Assert.IsNotNull(nameProperty);
            Assert.IsTrue(nameProperty.IsReadOnly);
            Assert.AreEqual("Algemeen", nameProperty.Category);
            Assert.AreEqual("Naam", nameProperty.DisplayName);
            Assert.AreEqual("Naam van het dijkprofiel.", nameProperty.Description);

            PropertyDescriptor orientationProperty = dynamicProperties[orientationPropertyIndex];
            Assert.IsNotNull(orientationProperty);
            Assert.IsTrue(orientationProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", orientationProperty.Category);
            Assert.AreEqual("Oriëntatie [°]", orientationProperty.DisplayName);
            Assert.AreEqual("Oriëntatie van de dijk.", orientationProperty.Description);

            PropertyDescriptor breakWaterProperty = dynamicProperties[breakWaterPropertyIndex];
            Assert.IsNotNull(breakWaterProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(breakWaterProperty.Converter);
            Assert.IsTrue(breakWaterProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", breakWaterProperty.Category);
            Assert.AreEqual("Dam", breakWaterProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dam.", breakWaterProperty.Description);

            PropertyDescriptor foreshoreProperty = dynamicProperties[foreshorePropertyIndex];
            Assert.IsNotNull(foreshoreProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(foreshoreProperty.Converter);
            Assert.IsTrue(foreshoreProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", foreshoreProperty.Category);
            Assert.AreEqual("Voorlandgeometrie", foreshoreProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de voorlandgeometrie.", foreshoreProperty.Description);

            PropertyDescriptor dikeGeometryProperty = dynamicProperties[dikeGeometryPropertyIndex];
            Assert.IsNotNull(dikeGeometryProperty);
            Assert.IsInstanceOf<ExpandableObjectConverter>(dikeGeometryProperty.Converter);
            Assert.IsTrue(dikeGeometryProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeGeometryProperty.Category);
            Assert.AreEqual("Dijkgeometrie", dikeGeometryProperty.DisplayName);
            Assert.AreEqual("Eigenschappen van de dijkgeometrie.", dikeGeometryProperty.Description);

            PropertyDescriptor dikeHeightProperty = dynamicProperties[dikeHeightPropertyIndex];
            Assert.IsNotNull(dikeHeightProperty);
            Assert.IsTrue(dikeHeightProperty.IsReadOnly);
            Assert.AreEqual("Schematisatie", dikeHeightProperty.Category);
            Assert.AreEqual("Dijkhoogte [m+NAP]", dikeHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dijk [m+NAP].", dikeHeightProperty.Description);
        }

        private const int namePropertyIndex = 0;
        private const int orientationPropertyIndex = 1;
        private const int breakWaterPropertyIndex = 2;
        private const int foreshorePropertyIndex = 3;
        private const int dikeGeometryPropertyIndex = 4;
        private const int dikeHeightPropertyIndex = 5;
    }
}