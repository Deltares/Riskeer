using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var view = new PipingFailureMechanismView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsInstanceOf<IObserver>(view);
            Assert.IsNotNull(view.Map);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void DefaultConstructor_Always_AddsBaseMapWithoutData()
        {
            // Call
            var view = new PipingFailureMechanismView();

            // Assert
            Assert.AreEqual(1, view.Controls.Count);
            var mapObject = view.Controls[0] as BaseMap;
            Assert.NotNull(mapObject);
            Assert.AreEqual(DockStyle.Fill, mapObject.Dock);
            Assert.IsNull(mapObject.Data);
        }

        [Test]
        public void Data_EmptyAssessmentSection_NoMapDataSet()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase();

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            // Call
            view.Data = pipingContext;

            // Assert
            var mapData = (MapDataCollection)map.Data;

            Assert.AreEqual(0, mapData.List.Count);
        }

        [Test]
        public void Data_SetMapData_MapDataSet()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];
            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine(),
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.Add(new RingtoetsPipingSurfaceLine());

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            // Call
            view.Data = pipingContext;

            // Assert
            Assert.AreSame(pipingContext, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            var mapData = map.Data as MapDataCollection;
            Assert.IsNotNull(mapData);
            Assert.AreEqual(1, mapData.List.Count(md => md is MapPointData));
            Assert.AreEqual(1, mapData.List.Count(md => md is MapLineData));
            Assert.AreEqual(1, mapData.List.Count(md => md is MapMultiLineData));
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.Attach(observer);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            view.Data = pipingContext;
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
        public void UpdateObserver_ReferenceLineUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });
            assessmentSectionBase.Attach(observer);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            view.Data = pipingContext;
            var mapData = map.Data;

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
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_SurfaceLinesUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSectionBase = new TestAssessmentSectionBase();
            var pipingFailureMechanism = new PipingFailureMechanism();
            ((IObservable)pipingFailureMechanism.SurfaceLines).Attach(observer);

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            view.Data = pipingContext;
            var mapData = map.Data;

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new Collection<Point3D>
            {
                new Point3D(1,2,3),
                new Point3D(1,2,3)
            });
            pipingFailureMechanism.SurfaceLines.Add(surfaceLine);

            // Call
            ((IObservable)pipingFailureMechanism.SurfaceLines).NotifyObservers();

            // Assert
            Assert.AreNotEqual(mapData, map.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

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
            assessmentSectionBase.Attach(observer);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            view.Data = pipingContext;

            var assessmentSectionBase2 = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSectionBase2.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 1.0),
                new Point2D(4.0, 3.0)
            });
            assessmentSectionBase2.Attach(observer);

            // Call
            assessmentSectionBase2.NotifyObservers();

            // Assert
            Assert.AreEqual(pipingContext, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_DataNull_MapDataNotUpdated()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

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

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            view.Data = pipingContext;

            assessmentSectionBase.Attach(observer);

            view.Data = null;
            MapData dataBeforeUpdate = map.Data;

            assessmentSectionBase.ReferenceLine = new ReferenceLine();
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(34.0, 2.0)
            });

            // Call
            assessmentSectionBase.NotifyObservers();

            // Assert
            Assert.AreEqual(dataBeforeUpdate, map.Data);
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            var mocks = new MockRepository();
            AssessmentSectionBase oldAssessmentSectionMock = mocks.Stub<AssessmentSectionBase>();
            AssessmentSectionBase newAssessmentSectionMock = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            newAssessmentSectionMock.ReferenceLine = new ReferenceLine();
            newAssessmentSectionMock.ReferenceLine.SetGeometry(new[]
            {
                    new Point2D(2,4),
                    new Point2D(3,4)
            });

            var oldPipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), oldAssessmentSectionMock);
            var newPipingFailureMechanismContext = new PipingFailureMechanismContext(new PipingFailureMechanism(), newAssessmentSectionMock);
            var view = new PipingFailureMechanismView();
            var map = (BaseMap)view.Controls[0];

            view.Data = oldPipingFailureMechanismContext;
            view.Data = newPipingFailureMechanismContext;
            MapData dataBeforeUpdate = map.Data;

            newAssessmentSectionMock.ReferenceLine.SetGeometry(Enumerable.Empty<Point2D>());

            // Call
            oldAssessmentSectionMock.NotifyObservers();

            // Assert
            Assert.AreEqual(dataBeforeUpdate, map.Data);

            mocks.VerifyAll();
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
