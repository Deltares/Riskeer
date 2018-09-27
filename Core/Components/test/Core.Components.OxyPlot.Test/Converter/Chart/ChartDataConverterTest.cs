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
using Core.Common.TestUtil;
using Core.Components.Chart.TestUtil;
using Core.Components.OxyPlot.Converter.Chart;
using NUnit.Framework;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter.Chart
{
    [TestFixture]
    public class ChartDataConverterTest
    {
        [Test]
        public void ConvertSeriesItems_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestChartDataConverter();
            var series = new TestSeries();

            // Call
            TestDelegate test = () => testConverter.ConvertSeriesData(null, series);

            // Assert
            const string expectedMessage = "Null data cannot be converted into series data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertSeriesItems_TargetSeriesNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestChartDataConverter();
            var chartData = new TestChartData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertSeriesData(chartData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertSeriesProperties_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestChartDataConverter();
            var series = new TestSeries();

            // Call
            TestDelegate test = () => testConverter.ConvertSeriesProperties(null, series);

            // Assert
            const string expectedMessage = "Null data cannot be converted into series data.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertSeriesProperties_TargetSeriesNull_ThrowsArgumentNullException()
        {
            // Setup
            var testConverter = new TestChartDataConverter();
            var chartData = new TestChartData("test data");

            // Call
            TestDelegate test = () => testConverter.ConvertSeriesProperties(chartData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void ConvertSeriesProperties_ChartData_NameSetToSeries()
        {
            // Setup
            const string name = "<Some name>";
            var testConverter = new TestChartDataConverter();
            var chartData = new TestChartData(name);
            var chartSeries = new TestSeries();

            // Call
            testConverter.ConvertSeriesProperties(chartData, chartSeries);

            // Assert
            Assert.AreEqual(name, chartSeries.Title);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ConvertSeriesProperties_ChartData_IsVisibleSetToSeries(bool isVisible)
        {
            // Setup
            var testConverter = new TestChartDataConverter();
            var chartData = new TestChartData("test data")
            {
                IsVisible = isVisible
            };
            var chartSeries = new TestSeries();

            // Call
            testConverter.ConvertSeriesProperties(chartData, chartSeries);

            // Assert
            Assert.AreEqual(isVisible, chartSeries.IsVisible);
        }

        private class TestSeries : LineSeries {}

        private class TestChartDataConverter : ChartDataConverter<TestChartData, TestSeries>
        {
            protected override void SetSeriesData(TestChartData data, TestSeries series) {}

            protected override void SetSeriesStyle(TestChartData data, TestSeries series) {}
        }
    }
}