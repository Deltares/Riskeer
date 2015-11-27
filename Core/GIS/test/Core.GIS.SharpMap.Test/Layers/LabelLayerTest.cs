using System.Drawing;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Extensions.Features;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using NUnit.Framework;
using Point = Core.GIS.NetTopologySuite.Geometries.Point;

namespace Core.GIS.SharpMap.Test.Layers
{
    [TestFixture]
    public class LabelLayerTest
    {
        [Test]
        public void SetGetText()
        {
            var map = new Map.Map(new Size(100, 100));

            var feature1 = new SimpleFeature
            {
                Data = 1, Geometry = new Point(50, 50)
            };
            var feature2 = new SimpleFeature
            {
                Data = 5, Geometry = new Point(50, 55)
            };
            var featureCollection = new FeatureCollection
            {
                Features =
                {
                    feature1, feature2
                }
            };

            var callCount = 0;

            var vectorLayer = new VectorLayer
            {
                DataSource = featureCollection
            };

            var labelLayer = new LabelLayer
            {
                Visible = true,
                DataSource = featureCollection,
                Parent = vectorLayer,
                Map = map,
                LabelStringDelegate = delegate(IFeature feature)
                {
                    callCount++;

                    Assert.IsInstanceOf<SimpleFeature>(feature);

                    return ((SimpleFeature) feature).Data.ToString();
                }
            };

            labelLayer.Render();

            Assert.AreEqual(2, callCount, "labels of 2 simple feature rendered");
        }

        private class SimpleFeature : Feature
        {
            public int Data { get; set; }
        }
    }
}