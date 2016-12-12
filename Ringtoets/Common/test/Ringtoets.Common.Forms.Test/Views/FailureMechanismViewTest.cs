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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class FailureMechanismViewTest
    {
        private const int referenceLineIndex = 0;
        private const int sectionsIndex = 1;
        private const int sectionsStartPointIndex = 2;
        private const int sectionsEndPointIndex = 3;
        private const int hydraulicBoundaryLocationsIndex = 4;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new FailureMechanismView<TestFailureMechanism>())
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
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_FailureMechanismContext_DataSet()
        {
            // Setup
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var failureMechanism = new TestFailureMechanism();
                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(failureMechanism, new TestAssessmentSection());

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
            using (var view = new FailureMechanismView<TestFailureMechanism>())
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
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), new TestAssessmentSection());

                view.Data = failureMechanismContext;

                // Precondition
                Assert.AreEqual(5, view.Map.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_EmptyFailureMechanismContext_NoMapDataSet()
        {
            // Setup
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), new TestAssessmentSection());

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void Data_FailureMechanismContext_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new FailureMechanismView<TestFailureMechanism>())
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

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                var failureMechanism = new TestFailureMechanism();
                failureMechanism.AddSection(new FailureMechanismSection("A", geometryPoints.Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("B", geometryPoints.Skip(1).Take(2)));
                failureMechanism.AddSection(new FailureMechanismSection("C", geometryPoints.Skip(2).Take(2)));

                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(failureMechanism, assessmentSection);

                // Call
                view.Data = failureMechanismContext;

                // Assert
                Assert.AreSame(failureMechanismContext, view.Data);

                var mapData = map.Data;
                Assert.IsInstanceOf<MapDataCollection>(mapData);

                var mapDataList = mapData.Collection.ToList();
                Assert.AreEqual(5, mapDataList.Count);
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, mapDataList[referenceLineIndex]);
                MapDataTestHelper.AssertFailureMechanismSectionsMapData(failureMechanism.Sections, mapDataList[sectionsIndex]);
                AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, mapDataList[sectionsStartPointIndex]);
                AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, mapDataList[sectionsEndPointIndex]);
                MapDataTestHelper.AssertHydraulicBoundaryLocationsMapData(hydraulicBoundaryDatabase.Locations, mapDataList[hydraulicBoundaryLocationsIndex]);
            }
        }

        [Test]
        public void UpdateObserver_AssessmentSectionUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new FailureMechanismView<TestFailureMechanism>())
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

                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), assessmentSection);

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
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var map = (MapControl) view.Controls[0];

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "test1", 1.0, 2.0)
                    }
                };

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
                };

                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), assessmentSection);

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
            using (var view = new FailureMechanismView<TestFailureMechanism>())
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

                var assessmentSection = new TestAssessmentSection
                {
                    HydraulicBoundaryDatabase = currentHydraulicBoundaryDatabase
                };

                view.Data = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), assessmentSection);

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
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new FailureMechanismView<TestFailureMechanism>())
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

                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), assessmentSection);

                view.Data = failureMechanismContext;

                var referenceLineMapData = map.Data.Collection.ElementAt(referenceLineIndex);

                // Precondition
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);

                // Call
                assessmentSection.ReferenceLine.SetGeometry(points2);
                assessmentSection.NotifyObservers();

                // Assert
                AssertReferenceLineMapData(assessmentSection.ReferenceLine, referenceLineMapData);
            }
        }

        [Test]
        public void UpdateObserver_FailureMechanismSectionsUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var map = (MapControl) view.Controls[0];

                var failureMechanism = new TestFailureMechanism();
                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(failureMechanism, new TestAssessmentSection());

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
                AssertFailureMechanismSectionsStartPointMapData(failureMechanism.Sections, sectionStartsMapData);
                AssertFailureMechanismSectionsEndPointMapData(failureMechanism.Sections, sectionsEndsMapData);
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
            const int updatedHydraulicLocationsLayerIndex = hydraulicBoundaryLocationsIndex - 1;

            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var map = (MapControl) view.Controls[0];

                var assessmentSection = new TestAssessmentSection();
                var failureMechanism = new TestFailureMechanism();
                var failureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(failureMechanism, assessmentSection);

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

            var oldFailureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), oldAssessmentSection);
            var newFailureMechanismContext = new FailureMechanismContext<TestFailureMechanism>(new TestFailureMechanism(), newAssessmentSection);
            using (var view = new FailureMechanismView<TestFailureMechanism>())
            {
                var map = (MapControl) view.Controls[0];

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

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Test failure mechanism", mapDataCollection.Name);

            var mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(5, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var sectionsMapData = (MapLineData) mapDataList[sectionsIndex];
            var sectionsStartPointMapData = (MapPointData) mapDataList[sectionsStartPointIndex];
            var sectionsEndPointMapData = (MapPointData) mapDataList[sectionsEndPointIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(sectionsMapData.Features);
            CollectionAssert.IsEmpty(sectionsStartPointMapData.Features);
            CollectionAssert.IsEmpty(sectionsEndPointMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Vakindeling", sectionsMapData.Name);
            Assert.AreEqual("Vakindeling (startpunten)", sectionsStartPointMapData.Name);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionsEndPointMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Comment Comments { get; private set; }
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