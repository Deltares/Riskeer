﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsOutputChartControlTest
    {
        private const int waternetZonesExtremeIndex = 14;
        private const int waternetZonesDailyIndex = 15;

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
                Assert.IsNull(chartControl.Data);
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
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
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
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, GetChartControl(control).Data);
            }
        }

        [Test]
        public void Data_SetValueWithoutOutputAndEmptyWaternet_ChartDataEmpty()
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

            // Call

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndEmptyWaternetChartData(GetChartControl(control).Data);
            }
        }

        [Test]
        public void Data_SetValueWithoutOutputAndWithWaternet_ChartDataEmpty()
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

            // Call
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(GetChartControl(control).Data);
            }
        }

        [Test]
        public void Data_SetToNull_ChartDataCleared()
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
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, GetChartControl(control).Data);

                // Call
                control.Data = null;

                // Assert
                Assert.IsNull(GetChartControl(control).Data);
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

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);

                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithoutOutputWithWaternet_ChartDataUpdated()
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

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);

                calculation.ClearOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithoutOutputWithoutWaternet_ChartDataUpdated()
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
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);

                calculation.ClearOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndEmptyWaternetChartData(chartData);
            }
        }

        [Test]
        public void GivenCalculationWithStochasticSoilProfileAndSurfaceLine_WhenStochasticSoilProfileUpdate_ThenChartDataUpdated()
        {
            // Given
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = GetStochasticSoilProfile2D();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = originalSoilProfile,
                    SurfaceLine = GetSurfaceLineWithGeometry()
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);

                MacroStabilityInwardsStochasticSoilProfile newSoilProfile = GetStochasticSoilProfile2D();

                // When
                calculation.InputParameters.StochasticSoilProfile = newSoilProfile;
                control.UpdateChartData();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);
            }
        }

        [Test]
        public void GivenViewWithWaternets_WhenWaternetSetEmpty_ThenNoChartData()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = GetStochasticSoilProfile2D();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = originalSoilProfile,
                    SurfaceLine = GetSurfaceLineWithGeometry()
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            using (var control = new MacroStabilityInwardsOutputChartControl())
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    control.Data = calculation;

                    // Precondition
                    MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, GetChartControl(control).Data);
                }

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyWaternetChartData(GetChartControl(control).Data);
            }
        }

        [Test]
        public void GivenViewWithEmptyWaternets_WhenWaternetSet_ThenChartDataSet()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = GetStochasticSoilProfile2D();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = originalSoilProfile,
                    SurfaceLine = GetSurfaceLineWithGeometry()
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyWaternetChartData(chartData);

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    control.UpdateChartData();

                    // Assert
                    MacroStabilityInwardsOutputViewChartDataAssert.AssertChartData(calculation, chartData);
                }
            }
        }

        [Test]
        public void GivenViewWithWaternets_WhenObserversNotifiedAndWaternetSame_ThenChartDataNotUpdated()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = GetStochasticSoilProfile2D();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = originalSoilProfile,
                    SurfaceLine = GetSurfaceLineWithGeometry()
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl
            {
                Data = calculation
            })
            {
                // Precondition
                ChartData[] chartData = GetChartControl(control).Data.Collection.ToArray();
                var waternetExtremeChartDataCollection = (ChartDataCollection) chartData[waternetZonesExtremeIndex];
                var waternetDailyChartDataCollection = (ChartDataCollection) chartData[waternetZonesDailyIndex];

                MacroStabilityInwardsViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetExtreme,
                                                                                 waternetExtremeChartDataCollection);
                MacroStabilityInwardsViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetDaily,
                                                                                 waternetDailyChartDataCollection);

                IEnumerable<ChartData> waternetExtremeChartData = waternetExtremeChartDataCollection.Collection;
                IEnumerable<ChartData> waternetDailyChartData = waternetDailyChartDataCollection.Collection;

                // Call
                control.UpdateChartData();

                // Assert
                CollectionAssert.AreEqual(waternetExtremeChartData, ((ChartDataCollection) chartData[waternetZonesExtremeIndex]).Collection);
                CollectionAssert.AreEqual(waternetDailyChartData, ((ChartDataCollection) chartData[waternetZonesDailyIndex]).Collection);
            }
        }

        private static MacroStabilityInwardsStochasticSoilProfile GetStochasticSoilProfile2D()
        {
            var layers = new[]
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
                                                     }),
                                                     new List<Ring>(),
                                                     new MacroStabilityInwardsSoilLayerData(),
                                                     new[]
                                                     {
                                                         new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                                                                                              {
                                                                                                  new Point2D(4.0, 2.0),
                                                                                                  new Point2D(0.0, 2.5)
                                                                                              }),
                                                                                              new List<Ring>(),
                                                                                              new MacroStabilityInwardsSoilLayerData(),
                                                                                              new[]
                                                                                              {
                                                                                                  new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                                                                                                  {
                                                                                                      new Point2D(4.0, 2.0),
                                                                                                      new Point2D(0.0, 2.5)
                                                                                                  }), new List<Ring>())
                                                                                              })
                                                     }
                ),
                new MacroStabilityInwardsSoilLayer2D(new Ring(new List<Point2D>
                {
                    new Point2D(2.0, 4.0),
                    new Point2D(2.0, 8.0)
                }), new List<Ring>())
            };

            return new MacroStabilityInwardsStochasticSoilProfile(0.5, new MacroStabilityInwardsSoilProfile2D("Ondergrondschematisatie",
                                                                                                              layers,
                                                                                                              new List<MacroStabilityInwardsPreconsolidationStress>()));
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