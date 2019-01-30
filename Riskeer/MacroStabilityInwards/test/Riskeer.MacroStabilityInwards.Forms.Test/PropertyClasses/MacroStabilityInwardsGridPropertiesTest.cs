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
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;

namespace Riskeer.MacroStabilityInwards.Forms.Test.PropertyClasses
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

            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

            // Call
            var properties = new MacroStabilityInwardsGridProperties(grid, changeHandler, false);

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MacroStabilityInwardsGrid>>(properties);
            Assert.AreSame(grid, properties.Data);
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
            // Setup
            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsGridProperties(grid, null, false);

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

            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

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

            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();

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

            var random = new Random(21);
            RoundedDouble xLeft = random.NextRoundedDouble();
            var xRight = (RoundedDouble) (1 + random.NextDouble());
            var zTop = (RoundedDouble) (1 + random.NextDouble());
            RoundedDouble zBottom = random.NextRoundedDouble();
            int numberOfHorizontalPoints = random.Next(1, 100);
            int numberOfVerticalPoints = random.Next(1, 100);

            // When
            properties.XLeft = xLeft;
            properties.XRight = xRight;
            properties.ZTop = zTop;
            properties.ZBottom = zBottom;
            properties.NumberOfHorizontalPoints = numberOfHorizontalPoints;
            properties.NumberOfVerticalPoints = numberOfVerticalPoints;

            // Then
            Assert.AreEqual(xLeft, grid.XLeft, grid.XLeft.GetAccuracy());
            Assert.AreEqual(xRight, grid.XRight, grid.XRight.GetAccuracy());
            Assert.AreEqual(zTop, grid.ZTop, grid.ZTop.GetAccuracy());
            Assert.AreEqual(zBottom, grid.ZBottom, grid.ZBottom.GetAccuracy());
            Assert.AreEqual(numberOfHorizontalPoints, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(numberOfVerticalPoints, grid.NumberOfVerticalPoints);
        }

        [Test]
        public void XLeft_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.XLeft = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void XRight_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.XRight = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ZTop_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.ZTop = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void ZBottom_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.ZBottom = (RoundedDouble) 1, calculation);
        }

        [Test]
        public void NumberOfHorizontalPoints_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.NumberOfHorizontalPoints = 1, calculation);
        }

        [Test]
        public void NumberOfVerticalPoints_SetValidValue_SetsValueAndUpdatesObservers()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call & Assert
            SetPropertyAndVerifyNotificationsForCalculation(properties => properties.NumberOfVerticalPoints = 1, calculation);
        }

        [Test]
        public void ToString_Always_ReturnEmptyString()
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();
            var properties = new MacroStabilityInwardsGridProperties(grid, changeHandler, false);

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

            MacroStabilityInwardsGrid grid = MacroStabilityInwardsGridTestFactory.Create();
            var properties = new MacroStabilityInwardsGridProperties(grid, changeHandler, isReadOnly);

            // Call
            bool result = properties.DynamicReadOnlyValidationMethod(string.Empty);

            // Assert
            Assert.AreEqual(isReadOnly, result);
        }

        private static void SetPropertyAndVerifyNotificationsForCalculation(Action<MacroStabilityInwardsGridProperties> setProperty,
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