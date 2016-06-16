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
    public class ChartAreaDataConverterTest
    {
        [Test]
        public void DefaultConstructor_IsChartDataConverter()
        {
            // Call
            var converter = new ChartAreaDataConverter();

            // Assert
            Assert.IsInstanceOf<ChartDataConverter<ChartAreaData>>(converter);
        }

        [Test]
        public void CanConvertSeries_AreaData_ReturnTrue()
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var areaData = new ChartAreaData(new Collection<Tuple<double, double>>(), "test data");

            // Call
            var canConvert = converter.CanConvertSeries(areaData);

            // Assert
            Assert.IsTrue(canConvert);
        }

        [Test]
        public void CanConvertSeries_ChartData_ReturnsFalse()
        {
            // Setup
            var converter = new ChartAreaDataConverter();
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
            var converter = new ChartAreaDataConverter();
            var random = new Random(21);
            var randomCount = random.Next(5, 10);
            var points = new Collection<Tuple<double, double>>();

            for (int i = 0; i < randomCount; i++)
            {
                points.Add(Tuple.Create(random.NextDouble(), random.NextDouble()));
            }

            var areaData = new ChartAreaData(points, "test data");

            // Call
            var series = converter.Convert(areaData);

            // Assert
            Assert.IsInstanceOf<IList<Series>>(series);
            var areaSeries = ((AreaSeries)series[0]);
            var expectedData = points.Select(t => new DataPoint(t.Item1, t.Item2)).ToArray();
            CollectionAssert.AreEqual(expectedData, areaSeries.Points);
            CollectionAssert.AreEqual(new Collection<DataPoint> { expectedData.First() }, areaSeries.Points2);
        }

        [Test]
        public void Convert_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new ChartAreaDataConverter();

            // Call
            TestDelegate test = () => testConverter.Convert(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Convert_DataCannotBeConverted_ThrowsArgumentException()
        {
            // Setup
            var testConverter = new ChartAreaDataConverter();
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
        public void Convert_WithDifferentFillColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new ChartAreaStyle(expectedColor, Color.Red, 3);
            var data = new ChartAreaData(new Collection<Tuple<double, double>>(), "test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var areaSeries = ((AreaSeries)series[0]);
            AssertColors(style.FillColor, areaSeries.Fill);
        }

        [Test]
        [TestCase(KnownColor.AliceBlue)]
        [TestCase(KnownColor.Azure)]
        [TestCase(KnownColor.Beige)]
        public void Convert_WithDifferentStrokeColors_AppliesStyleToSeries(KnownColor color)
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var expectedColor = Color.FromKnownColor(color);
            var style = new ChartAreaStyle(Color.Red, expectedColor, 3);
            var data = new ChartAreaData(new Collection<Tuple<double, double>>(), "test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var areaSeries = ((AreaSeries)series[0]);
            AssertColors(style.StrokeColor, areaSeries.Color);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(7)]
        public void Convert_WithDifferentStrokeWidths_AppliesStyleToSeries(int width)
        {
            // Setup
            var converter = new ChartAreaDataConverter();
            var style = new ChartAreaStyle(Color.Red, Color.Red, width);
            var data = new ChartAreaData(new Collection<Tuple<double, double>>(), "test")
            {
                Style = style
            };

            // Call
            var series = converter.Convert(data);

            // Assert
            var areaSeries = ((AreaSeries)series[0]);
            Assert.AreEqual(width, areaSeries.StrokeThickness);
        }

        private void AssertColors(Color color, OxyColor oxyColor)
        {
            OxyColor originalColor = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            Assert.AreEqual(originalColor, oxyColor);
        }
    }
}