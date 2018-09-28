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
    public class ChartMultipleAreaDataSeriesTest
    {
        private static readonly Color fillColor = Color.Red;
        private static readonly Color strokeColor = Color.Blue;

        [Test]
        public void Constructor_WithoutChartMultipleAreaData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartMultipleAreaDataSeries(null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("chartMultipleAreaData", parameter);
        }

        [Test]
        public void Constructor_ChartMultipleAreaDataWithTestProperties_ChartMultipleAreaDataSeriesCreatedAccordingly()
        {
            // Setup
            var chartMultipleAreaData = new ChartMultipleAreaData("Test name", new ChartAreaStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = 3
            });

            SetChartMultipleAreaDataTestProperties(chartMultipleAreaData);

            // Call
            var chartMultipleAreaDataSeries = new ChartMultipleAreaDataSeries(chartMultipleAreaData);

            // Assert
            Assert.IsInstanceOf<MultipleAreaSeries>(chartMultipleAreaDataSeries);
            Assert.IsInstanceOf<IChartDataSeries>(chartMultipleAreaDataSeries);
            AssertChartMultipleAreaDataSeriesTestProperties(chartMultipleAreaDataSeries);
        }

        [Test]
        public void Update_ChartMultipleAreaDataWithTestProperties_ChartMultipleAreaDataSeriesUpdatedAccordingly()
        {
            // Setup
            var chartMultipleAreaData = new ChartMultipleAreaData("Test name", new ChartAreaStyle
            {
                FillColor = fillColor,
                StrokeColor = strokeColor,
                StrokeThickness = 3
            });
            var chartMultipleAreaDataSeries = new ChartMultipleAreaDataSeries(chartMultipleAreaData);

            SetChartMultipleAreaDataTestProperties(chartMultipleAreaData);

            // Precondition
            AssertChartMultipleAreaDataSeriesDefaultProperties(chartMultipleAreaDataSeries);

            // Call
            chartMultipleAreaDataSeries.Update();

            // Assert
            AssertChartMultipleAreaDataSeriesTestProperties(chartMultipleAreaDataSeries);
        }

        [Test]
        public void GivenChartMultipleAreaDataSeries_WhenUpdatedAfterChartMultipleAreaDataAreasChanged_ChartMultipleAreaDataSeriesAreasChanged()
        {
            // Given
            var chartMultipleAreaData = new ChartMultipleAreaData("Test name")
            {
                Areas = new[]
                {
                    new[]
                    {
                        new Point2D(1.1, 2.2)
                    }
                }
            };

            var chartMultipleAreaDataSeries = new ChartMultipleAreaDataSeries(chartMultipleAreaData);
            IEnumerable<DataPoint>[] drawnAreas = chartMultipleAreaDataSeries.Areas.ToArray();

            // When
            chartMultipleAreaData.Areas = new[]
            {
                new[]
                {
                    new Point2D(3.3, 4.4)
                }
            };
            chartMultipleAreaDataSeries.Update();

            // Then
            CollectionAssert.AreNotEqual(drawnAreas, chartMultipleAreaDataSeries.Areas);
        }

        [Test]
        public void GivenChartMultipleAreaDataSeries_WhenUpdatedAndChartMultipleAreaDataAreasNotChanged_PreviousChartMultipleAreaDataSeriesAreasPreserved()
        {
            // Given
            var chartMultipleAreaData = new ChartMultipleAreaData("Test name")
            {
                Areas = new[]
                {
                    new[]
                    {
                        new Point2D(1.1, 2.2)
                    }
                }
            };

            var chartMultipleAreaDataSeries = new ChartMultipleAreaDataSeries(chartMultipleAreaData);
            IEnumerable<DataPoint>[] drawnAreas = chartMultipleAreaDataSeries.Areas.ToArray();

            // When
            chartMultipleAreaDataSeries.Update();

            // Then
            CollectionAssert.AreEqual(drawnAreas, chartMultipleAreaDataSeries.Areas);
        }

        private static void SetChartMultipleAreaDataTestProperties(ChartMultipleAreaData chartMultipleAreaData)
        {
            chartMultipleAreaData.Name = "Another name";
            chartMultipleAreaData.IsVisible = false;
            chartMultipleAreaData.Areas = new[]
            {
                new[]
                {
                    new Point2D(1.1, 2.2)
                }
            };
        }

        private static void AssertChartMultipleAreaDataSeriesTestProperties(ChartMultipleAreaDataSeries chartMultipleAreaDataSeries)
        {
            Assert.AreEqual("Another name", chartMultipleAreaDataSeries.Title);
            Assert.IsFalse(chartMultipleAreaDataSeries.IsVisible);

            Assert.AreEqual(3, chartMultipleAreaDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B), chartMultipleAreaDataSeries.Fill);
            Assert.AreEqual(OxyColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B), chartMultipleAreaDataSeries.Color);

            Assert.AreEqual(1, chartMultipleAreaDataSeries.Areas.Count);
        }

        private static void AssertChartMultipleAreaDataSeriesDefaultProperties(ChartMultipleAreaDataSeries chartMultipleAreaDataSeries)
        {
            Assert.AreEqual("Test name", chartMultipleAreaDataSeries.Title);
            Assert.IsTrue(chartMultipleAreaDataSeries.IsVisible);

            Assert.AreEqual(3, chartMultipleAreaDataSeries.StrokeThickness);
            Assert.AreEqual(OxyColor.FromArgb(fillColor.A, fillColor.R, fillColor.G, fillColor.B), chartMultipleAreaDataSeries.Fill);
            Assert.AreEqual(OxyColor.FromArgb(strokeColor.A, strokeColor.R, strokeColor.G, strokeColor.B), chartMultipleAreaDataSeries.Color);

            Assert.AreEqual(0, chartMultipleAreaDataSeries.Areas.Count);
        }
    }
}