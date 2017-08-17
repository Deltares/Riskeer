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
    public class MacroStabilityInwardsWaterStressesPropertiesTest
    {
        private const int expectedWaterLevelRiverAveragePropertyIndex = 0;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Call
            var properties = new MacroStabilityInwardsWaterStressesProperties(input, changeHandler);

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
            TestDelegate call = () => new MacroStabilityInwardsWaterStressesProperties(null, changeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsWaterStressesProperties(new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput()),
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

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Call
            var properties = new MacroStabilityInwardsWaterStressesProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(1, dynamicProperties.Count);

            const string waterStressesCategory = "Waterspanningen";

            PropertyDescriptor waterLevelRiverAverageProperty = dynamicProperties[expectedWaterLevelRiverAveragePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                waterLevelRiverAverageProperty,
                waterStressesCategory,
                "Gemiddeld hoog water (GHW) [m+NAP]",
                "Gemiddeld hoog water.");

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
            var properties = new MacroStabilityInwardsWaterStressesProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.WaterLevelRiverAverage, properties.WaterLevelRiverAverage);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            MacroStabilityInwardsInput input = calculationItem.InputParameters;

            var handler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsWaterStressesProperties(input, handler);

            const double waterLevelRiverAverage = 12;

            // When
            properties.WaterLevelRiverAverage = (RoundedDouble) waterLevelRiverAverage;

            // Then
            Assert.AreEqual(waterLevelRiverAverage, input.WaterLevelRiverAverage.Value);
        }

        [Test]
        public void WaterLevelRiverAverage_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.WaterLevelRiverAverage = (RoundedDouble) 1, calculation);
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

        private void SetPropertyAndVerifyNotifcationsForCalculation(Action<MacroStabilityInwardsWaterStressesProperties> setProperty,
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

            var properties = new MacroStabilityInwardsWaterStressesProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}