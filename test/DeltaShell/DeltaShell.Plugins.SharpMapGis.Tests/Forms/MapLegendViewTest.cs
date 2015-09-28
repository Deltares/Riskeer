using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils.Reflection;
using DeltaShell.Plugins.SharpMapGis.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using GisSharpBlog.NetTopologySuite.Index.Bintree;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class MapLegendViewTest
    {
        private readonly MockRepository mocks = new MockRepository();
        const string DataPath = @"..\..\..\..\..\test-data\DeltaShell\DeltaShell.Plugins.SharpMapGis.Tests\";

        [Test]
        public void Init()
        {
            var map = new Map();
            Form form = CreateFormWithMapLegendView(map);
            form.Dispose();
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void RemoveFromMapLegendViewDisposesLayerResources()
        {
            var mapLegendView = CreateMapLegendView();
            var map = new Map();
            mapLegendView.Map = map;
            
            const string path = DataPath + "rivers.shp";
            
            var shapeFile = mocks.StrictMock<ShapeFile>();
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
        [Category(TestCategory.WindowsForms)]
        public void ShowMapLegendView()
        {
            var map = new Map();
            Form form = CreateFormWithMapLegendView(map);

            const string path = DataPath + "rivers.shp";
            var shapeFile = new ShapeFile(path, false);

            var pen1 = new Pen(new SolidBrush(Color.Yellow), 3);
            var pen2 = new Pen(new SolidBrush(Color.Red), 5);

            VectorStyle style = GetStyle(pen1);
            VectorStyle style2 = GetStyle(pen2);
            var interval = new Interval(11, 12.1);
            var quantityTheme = new QuantityTheme("RIVERSEGME", style);
            quantityTheme.AddStyle(style, interval);
            quantityTheme.AddStyle(style2, interval);

            var visibleVectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile)
                {
                    ShowInTreeView = true,
                    Theme = quantityTheme
                };

            var invisibleVectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile)
                {
                    ShowInTreeView = false,
                    Theme = quantityTheme
                };

            map.Layers.Add(visibleVectorLayer);
            map.Layers.Add(invisibleVectorLayer);

            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void MapLegendViewWithOneStyle()
        {
            Map map = new Map();


            Form form = CreateFormWithMapLegendView(map);

            string path = DataPath + "rivers.shp";

            ShapeFile shapeFile = new ShapeFile(path, false);
            VectorLayer vectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile);


            Pen pen2 = new Pen(new SolidBrush(Color.Red), 5);

            VectorStyle style2 = GetStyle(pen2);

            vectorLayer.Style = style2;
            map.Layers.Add(vectorLayer);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowMapLegendViewWithGradientTheme()
        {
            Map map = new Map();
            Form form = CreateFormWithMapLegendView(map);

            string path = DataPath + "rivers.shp";
            ShapeFile shapeFile = new ShapeFile(path, false);
            VectorLayer vectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile);

            Pen pen1 = new Pen(new SolidBrush(Color.Yellow), 3);
            Pen pen2 = new Pen(new SolidBrush(Color.Red), 5);

            VectorStyle style = GetStyle(pen1);
            VectorStyle style2 = GetStyle(pen2);
            GradientTheme gradientTheme = new GradientTheme("RIVERSEGME", -999, +999, style, style2, null, null, null);

            vectorLayer.Theme = gradientTheme;
            map.Layers.Add(vectorLayer);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowMapLegendViewWithPolygonVectorLayer()
        {
            var map = new Map();
            Form form = CreateFormWithMapLegendView(map);

            string path = DataPath + "outline.shp";
            var shapeFile = new ShapeFile(path, false);
            var vectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile);

            map.Layers.Add(vectorLayer);
            WindowsFormsTestHelper.ShowModal(form);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowMapLegendViewWithLabelLayer()
        {
            Map map = new Map(new Size(2, 1));

            const string rivers = DataPath + "rivers.shp";
            string cities = DataPath + "cities_europe.shp";

            ShapeFile shapeFileRivers = new ShapeFile(rivers, false);
            ShapeFile shapeFileCities = new ShapeFile(cities, false);

            VectorLayer vlRivers = new VectorLayer(Path.GetFileNameWithoutExtension(rivers), shapeFileRivers);
            VectorLayer vlCities = new VectorLayer(Path.GetFileNameWithoutExtension(cities), shapeFileCities);
            vlCities.Name = "City points";

            var llCities = vlCities.LabelLayer;
            llCities.Name = "City labels";
            llCities.LabelColumn = "NAME";
            llCities.ShowInLegend = true;
            llCities.Visible = true;

            GroupLayer lg = new GroupLayer("Cities");
            lg.Layers.Add(vlCities);

            map.Layers.Add(lg);
            map.Layers.Add(vlRivers);
            map.BackColor = Color.WhiteSmoke;

            Form test = CreateFormWithMapLegendView(map);

            WindowsFormsTestHelper.ShowModal(test);
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
            var map = new Map { Name = "map1" };
            var layer1 = new VectorLayer { Name = "kees" };
            MapLegendView.HideAllLayersButThisOne(layer1, map);
        }

        private MapLegendView CreateMapLegendView()
        {
            IGui gui = mocks.Stub<IGui>();

            //create stubbed version of plugingui
            SharpMapGisGuiPlugin guiPlugin = new SharpMapGisGuiPlugin();

            IGisGuiService gisService = mocks.Stub<IGisGuiService>();
            guiPlugin.GisGuiService = gisService;

            Expect.Call(gui.Plugins).Return(new[] {guiPlugin}).Repeat.Any();
            Expect.Call(gui.ToolWindowViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            Expect.Call(gui.DocumentViews).Repeat.Any().Return(mocks.Stub<IViewList>());

            var app = mocks.Stub<IApplication>();
            var project = new Project();
            Expect.Call(app.Project).Repeat.Any().Return(project);
            Expect.Call(app.FileExporters).Repeat.Any().Return(new List<IFileExporter>());
            gui.Application = app;

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

        private static VectorStyle GetStyle(Pen pen)
        {
            VectorStyle style = new VectorStyle();

            style.Fill = Brushes.AntiqueWhite;
            style.Line = pen;
            style.EnableOutline = true;
            style.Outline = Pens.Black;
            return style;
        }
    }
}