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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Components.OxyPlot.Converter;
using Core.Components.OxyPlot.CustomSeries;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.Converter
{
    [TestFixture]
    public class ChartMultipleLineDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartMultipleLineDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartMultipleLineData, MultipleLineSeries>>(converter);
        }

        [Test]
        public void ConvertSeriesItems_ChartMultipleLineDataWithRandomLineData_ConvertsAllLinesToMultipleLineSeries()
        {
            // Setup
            var converter = new ChartMultipleLineDataConverter();
            var multipleLineSeries = new MultipleLineSeries();
            var random = new Random(21);
            int randomCount = random.Next(5, 10);

            var points1 = new Collection<Point2D>();
            var points2 = new Collection<Point2D>();

            for (var i = 0; i < randomCount; i++)
            {
                points1.Add(new Point2D(random.NextDouble(), random.NextDouble()));
                points2.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var lines = new List<Point2D[]>
            {
                points1.ToArray(),
                points2.ToArray()
            };

            var lineData = new ChartMultipleLineData("test data")
            {
                Lines = lines
            };

            // Call
            converter.ConvertSeriesData(lineData, multipleLineSeries);

            // Assert
            Assert.AreEqual(2, multipleLineSeries.Lines.Count);
            CollectionAssert.AreEqual(lines.ElementAt(0).Select(t => new DataPoint(t.X, t.Y)), multipleLineSeries.Lines[0]);
            CollectionAssert.AreEqual(lines.ElementAt(1).Select(t => new DataPoint(t.X, t.Y)), multipleLineSeries.Lines[1]);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_ChartLineStyleSetWithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartMultipleLineDataConverter();
            var multipleLineSeries = new MultipleLineSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartMultipleLineData("test")
            {
                Style = new ChartLineStyle(expectedColor, 3, DashStyle.Solid)
            };

            // Call
            converter.ConvertSeriesProperties(data, multipleLineSeries);

            // Assert
            Assert.AreEqual(OxyColor.FromArgb(expectedColor.A, expectedColor.R, expectedColor.G, expectedColor.B), multipleLineSeries.Color);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void ConvertSeriesProperties_ChartLineStyleSetWithDifferentStrokeWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartMultipleLineDataConverter();
            var multipleLineSeries = new MultipleLineSeries();
            var data = new ChartMultipleLineData("test")
            {
                Style = new ChartLineStyle(Color.Red, width, DashStyle.Solid)
            };

            // Call
            converter.ConvertSeriesProperties(data, multipleLineSeries);

            // Assert
            Assert.AreEqual(width, multipleLineSeries.StrokeThickness);
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
            var converter = new ChartMultipleLineDataConverter();
            var multipleLineSeries = new MultipleLineSeries();
            var data = new ChartMultipleLineData("test")
            {
                Style = new ChartLineStyle(Color.Red, 3, dashStyle)
            };

            // Call
            converter.ConvertSeriesProperties(data, multipleLineSeries);

            // Assert
            Assert.AreEqual(expectedLineStyle, multipleLineSeries.LineStyle);
            Assert.IsNull(multipleLineSeries.Dashes);
        }

        [Test]
        public void ConvertSeriesProperties_ChartLineStyleSetWithCustomDashStyle_AppliesStyleToSeries()
        {
            // Setup
            var converter = new ChartMultipleLineDataConverter();
            var multipleLineSeries = new MultipleLineSeries();
            var random = new Random(21);
            var dashes = new[]
            {
                random.NextDouble(),
                random.NextDouble()
            };
            var data = new ChartMultipleLineData("test")
            {
                Style = new ChartLineStyle(Color.Red, 3, dashes)
            };

            // Call
            converter.ConvertSeriesProperties(data, multipleLineSeries);

            // Assert
            Assert.AreEqual(LineStyle.Solid, multipleLineSeries.LineStyle);
            Assert.AreEqual(dashes, multipleLineSeries.Dashes);
        }
    }
}