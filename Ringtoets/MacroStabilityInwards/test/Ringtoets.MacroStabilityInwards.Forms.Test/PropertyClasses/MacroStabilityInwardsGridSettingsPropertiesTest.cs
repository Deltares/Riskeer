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
    public class MacroStabilityInwardsGridSettingsPropertiesTest
    {
        private const int expectedMoveGridPropertyIndex = 0;
        private const int expectedGridDeterminationPropertyIndex = 1;
        private const int expectedTangentLineDeterminationPropertyIndex = 2;
        private const int expectedTangentLineZTopPropertyIndex = 3;
        private const int expectedTangentLineZBottomPropertyIndex = 4;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());

            // Call
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

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
            TestDelegate call = () => new MacroStabilityInwardsGridSettingsProperties(null, changeHandler);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsGridSettingsProperties(new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput()),
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
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(5, dynamicProperties.Count);

            const string calculationGridsCategory = "Rekengrids";

            PropertyDescriptor moveGridProperty = dynamicProperties[expectedMoveGridPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                moveGridProperty,
                calculationGridsCategory,
                "Verplaats grid",
                "Sta automatische aanpassing van het grid toe?");

            PropertyDescriptor gridDeterminationProperty = dynamicProperties[expectedGridDeterminationPropertyIndex];
            Assert.AreEqual(typeof(EnumTypeConverter), gridDeterminationProperty.Converter.GetType());
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                gridDeterminationProperty,
                calculationGridsCategory,
                "Bepaling grid",
                "Rekengrid automatisch bepalen of handmatig invoeren?");

            PropertyDescriptor tangentLineDeterminationProperty = dynamicProperties[expectedTangentLineDeterminationPropertyIndex];
            Assert.AreEqual(typeof(EnumTypeConverter), tangentLineDeterminationProperty.Converter.GetType());
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineDeterminationProperty,
                calculationGridsCategory,
                "Bepaling tangentlijnen",
                "Bepaling raaklijnen op basis van grondlaagscheidingen of handmatig?");

            PropertyDescriptor tangentLineZTopProperty = dynamicProperties[expectedTangentLineZTopPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineZTopProperty,
                calculationGridsCategory,
                "Tangentlijn Z-boven [m+NAP]",
                "Verticale coordinaat van de bovenste raaklijn.");

            PropertyDescriptor tangentLineZBottomProperty = dynamicProperties[expectedTangentLineZBottomPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineZBottomProperty,
                calculationGridsCategory,
                "Tangentlijn Z-onder [m+NAP]",
                "Verticale coordinaat van de onderste raaklijn.");

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
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.MoveGrid, properties.MoveGrid);
            Assert.AreEqual(input.GridDetermination, properties.GridDetermination);
            Assert.AreEqual(input.TangentLineDetermination, properties.TangentLineDetermination);
            Assert.AreEqual(input.TangentLineZTop, properties.TangentLineZTop);
            Assert.AreEqual(input.TangentLineZBottom, properties.TangentLineZBottom);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());
            MacroStabilityInwardsInput input = calculationItem.InputParameters;

            var handler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, handler);

            var random = new Random();
            bool moveGrid = random.NextBoolean();
            var gridDetermination = random.NextEnumValue<MacroStabilityInwardsGridDetermination>();
            var tangentLineDetermination = random.NextEnumValue<MacroStabilityInwardsTangentLineDetermination>();
            double tangentLineZTop = random.Next();
            double tangentLineZBottom = random.Next();

            // When
            properties.MoveGrid = moveGrid;
            properties.GridDetermination = gridDetermination;
            properties.TangentLineDetermination = tangentLineDetermination;
            properties.TangentLineZTop = (RoundedDouble) tangentLineZTop;
            properties.TangentLineZBottom = (RoundedDouble) tangentLineZBottom;

            // Then
            Assert.AreEqual(moveGrid, input.MoveGrid);
            Assert.AreEqual(gridDetermination, input.GridDetermination);
            Assert.AreEqual(tangentLineDetermination, input.TangentLineDetermination);
            Assert.AreEqual(tangentLineZTop, input.TangentLineZTop.Value);
            Assert.AreEqual(tangentLineZBottom, input.TangentLineZBottom.Value);
        }

        [Test]
        public void MoveGrid_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.MoveGrid = true, calculation);
        }

        [Test]
        public void GridDetermination_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.GridDetermination = MacroStabilityInwardsGridDetermination.Manual, calculation);
        }

        [Test]
        public void TangentLineDetermination_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.TangentLineDetermination = MacroStabilityInwardsTangentLineDetermination.Specified, calculation);
        }

        [Test]
        public void TangentLineZTop_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.TangentLineZTop = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void TangentLineZBottom_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.TangentLineZBottom = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new GeneralMacroStabilityInwardsInput());
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        private static void SetPropertyAndVerifyNotifcationsForCalculation(Action<MacroStabilityInwardsGridSettingsProperties> setProperty,
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

            var properties = new MacroStabilityInwardsGridSettingsProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}