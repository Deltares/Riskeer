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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
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
        }

        [Test]
        public void DefaultConstructor_Always_AddsBaseMap()
        {
            // Call
            var view = new AssessmentSectionView();

            // Assert
            Assert.AreEqual(1, view.Controls.Count);
            object mapObject = view.Controls[0];
            Assert.IsInstanceOf<BaseMap>(mapObject);

            var map = (BaseMap) mapObject;
            Assert.AreEqual(DockStyle.Fill, map.Dock);
            Assert.NotNull(view.Map);
        }

        [Test]
        public void Data_ReferenceLineNull_NoLineDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            // Call
            view.Data = assessmentSectionBase;

            // Assert
            var mapData = (MapDataCollection) map.Data;

            Assert.AreEqual(1, mapData.List.Count);
            Assert.IsNotInstanceOf<MapLineData>(mapData.List[0]);
        }

        [Test]
        public void Data_HydraulicBoundaryDatabaseNull_NoPointDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            // Call
            view.Data = assessmentSectionBase;

            // Assert
            var mapData = (MapDataCollection)map.Data;

            Assert.AreEqual(1, mapData.List.Count);
            Assert.IsNotInstanceOf<MapPointData>(mapData.List[0]);
        }

        [Test]
        public void Data_SetToNull_BaseMapNoFeatures()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap) view.Controls[0];

            // Call
            TestDelegate testDelegate = () => view.Data = null;

            // Assert
            Assert.DoesNotThrow(testDelegate);
            Assert.IsNull(map.Data);
        }

        [Test]
        public void Data_SetToMapPointData_MapDataSet()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap) view.Controls[0];
            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            // Call
            view.Data = assessmentSectionBase;

            // Assert
            Assert.AreSame(assessmentSectionBase, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            
            mocks.ReplayAll();

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.Attach(observer);

            view.Data = assessmentSectionBase;
            var mapData = map.Data;

            assessmentSectionBase.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

            // Call
            assessmentSectionBase.NotifyObservers();

            // Assert
            Assert.AreNotEqual(mapData, map.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.Attach(observer);

            view.Data = assessmentSectionBase;

            var assessmentSectionBase2 = new AssessmentSectionBaseTestClass
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase2.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));
            assessmentSectionBase2.Attach(observer);

            // Call
            assessmentSectionBase.NotifyObservers();

            // Assert
            Assert.AreEqual(assessmentSectionBase, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_DataNull_MapDataNotUpdated()
        {
            // Setup
            var view = new AssessmentSectionView();
            var map = (BaseMap)view.Controls[0];
            
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSectionBase = new AssessmentSectionBaseTestClass
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            view.Data = assessmentSectionBase;
            
            assessmentSectionBase.Attach(observer);

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
            mocks.VerifyAll();
        }

        private class AssessmentSectionBaseTestClass : AssessmentSectionBase
        {
            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }
        }
    }
}