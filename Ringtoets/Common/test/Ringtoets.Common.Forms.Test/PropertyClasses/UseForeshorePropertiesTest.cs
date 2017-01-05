// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class UseForeshorePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new UseForeshoreProperties();

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useForeshoreProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de voorlandgeometrie worden gebruikt tijdens de berekening?",
                                                                            true);

            PropertyDescriptor coordinatesProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            "Misc",
                                                                            "Coördinaten [m]",
                                                                            "Lijst met punten in lokale coördinaten.",
                                                                            true);
        }

        [Test]
        public void Constructor_WithDataWithoutForeshoreGeometryExpectedValues()
        {
            // Setup
            var useForeshoreData = new TestUseForeshore();

            // Call
            var properties = new UseForeshoreProperties(useForeshoreData, null);

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());

            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[0];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(useForeshoreProperty,
                                                                            "Misc",
                                                                            "Gebruik",
                                                                            "Moet de voorlandgeometrie worden gebruikt tijdens de berekening?",
                                                                            true);

            PropertyDescriptor coordinatesProperty = dynamicProperties[1];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(coordinatesProperty,
                                                                            "Misc",
                                                                            "Coördinaten [m]",
                                                                            "Lijst met punten in lokale coördinaten.",
                                                                            true);
        }

        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void Constructor_WithDataWithForeshoreGeometryVariousNumberOfElements_ReturnExpectedValues(int numberOfPoint2D, bool isEnabled)
        {
            // Setup
            var useForeshoreData = new TestUseForeshore
            {
                ForeshoreGeometry = new RoundedPoint2DCollection(
                    0, Enumerable.Repeat(new Point2D(0, 0), numberOfPoint2D).ToArray())
            };

            // Call
            var properties = new UseForeshoreProperties(useForeshoreData, null);

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
        public void Constructor_WithUseForeshoreDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new UseForeshoreProperties(null, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("useForeshoreData", paramName);
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var useForeshoreData = new TestUseForeshore
            {
                UseForeshore = true
            };

            // Call
            var properties = new UseForeshoreProperties(useForeshoreData, null);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void GetProperties_ValidUseForeshore_ExpectedValues()
        {
            // Setup
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
            var properties = new UseForeshoreProperties(useForeshoreData, null);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.AreEqual(geometry, properties.Coordinates);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateData()
        {
            // Setup
            var useForeshoreData = new TestUseForeshore();
            var properties = new UseForeshoreProperties(useForeshoreData, null);

            // Call
            properties.UseForeshore = true;

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UseForeshore_WithOrWithoutOutput_InputNotifiedAndPropertyChangedCalled(bool hasOutput)
        {
            // Setup
            var mocks = new MockRepository();
            var calculationObserver = mocks.StrictMock<IObserver>();
            var inputObserver = mocks.StrictMock<IObserver>();
            inputObserver.Expect(o => o.UpdateObserver());
            var handler = mocks.StrictMock<IPropertyChangeHandler>();
            handler.Expect(o => o.PropertyChanged());
            mocks.ReplayAll();

            var calculation = new TestCalculation();
            if (hasOutput)
            {
                calculation.Output = new object();
            }

            calculation.Attach(calculationObserver);
            var input = new TestUseForeshore();
            input.Attach(inputObserver);

            var properties = new UseForeshoreProperties(input, handler);

            // Call
            properties.UseForeshore = true;

            // Assert
            mocks.VerifyAll();
        }

        private class TestUseForeshore : Observable, IUseForeshore
        {
            public bool UseForeshore { get; set; }
            public RoundedPoint2DCollection ForeshoreGeometry { get; set; }
        }
    }
}