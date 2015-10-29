using System.Windows.Forms;
using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools;
using NUnit.Framework;

namespace Core.GIS.SharpMap.UI.Tests.Tools
{
    public class CloneableFeature : Feature
    {
        public override object Clone()
        {
            return new CloneableFeature
            {
                Attributes = Attributes, Geometry = (IGeometry) Geometry.Clone()
            };
        }
    }

    [TestFixture]
    public class CurvePointToolTest
    {
        [Test]
        public void CanAddPointToLine()
        {
            var mapControl = new MapControl();

            var vectorLayer = new VectorLayer();
            var layerData = new FeatureCollection();
            vectorLayer.DataSource = layerData;
            layerData.FeatureType = typeof(CloneableFeature);

            layerData.Add(new LineString(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(100, 0)
            }));

            mapControl.Map.Layers.Add(vectorLayer);

            var firstFeature = (IFeature) layerData.Features[0];
            mapControl.SelectTool.Select(firstFeature);

            var curveTool = mapControl.GetToolByType<CurvePointTool>();

            curveTool.IsActive = true;
            var args = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            curveTool.OnMouseMove(new Coordinate(50, 0), new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
            curveTool.OnMouseDown(new Coordinate(50, 0), args);
            curveTool.OnMouseUp(new Coordinate(50, 0), args);

            Assert.AreEqual(3, firstFeature.Geometry.Coordinates.Length);
            Assert.AreEqual(50.0, firstFeature.Geometry.Coordinates[1].X);
        }

        [Test] //not working in UI?
        public void CanRemovePointFromLine()
        {
            var mapControl = new MapControl();

            var vectorLayer = new VectorLayer();
            var layerData = new FeatureCollection();
            vectorLayer.DataSource = layerData;
            layerData.FeatureType = typeof(CloneableFeature);

            layerData.Add(new LineString(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(50, 0),
                new Coordinate(100, 0)
            }));

            mapControl.Map.Layers.Add(vectorLayer);

            var firstFeature = (IFeature) layerData.Features[0];
            mapControl.SelectTool.Select(firstFeature);

            var curveTool = mapControl.GetToolByType<CurvePointTool>();

            curveTool.IsActive = true;
            curveTool.Mode = CurvePointTool.EditMode.Remove;
            var args = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            curveTool.OnMouseMove(new Coordinate(50, 0), new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
            curveTool.OnMouseDown(new Coordinate(50, 0), args); // delete tracker

            Assert.AreEqual(2, firstFeature.Geometry.Coordinates.Length);
            Assert.AreEqual(100.0, firstFeature.Geometry.Coordinates[1].X);
        }

        [Test]
        public void CanAddPointToPolygon()
        {
            var mapControl = new MapControl();

            var vectorLayer = new VectorLayer();
            var layerData = new FeatureCollection();
            vectorLayer.DataSource = layerData;
            layerData.FeatureType = typeof(CloneableFeature);

            layerData.Add(new Polygon(new LinearRing(
                                          new[]
                                          {
                                              new Coordinate(0, 0),
                                              new Coordinate(100, 0),
                                              new Coordinate(100, 100),
                                              new Coordinate(0, 100),
                                              new Coordinate(0, 0),
                                          })));

            mapControl.Map.Layers.Add(vectorLayer);

            var firstFeature = (IFeature) layerData.Features[0];
            mapControl.SelectTool.Select(firstFeature);

            var curveTool = mapControl.GetToolByType<CurvePointTool>();

            curveTool.IsActive = true;
            var args = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            curveTool.OnMouseMove(new Coordinate(50, 0), new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
            curveTool.OnMouseDown(new Coordinate(50, 0), args);
            curveTool.OnMouseUp(new Coordinate(50, 0), args);

            Assert.AreEqual(6, firstFeature.Geometry.Coordinates.Length);
            Assert.AreEqual(50.0, firstFeature.Geometry.Coordinates[1].X);
        }

        [Test] //not working in UI?
        public void CanRemovePointFromPolygon()
        {
            var mapControl = new MapControl();

            var vectorLayer = new VectorLayer();
            var layerData = new FeatureCollection();
            vectorLayer.DataSource = layerData;
            layerData.FeatureType = typeof(CloneableFeature);

            layerData.Add(new Polygon(new LinearRing(
                                          new[]
                                          {
                                              new Coordinate(0, 0),
                                              new Coordinate(100, 0),
                                              new Coordinate(100, 100),
                                              new Coordinate(0, 100),
                                              new Coordinate(0, 0),
                                          })));

            mapControl.Map.Layers.Add(vectorLayer);

            var firstFeature = (IFeature) layerData.Features[0];
            mapControl.SelectTool.Select(firstFeature);

            var curveTool = mapControl.GetToolByType<CurvePointTool>();

            curveTool.IsActive = true;
            curveTool.Mode = CurvePointTool.EditMode.Remove;
            var args = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            curveTool.OnMouseMove(new Coordinate(100, 0), new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
            curveTool.OnMouseDown(new Coordinate(100, 0), args); // delete tracker

            Assert.AreEqual(4, firstFeature.Geometry.Coordinates.Length);
            Assert.AreEqual(100.0, firstFeature.Geometry.Coordinates[1].X);
            Assert.AreEqual(100.0, firstFeature.Geometry.Coordinates[1].Y);
        }

        [Test] //not working in UI?
        public void CanRemoveStartPointFromPolygon()
        {
            var mapControl = new MapControl();

            var vectorLayer = new VectorLayer();
            var layerData = new FeatureCollection();
            vectorLayer.DataSource = layerData;
            layerData.FeatureType = typeof(CloneableFeature);

            layerData.Add(new Polygon(new LinearRing(
                                          new[]
                                          {
                                              new Coordinate(0, 0),
                                              new Coordinate(100, 0),
                                              new Coordinate(100, 100),
                                              new Coordinate(0, 100),
                                              new Coordinate(0, 0),
                                          })));

            mapControl.Map.Layers.Add(vectorLayer);

            var firstFeature = (IFeature) layerData.Features[0];
            mapControl.SelectTool.Select(firstFeature);

            var curveTool = mapControl.GetToolByType<CurvePointTool>();

            curveTool.IsActive = true;
            curveTool.Mode = CurvePointTool.EditMode.Remove;
            var args = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            curveTool.OnMouseMove(new Coordinate(0, 0), new MouseEventArgs(MouseButtons.None, 1, 0, 0, 0));
            curveTool.OnMouseDown(new Coordinate(0, 0), args); // delete tracker

            Assert.AreEqual(4, firstFeature.Geometry.Coordinates.Length);
            Assert.AreEqual(100.0, firstFeature.Geometry.Coordinates[1].X);
            Assert.AreEqual(100.0, firstFeature.Geometry.Coordinates[1].Y);
        }
    }
}