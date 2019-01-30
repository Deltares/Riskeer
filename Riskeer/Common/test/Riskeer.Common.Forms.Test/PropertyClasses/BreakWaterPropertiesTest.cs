// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PropertyClasses;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class BreakWaterPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var properties = new BreakWaterProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<ForeshoreProfile>>(properties);
            Assert.IsNull(properties.Data);
            Assert.IsEmpty(properties.ToString());
        }

        [Test]
        public void Data_SetForeshoreProfileInstanceWithBreakWater_ReturnCorrectPropertyValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Wall, 10.0));

            var properties = new BreakWaterProperties();

            // Call
            properties.Data = foreshoreProfile;

            // Assert
            Assert.IsTrue(properties.HasBreakWater);
            Assert.AreEqual(BreakWaterType.Wall, properties.BreakWaterType);
            Assert.AreEqual(10.0, properties.BreakWaterHeight.Value);
        }

        [Test]
        public void Data_SetForeshoreProfileInstanceWithoutBreakWater_ReturnCorrectPropertyValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            var properties = new BreakWaterProperties();

            // Call
            properties.Data = foreshoreProfile;

            // Assert
            Assert.IsFalse(properties.HasBreakWater);
        }

        [Test]
        public void PropertyAttributes_SetForeshoreProfileInstanceWithBreakWater_ReturnExpectedValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 10.0));

            // Call
            var properties = new BreakWaterProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            "Misc",
                                                                            "Aanwezig",
                                                                            "Is er een dam aanwezig?",
                                                                            true);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterTypeProperty,
                                                                            "Misc",
                                                                            "Type",
                                                                            "Het type van de dam.",
                                                                            true);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(breakWaterHeightProperty,
                                                                            "Misc",
                                                                            "Hoogte [m+NAP]",
                                                                            "De hoogte van de dam.",
                                                                            true);
        }

        [Test]
        public void PropertyAttributes_SetForeshoreProfileInstanceWithoutBreakWater_ReturnExpectedValues()
        {
            // Setup
            var foreshoreProfile = new TestForeshoreProfile();

            // Call
            var properties = new BreakWaterProperties
            {
                Data = foreshoreProfile
            };

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(1, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useBreakWaterProperty,
                                                                            "Misc",
                                                                            "Aanwezig",
                                                                            "Is er een dam aanwezig?",
                                                                            true);
        }
    }
}