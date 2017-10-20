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
    public class MacroStabilityInwardsOutputChartControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var control = new MacroStabilityInwardsOutputChartControl();

            // Assert
            Assert.IsInstanceOf<UserControl>(control);
            Assert.IsInstanceOf<IChartView>(control);
            Assert.IsNull(control.Data);

            Assert.AreEqual(1, control.Controls.Count);
            Assert.IsInstanceOf<IChartControl>(control.Controls[0]);
        }

        [Test]
        public void DefaultConstructor_Always_AddChartControlWithEmptyChartData()
        {
            // Call
            using (var control = new MacroStabilityInwardsOutputChartControl())
            {
                // Assert
                IChartControl chartControl = GetChartControl(control);
                Assert.IsInstanceOf<Control>(chartControl);
                Assert.AreSame(chartControl, chartControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) chartControl).Dock);
                Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
                Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartData(chartControl.Data, true);
            }
        }

        [Test]
        public void Data_MacroStabilityInwardsCalculationScenario_DataSet()
        {
            // Setup
            using (var control = new MacroStabilityInwardsOutputChartControl())
            {
                MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

                // Call
                control.Data = calculation;

                // Assert
                Assert.AreSame(calculation, control.Data);
            }
        }

        [Test]
        public void Data_OtherThanMacroStabilityInwardsCalculationScenario_DataNull()
        {
            // Setup
            using (var control = new MacroStabilityInwardsOutputChartControl())
            {
                // Call
                control.Data = new object();

                // Assert
                Assert.IsNull(control.Data);
            }
        }

        [Test]
        public void Data_SetValueWithOutput_ChartDatSet()
        {
            // Setup
            using (var control = new MacroStabilityInwardsOutputChartControl())
            {
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

                // Call
                control.Data = calculation;

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(surfaceLine,
                                                                               stochasticSoilProfile,
                                                                               GetChartControl(control).Data);
            }
        }

        [Test]
        public void Data_SetValueWithoutOutput_ChartDataEmpty()
        {
            // Setup
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

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(surfaceLine,
                                                                               stochasticSoilProfile,
                                                                               GetChartControl(control).Data);

                // Call
                control.Data = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartData(control.Chart.Data, false);
            }
        }

        [Test]
        public void Data_SetToNull_ChartDataEmpty()
        {
            // Setup
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

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(surfaceLine,
                                                                               stochasticSoilProfile,
                                                                               GetChartControl(control).Data);

                // Call
                control.Data = null;

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartData(control.Chart.Data, false);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithOutput_ChartDataUpdated()
        {
            // Setup
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

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartData(control.Chart.Data, false);

                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(surfaceLine,
                                                                               stochasticSoilProfile,
                                                                               GetChartControl(control).Data);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithoutOutput_ChartDataUpdated()
        {
            // Setup
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

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(surfaceLine,
                                                                               stochasticSoilProfile,
                                                                               GetChartControl(control).Data);

                calculation.ClearOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartData(control.Chart.Data, false);
            }
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

        private static IChartControl GetChartControl(MacroStabilityInwardsOutputChartControl view)
        {
            return ControlTestHelper.GetControls<IChartControl>(view, "chartControl").Single();
        }
    }
}