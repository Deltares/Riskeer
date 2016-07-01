﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.Forms;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingInputViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (PipingInputView view = new PipingInputView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IChartView>(view);
                Assert.IsNotNull(view.Chart);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddChartControlWithEmptyCollectionData()
        {
            // Call
            using (PipingInputView view = new PipingInputView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                ChartControl chartControl = view.Controls[0] as ChartControl;
                Assert.IsNotNull(chartControl);
                Assert.AreEqual(DockStyle.Fill, chartControl.Dock);
                Assert.IsNotNull(chartControl.Data);
                CollectionAssert.IsEmpty(chartControl.Data.List);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Distance_DisplayName, chartControl.BottomAxisTitle);
                Assert.AreEqual(RingtoetsCommonFormsResources.InputView_Height_DisplayName, chartControl.LeftAxisTitle);
            }
        }

        [Test]
        public void Data_PipingCalculation_DataSet()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput());

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanPipingCalculationScenario_DataNull()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                object calculation = new object();

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
            using (PipingInputView view = new PipingInputView())
            {
                var surfaceLine = GetSurfaceLineWithGeometry();
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(10, view.Chart.Data.List.Count);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Chart.Data);
            }
        }

        [Test]
        public void Data_WithSurfaceLineAndSoilProfile_ChartDataSetForSurfaceLineAndSoilProfile()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var surfaceLine = GetSurfaceLineWithGeometry();
                surfaceLine.SetDitchDikeSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetBottomDitchDikeSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetDitchPolderSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetBottomDitchPolderSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetDikeToeAtPolderAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetDikeToeAtRiverAt(new Point3D(1.2, 2.3, 4.0));

                var stochasticSoilProfile = GetStochasticSoilProfile();
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
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
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(10, chartData.List.Count);
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.List[soilProfileIndex]);
                AssertSurfaceLineChartData(surfaceLine, chartData.List[surfaceLineIndex]);
                AssertEntryPointLPointchartData(calculation.InputParameters, surfaceLine, chartData.List[entryPointIndex]);
                AssertExitPointLPointchartData(calculation.InputParameters, surfaceLine, chartData.List[exitPointIndex]);
                AssertCharacteristicPoints(surfaceLine, chartData.List);
            }
        }

        [Test]
        public void Data_WithoutSurfaceLine_CollectionOfEmptyChartDataSet()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput());

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(10, chartData.List.Count);
                var soilProfileData = (ChartDataCollection) chartData.List[soilProfileIndex];
                var lineData = (ChartLineData) chartData.List[surfaceLineIndex];
                var entryPointData = (ChartPointData) chartData.List[entryPointIndex];
                var exitPointData = (ChartPointData) chartData.List[exitPointIndex];
                var ditchDikeSideData = (ChartPointData) chartData.List[ditchDikeSideIndex];
                var bottomDitchDikeSideData = (ChartPointData) chartData.List[bottomDitchDikeSideIndex];
                var ditchPolderSideData = (ChartPointData) chartData.List[ditchPolderSideIndex];
                var bottomDitchPolderSideData = (ChartPointData) chartData.List[bottomDitchPolderSideIndex];
                var dikeToeAtPolderData = (ChartPointData) chartData.List[dikeToeAtPolderIndex];
                var dikeToeAtRiverData = (ChartPointData) chartData.List[dikeToeAtRiverIndex];

                CollectionAssert.IsEmpty(soilProfileData.List);
                CollectionAssert.IsEmpty(lineData.Points);
                CollectionAssert.IsEmpty(entryPointData.Points);
                CollectionAssert.IsEmpty(exitPointData.Points);
                CollectionAssert.IsEmpty(ditchDikeSideData.Points);
                CollectionAssert.IsEmpty(bottomDitchDikeSideData.Points);
                CollectionAssert.IsEmpty(ditchPolderSideData.Points);
                CollectionAssert.IsEmpty(bottomDitchPolderSideData.Points);
                CollectionAssert.IsEmpty(dikeToeAtPolderData.Points);
                CollectionAssert.IsEmpty(dikeToeAtRiverData.Points);
                Assert.AreEqual(Resources.StochasticSoilProfileProperties_DisplayName, soilProfileData.Name);
                Assert.AreEqual(Resources.RingtoetsPipingSurfaceLine_DisplayName, lineData.Name);
                Assert.AreEqual(Resources.PipingInput_EntryPointL_DisplayName, entryPointData.Name);
                Assert.AreEqual(Resources.PipingInput_ExitPointL_DisplayName, exitPointData.Name);
                Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchDikeSide, ditchDikeSideData.Name);
                Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, bottomDitchDikeSideData.Name);
                Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchPolderSide, ditchPolderSideData.Name);
                Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, bottomDitchPolderSideData.Name);
                Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtPolder, dikeToeAtPolderData.Name);
                Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtRiver, dikeToeAtRiverData.Name);
            }
        }

        [Test]
        public void Data_WithSurfaceLineWithoutStochasticSoilProfile_CollectionOfEmptyChartDataSetForSoilProfile()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = GetSurfaceLineWithGeometry()
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(10, chartData.List.Count);
                var soilProfileData = (ChartDataCollection) chartData.List[soilProfileIndex];
                CollectionAssert.IsEmpty(soilProfileData.List);
                Assert.AreEqual(Resources.StochasticSoilProfileProperties_DisplayName, soilProfileData.Name);
            }
        }

        [Test]
        public void Data_WithSurfaceLineWithoutSoilProfile_CollectionOfEmptyChartDataSetForSoilProfile()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = GetSurfaceLineWithGeometry(),
                        StochasticSoilProfile = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(10, chartData.List.Count);
                var soilProfileData = (ChartDataCollection) chartData.List[soilProfileIndex];
                CollectionAssert.IsEmpty(soilProfileData.List);
                Assert.AreEqual(Resources.StochasticSoilProfileProperties_DisplayName, soilProfileData.Name);
            }
        }

        [Test]
        public void UpdateObservers_CalculationNameUpdated_ChartTitleUpdated()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var initialName = "Initial name";
                var updatedName = "Updated name";

                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
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
        public void UpdateObservers_OtherCalculationNameUpdated_ChartTitleNotUpdated()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var initialName = "Initial name";
                var updatedName = "Updated name";

                var calculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = initialName
                };
                var calculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = initialName
                };

                view.Data = calculation1;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                calculation2.Name = updatedName;

                // Call
                calculation1.NotifyObservers();

                // Assert
                Assert.AreEqual(initialName, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void UpdateObservers_CalculationUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var initialName = "Initial name";
                var updatedName = "Updated name";

                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = initialName
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                var calculation2 = new PipingCalculationScenario(new GeneralPipingInput())
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
        public void UpdateObservers_CalculationSurfaceLineUpdated_SetNewChartData()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var characteristicPoint = new Point3D(1.2, 2.3, 4.0);
                var surfaceLine = GetSurfaceLineWithGeometry();

                surfaceLine.SetDitchDikeSideAt(characteristicPoint);
                surfaceLine.SetBottomDitchDikeSideAt(characteristicPoint);
                surfaceLine.SetDitchPolderSideAt(characteristicPoint);
                surfaceLine.SetBottomDitchPolderSideAt(characteristicPoint);
                surfaceLine.SetDikeToeAtPolderAt(characteristicPoint);
                surfaceLine.SetDikeToeAtRiverAt(characteristicPoint);

                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                view.Data = calculation;
                ChartLineData oldSurfaceLineChartData = (ChartLineData) view.Chart.Data.List[surfaceLineIndex];
                ChartPointData oldEntryPointChartData = (ChartPointData) view.Chart.Data.List[entryPointIndex];
                ChartPointData oldExitPointChartData = (ChartPointData) view.Chart.Data.List[exitPointIndex];
                ChartPointData oldDitchDikeSideData = (ChartPointData) view.Chart.Data.List[ditchDikeSideIndex];
                ChartPointData oldBottomDitchDikeSideData = (ChartPointData) view.Chart.Data.List[bottomDitchDikeSideIndex];
                ChartPointData oldDitchPolderSideData = (ChartPointData) view.Chart.Data.List[ditchPolderSideIndex];
                ChartPointData oldBottomDitchPolderSideData = (ChartPointData) view.Chart.Data.List[bottomDitchPolderSideIndex];
                ChartPointData oldDikeToeAtPolderData = (ChartPointData) view.Chart.Data.List[dikeToeAtPolderIndex];
                ChartPointData oldDikeToeAtRiverData = (ChartPointData) view.Chart.Data.List[dikeToeAtRiverIndex];

                var characteristicPoint2 = new Point3D(3.5, 2.3, 8.0);
                var surfaceLine2 = GetSecondSurfaceLineWithGeometry();

                surfaceLine2.SetDitchDikeSideAt(characteristicPoint2);
                surfaceLine2.SetBottomDitchDikeSideAt(characteristicPoint2);
                surfaceLine2.SetDitchPolderSideAt(characteristicPoint2);
                surfaceLine2.SetBottomDitchPolderSideAt(characteristicPoint2);
                surfaceLine2.SetDikeToeAtPolderAt(characteristicPoint2);
                surfaceLine2.SetDikeToeAtRiverAt(characteristicPoint2);

                calculation.InputParameters.SurfaceLine = surfaceLine2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                ChartLineData newSurfaceLineChartData = (ChartLineData) view.Chart.Data.List[surfaceLineIndex];
                Assert.AreNotEqual(oldSurfaceLineChartData, newSurfaceLineChartData);
                AssertSurfaceLineChartData(surfaceLine2, newSurfaceLineChartData);

                ChartPointData newEntryPointChartData = (ChartPointData) view.Chart.Data.List[entryPointIndex];
                Assert.AreNotEqual(oldEntryPointChartData, newEntryPointChartData);
                AssertEntryPointLPointchartData(calculation.InputParameters, surfaceLine2, newEntryPointChartData);

                ChartPointData newExitPointChartData = (ChartPointData) view.Chart.Data.List[exitPointIndex];
                Assert.AreNotEqual(oldExitPointChartData, newExitPointChartData);
                AssertExitPointLPointchartData(calculation.InputParameters, surfaceLine2, newExitPointChartData);

                ChartPointData newDitchDikeSideData = (ChartPointData) view.Chart.Data.List[ditchDikeSideIndex];
                Assert.AreNotEqual(oldDitchDikeSideData, newDitchDikeSideData);

                ChartPointData newBottomDitchDikeSideData = (ChartPointData) view.Chart.Data.List[bottomDitchDikeSideIndex];
                Assert.AreNotEqual(oldBottomDitchDikeSideData, newBottomDitchDikeSideData);

                ChartPointData newDitchPolderSideData = (ChartPointData) view.Chart.Data.List[ditchPolderSideIndex];
                Assert.AreNotEqual(oldDitchPolderSideData, newDitchPolderSideData);

                ChartPointData newBottomDitchPolderSideData = (ChartPointData) view.Chart.Data.List[bottomDitchPolderSideIndex];
                Assert.AreNotEqual(oldBottomDitchPolderSideData, newBottomDitchPolderSideData);

                ChartPointData newDikeToeAtPolderData = (ChartPointData) view.Chart.Data.List[dikeToeAtPolderIndex];
                Assert.AreNotEqual(oldDikeToeAtPolderData, newDikeToeAtPolderData);

                ChartPointData newDikeToeAtRiverData = (ChartPointData) view.Chart.Data.List[dikeToeAtRiverIndex];
                Assert.AreNotEqual(oldDikeToeAtRiverData, newDikeToeAtRiverData);

                AssertCharacteristicPoints(surfaceLine2, view.Chart.Data.List);
            }
        }

        [Test]
        public void UpdateObservers_StochasticSoilProfileUpdated_SetNewChartData()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var surfaceLine = GetSurfaceLineWithGeometry();
                var soilProfile = GetStochasticSoilProfile();
                var soilProfile2 = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = new PipingSoilProfile("profile", 3, new[]
                    {
                        new PipingSoilLayer(6),
                        new PipingSoilLayer(8),
                        new PipingSoilLayer(9)
                    }, SoilProfileType.SoilProfile1D, 1)
                };

                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = soilProfile
                    }
                };

                view.Data = calculation;
                ChartDataCollection oldSoilProfileData = (ChartDataCollection)view.Chart.Data.List[soilProfileIndex];

                calculation.InputParameters.StochasticSoilProfile = soilProfile2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                ChartDataCollection newSoilProfileData = (ChartDataCollection)view.Chart.Data.List[soilProfileIndex];
                Assert.AreNotEqual(oldSoilProfileData, newSoilProfileData);
                AssertSoilProfileChartData(soilProfile2, newSoilProfileData);
            }
        }

        [Test]
        public void UpdateObserver_OtherPipingCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var surfaceLine = GetSurfaceLineWithGeometry();
                PipingCalculationScenario calculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                PipingCalculationScenario calculation2 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                view.Data = calculation1;

                var surfaceLine2 = GetSecondSurfaceLineWithGeometry();

                calculation2.InputParameters.SurfaceLine = surfaceLine2;

                // Call
                calculation2.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(calculation1, view.Data);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var surfaceLine = GetSurfaceLineWithGeometry();
                PipingCalculationScenario calculation1 = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                PipingCalculationScenario calculation2 = new PipingCalculationScenario(new GeneralPipingInput());

                view.Data = calculation1;
                ChartData dataBeforeUpdate = view.Chart.Data;

                view.Data = calculation2;

                var surfaceLine2 = GetSecondSurfaceLineWithGeometry();

                calculation1.InputParameters.SurfaceLine = surfaceLine2;

                // Call
                calculation1.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
            }
        }

        private const int soilProfileIndex = 0;
        private const int surfaceLineIndex = 1;
        private const int ditchPolderSideIndex = 2;
        private const int bottomDitchPolderSideIndex = 3;
        private const int bottomDitchDikeSideIndex = 4;
        private const int ditchDikeSideIndex = 5;
        private const int dikeToeAtPolderIndex = 6;
        private const int dikeToeAtRiverIndex = 7;
        private const int exitPointIndex = 8;
        private const int entryPointIndex = 9;

        private static StochasticSoilProfile GetStochasticSoilProfile()
        {
            return new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new PipingSoilProfile("profile", -1, new[]
                {
                    new PipingSoilLayer(1),
                    new PipingSoilLayer(3),
                    new PipingSoilLayer(5)
                }, SoilProfileType.SoilProfile1D, 1)
            };
        }

        private static RingtoetsPipingSurfaceLine GetSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            return GetSurfaceLine(points);
        }

        private static RingtoetsPipingSurfaceLine GetSecondSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(3.5, 2.3, 8.0),
                new Point3D(6.9, 2.0, 2.0)
            };

           return GetSurfaceLine(points);
        }

        private static RingtoetsPipingSurfaceLine GetSurfaceLine(Point3D[] points)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line name"
            };
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }
        
        private void AssertSoilProfileChartData(StochasticSoilProfile soilProfile, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartDataCollection>(chartData);
            ChartDataCollection soilProfileChartData = (ChartDataCollection)chartData;

            var expectedLayerCount = soilProfile.SoilProfile.Layers.Count();
            Assert.AreEqual(expectedLayerCount, soilProfileChartData.List.Count);
            Assert.AreEqual(soilProfile.SoilProfile.Name, soilProfileChartData.Name);

            var pipingSoilLayers = soilProfile.SoilProfile.Layers.Select((l,i) => string.Format("{0} {1}", i + 1, l.MaterialName)).Reverse().ToArray();

            for (int i = 0; i < expectedLayerCount; i++)
            {
                Assert.AreEqual(pipingSoilLayers[i], soilProfileChartData.List[i].Name);
            }
        }
        
        private void AssertSurfaceLineChartData(RingtoetsPipingSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            ChartLineData surfaceLineChartData = (ChartLineData) chartData;

            Assert.AreEqual(surfaceLine.Points.Length, surfaceLineChartData.Points.Count());
            CollectionAssert.AreEqual(surfaceLine.ProjectGeometryToLZ(), surfaceLineChartData.Points);
            Assert.AreEqual(surfaceLine.Name, chartData.Name);
        }

        private void AssertEntryPointLPointchartData(PipingInput pipingInput, RingtoetsPipingSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartPointData>(chartData);
            ChartPointData entryPointChartData = (ChartPointData) chartData;

            Assert.AreEqual(1, entryPointChartData.Points.Count());
            Point2D entryPoint = new Point2D(pipingInput.EntryPointL, surfaceLine.GetZAtL(pipingInput.EntryPointL));
            CollectionAssert.AreEqual(new[]
            {
                entryPoint
            }, entryPointChartData.Points);
            Assert.AreEqual(Resources.PipingInput_EntryPointL_DisplayName, entryPointChartData.Name);
        }

        private void AssertExitPointLPointchartData(PipingInput pipingInput, RingtoetsPipingSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartPointData>(chartData);
            ChartPointData exitPointChartData = (ChartPointData) chartData;

            Assert.AreEqual(1, exitPointChartData.Points.Count());
            Point2D exitPoint = new Point2D(pipingInput.ExitPointL, surfaceLine.GetZAtL(pipingInput.ExitPointL));
            CollectionAssert.AreEqual(new[]
            {
                exitPoint
            }, exitPointChartData.Points);
            Assert.AreEqual(Resources.PipingInput_ExitPointL_DisplayName, exitPointChartData.Name);
        }

        private void AssertCharacteristicPoints(RingtoetsPipingSurfaceLine surfaceLine, IList<ChartData> characteristicPoints)
        {
            Point3D first = surfaceLine.Points.First();
            Point3D last = surfaceLine.Points.Last();
            Point2D firstPoint = new Point2D(first.X, first.Y);
            Point2D lastPoint = new Point2D(last.X, last.Y);

            var ditchDikeSideData = (ChartPointData) characteristicPoints[ditchDikeSideIndex];
            Assert.AreEqual(1, ditchDikeSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DitchDikeSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, ditchDikeSideData.Points);
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchDikeSide, ditchDikeSideData.Name);

            var bottomDitchDikeSideData = (ChartPointData) characteristicPoints[bottomDitchDikeSideIndex];
            Assert.AreEqual(1, bottomDitchDikeSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.BottomDitchDikeSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, bottomDitchDikeSideData.Points);
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, bottomDitchDikeSideData.Name);

            var ditchPolderSideData = (ChartPointData) characteristicPoints[ditchPolderSideIndex];
            Assert.AreEqual(1, ditchPolderSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DitchPolderSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, ditchPolderSideData.Points);
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchPolderSide, ditchPolderSideData.Name);

            var bottomDitchPolderSideData = (ChartPointData) characteristicPoints[bottomDitchPolderSideIndex];
            Assert.AreEqual(1, bottomDitchPolderSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.BottomDitchPolderSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, bottomDitchPolderSideData.Points);
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, bottomDitchPolderSideData.Name);

            var dikeToeAtPolderData = (ChartPointData) characteristicPoints[dikeToeAtPolderIndex];
            Assert.AreEqual(1, dikeToeAtPolderData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DikeToeAtPolder.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, dikeToeAtPolderData.Points);
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtPolder, dikeToeAtPolderData.Name);

            var dikeToeAtRiverData = (ChartPointData) characteristicPoints[dikeToeAtRiverIndex];
            Assert.AreEqual(1, dikeToeAtRiverData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DikeToeAtRiver.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, dikeToeAtRiverData.Points);
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtRiver, dikeToeAtRiverData.Name);
        }
    }
}