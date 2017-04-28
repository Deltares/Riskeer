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
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneErosionFailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int duneLocationsIndex = 4;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new DuneErosionFailureMechanismView())
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
            using (var view = new DuneErosionFailureMechanismView())
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
        public void Data_FailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
            {
                var failureMechanism = new DuneErosionFailureMechanism();

                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreEqual(failureMechanism.Name, view.Map.Data.Name);
                Assert.AreSame(failureMechanismContext, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanFailureMechanismContext_DataNull()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
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

            using (var view = new DuneErosionFailureMechanismView())
            {
                var failureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), assessmentSection);

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
            using (var view = new DuneErosionFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(5, view.Map.Data.Collection.Count());
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
        public void Data_EmptyFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                var failureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
                MapDataTestHelper.AssertImageBasedMapData(assessmentSection.BackgroundData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_FailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
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
                    ReferenceLine = referenceLine
                };

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocations =
                    {
                        new TestDuneLocation()
                    }
                };
                failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));

                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                MapDataCollection mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                List<MapData> mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(5, mapDataList.Count);
                MapDataTestHelper.AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, mapDataList[duneLocationsIndex]);
            }
        }

        [Test]
        public void UpdateObserver_AssessmentSectionUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var duneLocation1 = new TestDuneLocation();

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocations =
                    {
                        duneLocation1
                    }
                };
                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

                view.Data = failureMechanismContext;

                MapData duneLocationsMapData = map.Data.Collection.ElementAt(duneLocationsIndex);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, duneLocationsMapData);

                // Call
                failureMechanism.DuneLocations.Add(new TestDuneLocation());
                assessmentSection.NotifyObservers();

                // Assert
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, duneLocationsMapData);
            }
        }

        [Test]
        public void UpdateObserver_DuneLocationsUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var duneLocation1 = new TestDuneLocation();

                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocations =
                    {
                        duneLocation1
                    }
                };
                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

                view.Data = failureMechanismContext;

                MapData duneLocationsMapData = map.Data.Collection.ElementAt(duneLocationsIndex);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, duneLocationsMapData);

                // Call
                failureMechanism.DuneLocations.Add(new TestDuneLocation());
                failureMechanism.DuneLocations.NotifyObservers();

                // Assert
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, duneLocationsMapData);
            }
        }

        [Test]
        public void GivenAssessmentSectionWithDuneLocations_WhenNewDuneLocationsAreSetAndNotified_ThenMapDataUpdated()
        {
            // Given
            using (var view = new DuneErosionFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var failureMechanism = new DuneErosionFailureMechanism
                {
                    DuneLocations =
                    {
                        new TestDuneLocation()
                    }
                };
                view.Data = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

                MapData duneLocationsMapData = map.Data.Collection.ElementAt(duneLocationsIndex);

                // Precondition
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, duneLocationsMapData);

                // When
                failureMechanism.DuneLocations.Clear();
                failureMechanism.DuneLocations.Add(new TestDuneLocation());
                failureMechanism.DuneLocations.NotifyObservers();

                // Then
                AssertDuneLocationsMapData(failureMechanism.DuneLocations, duneLocationsMapData);
            }
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new DuneErosionFailureMechanismView())
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

                var failureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                MapData referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

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
            using (var view = new DuneErosionFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var failureMechanism = new DuneErosionFailureMechanism();
                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, new ObservableTestAssessmentSectionStub());

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
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            const int updatedRefenceLineLayerIndex = referenceLineIndex + 4;
            const int updatedSectionsLayerIndex = sectionsIndex - 1;
            const int updateSectionStartLayerIndex = sectionsStartPointIndex - 1;
            const int updatedSectionEndLayerIndex = sectionsEndPointIndex - 1;
            const int updatedDuneLocationsLayerIndex = duneLocationsIndex - 1;

            using (var view = new DuneErosionFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                var assessmentSection = new ObservableTestAssessmentSectionStub();
                var failureMechanism = new DuneErosionFailureMechanism();
                var failureMechanismContext = new DuneErosionFailureMechanismContext(failureMechanism, assessmentSection);

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

                var duneLocationsData = (MapPointData) mapDataList[updatedDuneLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", duneLocationsData.Name);

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

                var actualDuneLocationsData = (MapPointData) mapDataList[updatedDuneLocationsLayerIndex];
                Assert.AreEqual("Hydraulische randvoorwaarden", actualDuneLocationsData.Name);
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

            var oldFailureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), oldAssessmentSection);
            var newFailureMechanismContext = new DuneErosionFailureMechanismContext(new DuneErosionFailureMechanism(), newAssessmentSection);
            using (var view = new DuneErosionFailureMechanismView())
            {
                IMapControl map = ((RingtoetsMapControl) view.Controls[0]).MapControl;

                view.Data = oldFailureMechanismContext;
                view.Data = newFailureMechanismContext;
                MapData dataBeforeUpdate = map.Data;

                newAssessmentSection.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

                // Call
                oldAssessmentSection.NotifyObservers();

                // Assert
                Assert.AreEqual(dataBeforeUpdate, map.Data);
            }
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Duinwaterkering - Duinafslag", mapDataCollection.Name);

            List<MapData> mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(5, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var duneLocationsMapData = (MapPointData) mapDataList[duneLocationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(duneLocationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", duneLocationsMapData.Name);
        }

        private static void AssertDuneLocationsMapData(IEnumerable<DuneLocation> duneLocations, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var duneLocationsMapData = (MapPointData) mapData;
            if (duneLocations == null)
            {
                CollectionAssert.IsEmpty(duneLocationsMapData.Features);
            }
            else
            {
                DuneLocation[] duneLocationsArray = duneLocations.ToArray();

                Assert.AreEqual(duneLocationsArray.Length, duneLocationsMapData.Features.Length);
                CollectionAssert.AreEqual(duneLocationsArray.Select(hrp => hrp.Location),
                                          duneLocationsMapData.Features.SelectMany(f => f.MapGeometries.First().PointCollections.First()));
            }
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);
        }
    }
}