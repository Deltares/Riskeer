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
using Core.Components.OxyPlot.CustomSeries;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter
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
            Assert.IsInstanceOf<ChartDataConverter<ChartMultipleAreaData>>(converter);
        }

        [Test]
        public void CanConvertSeries_AreaData_ReturnTrue()
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var areaData = new ChartMultipleAreaData("test data");

            // Call
            var canConvert = converter.CanConvertSeries(areaData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var chartData = new TestChartData();

            // Call
            var canConvert = converter.CanConvertSeries(chartData);

            // Assert
            Assert.IsFalse(canConvert);
        }

        [Test]
        public void Convert_RandomAreaData_ReturnsNewSeries()
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);

            var points = new Collection<Point2D>();

            for (var i = 0; i < randomCount; i++)
            {
                points.Add(new Point2D(random.NextDouble(), random.NextDouble()));
            }

            var areas = new List<Point2D[]>
            {
                points.ToArray()
            };

            var areaData = new ChartMultipleAreaData("test data")
            {
                Areas = areas
            };

            // Call
            var series = converter.Convert(areaData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            var areaSeries = (MultipleAreaSeries) series[0];
            var expectedData = areas.ElementAt(0).Select(t => new DataPoint(t.X, t.Y)).ToArray();
            CollectionAssert.AreEqual(expectedData, areaSeries.Areas[0]);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new ChartMultipleAreaDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new ChartMultipleAreaDataConverter();
            var testChartData = new TestChartData();

            // Precondition
            Assert.IsFalse(testConverter.CanConvertSeries(testChartData));

            // Call
            TestDelegate test = () => testConverter.Convert(testChartData);

            // Assert
            var expectedMessage = string.Format("The data of type {0} cannot be converted by this converter.", testChartData.GetType());
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentFillColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new ChartAreaStyle(expectedColor, Color.Red, 3);
            var data = new ChartMultipleAreaData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var multipleAreaSeries = (MultipleAreaSeries) series[0];
            AssertColors(style.FillColor, multipleAreaSeries.Fill);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new ChartAreaStyle(Color.Red, expectedColor, 3);
            var data = new ChartMultipleAreaData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var multipleAreaSeries = (MultipleAreaSeries) series[0];
            AssertColors(style.StrokeColor, multipleAreaSeries.Color);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentStrokeWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartMultipleAreaDataConverter();
            var style = new ChartAreaStyle(Color.Red, Color.Red, width);
            var data = new ChartMultipleAreaData("test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var multipleAreaSeries = (MultipleAreaSeries) series[0];
            Assert.AreEqual(width, multipleAreaSeries.StrokeThickness);
        }

        private void AssertColors(Color color, OxyColor oxyColor)
        {
            OxyColor originalColor = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            Assert.AreEqual(originalColor, oxyColor);
        }
    }
}