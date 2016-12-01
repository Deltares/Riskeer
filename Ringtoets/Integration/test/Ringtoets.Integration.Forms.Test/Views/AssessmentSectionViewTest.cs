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
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionViewTest
    {
        private const int referenceLineIndex = 0;
        private const int hydraulicBoundaryDatabaseIndex = 1;

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
                Assert.AreEqual(DockStyle.Fill, ((Control)view.Map).Dock);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_AssessmentSection_DataSet()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new TestAssessmentSection();

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
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            using (var view = new AssessmentSectionView
            {
                Data = new TestAssessmentSection()
            })
            {
                // Precondition
                Assert.AreEqual(2, view.Map.Data.Collection.Count());

                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
                Assert.IsNull(view.Map.Data);
            }
        }

        [Test]
        public void Data_EmptyAssessmentSection_DataUpdatedToCollectionOfEmptyMapData()
        {
            // Setup
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new TestAssessmentSection();

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
                AssertEmptyMapData(view.Map.Data);
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

                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

                var assessmentSection = new TestAssessmentSection
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
                var assessmentSection = new TestAssessmentSection
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
        public void GivenChangedHydraulicBoundaryDatabase_WhenObserversNotified_MapDataUpdated()
        {
            // Given
            using (var view = new AssessmentSectionView())
            {
                var assessmentSection = new TestAssessmentSection
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

                var assessmentSection = new TestAssessmentSection
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
                var assessmentSection = new TestAssessmentSection();

                view.Data = assessmentSection;

                var assessmentSection2 = new TestAssessmentSection
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

                var assessmentSection = new TestAssessmentSection
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

                var hrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryDatabaseIndex - 1);
                Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);

                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
                assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

                // Call
                assessmentSection.NotifyObservers();

                // Assert
                var actualReferenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex + 1);
                Assert.AreEqual("Referentielijn", actualReferenceLineMapData.Name);

                var actualHrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryDatabaseIndex - 1);
                Assert.AreEqual("Hydraulische randvoorwaarden", actualHrLocationsMapData.Name);
            }
        }

        private static void AssertHydraulicBoundaryDatabaseData(MapDataCollection mapData, HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            var hrLocationsMapData = (MapPointData) mapData.Collection.ElementAt(hydraulicBoundaryDatabaseIndex);
            CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations.Select(l => l.Location), hrLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName, hrLocationsMapData.Name);
            Assert.IsTrue(hrLocationsMapData.IsVisible);
        }

        private static void AssertReferenceLineData(MapDataCollection mapData, ReferenceLine referenceLine)
        {
            var referenceLineMapData = (MapLineData) mapData.Collection.ElementAt(referenceLineIndex);
            CollectionAssert.AreEqual(referenceLine.Points, referenceLineMapData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual(RingtoetsCommonDataResources.ReferenceLine_DisplayName, referenceLineMapData.Name);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        private static void AssertEmptyMapData(MapDataCollection mapData)
        {
            Assert.IsInstanceOf<MapDataCollection>(mapData);

            var mapDataList = mapData.Collection.ToList();

            Assert.AreEqual(2, mapDataList.Count);

            var referenceLineMapData = (MapLineData) mapDataList[referenceLineIndex];
            var hydraulicBoundaryDatabaseMapData = (MapPointData) mapDataList[hydraulicBoundaryDatabaseIndex];

            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabaseMapData.Features);

            Assert.AreEqual(RingtoetsCommonDataResources.ReferenceLine_DisplayName, referenceLineMapData.Name);
            Assert.AreEqual(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName, hydraulicBoundaryDatabaseMapData.Name);
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public AssessmentSectionComposition Composition { get; private set; }

            public Comment Comments { get; private set; }

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