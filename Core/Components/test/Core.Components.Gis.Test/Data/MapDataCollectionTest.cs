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
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataCollectionTest
    {
        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new MapDataCollection(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the map data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Setup
            const string name = "Some name";

            // Call
            var mapDataCollection = new MapDataCollection(name);

            // Assert
            Assert.AreEqual(name, mapDataCollection.Name);
            Assert.IsInstanceOf<MapData>(mapDataCollection);
            CollectionAssert.IsEmpty(mapDataCollection.Collection);
            Assert.IsFalse(mapDataCollection.IsVisible);
        }

        [Test]
        public void IsVisible_WithAllChildrenVisibleTrue_ReturnsTrue()
        {
            // Setup
            var collection = new MapDataCollection("test");
            collection.Add(new TestMapData());
            collection.Add(new TestMapData());

            // Call
            bool isVisible = collection.IsVisible;

            // Assert
            Assert.IsTrue(isVisible);
        }

        [Test]
        public void IsVisible_WithChildrenVisibleFalse_ReturnsFalse()
        {
            // Setup
            var collection = new MapDataCollection("test");
            collection.Add(new TestMapData());
            collection.Add(new TestMapData
            {
                IsVisible = false
            });

            // Call
            bool isVisible = collection.IsVisible;

            // Assert
            Assert.IsFalse(isVisible);
        }

        [Test]
        public void IsVisible_WithVisibleMapDataAndEmptyCollectionAsChildren_ReturnsTrue()
        {
            // Setup
            var collection = new MapDataCollection("test");
            collection.Add(new TestMapData());
            var nestedCollection = new MapDataCollection("nested");
            collection.Add(nestedCollection);

            // Precondition
            Assert.IsFalse(nestedCollection.IsVisible);
            
            // Call
            bool isVisible = collection.IsVisible;

            // Assert
            Assert.IsTrue(isVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsVisible_SetNewValue_SetsVisibilityOfAllChildrenToNewValue(bool newValue)
        {
            // Setup
            var collection = new MapDataCollection("test");
            collection.Add(new TestMapData
            {
                IsVisible = !newValue
            });
            collection.Add(new TestMapData
            {
                IsVisible = !newValue
            });

            // Precondition
            Assert.AreNotEqual(newValue, collection.IsVisible);

            // Call
            collection.IsVisible = newValue;

            // Assert
            Assert.AreEqual(newValue, collection.IsVisible);

            foreach (MapData mapData in collection.Collection)
            {
                Assert.AreEqual(newValue, mapData.IsVisible);
            }
        }

        [Test]
        public void Add_NotNull_AddsItemToCollection()
        {
            // Setup
            var item = new MapLineData("test");
            var mapDataCollection = new MapDataCollection("test");

            // Call
            mapDataCollection.Add(item);

            // Assert
            List<MapData> mapData = mapDataCollection.Collection.ToList();
            Assert.AreEqual(1, mapData.Count);
            Assert.AreSame(item, mapData.First());
        }

        [Test]
        public void Add_Null_ThrowsArgumentNullException()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");

            // Call
            TestDelegate call = () => mapDataCollection.Add(null);

            // Assert
            const string expectedMessage = "An item cannot be null when adding it to the collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Insert_ItemNotNullAndValidIndex_InsertsItemToCollectionAtGivenIndex()
        {
            // Setup
            var itemToInsert = new MapLineData("test");
            var existingItem = new MapPointData("test");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(existingItem);

            // Precondition
            Assert.AreEqual(1, mapDataCollection.Collection.Count());
            Assert.AreSame(existingItem, mapDataCollection.Collection.ElementAt(0));

            // Call
            mapDataCollection.Insert(0, itemToInsert);

            // Assert
            Assert.AreEqual(2, mapDataCollection.Collection.Count());
            Assert.AreSame(itemToInsert, mapDataCollection.Collection.ElementAt(0));
            Assert.AreSame(existingItem, mapDataCollection.Collection.ElementAt(1));
        }

        [Test]
        public void Insert_ItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test");

            // Call
            TestDelegate call = () => mapDataCollection.Insert(0, null);

            // Assert
            const string expectedMessage = "An item cannot be null when adding it to the collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Remove_ExistingItem_RemovesItem()
        {
            // Setup
            var item = new MapLineData("test");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(item);

            // Precondition
            Assert.AreEqual(1, mapDataCollection.Collection.Count());
            Assert.IsInstanceOf<MapLineData>(mapDataCollection.Collection.First());

            // Call
            mapDataCollection.Remove(item);

            // Assert
            CollectionAssert.IsEmpty(mapDataCollection.Collection);
        }

        [Test]
        public void Remove_Null_DoesNotRemove()
        {
            // Setup
            var item = new MapLineData("test");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(item);

            // Precondition
            Assert.AreEqual(1, mapDataCollection.Collection.Count());
            Assert.IsInstanceOf<MapLineData>(mapDataCollection.Collection.First());
            List<MapData> listBeforeRemove = mapDataCollection.Collection.ToList();

            // Call
            mapDataCollection.Remove(null);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, mapDataCollection.Collection);
        }

        [Test]
        public void Remove_NotExistingItem_DoesNotRemove()
        {
            // Setup
            var item = new MapLineData("test");
            var otherItem = new MapPointData("another test");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(item);

            // Precondition
            Assert.AreEqual(1, mapDataCollection.Collection.Count());
            Assert.IsInstanceOf<MapLineData>(mapDataCollection.Collection.First());
            List<MapData> listBeforeRemove = mapDataCollection.Collection.ToList();

            // Call
            mapDataCollection.Remove(otherItem);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, mapDataCollection.Collection);
        }

        [Test]
        public void Clear_Always_RemovesAllItems()
        {
            // Setup
            var item1 = new MapLineData("test");
            var item2 = new MapLineData("test");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(item1);
            mapDataCollection.Add(item2);

            // Precondition
            Assert.AreEqual(2, mapDataCollection.Collection.Count());

            // Call
            mapDataCollection.Clear();

            // Assert
            CollectionAssert.IsEmpty(mapDataCollection.Collection);
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int invalidIndex)
        {
            // Setup
            var itemToInsert = new MapLineData("test");
            var existingItem = new MapPointData("test");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(existingItem);

            // Call
            TestDelegate call = () => mapDataCollection.Insert(invalidIndex, itemToInsert);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call, "index");
        }
    }
}