// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Gui.PropertyBag;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using Core.Plugins.Map.PropertyClasses;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.PropertyClasses
{
    [TestFixture]
    public class MapDataCollectionPropertiesTest
    {
        private const int namePropertyIndex = 0;
        private const int isVisiblePropertyIndex = 1;

        [Test]
        public void Constructor_MapDataCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapDataCollectionProperties(null, Enumerable.Empty<MapDataCollection>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mapDataCollection", exception.ParamName);
        }

        [Test]
        public void Constructor_ParentsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapDataCollectionProperties(new MapDataCollection("Test"), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parents", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Test");
            mapDataCollection.Add(new TestFeatureBasedMapData());

            // Call
            var properties = new MapDataCollectionProperties(mapDataCollection, Enumerable.Empty<MapDataCollection>());

            // Assert
            Assert.IsInstanceOf<ObjectProperties<MapDataCollection>>(properties);
            Assert.AreSame(mapDataCollection, properties.Data);

            Assert.AreEqual(mapDataCollection.Name, properties.Name);
            Assert.AreEqual(mapDataCollection.IsVisible, properties.IsVisible);
        }

        [Test]
        public void Constructor_Always_PropertiesHaveExpectedAttributesValues()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Test");

            // Call
            var properties = new MapDataCollectionProperties(mapDataCollection, Enumerable.Empty<MapDataCollection>());

            // Assert
            PropertyDescriptorCollection dynamicProperties = PropertiesTestHelper.GetAllVisiblePropertyDescriptors(properties);
            Assert.AreEqual(2, dynamicProperties.Count);

            const string mapDataCollectionCategory = "Kaartlagenmap";

            PropertyDescriptor nameProperty = dynamicProperties[namePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(nameProperty,
                                                                            mapDataCollectionCategory,
                                                                            "Naam",
                                                                            "De naam van deze kaartlagenmap.",
                                                                            true);

            PropertyDescriptor isVisibleProperty = dynamicProperties[isVisiblePropertyIndex];
            PropertiesTestHelper.AssertRequiredPropertyDescriptorProperties(isVisibleProperty,
                                                                            mapDataCollectionCategory,
                                                                            "Weergeven",
                                                                            "Geeft aan of deze kaartlagenmap wordt weergegeven.");
        }

        [Test]
        public void IsVisible_Always_NotifiesDataAndParents()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(2);
            mocks.ReplayAll();

            var parentMapDataCollection = new MapDataCollection("Parent 1");
            var mapDataCollection = new MapDataCollection("Collection");

            parentMapDataCollection.Add(mapDataCollection);

            parentMapDataCollection.Attach(observer);
            mapDataCollection.Attach(observer);

            var properties = new MapDataCollectionProperties(mapDataCollection, new[]
            {
                parentMapDataCollection
            });

            // Call
            properties.IsVisible = false;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, 1)]
        [TestCase(false, 4)]
        public void GivenPropertiesWithChildren_WhenIsVisibleChanged_ThenOnlyChangedChildrenNotified(bool isVisible, int expectedNotifications)
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(expectedNotifications);
            mocks.ReplayAll();

            var mapDataCollection = new MapDataCollection("Collection");
            var childLayer1 = new TestFeatureBasedMapData();
            var childCollection = new MapDataCollection("Child");
            var childLayer2 = new TestFeatureBasedMapData();

            mapDataCollection.Add(childLayer1);
            mapDataCollection.Add(childCollection);
            childCollection.Add(childLayer2);

            mapDataCollection.Attach(observer);
            childLayer1.Attach(observer);
            childCollection.Attach(observer);
            childLayer2.Attach(observer);

            var properties = new MapDataCollectionProperties(mapDataCollection, Enumerable.Empty<MapDataCollection>());

            // When
            properties.IsVisible = isVisible;

            // Then
            mocks.VerifyAll();
        }
    }
}