﻿using DelftTools.Shell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Commands;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using GeoAPI.Extensions.Feature;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.UI.Forms;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Commands
{
    [TestFixture]
    public class MapZoomToFeatureCommandTest
    {
        private readonly double envelopeExpansion = 10.0;

        [Test]
        public void NoActionIfMapViewIsNotActive()
        {
            MockRepository mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            IViewList viewList = mocks.Stub<IViewList>();
            MapView mapView = mocks.Stub<MapView>();
            Map map = mocks.StrictMock<Map>();
            IFeature feature = mocks.Stub<IFeature>();

            Expect.Call(gui.ToolWindowViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewList).Repeat.Any();
            mocks.ReplayAll();
            mapView.Map = map;
            new SharpMapGisGuiPlugin
            {
                Gui = gui
            }; // sets private gui field

            viewList.ActiveView = null;

            var command = new MapZoomToFeatureCommand();
            command.Execute(feature);
            mocks.VerifyAll();
        }

        [Test]
        public void ZoomToFeatureGeometryAsPoint()
        {
            //we could put the next 7 lines in the setup, but then we need mocks.BackToRecordAll() to undo the verivied state
            //t00 complicated ;)
            MockRepository mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            IViewList viewList = mocks.Stub<IViewList>();
            MapView mapView = mocks.Stub<MapView>();
            mapView.Stub(mv => mv.MapControl).Return(mocks.Stub<MapControl>()); // to set readonly property
            Map map = mocks.DynamicMock<Map>();
            IFeature feature = mocks.Stub<IFeature>();

            var point = new Point(42, 24);
            var envelope = new Envelope(point.X - envelopeExpansion, point.X + envelopeExpansion, point.Y - envelopeExpansion, point.Y + envelopeExpansion);

            Expect.Call(gui.ToolWindowViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewList).Repeat.Any();

            //the assertion..check we get a zoomtobox for the the feature
            Expect.Call(() => mapView.EnsureVisible(feature));

            mocks.ReplayAll();

            mapView.Map = map;
            new SharpMapGisGuiPlugin
            {
                Gui = gui
            }; // sets private gui field
            viewList.ActiveView = mapView;
            feature.Geometry = point;

            //zoom it!
            var command = new MapZoomToFeatureCommand();
            command.Execute(feature);

            mocks.VerifyAll();
        }

        [Test]
        public void ZoomToFeatureGeometryAsLineString()
        {
            MockRepository mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            IViewList viewList = mocks.Stub<IViewList>();
            MapView mapView = mocks.Stub<MapView>();
            mapView.Stub(mv => mv.MapControl).Return(mocks.Stub<MapControl>()); // to set readonly property
            //mapView.Expect(mv => mv.EnsureVisible(null)).IgnoreArguments().Repeat.Any();
            Map map = mocks.DynamicMock<Map>();
            IFeature feature = mocks.Stub<IFeature>();

            var line = new LineString(new[]
            {
                new Coordinate(0, 0),
                new Coordinate(20, 0)
            });
            var x1 = (line.StartPoint.X < line.EndPoint.X) ? line.StartPoint.X : line.EndPoint.X;
            var x2 = (line.StartPoint.X > line.EndPoint.X) ? line.StartPoint.X : line.EndPoint.X;
            var y1 = (line.StartPoint.Y < line.EndPoint.Y) ? line.StartPoint.Y : line.EndPoint.Y;
            var y2 = (line.StartPoint.Y > line.EndPoint.Y) ? line.StartPoint.Y : line.EndPoint.Y;
            var envelope = new Envelope(x1 - envelopeExpansion, x2 + envelopeExpansion,
                                        y1 - envelopeExpansion, y2 + envelopeExpansion);

            Expect.Call(gui.ToolWindowViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewList).Repeat.Any();

            //the assertion..check we get a zoomtobox for the the feature

            Expect.Call(() => mapView.EnsureVisible(feature));
            mocks.ReplayAll();

            mapView.Map = map;
            new SharpMapGisGuiPlugin
            {
                Gui = gui
            }; // sets private gui field
            viewList.ActiveView = mapView;
            feature.Geometry = line;

            //zoom it!
            var command = new MapZoomToFeatureCommand();
            command.Execute(feature);

            mocks.VerifyAll();
        }
    }
}