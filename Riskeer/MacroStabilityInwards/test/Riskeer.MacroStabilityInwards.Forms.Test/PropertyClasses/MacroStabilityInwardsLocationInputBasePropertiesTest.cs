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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsLocationInputBasePropertiesTest
    {
        private const int expectedwaterLevelPolderPropertyIndex = 0;
        private const int expecteOffsetPropertyIndex = 1;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput();

            // Call
            var properties = new TestMacroStabilityInwardsLocationProperties(input, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsLocationInputBase>>(properties);
            Assert.AreSame(input, properties.Data);

            TestHelper.AssertTypeConverter<TestMacroStabilityInwardsLocationProperties, ExpandableObjectConverter>(
                nameof(TestMacroStabilityInwardsLocationProperties.Offsets));

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
            TestDelegate call = () => new TestMacroStabilityInwardsLocationProperties(null, changeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestMacroStabilityInwardsLocationProperties(new TestMacroStabilityInwardsLocationInput(),
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

            var input = new TestMacroStabilityInwardsLocationInput();

            // Call
            var properties = new TestMacroStabilityInwardsLocationProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(2, dynamicProperties.Count);

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

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput();

            // Call
            var properties = new TestMacroStabilityInwardsLocationProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.WaterLevelPolder, properties.WaterLevelPolder);
            Assert.AreSame(input, properties.Offsets.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var input = new TestMacroStabilityInwardsLocationInput();

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new TestMacroStabilityInwardsLocationProperties(input, handler);

            var random = new Random(21);
            double waterLevelPolder = random.NextDouble();

            // When
            properties.WaterLevelPolder = (RoundedDouble) waterLevelPolder;

            // Then
            Assert.AreEqual(waterLevelPolder, input.WaterLevelPolder,
                            input.WaterLevelPolder.GetAccuracy());
        }

        [Test]
        public void WaterLevelPolder_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.WaterLevelPolder = (RoundedDouble) 1);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput();
            var properties = new TestMacroStabilityInwardsLocationProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(Action<TestMacroStabilityInwardsLocationProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new TestMacroStabilityInwardsLocationProperties(new TestMacroStabilityInwardsLocationInput(), handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private class TestMacroStabilityInwardsLocationProperties : MacroStabilityInwardsLocationInputBaseProperties<MacroStabilityInwardsLocationInputBase>
        {
            public TestMacroStabilityInwardsLocationProperties(MacroStabilityInwardsLocationInputBase data,
                                                               IObservablePropertyChangeHandler handler)
                : base(data, handler) {}
        }

        private class TestMacroStabilityInwardsLocationInput : MacroStabilityInwardsLocationInputBase {}
    }
}