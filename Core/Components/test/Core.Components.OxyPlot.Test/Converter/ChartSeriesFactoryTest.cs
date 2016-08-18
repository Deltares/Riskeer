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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using Core.Components.OxyPlot.CustomSeries;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartSeriesFactoryTest
    {
        [Test]
        public void Create_AreaData_ReturnsAreaSeries()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var expectedData = CreateExpectedData(GetTestPointsFunc());

            // Call
            IList<Series> series = factory.Create(new ChartAreaData("test data")
            {
                Points = GetTestPointsFunc()
            });

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var areaSeries = (AreaSeries) series[0];
            CollectionAssert.AreEqual(expectedData, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint>
            {
                expectedData.First()
            }, areaSeries.Points2);
        }

        [Test]
        public void Create_LineData_ReturnsLineSeries()
        {
            // Setup
            var factory = new ChartSeriesFactory();

            // Call
            IList<Series> series = factory.Create(new ChartLineData("test data")
            {
                Points = GetTestPointsFunc()
            });

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var lineSeries = (LineSeries) series[0];
            CollectionAssert.AreEqual(GetTestPointsFunc(), lineSeries.ItemsSource);
            Assert.AreNotSame(GetTestPointsFunc(), lineSeries.ItemsSource);
        }

        [Test]
        public void Create_PointData_ReturnsLinesSeriesWithPointStyle()
        {
            // Setup
            var factory = new ChartSeriesFactory();

            // Call
            IList<Series> series = factory.Create(new ChartPointData("test data")
            {
                Points = GetTestPointsFunc()
            });

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var lineSeries = (LineSeries) series[0];
            CollectionAssert.AreEqual(GetTestPointsFunc(), lineSeries.ItemsSource);
            Assert.AreNotSame(GetTestPointsFunc(), lineSeries.ItemsSource);
            Assert.AreEqual(LineStyle.None, lineSeries.LineStyle);
            Assert.AreEqual(MarkerType.Circle, lineSeries.MarkerType);
        }

        [Test]
        public void Create_MultipleAreaData_ReturnsLinesSeriesWithAreaStyle()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var testAreaA = GetTestPointsFunc();
            var testAreaB = GetTestPointsFunc();
            var expectedDataA = CreateExpectedData(testAreaA);
            var expectedDataB = CreateExpectedData(testAreaB);

            // Call
            IList<Series> series = factory.Create(new ChartMultipleAreaData("test data")
            {
                Areas = new[]
                {
                    testAreaA,
                    testAreaB
                }
            });

            // Assert
            Assert.AreEqual(1, series.Count);
            Assert.IsInstanceOf<IList<Series>>(series);
            var multipleAreaSeries = (MultipleAreaSeries) series[0];
            CollectionAssert.AreEqual(expectedDataA, multipleAreaSeries.Areas[0]);
            CollectionAssert.AreEqual(expectedDataB, multipleAreaSeries.Areas[1]);
        }

        [Test]
        public void Create_OtherData_ThrowsNotSupportedException()
        {
            // Setup
            var factory = new ChartSeriesFactory();
            var testData = new TestChartData();

            // Call
            TestDelegate test = () => factory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private static ICollection<DataPoint> CreateExpectedData(IEnumerable<Point2D> testData)
        {
            return testData.Select(p => new DataPoint(p.X, p.Y)).ToArray();
        }

        private static Point2D[] GetTestPointsFunc()
        {
            return new[]
            {
                new Point2D(1.2, 3.4),
                new Point2D(3.2, 3.4),
                new Point2D(0.2, 2.4)
            };
        }
    }
}