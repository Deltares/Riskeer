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
using Core.Components.Chart.Data;
using Core.Components.Chart.TestUtil;
using NUnit.Framework;

namespace Core.Components.Chart.Test.Data
{
    [TestFixture]
    public class ChartDataCollectionTest
    {
        [Test]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase(null)]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new ChartDataCollection(invalidName);

            // Assert
            const string expectedMessage = "A name must be set to the chart data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_ValidName_NameAndDefaultValuesSet()
        {
            // Setup
            const string name = "Some name";

            // Call
            var chartDataCollection = new ChartDataCollection(name);

            // Assert
            Assert.AreEqual(name, chartDataCollection.Name);
            Assert.IsInstanceOf<ChartData>(chartDataCollection);
            CollectionAssert.IsEmpty(chartDataCollection.Collection);
        }

        [Test]
        public void Add_NotNull_AddsItemToCollection()
        {
            // Setup
            var item = new ChartLineData("test");
            var chartDataCollection = new ChartDataCollection("test");

            // Call
            chartDataCollection.Add(item);

            // Assert
            List<ChartData> chartData = chartDataCollection.Collection.ToList();
            Assert.AreEqual(1, chartData.Count);
            Assert.AreSame(item, chartData.First());
        }

        [Test]
        public void Add_Null_ThrowsArgumentNullException()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("test");

            // Call
            TestDelegate call = () => chartDataCollection.Add(null);

            // Assert
            const string expectedMessage = "An item cannot be null when adding it to the collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Insert_ItemNotNullAndValidIndex_InsertsItemToCollectionAtGivenIndex()
        {
            // Setup
            var itemToInsert = new ChartLineData("test");
            var existingItem = new TestChartData("test");
            var chartDataCollection = new ChartDataCollection("test");

            chartDataCollection.Add(existingItem);

            // Precondition
            Assert.AreEqual(1, chartDataCollection.Collection.Count());
            Assert.AreSame(existingItem, chartDataCollection.Collection.ElementAt(0));

            // Call
            chartDataCollection.Insert(0, itemToInsert);

            // Assert
            Assert.AreEqual(2, chartDataCollection.Collection.Count());
            Assert.AreSame(itemToInsert, chartDataCollection.Collection.ElementAt(0));
            Assert.AreSame(existingItem, chartDataCollection.Collection.ElementAt(1));
        }

        [Test]
        public void Insert_ItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("test");

            // Call
            TestDelegate call = () => chartDataCollection.Insert(0, null);

            // Assert
            const string expectedMessage = "An item cannot be null when adding it to the collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Remove_ExistingItem_RemovesItem()
        {
            // Setup
            var item = new ChartLineData("test");
            var chartDataCollection = new ChartDataCollection("test");

            chartDataCollection.Add(item);

            // Precondition
            Assert.AreEqual(1, chartDataCollection.Collection.Count());
            Assert.IsInstanceOf<ChartLineData>(chartDataCollection.Collection.First());

            // Call
            chartDataCollection.Remove(item);

            // Assert
            CollectionAssert.IsEmpty(chartDataCollection.Collection);
        }

        [Test]
        public void Remove_Null_DoesNotRemove()
        {
            // Setup
            var item = new ChartLineData("test");
            var chartDataCollection = new ChartDataCollection("test");

            chartDataCollection.Add(item);

            // Precondition
            Assert.AreEqual(1, chartDataCollection.Collection.Count());
            Assert.IsInstanceOf<ChartLineData>(chartDataCollection.Collection.First());
            List<ChartData> listBeforeRemove = chartDataCollection.Collection.ToList();

            // Call
            chartDataCollection.Remove(null);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, chartDataCollection.Collection);
        }

        [Test]
        public void Remove_NotExistingItem_DoesNotRemove()
        {
            // Setup
            var item = new ChartLineData("test");
            var otherItem = new ChartPointData("another test");
            var chartDataCollection = new ChartDataCollection("test");

            chartDataCollection.Add(item);

            // Precondition
            Assert.AreEqual(1, chartDataCollection.Collection.Count());
            Assert.IsInstanceOf<ChartLineData>(chartDataCollection.Collection.First());
            List<ChartData> listBeforeRemove = chartDataCollection.Collection.ToList();

            // Call
            chartDataCollection.Remove(otherItem);

            // Assert
            CollectionAssert.AreEqual(listBeforeRemove, chartDataCollection.Collection);
        }

        [Test]
        public void Clear_Always_RemovesAllItems()
        {
            // Setup
            var item1 = new ChartLineData("test");
            var item2 = new ChartLineData("test");
            var chartDataCollection = new ChartDataCollection("test");

            chartDataCollection.Add(item1);
            chartDataCollection.Add(item2);

            // Precondition
            Assert.AreEqual(2, chartDataCollection.Collection.Count());

            // Call
            chartDataCollection.Clear();

            // Assert
            CollectionAssert.IsEmpty(chartDataCollection.Collection);
        }

        [TestCase(-1)]
        [TestCase(2)]
        public void Insert_InvalidIndex_ThrowsArgumentOutOfRangeException(int invalidIndex)
        {
            // Setup
            var itemToInsert = new ChartLineData("test");
            var existingItem = new TestChartData("test");
            var chartDataCollection = new ChartDataCollection("test");

            chartDataCollection.Add(existingItem);

            // Call
            TestDelegate call = () => chartDataCollection.Insert(invalidIndex, itemToInsert);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(call, "index");
        }
    }
}