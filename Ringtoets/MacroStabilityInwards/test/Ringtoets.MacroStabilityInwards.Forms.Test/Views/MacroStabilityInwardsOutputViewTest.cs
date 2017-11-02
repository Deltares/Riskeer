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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;

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
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNull(view.Data);
                Assert.IsNotNull(view.Chart);
                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);

                Assert.AreEqual(1, splitContainer.Panel1.Controls.Count);
                Assert.AreEqual(1, splitContainer.Panel2.Controls.Count);
                Assert.IsInstanceOf<MacroStabilityInwardsOutputChartControl>(splitContainer.Panel1.Controls[0]);
                Assert.IsInstanceOf<MacroStabilityInwardsSlicesTable>(splitContainer.Panel2.Controls[0]);

                var tableControl = (MacroStabilityInwardsSlicesTable) splitContainer.Panel2.Controls[0];
                CollectionAssert.IsEmpty(tableControl.Rows);
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
        public void Data_MacroStabilityInwardsCalculationScenario_DataSetToChartControl()
        {
            // Setup
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.AreSame(calculation, chartControl.Data);
            }
        }

        [Test]
        public void Data_SetToNull_ChartControlDataNull()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView
            {
                Data = calculation
            })
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

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
        public void GivenCalculationWithOutput_WhenOutputCleared_ThenChartDataUpdated()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    },
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };

                view.Data = calculation;

                // Precondition
                ChartDataCollection chartData = GetChartControl(chartControl).Data;
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);

                // When
                calculation.ClearOutput();
                calculation.NotifyObservers();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_WhenOutputSet_ThenChartDataUpdated()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                view.Data = calculation;

                // Precondition
                ChartDataCollection chartData = GetChartControl(chartControl).Data;
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);

                // When
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation.NotifyObservers();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
            }
        }

        [Test]
        public void GivenViewWithOutputSet_WhenInputChangedAndObserversNotified_ThenChartDataUpdated()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    },
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };

                view.Data = calculation;

                ChartDataCollection chartData = GetChartControl(chartControl).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);

                // When
                MacroStabilityInwardsStochasticSoilProfile newSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                calculation.InputParameters.StochasticSoilProfile = newSoilProfile;
                calculation.InputParameters.NotifyObservers();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
            }
        }

        [Test]
        public void GivenViewWithoutOutputSet_WhenInputChangedAndObserversNotified_ThenChartDataUpdated()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                view.Data = calculation;

                ChartDataCollection chartData = GetChartControl(chartControl).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndEmptyWaternetChartData(chartData);

                // When
                MacroStabilityInwardsStochasticSoilProfile newSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                calculation.InputParameters.StochasticSoilProfile = newSoilProfile;
                calculation.InputParameters.NotifyObservers();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndEmptyWaternetChartData(chartData);
            }
        }

        [Test]
        public void GivenViewWithOutputSet_WhenOutputUpdated_ThenTableUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsOutputView())
            {
                MacroStabilityInwardsSlicesTable slicesTable = GetSlicesTable(view);

                var outputWithoutSlices = new MacroStabilityInwardsOutput(new MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                                                                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                                                                                                                new[]
                                                                                                                {
                                                                                                                    MacroStabilityInwardsSliceTestFactory.CreateSlice()
                                                                                                                }, 0, 0),
                                                                          new MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGridTestFactory.Create(),
                                                                                                                      MacroStabilityInwardsGridTestFactory.Create(),
                                                                                                                      new double[0]),
                                                                          new MacroStabilityInwardsOutput.ConstructionProperties());

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Output = outputWithoutSlices
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(1, slicesTable.Rows.Count);

                // When
                calculation.InputParameters.Attach(observer);
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation.InputParameters.NotifyObservers();

                // Then
                Assert.AreEqual(3, slicesTable.Rows.Count);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithOutputSet_WhenOutputCleared_ThenTableCleared()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsOutputView())
            {
                MacroStabilityInwardsSlicesTable slicesTable = GetSlicesTable(view);

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(3, slicesTable.Rows.Count);

                // When
                calculation.InputParameters.Attach(observer);
                calculation.ClearOutput();
                calculation.InputParameters.NotifyObservers();

                // Then
                Assert.AreEqual(0, slicesTable.Rows.Count);
                mocks.VerifyAll();
            }
        }

        private static MacroStabilityInwardsOutputChartControl GetChartControl(Form form)
        {
            return ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();
        }

        private static MacroStabilityInwardsSlicesTable GetSlicesTable(MacroStabilityInwardsOutputView view)
        {
            return ControlTestHelper.GetControls<MacroStabilityInwardsSlicesTable>(view, "slicesTable").Single();
        }

        private static IChartControl GetChartControl(MacroStabilityInwardsOutputChartControl view)
        {
            return ControlTestHelper.GetControls<IChartControl>(view, "chartControl").Single();
        }

        private static MacroStabilityInwardsSurfaceLine GetSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            return GetSurfaceLine(points);
        }

        private static MacroStabilityInwardsSurfaceLine GetSurfaceLine(Point3D[] points)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface line name");
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }
    }
}