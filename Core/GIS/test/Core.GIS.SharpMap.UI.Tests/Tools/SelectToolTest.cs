using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.TestUtils;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.NetTopologySuite.IO;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.UI.Forms;
using Core.GIS.SharpMap.UI.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace Core.GIS.SharpMap.UI.Tests.Tools
{
    [TestFixture]
    public class SelectToolTest
    {
        [Test]
        public void ClearSelectionOnLayerRemove()
        {
            var featureProvider = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 0)")
                    }
                }
            };
            var layer = new VectorLayer
            {
                DataSource = featureProvider
            };

            using (var mapControl = new MapControl
            {
                Map =
                {
                    Layers =
                    {
                        layer
                    }
                }
            })
            {
                WindowsFormsTestHelper.Show(mapControl);

                var selectTool = mapControl.SelectTool;

                selectTool.Select(featureProvider.Features.Cast<IFeature>());

                mapControl.Map.Layers.Clear();

                selectTool.Selection
                          .Should("selection is cleared on layer remove").Be.Empty();
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void ClearSelectionOnParentGroupLayerRemove()
        {
            var featureProvider = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 0)")
                    }
                }
            };
            var layer = new VectorLayer
            {
                DataSource = featureProvider
            };
            var groupLayer = new GroupLayer
            {
                Layers =
                {
                    layer
                }
            };

            using (var mapControl = new MapControl
            {
                Map =
                {
                    Layers =
                    {
                        groupLayer
                    }
                }
            })
            {
                WindowsFormsTestHelper.Show(mapControl);

                var selectTool = mapControl.SelectTool;

                selectTool.Select(featureProvider.Features.Cast<IFeature>());

                mapControl.Map.Layers.Remove(groupLayer);
                mapControl.Map.NotifyObservers();

                selectTool.Selection
                          .Should("selection is cleared on layer remove").Be.Empty();
            }

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void DeActiveSelectionShouldResetMultiSelectionMode()
        {
            var mapControl = new MapControl();
            var selectTool = mapControl.SelectTool;

            selectTool.MultiSelectionMode = MultiSelectionMode.Lasso;

            mapControl.ActivateTool(selectTool);
            Assert.AreEqual(MultiSelectionMode.Lasso, selectTool.MultiSelectionMode);

            mapControl.ActivateTool(mapControl.MoveTool);
            Assert.AreEqual(MultiSelectionMode.Rectangle, selectTool.MultiSelectionMode);
        }

        [Test]
        public void FindNearestFeature()
        {
            var mapControl = new MapControl();

            var layer1 = new VectorLayer();
            var layer1Data = new FeatureCollection();
            layer1.DataSource = layer1Data;
            layer1Data.FeatureType = typeof(Feature);
            layer1Data.Add(new Point(5, 5));
            layer1Data.Add(new Point(1, 1));

            var layer2 = new VectorLayer();
            var layer2Data = new FeatureCollection();
            layer2.DataSource = layer2Data;
            layer2Data.FeatureType = typeof(Feature);
            layer2Data.Add(new Point(4, 5));
            layer2Data.Add(new Point(0, 1));

            mapControl.Map.Layers.Add(layer1);
            mapControl.Map.Layers.Add(layer2);

            ILayer outLayer;
            var feature = mapControl.SelectTool.FindNearestFeature(new Coordinate(4, 4), 2.2f, out outLayer, null);

            // expect coordinate in topmost layer
            Assert.AreEqual(outLayer, layer1);
            Assert.AreEqual(5, feature.Geometry.Coordinate.X);
            Assert.AreEqual(5, feature.Geometry.Coordinate.Y);

            layer2.Visible = false;

            feature = mapControl.SelectTool.FindNearestFeature(new Coordinate(4, 4), 2.2f, out outLayer, null);

            Assert.AreEqual(outLayer, layer1);
            Assert.AreEqual(5, feature.Geometry.Coordinate.X);
            Assert.AreEqual(5, feature.Geometry.Coordinate.Y);
        }

        [Test]
        public void FindNearestFeaturesForContextMenu()
        {
            var mapControl = new MapControl();

            var layer1 = new VectorLayer();
            var layer1Data = new FeatureCollection();
            layer1.DataSource = layer1Data;
            layer1Data.FeatureType = typeof(Feature);
            layer1Data.Add(new Point(4, 4));
            layer1Data.Add(new Point(1, 1));

            var layer2 = new VectorLayer();
            var layer2Data = new FeatureCollection();
            layer2.DataSource = layer2Data;
            layer2Data.FeatureType = typeof(Feature);
            layer2Data.Add(new Point(4, 4));
            layer2Data.Add(new Point(0, 1));

            mapControl.Map.Layers.Add(layer1);
            mapControl.Map.Layers.Add(layer2);

            // zoom 4x because the limit (10 pixels around coordinate) used for finding the next feature depends on world to pixel ratio
            mapControl.Map.Zoom = mapControl.Map.Zoom*4;

            var items = mapControl.SelectTool.GetContextMenuItems(new Coordinate(4, 4));

            var mapToolContextMenuItem = items.FirstOrDefault();
            Assert.IsNotNull(mapToolContextMenuItem);
            Assert.IsInstanceOf<ToolStripMenuItem>(mapToolContextMenuItem.MenuItem);

            var dropDownItems = mapToolContextMenuItem.MenuItem.DropDownItems;
            Assert.AreEqual(3, dropDownItems.Count);

            layer2.Visible = false;

            items = mapControl.SelectTool.GetContextMenuItems(new Coordinate(4, 4));

            mapToolContextMenuItem = items.FirstOrDefault();
            Assert.IsNotNull(mapToolContextMenuItem);
            Assert.IsInstanceOf<ToolStripMenuItem>(mapToolContextMenuItem.MenuItem);

            dropDownItems = mapToolContextMenuItem.MenuItem.DropDownItems;
            Assert.AreEqual(2, dropDownItems.Count);
        }

        [Test]
        public void TestAddSelection()
        {
            var mapControl = new MapControl();
            var layerData = new FeatureCollection();

            var layer = new VectorLayer
            {
                Visible = false, DataSource = layerData
            };
            var feature = new TestFeature
            {
                Geometry = new Point(0, 0)
            };
            layerData.FeatureType = typeof(TestFeature);
            layerData.Add(feature);

            mapControl.Map.Layers.Add(layer);

            Assert.AreEqual(0, mapControl.SelectTool.Selection.Count());

            mapControl.SelectTool.AddSelection(new IFeature[0]);

            Assert.AreEqual(0, mapControl.SelectTool.Selection.Count(), "No features should be added as none are passed");

            mapControl.SelectTool.AddSelection(new[]
            {
                feature
            });

            Assert.AreEqual(0, mapControl.SelectTool.Selection.Count(), "No features should be added as none are visible");

            layer.Visible = true;
            mapControl.SelectTool.AddSelection(new[]
            {
                feature
            });

            Assert.AreEqual(1, mapControl.SelectTool.Selection.Count());

            mapControl.SelectTool.AddSelection(new[]
            {
                feature
            });

            Assert.AreEqual(1, mapControl.SelectTool.Selection.Count(), "Should not expand selection with items that are already selected");
        }

        [Test]
        public void SetLayerWithManySelectedFeaturesVisibilityFalseShouldBeFast()
        {
            var mapControl = new MapControl();
            var features = Enumerable.Range(0, 10000).Select(i => new TestFeature
            {
                Geometry = new Point(i, i)
            }).ToList();
            var layerData = new FeatureCollection(features, typeof(TestFeature));
            var vectorLayer = new VectorLayer
            {
                Visible = true, DataSource = layerData
            };

            mapControl.Map.Layers.Add(vectorLayer);
            mapControl.SelectTool.AddSelection(features.Take(5000));

            TestHelper.AssertIsFasterThan(5000, () =>
            {
                vectorLayer.Visible = false;
                mapControl.SelectTool.RefreshSelection();
            });
        }

        [Test]
        public void SelectionMultipleFeaturesOnMultipleLayers()
        {
            var featureProvider1 = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 0)")
                    },
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 1)")
                    },
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 2)")
                    }
                }
            };
            var layer1 = new VectorLayer
            {
                DataSource = featureProvider1
            };
            var featureProvider2 = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(1 0)")
                    },
                }
            };
            var layer2 = new VectorLayer
            {
                DataSource = featureProvider2
            };

            using (var mapControl = new MapControl
            {
                Map =
                {
                    Layers =
                    {
                        layer1, layer2
                    }
                }
            })
            {
                var selectTool = mapControl.SelectTool;

                var listOfSelectedFeatures = new[]
                {
                    featureProvider1.Features[0],
                    featureProvider1.Features[2],
                    featureProvider2.Features[0]
                };

                selectTool.Select(listOfSelectedFeatures.Cast<IFeature>());

                selectTool.Selection.Count()
                          .Should("selection are 3 features in 2 layers, 2 features in layer1, 1 feature in layer 2").Be.EqualTo(3);
            }
        }

        [Test]
        public void SelectionMultipleFeaturesInMultipleLayersOnOnlyOneLayer()
        {
            var featureProvider1 = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 0)")
                    },
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 1)")
                    },
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 2)")
                    }
                }
            };
            var layer1 = new VectorLayer
            {
                DataSource = featureProvider1
            };
            var featureProvider2 = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(1 0)")
                    },
                }
            };
            var layer2 = new VectorLayer
            {
                DataSource = featureProvider2
            };

            using (var mapControl = new MapControl
            {
                Map =
                {
                    Layers =
                    {
                        layer1, layer2
                    }
                }
            })
            {
                var selectTool = mapControl.SelectTool;

                var listOfSelectedFeatures = new[]
                {
                    featureProvider1.Features[0],
                    featureProvider1.Features[2],
                    featureProvider2.Features[0]
                };

                selectTool.Select(listOfSelectedFeatures.Cast<IFeature>(), layer1);

                selectTool.Selection.Count()
                          .Should("selection are 3 features in 2 layers, but only select the 2 features in layer1!").Be.EqualTo(2);

                selectTool.Select(listOfSelectedFeatures.Cast<IFeature>(), layer2);

                selectTool.Selection.Count()
                          .Should("selection are 3 features in 2 layers, but only select the 1 feature in layer2!").Be.EqualTo(1);
            }
        }

        /// <summary>
        /// Test for TOOLS-22796:'Selecting area feature type in project tree selects all features in map which can be extremely slow'
        /// </summary>
        [Test]
        public void SelectionMoreThanMaxMultipleFeaturesInMultipleLayers()
        {
            var maxSelectableFeatures = SelectTool.MaxSelectedFeatures;
            var featureProvider1 = new FeatureCollection(
                Enumerable.Range(0, maxSelectableFeatures*2/3)
                          .Select(i => new Feature
                          {
                              Geometry = new WKTReader().Read("POINT(0 " + i + ")")
                          }).ToList(),
                typeof(Feature));

            var layer1 = new VectorLayer
            {
                DataSource = featureProvider1
            };

            var featureProvider2 = new FeatureCollection(
                Enumerable.Range(0, maxSelectableFeatures*2/3)
                          .Select(i => new Feature
                          {
                              Geometry = new WKTReader().Read("POINT(1 " + i + ")")
                          }).ToList(),
                typeof(Feature));
            var layer2 = new VectorLayer
            {
                DataSource = featureProvider2
            };

            using (var mapControl = new MapControl
            {
                Map =
                {
                    Layers =
                    {
                        layer1, layer2
                    }
                }
            })
            {
                var selectTool = mapControl.SelectTool;

                IEnumerable<IFeature> listOfSelectedFeatures =
                    featureProvider1.Features.Cast<IFeature>().ToList().Concat(featureProvider2.Features.Cast<IFeature>().ToList());

                var ofSelectedFeatures = listOfSelectedFeatures as IFeature[] ?? listOfSelectedFeatures.ToArray();
                selectTool.Select(ofSelectedFeatures);

                var message =
                    string.Format("selection are all {1} features in 2 layers, but only select the first {0} features!", maxSelectableFeatures, ofSelectedFeatures.Count());
                selectTool.Selection.Count()
                          .Should(message).Be.EqualTo(maxSelectableFeatures);
            }
        }

        private class TestFeature : Feature {}
    }
}