using System;
using System.IO;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.TestUtils;
using Core.Common.Utils.Reflection;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.SharpMapGis.Test.Forms
{
    [TestFixture]
    public class MapLegendViewTest
    {
        private readonly MockRepository mocks = new MockRepository();

        [Test]
        public void Init()
        {
            var map = new Map();
            Form form = CreateFormWithMapLegendView(map);
            form.Dispose();
        }

        [Test]
        public void RemoveFromMapLegendViewDisposesLayerResources()
        {
            var mapLegendView = CreateMapLegendView();
            var map = new Map();
            mapLegendView.Map = map;

            string path = TestHelper.GetDataDir() + "rivers.shp";

            var shapeFile = mocks.StrictMock<ShapeFile>(path);
            shapeFile.Expect(sf => sf.GetExtents()).Return(null).Repeat.Any();
            shapeFile.Expect(sf => sf.FeaturesChanged += null).IgnoreArguments().Repeat.Once();
            shapeFile.Expect(sf => sf.FeaturesChanged -= null).IgnoreArguments().Repeat.Twice();
            shapeFile.Expect(sf => sf.CoordinateSystemChanged += null).IgnoreArguments().Repeat.Once();
            shapeFile.Expect(sf => sf.CoordinateSystemChanged -= null).IgnoreArguments().Repeat.Twice();
            shapeFile.Expect(sf => sf.AddNewFeatureFromGeometryDelegate = null).IgnoreArguments().Repeat.Once();
            shapeFile.Expect(sf => sf.AddNewFeatureFromGeometryDelegate).Return(null).Repeat.Once();
            shapeFile.Expect(sf => sf.Dispose()).Repeat.Once(); //this is what we are checking
            shapeFile.Replay();

            var vectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile);
            map.Layers.Add(vectorLayer);

            TypeUtils.CallPrivateMethod<MapLegendView>(mapLegendView, "RemoveLayer", vectorLayer);

            shapeFile.VerifyAllExpectations();
        }

        [Test]
        public void HideAllLayersButThisShouldWorkInMap()
        {
            //should work for layers on Map level
            var map = new Map();
            var layer1 = new VectorLayer();
            var layer2 = new VectorLayer();
            map.Layers.Add(layer1);
            map.Layers.Add(layer2);

            MapLegendView.HideAllLayersButThisOne(layer1, map);

            Assert.IsTrue(layer1.Visible);
            Assert.IsFalse(layer2.Visible);
        }

        [Test]
        public void HideAllLayersButThisShouldWorkInGroupLayer()
        {
            var map = new Map();
            var layer1 = new VectorLayer();
            var groupLayer = new GroupLayer();
            var nestedLayer1 = new VectorLayer();
            var nestedLayer2 = new VectorLayer();
            groupLayer.Layers.Add(nestedLayer1);
            groupLayer.Layers.Add(nestedLayer2);

            map.Layers.Add(layer1);
            map.Layers.Add(groupLayer);

            MapLegendView.HideAllLayersButThisOne(nestedLayer1, map);
            Assert.IsTrue(layer1.Visible);
            Assert.IsTrue(groupLayer.Visible);
            //only the other layer in the group should be turned off
            Assert.IsFalse(nestedLayer2.Visible);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Layer kees not part of map map1")]
        public void HideAllLayersButThisOneThrowsExceptionIfLayerIsNotInMap()
        {
            var map = new Map
            {
                Name = "map1"
            };
            var layer1 = new VectorLayer
            {
                Name = "kees"
            };
            MapLegendView.HideAllLayersButThisOne(layer1, map);
        }

        private MapLegendView CreateMapLegendView()
        {
            var project = new Project();
            var gui = mocks.Stub<IGui>();
            var applicationCore = new ApplicationCore();

            //create stubbed version of plugingui
            SharpMapGisGuiPlugin guiPlugin = new SharpMapGisGuiPlugin();

            IGisGuiService gisService = mocks.Stub<IGisGuiService>();
            guiPlugin.GisGuiService = gisService;

            Expect.Call(gui.ApplicationCore).Return(applicationCore).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new[]
            {
                guiPlugin
            }).Repeat.Any();
            Expect.Call(gui.ToolWindowViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(mocks.Stub<IViewList>()).Repeat.Any();

            gui.Project = project;

            mocks.ReplayAll();

            guiPlugin.Gui = gui;

            guiPlugin.Activate();

            var mapLegendView = guiPlugin.MapLegendView;
            mapLegendView.Dock = DockStyle.Fill;

            return mapLegendView;
        }

        private Form CreateFormWithMapLegendView(Map map)
        {
            var mapLegendView = CreateMapLegendView();
            mapLegendView.Map = map;
            mapLegendView.Dock = DockStyle.Fill;
            Form form = new Form();
            form.Controls.Add(mapLegendView);
            return form;
        }
    }
}