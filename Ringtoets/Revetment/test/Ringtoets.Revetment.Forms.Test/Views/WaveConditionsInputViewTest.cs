// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Views;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.Revetment.Forms.Test.Views
{
    [TestFixture]
    public class WaveConditionsInputViewTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new WaveConditionsInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);
            }
        }

        [Test]
        public void Constructor_Always_AddEmptyChartControl()
        {
            // Call
            using (var view = new WaveConditionsInputView())
            {
                // Assert
                var chartControl = (IChartControl) view.Controls.Find("chartControl", true).First();
                Assert.IsInstanceOf<Control>(chartControl);
                Assert.AreSame(chartControl, chartControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) chartControl).Dock);
                Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
                Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);
                Assert.IsNull(chartControl.Data);
            }
        }

        [Test]
        public void Data_IWaveConditionsCalculation_DataSet()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
            {
                var calculation = new TestWaveConditionsCalculation();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanIWwaveCondittionsCalculation_DataNull()
        {
            // Setup
            using (var view = new WaveConditionsInputView())
            {
                var calculation = new object();

                // Call
                view.Data = calculation;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_SetToNull_ChartDataCleared()
        {
            // Setup
            using (var view = new WaveConditionsInputView
            {
                Data = new TestWaveConditionsCalculation()
            })
            {
                // Precondition
                Assert.AreEqual(1, view.Chart.Data.Collection.Count());
                Assert.AreEqual("Nieuwe berekening", view.Chart.ChartTitle);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Chart.Data);
                Assert.AreEqual(string.Empty, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void Data_SetChartData_ChartDataSet()
        {
            // Setup
            const string calculationName = "Calculation name";

            using (var view = new WaveConditionsInputView())
            {
                var calculation = new TestWaveConditionsCalculation
                {
                    Name = calculationName
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.AreEqual(calculationName, view.Chart.ChartTitle);

                ChartDataCollection chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(1, chartData.Collection.Count());
                AssertForeshoreChartData(calculation.InputParameters.ForeshoreProfile, chartData.Collection.ElementAt(0));
            }
        }

        private static void AssertForeshoreChartData(ForeshoreProfile foreshoreProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var foreshoreChartData = (ChartLineData) chartData;

            RoundedPoint2DCollection foreshoreGeometry = foreshoreProfile.Geometry;
            Assert.AreEqual(foreshoreGeometry.Count(), foreshoreChartData.Points.Length);
            CollectionAssert.AreEqual(foreshoreGeometry, foreshoreChartData.Points);

            string expectedName = $"{foreshoreProfile.Name} - Voorlandprofiel";
            Assert.AreEqual(expectedName, chartData.Name);
        }
    }
}