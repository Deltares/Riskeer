using System.Drawing;
using System.Linq;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Editors.Interactors;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.UI.Forms;
using SharpMapTestUtils.TestClasses;
using Point = GisSharpBlog.NetTopologySuite.Geometries.Point;

namespace SharpMap.Tests.Editors
{
    [TestFixture]
    public class PointInteractorTest
    {
        private static SampleFeature sampleFeature;

        [SetUp]
        public void SetUp()
        {
            sampleFeature = new SampleFeature { Geometry = new Point(0, 0) };
        }

        private static VectorStyle GetStyle(Pen pen)
        {
            return new VectorStyle
            {
                Fill = Brushes.AntiqueWhite,
                Line = pen,
                EnableOutline = true,
                Outline = Pens.Black,
                Symbol = new Bitmap(10, 10)
            };
        }
        [Test]
        public void PointMutatorCreationWithoutMapControlTest()
        {
            var pointEditor = new PointInteractor(null, sampleFeature, GetStyle(Pens.Red), null);
            
            Assert.AreEqual(null, pointEditor.TargetFeature);
            Assert.AreNotEqual(null, pointEditor.SourceFeature);
            
            // The tracker has focus by default; is this ok
            var trackers = pointEditor.Trackers.Where(t => t.Selected);
            Assert.AreEqual(1, trackers.Count());

            var tracker = pointEditor.Trackers[0];
            Assert.AreNotEqual(null, tracker);

            pointEditor.Start();
            Assert.AreNotEqual(null, pointEditor.TargetFeature);
            Assert.AreNotEqual(pointEditor.SourceFeature, pointEditor.TargetFeature);
        }

        [Test]
        public void SelectionTest()
        {
            var pointEditor = new PointInteractor(null, sampleFeature, GetStyle(Pens.Red), null);
            var tracker = pointEditor.Trackers[0];
            
            // The tracker has focus by default; is this ok
            Assert.AreEqual(true, tracker.Selected);
            
            pointEditor.SetTrackerSelection(tracker, false);
            Assert.AreEqual(false, tracker.Selected);
        }

        [Test]
        public void PointMutatorCreationWithMapControlTest()
        {
            var mapControl = new MapControl {Map = {Size = new Size(1000, 1000)}};
            var pointEditor = new PointInteractor(new VectorLayer { Map = mapControl.Map }, sampleFeature, GetStyle(Pens.Red), null);
            
            Assert.AreEqual(null, pointEditor.TargetFeature);
            Assert.AreNotEqual(null, pointEditor.SourceFeature);

            var tracker = pointEditor.GetTrackerAtCoordinate(new Coordinate(0, 0));
            Assert.AreNotEqual(null, tracker);

            pointEditor.Start();
            pointEditor.MoveTracker(tracker, 5.0, 5.0);
            pointEditor.Stop();

            Assert.AreEqual(5.0, tracker.Geometry.Coordinates[0].X);
            Assert.AreEqual(5.0, tracker.Geometry.Coordinates[0].Y);
            Assert.AreEqual(5.0, sampleFeature.Geometry.Coordinates[0].X);
            Assert.AreEqual(5.0, sampleFeature.Geometry.Coordinates[0].Y);
        }
    }
}