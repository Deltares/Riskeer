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

namespace Ringtoets.Common.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class UseForeshorePropertiesTest
    {
        [Test]
        public void Constructor_IUseForeshoreNull_ExpectedValues()
        {
            // Call
            var properties = new UseForeshoreProperties(null);

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var useForshoreData = new TestUseForeshore
            {
                UseForeshore = true
            };

            // Call
            var properties = new UseForeshoreProperties(useForshoreData);

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
            var useForshoreData = new TestUseForeshore
            {
                ForeshoreGeometry = new RoundedPoint2DCollection(2, geometry),
                UseForeshore = true
            };

            // Call
            var properties = new UseForeshoreProperties(useForshoreData);

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.AreEqual(geometry, properties.Coordinates);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var useForshoreData = new TestUseForeshore();
            var properties = new UseForeshoreProperties(useForshoreData);

            useForshoreData.Attach(observerMock);

            // Call
            properties.UseForeshore = true;

            // Assert
            Assert.IsTrue(properties.UseForeshore);
            Assert.IsNull(properties.Coordinates);
            Assert.AreEqual(string.Empty, properties.ToString());
            mockRepository.VerifyAll();
        }

        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void PropertyAttributes_VariousNumberOfElements_ReturnExpectedValues(int numberOfPoint2D, bool isEnabled)
        {
            // Setup
            var useForshoreData = new TestUseForeshore
            {
                ForeshoreGeometry = new RoundedPoint2DCollection(
                    0, Enumerable.Repeat(new Point2D(0, 0), numberOfPoint2D).ToArray())
            };

            // Call
            var properties = new UseForeshoreProperties(useForshoreData);

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

        private class TestUseForeshore : Observable, IUseForeshore
        {
            public bool UseForeshore { get; set; }
            public RoundedPoint2DCollection ForeshoreGeometry { get; set; }
        }
    }
}