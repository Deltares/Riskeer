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
using Core.Components.Chart.Data;
using Core.Components.Chart.TestUtil;
using Core.Components.OxyPlot.DataSeries.Chart;
using NUnit.Framework;

namespace Core.Components.OxyPlot.Test.DataSeries.Chart
{
    [TestFixture]
    public class ChartDataSeriesFactoryTest
    {
        [Test]
        public void Create_ChartPointData_ReturnChartPointDataSeries()
        {
            // Call
            IChartDataSeries series = ChartDataSeriesFactory.Create(new ChartPointData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartPointDataSeries>(series);
        }

        [Test]
        public void Create_ChartLineData_ReturnChartLineDataSeries()
        {
            // Call
            IChartDataSeries series = ChartDataSeriesFactory.Create(new ChartLineData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartLineDataSeries>(series);
        }

        [Test]
        public void Create_ChartAreaData_ReturnChartAreaDataSeries()
        {
            // Call
            IChartDataSeries series = ChartDataSeriesFactory.Create(new ChartAreaData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartAreaDataSeries>(series);
        }

        [Test]
        public void Create_ChartMultipleAreaData_ReturnChartMultipleAreaDataSeries()
        {
            // Call
            IChartDataSeries series = ChartDataSeriesFactory.Create(new ChartMultipleAreaData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartMultipleAreaDataSeries>(series);
        }

        [Test]
        public void Create_ChartMultipleLineData_ReturnChartMultipleLineDataSeries()
        {
            // Call
            IChartDataSeries series = ChartDataSeriesFactory.Create(new ChartMultipleLineData("test data"));

            // Assert
            Assert.IsInstanceOf<ChartMultipleLineDataSeries>(series);
        }

        [Test]
        public void Create_OtherData_ThrowsNotSupportedException()
        {
            // Setup
            var testData = new TestChartData("test data");

            // Call
            TestDelegate test = () => ChartDataSeriesFactory.Create(testData);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void Create_NullData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ChartDataSeriesFactory.Create(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }
    }
}