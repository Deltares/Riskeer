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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.HydraRing.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;
using Ringtoets.HydraRing.Data.TestUtil;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using GrassCoverErosionInwardsDataResources = Ringtoets.GrassCoverErosionInwards.Data.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryDatabaseIndex = 4;
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
        public void DefaultConstructor_Always_AddMapControlWithCollectionOfEmptyMapData()
        {
            // Call
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.AreEqual(GrassCoverErosionInwardsDataResources.GrassCoverErosionInwardsFailureMechanism_DisplayName, view.Map.Data.Name);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void Data_GrassCoverErosionInwardsFailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), new TestAssessmentSection());

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
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), new TestAssessmentSection());

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
        public void Data_EmptyGrassCoverErosionInwardsFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), new TestAssessmentSection());

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void Data_GrassCoverErosionInwardsFailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var geometryPoints = new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(2.0, 0.0),
                    new Point2D(4.0, 4.0),
                    new Point2D(6.0, 4.0)
                };

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));

                failureMechanism.DikeProfiles.Add(new TestDikeProfile());
                failureMechanism.DikeProfiles.Add(new TestDikeProfile());

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                var mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                var mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(8, mapDataList.Count);
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                AssertHydraulicBoundaryLocationsMapData(assessmentSection.HydraulicBoundaryDatabase, mapDataList[hydraulicBoundaryDatabaseIndex]);
                AssertDikeProfiles(failureMechanism.DikeProfiles, mapDataList[dikeProfilesIndex]);
                AssertForeshoreProfiles(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), mapDataList[foreshoreProfilesIndex]);
            }
        }

        [Test]
        public void NotifyObservers_HydraulicBoundaryDatabaseUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
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

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase1
                };

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                var hydraulicBoundaryDatabaseMapData = map.Data.Collection.ElementAt(hydraulicBoundaryDatabaseIndex);

                // Precondition
                AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase1, hydraulicBoundaryDatabaseMapData);

                assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase2;

                // Call
                assessmentSection.NotifyObservers();

                // Assert
                AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase2, hydraulicBoundaryDatabaseMapData);
            }
        }

        [Test]
        public void NotifyObservers_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
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

                var assessmentSection = new TestAssessmentSection
                {
                    ReferenceLine = new ReferenceLine()
                };
                assessmentSection.ReferenceLine.SetGeometry(points1);

                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(new GrassCoverErosionInwardsFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                var referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                assessmentSection.ReferenceLine.SetGeometry(points2);

                // Call
                assessmentSection.NotifyObservers();

                // Assert
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
            }
        }

        [Test]
        public void NotifyObservers_FailureMechanismSectionsUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new TestAssessmentSection());

                view.Data = failureMechanismContext;

                var sectionMapData = (MapLineData) map.Data.Collection.ElementAt(sectionsIndex);
                var sectionStartsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsStartPointIndex);
                var sectionsEndsMapData = (MapPointData) map.Data.Collection.ElementAt(sectionsEndPointIndex);

                failureMechanism.AddSection(new FailureMechanismSection(string.Empty, new[]
                {
                    new Point2D(1, 2),
                    new Point2D(1, 2)
                }));

                // Call
                failureMechanism.NotifyObservers();

                // Assert
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, sectionMapData);
                AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionStartsMapData);
                AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsEndsMapData);
            }
        }

        [Test]
        public void NotifyObservers_DikeProfilesUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl)view.Controls[0];

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new TestAssessmentSection());

                failureMechanism.DikeProfiles.Add(new TestDikeProfile());

                view.Data = failureMechanismContext;

                var dikeProfileData = map.Data.Collection.ElementAt(dikeProfilesIndex);

                // Precondition
                AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);

                failureMechanism.DikeProfiles.Add(new TestDikeProfile());

                // Call
                failureMechanism.DikeProfiles.NotifyObservers();

                // Assert
                AssertDikeProfiles(failureMechanism.DikeProfiles, dikeProfileData);
            }
        }

        [Test]
        public void NotifyObservers_ForeshoreProfilesUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl)view.Controls[0];

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new TestAssessmentSection());

                failureMechanism.DikeProfiles.Add(new TestDikeProfile());

                view.Data = failureMechanismContext;

                var dikeProfileData = map.Data.Collection.ElementAt(foreshoreProfilesIndex);

                // Precondition
                AssertForeshoreProfiles(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), dikeProfileData);

                failureMechanism.DikeProfiles.Add(new TestDikeProfile());

                // Call
                failureMechanism.DikeProfiles.NotifyObservers();

                // Assert
                AssertForeshoreProfiles(failureMechanism.DikeProfiles.Select(dp => dp.ForeshoreProfile), dikeProfileData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationGroupUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl)view.Controls[0];

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new TestAssessmentSection());

                var dikeProfileA = new TestDikeProfile(new Point2D(1.3, 1.3));
                var dikeProfileB = new TestDikeProfile(new Point2D(1.5, 1.5));

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

                var calculationMapData = (MapLineData)map.Data.Collection.ElementAt(calculationsIndex);

                failureMechanism.CalculationsGroup.Children.Add(calculationB);

                // Call
                failureMechanism.CalculationsGroup.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationInputUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl)view.Controls[0];

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new TestAssessmentSection());

                var dikeProfileA = new TestDikeProfile(new Point2D(1.3, 1.3));
                var dikeProfileB = new TestDikeProfile(new Point2D(1.5, 1.5));

                var calculationA = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };
                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData)map.Data.Collection.ElementAt(calculationsIndex);

                calculationA.InputParameters.DikeProfile = dikeProfileB;

                // Call
                calculationA.InputParameters.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
            }
        }

        [Test]
        public void UpdateObserver_CalculationUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl)view.Controls[0];

                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, new TestAssessmentSection());

                var dikeProfileA = new TestDikeProfile(new Point2D(1.3, 1.3));

                var calculationA = new GrassCoverErosionInwardsCalculation
                {
                    InputParameters =
                    {
                        DikeProfile = dikeProfileA,
                        HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                    }
                };

                view.Data = failureMechanismContext;

                var calculationMapData = (MapLineData)map.Data.Collection.ElementAt(calculationsIndex);

                calculationA.Name = "new name";

                // Call
                calculationA.NotifyObservers();

                // Assert
                AssertCalculationsMapData(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>(), calculationMapData);
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
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryDatabaseIndex - 1;
            const int updatedDikeProfilesLayerIndex = dikeProfilesIndex - 1;
            const int updatedForeshoreProfilesLayerIndex = foreshoreProfilesIndex - 1;
            const int updatedCalculationsIndex = calculationsIndex - 1;

            using (var view = new GrassCoverErosionInwardsFailureMechanismView())
            {
                var map = (MapControl) view.Controls[0];

                var assessmentSection = new TestAssessmentSection();
                var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
                var failureMechanismContext = new GrassCoverErosionInwardsFailureMechanismContext(failureMechanism, assessmentSection);

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

                var dikeProfilesData = (MapLineData) mapDataList[updatedDikeProfilesLayerIndex];
                Assert.AreEqual("Dijkprofielen", dikeProfilesData.Name);

                var foreshoreProfilesData = (MapLineData)mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", foreshoreProfilesData.Name);

                var calculationsData = (MapLineData)mapDataList[updatedCalculationsIndex];
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

                // Call
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

                var actualForeshoreProfilesData = (MapLineData)mapDataList[updatedForeshoreProfilesLayerIndex];
                Assert.AreEqual("Voorlandprofielen", actualForeshoreProfilesData.Name);

                var actualCalculationsData = (MapLineData)mapDataList[updatedCalculationsIndex];
                Assert.AreEqual("Berekeningen", actualCalculationsData.Name);
            }
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            IAssessmentSection oldAssessmentSection = new TestAssessmentSection();
            IAssessmentSection newAssessmentSection = new TestAssessmentSection();

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
                var map = (MapControl) view.Controls[0];

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

        private static void AssertReferenceLineMapData(ReferenceLine referenceLine, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var referenceLineData = (MapLineData) mapData;
            if (referenceLine == null)
            {
                CollectionAssert.IsEmpty(referenceLineData.Features.First().MapGeometries.First().PointCollections.First());
            }
            else
            {
                CollectionAssert.AreEqual(referenceLine.Points, referenceLineData.Features.First().MapGeometries.First().PointCollections.First());
            }
            Assert.AreEqual("Referentielijn", mapData.Name);
        }

        private static void AssertDikeProfiles(IEnumerable<DikeProfile> dikeProfiles, MapData mapData)
        {
            Assert.NotNull(dikeProfiles, "dikeProfiles should never be null.");

            var dikeProfilesData = (MapLineData) mapData;
            var dikeProfileArray = dikeProfiles.ToArray();

            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual(dikeProfileArray.Length, dikeProfilesData.Features.Length);

            for (int i = 0; i < dikeProfileArray.Length; i++)
            {
                var profileDataA = dikeProfilesData.Features[i].MapGeometries.First();
                CollectionAssert.AreEquivalent(dikeProfileArray[0].DikeGeometry, profileDataA.PointCollections.First());
            }
            
            Assert.AreEqual("Dijkprofielen", mapData.Name);
        }

        private static void AssertForeshoreProfiles(IEnumerable<ForeshoreProfile> foreshoreProfiles, MapData mapData)
        {
            Assert.NotNull(foreshoreProfiles, "foreshoreProfiles should never be null.");

            var foreshoreProfilesData = (MapLineData) mapData;
            var foreshoreProfileArray = foreshoreProfiles.ToArray();

            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual(foreshoreProfileArray.Length, foreshoreProfilesData.Features.Length);

            for (int i = 0; i < foreshoreProfileArray.Length; i++)
            {
                var profileDataA = foreshoreProfilesData.Features[i].MapGeometries.First();
                CollectionAssert.AreEquivalent(foreshoreProfileArray[0].Geometry, profileDataA.PointCollections.First());
            }

            Assert.AreEqual("Voorlandprofielen", mapData.Name);
        }

        private static void AssertHydraulicBoundaryLocationsMapData(HydraulicBoundaryDatabase database, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var hydraulicLocationsMapData = (MapPointData) mapData;
            if (database == null)
            {
                CollectionAssert.IsEmpty(hydraulicLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            }
            else
            {
                CollectionAssert.AreEqual(database.Locations.Select(hrp => hrp.Location), hydraulicLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            }
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);
        }

        private static void AssertFailureMechanismSectionsStartPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData) mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetStart()), sectionsStartPointData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Vakindeling (startpunten)", mapData.Name);
        }

        private static void AssertFailureMechanismSectionsEndPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData) mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetLast()), sectionsStartPointData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Vakindeling (eindpunten)", mapData.Name);
        }

        private static void AssertCalculationsMapData(IEnumerable<GrassCoverErosionInwardsCalculation> calculations, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var calculationsMapData = (MapLineData)mapData;
            var calculationsArray = calculations.ToArray();
            var calculationsFeatures = calculationsMapData.Features.ToArray();
            Assert.AreEqual(calculationsArray.Length, calculationsFeatures.Length);


            for (int index = 0; index < calculationsArray.Length; index++)
            {
                var geometries = calculationsFeatures[index].MapGeometries.ToArray();
                Assert.AreEqual(1, geometries.Length);

                GrassCoverErosionInwardsCalculation calculation = calculationsArray[index];
                CollectionAssert.AreEquivalent(new[] {
                        calculation.InputParameters.DikeProfile.WorldReferencePoint, 
                        calculation.InputParameters.HydraulicBoundaryLocation.Location
                    },
                geometries[0].PointCollections.First());
            }
            Assert.AreEqual("Berekeningen", mapData.Name);
        }

        private static void AssertEmptyMapData(MapDataCollection mapData)
        {
            Assert.IsInstanceOf<MapDataCollection>(mapData);

            var mapDataList = mapData.Collection.ToList();

            Assert.AreEqual(8, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var dikeProfilesMapData = (MapLineData) mapDataList[dikeProfilesIndex];
            var foreshoreProfilesMapData = (MapLineData) mapDataList[foreshoreProfilesIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryDatabaseMapData = (MapPointData) mapDataList[hydraulicBoundaryDatabaseIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(dikeProfilesMapData.Features);
            CollectionAssert.IsEmpty(foreshoreProfilesMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabaseMapData.Features);

            Assert.AreEqual(RingtoetsCommonDataResources.ReferenceLine_DisplayName, referenceLineMapData.Name);
            Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName, sectionsMapData.Name);
            Assert.AreEqual(RingtoetsCommonFormsResources.DikeProfiles_DisplayName, dikeProfilesMapData.Name);
            Assert.AreEqual(RingtoetsCommonFormsResources.ForeshoreProfiles_DisplayName, foreshoreProfilesMapData.Name);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_StartPoints_DisplayName), sectionsStartPointMapData.Name);
            Assert.AreEqual(GetSectionPointDisplayName(RingtoetsCommonFormsResources.FailureMechanismSections_EndPoints_DisplayName), sectionsEndPointMapData.Name);
            Assert.AreEqual(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName, hydraulicBoundaryDatabaseMapData.Name);
        }

        private static string GetSectionPointDisplayName(string name)
        {
            return string.Format("{0} ({1})",
                                 RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                                 name);
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Comments { get; set; }
            public AssessmentSectionComposition Composition { get; private set; }
            public ReferenceLine ReferenceLine { get; set; }
            public FailureMechanismContribution FailureMechanismContribution { get; private set; }
            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

            public IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }

            public void ChangeComposition(AssessmentSectionComposition newComposition)
            {
                throw new NotImplementedException();
            }
        }
    }
}