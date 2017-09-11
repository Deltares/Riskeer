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
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Common.Utils;
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
    public class MacroStabilityInwardsSlipPlaneSettingsPropertiesTest
    {
        private const int expectedCreateZonesPropertyIndex = 0;
        private const int expectedAutomaticForbiddenZonesPropertyIndex = 1;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput();

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
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlipPlaneSettingsProperties(new MacroStabilityInwardsInput(),
                                                                                           null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput();

            // Call
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

            const string category = "Zonering glijvlak";

            PropertyDescriptor createZonesProperty = dynamicProperties[expectedCreateZonesPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                createZonesProperty,
                category,
                "Bepaling",
                "Gebruik zoneringsgrenzen bij het bepalen van het intredepunt van het glijvlak?");

            PropertyDescriptor automaticForbiddenZonesProperty = dynamicProperties[expectedAutomaticForbiddenZonesPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsSlipPlaneSettingsProperties, EnumTypeConverter>(nameof(properties.AutomaticForbiddenZones));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                automaticForbiddenZonesProperty,
                category,
                "Methode",
                "Zoneringsgrenzen automatisch bepalen of handmatig invoeren?",
                true);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput();

            // Call
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.CreateZones, properties.CreateZones);
            Assert.AreEqual(input.AutomaticForbiddenZones, properties.AutomaticForbiddenZones);
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

            var random = new Random();
            bool createZones = random.NextBoolean();

            // When
            properties.CreateZones = createZones;

            // Then
            Assert.AreEqual(createZones, input.CreateZones);
        }

        [Test]
        public void CreateZones_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, handler);

            // Call
            properties.CreateZones = false;

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput();
            var properties = new MacroStabilityInwardsSlipPlaneSettingsProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }
    }
}