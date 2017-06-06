// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
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
            var data = new ChartLineData("test", new ChartLineStyle(expectedColor, 3, DashStyle.Solid));

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
            var data = new ChartLineData("test", new ChartLineStyle(Color.Red, width, DashStyle.Solid));

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            Assert.AreEqual(width, lineSeries.StrokeThickness);
        }

        [Test]
        [TestCase(DashStyle.Solid, LineStyle.Solid)]
        [TestCase(DashStyle.Dash, LineStyle.Dash)]
        [TestCase(DashStyle.Dot, LineStyle.Dot)]
        [TestCase(DashStyle.DashDot, LineStyle.DashDot)]
        [TestCase(DashStyle.DashDotDot, LineStyle.DashDotDot)]
        public void ConvertSeriesProperties_ChartLineStyleSetWithDifferentDashStyles_AppliesStyleToSeries(DashStyle dashStyle, LineStyle expectedLineStyle)
        {
            // Setup
            var converter = new ChartLineDataConverter();
            var lineSeries = new LineSeries();
            var data = new ChartLineData("test", new ChartLineStyle(Color.Red, 3, dashStyle));

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