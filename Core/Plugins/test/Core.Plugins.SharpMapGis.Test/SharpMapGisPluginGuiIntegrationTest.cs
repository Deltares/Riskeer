using System;
using System.Linq;
using Core.Common.Gui;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.IO;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.GIS.SharpMap.UI.Tools;
using Core.Plugins.SharpMapGis.Gui;
using Core.Plugins.SharpMapGis.Gui.Forms;
using NUnit.Framework;

namespace Core.Plugins.SharpMapGis.Test
{
    [TestFixture]
    public class SharpMapGisPluginGuiIntegrationTest
    {
        [Test]
        public void SelectMultipleFeaturesUpdatesAllMaps()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.ApplicationCore.AddPlugin(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new SharpMapGisGuiPlugin());
                gui.Run();

                var featureProvider = new FeatureCollection
                {
                    Features =
                    {
                        new Feature
                        {
                            Geometry = new WKTReader().Read("LINESTRING(0 0,80000000 0)")
                        },
                        new Feature
                        {
                            Geometry = new WKTReader().Read("POINT(50000000 0)")
                        }
                    }
                };

                var features = featureProvider.Features;

                var layer = new VectorLayer
                {
                    DataSource = featureProvider
                };
                var map = new Map
                {
                    Name = "Map"
                };
                map.Layers.Add(layer);
                map.NotifyObservers();

                gui.Project.Items.Add(map);

                gui.CommandHandler.OpenView(map);

                var mapView = (MapView) gui.DocumentViews.ActiveView;

                int called = 0;

                mapView.MapControl.SelectTool.SelectionChanged += delegate(object sender, EventArgs e)
                {
                    var selectTool = (SelectTool) sender;

                    if (!selectTool.Selection.Any())
                    {
                        return; // selection cleared
                    }

                    Assert.AreEqual(features, selectTool.Selection.ToList());
                    called++;
                };

                gui.Selection = features;

                Assert.AreEqual(1, called);
            }
        }

        [Test]
        public void SetMapSelectionDoesNotSetItAgain()
        {
            using (var gui = new RingtoetsGui())
            {
                gui.ApplicationCore.AddPlugin(new SharpMapGisApplicationPlugin());
                gui.Plugins.Add(new SharpMapGisGuiPlugin());
                gui.Run();

                var featureProvider = new FeatureCollection
                {
                    Features =
                    {
                        new Feature
                        {
                            Geometry = new WKTReader().Read("LINESTRING(0 0,80000000 0)")
                        },
                        new Feature
                        {
                            Geometry = new WKTReader().Read("POINT(50000000 0)")
                        }
                    }
                };

                var features = featureProvider.Features;

                var layer = new VectorLayer
                {
                    DataSource = featureProvider
                };
                var map = new Map
                {
                    Name = "Map"
                };
                map.Layers.Add(layer);

                gui.Project.Items.Add(map);

                gui.CommandHandler.OpenView(map);

                var mapView = (MapView) gui.DocumentViews.ActiveView;

                int called = 0;

                mapView.MapControl.SelectTool.SelectionChanged += delegate
                {
                    if (mapView.MapControl.SelectTool.Selection == Enumerable.Empty<IFeature>())
                    {
                        return; // selection cleared
                    }

                    called++;
                };

                mapView.MapControl.SelectTool.Select(features.OfType<IFeature>());

                Assert.AreEqual(1, called);
            }
        }
    }
}