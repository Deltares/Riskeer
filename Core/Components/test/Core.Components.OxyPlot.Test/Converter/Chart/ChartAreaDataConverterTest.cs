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
    public class ChartAreaDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartAreaDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartAreaData, AreaSeries>>(converter);
        }

        [Test]
        public void ConvertSeriesItems_ChartAreaDataWithRandomPointData_ConvertsAllPointsToAreaSeries()
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var areaSeries = new AreaSeries();
            var random = new Random(21);
            int randomCount = random.Next(5, 10);
            var points = new Collection<Point2D>();
            for (var i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var areaData = new ChartAreaData("test data")
            {
                Points = points.ToArray()
            };

            // Call
            converter.ConvertSeriesData(areaData, areaSeries);

            // Assert
            DataPoint[] expectedPoints = points.Select(t => new DataPoint(t.X, t.Y)).ToArray();
            CollectionAssert.AreEqual(expectedPoints, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint>
            {
                expectedPoints.First()
            }, areaSeries.Points2);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_WithDifferentFillColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var areaSeries = new AreaSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartAreaData("test", new ChartAreaStyle
            {
                FillColor = expectedColor,
                StrokeColor = Color.Red,
                StrokeThickness = 3
            });

            // Call
            converter.ConvertSeriesProperties(data, areaSeries);

            // Assert
            AssertColors(expectedColor, areaSeries.Fill);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_WithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var areaSeries = new AreaSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartAreaData("test", new ChartAreaStyle
            {
                FillColor = Color.Red,
                StrokeColor = expectedColor,
                StrokeThickness = 3
            });

            // Call
            converter.ConvertSeriesProperties(data, areaSeries);

            // Assert
            AssertColors(expectedColor, areaSeries.Color);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void ConvertSeriesProperties_WithDifferentStrokeWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var areaSeries = new AreaSeries();
            var data = new ChartAreaData("test", new ChartAreaStyle
            {
                FillColor = Color.Red,
                StrokeColor = Color.Red,
                StrokeThickness = width
            });

            // Call
            converter.ConvertSeriesProperties(data, areaSeries);

            // Assert
            Assert.AreEqual(width, areaSeries.StrokeThickness);
        }

        private static void AssertColors(Color color, OxyColor oxyColor)
        {
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), oxyColor);
        }
    }
}