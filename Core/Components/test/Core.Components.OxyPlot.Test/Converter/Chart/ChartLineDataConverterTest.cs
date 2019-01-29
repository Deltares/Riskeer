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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Components.OxyPlot.Converter.Chart;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter.Chart
{
    [TestFixture]
    public class ChartLineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartLineDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartLineData, LineSeries>>(converter);
        }

        [Test]
        public void ConvertSeriesItems_ChartLineDataWithRandomPointData_ConvertsAllPointsToLineSeries()
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var lineSeries = new LineSeries();
            var random = new Random(21);
            int randomCount = random.Next(5, 10);
            var points = new Collection<Point2D>();

            for (var i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var lineData = new ChartLineData("test data")
            {
                Points = points.ToArray()
            };

            // Call
            converter.ConvertSeriesData(lineData, lineSeries);

            // Assert
            CollectionAssert.AreEqual(points.Select(p => new DataPoint(p.X, p.Y)), lineSeries.ItemsSource);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_ChartLineStyleSetWithDifferentColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var lineSeries = new LineSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartLineData("test", new ChartLineStyle
            {
                Color = expectedColor,
                Width = 3,
                DashStyle = ChartLineDashStyle.Solid
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            AssertColors(expectedColor, lineSeries.Color);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void ConvertSeriesProperties_ChartLineStyleSetWithDifferentWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var lineSeries = new LineSeries();
            var data = new ChartLineData("test", new ChartLineStyle
            {
                Color = Color.Red,
                Width = width,
                DashStyle = ChartLineDashStyle.Solid
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            Assert.AreEqual(width, lineSeries.StrokeThickness);
        }

        [Test]
        [TestCase(ChartLineDashStyle.Solid, LineStyle.Solid)]
        [TestCase(ChartLineDashStyle.Dash, LineStyle.Dash)]
        [TestCase(ChartLineDashStyle.Dot, LineStyle.Dot)]
        [TestCase(ChartLineDashStyle.DashDot, LineStyle.DashDot)]
        [TestCase(ChartLineDashStyle.DashDotDot, LineStyle.DashDotDot)]
        public void ConvertSeriesProperties_ChartLineStyleSetWithDifferentDashStyles_AppliesStyleToSeries(ChartLineDashStyle dashStyle, LineStyle expectedLineStyle)
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var lineSeries = new LineSeries();
            var data = new ChartLineData("test", new ChartLineStyle
            {
                Color = Color.Red,
                Width = 3,
                DashStyle = dashStyle
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            Assert.AreEqual(expectedLineStyle, lineSeries.LineStyle);
        }

        private static void AssertColors(Color color, OxyColor oxyColor)
        {
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), oxyColor);
        }
    }
}