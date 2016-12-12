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
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.DataSeries;
using NUnit.Framework;

namespace Core.Components.OxyPlot.Test.DataSeries
{
    [TestFixture]
    public class ItemBasedChartDataSeriesFactoryTest
    {
        [Test]
        public void Create_ChartPointData_ReturnChartPointDataSeries()
        {
            // Call
            IItemBasedChartDataSeries series = ItemBasedChartDataSeriesFactory.Create(new ChartPointData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartPointDataSeries>(series);
        }

        [Test]
        public void Create_ChartLineData_ReturnChartLineDataSeries()
        {
            // Call
            IItemBasedChartDataSeries series = ItemBasedChartDataSeriesFactory.Create(new ChartLineData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartLineDataSeries>(series);
        }

        [Test]
        public void Create_ChartAreaData_ReturnChartAreaDataSeries()
        {
            // Call
            IItemBasedChartDataSeries series = ItemBasedChartDataSeriesFactory.Create(new ChartAreaData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartAreaDataSeries>(series);
        }

        [Test]
        public void Create_ChartMultipleAreaData_ReturnChartMultipleAreaDataSeries()
        {
            // Call
            IItemBasedChartDataSeries series = ItemBasedChartDataSeriesFactory.Create(new ChartMultipleAreaData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartMultipleAreaDataSeries>(series);
        }

        [Test]
        public void Create_OtherData_ThrownsNotSupportedException()
        {
            // Setup
            var testData = new TestItemBasedChartData("test data");

            // Call
            TestDelegate test = () => ItemBasedChartDataSeriesFactory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        private class TestItemBasedChartData : ItemBasedChartData
        {
            public TestItemBasedChartData(string name) : base(name) {}
        }
    }
}