﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Components.OxyPlot.CustomSeries;
using Core.Components.OxyPlot.DataSeries;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.DataSeries
{
    [TestFixture]
    public class ChartMultipleLineDataSeriesTest
    {
        [Test]
        public void Constructor_ChartMultipleLineDataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartMultipleLineDataSeries(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("chartMultipleLineData", parameter);
        }

        [Test]
        public void Constructor_ChartMultipleLineDataWithTestProperties_ChartMultipleLineDataSeriesCreatedAccordingly()
        {
            // Setup
            var chartMultipleLineData = new ChartMultipleLineData("Test name");

            SetChartMultipleLineDataTestProperties(chartMultipleLineData);

            // Call
            var chartMultipleLineDataSeries = new ChartMultipleLineDataSeries(chartMultipleLineData);

            // Assert
            Assert.IsInstanceOf<MultipleLineSeries>(chartMultipleLineDataSeries);
            Assert.IsInstanceOf<IItemBasedChartDataSeries>(chartMultipleLineDataSeries);
            AssertChartMultipleLineDataSeriesTestProperties(chartMultipleLineDataSeries);
        }

        [Test]
        public void Update_ChartMultipleLineDataWithTestProperties_ChartMultipleLineDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartMultipleLineData = new ChartMultipleLineData("Test name");
            var chartMultipleLineDataSeries = new ChartMultipleLineDataSeries(chartMultipleLineData);

            SetChartMultipleLineDataTestProperties(chartMultipleLineData);

            // Precondition
            AssertChartMultipleLineDataSeriesDefaultProperties(chartMultipleLineDataSeries);

            // Call
            chartMultipleLineDataSeries.Update();

            // Assert
            AssertChartMultipleLineDataSeriesTestProperties(chartMultipleLineDataSeries);
        }

        [Test]
        public void GivenChartMultipleLineDataSeries_WhenUpdatedAfterChartMultipleLineDataLinesChanged_ChartMultipleLineDataSeriesLinesChanged()
        {
            // Given
            var chartMultipleLineData = new ChartMultipleLineData("Test name")
            {
                Lines = new[]
                {
                    new[]
                    {
                        new Point2D(1.1, 2.2)
                    }
                }
            };

            var chartMultipleLineDataSeries = new ChartMultipleLineDataSeries(chartMultipleLineData);
            DataPoint[][] drawnLines = chartMultipleLineDataSeries.Lines.ToArray();

            // When
            chartMultipleLineData.Lines = new[]
            {
                new[]
                {
                    new Point2D(3.3, 4.4)
                }
            };
            chartMultipleLineDataSeries.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnLines, chartMultipleLineDataSeries.Lines);
        }

        [Test]
        public void GivenChartMultipleLineDataSeries_WhenUpdatedAndChartMultipleLineDataLinesNotChanged_PreviousChartMultipleLineDataSeriesLinesPreserved()
        {
            // Given
            var chartMultipleLineData = new ChartMultipleLineData("Test name")
            {
                Lines = new[]
                {
                    new[]
                    {
                        new Point2D(1.1, 2.2)
                    }
                }
            };

            var chartMultipleLineDataSeries = new ChartMultipleLineDataSeries(chartMultipleLineData);
            DataPoint[][] drawnLines = chartMultipleLineDataSeries.Lines.ToArray();

            // When
            chartMultipleLineDataSeries.Update();

            // Then
            CollectionAssert.AreEqual(drawnLines, chartMultipleLineDataSeries.Lines);
        }

        private static void SetChartMultipleLineDataTestProperties(ChartMultipleLineData chartMultipleLineData)
        {
            chartMultipleLineData.Name = "Another name";
            chartMultipleLineData.IsVisible = false;
            chartMultipleLineData.Style = new ChartLineStyle(Color.Red, 3, DashStyle.Dash);
            chartMultipleLineData.Lines = new[]
            {
                new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };
        }

        private static void AssertChartMultipleLineDataSeriesTestProperties(ChartMultipleLineDataSeries chartMultipleLineDataSeries)
        {
            Assert.AreEqual("Another name", chartMultipleLineDataSeries.Title);
            Assert.IsFalse(chartMultipleLineDataSeries.IsVisible);

            Assert.AreEqual(3, chartMultipleLineDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(Color.Red.A, Color.Red.R, Color.Red.G, Color.Red.B), chartMultipleLineDataSeries.Color);
            Assert.AreEqual(LineStyle.Dash, chartMultipleLineDataSeries.LineStyle);

            Assert.AreEqual(1, chartMultipleLineDataSeries.Lines.Count);
        }

        private static void AssertChartMultipleLineDataSeriesDefaultProperties(ChartMultipleLineDataSeries chartMultipleLineDataSeries)
        {
            Assert.AreEqual("Test name", chartMultipleLineDataSeries.Title);
            Assert.IsTrue(chartMultipleLineDataSeries.IsVisible);

            Assert.AreEqual(0, chartMultipleLineDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(0, 0, 0, 1), chartMultipleLineDataSeries.Color);
            Assert.AreEqual(LineStyle.Solid, chartMultipleLineDataSeries.LineStyle);

            Assert.AreEqual(0, chartMultipleLineDataSeries.Lines.Count);
        }
    }
}