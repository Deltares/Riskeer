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
using Core.Components.OxyPlot.Forms;
using Core.Components.Stack.Data;
using Demo.Riskeer.Views;
using NUnit.Framework;

namespace Demo.Ringtoets.Test.Views
{
    [TestFixture]
    public class StackChartDataViewTest
    {
        [Test]
        public void DefaultConstructor_Always_AddsChartControl()
        {
            // Call
            using (var chartView = new StackChartDataView())
            {
                // Assert
                Assert.AreEqual(1, chartView.Controls.Count);
                object chartObject = chartView.Controls[0];
                Assert.IsInstanceOf<StackChartControl>(chartObject);

                var chart = (StackChartControl) chartObject;
                Assert.AreEqual(DockStyle.Fill, chart.Dock);
            }
        }

        [Test]
        public void Data_SetToObject_DoesNotThrow()
        {
            // Setup
            using (var chartView = new StackChartDataView())
            {
                // Call
                TestDelegate testDelegate = () => chartView.Data = new object();

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.IsNull(chartView.Data);
            }
        }

        [Test]
        public void Data_SetToStackChartData_DataSet()
        {
            // Setup
            using (var chartView = new StackChartDataView())
            {
                var stackChartData = new StackChartData();

                // Call
                chartView.Data = stackChartData;

                // Assert
                Assert.AreSame(stackChartData, chartView.Data);
            }
        }
    }
}