// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingInputViewTest
    {
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

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new PipingInputView())
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
            using (var view = new PipingInputView())
            {
                // Assert
                var chartControl = (IChartControl) view.Controls.Find("chartControl", true).First();
                Assert.IsInstanceOf<Control>(chartControl);
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
            using (var view = new PipingInputView())
            {
                // Assert
                var tableControl = (PipingSoilLayerTable) view.Controls.Find("pipingSoilLayerTable", true).First();
                Assert.NotNull(tableControl);
                Assert.AreEqual(DockStyle.Bottom, tableControl.Dock);
                CollectionAssert.IsEmpty(tableControl.Rows);
            }
        }

        [Test]
        public void Data_PipingCalculationScenario_DataSet()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());

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
            using (var view = new PipingInputView())
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
            using (var view = new PipingInputView
            {
                Data = new PipingCalculationScenario(new GeneralPipingInput())
            })
            {
                // Precondition
                Assert.AreEqual(10, view.Chart.Data.Collection.Count());
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
            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                InputParameters =
                {
                    StochasticSoilProfile = GetStochasticSoilProfile()
                }
            };

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                var tableControl = view.Controls.Find("pipingSoilLayerTable", true).First() as PipingSoilLayerTable;

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
        public void Data_WithSurfaceLineAndSoilProfile_DataUpdatedToCollectionOfFilledChartData()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                surfaceLine.SetDitchDikeSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetBottomDitchDikeSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetDitchPolderSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetBottomDitchPolderSideAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetDikeToeAtPolderAt(new Point3D(1.2, 2.3, 4.0));
                surfaceLine.SetDikeToeAtRiverAt(new Point3D(1.2, 2.3, 4.0));

                PipingStochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile();
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
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
                Assert.AreEqual(10, chartData.Collection.Count());
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), true);
                AssertSurfaceLineChartData(surfaceLine, chartData.Collection.ElementAt(surfaceLineIndex));
                AssertEntryPointLPointchartData(calculation.InputParameters, surfaceLine, chartData.Collection.ElementAt(entryPointIndex));
                AssertExitPointLPointchartData(calculation.InputParameters, surfaceLine, chartData.Collection.ElementAt(exitPointIndex));
                AssertCharacteristicPoints(surfaceLine, chartData.Collection.ToList());
            }
        }

        [Test]
        public void Data_WithoutSurfaceLine_NoChartDataSet()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());

                // Call
                view.Data = calculation;

                // Assert
                AssertEmptyChartData(view.Chart.Data);
            }
        }

        [Test]
        public void Data_WithSurfaceLineWithoutStochasticSoilProfile_CollectionOfEmptyChartDataSetForSoilProfile()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                ChartDataCollection chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(10, chartData.Collection.Count());
                var soilProfileData = (ChartDataCollection) chartData.Collection.ElementAt(soilProfileIndex);
                CollectionAssert.IsEmpty(soilProfileData.Collection);
                Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
            }
        }

        [Test]
        public void Data_WithoutStochasticSoilProfile_SoilLayerTableEmpty()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput());

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
            using (var view = new PipingInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

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
        public void UpdateObserver_PreviousCalculationUpdated_ChartTitleNotUpdated()
        {
            // Setup
            using (var view = new PipingInputView())
            {
                const string initialName = "Initial name";
                const string updatedName = "Updated name";

                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = initialName
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                view.Data = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    Name = initialName
                };

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
            observer.Expect(o => o.UpdateObserver()).Repeat.Times(9);
            mocks.ReplayAll();

            using (var view = new PipingInputView())
            {
                var characteristicPoint = new Point3D(1.2, 2.3, 4.0);
                PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                surfaceLine.SetDitchDikeSideAt(characteristicPoint);
                surfaceLine.SetBottomDitchDikeSideAt(characteristicPoint);
                surfaceLine.SetDitchPolderSideAt(characteristicPoint);
                surfaceLine.SetBottomDitchPolderSideAt(characteristicPoint);
                surfaceLine.SetDikeToeAtPolderAt(characteristicPoint);
                surfaceLine.SetDikeToeAtRiverAt(characteristicPoint);

                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };

                view.Data = calculation;

                List<ChartData> chartDataList = view.Chart.Data.Collection.ToList();
                var surfaceLineChartData = (ChartLineData) chartDataList[surfaceLineIndex];
                var entryPointChartData = (ChartPointData) chartDataList[entryPointIndex];
                var exitPointChartData = (ChartPointData) chartDataList[exitPointIndex];
                var ditchDikeSideData = (ChartPointData) chartDataList[ditchDikeSideIndex];
                var bottomDitchDikeSideData = (ChartPointData) chartDataList[bottomDitchDikeSideIndex];
                var ditchPolderSideData = (ChartPointData) chartDataList[ditchPolderSideIndex];
                var bottomDitchPolderSideData = (ChartPointData) chartDataList[bottomDitchPolderSideIndex];
                var dikeToeAtPolderData = (ChartPointData) chartDataList[dikeToeAtPolderIndex];
                var dikeToeAtRiverData = (ChartPointData) chartDataList[dikeToeAtRiverIndex];

                surfaceLineChartData.Attach(observer);
                entryPointChartData.Attach(observer);
                exitPointChartData.Attach(observer);
                ditchDikeSideData.Attach(observer);
                bottomDitchDikeSideData.Attach(observer);
                ditchPolderSideData.Attach(observer);
                bottomDitchPolderSideData.Attach(observer);
                dikeToeAtPolderData.Attach(observer);
                dikeToeAtRiverData.Attach(observer);

                var characteristicPoint2 = new Point3D(3.5, 2.3, 8.0);
                PipingSurfaceLine surfaceLine2 = GetSecondSurfaceLineWithGeometry();

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
                chartDataList = view.Chart.Data.Collection.ToList();

                Assert.AreSame(surfaceLineChartData, (ChartLineData) chartDataList[surfaceLineIndex]);
                Assert.AreSame(entryPointChartData, (ChartPointData) chartDataList[entryPointIndex]);
                Assert.AreSame(exitPointChartData, (ChartPointData) chartDataList[exitPointIndex]);
                Assert.AreSame(ditchDikeSideData, (ChartPointData) chartDataList[ditchDikeSideIndex]);
                Assert.AreSame(bottomDitchDikeSideData, (ChartPointData) chartDataList[bottomDitchDikeSideIndex]);
                Assert.AreSame(ditchPolderSideData, (ChartPointData) chartDataList[ditchPolderSideIndex]);
                Assert.AreSame(bottomDitchPolderSideData, (ChartPointData) chartDataList[bottomDitchPolderSideIndex]);
                Assert.AreSame(dikeToeAtPolderData, (ChartPointData) chartDataList[dikeToeAtPolderIndex]);
                Assert.AreSame(dikeToeAtRiverData, (ChartPointData) chartDataList[dikeToeAtRiverIndex]);

                AssertSurfaceLineChartData(surfaceLine2, surfaceLineChartData);
                AssertEntryPointLPointchartData(calculation.InputParameters, surfaceLine2, entryPointChartData);
                AssertExitPointLPointchartData(calculation.InputParameters, surfaceLine2, exitPointChartData);
                AssertCharacteristicPoints(surfaceLine2, chartDataList);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_StochasticSoilProfileUpdated_ChartDataUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new PipingInputView())
            {
                PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                PipingStochasticSoilProfile soilProfile = GetStochasticSoilProfile();
                var soilProfile2 = new PipingStochasticSoilProfile(
                    0.5, new PipingSoilProfile("profile 2", -2, new[]
                    {
                        new PipingSoilLayer(0)
                        {
                            MaterialName = "Grass"
                        },
                        new PipingSoilLayer(2)
                        {
                            MaterialName = "Stone"
                        },
                        new PipingSoilLayer(3)
                        {
                            MaterialName = "Peat"
                        }
                    }, SoilProfileType.SoilProfile1D));

                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = soilProfile
                    }
                };

                view.Data = calculation;

                var soilProfileData = (ChartDataCollection) view.Chart.Data.Collection.ElementAt(soilProfileIndex);
                soilProfileData.Attach(observer);

                calculation.InputParameters.StochasticSoilProfile = soilProfile2;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                Assert.AreSame(soilProfileData, (ChartDataCollection) view.Chart.Data.Collection.ElementAt(soilProfileIndex));
                AssertSoilProfileChartData(soilProfile2, soilProfileData, true);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_ChartSeriesSameOrder()
        {
            // Setup
            const int updatedSoilProfileIndex = soilProfileIndex;
            const int updatedSurfaceLineIndex = surfaceLineIndex + 8;
            const int updatedDitchPolderSideIndex = ditchPolderSideIndex - 1;
            const int updatedBottomDitchPolderSideIndex = bottomDitchPolderSideIndex - 1;
            const int updatedBottomDitchDikeSideIndex = bottomDitchDikeSideIndex - 1;
            const int updatedDitchDikeSideIndex = ditchDikeSideIndex - 1;
            const int updatedDikeToeAtPolderIndex = dikeToeAtPolderIndex - 1;
            const int updatedDikeToeAtRiverIndex = dikeToeAtRiverIndex - 1;
            const int updatedExitPointIndex = exitPointIndex - 1;
            const int updatedEntryPointIndex = entryPointIndex - 1;

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            using (var view = new PipingInputView
            {
                Data = calculation
            })
            {
                ChartDataCollection chartData = view.Chart.Data;

                ChartData dataToMove = chartData.Collection.ElementAt(surfaceLineIndex);
                chartData.Remove(dataToMove);
                chartData.Add(dataToMove);

                List<ChartData> chartDataList = chartData.Collection.ToList();

                var soilProfileData = (ChartDataCollection) chartDataList[updatedSoilProfileIndex];
                var surfaceLineData = (ChartLineData) chartDataList[updatedSurfaceLineIndex];
                var entryPointData = (ChartPointData) chartDataList[updatedEntryPointIndex];
                var exitPointData = (ChartPointData) chartDataList[updatedExitPointIndex];
                var ditchDikeSideData = (ChartPointData) chartDataList[updatedDitchDikeSideIndex];
                var bottomDitchDikeSideData = (ChartPointData) chartDataList[updatedBottomDitchDikeSideIndex];
                var ditchPolderSideData = (ChartPointData) chartDataList[updatedDitchPolderSideIndex];
                var bottomDitchPolderSideData = (ChartPointData) chartDataList[updatedBottomDitchPolderSideIndex];
                var dikeToeAtPolderData = (ChartPointData) chartDataList[updatedDikeToeAtPolderIndex];
                var dikeToeAtRiverData = (ChartPointData) chartDataList[updatedDikeToeAtRiverIndex];

                Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
                Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
                Assert.AreEqual("Intredepunt", entryPointData.Name);
                Assert.AreEqual("Uittredepunt", exitPointData.Name);
                Assert.AreEqual("Insteek sloot dijkzijde", ditchDikeSideData.Name);
                Assert.AreEqual("Slootbodem dijkzijde", bottomDitchDikeSideData.Name);
                Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);
                Assert.AreEqual("Slootbodem polderzijde", bottomDitchPolderSideData.Name);
                Assert.AreEqual("Teen dijk binnenwaarts", dikeToeAtPolderData.Name);
                Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);

                PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                calculation.InputParameters.SurfaceLine = surfaceLine;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                chartDataList = chartData.Collection.ToList();

                var actualSoilProfileData = (ChartDataCollection) chartDataList[updatedSoilProfileIndex];
                var actualSurfaceLineData = (ChartLineData) chartDataList[updatedSurfaceLineIndex];
                var actualEntryPointData = (ChartPointData) chartDataList[updatedEntryPointIndex];
                var actualExitPointData = (ChartPointData) chartDataList[updatedExitPointIndex];
                var actualDitchDikeSideData = (ChartPointData) chartDataList[updatedDitchDikeSideIndex];
                var actualBottomDitchDikeSideData = (ChartPointData) chartDataList[updatedBottomDitchDikeSideIndex];
                var actualDitchPolderSideData = (ChartPointData) chartDataList[updatedDitchPolderSideIndex];
                var actualBottomDitchPolderSideData = (ChartPointData) chartDataList[updatedBottomDitchPolderSideIndex];
                var actualDikeToeAtPolderData = (ChartPointData) chartDataList[updatedDikeToeAtPolderIndex];
                var actualDikeToeAtRiverData = (ChartPointData) chartDataList[updatedDikeToeAtRiverIndex];

                Assert.AreEqual("Ondergrondschematisatie", actualSoilProfileData.Name);
                Assert.AreEqual(surfaceLine.Name, actualSurfaceLineData.Name);
                Assert.AreEqual("Intredepunt", actualEntryPointData.Name);
                Assert.AreEqual("Uittredepunt", actualExitPointData.Name);
                Assert.AreEqual("Insteek sloot dijkzijde", actualDitchDikeSideData.Name);
                Assert.AreEqual("Slootbodem dijkzijde", actualBottomDitchDikeSideData.Name);
                Assert.AreEqual("Insteek sloot polderzijde", actualDitchPolderSideData.Name);
                Assert.AreEqual("Slootbodem polderzijde", actualBottomDitchPolderSideData.Name);
                Assert.AreEqual("Teen dijk binnenwaarts", actualDikeToeAtPolderData.Name);
                Assert.AreEqual("Teen dijk buitenwaarts", actualDikeToeAtRiverData.Name);
            }
        }

        [Test]
        public void UpdateObserver_PreviousCalculationUpdated_ChartDataNotUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            using (var view = new PipingInputView())
            {
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = GetSurfaceLineWithGeometry()
                    }
                };

                view.Data = calculation;

                ChartDataCollection dataBeforeUpdate = view.Chart.Data;
                foreach (ChartData chartData in dataBeforeUpdate.Collection)
                {
                    chartData.Attach(observer);
                }

                view.Data = new PipingCalculationScenario(new GeneralPipingInput());

                calculation.InputParameters.SurfaceLine = GetSecondSurfaceLineWithGeometry();

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
                mocks.VerifyAll(); // No update observer expected
            }
        }

        [Test]
        public void GivenPipingInputViewWithSoilProfileSeries_WhenSurfaceLineSetToNull_ThenCollectionOfEmptyChartDataSetForSoilProfiles()
        {
            // Given
            using (var view = new PipingInputView())
            {
                PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                PipingStochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile();
                var calculation = new PipingCalculationScenario(new GeneralPipingInput())
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
                Assert.AreEqual(10, chartData.Collection.Count());
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), true);

                // When
                calculation.InputParameters.SurfaceLine = null;
                calculation.InputParameters.NotifyObservers();

                // Then
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), false);
            }
        }

        private static PipingStochasticSoilProfile GetStochasticSoilProfile()
        {
            return new PipingStochasticSoilProfile(0.5,
                                                   new PipingSoilProfile("profile", -1, new[]
                                                   {
                                                       new PipingSoilLayer(1)
                                                       {
                                                           MaterialName = "Sand"
                                                       },
                                                       new PipingSoilLayer(3)
                                                       {
                                                           MaterialName = "Clay"
                                                       },
                                                       new PipingSoilLayer(5)
                                                       {
                                                           MaterialName = "Stone"
                                                       }
                                                   }, SoilProfileType.SoilProfile1D));
        }

        private static PipingSurfaceLine GetSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            return GetSurfaceLine(points);
        }

        private static PipingSurfaceLine GetSecondSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(3.5, 2.3, 8.0),
                new Point3D(6.9, 2.0, 2.0)
            };

            return GetSurfaceLine(points);
        }

        private static PipingSurfaceLine GetSurfaceLine(Point3D[] points)
        {
            var surfaceLine = new PipingSurfaceLine("Surface line name");
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }

        private static void AssertEmptySoilLayerTable(PipingInputView view)
        {
            var tableControl = view.Controls.Find("pipingSoilLayerTable", true).First() as PipingSoilLayerTable;

            // Precondition
            Assert.NotNull(tableControl);
            CollectionAssert.IsEmpty(tableControl.Rows);
        }

        private static void AssertEmptyChartData(ChartDataCollection chartDataCollection)
        {
            Assert.AreEqual("Invoer", chartDataCollection.Name);

            List<ChartData> chartDatasList = chartDataCollection.Collection.ToList();

            Assert.AreEqual(10, chartDatasList.Count);

            var soilProfileData = (ChartDataCollection) chartDatasList[soilProfileIndex];
            var surfaceLineData = (ChartLineData) chartDatasList[surfaceLineIndex];
            var entryPointData = (ChartPointData) chartDatasList[entryPointIndex];
            var exitPointData = (ChartPointData) chartDatasList[exitPointIndex];
            var ditchDikeSideData = (ChartPointData) chartDatasList[ditchDikeSideIndex];
            var bottomDitchDikeSideData = (ChartPointData) chartDatasList[bottomDitchDikeSideIndex];
            var ditchPolderSideData = (ChartPointData) chartDatasList[ditchPolderSideIndex];
            var bottomDitchPolderSideData = (ChartPointData) chartDatasList[bottomDitchPolderSideIndex];
            var dikeToeAtPolderData = (ChartPointData) chartDatasList[dikeToeAtPolderIndex];
            var dikeToeAtRiverData = (ChartPointData) chartDatasList[dikeToeAtRiverIndex];

            CollectionAssert.IsEmpty(soilProfileData.Collection);
            CollectionAssert.IsEmpty(surfaceLineData.Points);
            CollectionAssert.IsEmpty(entryPointData.Points);
            CollectionAssert.IsEmpty(exitPointData.Points);
            CollectionAssert.IsEmpty(ditchDikeSideData.Points);
            CollectionAssert.IsEmpty(bottomDitchDikeSideData.Points);
            CollectionAssert.IsEmpty(ditchPolderSideData.Points);
            CollectionAssert.IsEmpty(bottomDitchPolderSideData.Points);
            CollectionAssert.IsEmpty(dikeToeAtPolderData.Points);
            CollectionAssert.IsEmpty(dikeToeAtRiverData.Points);

            Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
            Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
            Assert.AreEqual("Intredepunt", entryPointData.Name);
            Assert.AreEqual("Uittredepunt", exitPointData.Name);
            Assert.AreEqual("Insteek sloot dijkzijde", ditchDikeSideData.Name);
            Assert.AreEqual("Slootbodem dijkzijde", bottomDitchDikeSideData.Name);
            Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);
            Assert.AreEqual("Slootbodem polderzijde", bottomDitchPolderSideData.Name);
            Assert.AreEqual("Teen dijk binnenwaarts", dikeToeAtPolderData.Name);
            Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);
        }

        private static void AssertSoilProfileChartData(PipingStochasticSoilProfile soilProfile, ChartData chartData, bool mapDataShouldContainAreas)
        {
            Assert.IsInstanceOf<ChartDataCollection>(chartData);
            var soilProfileChartData = (ChartDataCollection) chartData;

            int expectedLayerCount = soilProfile.SoilProfile.Layers.Count();
            Assert.AreEqual(expectedLayerCount, soilProfileChartData.Collection.Count());
            Assert.AreEqual(soilProfile.SoilProfile.Name, soilProfileChartData.Name);

            for (var i = 0; i < expectedLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(soilProfile.SoilProfile.Layers.Reverse().ElementAt(i).MaterialName, chartMultipleAreaData.Name);
                Assert.AreEqual(mapDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }
        }

        private static void AssertSurfaceLineChartData(PipingSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var surfaceLineChartData = (ChartLineData) chartData;

            Assert.AreEqual(surfaceLine.Points.Count(), surfaceLineChartData.Points.Count());
            CollectionAssert.AreEqual(surfaceLine.LocalGeometry, surfaceLineChartData.Points);
            Assert.AreEqual(surfaceLine.Name, chartData.Name);
        }

        private static void AssertEntryPointLPointchartData(PipingInput pipingInput, PipingSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartPointData>(chartData);
            var entryPointChartData = (ChartPointData) chartData;

            Assert.AreEqual(1, entryPointChartData.Points.Count());
            var entryPoint = new Point2D(pipingInput.EntryPointL, surfaceLine.GetZAtL(pipingInput.EntryPointL));
            CollectionAssert.AreEqual(new[]
            {
                entryPoint
            }, entryPointChartData.Points);
            Assert.AreEqual("Intredepunt", entryPointChartData.Name);
        }

        private static void AssertExitPointLPointchartData(PipingInput pipingInput, PipingSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartPointData>(chartData);
            var exitPointChartData = (ChartPointData) chartData;

            Assert.AreEqual(1, exitPointChartData.Points.Count());
            var exitPoint = new Point2D(pipingInput.ExitPointL, surfaceLine.GetZAtL(pipingInput.ExitPointL));
            CollectionAssert.AreEqual(new[]
            {
                exitPoint
            }, exitPointChartData.Points);
            Assert.AreEqual("Uittredepunt", exitPointChartData.Name);
        }

        private static void AssertCharacteristicPoints(PipingSurfaceLine surfaceLine, IEnumerable<ChartData> characteristicPoints)
        {
            Point3D first = surfaceLine.Points.First();
            Point3D last = surfaceLine.Points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);

            var ditchDikeSideData = (ChartPointData) characteristicPoints.ElementAt(ditchDikeSideIndex);
            Assert.AreEqual(1, ditchDikeSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DitchDikeSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, ditchDikeSideData.Points);
            Assert.AreEqual("Insteek sloot dijkzijde", ditchDikeSideData.Name);

            var bottomDitchDikeSideData = (ChartPointData) characteristicPoints.ElementAt(bottomDitchDikeSideIndex);
            Assert.AreEqual(1, bottomDitchDikeSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.BottomDitchDikeSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, bottomDitchDikeSideData.Points);
            Assert.AreEqual("Slootbodem dijkzijde", bottomDitchDikeSideData.Name);

            var ditchPolderSideData = (ChartPointData) characteristicPoints.ElementAt(ditchPolderSideIndex);
            Assert.AreEqual(1, ditchPolderSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DitchPolderSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, ditchPolderSideData.Points);
            Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);

            var bottomDitchPolderSideData = (ChartPointData) characteristicPoints.ElementAt(bottomDitchPolderSideIndex);
            Assert.AreEqual(1, bottomDitchPolderSideData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.BottomDitchPolderSide.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, bottomDitchPolderSideData.Points);
            Assert.AreEqual("Slootbodem polderzijde", bottomDitchPolderSideData.Name);

            var dikeToeAtPolderData = (ChartPointData) characteristicPoints.ElementAt(dikeToeAtPolderIndex);
            Assert.AreEqual(1, dikeToeAtPolderData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DikeToeAtPolder.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, dikeToeAtPolderData.Points);
            Assert.AreEqual("Teen dijk binnenwaarts", dikeToeAtPolderData.Name);

            var dikeToeAtRiverData = (ChartPointData) characteristicPoints.ElementAt(dikeToeAtRiverIndex);
            Assert.AreEqual(1, dikeToeAtRiverData.Points.Count());
            CollectionAssert.AreEqual(new[]
            {
                surfaceLine.DikeToeAtRiver.ProjectIntoLocalCoordinates(firstPoint, lastPoint)
            }, dikeToeAtRiverData.Points);
            Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);
        }
    }
}