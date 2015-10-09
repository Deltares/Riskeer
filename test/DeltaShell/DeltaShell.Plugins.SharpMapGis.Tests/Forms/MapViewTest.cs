using System.Drawing;
using System.IO;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Reflection;
using DeltaShell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using GisSharpBlog.NetTopologySuite.Geometries;
using log4net;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Extensions.CoordinateSystems;
using SharpMap.Layers;
using SharpMapTestUtils;
using SharpTestsEx;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class MapViewTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapViewTest));

        [Test]
        public void DisablingLayerShouldRefreshMapOnce()
        {
            var mapView = new MapView();
            mapView.Map.Layers.Add(new GroupLayer("group1"));

            var refreshCount = 0;
            mapView.MapControl.MapRefreshed += delegate { refreshCount++; };

            mapView.Map.Layers.First().Visible = false;

            // TODO: currently second refresh can happen because of timer in MapControl - timer must be replaced by local Map / Layer / MapControl custom event
            refreshCount.Should("map should be refreshed once when layer property changes").Be.LessThanOrEqualTo(2);
        }

        [Test]
        public void DeletingLayerSetsRenderRequired()
        {
            using (var mapView = new MapView())
            {
                mapView.Map.Layers.Add(new GroupLayer("group1"));
                mapView.Map.Layers.RemoveAt(0);
                Assert.IsTrue(mapView.Map.RenderRequired);
            }
        }

        [Test]
        public void RenderMapWithLabelLayerAsDataSource()
        {
            Map map = new Map(new Size(2, 1));

            string rivers = DataPath + "rivers.shp";
            string cities = DataPath + "cities_europe.shp";

            ShapeFile shapeFileRivers = new ShapeFile(rivers, false);
            ShapeFile shapeFileCities = new ShapeFile(cities, false);

            VectorLayer vlRivers = new VectorLayer(Path.GetFileNameWithoutExtension(rivers), shapeFileRivers);
            VectorLayer vlCities = new VectorLayer(Path.GetFileNameWithoutExtension(cities), shapeFileCities);

            var llCities = vlCities.LabelLayer;
            llCities.Name = "Cities";
            llCities.LabelColumn = "NAME";
            llCities.ShowInLegend = true;
            llCities.Visible = true;

            GroupLayer lg = new GroupLayer("cities");
            lg.Layers.Add(vlCities);
            lg.Layers.Add(vlRivers);
            lg.Visible = true;

            map.Layers.Add(lg);
            map.BackColor = Color.WhiteSmoke;
            MapTestHelper.ShowModal(map);
        }

        [Test]
        [TestCase(@"osm\europe_western_europe_netherlands_location.shp", ShapeType.Point)]
        [TestCase(@"osm\europe_western_europe_netherlands_water.shp", ShapeType.Polygon)]
        [TestCase(@"osm\europe_western_europe_netherlands_highway.shp", ShapeType.PolyLine)]
        public void RenderLargeShapefile(string filePath, ShapeType type)
        {
            LogHelper.ConfigureLogging(Level.Debug);

            var path = DataPath + filePath;

            ShapeFile shp = null;
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();

            var src = Map.CoordinateSystemFactory.CreateFromEPSG(4326 /* WGS84 */);
            var dst = Map.CoordinateSystemFactory.CreateFromEPSG(3857 /* Web Mercator */);

            var layer = new VectorLayer
            {
                Name = Path.GetFileName(path),
                // RenderQuadTreeEnvelopes = true,
                // UseSimpleGeometryForQuadTree = true,
                UseQuadTree = true,
                // CoordinateTransformation = Map.CoordinateSystemFactory.CreateTransformation(src, dst)
            };

            var mapView = new MapView();

            using (var gui = new DeltaShellGui
            {
                Plugins =
                {
                    new SharpMapGisGuiPlugin()
                }
            })
            {
                var mapLegendView = new MapLegendView(gui)
                {
                    Map = mapView.Map
                };
                WindowsFormsTestHelper.Show(mapLegendView);

                WindowsFormsTestHelper.ShowModal(mapView, o =>
                {
                    TestHelper.AssertIsFasterThan(25000, () =>
                    {
                        shp = new ShapeFile(path, true);
                        //layer.SimplifyGeometryDuringRendering = false;
                        layer.DataSource = shp;

                        if (type == ShapeType.Polygon)
                        {
                            layer.Style.Fill = new SolidBrush(Color.FromArgb(100, 50, 50, 255));
                            //layer.Style.Fill = Brushes.Transparent;
                            layer.Style.Outline = new Pen(Color.FromArgb(150, 50, 50, 255), 1);
                            //layer.Style.EnableOutline = false;
                        }

                        mapView.Map.Layers.Add(layer);
                    });
                }, layer);

                mapView.Dispose();
            }
        }

        [Test]
        public void MapMaintainsZoomAndCenterWhenShownInMapView()
        {
            var map = new Map(new Size(640, 480));
            const int zoom = 5;
            map.Zoom = zoom;
            var center = new Coordinate(20, 20);
            map.Center = center;

            using (var view = new MapView
            {
                Size = new Size(640, 480), Map = map
            })
            {
                Assert.AreEqual(zoom, map.Zoom);
                Assert.AreEqual(center, map.Center);
            }
        }

        [Test]
        public void OpenMapViewWithVectorLayerAttributeTable()
        {
            var shapefile = new ShapeFile(DataPath + "rivers.shp");
            var layer = new VectorLayer
            {
                DataSource = shapefile
            };
            var map = new Map
            {
                Layers =
                {
                    layer
                }
            };
            var mapView = new MapView
            {
                Map = map
            };

            Assert.IsTrue(mapView.Splitter.IsCollapsed);

            Assert.IsFalse(mapView.IsTabControlVisible);

            mapView.OpenLayerAttributeTable(layer);

            Assert.IsTrue(mapView.IsTabControlVisible);
        }

        [Test]
        public void CloseLastTabCollapsesSplitter()
        {
            var shapefile = new ShapeFile(DataPath + "rivers.shp");
            var layer = new VectorLayer
            {
                DataSource = shapefile
            };
            var map = new Map
            {
                Layers =
                {
                    layer
                }
            };
            var mapView = new MapView
            {
                Map = map
            };

            var view = new VectorLayerAttributeTableView
            {
                Data = layer
            };

            mapView.TabControl.AddView(view);
            mapView.TabControl.RemoveView(view);

            Assert.IsTrue(mapView.Splitter.IsCollapsed);
        }

        [Test]
        public void RemovingGroupLayerShouldCloseOpenTabsForSubLayers()
        {
            var groupLayer = new GroupLayer();
            var subLayer = new VectorLayer();

            var mocks = new MockRepository();
            var tabControl = mocks.Stub<MapViewTabControl>();
            var mapViewEditor = mocks.StrictMock<ILayerEditorView>();

            var layerEditorViews = new EventedList<IView>
            {
                mapViewEditor
            };

            tabControl.Expect(tc => tc.ChildViews).Return(layerEditorViews).Repeat.Any();

            mapViewEditor.Expect(me => me.Data).Return(subLayer).Repeat.Any();

            // During the mapView.Map.Layers.Clear() the subView should : 
            // be removed from the tabControl, have its data set to null and be disposed
            tabControl.Expect(tc => tc.RemoveView(mapViewEditor));

            mocks.ReplayAll();

            var mapView = new MapView
            {
                GetDataForLayer = l => l
            };

            groupLayer.Layers.Add(subLayer);
            mapView.Map.Layers.Add(groupLayer);

            // use mocked tab control to check the removing of the sub-layer view
            TypeUtils.SetField(mapView, "tabControl", tabControl);

            mapView.Map.Layers.Clear();

            mocks.VerifyAll();
        }

        private const string DataPath = @"..\..\..\..\..\test-data\DeltaShell\DeltaShell.Plugins.SharpMapGis.Tests\";

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }
    }
}