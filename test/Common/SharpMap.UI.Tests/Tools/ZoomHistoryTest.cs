using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.UI.Forms;
using SharpMap.UI.Tools.Zooming;

namespace SharpMap.UI.Tests.Tools
{
    [TestFixture]
    public class ZoomHistoryTest
    {
        

        [Test]
        public void ZoomHistory()
        {
            const double cToleranceZoom = 0.001;

            // Setup test environment
            var mapControl = new MapControl();
            var zoomHistoryToolMapTool = new ZoomHistoryTool();
            mapControl.Tools.Add(zoomHistoryToolMapTool);

            // First rendering is ignored
            mapControl.Map.Zoom = 1.0;
            mapControl.Map.Center = new Coordinate(0.1, 0.2);
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(0, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);

            // First recorded rendering
            mapControl.Map.Zoom = 1.1;
            mapControl.Map.Center = new Coordinate(1.2, 1.3);
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(1, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);

            // Undo and redo the zoom
            zoomHistoryToolMapTool.PreviousZoomState();
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(0, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(1, zoomHistoryToolMapTool.RedoCount);
            Assert.AreEqual(1.0, mapControl.Map.Zoom, cToleranceZoom);
            Assert.AreEqual(0.1, mapControl.Map.Center.X, cToleranceZoom);
            Assert.AreEqual(0.2, mapControl.Map.Center.Y, cToleranceZoom);
          
            zoomHistoryToolMapTool.NextZoomState();
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(1, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);
            Assert.AreEqual(1.1, mapControl.Map.Zoom, cToleranceZoom);
            Assert.AreEqual(1.2, mapControl.Map.Center.X, cToleranceZoom);
            Assert.AreEqual(1.3, mapControl.Map.Center.Y, cToleranceZoom);

            // Second recorded rendering
            mapControl.Map.Zoom = 2.1;
            mapControl.Map.Center = new Coordinate(2.2, 2.3);
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(2, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);

            // Third recorded rendering
            mapControl.Map.Zoom = 3.1;
            mapControl.Map.Center = new Coordinate(3.2, 3.3);
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(3, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);

            // Fourth recorded rendering
            mapControl.Map.Zoom = 4.1;
            mapControl.Map.Center = new Coordinate(4.2, 4.3);
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(4, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);

            // Undo 2 times and redo 2 times
            Assert.AreEqual(4, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(0, zoomHistoryToolMapTool.RedoCount);

            zoomHistoryToolMapTool.PreviousZoomState();
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(3.1, mapControl.Map.Zoom, cToleranceZoom);
            Assert.AreEqual(3.2, mapControl.Map.Center.X, cToleranceZoom);
            Assert.AreEqual(3.3, mapControl.Map.Center.Y, cToleranceZoom);
            Assert.AreEqual(3, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(1, zoomHistoryToolMapTool.RedoCount);

            zoomHistoryToolMapTool.PreviousZoomState();
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(2.1, mapControl.Map.Zoom, cToleranceZoom);
            Assert.AreEqual(2.2, mapControl.Map.Center.X, cToleranceZoom);
            Assert.AreEqual(2.3, mapControl.Map.Center.Y, cToleranceZoom);
            Assert.AreEqual(2, zoomHistoryToolMapTool.UndoCount);
            Assert.AreEqual(2, zoomHistoryToolMapTool.RedoCount);
            
            zoomHistoryToolMapTool.NextZoomState();
            zoomHistoryToolMapTool.MapRendered(mapControl.Map);
            Assert.AreEqual(3.1, mapControl.Map.Zoom, cToleranceZoom);
            Assert.AreEqual(3.2, mapControl.Map.Center.X, cToleranceZoom);
            Assert.AreEqual(3.3, mapControl.Map.Center.Y, cToleranceZoom);
        }
    }
}
