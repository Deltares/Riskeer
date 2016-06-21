// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Components.OxyPlot.Forms;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

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
                Assert.AreEqual(Resources.PipingInputView_Distance_DisplayName, chartControl.BottomAxisTitle);
                Assert.AreEqual(Resources.PipingInputView_Height_DisplayName, chartControl.LeftAxisTitle);
            }
        }

        [Test]
        public void Data_PipingInput_DataSet()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                PipingInput input = new PipingInput(new GeneralPipingInput());

                // Call
                view.Data = input;

                // Assert
                Assert.AreSame(input, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanPipingInput_DataNull()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                object input = new object();

                // Call
                view.Data = input;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Calculation_Always_SetsCalculationAndUpdateChartTitle()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                PipingCalculationScenario calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = "Test name"
                };

                // Call
                view.Calculation = calculation;

                // Assert
                Assert.AreSame(calculation, view.Calculation);
                Assert.AreEqual(calculation.Name, view.Chart.ChartTitle);
            }
        }

        [Test]
        public void Data_SetChartData_ChartDataSet()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var points = new[]
                {
                    new Point3D(1.2, 2.3, 4.0),
                    new Point3D(2.7, 2.0, 6.0)
                };

                var surfaceLine = new RingtoetsPipingSurfaceLine()
                {
                    Name = "Surface line name"
                };
                surfaceLine.SetGeometry(points);

                var pipingInput = new PipingInput(new GeneralPipingInput())
                {
                    SurfaceLine = surfaceLine
                };

                // Call
                view.Data = pipingInput;

                // Assert
                Assert.AreSame(pipingInput, view.Data);
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(2, chartData.List.Count);
                AssertSurfaceLineChartData(surfaceLine, chartData.List[surfaceLineIndex]);
                AssertEntryPointLPointchartData(pipingInput, surfaceLine, chartData.List[entryPointIndex]);
            }
        }

        [Test]
        public void Data_WithoutSurfaceLine_ChartDataSet()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var pipingInput = new PipingInput(new GeneralPipingInput());

                // Call
                view.Data = pipingInput;

                // Assert
                Assert.AreSame(pipingInput, view.Data);
                Assert.IsInstanceOf<ChartDataCollection>(view.Chart.Data);
                var chartData = view.Chart.Data;
                Assert.IsNotNull(chartData);
                Assert.AreEqual(Resources.PipingInputContext_NodeDisplayName, chartData.Name);

                Assert.AreEqual(2, chartData.List.Count);
                var lineData = (ChartLineData) chartData.List[surfaceLineIndex];
                var pointData = (ChartPointData) chartData.List[entryPointIndex];

                Assert.AreEqual(0, lineData.Points.Count());
                Assert.AreEqual(0, pointData.Points.Count());
                Assert.AreEqual(Resources.RingtoetsPipingSurfaceLine_DisplayName, lineData.Name);
                Assert.AreEqual(Resources.PipingInput_EntryPointL_DisplayName, pointData.Name);
            }
        }

        [Test]
        public void UpdateObservers_CalculationNotSet_ChartTitleNotUpdated()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var pipingInput = new PipingInput(new GeneralPipingInput());
                view.Data = pipingInput;

                // Precondition
                Assert.IsNull(view.Chart.ChartTitle);

                // Call
                pipingInput.NotifyObservers();

                // Assert
                Assert.IsNull(view.Chart.ChartTitle);
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

                view.Calculation = calculation;

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

                view.Calculation = calculation1;

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

                view.Calculation = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                view.Calculation = null;

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
                var points = new[]
                {
                    new Point3D(1.2, 2.3, 4.0),
                    new Point3D(2.7, 2.0, 6.0)
                };

                var surfaceLine = new RingtoetsPipingSurfaceLine
                {
                    Name = "Surface line name"
                };
                surfaceLine.SetGeometry(points);

                var pipingInput = new PipingInput(new GeneralPipingInput())
                {
                    SurfaceLine = surfaceLine
                };

                view.Data = pipingInput;
                ChartLineData oldSurfaceLineChartData = (ChartLineData) view.Chart.Data.List[surfaceLineIndex];
                ChartPointData oldEntryPointChartData = (ChartPointData)view.Chart.Data.List[entryPointIndex];

                var points2 = new[]
                {
                    new Point3D(3.5, 2.3, 8.0),
                    new Point3D(6.9, 2.0, 2.0)
                };

                var surfaceLine2 = new RingtoetsPipingSurfaceLine
                {
                    Name = "Surface line name"
                };
                surfaceLine2.SetGeometry(points2);

                pipingInput.SurfaceLine = surfaceLine2;

                // Call
                pipingInput.NotifyObservers();

                // Assert
                ChartLineData newSurfaceLineChartData = (ChartLineData)view.Chart.Data.List[surfaceLineIndex];
                Assert.AreNotEqual(oldSurfaceLineChartData, newSurfaceLineChartData);
                AssertSurfaceLineChartData(surfaceLine2, newSurfaceLineChartData);

                ChartPointData newEntryPointChartData = (ChartPointData) view.Chart.Data.List[entryPointIndex];
                Assert.AreNotEqual(oldEntryPointChartData, newEntryPointChartData);
                AssertEntryPointLPointchartData(pipingInput, surfaceLine2, newEntryPointChartData);
            }
        }

        [Test]
        public void UpdateObserver_OtherPipingInputUpdated_ChartDataNotUpdated()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var points = new[]
                {
                    new Point3D(1.2, 2.3, 4.0),
                    new Point3D(2.7, 2.0, 6.0)
                };

                var surfaceLine = new RingtoetsPipingSurfaceLine
                {
                    Name = "Surface line name"
                };
                surfaceLine.SetGeometry(points);
                var input1 = new PipingInput(new GeneralPipingInput())
                {
                    SurfaceLine = surfaceLine
                };

                var input2 = new PipingInput(new GeneralPipingInput())
                {
                    SurfaceLine = surfaceLine
                };

                view.Data = input1;

                var points2 = new[]
                {
                    new Point3D(3.5, 2.3, 8.0),
                    new Point3D(6.9, 2.0, 2.0)
                };

                var surfaceLine2 = new RingtoetsPipingSurfaceLine();
                surfaceLine2.SetGeometry(points2);

                input2.SurfaceLine = surfaceLine2;

                // Call
                input2.NotifyObservers();

                // Assert
                Assert.AreEqual(input1, view.Data);
            }
        }

        [Test]
        public void UpdateObserver_DataNull_ChartDataNotUpdated()
        {
            // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var points = new[]
                {
                    new Point3D(1.2, 2.3, 4.0),
                    new Point3D(2.7, 2.0, 6.0)
                };

                var surfaceLine = new RingtoetsPipingSurfaceLine
                {
                    Name = "Surface line name"
                };
                surfaceLine.SetGeometry(points);
                var input = new PipingInput(new GeneralPipingInput())
                {
                    SurfaceLine = surfaceLine
                };

                view.Data = input;

                ChartData dataBeforeUpdate = view.Chart.Data;

                view.Data = null;

                var points2 = new[]
                {
                    new Point3D(3.5, 2.3, 8.0),
                    new Point3D(6.9, 2.0, 2.0)
                };

                var surfaceLine2 = new RingtoetsPipingSurfaceLine();
                surfaceLine2.SetGeometry(points2);

                input.SurfaceLine = surfaceLine2;

                // Call
                input.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
             // Setup
            using (PipingInputView view = new PipingInputView())
            {
                var points = new[]
                {
                    new Point3D(1.2, 2.3, 4.0),
                    new Point3D(2.7, 2.0, 6.0)
                };

                var surfaceLine = new RingtoetsPipingSurfaceLine
                {
                    Name = "Surface line name"
                };
                surfaceLine.SetGeometry(points);
                var input1 = new PipingInput(new GeneralPipingInput())
                {
                    SurfaceLine = surfaceLine
                };
                var input2 = new PipingInput(new GeneralPipingInput());

                view.Data = input1;
                ChartData dataBeforeUpdate = view.Chart.Data;

                view.Data = input2;

                var points2 = new[]
                {
                    new Point3D(3.5, 2.3, 8.0),
                    new Point3D(6.9, 2.0, 2.0)
                };

                var surfaceLine2 = new RingtoetsPipingSurfaceLine();
                surfaceLine2.SetGeometry(points2);

                input1.SurfaceLine = surfaceLine2;

                // Call
                input1.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
            }
        }

        private const int surfaceLineIndex = 1;
        private const int entryPointIndex = 0;

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
            CollectionAssert.AreEqual(new[] { entryPoint }, entryPointChartData.Points);
            Assert.AreEqual(Resources.PipingInput_EntryPointL_DisplayName, entryPointChartData.Name);
        }
    }
}