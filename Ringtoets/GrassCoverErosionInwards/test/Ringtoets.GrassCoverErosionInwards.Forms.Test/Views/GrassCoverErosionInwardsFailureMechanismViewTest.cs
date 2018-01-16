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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryLocationsIndex = 4;
        private const int dikeProfilesIndex = 5;
        private const int foreshoreProfilesIndex = 6;
        private const int calculationsIndex = 7;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
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
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
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
        public void Data_GrassCoverErosionInwardsFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanGrassCoverErosionInwardsFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_AssessmentSectionWithBackgroundData_BackgroundDataSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(8, view.Map.Data.Collection.Count());
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
                Assert.IsNull(view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_EmptyGrassCoverErosionInwardsFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_GrassCoverErosionInwardsFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var geometryPoints = new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(2.0, 0.0),
                    new Point2D(4.0, 4.0),
                    new Point2D(6.0, 4.0)
                };

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
                        }
                    },
                    ReferenceLine = referenceLine
                };

                DikeProfile dikeProfileA = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3));
                DikeProfile dikeProfileB = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5));

                var calculationA = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                var calculationB = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileB,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));

                failureMechanism.DikeProfiles.AddRange(new[]
                {
                    DikeProfileTestFactory.CreateDikeProfile(new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(1, 1)
                    }, "id1"),
                    DikeProfileTestFactory.CreateDikeProfile(new[]
                    {
                        new Point2D(2, 2),
                        new Point2D(3, 3)
                    }, "id2")
                }, "path");

                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(8, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations, mapDataList[hydraulicBoundaryLocationsIndex]);
                AssertDikeProfiles(failureMechanism.DikeProfiles, mapDataList[dikeProfilesIndex]);
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), mapDataList[foreshoreProfilesIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), mapDataList[calculationsIndex]);
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenHydraulicBoundaryLocationsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
                        }
                    }
                };

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);

                // When
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 3.0, 4.0));
                assessmentSection.HydraulicBoundaryDatabase.Locations.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithHydraulicBoundaryLocationsData_WhenLocationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            var random = new Random(21);
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0);
                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase =
                    {
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                };

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                MapData hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationOutputsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                                                hydraulicBoundaryLocationsMapData);

                // When
                hydraulicBoundaryLocation.DesignWaterLevelCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.WaveHeightCalculation1.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
                hydraulicBoundaryLocation.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationOutputsMapData(assessmentSection.HydraulicBoundaryDatabase.Locations,
                                                                                hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void GivenViewWithReferenceLineData_WhenReferenceLineUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
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

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                MapData referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                observers[dikeProfilesIndex].Expect(obs => obs.UpdateObserver());
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

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
        public void GivenViewWithFailureMechanismSectionsData_WhenFailureMechanismSectionsUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                view.Data = failureMechanismContext;

                var sectionMapData = (MapLineData) map.Data.Collection.ElementAt(sectionsIndex);
                var sectionStartsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsStartPointIndex);
                var sectionsEndsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsEndPointIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[referenceLineIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsStartPointIndex].Expect(obs => obs.UpdateObserver());
                observers[sectionsEndPointIndex].Expect(obs => obs.UpdateObserver());
                observers[hydraulicBoundaryLocationsIndex].Expect(obs => obs.UpdateObserver());
                observers[dikeProfilesIndex].Expect(obs => obs.UpdateObserver());
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
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
        public void GivenViewWithDikeProfilesData_WhenDikeProfilesUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                failureMechanism.DikeProfiles.AddRange(new[]
                {
                    DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id1")
                }, "path");

                view.Data = failureMechanismContext;

                MapData dikeProfileData = map.Data.Collection.ElementAt(dikeProfilesIndex);
                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[dikeProfilesIndex].Expect(obs => obs.UpdateObserver());
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);

                // When
                failureMechanism.DikeProfiles.AddRange(new[]
                {
                    DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id2")
                }, "path");
                failureMechanism.DikeProfiles.NotifyObservers();

                // Then
                AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithDikeProfileData_WhenDikeProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(string.Empty, "id1");
                failureMechanism.DikeProfiles.AddRange(new[]
                {
                    dikeProfile
                }, "path");

                view.Data = failureMechanismContext;

                MapData dikeProfileData = map.Data.Collection.ElementAt(dikeProfilesIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[dikeProfilesIndex].Expect(obs => obs.UpdateObserver());
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);

                // When
                DikeProfile dikeProfileToUpdateFrom = DikeProfileTestFactory.CreateDikeProfile("A new name", "id1");
                dikeProfile.CopyProperties(dikeProfileToUpdateFrom);
                dikeProfile.NotifyObservers();

                // Then
                AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithForeshoreProfileData_WhenForeshoreProfileUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }, "id1");
                failureMechanism.DikeProfiles.AddRange(new[]
                {
                    dikeProfile
                }, "path");

                view.Data = failureMechanismContext;

                MapData dikeProfileData = map.Data.Collection.ElementAt(foreshoreProfilesIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[dikeProfilesIndex].Expect(obs => obs.UpdateObserver());
                observers[foreshoreProfilesIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // Precondition
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), dikeProfileData);

                // When
                DikeProfile dikeProfileToUpdateFrom = DikeProfileTestFactory.CreateDikeProfile(new[]
                {
                    new Point2D(2, 2),
                    new Point2D(3, 3)
                }, "id1");
                dikeProfile.CopyProperties(dikeProfileToUpdateFrom);
                failureMechanism.DikeProfiles.NotifyObservers();

                // Then
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), dikeProfileData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationGroupData_WhenCalculationGroupUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                DikeProfile dikeProfileA = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3));
                DikeProfile dikeProfileB = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5));

                var calculationA = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                var calculationB = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileB,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
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
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationInputData_WhenCalculationInputUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                DikeProfile dikeProfileA = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3));
                DikeProfile dikeProfileB = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.5, 1.5));

                var calculationA = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                var mocks = new MockRepository();
                IObserver[] observers = AttachMapDataObservers(mocks, map.Data.Collection);
                observers[calculationsIndex].Expect(obs => obs.UpdateObserver());
                mocks.ReplayAll();

                // When
                calculationA.InputParameters.DikeProfile = dikeProfileB;
                calculationA.InputParameters.NotifyObservers();

                // Then
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GivenViewWithCalculationData_WhenCalculationUpdatedAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                DikeProfile dikeProfileA = DikeProfileTestFactory.CreateDikeProfile(new Point2D(1.3, 1.3));

                var calculationA = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());
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
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void NotifyObservers_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 7;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedDikeProfilesLayerIndex = dikeProfilesIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                view.Data = failureMechanismContext;

                MapDataCollection mapData = map.Data;

                var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                List<MapData> mapDataList = mapData.Collection.ToList();

                // Precondition
                var referenceLineData = (MapLineData) mapDataList[updatedRefenceLineLayerIndex];
                Assert.AreEqual("Referentielijn", referenceLineData.Name);

                var sectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", sectionsData.Name);

                var sectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", sectionStartsData.Name);

                var sectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", sectionEndsData.Name);

                var hydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicLocationsData.Name);

                var dikeProfilesData = (MapLineData) mapDataList[updatedDikeProfilesLayerIndex];
                Assert.AreEqual("Dijkprofielen", dikeProfilesData.Name);

                var foreshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

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

                var actualSectionsData = (MapLineData) mapDataList[updatedSectionsLayerIndex];
                Assert.AreEqual("Vakindeling", actualSectionsData.Name);

                var actualSectionStartsData = (MapPointData) mapDataList[updateSectionStartLayerIndex];
                Assert.AreEqual("Vakindeling (startpunten)", actualSectionStartsData.Name);

                var actualSectionEndsData = (MapPointData) mapDataList[updatedSectionEndLayerIndex];
                Assert.AreEqual("Vakindeling (eindpunten)", actualSectionEndsData.Name);

                var actualHydraulicLocationsData = (MapPointData) mapDataList[updatedHydraulicLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHydraulicLocationsData.Name);

                var actualDikeProfilesData = (MapLineData) mapDataList[updatedDikeProfilesLayerIndex];
                Assert.AreEqual("Dijkprofielen", actualDikeProfilesData.Name);

                var actualForeshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

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

            var oldGrassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), oldAssessmentSection);
            var newGrassCoverErosionInwardsFailureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), newAssessmentSection);
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                view.Data = oldGrassCoverErosionInwardsFailureMechanismContext;
                view.Data = newGrassCoverErosionInwardsFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertDikeProfiles(IEnumerable<DikeProfile> dikeProfiles, MapData mapData)
        {
            Assert.NotNull(dikeProfiles, "dikeProfiles should never be null.");

            var dikeProfilesData = (MapLineData) mapData;
            DikeProfile[] dikeProfileArray = dikeProfiles.ToArray();

            Assert.IsInstanceOf<MapLineData>(mapData);
            int dikeProfileCount = dikeProfileArray.Length;
            Assert.AreEqual(dikeProfileCount, dikeProfilesData.Features.Count());

            for (var i = 0; i < dikeProfileCount; i++)
            {
                MapGeometry profileDataA = dikeProfilesData.Features.ElementAt(i).MapGeometries.First();
                CollectionAssert.AreEquivalent(dikeProfileArray[0].DikeGeometry, profileDataA.PointCollections.First());
            }

            Assert.AreEqual("Dijkprofielen", mapData.Name);
        }

        private static void AssertCalculationsMapData(IEnumerable<GrassCoverErosionInwardsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            GrassCoverErosionInwardsCalculation[] calculationsArray = calculations.ToArray();
            MapFeature[] calculationsFeatures = calculationsMapData.Features.ToArray();
            int calculationsCount = calculationsArray.Length;
            Assert.AreEqual(calculationsCount, calculationsFeatures.Length);

            for (var index = 0; index < calculationsCount; index++)
            {
                MapGeometry[] geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                GrassCoverErosionInwardsCalculation calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                                               {
                                                   calculation.InputParameters.DikeProfile.WorldReferencePoint,
                                                   calculation.InputParameters.HydraulicBoundaryLocation.Location
                                               },
                                               geometries[0].PointCollections.First());
            }
            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Grasbekleding erosie kruin en binnentalud", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(8, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var dikeProfilesMapData = (MapLineData) mapDataList[dikeProfilesIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(dikeProfilesMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Dijkprofielen", dikeProfilesMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
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

            var sectionsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsIndex].Attach(sectionsMapDataObserver);

            var sectionsStartPointMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsStartPointIndex].Attach(sectionsStartPointMapDataObserver);

            var sectionsEndPointMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[sectionsEndPointIndex].Attach(sectionsEndPointMapDataObserver);

            var hydraulicBoundaryLocationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[hydraulicBoundaryLocationsIndex].Attach(hydraulicBoundaryLocationsMapDataObserver);

            var dikeProfilesObserver = mocks.StrictMock<IObserver>();
            mapDataArray[dikeProfilesIndex].Attach(dikeProfilesObserver);

            var foreshoreProfilesObserver = mocks.StrictMock<IObserver>();
            mapDataArray[foreshoreProfilesIndex].Attach(foreshoreProfilesObserver);

            var calculationsMapDataObserver = mocks.StrictMock<IObserver>();
            mapDataArray[calculationsIndex].Attach(calculationsMapDataObserver);

            return new[]
            {
                referenceLineMapDataObserver,
                sectionsMapDataObserver,
                sectionsStartPointMapDataObserver,
                sectionsEndPointMapDataObserver,
                hydraulicBoundaryLocationsMapDataObserver,
                dikeProfilesObserver,
                foreshoreProfilesObserver,
                calculationsMapDataObserver
            };
        }
    }
}