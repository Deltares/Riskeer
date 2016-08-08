// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
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
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to the map data.");
        }

        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Setup
            const string name = "Some name";

            // Call
            var data = new MapDataCollection(name);

            // Assert
            Assert.AreEqual(name, data.Name);
            Assert.IsInstanceOf<MapData>(data);
            CollectionAssert.IsEmpty(data.Collection);
        }

        [Test]
        public void Add_NotNull_AddsElementToCollection()
        {
            // Setup
            var data = new MapDataCollection("test");
            var objectToAdd = new MapLineData("test");

            // Call
            data.Add(objectToAdd);

            // Assert
            var mpaData = data.Collection.ToList();
            Assert.AreEqual(1, mpaData.Count);
            Assert.AreSame(objectToAdd, mpaData.First());
        }

        [Test]
        public void Add_Null_ThrowsArgumentNullException()
        {
            // Setup
            var data = new MapDataCollection("test");

            // Call
            TestDelegate call = () => data.Add(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "An element cannot be null when adding it to the collection.");
        }

        [Test]
        public void Insert_NotNull_InsertsElementToCollectionAtGivenPosition()
        {
            // Setup
            var mapData = new MapPointData("test");
            var data = new MapDataCollection("test");
            var objectToAdd = new MapLineData("test");

            data.Add(mapData);

            // Precondition
            Assert.AreEqual(1, data.Collection.Count());
            Assert.AreSame(mapData, data.Collection.ElementAt(0));

            // Call
            data.Insert(0, objectToAdd);

            // Assert
            Assert.AreEqual(2, data.Collection.Count());
            Assert.AreSame(objectToAdd, data.Collection.ElementAt(0));
            Assert.AreSame(mapData, data.Collection.ElementAt(1));
        }

        [Test]
        public void Insert_ElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var data = new MapDataCollection("test");

            // Call
            TestDelegate call = () => data.Insert(0, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "An element cannot be null when adding it to the collection.");
        }

        [Test]
        public void Remove_ExistingElement_RemovesElement()
        {
            // Setup
            var data = new MapDataCollection("test");
            var dataElement = new MapLineData("test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.Collection.Count());
            Assert.IsInstanceOf<MapLineData>(data.Collection.First());

            // Call
            data.Remove(dataElement);

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
        }

        [Test]
        public void Remove_Null_DoesNotRemove()
        {
            // Setup
            var data = new MapDataCollection("test");
            var dataElement = new MapLineData("test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.Collection.Count());
            Assert.IsInstanceOf<MapLineData>(data.Collection.First());
            var listBeforeRemove = data.Collection;

            // Call
            data.Remove(null);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, data.Collection);
        }

        [Test]
        public void Remove_NotExistingElement_DoesNotRemove()
        {
            // Setup
            var data = new MapDataCollection("test");
            var dataElement = new MapLineData("test");
            var otherDataElement = new MapPointData("another test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.Collection.Count());
            Assert.IsInstanceOf<MapLineData>(data.Collection.First());
            var listBeforeRemove = data.Collection;

            // Call
            data.Remove(otherDataElement);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, data.Collection);
        }

        [Test]
        public void Clear_Always_RemovesAllElements()
        {
            // Setup
            var data = new MapDataCollection("test");
            var dataElement1 = new MapLineData("test");
            var dataElement2 = new MapLineData("test");

            data.Add(dataElement1);
            data.Add(dataElement2);

            // Precondition
            Assert.AreEqual(2, data.Collection.Count());

            // Call
            data.Clear();

            // Assert
            CollectionAssert.IsEmpty(data.Collection);
        }
    }
}