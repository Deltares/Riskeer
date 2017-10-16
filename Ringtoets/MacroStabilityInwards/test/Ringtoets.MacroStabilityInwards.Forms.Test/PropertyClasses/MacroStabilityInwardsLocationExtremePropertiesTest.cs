// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsLocationExtremePropertiesTest
    {
        private const int expectedwaterLevelPolderPropertyIndex = 0;
        private const int expecteOffsetPropertyIndex = 1;
        private const int expectedPenetrationLengthPropertyIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsLocationInputExtreme();

            // Call
            var properties = new MacroStabilityInwardsLocationExtremeProperties(input, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsLocationInputExtreme>>(properties);
            Assert.AreSame(input, properties.Data);

            TestHelper.AssertTypeConverter<MacroStabilityInwardsLocationExtremeProperties, ExpandableObjectConverter>(
                nameof(MacroStabilityInwardsLocationExtremeProperties.Offsets));

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsLocationInputExtreme();

            // Call
            var properties = new MacroStabilityInwardsLocationExtremeProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string waterStressesCategory = "Waterspanningen";

            PropertyDescriptor waterLevelPolderProperty = dynamicProperties[expectedwaterLevelPolderPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterLevelPolderProperty,
                waterStressesCategory,
                "Polderpeil [m+NAP]",
                "Het niveau van het oppervlaktewater binnen een beheersgebied.");

            PropertyDescriptor offsetProperty = dynamicProperties[expecteOffsetPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                offsetProperty,
                waterStressesCategory,
                "Offsets PL 1",
                "Eigenschappen van offsets PL 1.",
                true);

            PropertyDescriptor penetrationLengthProperty = dynamicProperties[expectedPenetrationLengthPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                penetrationLengthProperty,
                waterStressesCategory,
                "Indringingslengte [m]",
                "De verticale afstand waarover de waterspanning in de deklaag verandert bij waterspanningsvariaties in de watervoerende zandlaag.");

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsLocationInputExtreme();

            // Call
            var properties = new MacroStabilityInwardsLocationExtremeProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.WaterLevelPolder, properties.WaterLevelPolder);
            Assert.AreSame(input, properties.Offsets.Data);
            Assert.AreEqual(input.PenetrationLength, properties.PenetrationLength);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var input = (MacroStabilityInwardsLocationInputExtreme) calculationItem.InputParameters.LocationInputExtreme;

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new MacroStabilityInwardsLocationExtremeProperties(input, handler);

            var random = new Random();
            double waterLevelPolder = random.Next();
            double penetrationLength = random.Next();

            // When
            properties.WaterLevelPolder = (RoundedDouble) waterLevelPolder;
            properties.PenetrationLength = (RoundedDouble) penetrationLength;

            // Then
            Assert.AreEqual(waterLevelPolder, input.WaterLevelPolder,
                            input.WaterLevelPolder.GetAccuracy());
            Assert.AreEqual(penetrationLength, input.PenetrationLength,
                            input.PenetrationLength.GetAccuracy());
        }

        [Test]
        public void WaterLevelPolder_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.WaterLevelPolder = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void PenetrationLength_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.PenetrationLength = (RoundedDouble) 1, calculation);
        }

        private static void SetPropertyAndVerifyNotifcationsForCalculation(Action<MacroStabilityInwardsLocationExtremeProperties> setProperty,
                                                                           MacroStabilityInwardsCalculation calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            MacroStabilityInwardsInput input = calculation.InputParameters;

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsLocationExtremeProperties((MacroStabilityInwardsLocationInputExtreme) input.LocationInputExtreme, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}