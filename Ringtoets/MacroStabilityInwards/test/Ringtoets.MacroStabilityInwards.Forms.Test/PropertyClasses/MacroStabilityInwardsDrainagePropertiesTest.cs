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
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsDrainagePropertiesTest
    {
        private const int expectedDrainageConstructionPresentPropertyIndex = 0;
        private const int expectedXCoordinateDrainageConstructionPropertyIndex = 1;
        private const int expectedZCoordinateDrainageConstructionPropertyIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Call
            var properties = new MacroStabilityInwardsDrainageProperties(input, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsInput>>(properties);
            Assert.AreSame(input, properties.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsDrainageProperties(null, changeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsDrainageProperties(new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput()),
                                                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues(bool drainageConstructionPresent)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                DrainageConstructionPresent = drainageConstructionPresent
            };

            // Call
            var properties = new MacroStabilityInwardsDrainageProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(3, dynamicProperties.Count);

            const string drainageCategory = "Drainage";

            PropertyDescriptor drainagePresentProperty = dynamicProperties[expectedDrainageConstructionPresentPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainagePresentProperty,
                drainageCategory,
                "Aanwezig",
                "Is drainage aanwezig?");

            PropertyDescriptor drainageXProperty = dynamicProperties[expectedXCoordinateDrainageConstructionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainageXProperty,
                drainageCategory,
                "X [m]",
                "X-coordinaat van het middelpunt van de drainage (in lokale coordinaten).",
                !drainageConstructionPresent);

            PropertyDescriptor drainageZProperty = dynamicProperties[expectedZCoordinateDrainageConstructionPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                drainageZProperty,
                drainageCategory,
                "Z [m+NAP]",
                "Z-coordinaat (hoogte) van het middelpunt van de drainage.",
                !drainageConstructionPresent);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Call
            var properties = new MacroStabilityInwardsDrainageProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.DrainageConstructionPresent, properties.DrainageConstructionPresent);
            Assert.AreEqual(input.XCoordinateDrainageConstruction, properties.XCoordinateDrainageConstruction);
            Assert.AreEqual(input.ZCoordinateDrainageConstruction, properties.ZCoordinateDrainageConstruction);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            MacroStabilityInwardsInput input = calculationItem.InputParameters;

            var handler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsDrainageProperties(input, handler);

            var random = new Random();
            bool constructionPresent = random.NextBoolean();
            double xCoordinateDrainageConstruction = random.Next();
            double zCoordinateDrainageConstruction = random.Next();

            // When
            properties.DrainageConstructionPresent = constructionPresent;
            properties.XCoordinateDrainageConstruction = (RoundedDouble) xCoordinateDrainageConstruction;
            properties.ZCoordinateDrainageConstruction = (RoundedDouble) zCoordinateDrainageConstruction;

            // Then
            Assert.AreEqual(constructionPresent, input.DrainageConstructionPresent);
            Assert.AreEqual(xCoordinateDrainageConstruction, input.XCoordinateDrainageConstruction.Value);
            Assert.AreEqual(zCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction.Value);
        }

        [Test]
        public void DrainageConstructionPresent_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.DrainageConstructionPresent = true, calculation);
        }

        [Test]
        public void XCoordinateDrainageConstruction_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.XCoordinateDrainageConstruction = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ZCoordinateDrainageConstruction_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.ZCoordinateDrainageConstruction = (RoundedDouble) 1, calculation);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnlyValidationMethod_Always_DependsDrainageConstructionPresent(bool drainageConstructionPresent)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput())
            {
                DrainageConstructionPresent = drainageConstructionPresent
            };

            var properties = new MacroStabilityInwardsDrainageProperties(input, handler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod("AssessmentLevel");

            // Assert
            Assert.AreNotEqual(drainageConstructionPresent, result);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());
            var properties = new MacroStabilityInwardsWaterStressesProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        private static void SetPropertyAndVerifyNotifcationsForCalculation(Action<MacroStabilityInwardsDrainageProperties> setProperty,
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

            var properties = new MacroStabilityInwardsDrainageProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}