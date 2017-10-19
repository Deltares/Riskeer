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
using Core.Common.Controls.Views;
using NUnit.Framework;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsOutputViewTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new MacroStabilityInwardsOutputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Assert.AreEqual(1, splitContainer.Panel1.Controls.Count);
                Assert.IsInstanceOf<MacroStabilityInwardsOutputChartControl>(splitContainer.Panel1.Controls[0]);
            }
        }

        [Test]
        public void Data_MacroStabilityInwardsCalculationScenario_DataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsOutputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanMacroStabilityInwardsCalculationScenario_DataNull()
        {
            // Setup
            using (var view = new MacroStabilityInwardsOutputView())
            {
                // Call
                view.Data = new object();

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_MacroStabilityInwardsCalculationScenarioWithOutput_DataSetToChartControl()
        {
            // Setup
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Output = new TestMacroStabilityInwardsOutput()
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.AreSame(calculation, chartControl.Data);
            }
        }

        [Test]
        public void Data_MacroStabilityInwardsCalculationScenarioWithoutOutput_ChartControlDataNull()
        {
            // Setup
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();

                var calculation = new MacroStabilityInwardsCalculationScenario();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.IsNull(chartControl.Data);
            }
        }

        [Test]
        public void Data_SetToNull_ChartControlDataNull()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = new TestMacroStabilityInwardsOutput()
            };

            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView
            {
                Data = calculation
            })
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();

                // Precondition
                Assert.AreSame(calculation, view.Data);
                Assert.AreSame(calculation, chartControl.Data);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(chartControl.Data);
            }
        }

        [Test]
        public void GivenCalculationWithOutput_WhenOutputCleared_ThenChartControlDataSetToNull()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Output = new TestMacroStabilityInwardsOutput()
                };

                view.Data = calculation;

                // Precondition
                Assert.AreSame(calculation, chartControl.Data);

                // When
                calculation.ClearOutput();
                calculation.NotifyObservers();

                // Then
                Assert.IsNull(chartControl.Data);
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_WhenOutputSet_ThenChartControlDataSetToCalculation()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();

                var calculation = new MacroStabilityInwardsCalculationScenario();

                view.Data = calculation;

                // Precondition
                Assert.IsNull(chartControl.Data);

                // When
                calculation.Output = new TestMacroStabilityInwardsOutput();
                calculation.NotifyObservers();

                // Then
                Assert.AreSame(calculation, chartControl.Data);
            }
        }
    }
}