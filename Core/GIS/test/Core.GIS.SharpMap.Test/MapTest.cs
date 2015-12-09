using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Utils.Collections.Generic;
using Core.Common.Utils.Reflection;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Converters.WellKnownText;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Styles;
using NUnit.Framework;
using Rhino.Mocks;
using GeometryFactory = Core.GIS.SharpMap.Converters.Geometries.GeometryFactory;

namespace Core.GIS.SharpMap.Test
{
    [TestFixture]
    public class MapTest
    {
        //TODO: rename this test
        [Test]
        public void EventBubbling2()
        {
            int changeCount = 0;
            var map = new Map.Map(new Size(2, 1));
            var vectorLayer = new VectorLayer("EventBubbling") { Style = new VectorStyle() };
            map.Layers.Add(vectorLayer);

            ((INotifyPropertyChanged) map).PropertyChanged +=
                (sender, e) =>
                {
                    Assert.AreEqual(e.PropertyName, "Line");
                    changeCount++;
                };

            Assert.AreEqual(0, changeCount);
            var pen1 = new Pen(new SolidBrush(Color.Yellow), 3);
            vectorLayer.Style.Line = pen1;
            Assert.AreEqual(1, changeCount);
        }

        //TODO: rename this test
        [Test]
        public void EventBubbling3()
        {
            int changeCount = 0;
            var map = new Map.Map(new Size(2, 1));
            var style = new VectorStyle();
            var vectorLayer = new VectorLayer("EventBubbling")
            {
                Style = style
            };
            map.Layers.Add(vectorLayer);

            ((INotifyPropertyChanged) map).PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e)
                {
                    Assert.AreEqual(e.PropertyName, "Line");
                    changeCount++;
                };

            Assert.AreEqual(0, changeCount);
            var pen1 = new Pen(new SolidBrush(Color.Yellow), 3);
            style.Line = pen1;
            Assert.AreEqual(1, changeCount);
        }

        [Test]
        public void Initalize_MapInstance()
        {
            var map = new Map.Map(new Size(2, 1));
            Assert.IsNotNull(map);
            Assert.IsNotNull(map.Layers);
            Assert.AreEqual(2f, map.Size.Width);
            Assert.AreEqual(1f, map.Size.Height);
            Assert.AreEqual(Color.Transparent, map.BackColor);
            Assert.AreEqual(1e9, map.MaximumZoom);
            Assert.AreEqual(1e-4, map.MinimumZoom);

            Assert.AreEqual(GeometryFactory.CreateCoordinate(0, 0), map.Center,
                            "map.Center should be initialized to (0,0)");
            Assert.AreEqual(1000, map.Zoom, "Map zoom should be initialized to 1000.0");
        }

