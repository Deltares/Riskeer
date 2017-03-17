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
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryLocationsIndex = 1;

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new AssessmentSectionView())
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
            using (var view = new AssessmentSectionView())
            {
                // Assert
                Assert.AreEqual(1, view.Controls.Count);
                Assert.AreSame(view.Map, view.Controls[0]);
                Assert.AreEqual(DockStyle.Fill, ((Control) view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_AssessmentSection_DataSet()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanAssessmentSection_DataNull()
        {
            // Setup
            using (var view = new AssessmentSectionView())
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

            using (var view = new AssessmentSectionView())
            {
                var mapControl = (RingtoetsMapControl) view.Map;

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection.BackgroundData, mapControl.BackgroundData);
            }
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new AssessmentSectionView
            {
                Data = new ObservableTestAssessmentSectionStub()
            })
            {
                var mapControl = (RingtoetsMapControl) view.Map;

                // Precondition
                Assert.AreEqual(2, mapControl.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(mapControl.Data);
                Assert.IsNull(mapControl.BackgroundData);
                Assert.IsNull(mapControl.BackgroundMapData);
            }
        }

        [Test]
        public void Data_EmptyAssessmentSection_DataUpdatedToCollectionOfEmptyMapData()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
                AssertEmptyMapData(view.Map.Data);
                ImageBasedMapData expectedImageBasedMapData = RingtoetsBackgroundMapDataFactory.CreateBackgroundMapData(
                    assessmentSection.BackgroundData);
                MapDataTestHelper.AssertImageBasedMapData(expectedImageBasedMapData, view.Map.BackgroundMapData);
            }
        }

        [Test]
        public void Data_WithReferenceLineAndHydraulicBoundaryDatabase_DataUpdatedToCollectionOfFilledMapData()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(1, "test", 1.0, 2.0)
                    }
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
                Assert.IsInstanceOf<MapDataCollection>(view.Map.Data);
                MapDataCollection mapData = view.Map.Data;
                Assert.IsNotNull(mapData);

                AssertHydraulicBoundaryDatabaseData(mapData, hydraulicBoundaryDatabase);
                AssertReferenceLineData(mapData, referenceLine);
            }
        }

        [Test]
        public void GivenChangedHydraulicBoundaryDatabase_WhenAssessmentSectionObserversNotified_MapDataUpdated()
        {
            // Given
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                };
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

                view.Data = assessmentSection;
                var mapData = view.Map.Data;

                // Precondition
                AssertHydraulicBoundaryDatabaseData(mapData, assessmentSection.HydraulicBoundaryDatabase);

                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

                // When
                assessmentSection.NotifyObservers();

                // Then
                AssertHydraulicBoundaryDatabaseData(mapData, assessmentSection.HydraulicBoundaryDatabase);
            }
        }

        [Test]
        public void GivenChangedHydraulicBoundaryDatabase_WhenHydraulicBoundaryDatabaseObserversNotified_MapDataUpdated()
        {
            // Given
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                };
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

                view.Data = assessmentSection;
                var mapData = view.Map.Data;

                // Precondition
                AssertHydraulicBoundaryDatabaseData(mapData, assessmentSection.HydraulicBoundaryDatabase);

                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                assessmentSection.NotifyObservers();

                // When
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "new 2", 2, 3));
                assessmentSection.HydraulicBoundaryDatabase.NotifyObservers();

                // Then
                AssertHydraulicBoundaryDatabaseData(mapData, assessmentSection.HydraulicBoundaryDatabase);
            }
        }
        
        [Test]
        public void UpdateObserver_ReferenceLineUpdated_MapDataUpdated()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var points = new List<Point2D>
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                };

                var pointsUpdate = new List<Point2D>
                {
                    new Point2D(2.0, 5.0),
                    new Point2D(4.0, 3.0)
                };

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    ReferenceLine = new ReferenceLine()
                };
                assessmentSection.ReferenceLine.SetGeometry(points);

                view.Data = assessmentSection;
                var mapData = view.Map.Data;

                // Precondition
                AssertReferenceLineData(mapData, assessmentSection.ReferenceLine);

                // Call
                assessmentSection.ReferenceLine.SetGeometry(pointsUpdate);
                assessmentSection.NotifyObservers();

                // Assert
                AssertReferenceLineData(mapData, assessmentSection.ReferenceLine);
            }
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new ObservableTestAssessmentSectionStub();

                view.Data = assessmentSection;

                var assessmentSection2 = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                    ReferenceLine = new ReferenceLine()
                };
                assessmentSection2.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));
                assessmentSection2.ReferenceLine.SetGeometry(new List<Point2D>
                {
                    new Point2D(2.0, 1.0),
                    new Point2D(4.0, 3.0)
                });

                // Call
                assessmentSection2.NotifyObservers();

                // Assert
                Assert.AreEqual(assessmentSection, view.Data);
                AssertEmptyMapData(view.Map.Data);
            }
        }

        [Test]
        public void UpdateObserver_DataUpdated_MapLayersSameOrder()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(new[]
                {
                    new Point2D(1.0, 2.0),
                    new Point2D(2.0, 1.0)
                });

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

                var assessmentSection = new ObservableTestAssessmentSectionStub
                {
                    HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                    ReferenceLine = referenceLine
                };

                view.Data = assessmentSection;

                MapDataCollection mapData = view.Map.Data;

                var dataToMove = mapData.Collection.ElementAt(0);
                mapData.Remove(dataToMove);
                mapData.Add(dataToMove);

                // Precondition
                var referenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
                Assert.AreEqual("Referentielijn", referenceLineMapData.Name);

                var hrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex - 1);
                Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);

                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

                // Call
                assessmentSection.NotifyObservers();

                // Assert
                var actualReferenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
                Assert.AreEqual("Referentielijn", actualReferenceLineMapData.Name);

                var actualHrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex - 1);
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHrLocationsMapData.Name);
            }
        }

        private static void AssertHydraulicBoundaryDatabaseData(MapDataCollection mapData, HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var hrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryLocationsIndex);
            CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations.Select(l => l.Location), hrLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);
            Assert.IsTrue(hrLocationsMapData.IsVisible);
        }

        private static void AssertReferenceLineData(MapDataCollection mapData, ReferenceLine referenceLine)
        {
            var referenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex);
            CollectionAssert.AreEqual(referenceLine.Points, referenceLineMapData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        private static void AssertEmptyMapData(MapDataCollection mapDataCollection)
        {
            Assert.AreEqual("Trajectkaart", mapDataCollection.Name);

            var mapDataList = mapDataCollection.Collection.ToList();

            Assert.AreEqual(2, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var hydraulicBoundaryLocationsMapData = (MapPointData) mapDataList[hydraulicBoundaryLocationsIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryLocationsMapData.Features);

            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicBoundaryLocationsMapData.Name);
        }
    }
}