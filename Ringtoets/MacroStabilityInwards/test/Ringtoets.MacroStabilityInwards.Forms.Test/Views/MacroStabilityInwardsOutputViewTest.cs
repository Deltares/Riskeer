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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
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
                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Assert.AreEqual(1, splitContainer.Panel1.Controls.Count);
                Assert.IsEmpty(splitContainer.Panel2.Controls);
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
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile2D();
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
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);

                // When
                calculation.ClearOutput();
                calculation.NotifyObservers();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerChartData(chartData);
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_WhenOutputSet_ThenChartDataUpdated()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityInwardsOutputView())
            {
                form.Controls.Add(view);
                form.Show();

                MacroStabilityInwardsOutputChartControl chartControl = GetChartControl(form);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile2D();
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
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerChartData(chartData);

                // When
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();
                calculation.NotifyObservers();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);
            }
        }

        private static MacroStabilityInwardsOutputChartControl GetChartControl(Form form)
        {
            return ControlTestHelper.GetControls<MacroStabilityInwardsOutputChartControl>(form, "macroStabilityInwardsOutputChartControl").Single();
        }

        private static IChartControl GetChartControl(MacroStabilityInwardsOutputChartControl view)
        {
            return ControlTestHelper.GetControls<IChartControl>(view, "chartControl").Single();
        }

        private static MacroStabilityInwardsStochasticSoilProfile GetStochasticSoilProfile2D()
        {
            return new MacroStabilityInwardsStochasticSoilProfile(0.5, new MacroStabilityInwardsSoilProfile2D("Ondergrondschematisatie", new[]
            {
                new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                {
                    new Point2D(0.0, 1.0),
                    new Point2D(2.0, 4.0)
                }), new List<Ring>()),
                new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                {
                    new Point2D(3.0, 1.0),
                    new Point2D(8.0, 3.0)
                }), new List<Ring>()),
                new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                {
                    new Point2D(2.0, 4.0),
                    new Point2D(2.0, 8.0)
                }), new List<Ring>())
            }, new List<MacroStabilityInwardsPreconsolidationStress>()));
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