using System;
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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismViewTest
    {

        private const int referenceLineLayerIndex = 0;
        private const int surfaceLinesLayerIndex = 1;
        private const int sectionsLayerIndex = 2;
        private const int sectionStartsLayerIndex = 3;
        private const int sectionEndsLayerIndex = 4;
        private const int hydraulicLocationsLayerIndex = 5;

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
        public void DefaultConstructor_Always_AddsMapControlWithEmptyCollectionData()
        {
            // Call
            var view = new PipingFailureMechanismView();

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
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionMock);

            // Call
            view.Data = pipingContext;

            // Assert
            var mapData = map.Data;

            Assert.AreEqual(6, mapData.List.Count);

            var referenceLineData = mapData.List[referenceLineLayerIndex] as FeatureBasedMapData;
            Assert.NotNull(referenceLineData);
            Assert.IsEmpty(referenceLineData.Features);
            Assert.AreEqual("Referentielijn", referenceLineData.Name);

            var surfaceLineData = mapData.List[surfaceLinesLayerIndex] as FeatureBasedMapData;
            Assert.NotNull(surfaceLineData);
            Assert.IsEmpty(surfaceLineData.Features);
            Assert.AreEqual("Profielschematisaties", surfaceLineData.Name);

            var sectionsData = mapData.List[sectionsLayerIndex] as FeatureBasedMapData;
            Assert.NotNull(sectionsData);
            Assert.IsEmpty(sectionsData.Features);
            Assert.AreEqual("Vakindeling", sectionsData.Name);

            var sectionStartsData = mapData.List[sectionStartsLayerIndex] as FeatureBasedMapData;
            Assert.NotNull(sectionStartsData);
            Assert.IsEmpty(sectionStartsData.Features);
            Assert.AreEqual("Vakindeling (startpunten)", sectionStartsData.Name);

            var sectionEndsData = mapData.List[sectionEndsLayerIndex] as FeatureBasedMapData;
            Assert.NotNull(sectionEndsData);
            Assert.IsEmpty(sectionEndsData.Features);
            Assert.AreEqual("Vakindeling (eindpunten)", sectionEndsData.Name);

            var hydraulicLocationsData = mapData.List[hydraulicLocationsLayerIndex] as FeatureBasedMapData;
            Assert.NotNull(hydraulicLocationsData);
            Assert.IsEmpty(hydraulicLocationsData.Features);
            Assert.AreEqual("Hydraulische randvoorwaarden", hydraulicLocationsData.Name);

            mocks.VerifyAll();
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

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1.0, 2.0),
                new Point2D(2.0, 1.0)
            });

            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            assessmentSectionMock.ReferenceLine = referenceLine;
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.Add(new RingtoetsPipingSurfaceLine());
            pipingFailureMechanism.AddSection(new FailureMechanismSection("A", refereceGeometryPoints.Take(2)));
            pipingFailureMechanism.AddSection(new FailureMechanismSection("B", refereceGeometryPoints.Skip(1).Take(2)));
            pipingFailureMechanism.AddSection(new FailureMechanismSection("C", refereceGeometryPoints.Skip(2).Take(2)));

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionMock);

            // Call
            view.Data = pipingContext;

            // Assert
            Assert.AreSame(pipingContext, view.Data);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            var mapData = map.Data;
            Assert.IsNotNull(mapData);

            Assert.AreEqual(6, mapData.List.Count);
            AssertReferenceMapData(assessmentSectionMock.ReferenceLine, mapData.List[referenceLineLayerIndex]);
            AssertSurfacelinesMapData(pipingFailureMechanism.SurfaceLines, mapData.List[surfaceLinesLayerIndex]);
            AssertFailureMechanismSectionsMapData(pipingFailureMechanism.Sections, mapData.List[sectionsLayerIndex]);
            AssertFailureMechanismSectionsStartPointMapData(pipingFailureMechanism.Sections, mapData.List[sectionStartsLayerIndex]);
            AssertFailureMechanismSectionsEndPointMapData(pipingFailureMechanism.Sections, mapData.List[sectionEndsLayerIndex]);
            AssertHydraulicBoundaryLocationsMapData(assessmentSectionMock.HydraulicBoundaryDatabase, mapData.List[hydraulicLocationsLayerIndex]);

            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_HydraulicBoundaryDatabaseUpdated_SetNewMapData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var hydraulicBoundaryDatabase1 = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase1.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1.0, 2.0));

            var assessmentSection = new TestAssessmentSection
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase1
            };

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            view.Data = pipingContext;
            var mapData = map.Data;

            var mapDataElementBeforeUpdate = mapData.List.ElementAt(hydraulicLocationsLayerIndex) as MapPointData;
            var geometryBeforeUpdate = mapDataElementBeforeUpdate.Features.First().MapGeometries.First().Points.First();

            // Precondition
            Assert.AreEqual(geometryBeforeUpdate, new Point2D(1.0, 2.0));

            var hydraulicBoundaryDatabase2 = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase2.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));

            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase2;

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            Assert.AreEqual(mapData, map.Data);
            CollectionAssert.AreEquivalent(mapData.List, map.Data.List);

            var mapDataElementAfterUpdate = map.Data.List.ElementAt(hydraulicLocationsLayerIndex) as MapPointData;
            var geometryAfterUpdate = mapDataElementAfterUpdate.Features.First().MapGeometries.First().Points.First();

            Assert.AreEqual(geometryAfterUpdate, new Point2D(2.0, 3.0));
        }

        [Test]
        public void UpdateObserver_ReferenceLineUpdated_SetNewMapData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

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

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            view.Data = pipingContext;
            var mapData = map.Data;

            var mapDataElementBeforeUpdate = mapData.List.ElementAt(referenceLineLayerIndex) as MapLineData;
            var geometryBeforeUpdate = mapDataElementBeforeUpdate.Features.First().MapGeometries.First().Points;

            // Precondition
            CollectionAssert.AreEquivalent(geometryBeforeUpdate, points);

            assessmentSection.ReferenceLine.SetGeometry(pointsUpdate);

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.IsInstanceOf<MapDataCollection>(map.Data);
            Assert.AreEqual(mapData, map.Data);
            CollectionAssert.AreEquivalent(mapData.List, map.Data.List);

            var mapDataElementAfterUpdate = map.Data.List.ElementAt(referenceLineLayerIndex) as MapLineData;
            var geometryAfterUpdate = mapDataElementAfterUpdate.Features.First().MapGeometries.First().Points;

            CollectionAssert.AreEquivalent(geometryAfterUpdate, pointsUpdate);
        }

        [Test]
        public void UpdateObserver_SurfaceLinesUpdated_SetNewMapData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionMock);

            view.Data = pipingContext;
            var oldSurfaceLineData = (FeatureBasedMapData)map.Data.List.ElementAt(surfaceLinesLayerIndex);

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new Collection<Point3D>
            {
                new Point3D(1,2,3),
                new Point3D(1,2,3)
            });
            pipingFailureMechanism.SurfaceLines.Add(surfaceLine);

            // Call
            pipingFailureMechanism.NotifyObservers();

            // Assert
            var surfaceLineData = (FeatureBasedMapData) map.Data.List.ElementAt(surfaceLinesLayerIndex);
            Assert.AreNotEqual(oldSurfaceLineData, surfaceLineData);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_FailureMechanismSectionsUpdated_SetNewMapData()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSectionMock);

            view.Data = pipingContext;
            var oldSectionsData = (FeatureBasedMapData)map.Data.List.ElementAt(sectionsLayerIndex);
            var oldSectionStartsData = (FeatureBasedMapData)map.Data.List.ElementAt(sectionStartsLayerIndex);
            var oldSectionEndsData = (FeatureBasedMapData)map.Data.List.ElementAt(sectionEndsLayerIndex);

            var section = new FailureMechanismSection(string.Empty, new []
            {
                new Point2D(1,2),
                new Point2D(1,2)
            });
            pipingFailureMechanism.AddSection(section);

            // Call
            pipingFailureMechanism.NotifyObservers();

            // Assert
            var sectionsData = (FeatureBasedMapData) map.Data.List.ElementAt(sectionsLayerIndex);
            var sectionStartsData = (FeatureBasedMapData) map.Data.List.ElementAt(sectionStartsLayerIndex);
            var sectionEndsData = (FeatureBasedMapData)map.Data.List.ElementAt(sectionEndsLayerIndex);
            Assert.AreNotEqual(oldSectionsData, sectionsData);
            Assert.AreNotEqual(oldSectionStartsData, sectionStartsData);
            Assert.AreNotEqual(oldSectionEndsData, sectionEndsData);
            Assert.IsInstanceOf<MapDataCollection>(map.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void UpdateObserver_OtherAssessmentSectionUpdated_MapDataNotUpdated()
        {
            // Setup
            var view = new PipingFailureMechanismView();
            var map = (MapControl)view.Controls[0];

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

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            view.Data = pipingContext;

            var assessmentSection2 = new TestAssessmentSection
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection2.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(2, "test2", 2.0, 3.0));
            assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 1.0),
                new Point2D(4.0, 3.0)
            });

            // Call
            assessmentSection2.NotifyObservers();

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

            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingContext = new PipingFailureMechanismContext(pipingFailureMechanism, assessmentSection);

            view.Data = pipingContext;

            view.Data = null;
            MapData dataBeforeUpdate = map.Data;

            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(new List<Point2D>
            {
                new Point2D(2.0, 5.0),
                new Point2D(34.0, 2.0)
            });

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.AreEqual(dataBeforeUpdate, map.Data);
        }

        [Test]
        public void NotifyObservers_DataUpdatedNotifyObserversOnOldData_NoUpdateInViewData()
        {
            // Setup
            IAssessmentSection oldAssessmentSectionMock = new TestAssessmentSection();
            IAssessmentSection newAssessmentSectionMock = new TestAssessmentSection();

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
        }

        private void AssertReferenceMapData(ReferenceLine referenceLine, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var referenceLineData = (MapLineData)mapData;
            if (referenceLine == null)
            {
                CollectionAssert.IsEmpty(referenceLineData.Features.First().MapGeometries.First().Points);
            }
            else
            {
                CollectionAssert.AreEqual(referenceLine.Points, referenceLineData.Features.First().MapGeometries.First().Points);
            }
            Assert.AreEqual("Referentielijn", mapData.Name);
        }

        private void AssertHydraulicBoundaryLocationsMapData(HydraulicBoundaryDatabase database, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var hydraulicLocationsMapData = (MapPointData)mapData;
            if (database == null)
            {
                CollectionAssert.IsEmpty(hydraulicLocationsMapData.Features.First().MapGeometries.First().Points);
            }
            else
            {
                CollectionAssert.AreEqual(database.Locations.Select(hrp => hrp.Location), hydraulicLocationsMapData.Features.First().MapGeometries.First().Points);
            }
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);
        }

        private void AssertFailureMechanismSectionsMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var sectionsMapLinesData = (MapLineData)mapData;
            var sectionMapLinesFeatures = sectionsMapLinesData.Features.ToArray();
            Assert.AreEqual(1, sectionMapLinesFeatures.Length);

            var geometries = sectionMapLinesFeatures.First().MapGeometries.ToArray();
            var sectionsArray = sections.ToArray();
            Assert.AreEqual(sectionsArray.Length, geometries.Length);

            for (int index = 0; index < sectionsArray.Length; index++)
            {
                var failureMechanismSection = sectionsArray[index];
                CollectionAssert.AreEquivalent(geometries[index].Points, failureMechanismSection.Points);
            }
            Assert.AreEqual("Vakindeling", mapData.Name);
        }

        private void AssertFailureMechanismSectionsStartPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData)mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetStart()), sectionsStartPointData.Features.First().MapGeometries.First().Points);
            Assert.AreEqual("Vakindeling (startpunten)", mapData.Name);
        }

        private void AssertFailureMechanismSectionsEndPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var sectionsStartPointData = (MapPointData)mapData;
            CollectionAssert.AreEqual(sections.Select(s => s.GetLast()), sectionsStartPointData.Features.First().MapGeometries.First().Points);
            Assert.AreEqual("Vakindeling (eindpunten)", mapData.Name);
        }

        private void AssertSurfacelinesMapData(IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            var surfacelinesMapData = (MapLineData)mapData;
            var surfacelineFeatures = surfacelinesMapData.Features.ToArray();
            Assert.AreEqual(1, surfacelineFeatures.Length);
            
            var geometries = surfacelineFeatures.First().MapGeometries.ToArray();
            var surfaceLinesArray = surfaceLines.ToArray();
            Assert.AreEqual(surfaceLinesArray.Length, geometries.Length);

            for (int index = 0; index < surfaceLinesArray.Length; index++)
            {
                var surfaceLine = surfaceLinesArray[index];
                CollectionAssert.AreEquivalent(geometries[index].Points, surfaceLine.Points.Select(p => new Point2D(p.X, p.Y)));
            }
            Assert.AreEqual("Profielschematisaties", mapData.Name);
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
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
                throw new System.NotImplementedException();
            }

            public long StorageId { get; set; }
        }
    }
}
