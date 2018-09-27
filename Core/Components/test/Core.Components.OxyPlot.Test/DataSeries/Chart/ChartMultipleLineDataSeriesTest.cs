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
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Components.OxyPlot.CustomSeries;
using Core.Components.OxyPlot.DataSeries.Chart;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.DataSeries.Chart
{
    [TestFixture]
    public class ChartMultipleLineDataSeriesTest
    {
        private static readonly Color color = Color.Red;

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
            var chartMultipleLineData = new ChartMultipleLineData("Test name", new ChartLineStyle
            {
                Color = color,
                Width = 3,
                DashStyle = ChartLineDashStyle.Dash
            });

            SetChartMultipleLineDataTestProperties(chartMultipleLineData);

            // Call
            var chartMultipleLineDataSeries = new ChartMultipleLineDataSeries(chartMultipleLineData);

            // Assert
            Assert.IsInstanceOf<MultipleLineSeries>(chartMultipleLineDataSeries);
            Assert.IsInstanceOf<IChartDataSeries>(chartMultipleLineDataSeries);
            AssertChartMultipleLineDataSeriesTestProperties(chartMultipleLineDataSeries);
        }

        [Test]
        public void Update_ChartMultipleLineDataWithTestProperties_ChartMultipleLineDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartMultipleLineData = new ChartMultipleLineData("Test name", new ChartLineStyle
            {
                Color = color,
                Width = 3,
                DashStyle = ChartLineDashStyle.Dash
            });
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
            IEnumerable<DataPoint>[] drawnLines = chartMultipleLineDataSeries.Lines.ToArray();

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
            IEnumerable<DataPoint>[] drawnLines = chartMultipleLineDataSeries.Lines.ToArray();

            // When
            chartMultipleLineDataSeries.Update();

            // Then
            CollectionAssert.AreEqual(drawnLines, chartMultipleLineDataSeries.Lines);
        }

        private static void SetChartMultipleLineDataTestProperties(ChartMultipleLineData chartMultipleLineData)
        {
            chartMultipleLineData.Name = "Another name";
            chartMultipleLineData.IsVisible = false;
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
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), chartMultipleLineDataSeries.Color);
            Assert.AreEqual(LineStyle.Dash, chartMultipleLineDataSeries.LineStyle);

            Assert.AreEqual(1, chartMultipleLineDataSeries.Lines.Count);
        }

        private static void AssertChartMultipleLineDataSeriesDefaultProperties(ChartMultipleLineDataSeries chartMultipleLineDataSeries)
        {
            Assert.AreEqual("Test name", chartMultipleLineDataSeries.Title);
            Assert.IsTrue(chartMultipleLineDataSeries.IsVisible);

            Assert.AreEqual(3, chartMultipleLineDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), chartMultipleLineDataSeries.Color);
            Assert.AreEqual(LineStyle.Dash, chartMultipleLineDataSeries.LineStyle);

            Assert.AreEqual(0, chartMultipleLineDataSeries.Lines.Count);
        }
    }
}