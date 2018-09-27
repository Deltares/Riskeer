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
using System.Linq;
using Core.Components.OxyPlot.DataSeries.Stack;
using Core.Components.Stack.Data;
using NUnit.Framework;

namespace Core.Components.OxyPlot.Test.DataSeries.Stack
{
    [TestFixture]
    public class RowChartDataSeriesFactoryTest
    {
        [Test]
        public void Create_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RowChartDataSeriesFactory.Create(null).ToList();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Create_EmptyStackChartData_ReturnEmptyEnumerable()
        {
            // Call
            IEnumerable<RowChartDataSeries> series = RowChartDataSeriesFactory.Create(new StackChartData());

            // Assert
            CollectionAssert.IsEmpty(series);
        }

        [Test]
        public void Create_CompleteStackChartData_ReturnChartDataSeriesForRows()
        {
            // Setup
            var data = new StackChartData();
            data.AddColumn("Column 1");
            data.AddColumn("Column 2");

            data.AddRow("Row 1", new[]
            {
                0.1,
                0.9
            });

            // Call
            IEnumerable<RowChartDataSeries> series = RowChartDataSeriesFactory.Create(data);

            // Assert
            Assert.AreEqual(data.Rows.Count(), series.Count());
        }
    }
}