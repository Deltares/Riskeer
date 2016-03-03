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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Ringtoets.Common.Data;
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
            Assert.IsNull(mapObject.Data);
        }

        [Test]
        public void Data_EmptyAssessmentSection_NoMapDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase();

            // Call
            view.Data = assessmentSectionBase;

            // Assert
            var mapData = (MapDataCollection) map.Data;

            Assert.AreEqual(2, mapData.List.Count);

            var hrLocationsMapData = (MapPointData)mapData.List[0];
            CollectionAssert.IsEmpty(hrLocationsMapData.Points);
            Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);
            Assert.IsTrue(hrLocationsMapData.IsVisible);

            var referenceLineMapData = (MapLineData)mapData.List[1];
            CollectionAssert.IsEmpty(referenceLineMapData.Points);
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        [Test]
        public void Data_SetMapData_MapDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (MapControl) view.Controls[0];

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase,
                ReferenceLine = referenceLine
            };

            // Call
            view.Data = assessmentSectionBase;

            // Assert
            Assert.AreSame(assessmentSectionBase, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            var mapData = map.Data as MapDataCollection;
            Assert.IsNotNull(mapData);

            var hrLocationsMapData = (MapPointData)mapData.List[0];
            CollectionAssert.AreEqual(hydraulicBoundaryDatabase.Locations.Select(l => l.Location), hrLocationsMapData.Points);
            Assert.AreEqual("Hydraulische randvoorwaarden", hrLocationsMapData.Name);
            Assert.IsTrue(hrLocationsMapData.IsVisible);

            var referenceLineMapData = (MapLineData)mapData.List[1];
            CollectionAssert.AreEqual(referenceLine.Points, referenceLineMapData.Points);
            Assert.AreEqual("Referentielijn", referenceLineMapData.Name);
            Assert.IsTrue(referenceLineMapData.IsVisible);
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            view.Data = assessmentSectionBase;
            var mapData = map.Data;

            assessmentSectionBase.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

            // Call
            assessmentSectionBase.NotifyObservers();

            // Assert
            Assert.AreNotEqual(mapData, map.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            view.Data = assessmentSectionBase;
            var mapData = map.Data;

            assessmentSectionBase.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(4.0, 3.0)
            });

            // Call
            assessmentSectionBase.NotifyObservers();

            // Assert
            Assert.AreNotEqual(mapData, map.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            view.Data = assessmentSectionBase;

            var assessmentSectionBase2 = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase2.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));
            assessmentSectionBase2.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 1.0),
                new Point2D(4.0, 3.0)
            });

            // Call
            assessmentSectionBase2.NotifyObservers();

            // Assert
            Assert.AreEqual(assessmentSectionBase, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
        }

        [Test]
        public void UpdateObserver_DataNull_MapDataNotUpdated()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            view.Data = assessmentSectionBase;

            MapData dataBeforeUpdate = map.Data;
            view.Data = null;

            assessmentSectionBase.ReferenceLine = new ReferenceLine();
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(34.0, 2.0)
            });

            // Call
            assessmentSectionBase.NotifyObservers();

            // Assert
            Assert.IsNull(view.Data);
            Assert.AreEqual(dataBeforeUpdate, map.Data);
        }

        private class TestAssessmentSectionBase : AssessmentSectionBase
        {
            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }
        }
    }
}