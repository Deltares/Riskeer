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
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.OxyPlot.Converter.Stack;
using Core.Components.Stack.Data;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Test.Converter.Stack
{
    [TestFixture]
    public class RowChartDataConverterTest
    {
        [Test]
        public void ConvertSeriesData_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RowChartDataConverter.ConvertSeriesData(null, new ColumnSeries());

            // Assert
            const string expectedMessage = "Null data cannot be converted into series data.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void ConvertSeriesData_SeriesNull_ThrowsArgumentNullException()
        {
            // Setup
            var rowData = new RowChartData("data", new double[0], null);

            // Call
            TestDelegate test = () => RowChartDataConverter.ConvertSeriesData(rowData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            Assert.AreEqual("series", exception.ParamName);
        }

        [Test]
        public void ConvertSeriesData_DataWithValues_ColumnItemsAddedToSeries()
        {
            // Setup
            var values = new[]
            {
                0.2,
                0.7,
                0.1
            };

            var rowData = new RowChartData("data", values, null);
            var series = new ColumnSeries();

            // Call
            RowChartDataConverter.ConvertSeriesData(rowData, series);

            // Assert
            CollectionAssert.AreEqual(values, series.Items.Select(i => i.Value));
        }

        [Test]
        public void ConvertSeriesProperties_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RowChartDataConverter.ConvertSeriesProperties(null, new ColumnSeries());

            // Assert
            const string expectedMessage = "Null data cannot be converted into series data.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void ConvertSeriesProperties_SeriesNull_ThrowsArgumentNullException()
        {
            // Setup
            var rowData = new RowChartData("data", new double[0], null);

            // Call
            TestDelegate test = () => RowChartDataConverter.ConvertSeriesProperties(rowData, null);

            // Assert
            const string expectedMessage = "Null data cannot be used as conversion target.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            Assert.AreEqual("series", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ConvertSeriesProperties_WithData_SetPropertiesOfSeries(bool withColor)
        {
            // Setup
            const string name = "data";
            Color? color = withColor
                               ? (Color?) Color.Red
                               : null;

            var rowData = new RowChartData(name, new double[0], color);
            var series = new ColumnSeries();

            // Call
            RowChartDataConverter.ConvertSeriesProperties(rowData, series);

            // Assert
            Assert.AreEqual(name, series.Title);

            OxyColor actualColor = withColor
                                       ? OxyColor.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B)
                                       : OxyColors.Automatic;
            Assert.AreEqual(actualColor, series.FillColor);
        }
    }
}