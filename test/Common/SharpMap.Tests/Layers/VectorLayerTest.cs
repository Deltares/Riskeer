using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using DelftTools.TestUtils;
using GeoAPI.Extensions.Feature;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;
using NetTopologySuite.Extensions.Features;
using NUnit.Framework;
using SharpMap.Api;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpTestsEx;

namespace SharpMap.Tests.Layers
{
    [TestFixture]
    public class VectorLayerTest
    {
        [Test]
        [ExpectedException(typeof(ReadOnlyException))]
        public void SettingNameOnNameIsReadOnlyLayerThrowsException()
        {
            var vectorLayer = new VectorLayer("test layer");
            vectorLayer.NameIsReadOnly = true;
            vectorLayer.Name = "new test layer";
        }

        [Test]
        public void EventBubbling()
        {
            var style = new VectorStyle();
            var vectorLayer = new VectorLayer("EventBubbling")
            {
                Style = style
            };
            var changeCount = 0;

            ((INotifyPropertyChanged) vectorLayer).PropertyChanged +=
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
        public void LoadFromFile()
        {
            string filePath = Path.GetFullPath(TestHelper.GetDataDir() + @"\rivers.shp");
            IFeatureProvider dataSource = new ShapeFile(filePath, false);
            VectorLayer vectorLayer = new VectorLayer("rivers", dataSource);
            Assert.AreEqual("rivers", vectorLayer.Name);
            Assert.AreEqual(dataSource, vectorLayer.DataSource);
        }

        [Test]
        public void NoExceptionShouldBeThrownWhenZoomLevelIsTooLarge()
        {
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

            var layer = new VectorLayer
            {
                DataSource = featureProvider
            };
            var map = new Map
            {
                Layers =
                {
                    layer
                },
                Size = new Size(1000, 1000)
            };
            map.Render();
            map.ZoomToFit(new Envelope(50000000, 50000001, 0, 1));

            map.Render();
        }

        [Test]
        public void RenderLabelsForVectorLayerFeatures()
        {
            var featureProvider = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 0)")
                    },
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(100 100)")
                    }
                }
            };

            var layer = new VectorLayer
            {
                DataSource = featureProvider,
                LabelLayer =
                {
                    Visible = true
                } // set labels on, happens in ThemeEditorDialog
            };

            // make label layer use delegate so that we can check if it renders
            var callCount = 0;
            layer.LabelLayer.LabelStringDelegate = delegate(IFeature feature)
            {
                callCount++;
                return feature.ToString();
            };

            var map = new Map
            {
                Layers =
                {
                    layer
                },
                Size = new Size(1000, 1000)
            };
            map.Render();

            callCount
                .Should("labels are rendered for 2 features of the vector layer").Be.EqualTo(2);
        }

        [Test]
        public void LabelsLayerIsInitializedAndOffByDefault()
        {
            var layer = new VectorLayer();
            layer.LabelLayer.Visible
                 .Should("label layer is off by default").Be.False();
        }

        [Test]
        public void DoNotRenderLabelsForVectorLayerFeaturesWhenLabelsAreNotVisible()
        {
            var featureProvider = new FeatureCollection
            {
                Features =
                {
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(0 0)")
                    },
                    new Feature
                    {
                        Geometry = new WKTReader().Read("POINT(100 100)")
                    }
                }
            };

            var layer = new VectorLayer
            {
                DataSource = featureProvider
            };

            // make label layer use delegate so that we can check if it renders
            var callCount = 0;
            layer.LabelLayer.LabelStringDelegate = delegate(IFeature feature)
            {
                callCount++;
                return feature.ToString();
            };

            var map = new Map
            {
                Layers =
                {
                    layer
                },
                Size = new Size(1000, 1000)
            };
            map.Render();

            callCount
                .Should("labels are not rendered when label layer is not visible").Be.EqualTo(0);
        }
    }
}