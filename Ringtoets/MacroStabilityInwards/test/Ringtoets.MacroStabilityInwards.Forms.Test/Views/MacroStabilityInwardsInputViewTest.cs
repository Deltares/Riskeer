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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsInputViewTest
    {
        private const int soilProfileIndex = 0;
        private const int surfaceLineIndex = 1;
        private const int surfaceLevelOutsideIndex = 2;
        private const int dikeToeAtRiverIndex = 3;
        private const int trafficLoadOutsideIndex = 4;
        private const int trafficLoadInsideIndex = 5;
        private const int dikeTopAtPolderIndex = 6;
        private const int shoulderBaseInsideIndex = 7;
        private const int shoulderTopInsideIndex = 2;
        private const int dikeToeAtPolderIndex = 2;
        private const int ditchDikeSideIndex = 2;
        private const int bottomDitchDikeSideIndex = 2;
        private const int bottomDitchPolderSideIndex = 2;
        private const int ditchPolderSideIndex = 8;
        private const int surfaceLevelInsideIndex = 2;
        private const int nrOfChartData = 9;

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
                MacroStabilityInwardsSoilLayerTable tableControl = GetSoilLayerTable(view);
                Assert.NotNull(tableControl);
                Assert.AreEqual(DockStyle.Bottom, tableControl.Dock);
                CollectionAssert.IsEmpty(tableControl.Rows);
            }
        }

        [Test]
        public void Data_CalculationScenario_DataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanCalculationScenario_DataNull()
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
                Data = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    StochasticSoilProfile = new StochasticSoilProfile(0.1, SoilProfileType.SoilProfile1D, 1)
                    {
                        SoilProfile = new MacroStabilityInwardsSoilProfile(
                            "profile",
                            -1,
                            new[]
                            {
                                new MacroStabilityInwardsSoilLayer(3.0),
                                new MacroStabilityInwardsSoilLayer(2.0),
                                new MacroStabilityInwardsSoilLayer(0)
                            },
                            SoilProfileType.SoilProfile1D,
                            1)
                    }
                }
            };

            using (var view = new MacroStabilityInwardsInputView
            {
                Data = calculation
            })
            {
                var tableControl = view.Controls.Find("soilLayerTable", true).First() as MacroStabilityInwardsSoilLayerTable;

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
            using (var view = new MacroStabilityInwardsInputView())
            {
                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                StochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile();
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
                AssertSoilProfileChartData(stochasticSoilProfile, chartData.Collection.ElementAt(soilProfileIndex), true);
                AssertSurfaceLineChartData(surfaceLine, chartData.Collection.ElementAt(surfaceLineIndex));
            }
        }

        [Test]
        public void Data_WithoutSurfaceLine_NoChartDataSet()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

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
            using (var view = new MacroStabilityInwardsInputView())
            {
                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
                Assert.AreEqual(nrOfChartData, chartData.Collection.Count());
                var soilProfileData = (ChartDataCollection) chartData.Collection.ElementAt(soilProfileIndex);
                CollectionAssert.IsEmpty(soilProfileData.Collection);
                Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
            }
        }

        [Test]
        public void Data_WithSurfaceLineWithoutSoilProfile_CollectionOfEmptyChartDataSetForSoilProfile()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine,
                        StochasticSoilProfile = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
                    }
                };

                // Call
                view.Data = calculation;

                // Assert
                Assert.AreSame(calculation, view.Data);
                ChartDataCollection chartData = view.Chart.Data;
                Assert.IsInstanceOf<ChartDataCollection>(chartData);
                Assert.AreEqual(nrOfChartData, chartData.Collection.Count());
                var soilProfileData = (ChartDataCollection) chartData.Collection.ElementAt(soilProfileIndex);
                CollectionAssert.IsEmpty(soilProfileData.Collection);
                Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
            }
        }

        [Test]
        public void Data_WithoutStochasticSoilProfile_SoilLayerTableEmpty()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

                // Call
                view.Data = calculation;

                // Assert
                AssertEmptySoilLayerTable(view);
            }
        }

        [Test]
        public void Data_WithoutSoilProfile_SoilLayerTableEmpty()
        {
            // Setup
            using (var view = new MacroStabilityInwardsInputView())
            {
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                {
                    InputParameters =
                    {
                        StochasticSoilProfile = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
                    }
                };

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

                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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

                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
                {
                    Name = initialName
                };

                view.Data = calculation;

                // Precondition
                Assert.AreEqual(initialName, view.Chart.ChartTitle);

                var calculation2 = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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

                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine2 = GetSecondSurfaceLineWithGeometry();

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
        public void UpdateObserver_StochasticSoilProfileUpdated_ChartDataUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            using (var view = new MacroStabilityInwardsInputView())
            {
                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                StochasticSoilProfile soilProfile = GetStochasticSoilProfile();
                var soilProfile2 = new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
                {
                    SoilProfile = new MacroStabilityInwardsSoilProfile("profile", -2, new[]
                    {
                        new MacroStabilityInwardsSoilLayer(0),
                        new MacroStabilityInwardsSoilLayer(2),
                        new MacroStabilityInwardsSoilLayer(3)
                    }, SoilProfileType.SoilProfile1D, 1)
                };

                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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
            const int updatedSoilProfileIndex = soilProfileIndex + nrOfChartData - 1;
            const int updatedSurfaceLineIndex = surfaceLineIndex - 1;
            const int updatedSurfaceLevelOutsideIndex = surfaceLevelOutsideIndex - 1;
            const int updatedDikeToeAtRiverIndex = dikeToeAtRiverIndex - 1;
            const int updatedTrafficLoadOutsideIndex = trafficLoadOutsideIndex - 1;
            const int updatedTrafficLoadInsideIndex = trafficLoadInsideIndex - 1;
            const int updatedDikeTopAtPolderIndex = dikeTopAtPolderIndex - 1;
            const int updatedShoulderBaseInsideIndex = shoulderBaseInsideIndex - 1;
            const int updatedDitchPolderSideIndex = ditchPolderSideIndex - 1;

            var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

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
                var surfaceLevelOutsideData = (ChartPointData) chartDataList[updatedSurfaceLevelOutsideIndex];
                var dikeToeAtRiverData = (ChartPointData) chartDataList[updatedDikeToeAtRiverIndex];
                var trafficLoadOutsideData = (ChartPointData) chartDataList[updatedTrafficLoadOutsideIndex];
                var trafficLoadInsideData = (ChartPointData) chartDataList[updatedTrafficLoadInsideIndex];
                var dikeTopAtPolderData = (ChartPointData) chartDataList[updatedDikeTopAtPolderIndex];
                var shoulderBaseInsideData = (ChartPointData) chartDataList[updatedShoulderBaseInsideIndex];
                var ditchPolderSideData = (ChartPointData) chartDataList[updatedDitchPolderSideIndex];

                Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
                Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
                Assert.AreEqual("Maaiveld buitenwaarts", surfaceLevelOutsideData.Name);
                Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);
                Assert.AreEqual("Verkeersbelasting kant buitenwaarts", trafficLoadOutsideData.Name);
                Assert.AreEqual("Verkeersbelasting kant binnenwaarts", trafficLoadInsideData.Name);
                Assert.AreEqual("Kruin binnentalud", dikeTopAtPolderData.Name);
                Assert.AreEqual("Insteek binnenberm", shoulderBaseInsideData.Name);
                Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);

                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                calculation.InputParameters.SurfaceLine = surfaceLine;

                // Call
                calculation.InputParameters.NotifyObservers();

                // Assert
                chartDataList = chartData.Collection.ToList();

                var actualSoilProfileData = (ChartDataCollection) chartDataList[updatedSoilProfileIndex];
                var actualSurfaceLineData = (ChartLineData) chartDataList[updatedSurfaceLineIndex];
                var actualSurfaceLevelOutsideData = (ChartPointData) chartDataList[updatedSurfaceLevelOutsideIndex];
                var actualDikeToeAtRiverData = (ChartPointData) chartDataList[updatedDikeToeAtRiverIndex];
                var actualTrafficLoadOutsideData = (ChartPointData) chartDataList[updatedTrafficLoadOutsideIndex];
                var actualTrafficLoadInsideData = (ChartPointData) chartDataList[updatedTrafficLoadInsideIndex];
                var actualDikeTopAtPolderData = (ChartPointData) chartDataList[updatedDikeTopAtPolderIndex];
                var actualShoulderBaseInsideData = (ChartPointData) chartDataList[updatedShoulderBaseInsideIndex];
                var actualDitchPolderSideData = (ChartPointData) chartDataList[updatedDitchPolderSideIndex];

                Assert.AreEqual("Ondergrondschematisatie", actualSoilProfileData.Name);
                Assert.AreEqual(surfaceLine.Name, actualSurfaceLineData.Name);
                Assert.AreEqual("Maaiveld buitenwaarts", actualSurfaceLevelOutsideData.Name);
                Assert.AreEqual("Teen dijk buitenwaarts", actualDikeToeAtRiverData.Name);
                Assert.AreEqual("Verkeersbelasting kant buitenwaarts", actualTrafficLoadOutsideData.Name);
                Assert.AreEqual("Verkeersbelasting kant binnenwaarts", actualTrafficLoadInsideData.Name);
                Assert.AreEqual("Kruin binnentalud", actualDikeTopAtPolderData.Name);
                Assert.AreEqual("Insteek binnenberm", actualShoulderBaseInsideData.Name);
                Assert.AreEqual("Insteek sloot polderzijde", actualDitchPolderSideData.Name);
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
                var calculation1 = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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

                view.Data = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput());

                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine2 = GetSecondSurfaceLineWithGeometry();

                calculation1.InputParameters.SurfaceLine = surfaceLine2;

                // Call
                calculation1.InputParameters.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, view.Chart.Data);
                mocks.VerifyAll(); // no update observer expected
            }
        }

        [Test]
        public void GivenInputViewWithSoilProfileSeries_WhenSurfaceLineSetToNull_ThenCollectionOfEmptyChartDataSetForSoilProfiles()
        {
            // Given
            using (var view = new MacroStabilityInwardsInputView())
            {
                RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();
                StochasticSoilProfile stochasticSoilProfile = GetStochasticSoilProfile();
                var calculation = new MacroStabilityInwardsCalculationScenario(new GeneralMacroStabilityInwardsInput())
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

        private static IChartControl GetChartControl(MacroStabilityInwardsInputView view)
        {
            return ControlTestHelper.GetControls<IChartControl>(view, "chartControl").Single();
        }

        private static MacroStabilityInwardsSoilLayerTable GetSoilLayerTable(MacroStabilityInwardsInputView view)
        {
            return ControlTestHelper.GetControls<MacroStabilityInwardsSoilLayerTable>(view, "soilLayerTable").Single();
        }

        private static StochasticSoilProfile GetStochasticSoilProfile()
        {
            return new StochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 1)
            {
                SoilProfile = new MacroStabilityInwardsSoilProfile("profile", -1, new[]
                {
                    new MacroStabilityInwardsSoilLayer(1),
                    new MacroStabilityInwardsSoilLayer(3),
                    new MacroStabilityInwardsSoilLayer(5)
                }, SoilProfileType.SoilProfile1D, 1)
            };
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine GetSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            return GetSurfaceLine(points);
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine GetSecondSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(3.5, 2.3, 8.0),
                new Point3D(6.9, 2.0, 2.0)
            };

            return GetSurfaceLine(points);
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine GetSurfaceLine(Point3D[] points)
        {
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Surface line name"
            };
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }

        private static void AssertEmptySoilLayerTable(MacroStabilityInwardsInputView view)
        {
            MacroStabilityInwardsSoilLayerTable tableControl = GetSoilLayerTable(view);

            // Precondition
            Assert.NotNull(tableControl);
            CollectionAssert.IsEmpty(tableControl.Rows);
        }

        private static void AssertEmptyChartData(ChartDataCollection chartDataCollection)
        {
            Assert.AreEqual("Invoer", chartDataCollection.Name);

            List<ChartData> chartDatasList = chartDataCollection.Collection.ToList();

            Assert.AreEqual(nrOfChartData, chartDatasList.Count);

            var soilProfileData = (ChartDataCollection) chartDatasList[soilProfileIndex];
            var surfaceLineData = (ChartLineData) chartDatasList[surfaceLineIndex];
            var surfaceLevelOutsideData = (ChartPointData) chartDatasList[surfaceLevelOutsideIndex];
            var dikeToeAtRiverData = (ChartPointData) chartDatasList[dikeToeAtRiverIndex];
            var trafficLoadOutsideData = (ChartPointData) chartDatasList[trafficLoadOutsideIndex];
            var trafficLoadInsideData = (ChartPointData) chartDatasList[trafficLoadInsideIndex];
            var dikeTopAtPolderData = (ChartPointData) chartDatasList[dikeTopAtPolderIndex];
            var shoulderBaseInsideData = (ChartPointData) chartDatasList[shoulderBaseInsideIndex];
            var ditchPolderSideData = (ChartPointData) chartDatasList[ditchPolderSideIndex];

            CollectionAssert.IsEmpty(soilProfileData.Collection);
            CollectionAssert.IsEmpty(surfaceLineData.Points);
            CollectionAssert.IsEmpty(surfaceLevelOutsideData.Points);
            CollectionAssert.IsEmpty(dikeToeAtRiverData.Points);
            CollectionAssert.IsEmpty(trafficLoadOutsideData.Points);
            CollectionAssert.IsEmpty(trafficLoadInsideData.Points);
            CollectionAssert.IsEmpty(dikeTopAtPolderData.Points);
            CollectionAssert.IsEmpty(shoulderBaseInsideData.Points);
            CollectionAssert.IsEmpty(ditchPolderSideData.Points);

            Assert.AreEqual("Ondergrondschematisatie", soilProfileData.Name);
            Assert.AreEqual("Profielschematisatie", surfaceLineData.Name);
            Assert.AreEqual("Maaiveld buitenwaarts", surfaceLevelOutsideData.Name);
            Assert.AreEqual("Teen dijk buitenwaarts", dikeToeAtRiverData.Name);
            Assert.AreEqual("Verkeersbelasting kant buitenwaarts", trafficLoadOutsideData.Name);
            Assert.AreEqual("Verkeersbelasting kant binnenwaarts", trafficLoadInsideData.Name);
            Assert.AreEqual("Kruin binnentalud", dikeTopAtPolderData.Name);
            Assert.AreEqual("Insteek binnenberm", shoulderBaseInsideData.Name);
            Assert.AreEqual("Insteek sloot polderzijde", ditchPolderSideData.Name);
        }

        private static void AssertSoilProfileChartData(StochasticSoilProfile soilProfile, ChartData chartData, bool mapDataShouldContainAreas)
        {
            Assert.IsInstanceOf<ChartDataCollection>(chartData);
            var soilProfileChartData = (ChartDataCollection) chartData;

            int expectedLayerCount = soilProfile.SoilProfile.Layers.Count();
            Assert.AreEqual(expectedLayerCount, soilProfileChartData.Collection.Count());
            Assert.AreEqual(soilProfile.SoilProfile.Name, soilProfileChartData.Name);

            string[] soilLayers = soilProfile.SoilProfile.Layers.Select((l, i) => string.Format("{0} {1}", i + 1, l.MaterialName)).Reverse().ToArray();

            for (var i = 0; i < expectedLayerCount; i++)
            {
                var chartMultipleAreaData = soilProfileChartData.Collection.ElementAt(i) as ChartMultipleAreaData;

                Assert.IsNotNull(chartMultipleAreaData);
                Assert.AreEqual(soilLayers[i], chartMultipleAreaData.Name);
                Assert.AreEqual(mapDataShouldContainAreas, chartMultipleAreaData.Areas.Any());
            }
        }

        private static void AssertSurfaceLineChartData(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, ChartData chartData)
        {
            Assert.IsInstanceOf<ChartLineData>(chartData);
            var surfaceLineChartData = (ChartLineData) chartData;

            Assert.AreEqual(surfaceLine.Points.Length, surfaceLineChartData.Points.Length);
            CollectionAssert.AreEqual(surfaceLine.ProjectGeometryToLZ(), surfaceLineChartData.Points);
            Assert.AreEqual(surfaceLine.Name, chartData.Name);
        }
    }
}