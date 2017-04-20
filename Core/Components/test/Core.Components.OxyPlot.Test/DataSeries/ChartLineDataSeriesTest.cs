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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Components.OxyPlot.DataSeries;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.DataSeries
{
    [TestFixture]
    public class ChartLineDataSeriesTest
    {
        [Test]
        public void Constructor_WithoutChartLineData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartLineDataSeries(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("chartLineData", parameter);
        }

        [Test]
        public void Constructor_ChartLineDataWithTestProperties_ChartLineDataSeriesCreatedAccordingly()
        {
            // Setup
            var chartLineData = new ChartLineData("Test name");

            SetChartLineDataTestProperties(chartLineData);

            // Call
            var chartLineDataSeries = new ChartLineDataSeries(chartLineData);

            // Assert
            Assert.IsInstanceOf<LineSeries>(chartLineDataSeries);
            Assert.IsInstanceOf<IItemBasedChartDataSeries>(chartLineDataSeries);
            AssertChartLineDataSeriesTestProperties(chartLineDataSeries);
        }

        [Test]
        public void Update_ChartLineDataWithTestProperties_ChartLineDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartLineData = new ChartLineData("Test name");
            var chartLineDataSeries = new ChartLineDataSeries(chartLineData);

            SetChartLineDataTestProperties(chartLineData);

            // Precondition
            AssertChartLineDataSeriesDefaultProperties(chartLineDataSeries);

            // Call
            chartLineDataSeries.Update();

            // Assert
            AssertChartLineDataSeriesTestProperties(chartLineDataSeries);
        }

        [Test]
        public void GivenChartLineDataSeries_WhenUpdatedAfterChartLineDataPointsChanged_ChartLineDataSeriesPointsChanged()
        {
            // Given
            var chartLineData = new ChartLineData("Test name")
            {
                Points = new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };

            var chartLineDataSeries = new ChartLineDataSeries(chartLineData);
            IEnumerable<DataPoint> drawnPoints = chartLineDataSeries.ItemsSource.Cast<DataPoint>();

            // When
            chartLineData.Points = new[]
            {
                new Point2D(3.3, 4.4)
            };
            chartLineDataSeries.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnPoints, chartLineDataSeries.ItemsSource.Cast<DataPoint>());
        }

        [Test]
        public void GivenChartLineDataSeries_WhenUpdatedAndChartLineDataPointsNotChanged_PreviousChartLineDataSeriesPointsPreserved()
        {
            // Given
            var chartLineData = new ChartLineData("Test name")
            {
                Points = new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };

            var chartLineDataSeries = new ChartLineDataSeries(chartLineData);
            IEnumerable<DataPoint> drawnPoints = chartLineDataSeries.ItemsSource.Cast<DataPoint>();

            // When
            chartLineDataSeries.Update();

            // Then
            CollectionAssert.AreEqual(drawnPoints, chartLineDataSeries.ItemsSource.Cast<DataPoint>());
        }

        private static void SetChartLineDataTestProperties(ChartLineData chartLineData)
        {
            chartLineData.Name = "Another name";
            chartLineData.IsVisible = false;
            chartLineData.Style = new ChartLineStyle(Color.Blue, 3, DashStyle.DashDot);
            chartLineData.Points = new[]
            {
                new Point2D(1.1, 2.2)
            };
        }

        private static void AssertChartLineDataSeriesTestProperties(ChartLineDataSeries chartLineDataSeries)
        {
            Assert.AreEqual("Another name", chartLineDataSeries.Title);
            Assert.IsFalse(chartLineDataSeries.IsVisible);

            Assert.AreEqual(3, chartLineDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(Color.Blue.A, Color.Blue.R, Color.Blue.G, Color.Blue.B), chartLineDataSeries.Color);
            Assert.AreEqual(LineStyle.DashDot, chartLineDataSeries.LineStyle);

            Assert.AreEqual(1, chartLineDataSeries.ItemsSource.Cast<DataPoint>().Count());
        }

        private static void AssertChartLineDataSeriesDefaultProperties(ChartLineDataSeries chartLineDataSeries)
        {
            Assert.AreEqual("Test name", chartLineDataSeries.Title);
            Assert.IsTrue(chartLineDataSeries.IsVisible);

            Assert.AreEqual(2, chartLineDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(0, 0, 0, 1), chartLineDataSeries.Color);
            Assert.AreEqual(LineStyle.Automatic, chartLineDataSeries.LineStyle);

            Assert.AreEqual(0, chartLineDataSeries.ItemsSource.Cast<DataPoint>().Count());
        }
    }
}