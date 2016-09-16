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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveConditionsInputBreakWaterPropertiesTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var properties = new WaveConditionsInputBreakWaterProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WaveConditionsInput>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone);
            var properties = new WaveConditionsInputBreakWaterProperties();

            // Call
            properties.Data = input;

            // Assert
            Assert.IsFalse(properties.UseBreakWater);
            Assert.AreEqual(BreakWaterType.Dam, properties.BreakWaterType);
            Assert.AreEqual(0.0, properties.BreakWaterHeight.Value);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 3;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mockRepository.ReplayAll();

            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone);
            var properties = new WaveConditionsInputBreakWaterProperties
            {
                Data = input
            };

            input.Attach(observerMock);

            RoundedDouble newBreakWaterHeight = new RoundedDouble(2, 9);
            const BreakWaterType newBreakWaterType = BreakWaterType.Wall;

            // Call
            properties.BreakWaterHeight = newBreakWaterHeight;
            properties.BreakWaterType = newBreakWaterType;
            properties.UseBreakWater = false;

            // Assert
            Assert.IsFalse(input.UseBreakWater);
            Assert.AreEqual(newBreakWaterType, input.BreakWater.Type);
            Assert.AreEqual(newBreakWaterHeight, input.BreakWater.Height);
            mockRepository.VerifyAll();
        }

        [TestCase(true, true, TestName = "Property_ForeshoreProfileAndUseBreakWaterState_ReturnValues(true, true)")]
        [TestCase(true, false, TestName = "Property_ForeshoreProfileAndUseBreakWaterState_ReturnValues(true, false)")]
        [TestCase(false, true, TestName = "Property_ForeshoreProfileAndUseBreakWaterState_ReturnValues(false, true)")]
        [TestCase(false, false, TestName = "Property_ForeshoreProfileAndUseBreakWaterState_ReturnValues(false, false)")]
        public void PropertyAttributes_SpecificForeshoreProfileAndUseBreakWaterState_ReturnExpectedValues(bool useBreakWaterState, bool useForeshoreProfile)
        {
            // Setup
            var input = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone);

            BreakWater breakWater = null;
            if (useBreakWaterState)
            {
                breakWater = new BreakWater(BreakWaterType.Caisson, 1.1);
            }
            if (useForeshoreProfile)
            {
                input.ForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0), Enumerable.Empty<Point2D>(),
                                                              breakWater, new ForeshoreProfile.ConstructionProperties());
            }

            // Precondition
            Assert.AreEqual(useBreakWaterState && useForeshoreProfile, input.UseBreakWater);

            // Call
            var properties = new WaveConditionsInputBreakWaterProperties
            {
                Data = input
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(3, dynamicProperties.Count);

            PropertyDescriptor useBreakWaterProperty = dynamicProperties[0];
            Assert.IsNotNull(useBreakWaterProperty);
            Assert.AreEqual(!useForeshoreProfile, useBreakWaterProperty.IsReadOnly);
            Assert.AreEqual("Gebruik", useBreakWaterProperty.DisplayName);
            Assert.AreEqual("Moet de dam worden gebruikt tijdens de berekening?", useBreakWaterProperty.Description);

            PropertyDescriptor breakWaterTypeProperty = dynamicProperties[1];
            Assert.IsNotNull(breakWaterTypeProperty);
            Assert.AreEqual(!(useBreakWaterState && useForeshoreProfile), breakWaterTypeProperty.IsReadOnly);
            Assert.AreEqual("Type", breakWaterTypeProperty.DisplayName);
            Assert.AreEqual("Het type van de dam.", breakWaterTypeProperty.Description);

            PropertyDescriptor breakWaterHeightProperty = dynamicProperties[2];
            Assert.IsNotNull(breakWaterHeightProperty);
            Assert.AreEqual(!(useBreakWaterState && useForeshoreProfile), breakWaterHeightProperty.IsReadOnly);
            Assert.AreEqual("Hoogte [m+NAP]", breakWaterHeightProperty.DisplayName);
            Assert.AreEqual("De hoogte van de dam.", breakWaterHeightProperty.Description);
        }
    }
}