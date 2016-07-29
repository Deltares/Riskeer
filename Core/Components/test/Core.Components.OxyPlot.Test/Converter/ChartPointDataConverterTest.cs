// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Converter;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
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
            Assert.IsInstanceOf<ChartDataConverter<ChartPointData>>(converter);
        }

        [Test]
        public void CanConvertSeries_PointData_ReturnTrue()
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var pointData = new ChartPointData("test data");

            // Call
            var canConvert = converter.CanConvertSeries(pointData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var chartData = new TestChartData();

            // Call
            var canConvert = converter.CanConvertSeries(chartData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_RandomPointData_ReturnsNewSeries()
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);

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
            var series = converter.Convert(pointData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            var lineSeries = (LineSeries) series[0];
            CollectionAssert.AreEqual(points, lineSeries.ItemsSource);
            Assert.AreNotSame(pointData.Points, lineSeries.ItemsSource);
            Assert.AreEqual(LineStyle.None, lineSeries.LineStyle);
            Assert.AreEqual(MarkerType.Circle, lineSeries.MarkerType);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new ChartPointDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new ChartPointDataConverter();
            var testChartData = new TestChartData();
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testChartData.GetType());
            // Precondition
            Assert.IsFalse(testConverter.CanConvertSeries(testChartData));

            // Call
            TestDelegate test = () => testConverter.Convert(testChartData);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new ChartPointStyle(expectedColor, 3, Color.Red, 2, ChartPointSymbol.Circle);
            var data = new ChartPointData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var lineSeries = (LineSeries) series[0];
            AssertColors(style.Color, lineSeries.MarkerFill);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new ChartPointStyle(Color.Red, 3, expectedColor, 2, ChartPointSymbol.Circle);
            var data = new ChartPointData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var lineSeries = (LineSeries) series[0];
            AssertColors(style.StrokeColor, lineSeries.MarkerStroke);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var style = new ChartPointStyle(Color.Red, width, Color.Red, 2, ChartPointSymbol.Circle);
            var data = new ChartPointData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var lineSeries = (LineSeries) series[0];
            Assert.AreEqual(width, lineSeries.MarkerSize);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentStrokeThickness_AppliesStyleToSeries(int strokeThickness)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var style = new ChartPointStyle(Color.Red, 3, Color.Red, strokeThickness, ChartPointSymbol.Circle);
            var data = new ChartPointData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var lineSeries = (LineSeries) series[0];
            Assert.AreEqual(strokeThickness, lineSeries.MarkerStrokeThickness);
        }

        [Test]
        [TestCase(ChartPointSymbol.None, MarkerType.None)]
        [TestCase(ChartPointSymbol.Circle, MarkerType.Circle)]
        [TestCase(ChartPointSymbol.Square, MarkerType.Square)]
        [TestCase(ChartPointSymbol.Diamond, MarkerType.Diamond)]
        [TestCase(ChartPointSymbol.Triangle, MarkerType.Triangle)]
        public void Convert_WithDifferentChartPointSymbols_AppliesStyleToSeries(ChartPointSymbol symbol, MarkerType expectedMarkerType)
        {
            // Setup
            var converter = new ChartPointDataConverter();
            var style = new ChartPointStyle(Color.Red, 3, Color.Red, 2, symbol);
            var data = new ChartPointData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var lineSeries = (LineSeries) series[0];
            Assert.AreEqual(expectedMarkerType, lineSeries.MarkerType);
        }

        private void AssertColors(Color color, OxyColor oxyColor)
        {
            OxyColor originalColor = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            Assert.AreEqual(originalColor, oxyColor);
        }
    }
}