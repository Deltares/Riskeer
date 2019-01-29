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

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class MacroStabilityInwardsGridSettingsPropertiesTest
    {
        private const int expectedMoveGridPropertyIndex = 0;
        private const int expectedGridDeterminationTypePropertyIndex = 1;
        private const int expectedTangentLineDeterminationTypePropertyIndex = 2;
        private const int expectedTangentLineZTopPropertyIndex = 3;
        private const int expectedTangentLineZBottomPropertyIndex = 4;
        private const int expectedTangentLineNumberPropertyIndex = 5;
        private const int expectedLeftGridPropertyIndex = 6;
        private const int expectedRightGridPropertyIndex = 7;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

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
            // Setup
            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            TestDelegate call = () => new MacroStabilityInwardsGridSettingsProperties(input, null);

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

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());

            // Call
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(8, dynamicProperties.Count);

            const string calculationGridsCategory = "Rekengrids";

            PropertyDescriptor moveGridProperty = dynamicProperties[expectedMoveGridPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                moveGridProperty,
                calculationGridsCategory,
                "Verplaats grid",
                "Sta automatische aanpassing van het grid toe?");

            PropertyDescriptor gridDeterminationTypeProperty = dynamicProperties[expectedGridDeterminationTypePropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsGridSettingsProperties, EnumTypeConverter>(nameof(properties.GridDeterminationType));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                gridDeterminationTypeProperty,
                calculationGridsCategory,
                "Bepaling grid",
                "Rekengrid automatisch bepalen of handmatig invoeren?");

            PropertyDescriptor tangentLineDeterminationTypeProperty = dynamicProperties[expectedTangentLineDeterminationTypePropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsGridSettingsProperties, EnumTypeConverter>(nameof(properties.TangentLineDeterminationType));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineDeterminationTypeProperty,
                calculationGridsCategory,
                "Bepaling tangentlijnen",
                "Bepaling raaklijnen op basis van grondlaagscheidingen of handmatig?",
                true);

            PropertyDescriptor tangentLineZTopProperty = dynamicProperties[expectedTangentLineZTopPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineZTopProperty,
                calculationGridsCategory,
                "Tangentlijn Z-boven [m+NAP]",
                "Verticale coördinaat van de bovenste raaklijn.",
                true);

            PropertyDescriptor tangentLineZBottomProperty = dynamicProperties[expectedTangentLineZBottomPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineZBottomProperty,
                calculationGridsCategory,
                "Tangentlijn Z-onder [m+NAP]",
                "Verticale coördinaat van de onderste raaklijn.",
                true);

            PropertyDescriptor tangentLineNumberProperty = dynamicProperties[expectedTangentLineNumberPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                tangentLineNumberProperty,
                calculationGridsCategory,
                "Aantal tangentlijnen",
                "Het aantal raaklijnen dat bepaald moet worden.",
                true);

            PropertyDescriptor leftGridProperty = dynamicProperties[expectedLeftGridPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsGridSettingsProperties, ExpandableObjectConverter>(nameof(properties.LeftGrid));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                leftGridProperty,
                calculationGridsCategory,
                "Linker grid",
                "Eigenschappen van het linker grid.",
                true);

            PropertyDescriptor rightGridProperty = dynamicProperties[expectedRightGridPropertyIndex];
            TestHelper.AssertTypeConverter<MacroStabilityInwardsGridSettingsProperties, ExpandableObjectConverter>(nameof(properties.RightGrid));
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                rightGridProperty,
                calculationGridsCategory,
                "Rechter grid",
                "Eigenschappen van het rechter grid.",
                true);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Manual)]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Automatic)]
        public void GetProperties_WithData_ReturnExpectedValues(
            MacroStabilityInwardsGridDeterminationType gridDeterminationType)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                GridDeterminationType = gridDeterminationType
            };

            // Call
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Assert
            Assert.AreEqual(input.MoveGrid, properties.MoveGrid);
            Assert.AreEqual(input.GridDeterminationType, properties.GridDeterminationType);
            Assert.AreEqual(input.TangentLineDeterminationType, properties.TangentLineDeterminationType);
            Assert.AreEqual(input.TangentLineZTop, properties.TangentLineZTop);
            Assert.AreEqual(input.TangentLineZBottom, properties.TangentLineZBottom);
            Assert.AreEqual(input.TangentLineNumber, properties.TangentLineNumber);

            bool gridIsReadOnly = gridDeterminationType == MacroStabilityInwardsGridDeterminationType.Automatic;
            Assert.AreSame(input.LeftGrid, properties.LeftGrid.Data);
            Assert.AreEqual(gridIsReadOnly, properties.LeftGrid.DynamicReadOnlyValidationMethod(string.Empty));
            Assert.AreSame(input.RightGrid, properties.RightGrid.Data);
            Assert.AreEqual(gridIsReadOnly, properties.RightGrid.DynamicReadOnlyValidationMethod(string.Empty));
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnInput()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            MacroStabilityInwardsInput input = calculationItem.InputParameters;

            var handler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, handler);

            var random = new Random(21);
            bool moveGrid = random.NextBoolean();
            var gridDeterminationType = random.NextEnumValue<MacroStabilityInwardsGridDeterminationType>();
            var tangentLineDeterminationType = random.NextEnumValue<MacroStabilityInwardsTangentLineDeterminationType>();
            double tangentLineZTop = random.NextDouble(2.0, 3.0);
            double tangentLineZBottom = random.NextDouble(0.0, 1.0);
            int tangentLineNumber = random.Next(1, 51);

            // When
            properties.MoveGrid = moveGrid;
            properties.GridDeterminationType = gridDeterminationType;
            properties.TangentLineDeterminationType = tangentLineDeterminationType;
            properties.TangentLineZTop = (RoundedDouble) tangentLineZTop;
            properties.TangentLineZBottom = (RoundedDouble) tangentLineZBottom;
            properties.TangentLineNumber = tangentLineNumber;

            // Then
            Assert.AreEqual(moveGrid, input.MoveGrid);
            Assert.AreEqual(gridDeterminationType, input.GridDeterminationType);
            Assert.AreEqual(tangentLineDeterminationType, input.TangentLineDeterminationType);
            Assert.AreEqual(tangentLineZTop, input.TangentLineZTop, input.TangentLineZTop.GetAccuracy());
            Assert.AreEqual(tangentLineZBottom, input.TangentLineZBottom, input.TangentLineZBottom.GetAccuracy());
            Assert.AreEqual(tangentLineNumber, input.TangentLineNumber);
        }

        [Test]
        public void MoveGrid_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.MoveGrid = true, calculation);
        }

        [Test]
        public void GridDeterminationType_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual, calculation);
        }

        [Test]
        public void TangentLineDeterminationType_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified, calculation);
        }

        [Test]
        public void TangentLineZTop_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.TangentLineZTop = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void TangentLineZBottom_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.TangentLineZBottom = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void TangentLineNumber_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.TangentLineNumber = 10, calculation);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties());
            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnly_GridDeterminationType_ReturnsExpectedResult(bool isGridDeterminationTypeAutomatic)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                GridDeterminationType = isGridDeterminationTypeAutomatic
                                            ? MacroStabilityInwardsGridDeterminationType.Automatic
                                            : MacroStabilityInwardsGridDeterminationType.Manual
            };

            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(string.Empty);

            // Assert
            Assert.AreEqual(isGridDeterminationTypeAutomatic, result);
        }

        [Test]
        [Combinatorial]
        public void DynamicReadOnly_TangentLineDeterminationType_ReturnsExpectedResult(
            [Values(true, false)] bool isTangentlineDeterminationTypeLayerSeparated,
            [Values(true, false)] bool isGridDeterminationTypeAutomatic,
            [Values(nameof(MacroStabilityInwardsGridSettingsProperties.TangentLineZTop),
                nameof(MacroStabilityInwardsGridSettingsProperties.TangentLineZBottom),
                nameof(MacroStabilityInwardsGridSettingsProperties.TangentLineNumber))]
            string propertyName)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties())
            {
                TangentLineDeterminationType = isTangentlineDeterminationTypeLayerSeparated
                                                   ? MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated
                                                   : MacroStabilityInwardsTangentLineDeterminationType.Specified,
                GridDeterminationType = isGridDeterminationTypeAutomatic
                                            ? MacroStabilityInwardsGridDeterminationType.Automatic
                                            : MacroStabilityInwardsGridDeterminationType.Manual
            };

            var properties = new MacroStabilityInwardsGridSettingsProperties(input, changeHandler);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(propertyName);

            // Assert
            Assert.AreEqual(isTangentlineDeterminationTypeLayerSeparated || isGridDeterminationTypeAutomatic, result);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(Action<MacroStabilityInwardsGridSettingsProperties> setProperty,
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