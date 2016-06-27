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
    public class DikeProfileBreakWaterPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var properties = new DikeProfileBreakWaterProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<DikeProfile>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Data_SetDikeProfileInstanceWithBreakWater_ReturnCorrectPropertyValues()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0])
            {
                BreakWater = new BreakWater(BreakWaterType.Wall, 10.0)
            };

            var properties = new DikeProfileBreakWaterProperties();

            // Call
            properties.Data = dikeProfile;

            // Assert
            Assert.IsTrue(properties.HasBreakWater);
            Assert.AreEqual(BreakWaterType.Wall, properties.BreakWaterType);
            Assert.AreEqual(10.0, properties.BreakWaterHeight.Value);
        }

        [Test]
        public void Data_SetDikeProfileInstanceWithoutBreakWater_ReturnCorrectPropertyValues()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            var properties = new DikeProfileBreakWaterProperties();

            // Call
            properties.Data = dikeProfile;

            // Assert
            Assert.IsFalse(properties.HasBreakWater);
        }

        [Test]
        public void PropertyAttributes_SetDikeProfileInstanceWithBreakWater_ReturnExpectedValues()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0])
            {
                BreakWater = new BreakWater(BreakWaterType.Caisson, 10.0)
            };

            // Call
            var properties = new DikeProfileBreakWaterProperties
            {
                Data = dikeProfile
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            Assert.IsNotNull(useBreakWaterProperty);
            Assert.IsTrue(useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual("Aanwezig", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Is er een dam aanwezig?", useBreakWaterProperty.Description);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsNotNull(breakWaterTypeProperty);
            Assert.IsTrue(breakWaterTypeProperty.IsBrowsable);
            Assert.IsTrue(breakWaterTypeProperty.IsReadOnly);
            Assert.AreEqual("Type", breakWaterTypeProperty.DisplayName);
            Assert.AreEqual("Het type van de dam.", breakWaterTypeProperty.Description);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsNotNull(breakWaterHeightProperty);
            Assert.IsTrue(breakWaterHeightProperty.IsBrowsable);
            Assert.IsTrue(breakWaterHeightProperty.IsReadOnly);
            Assert.AreEqual("Hoogte [m+NAP]", breakWaterHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dam [m+NAP].", breakWaterHeightProperty.Description);
        }

        [Test]
        public void PropertyAttributes_SetDikeProfileInstanceWithoutBreakWater_ReturnExpectedValues()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0]);

            // Call
            var properties = new DikeProfileBreakWaterProperties
            {
                Data = dikeProfile
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties();
            Assert.AreEqual(4, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            Assert.IsNotNull(useBreakWaterProperty);
            Assert.IsTrue(useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual("Aanwezig", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Is er een dam aanwezig?", useBreakWaterProperty.Description);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsNotNull(breakWaterTypeProperty);
            Assert.IsFalse(breakWaterTypeProperty.IsBrowsable);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsNotNull(breakWaterHeightProperty);
            Assert.IsFalse(breakWaterHeightProperty.IsBrowsable);
        }
    }
}