﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class UseForeshorePropertiesTest
    {
        [Test]
        public void Constructor_UseForeshoreDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new UseForeshoreProperties<TestUseForeshore>(null, calculation, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("useForeshoreData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            TestUseForeshore testUseForeshore = new TestUseForeshore();

            var mocks = new MockRepository();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new UseForeshoreProperties<TestUseForeshore>(testUseForeshore, null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculation", paramName);
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            TestUseForeshore testUseForeshore = new TestUseForeshore();

            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new UseForeshoreProperties<TestUseForeshore>(testUseForeshore, calculation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void Constructor_WithDataWithForeshoreGeometryVariousNumberOfElements_ReturnExpectedValues(int numberOfPoint2D, bool isEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var useForeshoreData = new TestUseForeshore
            {
                ForeshoreGeometry = new RoundedPoint2DCollection(
                    0, Enumerable.Repeat(new Point2D(0, 0), numberOfPoint2D).ToArray())
            };

            // Call
            var properties = new UseForeshoreProperties<TestUseForeshore>(useForeshoreData, calculation, handler);

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useForeshoreProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de voorlandgeometrie worden gebruikt tijdens de berekening?",
                                                                            !isEnabled);

            PropertyDescriptor coordinatesProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            "Misc",
                                                                            "Coördinaten [m]",
                                                                            "Lijst met punten in lokale coördinaten.",
                                                                            true);
        }
        [Test]
        public void Constructor_WithDataWithoutForeshoreGeometry_CoordinatesNull()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var useForeshoreData = new TestUseForeshore();

            // Call
            var properties = new UseForeshoreProperties<TestUseForeshore>(useForeshoreData, calculation, handler);

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var useForeshoreData = new TestUseForeshore
            {
                UseForeshore = true
            };

            // Call
            var properties = new UseForeshoreProperties<TestUseForeshore>(useForeshoreData, calculation, handler);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void GetProperties_ValidUseForeshore_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var handler = mocks.Stub<ICalculationInputPropertyChangeHandler>();
            mocks.ReplayAll();

            var geometry = new[]
            {
                new Point2D(1, 1)
            };
            var useForeshoreData = new TestUseForeshore
            {
                ForeshoreGeometry = new RoundedPoint2DCollection(2, geometry),
                UseForeshore = true
            };

            // Call
            var properties = new UseForeshoreProperties<TestUseForeshore>(useForeshoreData, calculation, handler);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.AreEqual(geometry, properties.Coordinates);
        }

        [Test]
        public void UseForeshore_Always_InputNotifiedAndPropertyChangedCalled()
        {
            var useForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotifcationsAndOutputForCalculation(properties => properties.UseForeshore = useForeshore,
                                                                    useForeshore,
                                                                    new TestUseForeshore());
        }

        private void SetPropertyAndVerifyNotifcationsAndOutputForCalculation<TPropertyValue>(
            Action<UseForeshoreProperties<TestUseForeshore>> setProperty,
            TPropertyValue expectedValueSet,
            TestUseForeshore input)
        {
            // Setup
            var mocks = new MockRepository();
            var calculation = mocks.Stub<ICalculation>();
            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());
            mocks.ReplayAll();

            var handler = new CalculationInputSetPropertyValueAfterConfirmationParameterTester<TPropertyValue>(
                input,
                calculation,
                expectedValueSet,
                new[]
                {
                    observable
                });

            var properties = new UseForeshoreProperties<TestUseForeshore>(input, calculation, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        public class TestUseForeshore : Observable, ICalculationInput, IUseForeshore
        {
            public bool UseForeshore { get; set; }
            public RoundedPoint2DCollection ForeshoreGeometry { get; set; }
        }
    }
}