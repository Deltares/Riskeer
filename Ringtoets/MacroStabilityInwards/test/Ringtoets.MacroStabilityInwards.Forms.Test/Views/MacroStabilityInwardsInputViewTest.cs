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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsInputViewTest
    {
        private const int soilProfileIndex = 0;
        private const int surfaceLineIndex = 1;
        private const int surfaceLevelInsideIndex = 2;
        private const int ditchPolderSideIndex = 3;
        private const int bottomDitchPolderSideIndex = 4;
        private const int bottomDitchDikeSideIndex = 5;
        private const int ditchDikeSideIndex = 6;
        private const int dikeToeAtPolderIndex = 7;
        private const int shoulderTopInsideIndex = 8;
        private const int shoulderBaseInsideIndex = 9;
        private const int dikeTopAtPolderIndex = 10;
        private const int dikeToeAtRiverIndex = 11;
        private const int dikeTopAtRiverIndex = 12;
        private const int surfaceLevelOutsideIndex = 13;
        private const int tangentLinesIndex = 14;
        private const int leftGridIndex = 15;
        private const int rightGridIndex = 16;
        private const int waternetZonesExtremeIndex = 17;
        private const int waternetZonesDailyIndex = 18;
        private const int nrOfChartData = 19;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new MacroStabilityInwardsInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
                Assert.AreEqual(2, view.Controls.Count);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddEmptyChartControl()
        {
            // Call
            using (var view = new MacroStabilityInwardsInputView())
            {
                // Assert
                IChartControl chartControl = GetChartControl(view);
                Assert.IsInstanceOf<Control>(chartControl);
                Assert.AreSame(chartControl, chartControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) chartControl).Dock);
                Assert.AreEqual("Afstand [m]", chartControl.BottomAxisTitle);
                Assert.AreEqual("Hoogte [m+NAP]", chartControl.LeftAxisTitle);
                Assert.IsNull(chartControl.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddEmptyTableControl()
        {
            // Call
            using (var view = new MacroStabilityInwardsInputView())
            {
                // Assert
                MacroStabilityInwardsSoilLayerDataTable tableControl = GetSoilLayerTable(view);
                Assert.NotNull(tableControl);
                Assert.AreEqual(DockStyle.Bottom, tableControl.Dock);
                CollectionAssert.IsEmpty(tableControl.Rows);
            }
        }

        [Test]
        public void Data_MacroStabilityInwardsCalculationScenario_DataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
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
            using (var view = new MacroStabilityInwardsInputView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_SetToNull_ChartDataCleared()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView
            {
                Data = new MacroStabilityInwardsCalculationScenario()
            })
            {
                // Precondition
                Assert.AreEqual(nrOfChartData, view.Chart.Data.Collection.Count());
                Assert.AreEqual("Nieuwe berekening", view.Chart.ChartTitle);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Chart.Data);
                Assert.IsEmpty(view.Chart.ChartTitle);
            }
        }

        [Test]
        public void Data_SetToNull_TableDataCleared()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(
                        0.1, new MacroStabilityInwardsSoilProfile1D(
                            "profile",
                            -1,
                            new[]
                            {
                                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(1.0),
                                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(2.0),
                                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(3.0)
                            })),
                    SurfaceLine = GetSurfaceLineWithGeometry()
                }
            };

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                MacroStabilityInwardsSoilLayerDataTable tableControl = ControlTestHelper.GetControls<MacroStabilityInwardsSoilLayerDataTable>(
                    view, "soilLayerDataTable").Single();

                // Precondition
                Assert.NotNull(tableControl);
                Assert.AreEqual(3, tableControl.Rows.Count);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                CollectionAssert.IsEmpty(tableControl.Rows);
            }
        }

        [Test]
        public void Data_WithSurfaceLineAndStochasticSoilProfile1D_DataUpdatedToCollectionOfFilledChartData()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile1D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                ChartDataCollection chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                MacroStabilityInwardsInputViewChartDataAssert.AssertChartData(calculation, chartData);
            }
        }

        [Test]
        public void Data_WithSurfaceLineAndStochasticSoilProfile2D_DataUpdatedToCollectionOfFilledChartData()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                ChartDataCollection chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                MacroStabilityInwardsInputViewChartDataAssert.AssertChartData(calculation, chartData);
            }
        }

        [Test]
        public void Data_WithoutSurfaceLine_NoChartDataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();

                // Call
                view.Data = calculation;

                // Assert
                MacroStabilityInwardsInputViewChartDataAssert.AssertEmptyChartData(view.Chart.Data);
            }
        }

        [Test]
        public void Data_WithoutStochasticSoilProfile_SoilLayerTableEmpty()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario();

                // Call
                view.Data = calculation;

                // Assert
                MacroStabilityInwardsSoilLayerDataTable tableControl =
                    ControlTestHelper.GetControls<MacroStabilityInwardsSoilLayerDataTable>(view, "soilLayerDataTable")
                                     .SingleOrDefault();

                Assert.IsNotNull(tableControl);
                CollectionAssert.IsEmpty(tableControl.Rows);
            }
        }

        [Test]
        public void Data_EmptyWaternets_NoWaternetChartDataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                MacroStabilityInwardsInputViewChartDataAssert.AssertEmptyWaternetChartData(view.Chart.Data);
            }
        }

        [Test]
        public void Data_WithWaternets_WaternetChartDataSet()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                ChartData[] chartData = view.Chart.Data.Collection.ToArray();
                MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetDaily,
                                                                                      (ChartDataCollection) chartData[waternetZonesDailyIndex]);
                MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetExtreme,
                                                                                      (ChartDataCollection) chartData[waternetZonesExtremeIndex]);
            }
        }

        [Test]
        public void UpdateObserver_CalculationNameUpdated_ChartTitleUpdated()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Name = initialName
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                calculation.Name = updatedName;

                // Call
                calculation.NotifyObservers();

                // Assert
                Assert.AreEqual(updatedName, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void UpdateObserver_OtherCalculationUpdated_ChartTitleNotUpdated()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    Name = initialName
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                var calculation2 = new MacroStabilityInwardsCalculationScenario
                {
                    Name = initialName
                };

                view.Data = calculation2;

                calculation.Name = updatedName;

                // Call
                calculation.NotifyObservers();

                // Assert
                Assert.AreEqual(initialName, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void UpdateObserver_CalculationSurfaceLineUpdated_ChartDataUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                view.Data = calculation;

                List<ChartData> chartDataList = view.Chart.Data.Collection.ToList();
                var surfaceLineChartData = (ChartLineData) chartDataList[surfaceLineIndex];

                surfaceLineChartData.Attach(observer);

                MacroStabilityInwardsSurfaceLine surfaceLine2 = GetSecondSurfaceLineWithGeometry();

                calculation.InputParameters.SurfaceLine = surfaceLine2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                chartDataList = view.Chart.Data.Collection.ToList();

                Assert.AreSame(surfaceLineChartData, (ChartLineData) chartDataList[surfaceLineIndex]);

                MacroStabilityInwardsViewChartDataAssert.AssertSurfaceLineChartData(surfaceLine2, surfaceLineChartData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenCalculationWithStochasticSoilProfileAndSurfaceLine_WhenStochasticSoilProfileUpdate_ThenChartDataUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsStochasticSoilProfile originalSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        StochasticSoilProfile = originalSoilProfile,
                        SurfaceLine = GetSurfaceLineWithGeometry()
                    }
                };

                view.Data = calculation;

                List<ChartData> chartDataList = view.Chart.Data.Collection.ToList();
                var surfaceLineChartData = (ChartDataCollection) chartDataList[soilProfileIndex];

                surfaceLineChartData.Attach(observer);

                MacroStabilityInwardsStochasticSoilProfile newSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();

                // When
                calculation.InputParameters.StochasticSoilProfile = newSoilProfile;
                calculation.InputParameters.NotifyObservers();

                // Then
                chartDataList = view.Chart.Data.Collection.ToList();

                Assert.AreSame(surfaceLineChartData, (ChartDataCollection) chartDataList[soilProfileIndex]);

                MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(calculation.InputParameters.SoilProfileUnderSurfaceLine,
                                                                                    newSoilProfile.SoilProfile.Name,
                                                                                    true,
                                                                                    surfaceLineChartData);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_ChartSeriesSameOrder()
        {
            // Setup
            const int updatedSoilProfileIndex = soilProfileIndex + nrOfChartData - 1;
            const int updatedSurfaceLineIndex = surfaceLineIndex - 1;
            const int updatedSurfaceLevelInsideIndex = surfaceLevelInsideIndex - 1;
            const int updatedDitchPolderSideIndex = ditchPolderSideIndex - 1;
            const int updatedBottomDitchPolderSideIndex = bottomDitchPolderSideIndex - 1;
            const int updatedBottomDitchDikeSideIndex = bottomDitchDikeSideIndex - 1;
            const int updatedDitchDikeSideIndex = ditchDikeSideIndex - 1;
            const int updatedDikeToeAtPolderIndex = dikeToeAtPolderIndex - 1;
            const int updatedShoulderTopInsideIndex = shoulderTopInsideIndex - 1;
            const int updatedShoulderBaseInsideIndex = shoulderBaseInsideIndex - 1;
            const int updatedDikeTopAtPolderIndex = dikeTopAtPolderIndex - 1;
            const int updatedDikeToeAtRiverIndex = dikeToeAtRiverIndex - 1;
            const int updatedDikeTopAtRiverIndex = dikeTopAtRiverIndex - 1;
            const int updatedSurfaceLevelOutsideIndex = surfaceLevelOutsideIndex - 1;
            const int updatedTangentLinesIndex = tangentLinesIndex - 1;
            const int updatedLeftGridIndex = leftGridIndex - 1;
            const int updatedRightGridIndex = rightGridIndex - 1;
            const int updatedWaternetZonesExtremeIndex = waternetZonesExtremeIndex - 1;
            const int updatedWaternetZonesDailyIndex = waternetZonesDailyIndex - 1;

            var calculation = new MacroStabilityInwardsCalculationScenario();

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                ChartDataCollection chartData = view.Chart.Data;

                ChartData dataToMove = chartData.Collection.ElementAt(soilProfileIndex);
                chartData.Remove(dataToMove);
                chartData.Add(dataToMove);

                List<ChartData> chartDataList = chartData.Collection.ToList();

                var soilProfileData = (ChartDataCollection) chartDataList[updatedSoilProfileIndex];
                var surfaceLineData = (ChartLineData) chartDataList[updatedSurfaceLineIndex];
                var surfaceLevelInsideData = (ChartPointData) chartDataList[updatedSurfaceLevelInsideIndex];
                var ditchPolderSideData = (ChartPointData) chartDataList[updatedDitchPolderSideIndex];
                var bottomDitchPolderSideData = (ChartPointData) chartDataList[updatedBottomDitchPolderSideIndex];
                var bottomDitchDikeSideData = (ChartPointData) chartDataList[updatedBottomDitchDikeSideIndex];
                var ditchDikeSideData = (ChartPointData) chartDataList[updatedDitchDikeSideIndex];
                var dikeToeAtPolderData = (ChartPointData) chartDataList[updatedDikeToeAtPolderIndex];
                var shoulderTopInsideData = (ChartPointData) chartDataList[updatedShoulderTopInsideIndex];
                var shoulderBaseInsideData = (ChartPointData) chartDataList[updatedShoulderBaseInsideIndex];
                var dikeTopAtPolderData = (ChartPointData) chartDataList[updatedDikeTopAtPolderIndex];
                var dikeToeAtRiverData = (ChartPointData) chartDataList[updatedDikeToeAtRiverIndex];
                var dikeTopAtRiverData = (ChartPointData) chartDataList[updatedDikeTopAtRiverIndex];
                var surfaceLevelOutsideData = (ChartPointData) chartDataList[updatedSurfaceLevelOutsideIndex];
                var tangentLinesData = (ChartMultipleLineData) chartDataList[updatedTangentLinesIndex];
                var leftGridData = (ChartPointData) chartDataList[updatedLeftGridIndex];
                var rightGridData = (ChartPointData) chartDataList[updatedRightGridIndex];
                var waternetZonesExtremeData = (ChartDataCollection) chartDataList[updatedWaternetZonesExtremeIndex];
                var waternetZonesDailyData = (ChartDataCollection) chartDataList[updatedWaternetZonesDailyIndex];

                Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
                Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
                Assert.AreEqual("Maaiveld binnenwaarts", surfaceLevelInsideData.Name);
                Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);
                Assert.AreEqual("Slootbodem polderzijde", bottomDitchPolderSideData.Name);
                Assert.AreEqual("Slootbodem dijkzijde", bottomDitchDikeSideData.Name);
                Assert.AreEqual("Insteek sloot dijkzijde", ditchDikeSideData.Name);
                Assert.AreEqual("Teen dijk binnenwaarts", dikeToeAtPolderData.Name);
                Assert.AreEqual("Kruin binnenberm", shoulderTopInsideData.Name);
                Assert.AreEqual("Insteek binnenberm", shoulderBaseInsideData.Name);
                Assert.AreEqual("Kruin binnentalud", dikeTopAtPolderData.Name);
                Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);
                Assert.AreEqual("Kruin buitentalud", dikeTopAtRiverData.Name);
                Assert.AreEqual("Maaiveld buitenwaarts", surfaceLevelOutsideData.Name);
                Assert.AreEqual("Tangentlijnen", tangentLinesData.Name);
                Assert.AreEqual("Linker grid", leftGridData.Name);
                Assert.AreEqual("Rechter grid", rightGridData.Name);
                Assert.AreEqual("Zones extreem", waternetZonesExtremeData.Name);
                Assert.AreEqual("Zones dagelijks", waternetZonesDailyData.Name);

                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                calculation.InputParameters.SurfaceLine = surfaceLine;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                chartDataList = chartData.Collection.ToList();

                var actualSurfaceLineData = (ChartLineData) chartDataList[updatedSurfaceLineIndex];
                var actualSoilProfileData = (ChartDataCollection) chartDataList[updatedSoilProfileIndex];
                var actualSurfaceLevelInsideData = (ChartPointData) chartDataList[updatedSurfaceLevelInsideIndex];
                var actualDitchPolderSideData = (ChartPointData) chartDataList[updatedDitchPolderSideIndex];
                var actualBottomDitchPolderSideData = (ChartPointData) chartDataList[updatedBottomDitchPolderSideIndex];
                var actualBottomDitchDikeSideData = (ChartPointData) chartDataList[updatedBottomDitchDikeSideIndex];
                var actualDitchDikeSideData = (ChartPointData) chartDataList[updatedDitchDikeSideIndex];
                var actualDikeToeAtPolderData = (ChartPointData) chartDataList[updatedDikeToeAtPolderIndex];
                var actualShoulderTopInsideData = (ChartPointData) chartDataList[updatedShoulderTopInsideIndex];
                var actualShoulderBaseInsideData = (ChartPointData) chartDataList[updatedShoulderBaseInsideIndex];
                var actualDikeTopAtPolderData = (ChartPointData) chartDataList[updatedDikeTopAtPolderIndex];
                var actualDikeToeAtRiverData = (ChartPointData) chartDataList[updatedDikeToeAtRiverIndex];
                var actualDikeTopAtRiverData = (ChartPointData) chartDataList[updatedDikeTopAtRiverIndex];
                var actualSurfaceLevelOutsideData = (ChartPointData) chartDataList[updatedSurfaceLevelOutsideIndex];
                var actualTangentLinesData = (ChartMultipleLineData) chartDataList[updatedTangentLinesIndex];
                var actualLeftGridData = (ChartPointData) chartDataList[updatedLeftGridIndex];
                var actualRightGridData = (ChartPointData) chartDataList[updatedRightGridIndex];
                var actualWaternetZonesExtremeData = (ChartDataCollection) chartDataList[updatedWaternetZonesExtremeIndex];
                var actualWaternetZonesDailyData = (ChartDataCollection) chartDataList[updatedWaternetZonesDailyIndex];

                Assert.AreEqual(surfaceLine.Name, actualSurfaceLineData.Name);
                Assert.AreEqual("Ondergrondschematisatie", actualSoilProfileData.Name);
                Assert.AreEqual("Maaiveld binnenwaarts", actualSurfaceLevelInsideData.Name);
                Assert.AreEqual("Insteek sloot polderzijde", actualDitchPolderSideData.Name);
                Assert.AreEqual("Slootbodem polderzijde", actualBottomDitchPolderSideData.Name);
                Assert.AreEqual("Slootbodem dijkzijde", actualBottomDitchDikeSideData.Name);
                Assert.AreEqual("Insteek sloot dijkzijde", actualDitchDikeSideData.Name);
                Assert.AreEqual("Teen dijk binnenwaarts", actualDikeToeAtPolderData.Name);
                Assert.AreEqual("Kruin binnenberm", actualShoulderTopInsideData.Name);
                Assert.AreEqual("Insteek binnenberm", actualShoulderBaseInsideData.Name);
                Assert.AreEqual("Kruin binnentalud", actualDikeTopAtPolderData.Name);
                Assert.AreEqual("Teen dijk buitenwaarts", actualDikeToeAtRiverData.Name);
                Assert.AreEqual("Kruin buitentalud", actualDikeTopAtRiverData.Name);
                Assert.AreEqual("Maaiveld buitenwaarts", actualSurfaceLevelOutsideData.Name);
                Assert.AreEqual("Tangentlijnen", actualTangentLinesData.Name);
                Assert.AreEqual("Linker grid", actualLeftGridData.Name);
                Assert.AreEqual("Rechter grid", actualRightGridData.Name);
                Assert.AreEqual("Zones extreem", actualWaternetZonesExtremeData.Name);
                Assert.AreEqual("Zones dagelijks", actualWaternetZonesDailyData.Name);
            }
        }

        [Test]
        public void UpdateObserver_OtherCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation1 = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = GetSurfaceLineWithGeometry()
                    }
                };

                view.Data = calculation1;
                ChartDataCollection dataBeforeUpdate = view.Chart.Data;

                foreach (ChartData chartData in dataBeforeUpdate.Collection)
                {
                    chartData.Attach(observer);
                }

                view.Data = new MacroStabilityInwardsCalculationScenario();

                MacroStabilityInwardsSurfaceLine surfaceLine2 = GetSecondSurfaceLineWithGeometry();

                calculation1.InputParameters.SurfaceLine = surfaceLine2;

                // Call
                calculation1.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
                mocks.VerifyAll(); // no update observer expected
            }
        }

        [Test]
        public void UpdateObserver_CalculationInputGridSettingsUpdated_GridChartDataUpdated()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                MacroStabilityInwardsInput input = calculation.InputParameters;
                input.GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual;
                SetGridValues(input.LeftGrid);
                SetGridValues(input.RightGrid);

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                ChartDataCollection chartData = view.Chart.Data;
                List<ChartData> chartDataList = chartData.Collection.ToList();
                var actualLeftGridData = (ChartPointData) chartDataList[leftGridIndex];
                var actualRightGridData = (ChartPointData) chartDataList[rightGridIndex];

                MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(input.LeftGrid, actualLeftGridData);
                MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(input.RightGrid, actualRightGridData);
            }
        }

        [Test]
        public void GivenViewWithStochasticSoilProfile_WhenStochasticSoilProfileUpdated_ThenDataTableUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSoilLayerDataTable soilLayerDataTable = GetSoilLayerTable(view);

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.5,
                                                                                               new MacroStabilityInwardsSoilProfile1D(
                                                                                                   "profile 1D",
                                                                                                   -1,
                                                                                                   new[]
                                                                                                   {
                                                                                                       MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D()
                                                                                                   })),
                        SurfaceLine = GetSurfaceLineWithGeometry()
                    }
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(1, soilLayerDataTable.Rows.Count);

                // When
                calculation.InputParameters.Attach(observer);
                calculation.InputParameters.StochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.5,
                                                                                                                   new MacroStabilityInwardsSoilProfile1D(
                                                                                                                       "new profile 1D",
                                                                                                                       -1,
                                                                                                                       new[]
                                                                                                                       {
                                                                                                                           MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(3),
                                                                                                                           MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(4)
                                                                                                                       }));
                calculation.InputParameters.NotifyObservers();

                // Then
                Assert.AreEqual(2, soilLayerDataTable.Rows.Count);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithGridPoints_WhenGridDeterminationTypeSetToAutomatic_ThenNoGridPoints()
        {
            // Given
            var calculation = new MacroStabilityInwardsCalculationScenario();
            MacroStabilityInwardsInput input = calculation.InputParameters;
            input.GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual;
            SetGridValues(input.LeftGrid);
            SetGridValues(input.RightGrid);

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Precondition
                ChartDataCollection chartData = view.Chart.Data;
                List<ChartData> chartDataList = chartData.Collection.ToList();
                var leftGridData = (ChartPointData) chartDataList[leftGridIndex];
                var rightGridData = (ChartPointData) chartDataList[rightGridIndex];

                MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(input.LeftGrid, leftGridData);
                MacroStabilityInwardsViewChartDataAssert.AssertGridChartData(input.RightGrid, rightGridData);

                // When
                input.GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Automatic;
                input.NotifyObservers();

                // Then
                chartDataList = chartData.Collection.ToList();
                var updatedLeftGridData = (ChartPointData) chartDataList[leftGridIndex];
                var updatedRightGridData = (ChartPointData) chartDataList[rightGridIndex];
                CollectionAssert.IsEmpty(updatedLeftGridData.Points);
                CollectionAssert.IsEmpty(updatedRightGridData.Points);
            }
        }

        [Test]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Manual, MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated)]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Automatic, MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated)]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Automatic, MacroStabilityInwardsTangentLineDeterminationType.Specified)]
        public void GivenViewWithSpecifiedTangentLines_WhenTangentLineOrGridDeterminationTypeSetToManual_ThenNoTangentLines(
            MacroStabilityInwardsGridDeterminationType gridDeterminationType,
            MacroStabilityInwardsTangentLineDeterminationType tangentLineDeterminationType)
        {
            // Given
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = GetSurfaceLineWithGeometry(),
                    GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Manual,
                    TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.Specified,
                    TangentLineZTop = new RoundedDouble(2, 10.0),
                    TangentLineZBottom = new RoundedDouble(2, 5.0),
                    TangentLineNumber = 2
                }
            };
            MacroStabilityInwardsInput input = calculation.InputParameters;

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                // Precondition
                ChartDataCollection chartData = view.Chart.Data;
                List<ChartData> chartDataList = chartData.Collection.ToList();
                var tangentLinesData = (ChartMultipleLineData) chartDataList[tangentLinesIndex];
                CollectionAssert.AreEqual(new[]
                {
                    new[]
                    {
                        new Point2D(0.0, 10.0),
                        new Point2D(1.58, 10.0)
                    },
                    new[]
                    {
                        new Point2D(0.0, 5.0),
                        new Point2D(1.58, 5.0)
                    }
                }, tangentLinesData.Lines);

                // When
                input.GridDeterminationType = gridDeterminationType;
                input.TangentLineDeterminationType = tangentLineDeterminationType;
                input.NotifyObservers();

                // Then
                CollectionAssert.IsEmpty(tangentLinesData.Lines);
            }
        }

        [Test]
        public void GivenMacroStabilityInputViewWithSoilProfileSeries_WhenSurfaceLineSetToNull_ThenCollectionOfEmptyChartDataSetForSoilProfiles()
        {
            // Given
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                view.Data = calculation;

                ChartDataCollection chartData = view.Chart.Data;

                // Precondition
                Assert.IsNotNull(chartData);
                Assert.AreEqual(nrOfChartData, chartData.Collection.Count());
                MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(calculation.InputParameters.SoilProfileUnderSurfaceLine,
                                                                                    stochasticSoilProfile.SoilProfile.Name,
                                                                                    true,
                                                                                    chartData.Collection.ElementAt(soilProfileIndex));

                // When
                calculation.InputParameters.SurfaceLine = null;
                calculation.InputParameters.NotifyObservers();

                // Then
                MacroStabilityInwardsViewChartDataAssert.AssertSoilProfileChartData(calculation.InputParameters.SoilProfileUnderSurfaceLine,
                                                                                    stochasticSoilProfile.SoilProfile.Name,
                                                                                    true,
                                                                                    chartData.Collection.ElementAt(soilProfileIndex));
            }
        }

        [Test]
        public void GivenViewWithWaternets_WhenWaternetSetEmpty_ThenNoChartData()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = stochasticSoilProfile
                    }
                };

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    view.Data = calculation;

                    // Precondition
                    ChartData[] chartData = view.Chart.Data.Collection.ToArray();
                    MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetDaily,
                                                                                          (ChartDataCollection) chartData[waternetZonesDailyIndex]);
                    MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetExtreme,
                                                                                          (ChartDataCollection) chartData[waternetZonesExtremeIndex]);
                }

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                MacroStabilityInwardsInputViewChartDataAssert.AssertEmptyWaternetChartData(view.Chart.Data);
            }
        }

        [Test]
        public void GivenViewWithEmptyWaternets_WhenWaternetSet_ThenChartDataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
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
                MacroStabilityInwardsInputViewChartDataAssert.AssertEmptyWaternetChartData(view.Chart.Data);

                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    calculation.InputParameters.NotifyObservers();

                    // Assert
                    ChartData[] chartData = view.Chart.Data.Collection.ToArray();
                    MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetDaily,
                                                                                          (ChartDataCollection) chartData[waternetZonesDailyIndex]);
                    MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetExtreme,
                                                                                          (ChartDataCollection) chartData[waternetZonesExtremeIndex]);
                }
            }
        }

        [Test]
        public void GivenViewWithWaternets_WhenObserversNotifiedAndWaternetSame_ThenChartDataNotUpdated()
        {
            // Setup
            using (new MacroStabilityInwardsCalculatorFactoryConfig())
            using (var view = new MacroStabilityInwardsInputView())
            {
                MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile =
                    MacroStabilityInwardsStochasticSoilProfileTestFactory.CreateMacroStabilityInwardsStochasticSoilProfile2D();
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
                ChartData[] chartData = view.Chart.Data.Collection.ToArray();
                var waternetExtremeChartDataCollection = (ChartDataCollection) chartData[waternetZonesExtremeIndex];
                var waternetDailyChartDataCollection = (ChartDataCollection) chartData[waternetZonesDailyIndex];

                MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetExtreme,
                                                                                      waternetExtremeChartDataCollection);
                MacroStabilityInwardsInputViewChartDataAssert.AssertWaternetChartData(calculation.InputParameters.WaternetDaily,
                                                                                      waternetDailyChartDataCollection);

                IEnumerable<ChartData> waternetExtremeChartData = waternetExtremeChartDataCollection.Collection;
                IEnumerable<ChartData> waternetDailyChartData = waternetDailyChartDataCollection.Collection;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                CollectionAssert.AreEqual(waternetExtremeChartData, ((ChartDataCollection) chartData[waternetZonesExtremeIndex]).Collection);
                CollectionAssert.AreEqual(waternetDailyChartData, ((ChartDataCollection) chartData[waternetZonesDailyIndex]).Collection);
            }
        }

        private static void SetGridValues(MacroStabilityInwardsGrid grid)
        {
            grid.NumberOfHorizontalPoints = 2;
            grid.XLeft = (RoundedDouble) 1;
            grid.XRight = (RoundedDouble) 2;
            grid.NumberOfVerticalPoints = 2;
            grid.ZBottom = (RoundedDouble) 1;
            grid.ZTop = (RoundedDouble) 2;
        }

        private static IChartControl GetChartControl(MacroStabilityInwardsInputView view)
        {
            return ControlTestHelper.GetControls<IChartControl>(view, "chartControl").Single();
        }

        private static MacroStabilityInwardsSoilLayerDataTable GetSoilLayerTable(MacroStabilityInwardsInputView view)
        {
            return ControlTestHelper.GetControls<MacroStabilityInwardsSoilLayerDataTable>(view, "soilLayerDataTable").Single();
        }

        private static MacroStabilityInwardsStochasticSoilProfile GetStochasticSoilProfile1D()
        {
            return new MacroStabilityInwardsStochasticSoilProfile(0.5, new MacroStabilityInwardsSoilProfile1D("profile 1D", -1, new[]
            {
                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(2),
                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(3),
                MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(5)
            }));
        }

        MacroStabilityInwardsSurfaceLine GetSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            return GetSurfaceLine(points);
        }

        MacroStabilityInwardsSurfaceLine GetSecondSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(3.5, 2.3, 8.0),
                new Point3D(6.9, 2.0, 2.0)
            };

            return GetSurfaceLine(points);
        }

        MacroStabilityInwardsSurfaceLine GetSurfaceLine(Point3D[] points)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface line name");
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }
    }
}