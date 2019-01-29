// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Drawing;
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
    public class ChartAreaDataSeriesTest
    {
        private static readonly Color fillColor = Color.Red;
        private static readonly Color strokeColor = Color.Blue;

        [Test]
        public void Constructor_WithoutChartAreaData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartAreaDataSeries(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("chartAreaData", parameter);
        }

        [Test]
        public void Constructor_ChartAreaDataWithTestProperties_ChartAreaDataSeriesCreatedAccordingly()
        {
            // Setup
            var chartAreaData = new ChartAreaData("Test name", new ChartAreaStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = 3
            });

            SetChartAreaDataTestProperties(chartAreaData);

            // Call
            var chartAreaDataSeries = new ChartAreaDataSeries(chartAreaData);

            // Assert
            Assert.IsInstanceOf<AreaSeries>(chartAreaDataSeries);
            Assert.IsInstanceOf<IChartDataSeries>(chartAreaDataSeries);
            AssertChartAreaDataSeriesTestProperties(chartAreaDataSeries);
        }

        [Test]
        public void Update_ChartAreaDataWithTestProperties_ChartAreaDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartAreaData = new ChartAreaData("Test name", new ChartAreaStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = 3
            });
            var chartAreaDataSeries = new ChartAreaDataSeries(chartAreaData);

            SetChartAreaDataTestProperties(chartAreaData);

            // Precondition
            AssertChartAreaDataSeriesDefaultProperties(chartAreaDataSeries);

            // Call
            chartAreaDataSeries.Update();

            // Assert
            AssertChartAreaDataSeriesTestProperties(chartAreaDataSeries);
        }

        [Test]
        public void GivenChartAreaDataSeries_WhenUpdatedAfterChartAreaDataPointsChanged_ChartAreaDataSeriesPointsChanged()
        {
            // Given
            var chartAreaData = new ChartAreaData("Test name")
            {
                Points = new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };

            var chartAreaDataSeries = new ChartAreaDataSeries(chartAreaData);
            DataPoint[] drawnPoints = chartAreaDataSeries.Points.ToArray();

            // When
            chartAreaData.Points = new[]
            {
                new Point2D(3.3, 4.4)
            };
            chartAreaDataSeries.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnPoints, chartAreaDataSeries.Points);
        }

        [Test]
        public void GivenChartAreaDataSeries_WhenUpdatedAndChartAreaDataPointsNotChanged_PreviousChartAreaDataSeriesPointsPreserved()
        {
            // Given
            var chartAreaData = new ChartAreaData("Test name")
            {
                Points = new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };

            var chartAreaDataSeries = new ChartAreaDataSeries(chartAreaData);
            DataPoint[] drawnPoints = chartAreaDataSeries.Points.ToArray();

            // When
            chartAreaDataSeries.Update();

            // Then
            CollectionAssert.AreEqual(drawnPoints, chartAreaDataSeries.Points);
        }

        private static void SetChartAreaDataTestProperties(ChartAreaData chartAreaData)
        {
            chartAreaData.Name = "Another name";
            chartAreaData.IsVisible = false;
            chartAreaData.Points = new[]
            {
                new Point2D(1.1, 2.2)
            };
        }

        private static void AssertChartAreaDataSeriesTestProperties(ChartAreaDataSeries chartAreaDataSeries)
        {
            Assert.AreEqual("Another name", chartAreaDataSeries.Title);
            Assert.IsFalse(chartAreaDataSeries.IsVisible);

            Assert.AreEqual(3, chartAreaDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B), chartAreaDataSeries.Fill);
            Assert.AreEqual(OxyColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B), chartAreaDataSeries.Color);

            Assert.AreEqual(1, chartAreaDataSeries.Points.Count);
        }

        private static void AssertChartAreaDataSeriesDefaultProperties(ChartAreaDataSeries chartAreaDataSeries)
        {
            Assert.AreEqual("Test name", chartAreaDataSeries.Title);
            Assert.IsTrue(chartAreaDataSeries.IsVisible);

            Assert.AreEqual(3, chartAreaDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B), chartAreaDataSeries.Fill);
            Assert.AreEqual(OxyColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B), chartAreaDataSeries.Color);

            Assert.AreEqual(0, chartAreaDataSeries.Points.Count);
        }
    }
}