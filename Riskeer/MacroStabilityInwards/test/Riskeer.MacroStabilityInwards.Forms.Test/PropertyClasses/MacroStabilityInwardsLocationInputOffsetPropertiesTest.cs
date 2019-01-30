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
    public class MacroStabilityInwardsLocationInputOffsetPropertiesTest
    {
        private const int expectedUseDefaultOffsetsPropertyIndex = 0;
        private const int expectedPhreaticLineOffsetBelowDikeTopAtRiverPropertyIndex = 1;
        private const int expectedPhreaticLineOffsetBelowDikeTopAtPolderPropertyIndex = 2;
        private const int expectedPhreaticLineOffsetBelowShoulderBaseInsidePropertyIndex = 3;
        private const int expectedPhreaticLineOffsetBelowDikeToeAtPolderPropertyIndex = 4;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput();

            // Call
            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, changeHandler);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsLocationInputBase>>(properties);
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
            TestDelegate call = () => new MacroStabilityInwardsLocationInputOffsetProperties(null, changeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsLocationInputOffsetProperties(new TestMacroStabilityInwardsLocationInput(),
                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues(bool useDefaultOffsets)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput
            {
                UseDefaultOffsets = useDefaultOffsets
            };

            // Call
            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(5, dynamicProperties.Count);

            const string offsetCategory = "Offsets PL 1";

            PropertyDescriptor useDefaultOffsetsProperty = dynamicProperties[expectedUseDefaultOffsetsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                useDefaultOffsetsProperty,
                offsetCategory,
                "Gebruik default waarden voor offsets van PL 1",
                "Gebruik standaard waterstandsverschillen voor het bepalen van de freatische lijn?");

            PropertyDescriptor phreaticLineOffsetBelowDikeTopAtRiverProperty = dynamicProperties[expectedPhreaticLineOffsetBelowDikeTopAtRiverPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                phreaticLineOffsetBelowDikeTopAtRiverProperty,
                offsetCategory,
                "PL 1 offset onder buitenkruin [m]",
                "Verschil tussen de waterstand en de freatische lijn onder kruin buitentalud.",
                useDefaultOffsets);

            PropertyDescriptor phreaticLineOffsetBelowDikeTopAtPolderProperty = dynamicProperties[expectedPhreaticLineOffsetBelowDikeTopAtPolderPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                phreaticLineOffsetBelowDikeTopAtPolderProperty,
                offsetCategory,
                "PL 1 offset onder binnenkruin [m]",
                "Verschil tussen de waterstand en de freatische lijn onder kruin binnentalud.",
                useDefaultOffsets);

            PropertyDescriptor phreaticLineOffsetBelowShoulderBaseInsideProperty = dynamicProperties[expectedPhreaticLineOffsetBelowShoulderBaseInsidePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                phreaticLineOffsetBelowShoulderBaseInsideProperty,
                offsetCategory,
                "PL 1 offset onder insteek binnenberm [m]",
                "Waterstandsverschil tussen het maaiveld en de freatische lijn onder insteek binnenberm.",
                useDefaultOffsets);

            PropertyDescriptor phreaticLineOffsetBelowDikeToeAtPolderProperty = dynamicProperties[expectedPhreaticLineOffsetBelowDikeToeAtPolderPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                phreaticLineOffsetBelowDikeToeAtPolderProperty,
                offsetCategory,
                "PL 1 offset onder teen dijk binnenwaarts [m]",
                "Waterstandsverschil tussen het maaiveld en de freatische lijn onder teen dijk binnenwaarts.",
                useDefaultOffsets);

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
            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.UseDefaultOffsets, properties.UseDefaultOffsets);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeTopAtRiver, properties.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeTopAtPolder, properties.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(input.PhreaticLineOffsetBelowShoulderBaseInside, properties.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(input.PhreaticLineOffsetBelowDikeToeAtPolder, properties.PhreaticLineOffsetBelowDikeToeAtPolder);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            var input = new TestMacroStabilityInwardsLocationInput();

            var handler = new ObservablePropertyChangeHandler(calculationItem, calculationItem.InputParameters);
            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, handler);

            var random = new Random(21);
            bool useDefaultOffsets = random.NextBoolean();
            double phreaticLineOffsetBelowDikeTopAtRiver = random.NextDouble();
            double phreaticLineOffsetBelowDikeTopAtPolder = random.NextDouble();
            double phreaticLineOffsetBelowShoulderBaseInside = random.NextDouble();
            double phreaticLineOffsetBelowDikeToeAtPolder = random.NextDouble();

            // When
            properties.UseDefaultOffsets = useDefaultOffsets;
            properties.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtRiver;
            properties.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeTopAtPolder;
            properties.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) phreaticLineOffsetBelowShoulderBaseInside;
            properties.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) phreaticLineOffsetBelowDikeToeAtPolder;

            // Then
            Assert.AreEqual(useDefaultOffsets, input.UseDefaultOffsets);
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtRiver, input.PhreaticLineOffsetBelowDikeTopAtRiver,
                            input.PhreaticLineOffsetBelowDikeTopAtRiver.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowDikeTopAtPolder, input.PhreaticLineOffsetBelowDikeTopAtPolder,
                            input.PhreaticLineOffsetBelowDikeTopAtPolder.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowShoulderBaseInside, input.PhreaticLineOffsetBelowShoulderBaseInside,
                            input.PhreaticLineOffsetBelowShoulderBaseInside.GetAccuracy());
            Assert.AreEqual(phreaticLineOffsetBelowDikeToeAtPolder, input.PhreaticLineOffsetBelowDikeToeAtPolder,
                            input.PhreaticLineOffsetBelowDikeToeAtPolder.GetAccuracy());
        }

        [Test]
        public void UseDefaultOffsets_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.UseDefaultOffsets = true);
        }

        [Test]
        public void PhreaticLineOffsetBelowDikeTopAtRiver_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 1);
        }

        [Test]
        public void PhreaticLineOffsetBelowDikeTopAtPolder_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 1);
        }

        [Test]
        public void PhreaticLineOffsetBelowShoulderBaseInside_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 1);
        }

        [Test]
        public void PhreaticLineOffsetBelowDikeToeAtPolder_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 1);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnlyValidationMethod_Always_DependsUseDefaultOffsets(bool useDefaultOffsets)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput
            {
                UseDefaultOffsets = useDefaultOffsets
            };

            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, handler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod("");

            // Assert
            Assert.AreEqual(useDefaultOffsets, result);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput();
            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(Action<MacroStabilityInwardsLocationInputOffsetProperties> setProperty)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var input = new TestMacroStabilityInwardsLocationInput();

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsLocationInputOffsetProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private class TestMacroStabilityInwardsLocationInput : MacroStabilityInwardsLocationInputBase {}
    }
}