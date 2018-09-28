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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Components.OxyPlot.Converter.Chart;
using Core.Components.OxyPlot.CustomSeries;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.Converter.Chart
{
    [TestFixture]
    public class ChartMultipleAreaDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartMultipleAreaDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartMultipleAreaData, MultipleAreaSeries>>(converter);
        }

        [Test]
        public void ConvertSeriesItems_ChartMultipleAreaDataWithRandomAreaData_ConvertsAllAreasToMultipleAreaSeries()
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var multipleAreaSeries = new MultipleAreaSeries();
            var random = new Random(21);
            int randomCount = random.Next(5, 10);

            var points1 = new Collection<Point2D>();
            var points2 = new Collection<Point2D>();

            for (var i = 0; i < randomCount; i++)
            {
                points1.Add(new Point2D(random.NextDouble(), random.NextDouble()));
                points2.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var areas = new List<Point2D[]>
            {
                points1.ToArray(),
                points2.ToArray()
            };

            var areaData = new ChartMultipleAreaData("test data")
            {
                Areas = areas
            };

            // Call
            converter.ConvertSeriesData(areaData, multipleAreaSeries);

            // Assert
            Assert.AreEqual(2, multipleAreaSeries.Areas.Count);
            CollectionAssert.AreEqual(areas.ElementAt(0).Select(t => new DataPoint(t.X, t.Y)), multipleAreaSeries.Areas[0]);
            CollectionAssert.AreEqual(areas.ElementAt(1).Select(t => new DataPoint(t.X, t.Y)), multipleAreaSeries.Areas[1]);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_ChartAreaStyleSetWithDifferentFillColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var multipleAreaSeries = new MultipleAreaSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartMultipleAreaData("test", new ChartAreaStyle
            {
                FillColor = expectedColor,
                StrokeColor = Color.Red,
                StrokeThickness = 3
            });

            // Call
            converter.ConvertSeriesProperties(data, multipleAreaSeries);

            // Assert
            AssertColors(expectedColor, multipleAreaSeries.Fill);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void ConvertSeriesProperties_ChartAreaStyleSetWithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var multipleAreaSeries = new MultipleAreaSeries();
            Color expectedColor = Color.FromKnownColor(color);
            var data = new ChartMultipleAreaData("test", new ChartAreaStyle
            {
                FillColor = Color.Red,
                StrokeColor = expectedColor,
                StrokeThickness = 3
            });

            // Call
            converter.ConvertSeriesProperties(data, multipleAreaSeries);

            // Assert
            AssertColors(expectedColor, multipleAreaSeries.Color);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void ConvertSeriesProperties_ChartAreaStyleSetWithDifferentStrokeWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var multipleAreaSeries = new MultipleAreaSeries();
            var data = new ChartMultipleAreaData("test", new ChartAreaStyle
            {
                FillColor = Color.Red,
                StrokeColor = Color.Red,
                StrokeThickness = width
            });

            // Call
            converter.ConvertSeriesProperties(data, multipleAreaSeries);

            // Assert
            Assert.AreEqual(width, multipleAreaSeries.StrokeThickness);
        }

        private static void AssertColors(Color color, OxyColor oxyColor)
        {
            Assert.AreEqual(OxyColor.FromArgb(color.A, color.R, color.G, color.B), oxyColor);
        }
    }
}