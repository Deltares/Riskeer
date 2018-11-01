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
using System.Collections.Generic;
using Core.Components.Gis.Data;
using Core.Components.Gis.Helpers;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Helpers
{
    [TestFixture]
    public class MapDataCollectionHelperTest
    {
        [Test]
        public void GetChildVisibilityStates_MapDataCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MapDataCollectionHelper.GetChildVisibilityStates(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mapDataCollection", exception.ParamName);
        }

        [Test]
        public void GetChildVisibilityStates_MapDataCollectionWithoutChildren_ReturnsEmptyDictionary()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");

            // Call
            Dictionary<MapData, bool> states = MapDataCollectionHelper.GetChildVisibilityStates(mapDataCollection);

            // Assert
            CollectionAssert.IsEmpty(states);
        }

        [Test]
        public void GetChildVisibilityStates_MapDataCollectionWithChildren_ReturnsDictionary()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");
            var mapData1 = new TestFeatureBasedMapData();
            var mapData2 = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var mapData3 = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var nestedCollection = new MapDataCollection("nested");
            nestedCollection.Add(mapData3);

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(nestedCollection);

            // Call
            Dictionary<MapData, bool> states = MapDataCollectionHelper.GetChildVisibilityStates(mapDataCollection);

            // Assert
            var expectedDictionary = new Dictionary<MapData, bool>
            {
                {
                    mapData1, true
                },
                {
                    mapData2, false
                },
                {
                    mapData3, false
                },
                {
                    nestedCollection, false
                }
            };
            CollectionAssert.AreEqual(expectedDictionary, states);
        }

        [Test]
        public void GetNestedCollectionVisibilityStates_MapDataCollectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MapDataCollectionHelper.GetNestedCollectionVisibilityStates(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("mapDataCollection", exception.ParamName);
        }

        [Test]
        public void GetNestedCollectionVisibilityStates_MapDataCollectionWithoutChildren_ReturnsEmptyDictionary()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");

            // Call
            Dictionary<MapDataCollection, MapDataCollectionVisibility> states = MapDataCollectionHelper.GetNestedCollectionVisibilityStates(mapDataCollection);

            // Assert
            CollectionAssert.IsEmpty(states);
        }

        [Test]
        public void GetNestedCollectionVisibilityStates_MapDataCollectionWithChildren_ReturnsDictionary()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");
            var mapData1 = new TestFeatureBasedMapData();
            var mapData2 = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var mapData3 = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var nestedCollection = new MapDataCollection("nested");
            nestedCollection.Add(mapData3);

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(nestedCollection);

            // Call
            Dictionary<MapDataCollection, MapDataCollectionVisibility> states = MapDataCollectionHelper.GetNestedCollectionVisibilityStates(mapDataCollection);

            // Assert
            var expectedDictionary = new Dictionary<MapDataCollection, MapDataCollectionVisibility>
            {
                {
                    nestedCollection, MapDataCollectionVisibility.NotVisible
                }
            };
            CollectionAssert.AreEqual(expectedDictionary, states);
        }
    }
}