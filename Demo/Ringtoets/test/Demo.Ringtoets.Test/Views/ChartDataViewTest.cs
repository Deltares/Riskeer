// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Windows.Forms;
using Core.Components.Chart.Data;
using Core.Components.OxyPlot.Forms;
using Demo.Ringtoets.Views;
using NUnit.Framework;

namespace Demo.Ringtoets.Test.Views
{
    [TestFixture]
    public class ChartDataViewTest
    {
        [Test]
        public void DefaultConstructor_Always_AddsChartControl()
        {
            // Call
            using (var chartView = new ChartDataView())
            {
                // Assert
                Assert.AreEqual(1, chartView.Controls.Count);
                object chartObject = chartView.Controls[0];
                Assert.IsInstanceOf<ChartControl>(chartObject);

                var chart = (ChartControl) chartObject;
                Assert.AreEqual(DockStyle.Fill, chart.Dock);
                Assert.NotNull(chartView.Chart);
            }
        }

        [Test]
        public void Data_SetToObject_DoesNotThrow()
        {
            // Setup
            using (var chartView = new ChartDataView())
            {
                // Call
                TestDelegate testDelegate = () => chartView.Data = new object();

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.IsNull(chartView.Data);
            }
        }

        [Test]
        public void Data_SetToCollectionChartData_ChartDataSet()
        {
            // Setup
            using (var chartView = new ChartDataView())
            {
                var chartDataCollection = new ChartDataCollection("test data");

                // Call
                chartView.Data = chartDataCollection;

                // Assert
                Assert.AreSame(chartDataCollection, chartView.Data);
            }
        }
    }
}