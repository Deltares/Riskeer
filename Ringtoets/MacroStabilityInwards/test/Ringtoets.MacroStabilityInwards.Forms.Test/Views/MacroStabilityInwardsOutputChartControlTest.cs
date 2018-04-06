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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
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
    public class MacroStabilityInwardsOutputChartControlTest
    {
        private const int soilProfileIndex = 0;
        private const int waternetZonesExtremeIndex = 14;
        private const int waternetZonesDailyIndex = 15;

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsOutputChartControl(null, GetTestNormativeAssessmentLevel);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_GetNormativeAssessmentLevelFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsOutputChartControl(new MacroStabilityInwardsCalculationScenario(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("getNormativeAssessmentLevelFunc", paramName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call
            var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel);

            // Assert
            Assert.IsInstanceOf<UserControl>(control);
            Assert.IsInstanceOf<IChartView>(control);
            Assert.AreSame(calculation, control.Data);
            Assert.IsNotNull(control.Chart);
            Assert.AreEqual(1, control.Controls.Count);

            IChartControl chartControl = GetChartControl(control);
            Assert.IsInstanceOf<Control>(chartControl);
            Assert.AreEqual(DockStyle.Fill, ((Control) chartControl).Dock);
            Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
            Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);
        }

        [Test]
        public void Constructor_CalculationWithOutput_ChartDataSet()
        {
            // Setup
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

            // Call
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                // Assert
                IChartControl chartControl = GetChartControl(control);
                ChartDataCollection chartData = chartControl.Data;
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertOutputChartData(calculation, chartData);
                Assert.AreEqual(calculation.Name, chartControl.ChartTitle);
            }
        }

        [Test]
        public void Constructor_CalculationWithoutStochasticSoilProfile_ChartDataSetWithDefaultSoilProfileChartDataName()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            // Call
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                // Assert
                IChartControl chartControl = GetChartControl(control);
                ChartDataCollection chartData = chartControl.Data;
                MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(calculation.InputParameters.SoilProfileUnderSurfaceLine,
                                                                                    "Ondergrondschematisatie",
                                                                                    false,
                                                                                    chartData.Collection.ElementAt(soilProfileIndex));

                Assert.AreEqual(calculation.Name, chartControl.ChartTitle);
            }
        }

        [Test]
        public void Constructor_CalculationWithoutOutputAndEmptyWaternet_ChartDataEmpty()
        {
            // Setup
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

            // Call
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                // Assert
                IChartControl chartControl = GetChartControl(control);
                ChartDataCollection chartData = chartControl.Data;
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndEmptyWaternetChartData(chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyOutputChartData(chartData);
                Assert.AreEqual(calculation.Name, chartControl.ChartTitle);
            }
        }

        [Test]
        public void Constructor_CalculationWithoutOutputAndWithWaternet_ChartDataEmpty()
        {
            // Setup
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

            // Call
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                // Assert
                IChartControl chartControl = GetChartControl(control);
                ChartDataCollection chartData = chartControl.Data;
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyOutputChartData(chartData);
                Assert.AreEqual(calculation.Name, chartControl.ChartTitle);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithOutput_ChartDataUpdated()
        {
            // Setup
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

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyOutputChartData(chartData);

                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertOutputChartData(calculation, chartData);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithoutOutputWithWaternet_ChartDataUpdated()
        {
            // Setup
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

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertOutputChartData(calculation, chartData);

                calculation.ClearOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyOutputChartData(chartData);
            }
        }

        [Test]
        public void UpdateChartData_CalculationWithoutOutputWithoutWaternet_ChartDataUpdated()
        {
            // Setup
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

            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertOutputChartData(calculation, chartData);

                calculation.ClearOutput();

                // Call
                control.UpdateChartData();

                // Assert
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyChartDataWithEmptySoilLayerAndWithWaternetChartData(chartData);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyOutputChartData(chartData);
            }
        }

        [Test]
        public void UpdateChartData_CalculationNameChanged_ChartTitleUpdated()
        {
            // Setup
            const string newCalculationName = "Test name";

            var calculation = new MacroStabilityInwardsCalculationScenario();

            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                IChartControl chartControl = GetChartControl(control);

                // Precondition
                Assert.AreEqual(calculation.Name, chartControl.ChartTitle);

                // Call
                calculation.Name = newCalculationName;
                control.UpdateChartData();

                // Assert
                Assert.AreEqual(newCalculationName, chartControl.ChartTitle);
            }
        }

        [Test]
        public void GivenCalculationWithStochasticSoilProfileAndSurfaceLine_WhenStochasticSoilProfileUpdate_ThenChartDataUpdated()
        {
            // Given
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

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
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);

                MacroStabilityInwardsStochasticSoilProfile newSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

                // When
                calculation.InputParameters.StochasticSoilProfile = newSoilProfile;
                control.UpdateChartData();

                // Then
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
            }
        }

        [Test]
        public void GivenViewWithWaternets_WhenWaternetSetEmpty_ThenNoChartData()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = originalSoilProfile,
                    SurfaceLine = GetSurfaceLineWithGeometry()
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            using (var macroStabilityInwardsCalculatorFactoryConfig = new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, GetChartControl(control).Data);

                macroStabilityInwardsCalculatorFactoryConfig.Dispose();

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
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = originalSoilProfile,
                    SurfaceLine = GetSurfaceLineWithGeometry()
                },
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                ChartDataCollection chartData = GetChartControl(control).Data;

                // Precondition
                MacroStabilityInwardsOutputViewChartDataAssert.AssertEmptyWaternetChartData(chartData);

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    control.UpdateChartData();

                    // Assert
                    MacroStabilityInwardsOutputViewChartDataAssert.AssertInputChartData(calculation, chartData);
                }
            }
        }

        [Test]
        public void GivenViewWithWaternets_WhenObserversNotifiedAndWaternetSame_ThenChartDataNotUpdated()
        {
            // Setup
            MacroStabilityInwardsStochasticSoilProfile originalSoilProfile = MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

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
            using (var control = new MacroStabilityInwardsOutputChartControl(calculation, GetTestNormativeAssessmentLevel))
            {
                // Precondition
                ChartData[] chartData = GetChartControl(control).Data.Collection.ToArray();
                var waternetExtremeChartDataCollection = (ChartDataCollection) chartData[waternetZonesExtremeIndex];
                var waternetDailyChartDataCollection = (ChartDataCollection) chartData[waternetZonesDailyIndex];

                MacroStabilityInwardsOutputViewChartDataAssert.AssertWaternetChartData(DerivedMacroStabilityInwardsInput.GetWaternetExtreme(calculation.InputParameters, RoundedDouble.NaN),
                                                                                       waternetExtremeChartDataCollection);
                MacroStabilityInwardsOutputViewChartDataAssert.AssertWaternetChartData(DerivedMacroStabilityInwardsInput.GetWaternetDaily(calculation.InputParameters),
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

        private static RoundedDouble GetTestNormativeAssessmentLevel()
        {
            return (RoundedDouble) 1.1;
        }
    }
}