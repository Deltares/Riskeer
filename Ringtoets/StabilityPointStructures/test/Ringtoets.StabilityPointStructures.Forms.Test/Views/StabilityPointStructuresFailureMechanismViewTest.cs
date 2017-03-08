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
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Views;

namespace Ringtoets.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryLocationsIndex = 4;
        private const int foreshoreProfilesIndex = 5;
        private const int structuresIndex = 6;
        private const int calculationsIndex = 7;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new StabilityPointStructuresFailureMechanismView())
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
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_StabilityPointStructuresFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(
                    new StabilityPointStructuresFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanStabilityPointStructuresFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
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

            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(
                    new StabilityPointStructuresFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert

                WmtsMapData expectedWmtsBackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(assessmentSection.BackgroundData);
                MapDataTestHelper.AssertWmtsMapData(expectedWmtsBackgroundMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(
                    new StabilityPointStructuresFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(8, view.Map.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
                Assert.IsNull(view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_EmptyStabilityPointStructuresFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(
                    new StabilityPointStructuresFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
                WmtsMapData expectedWmtsBackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(
                    assessmentSection.BackgroundData);
                MapDataTestHelper.AssertWmtsMapData(expectedWmtsBackgroundMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_StabilityPointStructuresFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
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

                var calculationLocationA = new Point2D(1.2, 2.3);
                var calculationLocationB = new Point2D(2.7, 2.0);

                var hydraulicBoundaryLocationA = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3);
                var hydraulicBoundaryLocationB = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6);

                var calculationA = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocationA,
                        Structure = new TestStabilityPointStructure(calculationLocationA)
                    }
                };

                var calculationB = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocationB,
                        Structure = new TestStabilityPointStructure(calculationLocationB)
                    }
                };

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
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
                failureMechanism.CalculationsGroup.Children.Add(calculationA);
                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                var mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                var mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(8, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapDataList[hydraulicBoundaryLocationsIndex]);
                MapDataTestHelper.AssertForeshoreProfilesMapData(failureMechanism.ForeshoreProfiles, mapDataList[foreshoreProfilesIndex]);

                AssertCalculationsMapData(
                    failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>(),
                    mapDataList[calculationsIndex]);
            }
        }

        [Test]
        public void UpdateObserver_AssessmentSectionUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
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

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), assessmentSection);

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
            using (var view = new StabilityPointStructuresFailureMechanismView())
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

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), assessmentSection);

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
        public void GivenAssessmentSectionWithHydraulicBoundaryDatabase_WhenNewDatabaseIsSetAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

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

                view.Data = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), assessmentSection);

                var hydraulicBoundaryLocationsMapData = map.Data.Collection.ElementAt(hydraulicBoundaryLocationsIndex);

                // Precondition
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(currentHydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);

                // When
                assessmentSection.HydraulicBoundaryDatabase = newHydraulicBoundaryDatabase;
                assessmentSection.NotifyObservers();
                newHydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "new 2", 2, 3));
                newHydraulicBoundaryDatabase.NotifyObservers();

                // Then
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(newHydraulicBoundaryDatabase.Locations, hydraulicBoundaryLocationsMapData);
            }
        }

        [Test]
        public void GivenChangedBackgroundMapData_WhenBackgroundMapDataObserversNotified_MapDataUpdated()
        {
            // Given
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();
                view.Data = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(),
                                                                                assessmentSection);

                BackgroundData backgroundData = assessmentSection.BackgroundData;

                backgroundData.Name = "some Name";
                backgroundData.Parameters["SourceCapabilitiesUrl"] = "some URL";
                backgroundData.Parameters["SelectedCapabilityIdentifier"] = "some Identifier";
                backgroundData.Parameters["PreferredFormat"] = "image/some Format";
                backgroundData.IsConfigured = true;

                // When
                backgroundData.NotifyObservers();

                // Then
                WmtsMapData expectedWmtsBackgroundMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(backgroundData);
                MapDataTestHelper.AssertWmtsMapData(expectedWmtsBackgroundMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
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

                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), assessmentSection);

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
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

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
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

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
        public void UpdateObserver_StructuresUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                failureMechanism.ForeshoreProfiles.Add(new TestForeshoreProfile());

                view.Data = failureMechanismContext;

                var structuresData = map.Data.Collection.ElementAt(structuresIndex);

                // Precondition
                AssertStructures(failureMechanism.StabilityPointStructures, structuresData);

                // Call
                failureMechanism.StabilityPointStructures.Add(new TestStabilityPointStructure());
                failureMechanism.StabilityPointStructures.NotifyObservers();

                // Assert
                AssertStructures(failureMechanism.StabilityPointStructures, structuresData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationGroupUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var calculationLocationA = new Point2D(1.2, 2.3);
                var calculationLocationB = new Point2D(2.7, 2.0);

                var hydraulicBoundaryLocationA = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3);
                var hydraulicBoundaryLocationB = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6);

                var calculationA = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocationA,
                        Structure = new TestStabilityPointStructure(calculationLocationA)
                    }
                };

                var calculationB = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocationB,
                        Structure = new TestStabilityPointStructure(calculationLocationB)
                    }
                };

                failureMechanism.CalculationsGroup.Children.Add(calculationA);

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                // Call
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationInputUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var calculationLocationA = new Point2D(1.2, 2.3);
                var calculationLocationB = new Point2D(2.7, 2.0);

                var hydraulicBoundaryLocationA = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3);

                var calculationA = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocationA,
                        Structure = new TestStabilityPointStructure(calculationLocationA)
                    }
                };

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                calculationA.InputParameters.Structure = new TestStabilityPointStructure(calculationLocationB);

                // Call
                calculationA.InputParameters.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

                var calculationLocationA = new Point2D(1.2, 2.3);

                var hydraulicBoundaryLocationA = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3);

                var calculationA = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocationA,
                        Structure = new TestStabilityPointStructure(calculationLocationA)
                    }
                };

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData) map.Data.Collection.ElementAt(calculationsIndex);

                calculationA.Name = "new name";

                // Call
                calculationA.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 7;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedStructuresLayerIndex = structuresIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var failureMechanism = new StabilityPointStructuresFailureMechanism();
                var failureMechanismContext = new StabilityPointStructuresFailureMechanismContext(failureMechanism, assessmentSection);

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

                var structuresData = (MapPointData) mapDataList[updatedStructuresLayerIndex];
                Assert.AreEqual("Kunstwerken", structuresData.Name);

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

                var actualStructuresData = (MapPointData) mapDataList[updatedStructuresLayerIndex];
                Assert.AreEqual("Kunstwerken", actualStructuresData.Name);

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

            var oldStabilityPointStructuresFailureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), oldAssessmentSection);
            var newStabilityPointStructuresFailureMechanismContext = new StabilityPointStructuresFailureMechanismContext(new StabilityPointStructuresFailureMechanism(), newAssessmentSection);
            using (var view = new StabilityPointStructuresFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                view.Data = oldStabilityPointStructuresFailureMechanismContext;
                view.Data = newStabilityPointStructuresFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertStructures(IEnumerable<StructureBase> structures, MapData mapData)
        {
            Assert.NotNull(structures, "structures should never be null.");

            var structuresData = (MapPointData) mapData;
            var structuresArray = structures.ToArray();

            Assert.AreEqual(structuresArray.Length, structuresData.Features.Length);

            for (int i = 0; i < structuresArray.Length; i++)
            {
                var profileDataA = structuresData.Features[i].MapGeometries.First();
                Assert.AreEqual(structuresArray[0].Location, profileDataA.PointCollections.First().First());
            }

            Assert.AreEqual("Kunstwerken", mapData.Name);
        }

        private static void AssertCalculationsMapData(IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations, MapData mapData)
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

                StructuresCalculation<StabilityPointStructuresInput> calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[]
                {
                    calculation.InputParameters.Structure.Location,
                    calculation.InputParameters.HydraulicBoundaryLocation.Location
                }, geometries[0].PointCollections.First());
            }
            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Kunstwerken - Sterkte en stabiliteit puntconstructies", mapDataCollection.Name);

            var mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(8, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var structuresMapData = (MapPointData) mapDataList[structuresIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];
            var calculationsMapData = (MapLineData) mapDataList[calculationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(structuresMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);
            CollectionAssert.IsEmpty(calculationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Voorlandprofielen", foreshoreProfilesMapData.Name);
            Assert.AreEqual("Kunstwerken", structuresMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
            Assert.AreEqual("Berekeningen", calculationsMapData.Name);
        }
    }
}