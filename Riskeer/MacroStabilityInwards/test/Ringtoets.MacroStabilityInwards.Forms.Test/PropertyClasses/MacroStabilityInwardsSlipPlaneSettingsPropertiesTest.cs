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

using System;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Util;
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
    public class MacroStabilityInwardsSlipPlaneSettingsPropertiesTest
    {
        private const int expectedCreateZonesPropertyIndex = 0;
        private const int expectedZoningBoundariesDeterminationTypePropertyIndex = 1;
        private const int expectedZoneBoundayrLeftPropertyIndex = 2;
        private const int expectedZoneBoundaryRightPropertyIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

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
            TestDelegate call = () => new MacroStabilityInwardsSlipPlaneSettingsProperties(null, changeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlipPlaneSettingsProperties(input, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        [TestCase(true, MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, false, true)]
        [TestCase(true, MacroStabilityInwardsZoningBoundariesDeterminationType.Manual, false, false)]
        [TestCase(false, MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, true, true)]
        [TestCase(false, MacroStabilityInwardsZoningBoundariesDeterminationType.Manual, true, true)]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues(
            bool createZones,
            MacroStabilityInwardsZoningBoundariesDeterminationType zoningBoundariesDeterminationType,
            bool expectedDeterminationTypeReadOnly,
            bool expectedZoneBoundariesReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                CreateZones = createZones,
                ZoningBoundariesDeterminationType = zoningBoundariesDeterminationType
            };

            // Call
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(4, dynamicProperties.Count);

            const string category = "Zonering glijvlak";

            PropertyDescriptor createZonesProperty = dynamicProperties[expectedCreateZonesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                createZonesProperty,
                category,
                "Bepaling",
                "Gebruik zoneringsgrenzen bij het bepalen van het intredepunt van het glijvlak?");

            PropertyDescriptor zoningBoundariesDeterminationTypeProperty = dynamicProperties[expectedZoningBoundariesDeterminationTypePropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsSlipPlaneSettingsProperties, EnumTypeConverter>(nameof(properties.ZoningBoundariesDeterminationType));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                zoningBoundariesDeterminationTypeProperty,
                category,
                "Methode",
                "Zoneringsgrenzen automatisch bepalen of handmatig invoeren?",
                expectedDeterminationTypeReadOnly);

            PropertyDescriptor zoneBoundaryLeftProperty = dynamicProperties[expectedZoneBoundayrLeftPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                zoneBoundaryLeftProperty,
                category,
                "Zoneringsgrens links",
                "Linker grens voor bepaling intredepunt van het glijvlak.",
                expectedZoneBoundariesReadOnly);

            PropertyDescriptor zoneBoundaryRightProperty = dynamicProperties[expectedZoneBoundaryRightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                zoneBoundaryRightProperty,
                category,
                "Zoneringsgrens rechts",
                "Rechter grens voor bepaling intredepunt van het glijvlak.",
                expectedZoneBoundariesReadOnly);

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
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.CreateZones, properties.CreateZones);
            Assert.AreEqual(input.ZoningBoundariesDeterminationType, properties.ZoningBoundariesDeterminationType);
            Assert.AreEqual(input.ZoneBoundaryLeft, properties.ZoneBoundaryLeft);
            Assert.AreEqual(input.ZoneBoundaryRight, properties.ZoneBoundaryRight);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            MacroStabilityInwardsInput input = calculationItem.InputParameters;

            var handler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, handler);

            var random = new Random(21);
            bool createZones = random.NextBoolean();
            var determinationType = random.NextEnumValue<MacroStabilityInwardsZoningBoundariesDeterminationType>();
            RoundedDouble boundaryLeft = random.NextRoundedDouble();
            RoundedDouble boundaryRight = random.NextRoundedDouble();

            // When
            properties.CreateZones = createZones;
            properties.ZoningBoundariesDeterminationType = determinationType;
            properties.ZoneBoundaryLeft = boundaryLeft;
            properties.ZoneBoundaryRight = boundaryRight;

            // Then
            Assert.AreEqual(createZones, input.CreateZones);
            Assert.AreEqual(determinationType, input.ZoningBoundariesDeterminationType);
            Assert.AreEqual(boundaryLeft, input.ZoneBoundaryLeft, input.ZoneBoundaryLeft.GetAccuracy());
            Assert.AreEqual(boundaryRight, input.ZoneBoundaryRight, input.ZoneBoundaryRight.GetAccuracy());
        }

        [Test]
        public void CreateZones_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotifications(properties => properties.CreateZones = false);
        }

        [Test]
        public void ZoningBoundariesDeterminationType_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotifications(properties => properties.ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual);
        }

        [Test]
        public void ZoneBoundaryLeft_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotifications(properties => properties.ZoneBoundaryLeft = new Random(39).NextRoundedDouble());
        }

        [Test]
        public void ZoneBoundaryRight_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotifications(properties => properties.ZoneBoundaryRight = new Random(39).NextRoundedDouble());
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        [Test]
        [TestCase(true, MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, false, true)]
        [TestCase(true, MacroStabilityInwardsZoningBoundariesDeterminationType.Manual, false, false)]
        [TestCase(false, MacroStabilityInwardsZoningBoundariesDeterminationType.Automatic, true, true)]
        [TestCase(false, MacroStabilityInwardsZoningBoundariesDeterminationType.Manual, true, true)]
        public void DynamicReadOnlyValidationMethod_WithCreateZonesAndZoningBoundariesDeterminationType_ReturnsExpectedValues(
            bool createZones,
            MacroStabilityInwardsZoningBoundariesDeterminationType zoningBoundariesDeterminationType,
            bool expectedDeterminationTypeReadOnly,
            bool expectedZoneBoundariesReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                CreateZones = createZones,
                ZoningBoundariesDeterminationType = zoningBoundariesDeterminationType
            };

            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Call
            bool isZoningBoundariesDeterminationTypeReadOnly = properties.DynamicReadOnlyValidationMethod(nameof(properties.ZoningBoundariesDeterminationType));
            bool isZoneBoundaryLeftReadOnly = properties.DynamicReadOnlyValidationMethod(nameof(properties.ZoneBoundaryLeft));
            bool isZoneBoundaryRightReadOnly = properties.DynamicReadOnlyValidationMethod(nameof(properties.ZoneBoundaryRight));

            // Assert
            Assert.AreEqual(expectedDeterminationTypeReadOnly, isZoningBoundariesDeterminationTypeReadOnly);
            Assert.AreEqual(expectedZoneBoundariesReadOnly, isZoneBoundaryLeftReadOnly);
            Assert.AreEqual(expectedZoneBoundariesReadOnly, isZoneBoundaryRightReadOnly);
        }

        [Test]
        public void DynamicReadOnlyValidationMethod_AnyOtherParameter_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Call
            bool isReadOnly = properties.DynamicReadOnlyValidationMethod("test parameter name 123");

            // Assert
            Assert.IsFalse(isReadOnly);
        }

        private static void SetPropertyAndVerifyNotifications(Action<MacroStabilityInwardsSlipPlaneSettingsProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}