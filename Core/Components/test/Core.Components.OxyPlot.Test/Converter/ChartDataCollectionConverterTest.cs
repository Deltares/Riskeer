﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartDataCollectionConverterTest
    {

        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartDataCollectionConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartDataCollection>>(converter);
        }

        [Test]
        public void CanConvertSeries_ChartDataCollection_ReturnTrue()
        {
            // Setup
            var converter = new ChartDataCollectionConverter();
            var collectionData = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var canConvert = converter.CanConvertSeries(collectionData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartDataCollectionConverter();
            var chartData = new TestChartData();

            // Call
            var canConvert = converter.CanConvertSeries(chartData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_CollectionOfAreaDataAndLineData_ReturnsTwoNewSeries()
        {
            // Setup
            var converter = new ChartDataCollectionConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var pointsArea = new Collection<Tuple<double, double>>();
            var pointsLine = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                pointsArea.Add(Tuple.Create(random.NextDouble(), random.NextDouble()));
                pointsLine.Add(Tuple.Create(random.NextDouble(), random.NextDouble()));
            }

            var collectionData = new ChartDataCollection(new List<ChartData>(), "test data");
            var areaData = new ChartAreaData(pointsArea, "test data");
            var lineData = new ChartLineData(pointsLine, "test data");

            collectionData.List.Add(areaData);
            collectionData.List.Add(lineData);

            // Call
            var series = converter.Convert(collectionData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            Assert.AreEqual(2, series.Count);
            Assert.IsInstanceOf<AreaSeries>(series[0]);
            Assert.IsInstanceOf<LineSeries>(series[1]);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new ChartDataCollectionConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new ChartDataCollectionConverter();
            var testChartData = new TestChartData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testChartData.GetType());
            // Precondition
            Assert.IsFalse(testConverter.CanConvertSeries(testChartData));

            // Call
            TestDelegate test = () => testConverter.Convert(testChartData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }
    }
}