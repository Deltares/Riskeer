using System.Drawing;
using System.Linq;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Editors.FallOff;
using Core.GIS.SharpMap.Editors.Interactors;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Styles;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMapTestUtil.TestClasses;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Editors
{
    /// <summary>
    /// For more basec test of FeatureMutator please refer to PointInteractorTest
    /// </summary>
    [TestFixture]
    public class LineStringInteractorTest
    {
        private static SampleFeature sampleFeature;

        [SetUp]
        public void SetUp()
        {
            sampleFeature = new SampleFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(0, 0),
                    new Coordinate(10, 0),
                    new Coordinate(20, 0),
                    new Coordinate(30, 0),
                    new Coordinate(40, 0)
                })
            };
        }

        [Test]
        public void LineStringEditorCreationWithoutMapControlTest()
        {
            var mapControl = new MapControl();
            mapControl.Map.ZoomToFit(new Envelope(new Coordinate(0, 0), new Coordinate(1000, 1000)));

            var lineStringEditor = new LineStringInteractor(null, sampleFeature, GetStyle(), null);
            Assert.AreEqual(null, lineStringEditor.TargetFeature);
            Assert.AreNotEqual(null, lineStringEditor.SourceFeature);

            // There are no default focused trackers
            var trackers = lineStringEditor.Trackers.Where(t => t.Selected);
            Assert.AreEqual(0, trackers.Count());

            TrackerFeature tracker = lineStringEditor.Trackers[2];
            Assert.AreNotEqual(null, tracker);
            Assert.AreEqual(20.0, tracker.Geometry.Coordinates[0].X);
            Assert.AreEqual(0.0, tracker.Geometry.Coordinates[0].Y);

            lineStringEditor.Start();
            lineStringEditor.SetTrackerSelection(tracker, true);

            trackers = lineStringEditor.Trackers.Where(t => t.Selected);
            Assert.AreEqual(1, trackers.Count());
            Assert.AreNotEqual(null, lineStringEditor.TargetFeature);
            Assert.AreNotEqual(lineStringEditor.SourceFeature, lineStringEditor.TargetFeature);
        }

        [Test]
        public void MoveTrackerWithSelection()
        {
            var lineStringEditor = new LineStringInteractor(null, sampleFeature, GetStyle(), null);
            TrackerFeature tracker = lineStringEditor.Trackers[2]; // 20,0

            tracker.Selected = true;
            lineStringEditor.Start();
            lineStringEditor.MoveTracker(tracker, 0, 5);

            Assert.AreEqual(20, tracker.Geometry.Coordinates[0].X);
            Assert.AreEqual(5, tracker.Geometry.Coordinates[0].Y);

            // check if changed coordinate is NOT set to sampleFeature 
            Assert.AreEqual(20, sampleFeature.Geometry.Coordinates[2].X);
            Assert.AreEqual(0, sampleFeature.Geometry.Coordinates[2].Y);

            // todo redesign .Stop
            //lineStringMutator.Stop();
            //// check if changed coordinate IS set to sampleFeature 
            //Assert.AreEqual(20, sampleFeature.Geometry.Coordinates[2].X);
            //Assert.AreEqual(5, sampleFeature.Geometry.Coordinates[2].Y);
        }

        [Test]
        public void MoveTestWithLinearFalOffPolicy()
        {
            var lineStringEditor = new LineStringInteractor(null, sampleFeature, GetStyle(), null);
            lineStringEditor.FallOffPolicy = new LinearFallOffPolicy();
            var tracker = lineStringEditor.Trackers[2]; // 20,0
            tracker.Selected = true;
            lineStringEditor.Start();
            lineStringEditor.MoveTracker(tracker, 0, 5);

            // todo redesign .Stop
            //lineStringMutator.Stop();
            //Assert.AreEqual(20, sampleFeature.Geometry.Coordinates[2].X);
            //Assert.AreEqual(5, sampleFeature.Geometry.Coordinates[2].Y);
        }

        [Test]
        public void SelectionTestViaCoordinates()
        {
            var mapControl = new MapControl
            {
                Map =
                {
                    Size = new Size(1000, 1000)
                }
            };
            var lineStringEditor = new LineStringInteractor(new VectorLayer
            {
                Map = mapControl.Map
            }, sampleFeature, GetStyle(), null);

            var trackerFeatureAtCoordinate = lineStringEditor.GetTrackerAtCoordinate(new Coordinate(20, 0));
            var trackerFeatureAtIndex = lineStringEditor.Trackers[2];
            Assert.AreEqual(trackerFeatureAtIndex, trackerFeatureAtCoordinate);

            trackerFeatureAtCoordinate.Selected = true;
            Assert.AreEqual(1, lineStringEditor.Trackers.Count(t => t.Selected));
        }

        [Test]
        public void MultipleSelectionTestViaCoordinatesNullFallOff()
        {
            // todo write
            var mapControl = new MapControl
            {
                Map =
                {
                    Size = new Size(1000, 1000)
                }
            };
            var lineStringEditor = new LineStringInteractor(new VectorLayer
            {
                Map = mapControl.Map
            }, sampleFeature, GetStyle(), null);

            var trackerFeatureAtCoordinate10 = lineStringEditor.GetTrackerAtCoordinate(new Coordinate(10, 0));
            trackerFeatureAtCoordinate10.Selected = true;

            var trackerFeatureAtCoordinate30 = lineStringEditor.GetTrackerAtCoordinate(new Coordinate(30, 0));
            trackerFeatureAtCoordinate30.Selected = true;

            Assert.AreEqual(2, lineStringEditor.Trackers.Count(t => t.Selected));

            lineStringEditor.Start();
            lineStringEditor.MoveTracker(trackerFeatureAtCoordinate30, 0, 5);

            // only tracker at 10 is moved
            Assert.AreEqual(5.0, trackerFeatureAtCoordinate30.Geometry.Coordinates[0].Y);
            Assert.AreEqual(0.0, trackerFeatureAtCoordinate10.Geometry.Coordinates[0].Y);

            lineStringEditor.Stop();
        }

        [Test]
        public void MultipleSelectionTestViaCoordinatesNoFallOffPolicy()
        {
            // todo write
            var mapControl = new MapControl
            {
                Map =
                {
                    Size = new Size(1000, 1000)
                }
            };
            var lineStringEditor = new LineStringInteractor(new VectorLayer
            {
                Map = mapControl.Map
            }, sampleFeature, GetStyle(), null);

            var trackerFeatureAtCoordinate10 = lineStringEditor.GetTrackerAtCoordinate(new Coordinate(10, 0));
            trackerFeatureAtCoordinate10.Selected = true;

            var trackerFeatureAtCoordinate30 = lineStringEditor.GetTrackerAtCoordinate(new Coordinate(30, 0));
            trackerFeatureAtCoordinate30.Selected = true;
            lineStringEditor.FallOffPolicy = new NoFallOffPolicy();
            lineStringEditor.Start();
            lineStringEditor.MoveTracker(trackerFeatureAtCoordinate30, 0, 5);

            // both tracker at 10 and 30 is moved
            Assert.AreEqual(5.0, trackerFeatureAtCoordinate30.Geometry.Coordinates[0].Y);
            Assert.AreEqual(5.0, trackerFeatureAtCoordinate10.Geometry.Coordinates[0].Y);
            lineStringEditor.Stop();
        }

        [Test]
        public void AllTrackerTest()
        {
            var mapControl = new MapControl
            {
                Map =
                {
                    Size = new Size(1000, 1000)
                }
            };
            var lineStringEditor = new LineStringInteractor(new VectorLayer
            {
                Map = mapControl.Map
            }, sampleFeature, GetStyle(), null);

            var trackerFeature = lineStringEditor.GetTrackerAtCoordinate(new Coordinate(15, 0));
            lineStringEditor.Start();
            lineStringEditor.MoveTracker(trackerFeature, 0.0, 5.0);

            Assert.AreEqual(5.0, lineStringEditor.TargetFeature.Geometry.Coordinates[0].Y);
            Assert.AreEqual(5.0, lineStringEditor.TargetFeature.Geometry.Coordinates[1].Y);
            Assert.AreEqual(5.0, lineStringEditor.TargetFeature.Geometry.Coordinates[2].Y);
            Assert.AreEqual(5.0, lineStringEditor.TargetFeature.Geometry.Coordinates[3].Y);
            Assert.AreEqual(5.0, lineStringEditor.TargetFeature.Geometry.Coordinates[4].Y);

            lineStringEditor.Stop();
        }

        private static VectorStyle GetStyle()
        {
            var style = new VectorStyle
            {
                Fill = Brushes.AntiqueWhite,
                Line = Pens.Red,
                EnableOutline = true,
                Outline = Pens.Black,
                Symbol = new Bitmap(10, 10)
            };

            return style;
        }
    }
}