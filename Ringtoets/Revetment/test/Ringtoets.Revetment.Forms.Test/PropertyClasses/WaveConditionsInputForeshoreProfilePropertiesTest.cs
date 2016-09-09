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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PropertyClasses;

namespace Ringtoets.Revetment.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class WaveConditionsInputForeshoreProfilePropertiesTest
    {
        private const int useForeshorePropertyIndex = 0;
        private const int coordinatesPropertyIndex = 1;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            var properties = new WaveConditionsInputForeshoreProfileProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<WaveConditionsInput>>(properties);
            Assert.IsNull(properties.Data);
            Assert.AreEqual(string.Empty, properties.ToString());
        }

        [Test]
        public void Data_SetNewInputContextInstance_ReturnCorrectPropertyValues()
        {
            // Setup
            var input = new WaveConditionsInput();
            var properties = new WaveConditionsInputForeshoreProfileProperties();

            // Call
            properties.Data = input;

            // Assert
            Assert.IsFalse(properties.UseForeshore);
            CollectionAssert.IsEmpty(properties.Coordinates);
        }

        [Test]
        public void Data_SetInputContextInstanceWithData_ReturnCorrectPropertyValues()
        {
            // Setup
            var input = new WaveConditionsInput
            {
                ForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        new[]
                                                        {
                                                            new Point2D(1.1, 2.2),
                                                            new Point2D(3.3, 4.4)
                                                        }, null, new ForeshoreProfile.ConstructionProperties())
            };
            var properties = new WaveConditionsInputForeshoreProfileProperties();

            // Call
            properties.Data = input;

            // Assert
            var expectedCoordinates = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            };
            Assert.IsTrue(properties.UseForeshore);
            CollectionAssert.AreEqual(expectedCoordinates, properties.Coordinates);
        }

        [Test]
        public void SetProperties_IndividualProperties_UpdateDataAndNotifyObservers()
        {
            // Setup
            var mockRepository = new MockRepository();
            var observerMock = mockRepository.StrictMock<IObserver>();
            const int numberProperties = 1;
            observerMock.Expect(o => o.UpdateObserver()).Repeat.Times(numberProperties);
            mockRepository.ReplayAll();

            var input = new WaveConditionsInput();
            var properties = new WaveConditionsInputForeshoreProfileProperties
            {
                Data = input
            };

            input.Attach(observerMock);

            // Call
            properties.UseForeshore = false;

            // Assert
            Assert.IsFalse(input.UseForeshore);
            mockRepository.VerifyAll();
        }

        [TestCase(true, 0, true, TestName = "Properties_ForeshoreProfileAndForelands_ReturnValues(true, 0, true)")]
        [TestCase(true, 1, true, TestName = "Properties_ForeshoreProfileAndForelands_ReturnValues(true, 1, true)")]
        [TestCase(true, 2, false, TestName = "Properties_ForeshoreProfileAndForelands_ReturnValues(true, 2, false)")]
        [TestCase(false, 0, true, TestName = "Properties_ForeshoreProfileAndForelands_ReturnValues(false, 0, true)")]
        public void PropertyAttributes_WithOrWithoutForeshoreProfileAndForelands_ReturnExpectedValues(bool withDikeProfile, int forlands, bool expectedCoordinatesPropertyReadOnly)
        {
            // Setup
            var input = new WaveConditionsInput();

            if (withDikeProfile)
            {
                var point2Ds = new List<Point2D>();
                for (var i = 0; i < forlands; i++)
                {
                    point2Ds.Add(new Point2D(i, i));
                }

                input.ForeshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                              point2Ds, null, new ForeshoreProfile.ConstructionProperties());
            }

            // Call
            var properties = new WaveConditionsInputForeshoreProfileProperties
            {
                Data = input
            };

            // Assert
            var dynamicPropertyBag = new DynamicPropertyBag(properties);
            PropertyDescriptorCollection dynamicProperties = dynamicPropertyBag.GetProperties(new Attribute[]
            {
                BrowsableAttribute.Yes
            });
            Assert.AreEqual(2, dynamicProperties.Count);

            PropertyDescriptor useForeshoreProperty = dynamicProperties[useForeshorePropertyIndex];
            Assert.IsNotNull(useForeshoreProperty);
            Assert.AreEqual(expectedCoordinatesPropertyReadOnly, useForeshoreProperty.IsReadOnly);
            Assert.AreEqual("Gebruik", useForeshoreProperty.DisplayName);
            Assert.AreEqual("Moet het voorlandprofiel worden gebruikt tijdens de berekening?", useForeshoreProperty.Description);

            PropertyDescriptor coordinatesProperty = dynamicProperties[coordinatesPropertyIndex];
            Assert.IsNotNull(coordinatesProperty);
            Assert.IsTrue(coordinatesProperty.IsReadOnly);
            Assert.AreEqual("Coördinaten [m]", coordinatesProperty.DisplayName);
            Assert.AreEqual("Lijst met punten in lokale coördinaten.", coordinatesProperty.Description);
        }
    }
}