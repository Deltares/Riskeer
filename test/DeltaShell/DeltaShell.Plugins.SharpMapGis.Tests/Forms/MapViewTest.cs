using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Reflection;
using DeltaShell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using GeoAPI.CoordinateSystems;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using log4net;
using log4net.Core;
using NetTopologySuite.Extensions.Features;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.Api;
using SharpMap.Data.Providers;
using SharpMap.Extensions.CoordinateSystems;
using SharpMap.Layers;
using SharpMap.UI.Forms;
using SharpMap.Utilities.SpatialIndexing;
using SharpMapTestUtils;
using SharpTestsEx;
using Point = GisSharpBlog.NetTopologySuite.Geometries.Point;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Forms
{
    [TestFixture]
    public class MapViewTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapViewTest));
        const string DataPath = @"..\..\..\..\..\test-data\DeltaShell\DeltaShell.Plugins.SharpMapGis.Tests\";

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

        [Test, Category(TestCategory.WindowsForms)]
        public void DisablingLayerShouldRefreshMapOnce()
        {
            var mapView = new MapView();
            mapView.Map.Layers.Add(new GroupLayer("group1"));

            WindowsFormsTestHelper.Show(mapView);

            while (mapView.MapControl.IsProcessing)
            {
                Application.DoEvents();
            }

            var refreshCount = 0;
            mapView.MapControl.MapRefreshed += delegate { refreshCount++; };

            mapView.Map.Layers.First().Visible = false;

            while(mapView.MapControl.IsProcessing)
            {
                Application.DoEvents();
            }

            // TODO: currently second refresh can happen because of timer in MapControl - timer must be replaced by local Map / Layer / MapControl custom event
            refreshCount.Should("map should be refreshed once when layer property changes").Be.LessThanOrEqualTo(2);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void DeletingLayerSetsRenderRequired()
        {
            using (var mapView = new MapView())
            {
                mapView.Map.Layers.Add(new GroupLayer("group1"));
                mapView.Map.Layers.RemoveAt(0);
                Assert.IsTrue(mapView.Map.RenderRequired);
            }
        }

        [Test, Category(TestCategory.WindowsForms)]
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
        [Category(TestCategory.WorkInProgress)] // can't find env variable on build server, in progress ...
        [Category("CoordinateSystem")]

        [TestCase(@"osm\europe_western_europe_netherlands_location.shp", ShapeType.Point)]
        [TestCase(@"osm\europe_western_europe_netherlands_water.shp", ShapeType.Polygon)]
        [TestCase(@"osm\europe_western_europe_netherlands_highway.shp", ShapeType.PolyLine)]
        [Category(TestCategory.Performance)]
        [Category(TestCategory.Slow)]
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

            using (var gui = new DeltaShellGui { Plugins = { new SharpMapGisGuiPlugin() } })
            {
                var mapLegendView = new MapLegendView(gui) { Map = mapView.Map };
                WindowsFormsTestHelper.Show(mapLegendView);

                WindowsFormsTestHelper.ShowModal(mapView, o =>
                {
                    TestHelper.AssertIsFasterThan(20000, () =>
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

                    // build quad tree from SharpMap
/*
                    var boxObjects = new List<QuadTree.BoxObjects>();
                    var features = shp.Features;
                    for (int i = 0; i < features.Count; i++)
                    {
                        var feature = (IFeature)features[i];
                        boxObjects.Add(new QuadTree.BoxObjects { box = feature.Geometry.EnvelopeInternal, ID = (uint)i });
                    }

                    Heuristic heur;
                    heur.maxdepth = (int)Math.Ceiling(0.5 * Math.Log(shp.Features.Count, 2));
                    heur.minerror = 10;
                    heur.tartricnt = 5;
                    heur.mintricnt = 2;
                    var quadTree = new QuadTree(boxObjects, 0, heur);

                    AddSharpMapQuadTreeFeatures(quadTree, mapView.Map);
*/


                }, layer);

                mapView.Dispose();
            }
        }

        IGeometryFactory geometryFactory = new GeometryFactory();
        IDictionary<int, VectorLayer> quadTreeLayers = new Dictionary<int, VectorLayer>();

        private void AddSharpMapQuadTreeFeatures(QuadTreeOld node, Map map)
        {
            var e = node.Box;
            var feature = new Feature { Geometry = geometryFactory.ToGeometry(e) };

            VectorLayer vectorLayer = null;
            if (!quadTreeLayers.TryGetValue((int)node.Depth, out vectorLayer))
            {
                var featureCollection = new FeatureCollection { Features = new List<Feature> { feature } };
                vectorLayer = new VectorLayer { DataSource = featureCollection, Style = { Fill = Brushes.Transparent }, Name = "quad tree, level: " + node.Depth };
                map.Layers.Insert(0, vectorLayer);
                quadTreeLayers[(int)node.Depth] = vectorLayer;
            }
            else
            {
                vectorLayer.DataSource.Features.Add(feature);
            }

            if (node.Child0 != null)
            {
                AddSharpMapQuadTreeFeatures(node.Child0, map);
            }
            if (node.Child0 != null)
            {
                AddSharpMapQuadTreeFeatures(node.Child1, map);
            }
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        [Category(TestCategory.WorkInProgress)] // can't find env variable on build server, in progress ...
        [Category("CoordinateSystem")]
        public void RenderMapWithTransformation()
        {
            LogHelper.ConfigureLogging(Level.Debug);
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();

            log.DebugFormat("GDAL_DATA: {0}", Environment.GetEnvironmentVariable("GDAL_DATA")); 

            // nice ball: +proj=lcc +lat_1=44.1 +lat_0=44.1 +lon_0=0 +k_0=0.999877499 +x_0=600000 +y_0=200000 +a=6378249.2 +b=6356515 +towgs84=-168,-60,320,0,0,0,0 +pm=paris +units=m'
            var crsWgs84 = Map.CoordinateSystemFactory.CreateFromEPSG(4326 /* WGS84 */);
            var crsMercator = Map.CoordinateSystemFactory.CreateFromEPSG(3857 /* Web Mercator */);
            var crsRd = Map.CoordinateSystemFactory.CreateFromEPSG(28992 /* RD New */);

            const string path = DataPath + "World_countries_shp.shp";
            var shp = new ShapeFile(path, true);
            var layer = new VectorLayer
                            {
                                DataSource = shp,
                                UseQuadTree = true,
                                //RenderQuadTree = true
                                RenderQuadTreeEnvelopes = true,
                                UseSimpleGeometryForQuadTree = true
                            };


/*
            const string path2 = DataPath + @"NAD83\watershed_major.shp";
            var shp2 = new ShapeFile(path2, true);
            var layer2 = new VectorLayer
            {
                DataSource = shp2,
                Style = { Fill = new SolidBrush(Color.FromArgb(100, 200, 50, 50))},
                UseQuadTree = true
            };

            const string path3 = DataPath + @"Gemeenten.shp";
            var shp3 = new ShapeFile(path3, true);
            var layer3 = new VectorLayer
            {
                DataSource = shp3,
                Style = { Fill = new SolidBrush(Color.FromArgb(100, 50, 200, 50)) },
                UseQuadTree = true,
                //RenderQuadTree = true
            };

            const string path4 = DataPath + @"osm\europe_western_europe_netherlands_water.shp";
            var shp4 = new ShapeFile(path4, true);
            var layer4 = new VectorLayer
            {
                DataSource = shp4,
                CoordinateTransformation = new OgrCoordinateTransformation((OgrCoordinateSystem)shp4.CoordinateSystem, (OgrCoordinateSystem)dst),
                //SimplifyGeometryDuringRendering = false,
                //SkipRenderingOfVerySmallFeatures = false,
                Style = { Fill = new SolidBrush(Color.FromArgb(100, 50, 200, 50)) }
            };
*/

            var map = new Map
                          {
                              Layers = { /*layer2, layer3, layer4, layerlines,*/ layer }, 
                              Size = new Size(640, 480),
                              CoordinateSystem = crsMercator 
                          };

            var control = new SelectCoordinateSystemDialog(Map.CoordinateSystemFactory.SupportedCoordinateSystems, Map.CoordinateSystemFactory.CustomCoordinateSystems)
                              {
                                  Dock = DockStyle.Fill,
                                  SelectedCoordinateSystem = crsMercator
                              };
            control.SelectedCoordinateSystemChanged += delegate(ICoordinateSystem system)
                                                           {
                                                               map.CoordinateSystem = system;
                                                               map.ZoomToExtents();
                                                           };

            WindowsFormsTestHelper.Show(control, map, layer);

            MapTestHelper.ShowModal(map);
        }

        [Test]
        public void MapMaintainsZoomAndCenterWhenShownInMapView()
        {
            var map = new Map(new Size(640, 480));
            const int zoom = 5;
            map.Zoom = zoom;
            var center = new Coordinate(20, 20);
            map.Center = center;

            using (var view = new MapView {Size = new Size(640, 480), Map = map})
            {
                Assert.AreEqual(zoom, map.Zoom);
                Assert.AreEqual(center, map.Center);
            }
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void GetContextMenuOnMapToCreatePng()
        {
            MapView mapView = new MapView();
            var map = new Map(new Size(2, 1));
            mapView.Map = map;

            const string sharpMapTestPath = @"..\..\..\..\..\test-data\DeltaShell\DeltaShell.Plugins.SharpMapGis.Tests\";
            const string path = sharpMapTestPath + "roads.shp";
            var shapeFile = new ShapeFile(path, false);
            var vectorLayer = new VectorLayer(Path.GetFileNameWithoutExtension(path), shapeFile);
            map.Layers.Add(vectorLayer);

            var form1 = new Form();
            mapView.Dock = DockStyle.Fill;
            form1.Controls.Add(mapView);
            
            WindowsFormsTestHelper.ShowModal(form1);
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void OpenMapViewWithVectorLayerAttributeTable()
        {
            var shapefile = new ShapeFile(DataPath + "rivers.shp");
            var layer = new VectorLayer { DataSource = shapefile };
            var map = new Map { Layers = { layer } };
            var mapView = new MapView { Map = map };
            
            Action<Form> afterShow = delegate
            {
                Assert.IsTrue(mapView.Splitter.IsCollapsed);

                Assert.IsFalse(mapView.IsTabControlVisible);

                mapView.OpenLayerAttributeTable(layer);

                Assert.IsTrue(mapView.IsTabControlVisible);
            };

            WindowsFormsTestHelper.ShowModal(mapView, afterShow);
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void CloseLastTabCollapsesSplitter()
        {
            var shapefile = new ShapeFile(DataPath + "rivers.shp");
            var layer = new VectorLayer { DataSource = shapefile };
            var map = new Map { Layers = { layer } };
            var mapView = new MapView { Map = map };

            Action<Form> afterShow = delegate
            {
                var view = new VectorLayerAttributeTableView { Data = layer };
                
                mapView.TabControl.AddView(view);
                mapView.TabControl.RemoveView(view);

                Assert.IsTrue(mapView.Splitter.IsCollapsed);
            };

            WindowsFormsTestHelper.ShowModal(mapView, afterShow);
        }

        [Test]
        public void RemovingGroupLayerShouldCloseOpenTabsForSubLayers()
        {
            var groupLayer = new GroupLayer();
            var subLayer = new VectorLayer();

            var mocks = new MockRepository();
            var tabControl = mocks.Stub<MapViewTabControl>();
            var mapViewEditor = mocks.StrictMock<ILayerEditorView>();

            var layerEditorViews = new EventedList<IView> {mapViewEditor};

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

        [Test, Category(TestCategory.WindowsForms)]
        public void MapViewSynchronizesMapSelectionWithMapEditors()
        {
            var mocks = new MockRepository();

            var featureToSync = mocks.Stub<IFeature>();
            var featureProvider = mocks.Stub<IFeatureProvider>();

            var tabControl = mocks.Stub<MapViewTabControl>();
            var mapViewEditor = mocks.StrictMock<ILayerEditorView>();

            featureToSync.Geometry = new Point(2, 2);
            var layerEditorViews = new EventedList<IView> {mapViewEditor};

            tabControl.Expect(tc => tc.Dispose()).Repeat.Any();
            tabControl.Expect(tc => tc.ChildViews).Return(layerEditorViews).Repeat.Any();
            
            featureProvider.Expect(fp => fp.CoordinateSystemChanged += null).IgnoreArguments().Repeat.Any();
            featureProvider.Expect(fp => fp.CoordinateSystemChanged -= null).IgnoreArguments().Repeat.Any();
            featureProvider.Expect(fp => fp.Contains(featureToSync)).Return(true).Repeat.Any();
            featureProvider.Expect(fp => fp.GetFeature(0)).Return(featureToSync).Repeat.Any();
            featureProvider.Expect(fp => fp.GetFeatureCount()).Return(1).Repeat.Any();
            //featureProvider.Expect(fp => fp.GetFeatures(new Envelope())).IgnoreArguments().Return(new[] { featureToSync }).Repeat.Any();
            featureProvider.Expect(fp => fp.GetExtents()).Return(new Envelope(0, 10, 0, 10)).Repeat.Any();

            featureProvider.FeaturesChanged += null;
            LastCall.IgnoreArguments();

            // expect call to mapViewEditor.SelectedFeatures after map selection
            mapViewEditor.Expect(me => me.SelectedFeatures).SetPropertyWithArgument(new[] { featureToSync });

            mocks.ReplayAll();

            var mapView = new MapView();
            mapView.Map.Layers.Add(new VectorLayer { DataSource = featureProvider });

            // use mocked tab control to check synchronization
            TypeUtils.SetField(mapView, "tabControl", tabControl);

            // selecting feature will set the mapViewEditor.SelectedFeatures 
            mapView.MapControl.SelectTool.Select(new[] { featureToSync });

            WindowsFormsTestHelper.ShowModal(mapView);
        } 
    }
}