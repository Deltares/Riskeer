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
    public class ChartPointDataSeriesTest
    {
        private static readonly Color color = Color.Red;
        private static readonly Color strokeColor = Color.Blue;

        [Test]
        public void Constructor_WithoutChartPointData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartPointDataSeries(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("chartPointData", parameter);
        }

        [Test]
        public void Constructor_ChartPointDataWithTestProperties_ChartPointDataSeriesCreatedAccordingly()
        {
            // Setup
            var chartPointData = new ChartPointData("Test name", new ChartPointStyle
            {
                Color = color,
                StrokeColor = strokeColor,
                Size = 4,
                StrokeThickness = 2,
                Symbol = ChartPointSymbol.Circle
            });

            SetChartPointDataTestProperties(chartPointData);

            // Call
            var chartPointDataSeries = new ChartPointDataSeries(chartPointData);

            // Assert
            Assert.IsInstanceOf<LineSeries>(chartPointDataSeries);
            Assert.IsInstanceOf<IChartDataSeries>(chartPointDataSeries);
            AssertChartPointDataSeriesTestProperties(chartPointDataSeries);
        }

        [Test]
        public void Update_ChartPointDataWithTestProperties_ChartPointDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartPointData = new ChartPointData("Test name", new ChartPointStyle
            {
                Color = color,
                StrokeColor = strokeColor,
                Size = 4,
                StrokeThickness = 2,
                Symbol = ChartPointSymbol.Circle
            });
            var chartPointDataSeries = new ChartPointDataSeries(chartPointData);

            SetChartPointDataTestProperties(chartPointData);

            // Precondition
            AssertChartPointDataSeriesDefaultProperties(chartPointDataSeries);

            // Call
            chartPointDataSeries.Update();

            // Assert
            AssertChartPointDataSeriesTestProperties(chartPointDataSeries);
        }

        [Test]
        public void GivenChartPointDataSeries_WhenUpdatedAfterChartPointDataPointsChanged_ChartPointDataSeriesPointsChanged()
        {
            // Given
            var chartPointData = new ChartPointData("Test name")
            {
                Points = new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };

            var chartPointDataSeries = new ChartPointDataSeries(chartPointData);
            IEnumerable<DataPoint> drawnPoints = chartPointDataSeries.ItemsSource.Cast<DataPoint>();

            // When
            chartPointData.Points = new[]
            {
                new Point2D(3.3, 4.4)
            };
            chartPointDataSeries.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnPoints, chartPointDataSeries.ItemsSource.Cast<DataPoint>());
        }

        [Test]
        public void GivenChartPointDataSeries_WhenUpdatedAndChartPointDataPointsNotChanged_PreviousChartPointDataSeriesPointsPreserved()
        {
            // Given
            var chartPointData = new ChartPointData("Test name")
            {
                Points = new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };

            var chartPointDataSeries = new ChartPointDataSeries(chartPointData);
            IEnumerable<DataPoint> drawnPoints = chartPointDataSeries.ItemsSource.Cast<DataPoint>();

            // When
            chartPointDataSeries.Update();

            // Then
            CollectionAssert.AreEqual(drawnPoints, chartPointDataSeries.ItemsSource.Cast<DataPoint>());
        }

        private static void SetChartPointDataTestProperties(ChartPointData chartPointData)
        {
            chartPointData.Name = "Another name";
            chartPointData.IsVisible = false;
            chartPointData.Points = new[]
            {
                new Point2D(1.1, 2.2)
            };
        }

        private static void AssertChartPointDataSeriesTestProperties(ChartPointDataSeries chartPointDataSeries)
        {
            Assert.AreEqual("Another name", chartPointDataSeries.Title);
            Assert.IsFalse(chartPointDataSeries.IsVisible);

            Assert.AreEqual(4, chartPointDataSeries.MarkerSize);
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), chartPointDataSeries.MarkerFill);
            Assert.AreEqual(OxyColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B), chartPointDataSeries.MarkerStroke);
            Assert.AreEqual(2, chartPointDataSeries.MarkerStrokeThickness);
            Assert.AreEqual(MarkerType.Circle, chartPointDataSeries.MarkerType);

            Assert.AreEqual(1, chartPointDataSeries.ItemsSource.Cast<DataPoint>().Count());
        }

        private static void AssertChartPointDataSeriesDefaultProperties(ChartPointDataSeries chartPointDataSeries)
        {
            Assert.AreEqual("Test name", chartPointDataSeries.Title);
            Assert.IsTrue(chartPointDataSeries.IsVisible);

            Assert.AreEqual(4, chartPointDataSeries.MarkerSize);
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), chartPointDataSeries.MarkerFill);
            Assert.AreEqual(OxyColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B), chartPointDataSeries.MarkerStroke);
            Assert.AreEqual(2, chartPointDataSeries.MarkerStrokeThickness);
            Assert.AreEqual(MarkerType.Circle, chartPointDataSeries.MarkerType);

            Assert.AreEqual(0, chartPointDataSeries.ItemsSource.Cast<DataPoint>().Count());
        }
    }
}