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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class BackgroundMapDataContainerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var container = new BackgroundMapDataContainer();

            // Assert
            Assert.IsInstanceOf<Observable>(container);

            Assert.IsNull(container.MapData);
            Assert.IsTrue(container.IsVisible);
            Assert.AreEqual(2, container.Transparency.NumberOfDecimalPlaces);
            Assert.AreEqual(0, container.Transparency.Value);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.8)]
        [TestCase(1)]
        public void Transparency_ValidValues_ReturnNewlySetValue(double newValue)
        {
            // Setup
            var container = new BackgroundMapDataContainer();

            // Call
            container.Transparency = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, container.Transparency);
        }

        [Test]
        public void Transparency_WithMapData_MapDataTransparencyUpdated()
        {
            // Setup
            var imageBasedMapData = new ImageBasedMapDataStub();

            var container = new BackgroundMapDataContainer();
            container.MapData = imageBasedMapData;

            // Call
            container.Transparency = (RoundedDouble) 0.5;

            // Assert
            Assert.AreEqual(container.Transparency, imageBasedMapData.Transparency);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-123.56)]
        [TestCase(0.0 - 1e-2)]
        [TestCase(1.0 + 1e-2)]
        [TestCase(456.876)]
        [TestCase(double.NaN)]
        public void Transparency_SetInvalidValue_ThrowArgumentOutOfRangeException(double invalidTransparency)
        {
            // Setup
            var container = new BackgroundMapDataContainer();

            // Call
            TestDelegate call = () => container.Transparency = (RoundedDouble) invalidTransparency;

            // Assert
            var message = "De transparantie moet in het bereik [0,00, 1,00] liggen.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsVisible_SetNewValue_GetNewlySetValue(bool newValue)
        {
            // Setup
            var container = new BackgroundMapDataContainer();

            // Call
            container.IsVisible = newValue;

            // Assert
            Assert.AreEqual(newValue, container.IsVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsVisible_WithMapData_MapDataIsVisibleUpdated(bool isVisible)
        {
            // Setup
            var imageBasedMapData = new ImageBasedMapDataStub();

            var container = new BackgroundMapDataContainer
            {
                MapData = imageBasedMapData
            };

            // Call
            container.IsVisible = isVisible;

            // Assert
            Assert.AreEqual(container.IsVisible, imageBasedMapData.IsVisible);
        }

        [Test]
        [TestCase(true, 0.5)]
        [TestCase(false, 0.8)]
        public void MapData_SetNewValue_GetNewValueAndInitializePreconfiguration(bool isVisible, double transparency)
        {
            // Setup
            var imageBasedMapData = new ImageBasedMapDataStub();

            var container = new BackgroundMapDataContainer
            {
                IsVisible = isVisible,
                Transparency = (RoundedDouble) transparency
            };

            // Call
            container.MapData = imageBasedMapData;

            // Assert
            Assert.AreEqual(container.Transparency, imageBasedMapData.Transparency);
            Assert.AreEqual(container.IsVisible, imageBasedMapData.IsVisible);
            Assert.AreSame(imageBasedMapData, container.MapData);
        }

        [Test]
        public void NotifyObservers_WithMapData_NotifyObserversOfMapDataAndContainer()
        {
            // Setup
            var mocks = new MockRepository();
            var mapDataObserver = mocks.StrictMock<IObserver>();
            mapDataObserver.Expect(o => o.UpdateObserver());

            var containerObserver = mocks.StrictMock<IObserver>();
            containerObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var imageBasedMapData = new ImageBasedMapDataStub();
            imageBasedMapData.Attach(mapDataObserver);

            var container = new BackgroundMapDataContainer
            {
                MapData = imageBasedMapData
            };
            container.Attach(containerObserver);

            // Call
            container.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // 'imageBasedMapData' and 'container' should have it's observers notified.
        }


        [Test]
        public void NotifyObservers_NoMapData_NotifyObserversOfContainer()
        {
            // Setup
            var mocks = new MockRepository();
            var containerObserver = mocks.StrictMock<IObserver>();
            containerObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var container = new BackgroundMapDataContainer();
            container.Attach(containerObserver);

            // Call
            container.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // 'container' should have it's observers notified.
        }

        private class ImageBasedMapDataStub : ImageBasedMapData
        {
            public ImageBasedMapDataStub() : base("A") {}
        }
    }
}