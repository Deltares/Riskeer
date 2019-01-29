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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TestUtil;

namespace Riskeer.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class UseForeshorePropertiesTest
    {
        [Test]
        public void Constructor_UseForeshoreDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new UseForeshoreProperties(null, handler);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("useForeshoreData", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HandlerNull_ThrowsArgumentNullException()
        {
            // Setup
            var testUseForeshore = new TestUseForeshore();

            var mocks = new MockRepository();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new UseForeshoreProperties(testUseForeshore, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("handler", paramName);
        }

        [Test]
        public void Constructor_WithDataWithoutForeshoreGeometry_CoordinatesNull()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var useForeshoreData = new TestUseForeshore();

            // Call
            var properties = new UseForeshoreProperties(useForeshoreData, handler);

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var useForeshoreData = new TestUseForeshore
            {
                UseForeshore = true
            };

            // Call
            var properties = new UseForeshoreProperties(useForeshoreData, handler);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.IsEmpty(properties.ToString());
        }

        [Test]
        public void GetProperties_ValidUseForeshore_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
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
            var properties = new UseForeshoreProperties(useForeshoreData, handler);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.AreEqual(geometry, properties.Coordinates);
        }

        [Test]
        public void UseForeshore_Always_InputNotifiedAndPropertyChangedCalled()
        {
            bool useForeshore = new Random(21).NextBoolean();
            SetPropertyAndVerifyNotificationsAndOutputForCalculation(properties => properties.UseForeshore = useForeshore,
                                                                     new TestUseForeshore());
        }

        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void Constructor_WithDataWithForeshoreGeometryVariousNumberOfElements_ReturnExpectedValues(int numberOfPoint2D, bool isEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var handler = mocks.Stub<IObservablePropertyChangeHandler>();
            mocks.ReplayAll();

            var useForeshoreData = new TestUseForeshore
            {
                ForeshoreGeometry = new RoundedPoint2DCollection(
                    0, Enumerable.Repeat(new Point2D(0, 0), numberOfPoint2D).ToArray())
            };

            // Call
            var properties = new UseForeshoreProperties(useForeshoreData, handler);

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

        private static void SetPropertyAndVerifyNotificationsAndOutputForCalculation(
            Action<UseForeshoreProperties> setProperty,
            TestUseForeshore input)
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

            var properties = new UseForeshoreProperties(input, handler);

            // Call
            setProperty(properties);

            // Assert
            Assert.IsTrue(handler.Called);
            mocks.VerifyAll();
        }

        private class TestUseForeshore : CloneableObservable, ICalculationInput, IUseForeshore
        {
            public bool UseForeshore { get; set; }
            public RoundedPoint2DCollection ForeshoreGeometry { get; set; }
        }
    }
}