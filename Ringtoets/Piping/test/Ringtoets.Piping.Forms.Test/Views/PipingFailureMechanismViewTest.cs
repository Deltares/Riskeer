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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.BruTile.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Util.TypeConverters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int stochasticSoilModelsIndex = 1;
        private const int surfaceLinesIndex = 2;
        private const int sectionsIndex = 3;
        private const int sectionsStartPointIndex = 4;
        private const int sectionsEndPointIndex = 5;
        private const int hydraulicBoundaryLocationsIndex = 6;
        private const int calculationsIndex = 7;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new PipingFailureMechanismView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IMapView>(view);
                Assert.IsNotNull(view.Map);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddEmptyMapControl()
        {
            // Call
            using (var view = new PipingFailureMechanismView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.IsInstanceOf<RingtoetsMapControl>(view.Controls[0]);
                Assert.AreSame(view.Map, ((RingtoetsMapControl) view.Controls[0]).MapControl);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_PipingFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new PipingFailureMechanismContext(
                    new PipingFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanPipingFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new PipingFailureMechanismContext(
                    new PipingFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(8, view.Map.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_EmptyPipingFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new PipingFailureMechanismView())
            {
                var failureMechanismContext = new PipingFailureMechanismContext(
                    new PipingFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new PipingFailureMechanismView())
            {
                var failureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_SetToNull_ClearMapDataProperties()
        {
            // Setup
            WmtsMapData backgroundMapData = WmtsMapDataTestHelper.CreateDefaultPdokMapData();
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                BackgroundData = BackgroundDataConverter.ConvertTo(backgroundMapData)
            };

            using (new UseCustomTileSourceFactoryConfig(backgroundMapData))
            using (var view = new PipingFailureMechanismView())
            {
                view.Data = new PipingFailureMechanismContext(
                    new PipingFailureMechanism(), assessmentSection);

                // Precondition
                Assert.IsNotNull(view.Map.Data);
                Assert.IsNotNull(view.Map.BackgroundMapData);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Map.Data);
                Assert.IsNull(view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_PipingFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var geometryPoints = new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(2.0, 0.0),
                    new Point2D(4.0, 4.0),
                    new Point2D(6.0, 4.0)
                };

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
                    }
                };

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(0.0, 3.0),
                    new Point2D(3.0, 0.0)
                });

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                PipingStochasticSoilModel stochasticSoilModel1 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name1", new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(1.1, 2.2)
                });

                PipingStochasticSoilModel stochasticSoilModel2 = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("name2", new[]
                {
                    new Point2D(3.0, 4.0),
                    new Point2D(3.3, 4.4)
                });

                var surfaceLineA = new PipingSurfaceLine("Line A");
                surfaceLineA.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(3.0, 0.0, 1.7)
                });

                var surfaceLineB = new PipingSurfaceLine("Name B");
                surfaceLineB.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.5),
                    new Point3D(3.0, 0.0, 1.8)
                });
                surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
                surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

                var failureMechanism = new PipingFailureMechanism();
                const string arbitraryFilePath = "path";
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    surfaceLineA,
                    surfaceLineB
                }, arbitraryFilePath);
                failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));
                failureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    stochasticSoilModel1,
                    stochasticSoilModel2
                }, arbitraryFilePath);

                PipingCalculationScenario calculationA = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculationA.InputParameters.SurfaceLine = surfaceLineA;
                PipingCalculationScenario calculationB = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculationB.InputParameters.SurfaceLine = surfaceLineB;
                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(8, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, mapDataList[surfaceLinesIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapDataList[hydraulicBoundaryLocationsIndex]);
                AssertStochasticSoilModelsMapData(failureMechanism.StochasticSoilModels, mapDataList[stochasticSoilModelsIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<PipingCalculationScenario>(), mapDataList[calculationsIndex]);
            }
        }

        [Test]
        public void GivenViewWithAssessmentSectionData_WhenAssessmentSectionUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
                    }
                };
                var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0),
                        new HydraulicBoundaryLocation(3, "test3", 2.3, 4.6)
                    }
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase1
                };

                view.Data = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[stochasticSoilModelsIndex].Expect(obs => obs.UpdateObserver());
                observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase1.Locations, hydraulicBoundaryLocationsMapData);

                // When
                assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase2;
                assessmentSection.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase2.Locations, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryDatabaseData_WhenHydraulicBoundaryDatabaseUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
                    }
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
                };

                view.Data = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);

                // When
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));
                hydraulicBoundaryDatabase.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryDatabase_WhenNewDatabaseIsSetAndNotified_ThenMapDataUpdatedAndObserversNotified()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var currentHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "old 1", 1, 2)
                    }
                };
                var newHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "new 1", 1, 2)
                    }
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = currentHydraulicBoundaryDatabase
                };

                view.Data = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[stochasticSoilModelsIndex].Expect(obs => obs.UpdateObserver());
                observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver()).Repeat.Twice();
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(currentHydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);

                // When
                assessmentSection.HydraulicBoundaryDatabase = newHydraulicBoundaryDatabase;
                assessmentSection.NotifyObservers();
                newHydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "new 2", 2, 3));
                newHydraulicBoundaryDatabase.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(newHydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithReferenceLineData_WhenReferenceLineUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var points1 = new List<Point2D>
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                };

                var points2 = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    ReferenceLine = new ReferenceLine()
                };
                assessmentSection.ReferenceLine.SetGeometry(points1);

                view.Data = new PipingFailureMechanismContext(new PipingFailureMechanism(), assessmentSection);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[stochasticSoilModelsIndex].Expect(obs => obs.UpdateObserver());
                observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                MapData referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // When
                assessmentSection.ReferenceLine.SetGeometry(points2);
                assessmentSection.NotifyObservers();

                // Then
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithSurfaceLinesData_WhenSurfaceLinesUpdatedAndNotified_ThenMapDataUpdatedAndObserverNotified()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;
                var failureMechanism = new PipingFailureMechanism();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                var surfaceLine = new PipingSurfaceLine(string.Empty);

                var geometry1 = new Collection<Point3D>
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                };

                surfaceLine.SetGeometry(geometry1);

                view.Data = failureMechanismContext;

                var surfaceLineMapData = (MapLineData) map.Data.Collection.ElementAt(surfaceLinesIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    surfaceLine
                }, "path");
                failureMechanism.SurfaceLines.NotifyObservers();

                // Then
                AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, surfaceLineMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithSurfaceLineData_WhenSurfaceLineUpdatedAndNotified_ThenMapDataUpdatedAndObserverNotified()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;
                var surfaceLine = new PipingSurfaceLine(string.Empty);
                var failureMechanism = new PipingFailureMechanism();
                failureMechanism.SurfaceLines.AddRange(new[]
                {
                    surfaceLine
                }, "path");
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                view.Data = failureMechanismContext;

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(7, 8, 9),
                    new Point3D(10, 11, 12)
                });
                surfaceLine.NotifyObservers();

                // Then
                var surfaceLineMapData = (MapLineData) map.Data.Collection.ElementAt(surfaceLinesIndex);
                AssertSurfaceLinesMapData(failureMechanism.SurfaceLines, surfaceLineMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new PipingFailureMechanism();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                view.Data = failureMechanismContext;

                var sectionMapData = (MapLineData) map.Data.Collection.ElementAt(sectionsIndex);
                var sectionStartsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsStartPointIndex);
                var sectionsEndsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsEndPointIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[stochasticSoilModelsIndex].Expect(obs => obs.UpdateObserver());
                observers[surfaceLinesIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                }));
                failureMechanism.NotifyObservers();

                // Then
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionMapData);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionStartsMapData);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsEndsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithStochasticSoilModels_WhenStochasticSoilModelsUpdatedAndNotified_ThenMapDataUpdatedAndObserverNotified()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new PipingFailureMechanism();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("", new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                });

                view.Data = failureMechanismContext;

                var stochasticSoilModelMapData = (MapLineData) map.Data.Collection.ElementAt(stochasticSoilModelsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[stochasticSoilModelsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                failureMechanism.StochasticSoilModels.AddRange(new[]
                {
                    stochasticSoilModel
                }, "path");
                failureMechanism.StochasticSoilModels.NotifyObservers();

                // Then
                AssertStochasticSoilModelsMapData(failureMechanism.StochasticSoilModels, stochasticSoilModelMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new PipingFailureMechanism();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var surfaceLineA = new PipingSurfaceLine(string.Empty);
                surfaceLineA.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(3.0, 0.0, 1.7)
                });

                var surfaceLineB = new PipingSurfaceLine(string.Empty);
                surfaceLineB.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.5),
                    new Point3D(3.0, 0.0, 1.8)
                });
                surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
                surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

                PipingCalculationScenario calculationA = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculationA.InputParameters.SurfaceLine = surfaceLineA;
                PipingCalculationScenario calculationB = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculationB.InputParameters.SurfaceLine = surfaceLineB;
                failureMechanism.CalculationsGroup.Children.Add(calculationA);

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                failureMechanism.CalculationsGroup.Children.Add(calculationB);
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<PipingCalculationScenario>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var surfaceLineA = new PipingSurfaceLine(string.Empty);
                surfaceLineA.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(3.0, 0.0, 1.7)
                });

                var surfaceLineB = new PipingSurfaceLine(string.Empty);
                surfaceLineB.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.5),
                    new Point3D(3.0, 0.0, 1.8)
                });
                surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
                surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

                PipingCalculationScenario calculationA = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculationA.InputParameters.SurfaceLine = surfaceLineA;

                var failureMechanism = new PipingFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                calculationA.InputParameters.SurfaceLine = surfaceLineB;
                calculationA.InputParameters.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<PipingCalculationScenario>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var surfaceLineA = new PipingSurfaceLine(string.Empty);
                surfaceLineA.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.0),
                    new Point3D(3.0, 0.0, 1.7)
                });

                var surfaceLineB = new PipingSurfaceLine(string.Empty);
                surfaceLineB.SetGeometry(new[]
                {
                    new Point3D(0.0, 0.0, 1.5),
                    new Point3D(3.0, 0.0, 1.8)
                });
                surfaceLineA.ReferenceLineIntersectionWorldPoint = new Point2D(1.3, 1.3);
                surfaceLineB.ReferenceLineIntersectionWorldPoint = new Point2D(1.5, 1.5);

                PipingCalculationScenario calculationA = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
                calculationA.InputParameters.SurfaceLine = surfaceLineA;

                var failureMechanism = new PipingFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                calculationA.Name = "new name";
                calculationA.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<PipingCalculationScenario>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 7;
            const int updatedSurfaceLineLayerIndex = surfaceLinesIndex - 1;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedStochasticSoilModelsLayerIndex = stochasticSoilModelsIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var failureMechanism = new PipingFailureMechanism();
                var failureMechanismContext = new PipingFailureMechanismContext(failureMechanism, assessmentSection);

                view.Data = failureMechanismContext;

                MapDataCollection mapData = map.Data;

                var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                List<MapData> mapDataList = mapData.Collection.ToList();

                // Precondition
                var referenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", referenceLineData.Name);

                var surfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
                Assert.AreEqual("Profielschematisaties", surfaceLineData.Name);

                var sectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", sectionsData.Name);

                var sectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", sectionStartsData.Name);

                var sectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", sectionEndsData.Name);

                var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicLocationsData.Name);

                var stochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
                Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsData.Name);

                var calculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", calculationsData.Name);

                var points = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };
                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(points);
                assessmentSection.ReferenceLine = referenceLine;

                // Call
                assessmentSection.NotifyObservers();

                // Assert
                var actualReferenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", actualReferenceLineData.Name);

                var actualSurfaceLineData = (MapLineData) mapDataList[updatedSurfaceLineLayerIndex];
                Assert.AreEqual("Profielschematisaties", actualSurfaceLineData.Name);

                var actualSectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", actualSectionsData.Name);

                var actualSectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", actualSectionStartsData.Name);

                var actualSectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", actualSectionEndsData.Name);

                var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHydraulicLocationsData.Name);

                var actualStochasticSoilModelsData = (MapLineData) mapDataList[updatedStochasticSoilModelsLayerIndex];
                Assert.AreEqual("Stochastische ondergrondmodellen", actualStochasticSoilModelsData.Name);

                var actualCalculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            IAssessmentSection oldAssessmentSection = new ObservableTestAssessmentSectionStub();
            IAssessmentSection newAssessmentSection = new ObservableTestAssessmentSectionStub();

            newAssessmentSection.ReferenceLine = new ReferenceLine();
            newAssessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(2, 4),
                new Point2D(3, 4)
            });

            var oldPipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), oldAssessmentSection);
            var newPipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), newAssessmentSection);
            using (var view = new PipingFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                view.Data = oldPipingFailureMechanismContext;
                view.Data = newPipingFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertSurfaceLinesMapData(IEnumerable<PipingSurfaceLine> surfaceLines, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var surfaceLinesMapData = (MapLineData) mapData;
            MapFeature[] surfaceLineFeatures = surfaceLinesMapData.Features.ToArray();
            PipingSurfaceLine[] surfaceLinesArray = surfaceLines.ToArray();
            Assert.AreEqual(surfaceLinesArray.Length, surfaceLineFeatures.Length);

            for (var index = 0; index < surfaceLinesArray.Length; index++)
            {
                Assert.AreEqual(1, surfaceLineFeatures[index].MapGeometries.Count());
                PipingSurfaceLine surfaceLine = surfaceLinesArray[index];
                CollectionAssert.AreEquivalent(surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)), surfaceLineFeatures[index].MapGeometries.First().PointCollections.First());
            }
            Assert.AreEqual("Profielschematisaties", mapData.Name);
        }

        private static void AssertStochasticSoilModelsMapData(IEnumerable<PipingStochasticSoilModel> soilModels, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var soilModelsMapData = (MapLineData) mapData;
            MapFeature[] soilModelsFeatures = soilModelsMapData.Features.ToArray();
            PipingStochasticSoilModel[] stochasticSoilModelsArray = soilModels.ToArray();
            Assert.AreEqual(stochasticSoilModelsArray.Length, soilModelsFeatures.Length);

            for (var index = 0; index < stochasticSoilModelsArray.Length; index++)
            {
                Assert.AreEqual(1, soilModelsFeatures[index].MapGeometries.Count());
                PipingStochasticSoilModel stochasticSoilModel = stochasticSoilModelsArray[index];
                CollectionAssert.AreEquivalent(stochasticSoilModel.Geometry.Select(p => new Point2D(p)), soilModelsFeatures[index].MapGeometries.First().PointCollections.First());
            }
            Assert.AreEqual("Stochastische ondergrondmodellen", mapData.Name);
        }

        private static void AssertCalculationsMapData(IEnumerable<PipingCalculationScenario> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            PipingCalculationScenario[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (var index = 0; index < calculationsArray.Length; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                PipingCalculationScenario calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                                               {
                                                   calculation.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint,
                                                   calculation.InputParameters.HydraulicBoundaryLocation.Location
                                               },
                                               geometries[0].PointCollections.First());
            }
            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Piping", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(8, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var stochasticSoilModelsMapData = (MapLineData) mapDataList[stochasticSoilModelsIndex];
            var surfaceLinesMapData = (MapLineData) mapDataList[surfaceLinesIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(stochasticSoilModelsMapData.Features);
            CollectionAssert.IsEmpty(surfaceLinesMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Stochastische ondergrondmodellen", stochasticSoilModelsMapData.Name);
            Assert.AreEqual("Profielschematisaties", surfaceLinesMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Berekeningen", calculationsMapData.Name);
        }

        /// <summary>
        /// Attaches mocked observers to all <see cref="IObservable"/> map data components.
        /// </summary>
        /// <param name="mocks">The <see cref="MockRepository"/>.</param>
        /// <param name="mapData">The map data collection containing the <see cref="IObservable"/>
        /// elements.</param>
        /// <returns>An array of mocked observers attached to the data in <paramref name="mapData"/>.</returns>
        private static IObserver[] AttachMapDataObservers(MockRepository mocks, IEnumerable<MapData> mapData)
        {
            MapData[] mapDataArray = mapData.ToArray();

            var referenceLineMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[referenceLineIndex].Attach(referenceLineMapDataObserver);

            var stochasticSoilModelMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[stochasticSoilModelsIndex].Attach(stochasticSoilModelMapDataObserver);

            var surfaceLineMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[surfaceLinesIndex].Attach(surfaceLineMapDataObserver);

            var sectionsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsIndex].Attach(sectionsMapDataObserver);

            var sectionsStartPointMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsStartPointIndex].Attach(sectionsStartPointMapDataObserver);

            var sectionsEndPointMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsEndPointIndex].Attach(sectionsEndPointMapDataObserver);

            var hydraulicBoundaryLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[hydraulicBoundaryLocationsIndex].Attach(hydraulicBoundaryLocationsMapDataObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                stochasticSoilModelMapDataObserver,
                surfaceLineMapDataObserver,
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver,
                hydraulicBoundaryLocationsMapDataObserver,
                calculationsMapDataObserver
            };
        }
    }
}