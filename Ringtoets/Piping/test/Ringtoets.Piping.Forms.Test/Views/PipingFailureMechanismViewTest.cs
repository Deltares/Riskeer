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
        public void DefaultConstructor_Always_AddsMapControlWithoutData()
        {
            // Call
            var view = new PipingFailureMechanismView();

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
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase();

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            // Call
            view.Data = pipingContext;

            // Assert
            var mapData = (MapDataCollection)map.Data;

            Assert.AreEqual(6, mapData.List.Count);
            AssertSurfacelinesMapData(pipingFailureMechanism.SurfaceLines, mapData.List[0]);
            AssertFailureMechanismSectionsMapData(pipingFailureMechanism.Sections, mapData.List[1]);
            AssertFailureMechanismSectionsStartPointMapData(pipingFailureMechanism.Sections, mapData.List[2]);
            AssertFailureMechanismSectionsEndPointMapData(pipingFailureMechanism.Sections, mapData.List[3]);
            AssertHydraulicBoundaryLocationsMapData(assessmentSectionBase.HydraulicBoundaryDatabase, mapData.List[4]);
            AssertReferenceMapData(assessmentSectionBase.ReferenceLine, mapData.List[5]);
        }

        [Test]
        public void Data_SetMapData_MapDataSet()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var refereceGeometryPoints = new[]
            {
                new Point2D(0.0, 0.0), 
                new Point2D(2.0, 0.0), 
                new Point2D(4.0, 4.0), 
                new Point2D(6.0, 4.0)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(refereceGeometryPoints);

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = referenceLine
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));
            assessmentSectionBase.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.Add(new RingtoetsPipingSurfaceLine());
            pipingFailureMechanism.AddSection(new FailureMechanismSection("A", refereceGeometryPoints.Take(2)));
            pipingFailureMechanism.AddSection(new FailureMechanismSection("B", refereceGeometryPoints.Skip(1).Take(2)));
            pipingFailureMechanism.AddSection(new FailureMechanismSection("C", refereceGeometryPoints.Skip(2).Take(2)));

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            // Call
            view.Data = pipingContext;

            // Assert
            Assert.AreSame(pipingContext, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            var mapData = map.Data as MapDataCollection;
            Assert.IsNotNull(mapData);

            Assert.AreEqual(6, mapData.List.Count);
            AssertSurfacelinesMapData(pipingFailureMechanism.SurfaceLines, mapData.List[0]);
            AssertFailureMechanismSectionsMapData(pipingFailureMechanism.Sections, mapData.List[1]);
            AssertFailureMechanismSectionsStartPointMapData(pipingFailureMechanism.Sections, mapData.List[2]);
            AssertFailureMechanismSectionsEndPointMapData(pipingFailureMechanism.Sections, mapData.List[3]);
            AssertHydraulicBoundaryLocationsMapData(assessmentSectionBase.HydraulicBoundaryDatabase, mapData.List[4]);
            AssertReferenceMapData(assessmentSectionBase.ReferenceLine, mapData.List[5]);
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            assessmentSectionBase.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

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
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
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
        }

        [Test]
        public void UpdateObserver_SurfaceLinesUpdated_SetNewMapDataData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var assessmentSectionBase = new TestAssessmentSectionBase();
            var pipingFailureMechanism = new PipingFailureMechanism();

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
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            var view = new PipingFailureMechanismView();
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

            // Call
            assessmentSectionBase2.NotifyObservers();

            // Assert
            Assert.AreEqual(pipingContext, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
        }

        [Test]
        public void UpdateObserver_DataNull_MapDataNotUpdated()
        {
            // Setup
            var view = new PipingFailureMechanismView();
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

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionBase);

            view.Data = pipingContext;

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
            var map = (MapControl)view.Controls[0];

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

        private void AssertReferenceMapData(ReferenceLine referenceLine, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var referenceLineData = (MapLineData)mapData;
            if (referenceLine == null)
            {
                CollectionAssert.IsEmpty(referenceLineData.Points);
            }
            else
            {
                CollectionAssert.AreEqual(referenceLine.Points, referenceLineData.Points);
            }
            Assert.AreEqual("Referentielijn", mapData.Name);
        }

        private void AssertHydraulicBoundaryLocationsMapData(HydraulicBoundaryDatabase database, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var hydraulicLocationsMapData = (MapPointData)mapData;
            if (database == null)
            {
                CollectionAssert.IsEmpty(hydraulicLocationsMapData.Points);
            }
            else
            {
                CollectionAssert.AreEqual(database.Locations.Select(hrp => hrp.Location), hydraulicLocationsMapData.Points);
            }
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);
        }

        private void AssertFailureMechanismSectionsMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapMultiLineData>(mapData);
            var sectionsMapLinesData = (MapMultiLineData)mapData;
            foreach (var failureMechanismSection in sections)
            {
                CollectionAssert.Contains(sectionsMapLinesData.Lines, failureMechanismSection.Points);
            }
            Assert.AreEqual("Vakindeling", mapData.Name);
        }

        private void AssertFailureMechanismSectionsStartPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData)mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetStart()), sectionsStartPointData.Points);
            Assert.AreEqual("Vakindeling (startpunten)", mapData.Name);
        }

        private void AssertFailureMechanismSectionsEndPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData)mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetLast()), sectionsStartPointData.Points);
            Assert.AreEqual("Vakindeling (eindpunten)", mapData.Name);
        }

        private void AssertSurfacelinesMapData(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, MapData mapData)
        {
            Assert.IsInstanceOf<MapMultiLineData>(mapData);
            var surfacelinesMapData = (MapMultiLineData)mapData;
            foreach (var surfaceLine in surfaceLines)
            {
                CollectionAssert.Contains(surfacelinesMapData.Lines, surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)));
            }
            Assert.AreEqual("Profielmetingen", mapData.Name);
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
