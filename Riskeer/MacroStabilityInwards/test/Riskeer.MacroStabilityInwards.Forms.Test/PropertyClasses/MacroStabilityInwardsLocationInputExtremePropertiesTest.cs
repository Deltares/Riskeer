﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Gui.Attributes;
using Core.Gui.PropertyBag;
using Core.Gui.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsLocationInputExtremePropertiesTest
    {
        private const int expectedWaterLevelPolderPropertyIndex = 0;
        private const int expectedOffsetPropertyIndex = 1;
        private const int expectedPenetrationLengthPropertyIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsLocationInputExtremeProperties(input, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsLocationInputExtreme>>(properties);
            Assert.AreSame(input.LocationInputExtreme, properties.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsLocationInputExtremeProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string waterStressesCategory = "Waterspanningen";

            PropertyDescriptor waterLevelPolderProperty = dynamicProperties[expectedWaterLevelPolderPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterLevelPolderProperty,
                waterStressesCategory,
                "Polderpeil [m+NAP]",
                "Het niveau van het oppervlaktewater binnen een beheersgebied.");

            PropertyDescriptor offsetProperty = dynamicProperties[expectedOffsetPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                offsetProperty,
                waterStressesCategory,
                "Offsets PL 1",
                "Eigenschappen van offsets PL 1.",
                true);

            PropertyDescriptor penetrationLengthProperty = dynamicProperties[expectedPenetrationLengthPropertyIndex];
            Assert.IsNotNull(penetrationLengthProperty.Attributes[typeof(DynamicReadOnlyAttribute)]);
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

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsLocationInputExtremeProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.LocationInputExtreme.PenetrationLength, properties.PenetrationLength);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculation = new MacroStabilityInwardsCalculationScenario();
            MacroStabilityInwardsInput input = calculation.InputParameters;

            var handler = new ObservablePropertyChangeHandler(calculation, calculation.InputParameters);
            var properties = new MacroStabilityInwardsLocationInputExtremeProperties(input, handler);

            var random = new Random(21);
            double penetrationLength = random.NextDouble();

            // When
            properties.PenetrationLength = (RoundedDouble) penetrationLength;

            // Then
            Assert.AreEqual(penetrationLength, input.LocationInputExtreme.PenetrationLength,
                            input.LocationInputExtreme.PenetrationLength.GetAccuracy());
        }

        [Test]
        public void PenetrationLength_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var random = new Random(21);
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PenetrationLength = random.NextRoundedDouble(), calculation);
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, false)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, false)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, true)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, false)]
        public void DynamicReadOnlyValidationMethod_GivenDikeSoilScenario_ReturnsExpectedReadOnly(
            MacroStabilityInwardsDikeSoilScenario dikeSoilScenario, bool expectedReadOnly)
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    DikeSoilScenario = dikeSoilScenario
                }
            };

            var handler = new ObservablePropertyChangeHandler(calculation, calculation.InputParameters);
            var properties = new MacroStabilityInwardsLocationInputExtremeProperties(calculation.InputParameters, handler);

            // Call & Assert
            Assert.AreEqual(expectedReadOnly, properties.DynamicReadOnlyValidationMethod(nameof(MacroStabilityInwardsLocationInputExtremeProperties.PenetrationLength)));
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(Action<MacroStabilityInwardsLocationInputExtremeProperties> setProperty,
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

            var properties = new MacroStabilityInwardsLocationInputExtremeProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}