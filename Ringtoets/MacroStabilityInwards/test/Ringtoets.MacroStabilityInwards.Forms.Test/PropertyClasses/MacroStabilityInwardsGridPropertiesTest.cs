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
    public class MacroStabilityInwardsGridPropertiesTest
    {
        private const int expectedXLeftPropertyIndex = 0;
        private const int expectedXRightPropertyIndex = 1;
        private const int expectedZTopPropertyIndex = 2;
        private const int expectedZBottomPropertyIndex = 3;
        private const int expectedNumberOfHorizontalPointsPropertyIndex = 4;
        private const int expectedNumberOfVerticalPointsPropertyIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var data = new MacroStabilityInwardsGrid();

            // Call
            var properties = new MacroStabilityInwardsGridProperties(data, changeHandler, false);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsGrid>>(properties);
            Assert.AreSame(data, properties.Data);
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
            TestDelegate call = () => new MacroStabilityInwardsGridProperties(null, changeHandler, false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsGridProperties(new MacroStabilityInwardsGrid(), null, false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("handler", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Constructor_ValidData_PropertiesHaveExpectedAttributesValues(bool isReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var grid = new MacroStabilityInwardsGrid();

            // Call
            var properties = new MacroStabilityInwardsGridProperties(grid, changeHandler, isReadOnly);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);

            Assert.AreEqual(6, dynamicProperties.Count);

            const string gridCategory = "Grid";

            PropertyDescriptor xLeftProperty = dynamicProperties[expectedXLeftPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                xLeftProperty,
                gridCategory,
                "X links [m]",
                "Horizontale coördinaat van de linker kant van het rekengrid.",
                isReadOnly);

            PropertyDescriptor xRightProperty = dynamicProperties[expectedXRightPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                xRightProperty,
                gridCategory,
                "X rechts [m]",
                "Horizontale coördinaat van de rechter kant van het rekengrid.",
                isReadOnly);

            PropertyDescriptor zTopProperty = dynamicProperties[expectedZTopPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                zTopProperty,
                gridCategory,
                "Z boven [m+NAP]",
                "Verticale coördinaat van de bovenkant van het rekengrid.",
                isReadOnly);

            PropertyDescriptor zBottomProperty = dynamicProperties[expectedZBottomPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                zBottomProperty,
                gridCategory,
                "Z onder [m+NAP]",
                "Verticale coördinaat van de onderkant van het rekengrid.",
                isReadOnly);

            PropertyDescriptor numberOfHorizontalPointsProperty = dynamicProperties[expectedNumberOfHorizontalPointsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                numberOfHorizontalPointsProperty,
                gridCategory,
                "Aantal horizontale punten",
                "Aantal punten waarmee het grid wordt samengesteld in horizontale richting.",
                isReadOnly);

            PropertyDescriptor numberOfVerticalPointsProperty = dynamicProperties[expectedNumberOfVerticalPointsPropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(
                numberOfVerticalPointsProperty,
                gridCategory,
                "Aantal verticale punten",
                "Aantal punten waarmee het grid wordt samengesteld in verticale richting.",
                isReadOnly);

            mocks.VerifyAll();
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var grid = new MacroStabilityInwardsGrid();

            // Call
            var properties = new MacroStabilityInwardsGridProperties(grid, changeHandler, false);

            // Assert
            Assert.AreEqual(grid.XLeft, properties.XLeft);
            Assert.AreEqual(grid.XRight, properties.XRight);
            Assert.AreEqual(grid.ZTop, properties.ZTop);
            Assert.AreEqual(grid.ZBottom, properties.ZBottom);
            Assert.AreEqual(grid.NumberOfHorizontalPoints, properties.NumberOfHorizontalPoints);
            Assert.AreEqual(grid.NumberOfVerticalPoints, properties.NumberOfVerticalPoints);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenPropertiesWithData_WhenChangingProperties_ThenPropertiesSetOnGrid()
        {
            // Given
            var calculationItem = new MacroStabilityInwardsCalculationScenario();
            MacroStabilityInwardsInput input = calculationItem.InputParameters;
            MacroStabilityInwardsGrid grid = input.LeftGrid;

            var handler = new ObservablePropertyChangeHandler(calculationItem, input);
            var properties = new MacroStabilityInwardsGridProperties(grid, handler, false);

            var random = new Random();
            double xLeft = random.Next();
            double xRight = random.Next();
            double zTop = random.Next();
            double zBottom = random.Next();
            int numberOfHorizontalPoints = random.Next();
            int numberOfVerticalPoints = random.Next();

            // When
            properties.XLeft = (RoundedDouble) xLeft;
            properties.XRight = (RoundedDouble) xRight;
            properties.ZTop = (RoundedDouble) zTop;
            properties.ZBottom = (RoundedDouble) zBottom;
            properties.NumberOfHorizontalPoints = numberOfHorizontalPoints;
            properties.NumberOfVerticalPoints = numberOfVerticalPoints;

            // Then
            Assert.AreEqual(xLeft, grid.XLeft);
            Assert.AreEqual(xRight, grid.XRight);
            Assert.AreEqual(zTop, grid.ZTop);
            Assert.AreEqual(zBottom, grid.ZBottom);
            Assert.AreEqual(numberOfHorizontalPoints, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, grid.NumberOfVerticalPoints);
        }

        [Test]
        public void XLeft_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.XLeft = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void XRight_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.XRight = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ZTop_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.ZTop = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ZBottom_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.ZBottom = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void NumberOfHorizontalPoints_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.NumberOfHorizontalPoints = 1, calculation);
        }

        [Test]
        public void NumberOfVerticalPoints_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotifcationsForCalculation(properties => properties.NumberOfVerticalPoints = 1, calculation);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new MacroStabilityInwardsGridProperties(new MacroStabilityInwardsGrid(), changeHandler, false);

            // Call
            string toString = properties.ToString();

            // Assert
            Assert.AreEqual(string.Empty, toString);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DynamicReadOnly_isReadOnly_ReturnsExpectedResult(bool isReadOnly)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var properties = new MacroStabilityInwardsGridProperties(new MacroStabilityInwardsGrid(), changeHandler, isReadOnly);
            
            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(string.Empty);

            // Assert
            Assert.AreEqual(isReadOnly, result);
        }

        private static void SetPropertyAndVerifyNotifcationsForCalculation(Action<MacroStabilityInwardsGridProperties> setProperty,
                                                                           MacroStabilityInwardsCalculation calculation)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            MacroStabilityInwardsGrid grid = calculation.InputParameters.LeftGrid;

            var handler = new SetPropertyValueAfterConfirmationParameterTester(new[]
            {
                observable
            });

            var properties = new MacroStabilityInwardsGridProperties(grid, handler, false);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }
    }
}