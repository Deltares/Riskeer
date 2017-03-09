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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.Views;

namespace Ringtoets.WaveImpactAsphaltCover.Forms.Test.Views
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryLocationsIndex = 4;
        private const int foreshoreProfilesIndex = 5;
        private const int calculationsIndex = 6;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
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
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_WaveImpactAsphaltCoverFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(
                    new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanWaveImpactAsphaltCoverFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_AssessmentSectionWithBackgroundMapData_BackgroundMapDataSet()
        {
            // Setup
            IAssessmentSection assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(
                    new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                ImageBasedMapData expectedWmtsBackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(
                    assessmentSection.BackgroundData);
                MapDataTestHelper.AssertImageBasedMapData(expectedWmtsBackgroundMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(
                    new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(7, view.Map.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
                Assert.IsNull(view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_EmptyWaveImpactAsphaltCoverFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(
                    new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
                ImageBasedMapData expectedWmtsBackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(
                    assessmentSection.BackgroundData);
                MapDataTestHelper.AssertImageBasedMapData(expectedWmtsBackgroundMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_WaveImpactAsphaltCoverFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

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
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                var foreshoreProfileA = new TestForeshoreProfile(new Point2D(1.3, 1.3));
                var foreshoreProfileB = new TestForeshoreProfile(new Point2D(1.5, 1.5));

                var calculationA = new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        ForeshoreProfile = foreshoreProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                var calculationB = new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        ForeshoreProfile = foreshoreProfileB,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));

                failureMechanism.ForeshoreProfiles.Add(new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }));
                failureMechanism.ForeshoreProfiles.Add(new TestForeshoreProfile(new[]
                {
                    new Point2D(2, 2),
                    new Point2D(3, 3)
                }));
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationB);

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                var mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                var mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(7, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapDataList[hydraulicBoundaryLocationsIndex]);
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>(),
                                          mapDataList[calculationsIndex]);
            }
        }

        [Test]
        public void GivenChangedBackgroundMapData_WhenBackgroundMapDataObserversNotified_MapDataUpdated()
        {
            // Given
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();
                view.Data = new WaveImpactAsphaltCoverFailureMechanismContext(new WaveImpactAsphaltCoverFailureMechanism(),
                                                                              assessmentSection);

                BackgroundData backgroundData = assessmentSection.BackgroundData;

                backgroundData.Name = "some Name";
                backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl] = "some URL";
                backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier] = "some Identifier";
                backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat] = "image/some Format";
                backgroundData.IsConfigured = true;

                // When
                backgroundData.NotifyObservers();

                // Then
                ImageBasedMapData expectedWmtsBackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(
                    backgroundData);
                MapDataTestHelper.AssertImageBasedMapData(expectedWmtsBackgroundMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void UpdateObserver_AssessmentSectionUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

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
                        new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0)
                    }
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase1
                };

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                var hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase1.Locations, hydraulicBoundaryLocationsMapData);

                // Call
                assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase2;
                assessmentSection.NotifyObservers();

                // Assert
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase2.Locations, hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

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

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                var hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);

                // Call
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 3.0, 4.0));
                hydraulicBoundaryDatabase.NotifyObservers();

                // Assert
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

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

                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(new WaveImpactAsphaltCoverFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                var referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // Call
                assessmentSection.ReferenceLine.SetGeometry(points2);
                assessmentSection.NotifyObservers();

                // Assert
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
            }
        }

        [Test]
        public void UpdateObserver_FailureMechanismSectionsUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                view.Data = failureMechanismContext;

                var sectionMapData = (MapLineData) map.Data.Collection.ElementAt(sectionsIndex);
                var sectionStartsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsStartPointIndex);
                var sectionsEndsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsEndPointIndex);

                // Call
                failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                }));
                failureMechanism.NotifyObservers();

                // Assert
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionMapData);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionStartsMapData);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsEndsMapData);
            }
        }

        [Test]
        public void UpdateObserver_ForeshoreProfilesUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                failureMechanism.ForeshoreProfiles.Add(new TestForeshoreProfile(new[]
                {
                    new Point2D(0, 0),
                    new Point2D(1, 1)
                }));

                view.Data = failureMechanismContext;

                var foreshoreProfileData = map.Data.Collection.ElementAt(foreshoreProfilesIndex);

                // Precondition
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, foreshoreProfileData);

                // Call
                failureMechanism.ForeshoreProfiles.Add(new TestForeshoreProfile(new[]
                {
                    new Point2D(2, 2),
                    new Point2D(3, 3)
                }));
                failureMechanism.ForeshoreProfiles.NotifyObservers();

                // Assert
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, foreshoreProfileData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationGroupUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var foreshoreProfileA = new TestForeshoreProfile(new Point2D(1.3, 1.3));
                var foreshoreProfileB = new TestForeshoreProfile(new Point2D(1.5, 1.5));

                var calculationA = new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        ForeshoreProfile = foreshoreProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                var calculationB = new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        ForeshoreProfile = foreshoreProfileB,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationA);

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationB);

                // Call
                failureMechanism.WaveConditionsCalculationGroup.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationInputUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var foreshoreProfileA = new TestForeshoreProfile(new Point2D(1.3, 1.3));
                var foreshoreProfileB = new TestForeshoreProfile(new Point2D(1.5, 1.5));

                var calculationA = new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        ForeshoreProfile = foreshoreProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                calculationA.InputParameters.ForeshoreProfile = foreshoreProfileB;

                // Call
                calculationA.InputParameters.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var foreshoreProfileA = new TestForeshoreProfile(new Point2D(1.3, 1.3));

                var calculationA = new WaveImpactAsphaltCoverWaveConditionsCalculation
                {
                    InputParameters =
                    {
                        ForeshoreProfile = foreshoreProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                calculationA.Name = "new name";

                // Call
                calculationA.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 6;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
                var failureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(failureMechanism, assessmentSection);

                view.Data = failureMechanismContext;

                var mapData = map.Data;

                var dataToMove = (MapLineData) map.Data.Collection.ElementAt(referenceLineIndex);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                var mapDataList = mapData.Collection.ToList();

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

                var foreshoreProfilesData = (MapLineData) mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

                var calculationsData = (MapLineData) mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", calculationsData.Name);

                var points = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };
                ReferenceLine referenceLine = new ReferenceLine();
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

            var oldWaveImpactAsphaltCoverFailureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(new WaveImpactAsphaltCoverFailureMechanism(), oldAssessmentSection);
            var newWaveImpactAsphaltCoverFailureMechanismContext = new WaveImpactAsphaltCoverFailureMechanismContext(new WaveImpactAsphaltCoverFailureMechanism(), newAssessmentSection);
            using (var view = new WaveImpactAsphaltCoverFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                view.Data = oldWaveImpactAsphaltCoverFailureMechanismContext;
                view.Data = newWaveImpactAsphaltCoverFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertCalculationsMapData(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData) mapData;
            var calculationsArray = calculations.ToArray();
            var calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);

            for (int index = 0; index < calculationsArray.Length; index++)
            {
                var geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                WaveImpactAsphaltCoverWaveConditionsCalculation calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                {
                    calculation.InputParameters.ForeshoreProfile.WorldReferencePoint,
                    calculation.InputParameters.HydraulicBoundaryLocation.Location
                }, geometries[0].PointCollections.First());
            }
            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Dijken en dammen - Golfklappen op asfaltbekleding", mapDataCollection.Name);

            var mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(7, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Berekeningen", calculationsMapData.Name);
        }
    }
}