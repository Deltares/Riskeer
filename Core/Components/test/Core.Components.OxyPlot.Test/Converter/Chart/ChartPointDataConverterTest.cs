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
    public class ChartPointDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartPointDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartPointData, LineSeries>>(converter);
        }

        [Test]
        public void ConvertSeriesItems_ChartPointDataWithRandomPointData_ConvertsAllPointsToPointSeries()
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var lineSeries = new LineSeries();
            var random = new Random(21);
            int randomCount = random.Next(5, 10);

            var points = new Collection<Point2D>();

            for (var i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var pointData = new ChartPointData("test data")
            {
                Points = points.ToArray()
            };

            // Call
            converter.ConvertSeriesData(pointData, lineSeries);

            // Assert
            CollectionAssert.AreEqual(points.Select(p => new DataPoint(p.X, p.Y)), lineSeries.ItemsSource);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_ChartPointStyleSetWithDifferentColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var lineSeries = new LineSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartPointData("test", new ChartPointStyle
            {
                Color = expectedColor,
                StrokeColor = Color.Red,
                Size = 3,
                StrokeThickness = 2,
                Symbol = ChartPointSymbol.Circle
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            AssertColors(expectedColor, lineSeries.MarkerFill);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_ChartPointStyleSetWithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var lineSeries = new LineSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartPointData("test", new ChartPointStyle
            {
                Color = Color.Red,
                StrokeColor = expectedColor,
                Size = 3,
                StrokeThickness = 2,
                Symbol = ChartPointSymbol.Circle
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            AssertColors(expectedColor, lineSeries.MarkerStroke);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void ConvertSeriesProperties_ChartPointStyleSetWithDifferentWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var lineSeries = new LineSeries();
            var data = new ChartPointData("test", new ChartPointStyle
            {
                Color = Color.Red,
                StrokeColor = Color.Red,
                Size = width,
                StrokeThickness = 2,
                Symbol = ChartPointSymbol.Circle
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            Assert.AreEqual(width, lineSeries.MarkerSize);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void ConvertSeriesProperties_ChartPointStyleSetWithDifferentStrokeThickness_AppliesStyleToSeries(int strokeThickness)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var lineSeries = new LineSeries();
            var data = new ChartPointData("test", new ChartPointStyle
            {
                Color = Color.Red,
                StrokeColor = Color.Red,
                Size = 3,
                StrokeThickness = strokeThickness,
                Symbol = ChartPointSymbol.Circle
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            Assert.AreEqual(strokeThickness, lineSeries.MarkerStrokeThickness);
        }

        [Test]
        [TestCase(ChartPointSymbol.Circle, MarkerType.Circle)]
        [TestCase(ChartPointSymbol.Square, MarkerType.Square)]
        [TestCase(ChartPointSymbol.Diamond, MarkerType.Diamond)]
        [TestCase(ChartPointSymbol.Triangle, MarkerType.Triangle)]
        [TestCase(ChartPointSymbol.Star, MarkerType.Star)]
        public void ConvertSeriesProperties_ChartPointStyleSetWithDifferentChartPointSymbols_AppliesStyleToSeries(ChartPointSymbol symbol, MarkerType expectedMarkerType)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var lineSeries = new LineSeries();
            var data = new ChartPointData("test", new ChartPointStyle
            {
                Color = Color.Red,
                StrokeColor = Color.Red,
                Size = 3,
                StrokeThickness = 2,
                Symbol = symbol
            });

            // Call
            converter.ConvertSeriesProperties(data, lineSeries);

            // Assert
            Assert.AreEqual(expectedMarkerType, lineSeries.MarkerType);
        }

        private static void AssertColors(Color color, OxyColor oxyColor)
        {
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), oxyColor);
        }
    }
}