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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartDataCollectionTest
    {
        [Test]
        public void Constructor_NullList_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartDataCollection(null, "test data");

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Constructor_InvalidName_ThrowsArgumentExcpetion(string invalidName)
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();

            // Call
            TestDelegate test = () => new ChartDataCollection(list, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to chart data");
        }

        [Test]
        public void Constructor_ListSet_InstanceWithListSet()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();

            // Call
            var collection = new ChartDataCollection(list, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(collection);
            Assert.AreSame(list, collection.List);
        }

        [Test]
        public void Constructor_WithName_SetsName()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var name = "Some name";

            // Call
            var data = new ChartDataCollection(list, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        [Test]
        public void Add_NotNull_AddsElementToCollection()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var objectToAdd = new ChartLineData(Enumerable.Empty<Point2D>(), "test");

            // Call
            data.Add(objectToAdd);

            // Assert
            Assert.AreEqual(1, data.List.Count);
            Assert.AreSame(objectToAdd, data.List.First());
        }

        [Test]
        public void Add_Null_ThrowsArgumentNullException()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");

            // Call
            TestDelegate call = () => data.Add(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "An element cannot be null when adding it to the collection.");
        }

        [Test]
        public void Replace_NotNull_ReplacesElementInCollection()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var oldDataElement = new ChartLineData(Enumerable.Empty<Point2D>(), "test");
            var newDataElement = new ChartPointData(Enumerable.Empty<Point2D>(), "another test");

            data.Add(oldDataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());

            // Call
            data.Replace(oldDataElement, newDataElement);

            // Assert
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartPointData>(data.List.First());
        }

        [Test]
        public void Replace_NewElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var oldDataElement = new ChartLineData(Enumerable.Empty<Point2D>(), "test");

            data.Add(oldDataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());

            // Call
            TestDelegate test = () => data.Replace(oldDataElement, null);

            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "An element cannot be replaced with null. Use Remove instead.");
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());
        }

        [Test]
        public void Replace_OldElementNull_ThrowsArgumentNullException()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var dataElement = new ChartLineData(Enumerable.Empty<Point2D>(), "test");
            var newDataElement = new ChartPointData(Enumerable.Empty<Point2D>(), "another test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());

            // Call
            TestDelegate test = () => data.Replace(null, newDataElement);

            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A null element cannot be replaced. Use Add instead.");
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());
        }

        [Test]
        public void Remove_ExistingElement_RemovesElement()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var dataElement = new ChartLineData(Enumerable.Empty<Point2D>(), "test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());

            // Call
            data.Remove(dataElement);

            // Assert
            CollectionAssert.IsEmpty(data.List);
        }

        [Test]
        public void Remove_Null_DoesNotRemove()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var dataElement = new ChartLineData(Enumerable.Empty<Point2D>(), "test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());
            var listBeforeRemove = data.List;

            // Call
            data.Remove(null);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, data.List);
        }

        [Test]
        public void Remove_NotExistingElement_DoesNotRemove()
        {
            // Setup
            var list = Enumerable.Empty<ChartData>().ToList();
            var data = new ChartDataCollection(list, "test");
            var dataElement = new ChartLineData(Enumerable.Empty<Point2D>(), "test");
            var otherDataElement = new ChartPointData(Enumerable.Empty<Point2D>(), "another test");

            data.Add(dataElement);

            // Precondition
            Assert.AreEqual(1, data.List.Count);
            Assert.IsInstanceOf<ChartLineData>(data.List.First());
            var listBeforeRemove = data.List;

            // Call
            data.Remove(otherDataElement);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, data.List);
        }
    }
}