        [Test]
        public void ImageToWorld()
        {
            Map.Map map = new Map.Map(new Size(1000, 500))
            {
                Zoom = 360, Center = GeometryFactory.CreateCoordinate(0, 0)
            };
            Assert.AreEqual(GeometryFactory.CreateCoordinate(0, 0),
                            map.ImageToWorld(new PointF(500, 250)));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(-180, 90),
                            map.ImageToWorld(new PointF(0, 0)));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(-180, -90),
                            map.ImageToWorld(new PointF(0, 500)));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(180, 90),
                            map.ImageToWorld(new PointF(1000, 0)));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(180, -90),
                            map.ImageToWorld(new PointF(1000, 500)));
        }

        [Test]
        public void WorldToImage()
        {
            var map = new Map.Map(new Size(1000, 500))
            {
                Zoom = 360, Center = GeometryFactory.CreateCoordinate(0, 0)
            };
            Assert.AreEqual(new PointF(500, 250), map.WorldToImage(GeometryFactory.CreateCoordinate(0, 0)));
            Assert.AreEqual(new PointF(0, 0), map.WorldToImage(GeometryFactory.CreateCoordinate(-180, 90)));
            Assert.AreEqual(new PointF(0, 500), map.WorldToImage(GeometryFactory.CreateCoordinate(-180, -90)));
            Assert.AreEqual(new PointF(1000, 0), map.WorldToImage(GeometryFactory.CreateCoordinate(180, 90)));
            Assert.AreEqual(new PointF(1000, 500), map.WorldToImage(GeometryFactory.CreateCoordinate(180, -90)));
        }

        [Test]
        public void GetLayerByName_ReturnCorrectLayer()
        {
            var map = new Map.Map();
            map.Layers.Add(new VectorLayer("layer 1"));
            map.Layers.Add(new VectorLayer("Layer 3"));
            map.Layers.Add(new VectorLayer("Layer 2"));

            ILayer layer = map.GetLayerByName("Layer 2");
            Assert.IsNotNull(layer);
            Assert.AreEqual("Layer 2", layer.Name);
        }

        [Test]
        public void GetLayerByName_Indexer()
        {
            var map = new Map.Map();
            map.Layers.Add(new VectorLayer("Layer 1"));
            map.Layers.Add(new VectorLayer("Layer 3"));
            map.Layers.Add(new VectorLayer("Layer 2"));

            ILayer layer = map.GetLayerByName("Layer 2");
            Assert.IsNotNull(layer);
            Assert.AreEqual("Layer 2", layer.Name);
        }

        [Test]
        public void FindLayer_ReturnEnumerable()
        {
            var map = new Map.Map();
            map.Layers.Add(new VectorLayer("Layer 1"));
            map.Layers.Add(new VectorLayer("Layer 3"));
            map.Layers.Add(new VectorLayer("Layer 2"));
            map.Layers.Add(new VectorLayer("Layer 4"));

            int count = 0;
            foreach (ILayer lay in map.FindLayer("Layer 3"))
            {
                Assert.AreEqual("Layer 3", lay.Name);
                count++;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void GetExtents_ValidDatasource()
        {
            var map = new Map.Map(new Size(400, 200));
            var vLayer = new VectorLayer("Geom layer", CreateTestFeatureProvider());
            map.Layers.Add(vLayer);
            IEnvelope box = map.GetExtents();
            Assert.AreEqual(GeometryFactory.CreateEnvelope(0, 50, 0, 346.3493254), box);
        }

        [Test]
        public void GetPixelSize_FixedZoom_Return8_75()
        {
            var map = new Map.Map(new Size(400, 200))
            {
                Zoom = 3500
            };
            Assert.AreEqual(8.75, map.PixelSize);
        }

        [Test]
        public void GetMapHeight_FixedZoom_Return1750()
        {
            var map = new Map.Map(new Size(400, 200))
            {
                Zoom = 3500
            };
            Assert.AreEqual(1750, map.WorldHeight);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetMinimumZoom_NegativeValue_ThrowException()
        {
            new Map.Map
            {
                MinimumZoom = -1
            };
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void SetMaximumZoom_NegativeValue_ThrowException()
        {
            new Map.Map
            {
                MaximumZoom = -1
            };
        }

        [Test]
        public void SetMaximumZoom_OKValue()
        {
            var map = new Map.Map
            {
                MaximumZoom = 100.3
            };
            Assert.AreEqual(100.3, map.MaximumZoom);
        }

        [Test]
        public void SetMinimumZoom_OKValue()
        {
            var map = new Map.Map
            {
                MinimumZoom = 100.3
            };
            Assert.AreEqual(100.3, map.MinimumZoom);
        }

        [Test]
        public void SetZoom_ValueOutsideMax()
        {
            var map = new Map.Map
            {
                MaximumZoom = 100, Zoom = 150
            };
            Assert.AreEqual(100, map.MaximumZoom);
        }

        [Test]
        public void SetZoom_ValueBelowMin()
        {
            var map = new Map.Map
            {
                MinimumZoom = 100, Zoom = 50
            };
            Assert.AreEqual(100, map.MinimumZoom);
        }

        [Test]
        public void ZoomToBox_NoAspectCorrection()
        {
            var map = new Map.Map(new Size(400, 200));
            map.ZoomToFit(GeometryFactory.CreateEnvelope(20, 50, 100, 80));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(35, 90), map.Center);
            Assert.AreEqual(40d, map.Zoom);
        }

        [Test]
        public void ZoomToBox_WithAspectCorrection()
        {
            var map = new Map.Map(new Size(400, 200));
            map.ZoomToFit(GeometryFactory.CreateEnvelope(10, 20, 100, 180));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(15, 140), map.Center);
            Assert.AreEqual(160d, map.Zoom);
        }

        [Test]
        public void ZoomToBoxWithMarginDoesNotMessUpGivenEnvelope()
        {
            var map = new Map.Map(new Size(400, 200));
            IEnvelope envelope = GeometryFactory.CreateEnvelope(20, 50, 100, 80);

            //action! zoom to box with margin
            map.ZoomToFit(envelope, true);
            Assert.AreEqual(20, envelope.MinX);
            Assert.AreEqual(50, envelope.MaxX);
            Assert.AreEqual(80, envelope.MinY);
            Assert.AreEqual(100, envelope.MaxY);
        }

        [Test]
        public void ZoomToBoxWithMarginDoesAddMargin()
        {
            var map = new Map.Map(new Size(400, 200));
            IEnvelope envelope = GeometryFactory.CreateEnvelope(20, 50, 100, 80);

            //action! zoom to box with margin
            map.ZoomToFit(envelope, true);

            //see the heigth has a 10% extra!
            Assert.AreEqual(22, map.Envelope.Height);
        }

        [Test]
        public void ZoomToBoxSetsMapHeight()
        {
            var map = new Map.Map(new Size(400, 200));
            IEnvelope envelope = GeometryFactory.CreateEnvelope(20, 50, 100, 80);

            //action! zoom to box with margin
            map.ZoomToFit(envelope, false);

            //see the heigth has a 10% extra!
            Assert.AreEqual(20, map.Envelope.Height);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void GetMap_RenderLayerWithoutDatasource_ThrowException()
        {
            var map = new Map.Map();
            map.Layers.Add(new VectorLayer("Layer 1"));
            map.Render();
        }

        [Test]
        public void WorldToMap_DefaultMap_ReturnValue()
        {
            var map = new Map.Map(new Size(500, 200));
            map.Center = GeometryFactory.CreateCoordinate(23, 34);
            map.Zoom = 1000;
            PointF p = map.WorldToImage(GeometryFactory.CreateCoordinate(8, 50));
            Assert.AreEqual(new PointF(242f, 92), p);
        }

        [Test]
        public void ImageToWorld_DefaultMap_ReturnValue()
        {
            var map = new Map.Map(new Size(500, 200))
            {
                Center = GeometryFactory.CreateCoordinate(23, 34), Zoom = 1000
            };
            ICoordinate p = map.ImageToWorld(new PointF(242.5f, 92));
            Assert.AreEqual(GeometryFactory.CreateCoordinate(8, 50), p);
        }

        [Test]
        public void GetMap_GeometryProvider_ReturnImage()
        {
            var map = new Map.Map(new Size(400, 200));
            var vLayer = new VectorLayer("Geom layer", CreateTestFeatureProvider())
            {
                Style =
                {
                    Outline = new Pen(Color.Red, 2f),
                    EnableOutline = true,
                    Line = new Pen(Color.Green, 2f),
                    Fill = Brushes.Yellow
                }
            };
            map.Layers.Add(vLayer);

            var vLayer2 = new VectorLayer("Geom layer 2", vLayer.DataSource);
            vLayer.Style.SymbolOffset = new PointF(3, 4);
            vLayer.Style.SymbolRotation = 45;
            vLayer.Style.SymbolScale = 0.4f;
            map.Layers.Add(vLayer2);

            var vLayer3 = new VectorLayer("Geom layer 3", vLayer.DataSource)
            {
                Style =
                {
                    SymbolOffset = new PointF(3, 4), SymbolRotation = 45
                }
            };
            map.Layers.Add(vLayer3);

            var vLayer4 = new VectorLayer("Geom layer 4", vLayer.DataSource)
            {
                Style =
                {
                    SymbolOffset = new PointF(3, 4), SymbolScale = 0.4f
                }
            };
            vLayer4.ClippingEnabled = true;
            map.Layers.Add(vLayer4);

            map.ZoomToExtents();

            var img = map.Render();
            Assert.IsNotNull(img);
        }

        [Test]
        public void DefaultExtentForVectorLayer()
        {
            var geometry = GeometryFromWKT.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)");
            var provider = new DataTableFeatureProvider(geometry);
            var map = new Map.Map
            {
                Layers =
                {
                    new VectorLayer
                    {
                        DataSource = provider
                    }
                }
            };

            Assert.IsTrue(map.GetExtents().Contains(geometry.EnvelopeInternal));
        }

        [Test]
        public void Clone()
        {
            var map = new Map.Map(new Size(10, 100))
            {
                Center = GeometryFactory.CreateCoordinate(90, 900)
            };

            map.Layers.Add(new VectorLayer("Layer 1"));
            map.Layers.Add(new VectorLayer("Layer 3"));
            map.Layers.Add(new VectorLayer("Layer 2"));

            var clonedMap = (Map.Map) map.Clone();

            Assert.AreEqual(map.Name, clonedMap.Name);
            Assert.AreEqual(map.Layers.Count, clonedMap.Layers.Count);
            Assert.AreEqual(map.Size.Width, clonedMap.Size.Width);
            Assert.AreEqual(map.Size.Height, clonedMap.Size.Height);
            Assert.AreEqual(map.Center.X, clonedMap.Center.X);
            Assert.AreEqual(map.Center.Y, clonedMap.Center.Y);
            Assert.AreEqual(map.Zoom, clonedMap.Zoom, 1e-10);
        }

        [Test]
        public void AddingALayerShouldCauseZoomToExtendsIfNoValidExtendsBefore()
        {
            var map = new Map.Map(new Size(10, 100))
            {
                Center = GeometryFactory.CreateCoordinate(90, 900)
            };

            //now add a layer with defined extends 
            var geometry = GeometryFromWKT.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)");
            var dataSource = new DataTableFeatureProvider(geometry);

            var vectorLayerWithExtends = new VectorLayer("Layer with extends")
            {
                DataSource = dataSource
            };
            map.Layers.Add(vectorLayerWithExtends);

            Assert.AreEqual(new Envelope(19, 41, -85, 135), map.Envelope);
        }

        [Test]
        public void GetGroupLayerContainingLayer()
        {
            var map = new Map.Map();
            var groupLayer = new GroupLayer();
            var childLayer = new VectorLayer();

            map.Layers.Add(groupLayer);
            groupLayer.Layers.Add(childLayer);

            Assert.AreEqual(groupLayer, map.GetGroupLayerContainingLayer(childLayer));
            //the grouplayer is not part of a grouplayer
            Assert.IsNull(map.GetGroupLayerContainingLayer(groupLayer));
        }

        [Test]
        public void GetAllLayersWithGroupLayer()
        {
            var map = new Map.Map();
            var groupLayer = new GroupLayer();
            var nestedLayer = new VectorLayer();
            groupLayer.Layers.Add(nestedLayer);

            var vectorLayer = new VectorLayer();
            map.Layers = new EventedList<ILayer>
            {
                groupLayer, vectorLayer
            };

            Assert.AreEqual(new ILayer[]
            {
                groupLayer,
                nestedLayer,
                vectorLayer
            }, map.GetAllLayers(true).ToArray());
        }

        [Test]
        public void GetAllVisibleLayersWithGroupLayer()
        {
            var map = new Map.Map();
            var groupLayer = new GroupLayer();
            groupLayer.Visible = false;
            var nestedLayer = new VectorLayer();
            nestedLayer.Visible = true;
            groupLayer.Layers.Add(nestedLayer);

            var vectorLayer = new VectorLayer();
            map.Layers = new EventedList<ILayer>
            {
                groupLayer, vectorLayer
            };

            Assert.AreEqual(new ILayer[]
            {
                vectorLayer
            }, map.GetAllVisibleLayers(true).ToArray());
        }

        [Test]
        public void GetLayerByFeature()
        {
            var map = new Map.Map();
            var groupLayer1 = new GroupLayer();
            var groupLayer2 = new GroupLayer();
            var childLayer1 = new VectorLayer();
            var childLayer2 = new VectorLayer
            {
                Visible = false
            };
            var feature = new Feature();

            map.Layers.Add(groupLayer1);
            groupLayer1.Layers.Add(childLayer1);
            groupLayer1.Layers.Add(groupLayer2);
            groupLayer2.Layers.Add(childLayer2);

            Assert.IsNull(map.GetLayerByFeature(feature));

            childLayer2.DataSource = new FeatureCollection
            {
                Features = new List<Feature>
                {
                    feature
                },
                FeatureType = typeof(Feature)
            };

            Assert.AreSame(childLayer2, map.GetLayerByFeature(feature), "Should retrieved invisible layer.");

            childLayer1.DataSource = new FeatureCollection
            {
                Features = new List<Feature>
                {
                    feature
                },
                FeatureType = typeof(Feature)
            };

            Assert.AreSame(childLayer1, map.GetLayerByFeature(feature), "Should retrieve childLayer1 as it's first in the collection.");
        }

        [Test]
        public void HasDefaultEnvelopeSet()
        {
            var map = new Map.Map();
            Assert.IsTrue(map.HasDefaultEnvelopeSet);

            map.ZoomToFit(new Envelope(0, 50, 0, 50), false);
            Assert.IsFalse(map.HasDefaultEnvelopeSet);

            map.ZoomToFit(new Envelope(-500, 500, -500, 500), false);
            Assert.IsTrue(map.HasDefaultEnvelopeSet);
        }

        [Test]
        public void ResetRenderOrderShouldChangeLayerRenderOrder()
        {
            var mocks = new MockRepository();

            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();

            layer1.RenderOrder = 1;
            layer2.RenderOrder = 3;
            layer3.RenderOrder = 5;

            mocks.ReplayAll();

            var map = new Map.Map();
            map.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            TypeUtils.CallPrivateMethod(map, "ResetRenderOrder", 2);

            Assert.AreEqual(2, layer1.RenderOrder);
            Assert.AreEqual(3, layer2.RenderOrder);
            Assert.AreEqual(4, layer3.RenderOrder);
        }

        [Test]
        public void GetNewRenderOrderShouldReturnOneMoreThenHighestRenderOrder()
        {
            var mocks = new MockRepository();

            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();

            mocks.ReplayAll();

            var map = new Map.Map();
            map.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            layer1.RenderOrder = 1;
            layer2.RenderOrder = 3;
            layer3.RenderOrder = 5;

            var number = TypeUtils.CallPrivateMethod<int>(map, "GetNewRenderNumber");
            Assert.AreEqual(6, number);
        }

        [Test]
        public void RenderOrderIsUpdatedAfterAddingOrRemovingLayers()
        {
            var mocks = new MockRepository();

            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();

            layer1.RenderOrder = 0;
            layer2.RenderOrder = 0;
            layer3.RenderOrder = 0;

            mocks.ReplayAll();

            var map = new Map.Map();
            map.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            Assert.AreEqual(1, layer1.RenderOrder);
            Assert.AreEqual(2, layer2.RenderOrder);
            Assert.AreEqual(3, layer3.RenderOrder);

            map.Layers.Remove(layer2);
            Assert.AreEqual(1, layer1.RenderOrder);
            Assert.AreEqual(2, layer3.RenderOrder);
        }

        [Test]
        public void AddingLayerToGroupLayerAssignsSimilarRenderOrder()
        {
            var mocks = new MockRepository();

            var groupLayer = new GroupLayer();

            var layer = mocks.Stub<ILayer>();
            var newLayer = mocks.Stub<ILayer>();
            var backgroundLayer = mocks.Stub<ILayer>();

            layer.RenderOrder = 0;
            newLayer.RenderOrder = 0;
            backgroundLayer.RenderOrder = 0;

            mocks.ReplayAll();

            var map = new Map.Map();
            groupLayer.Layers.Add(layer);
            map.Layers.AddRange(new[]
            {
                groupLayer,
                backgroundLayer
            });
            groupLayer.Layers.Add(newLayer);

            Assert.Less(newLayer.RenderOrder, backgroundLayer.RenderOrder);
        }

        [Test]
        public void BringToFront()
        {
            var mocks = new MockRepository();

            var groupLayer = new GroupLayer();
            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();
            var layer4 = mocks.Stub<ILayer>();

            mocks.ReplayAll();

            var map = new Map.Map();
            map.Layers.AddRange(new[]
            {
                groupLayer,
                layer4
            });
            groupLayer.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            layer4.RenderOrder = 0;

            layer1.RenderOrder = 2;
            layer3.RenderOrder = 3;
            layer2.RenderOrder = 5;

            map.BringToFront(groupLayer);

            Assert.AreEqual(0, layer1.RenderOrder);
            Assert.AreEqual(2, layer2.RenderOrder);
            Assert.AreEqual(1, layer3.RenderOrder);
            Assert.AreEqual(3, layer4.RenderOrder);
        }

        [Test]
        public void SendToBack()
        {
            var mocks = new MockRepository();

            var groupLayer = new GroupLayer();
            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();
            var layer4 = mocks.Stub<ILayer>();

            mocks.ReplayAll();

            var map = new Map.Map();
            map.Layers.AddRange(new[]
            {
                groupLayer,
                layer4
            });
            groupLayer.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            layer4.RenderOrder = 5;

            layer1.RenderOrder = 1;
            layer3.RenderOrder = 2;
            layer2.RenderOrder = 4;

            map.SendToBack(groupLayer);

            Assert.AreEqual(1, layer1.RenderOrder);
            Assert.AreEqual(3, layer2.RenderOrder);
            Assert.AreEqual(2, layer3.RenderOrder);
            Assert.AreEqual(0, layer4.RenderOrder);
        }

        [Test]
        public void SendBackward()
        {
            var mocks = new MockRepository();

            var groupLayer = new GroupLayer();
            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();
            var layer4 = mocks.Stub<ILayer>();
            var layer5 = mocks.Stub<ILayer>();

            mocks.ReplayAll();

            var map = new Map.Map();
            //1,2,4       5      0
            map.Layers.AddRange(new[]
            {
                groupLayer,
                layer4,
                layer5
            });
            groupLayer.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            layer5.RenderOrder = 0;

            layer1.RenderOrder = 1;
            layer3.RenderOrder = 2;
            layer2.RenderOrder = 4;

            layer4.RenderOrder = 5;

            map.SendBackward(layer5);

            Assert.AreEqual(0, layer1.RenderOrder);
            Assert.AreEqual(3, layer2.RenderOrder);
            Assert.AreEqual(2, layer3.RenderOrder);
            Assert.AreEqual(4, layer4.RenderOrder);
            Assert.AreEqual(1, layer5.RenderOrder);
        }

        [Test]
        public void BringForward()
        {
            var mocks = new MockRepository();

            var groupLayer = new GroupLayer();
            var layer1 = mocks.Stub<ILayer>();
            var layer2 = mocks.Stub<ILayer>();
            var layer3 = mocks.Stub<ILayer>();
            var layer4 = mocks.Stub<ILayer>();
            var layer5 = mocks.Stub<ILayer>();

            mocks.ReplayAll();

            var map = new Map.Map();

            map.Layers.AddRange(new[]
            {
                groupLayer,
                layer4,
                layer5
            });
            groupLayer.Layers.AddRange(new[]
            {
                layer1,
                layer2,
                layer3
            });

            layer5.RenderOrder = 0;

            layer1.RenderOrder = 1;
            layer3.RenderOrder = 2;
            layer2.RenderOrder = 4;

            layer4.RenderOrder = 5;

            map.BringForward(layer3);

            Assert.AreEqual(0, layer5.RenderOrder);
            Assert.AreEqual(2, layer1.RenderOrder);
            Assert.AreEqual(1, layer3.RenderOrder);
            Assert.AreEqual(3, layer2.RenderOrder);
            Assert.AreEqual(4, layer4.RenderOrder);
        }

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

        private IFeatureProvider CreateTestFeatureProvider()
        {
            Collection<IGeometry> geoms = new Collection<IGeometry>
            {
                GeometryFromWKT.Parse("POINT EMPTY"),
                GeometryFromWKT.Parse("GEOMETRYCOLLECTION (POINT (10 10), POINT (30 30), LINESTRING (15 15, 20 20))"),
                GeometryFromWKT.Parse("MULTIPOLYGON (((0 0, 10 0, 10 10, 0 10, 0 0)), ((5 5, 7 5, 7 7, 5 7, 5 5)))"),
                GeometryFromWKT.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)"),
                GeometryFromWKT.Parse("MULTILINESTRING ((10 10, 40 50), (20 20, 30 20), (20 20, 50 20, 50 60, 20 20))"),
                GeometryFromWKT.Parse("POLYGON ((20 20, 20 30, 30 30, 30 20, 20 20), (21 21, 29 21, 29 29, 21 29, 21 21), (23 23, 23 27, 27 27, 27 23, 23 23))"),
                GeometryFromWKT.Parse("POINT (20.564 346.3493254)"),
                GeometryFromWKT.Parse("MULTIPOINT (20.564 346.3493254, 45 32, 23 54)"),
                GeometryFromWKT.Parse("MULTIPOLYGON EMPTY"),
                GeometryFromWKT.Parse("MULTILINESTRING EMPTY"),
                GeometryFromWKT.Parse("MULTIPOINT EMPTY"),
                GeometryFromWKT.Parse("LINESTRING EMPTY")
            };

            return new DataTableFeatureProvider(geoms);
        }
    }
}