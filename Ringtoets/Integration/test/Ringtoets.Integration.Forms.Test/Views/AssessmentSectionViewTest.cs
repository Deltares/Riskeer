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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class AssessmentSectionViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var view = new AssessmentSectionView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsInstanceOf<IObserver>(view);
            Assert.IsNotNull(view.Map);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void DefaultConstructor_Always_AddsMapControl()
        {
            // Call
            var view = new AssessmentSectionView();

            // Assert
            Assert.AreEqual(1, view.Controls.Count);
            var mapObject = view.Controls[0] as MapControl;
            Assert.NotNull(mapObject);
            Assert.AreEqual(DockStyle.Fill, mapObject.Dock);
            Assert.IsNotNull(mapObject.Data);
            CollectionAssert.IsEmpty(mapObject.Data.List);
        }

        [Test]
        public void Data_EmptyAssessmentSection_NoMapDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();

            var assessmentSection = new TestAssessmentSection();

            // Call
            view.Data = assessmentSection;

            // Assert
            MapDataCollection mapData = view.Map.Data;

            Assert.AreEqual(2, mapData.List.Count);

            var hrLocationsMapData = (MapPointData) mapData.List[0];
            CollectionAssert.IsEmpty(hrLocationsMapData.Features);
            Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);
            Assert.IsTrue(hrLocationsMapData.IsVisible);

            var referenceLineMapData = (MapLineData) mapData.List[1];
            CollectionAssert.IsEmpty(referenceLineMapData.Features);
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        [Test]
        public void Data_SetMapData_MapDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();

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

            var hrLocationsMapData = (MapPointData) mapData.List[0];
            CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations.Select(l => l.Location), hrLocationsMapData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);
            Assert.IsTrue(hrLocationsMapData.IsVisible);

            var referenceLineMapData = (MapLineData) mapData.List[1];
            CollectionAssert.AreEqual(referenceLine.Points, referenceLineMapData.Features.First().MapGeometries.First().PointCollections.First());
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new AssessmentSectionView();

            var assessmentSection = new TestAssessmentSection
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            view.Data = assessmentSection;
            var mapData = view.Map.Data;

            var mapDataElementBeforeUpdate = (MapPointData) mapData.List.First();
            var geometryBeforeUpdate = mapDataElementBeforeUpdate.Features.First().MapGeometries.First().PointCollections.First();

            // Precondition
            Assert.AreEqual(new Point2D(1.0, 2.0), geometryBeforeUpdate.First());

            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.IsInstanceOf<MapDataCollection>(view.Map.Data);
            Assert.AreEqual(mapData, view.Map.Data);
            CollectionAssert.AreEquivalent(mapData.List, view.Map.Data.List);

            var mapDataElementAfterUpdate = (MapPointData)view.Map.Data.List.First();
            var geometryAfterUpdate = mapDataElementAfterUpdate.Features.First().MapGeometries.First().PointCollections.First();

            Assert.AreEqual(new Point2D(2.0, 3.0), geometryAfterUpdate.First());
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new AssessmentSectionView();

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

            var mapDataElementBeforeUpdate = (MapLineData) mapData.List.ElementAt(1);
            var geometryBeforeUpdate = mapDataElementBeforeUpdate.Features.First().MapGeometries.First().PointCollections.First();

            // Precondition
            CollectionAssert.AreEquivalent(geometryBeforeUpdate, points);

            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSection.ReferenceLine.SetGeometry(pointsUpdate);

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.IsInstanceOf<MapDataCollection>(view.Map.Data);
            Assert.AreEqual(mapData, view.Map.Data);
            CollectionAssert.AreEquivalent(mapData.List, view.Map.Data.List);

            var mapDataElementAfterUpdate = (MapLineData)view.Map.Data.List.ElementAt(1);
            var geometryAfterUpdate = mapDataElementAfterUpdate.Features.First().MapGeometries.First().PointCollections.First();

            CollectionAssert.AreEquivalent(geometryAfterUpdate, pointsUpdate);
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            var view = new AssessmentSectionView();

            var assessmentSection = new TestAssessmentSection
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

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
            Assert.IsInstanceOf<MapDataCollection>(view.Map.Data);
        }

        [Test]
        public void Data_SetToNull_MapDataCleared()
        {
            // Setup
            var view = new AssessmentSectionView();

            var assessmentSection = new TestAssessmentSection
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            view.Data = assessmentSection;

            // Precondition
            Assert.AreEqual(assessmentSection, view.Data);

            // Call
            view.Data = null;

            // Assert
            Assert.IsNull(view.Data);
            Assert.IsNull(view.Map.Data);
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public AssessmentSectionComposition Composition { get; private set; }
            public string Comments { get; set; }
            public ReferenceLine ReferenceLine { get; set; }
            public FailureMechanismContribution FailureMechanismContribution { get; private set; }
            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

            public long StorageId { get; set; }

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