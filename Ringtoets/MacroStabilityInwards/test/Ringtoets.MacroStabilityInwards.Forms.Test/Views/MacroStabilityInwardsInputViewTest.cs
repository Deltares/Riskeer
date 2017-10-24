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
using Ringtoets.MacroStabilityInwards.Forms.Views;
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
        private const int leftGridIndex = 14;
        private const int rightGridIndex = 15;
        private const int waternetZonesExtremeIndex = 16;
        private const int waternetZonesDailyIndex = 17;
        private const int nrOfChartData = 18;

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
                                new MacroStabilityInwardsSoilLayer1D(3.0),
                                new MacroStabilityInwardsSoilLayer1D(2.0),
                                new MacroStabilityInwardsSoilLayer1D(0)
                            }))
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
        public void Data_WithSurfaceLineAndSoilProfile1D_DataUpdatedToCollectionOfFilledChartData()
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
                Assert.AreEqual(nrOfChartData, chartData.Collection.Count());
                AssertSurfaceLineChartData(surfaceLine, chartData.Collection.ElementAt(surfaceLineIndex));
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), true);
            }
        }

        [Test]
        public void Data_WithSurfaceLineAndSoilProfile2D_DataUpdatedToCollectionOfFilledChartData()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
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
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                ChartDataCollection chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(nrOfChartData, chartData.Collection.Count());
                AssertSurfaceLineChartData(surfaceLine, chartData.Collection.ElementAt(surfaceLineIndex));
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
                AssertEmptyChartData(view.Chart.Data);
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
                AssertEmptySoilLayerTable(view);
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

                AssertSurfaceLineChartData(surfaceLine2, surfaceLineChartData);
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
                MacroStabilityInwardsStochasticSoilProfile soilProfile2D = GetStochasticSoilProfile2D();

                var calculation = new MacroStabilityInwardsCalculationScenario
                {
                    InputParameters =
                    {
                        StochasticSoilProfile = soilProfile2D,
                        SurfaceLine = GetSurfaceLineWithGeometry()
                    }
                };

                view.Data = calculation;

                List<ChartData> chartDataList = view.Chart.Data.Collection.ToList();
                var surfaceLineChartData = (ChartDataCollection) chartDataList[soilProfileIndex];

                surfaceLineChartData.Attach(observer);

                MacroStabilityInwardsStochasticSoilProfile soilProfile1D = GetStochasticSoilProfile1D();

                // When
                calculation.InputParameters.StochasticSoilProfile = soilProfile1D;
                calculation.InputParameters.NotifyObservers();

                // Then
                chartDataList = view.Chart.Data.Collection.ToList();

                Assert.AreSame(surfaceLineChartData, (ChartDataCollection) chartDataList[soilProfileIndex]);

                AssertSoilProfileChartData(soilProfile1D, surfaceLineChartData, true);
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
            const int updatedSurfaceLevelOutsideIndex = surfaceLevelOutsideIndex - 1;
            const int updatedLeftGridIndex = leftGridIndex - 1;
            const int updatedRightGridIndex = rightGridIndex - 1;

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
                var surfaceLevelOutsideData = (ChartPointData) chartDataList[updatedSurfaceLevelOutsideIndex];
                var leftGridData = (ChartPointData) chartDataList[updatedLeftGridIndex];
                var rightGridData = (ChartPointData) chartDataList[updatedRightGridIndex];

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
                Assert.AreEqual("Maaiveld buitenwaarts", surfaceLevelOutsideData.Name);
                Assert.AreEqual("Linker grid", leftGridData.Name);
                Assert.AreEqual("Rechter grid", rightGridData.Name);

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
                var actualSurfaceLevelOutsideData = (ChartPointData) chartDataList[updatedSurfaceLevelOutsideIndex];
                var actualLeftGridData = (ChartPointData) chartDataList[updatedLeftGridIndex];
                var actualRightGridData = (ChartPointData) chartDataList[updatedRightGridIndex];

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
                Assert.AreEqual("Maaiveld buitenwaarts", actualSurfaceLevelOutsideData.Name);
                Assert.AreEqual("Linker grid", actualLeftGridData.Name);
                Assert.AreEqual("Rechter grid", actualRightGridData.Name);
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

                AssertGridPoints(input.LeftGrid, actualLeftGridData.Points);
                AssertGridPoints(input.RightGrid, actualRightGridData.Points);
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
                                                                                                       new MacroStabilityInwardsSoilLayer1D(1)
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
                                                                                                                           new MacroStabilityInwardsSoilLayer1D(3),
                                                                                                                           new MacroStabilityInwardsSoilLayer1D(4)
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

                AssertGridPoints(input.LeftGrid, leftGridData.Points);
                AssertGridPoints(input.RightGrid, rightGridData.Points);

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
        public void GivenMacroStabilityInputViewWithSoilProfileSeries_WhenSurfaceLineSetToNull_ThenCollectionOfEmptyChartDataSetForSoilProfiles()
        {
            // Given
            using (var view = new MacroStabilityInwardsInputView())
            {
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

                ChartDataCollection chartData = view.Chart.Data;

                // Precondition
                Assert.IsNotNull(chartData);
                Assert.AreEqual(nrOfChartData, chartData.Collection.Count());
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), true);

                // When
                calculation.InputParameters.SurfaceLine = null;
                calculation.InputParameters.NotifyObservers();

                // Then
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), false);
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

        private static void AssertGridPoints(MacroStabilityInwardsGrid grid, Point2D[] actualPoints)
        {
            var expectedPoints = new[]
            {
                new Point2D(grid.XLeft, grid.ZBottom),
                new Point2D(grid.XRight, grid.ZBottom),
                new Point2D(grid.XLeft, grid.ZTop),
                new Point2D(grid.XRight, grid.ZTop)
            };

            CollectionAssert.AreEqual(expectedPoints, actualPoints);
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
                new MacroStabilityInwardsSoilLayer1D(1),
                new MacroStabilityInwardsSoilLayer1D(3),
                new MacroStabilityInwardsSoilLayer1D(5)
            }));
        }

        private static MacroStabilityInwardsStochasticSoilProfile GetStochasticSoilProfile2D()
        {
            return new MacroStabilityInwardsStochasticSoilProfile(0.5, new MacroStabilityInwardsSoilProfile2D("profile 2D", new[]
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

        private static MacroStabilityInwardsSurfaceLine GetSecondSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(3.5, 2.3, 8.0),
                new Point3D(6.9, 2.0, 2.0)
            };

            return GetSurfaceLine(points);
        }

        private static MacroStabilityInwardsSurfaceLine GetSurfaceLine(Point3D[] points)
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface line name");
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }

        private static void AssertEmptySoilLayerTable(MacroStabilityInwardsInputView view)
        {
            MacroStabilityInwardsSoilLayerDataTable tableControl = GetSoilLayerTable(view);

            // Precondition
            Assert.NotNull(tableControl);
            CollectionAssert.IsEmpty(tableControl.Rows);
        }

        private static void AssertEmptyChartData(ChartDataCollection chartDataCollection)
        {
            Assert.AreEqual("Invoer", chartDataCollection.Name);

            List<ChartData> chartDatasList = chartDataCollection.Collection.ToList();

            Assert.AreEqual(nrOfChartData, chartDatasList.Count);

            var surfaceLineData = (ChartLineData) chartDatasList[surfaceLineIndex];
            var soilProfileData = (ChartDataCollection) chartDatasList[soilProfileIndex];
            var surfaceLevelInsideData = (ChartPointData) chartDatasList[surfaceLevelInsideIndex];
            var ditchPolderSideData = (ChartPointData) chartDatasList[ditchPolderSideIndex];
            var bottomDitchPolderSideData = (ChartPointData) chartDatasList[bottomDitchPolderSideIndex];
            var bottomDitchDikeSideData = (ChartPointData) chartDatasList[bottomDitchDikeSideIndex];
            var ditchDikeSideData = (ChartPointData) chartDatasList[ditchDikeSideIndex];
            var dikeToeAtPolderData = (ChartPointData) chartDatasList[dikeToeAtPolderIndex];
            var shoulderTopInsideData = (ChartPointData) chartDatasList[shoulderTopInsideIndex];
            var shoulderBaseInsideData = (ChartPointData) chartDatasList[shoulderBaseInsideIndex];
            var dikeTopAtPolderData = (ChartPointData) chartDatasList[dikeTopAtPolderIndex];
            var dikeToeAtRiverData = (ChartPointData) chartDatasList[dikeToeAtRiverIndex];
            var dikeTopAtRiverData = (ChartPointData) chartDatasList[dikeTopAtRiverIndex];
            var surfaceLevelOutsideData = (ChartPointData) chartDatasList[surfaceLevelOutsideIndex];
            var leftGridOutsideData = (ChartPointData) chartDatasList[leftGridIndex];
            var rightGridOutsideData = (ChartPointData) chartDatasList[rightGridIndex];
            var waternetZonesExtremeData = (ChartDataCollection) chartDatasList[waternetZonesExtremeIndex];
            var waternetZonesDailyData = (ChartDataCollection) chartDatasList[waternetZonesDailyIndex];

            CollectionAssert.IsEmpty(surfaceLineData.Points);
            CollectionAssert.IsEmpty(soilProfileData.Collection);
            CollectionAssert.IsEmpty(surfaceLevelInsideData.Points);
            CollectionAssert.IsEmpty(ditchPolderSideData.Points);
            CollectionAssert.IsEmpty(bottomDitchPolderSideData.Points);
            CollectionAssert.IsEmpty(bottomDitchDikeSideData.Points);
            CollectionAssert.IsEmpty(ditchDikeSideData.Points);
            CollectionAssert.IsEmpty(dikeToeAtPolderData.Points);
            CollectionAssert.IsEmpty(shoulderTopInsideData.Points);
            CollectionAssert.IsEmpty(shoulderBaseInsideData.Points);
            CollectionAssert.IsEmpty(dikeTopAtPolderData.Points);
            CollectionAssert.IsEmpty(dikeToeAtRiverData.Points);
            CollectionAssert.IsEmpty(dikeTopAtRiverData.Points);
            CollectionAssert.IsEmpty(surfaceLevelOutsideData.Points);
            CollectionAssert.IsEmpty(leftGridOutsideData.Points);
            CollectionAssert.IsEmpty(rightGridOutsideData.Points);
            CollectionAssert.IsEmpty(waternetZonesExtremeData.Collection);
            CollectionAssert.IsEmpty(waternetZonesDailyData.Collection);

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
            Assert.AreEqual("Linker grid", leftGridOutsideData.Name);
            Assert.AreEqual("Rechter grid", rightGridOutsideData.Name);
            Assert.AreEqual("Zones extreem", waternetZonesExtremeData.Name);
            Assert.AreEqual("Zones dagelijks", waternetZonesDailyData.Name);
        }

        private static void AssertSurfaceLineChartData(MacroStabilityInwardsSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var surfaceLineChartData = (ChartLineData) chartData;

            Assert.AreEqual(surfaceLine.Points.Length, surfaceLineChartData.Points.Length);
            CollectionAssert.AreEqual(surfaceLine.LocalGeometry, surfaceLineChartData.Points);
            Assert.AreEqual(surfaceLine.Name, chartData.Name);
        }

        private static void AssertSoilProfileChartData(MacroStabilityInwardsStochasticSoilProfile soilProfile,
                                                       ChartData chartData,
                                                       bool soilLayerMultipleAreaDataShouldContainAreas)
        {
            Assert.IsInstanceOf<ChartDataCollection>(chartData);
            var soilProfileChartData = (ChartDataCollection) chartData;

            int expectedSoilLayerCount = soilProfile.SoilProfile.Layers.Count();
            Assert.AreEqual(expectedSoilLayerCount + 1, soilProfileChartData.Collection.Count());
            Assert.AreEqual(soilProfile.SoilProfile.Name, soilProfileChartData.Name);

            string[] expectedSoilLayerNames = soilProfile.SoilProfile.Layers.Select((l, i) => $"{i + 1} {l.Data.MaterialName}").Reverse().ToArray();

            for (var i = 0; i < expectedSoilLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(expectedSoilLayerNames[i], chartMultipleAreaData.Name);
                Assert.AreEqual(soilLayerMultipleAreaDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }

            var holesMultipleAreaData = soilProfileChartData.Collection.Last() as ChartMultipleAreaData;
            Assert.IsNotNull(holesMultipleAreaData);
            Assert.AreEqual("Binnenringen", holesMultipleAreaData.Name);
            Assert.IsFalse(holesMultipleAreaData.Areas.Any());
        }
    }
}