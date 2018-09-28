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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Components.OxyPlot.DataSeries.Chart;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.DataSeries.Chart
{
    [TestFixture]
    public class ChartLineDataSeriesTest
    {
        private static readonly Color color = Color.Blue;

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
            var chartLineData = new ChartLineData("Test name", new ChartLineStyle
            {
                Color = color,
                Width = 3,
                DashStyle = ChartLineDashStyle.DashDot
            });

            SetChartLineDataTestProperties(chartLineData);

            // Call
            var chartLineDataSeries = new ChartLineDataSeries(chartLineData);

            // Assert
            Assert.IsInstanceOf<LineSeries>(chartLineDataSeries);
            Assert.IsInstanceOf<IChartDataSeries>(chartLineDataSeries);
            AssertChartLineDataSeriesTestProperties(chartLineDataSeries);
        }

        [Test]
        public void Update_ChartLineDataWithTestProperties_ChartLineDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartLineData = new ChartLineData("Test name", new ChartLineStyle
            {
                Color = color,
                Width = 3,
                DashStyle = ChartLineDashStyle.DashDot
            });
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
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), chartLineDataSeries.Color);
            Assert.AreEqual(LineStyle.DashDot, chartLineDataSeries.LineStyle);

            Assert.AreEqual(1, chartLineDataSeries.ItemsSource.Cast<DataPoint>().Count());
        }

        private static void AssertChartLineDataSeriesDefaultProperties(ChartLineDataSeries chartLineDataSeries)
        {
            Assert.AreEqual("Test name", chartLineDataSeries.Title);
            Assert.IsTrue(chartLineDataSeries.IsVisible);

            Assert.AreEqual(3, chartLineDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), chartLineDataSeries.Color);
            Assert.AreEqual(LineStyle.DashDot, chartLineDataSeries.LineStyle);

            Assert.AreEqual(0, chartLineDataSeries.ItemsSource.Cast<DataPoint>().Count());
        }
    }
}