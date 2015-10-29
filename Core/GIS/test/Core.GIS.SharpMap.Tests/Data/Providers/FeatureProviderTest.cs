using Core.Common.Utils.Collections.Generic;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Converters.WellKnownText;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMapTestUtils.TestClasses;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class FeatureProviderTest
    {
        [Test]
        public void CreateFromCollection()
        {
            EventedList<SampleFeature> features = new EventedList<SampleFeature>();

            features.Add(new SampleFeature());
            features.Add(new SampleFeature());
            features.Add(new SampleFeature());

            features[0].Geometry = GeometryFromWKT.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)");
            features[1].Geometry = GeometryFromWKT.Parse("LINESTRING (30 30, 30 40, 40 40, 40 30, 50 30)");
            features[2].Geometry = GeometryFromWKT.Parse("LINESTRING (40 40, 40 50, 50 50, 50 40, 60 40)");

            FeatureCollection featureCollection = new FeatureCollection
            {
                Features = features
            };

            Map.Map map = new Map.Map();

            VectorLayer vectorLayer = new VectorLayer();
            vectorLayer.DataSource = featureCollection;

            map.Layers.Add(vectorLayer);

            Assert.AreEqual(3, vectorLayer.DataSource.GetFeatureCount());
            Assert.AreEqual("LineString", vectorLayer.DataSource.GetGeometryByID(0).GeometryType);

            // ShowMap(map);
        }

        [Test]
        public void AddFeatureUsingGeometry()
        {
            EventedList<SampleFeature> features = new EventedList<SampleFeature>();

            FeatureCollection featureCollection = new FeatureCollection
            {
                Features = features
            };

            IGeometry geometry = GeometryFromWKT.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)");
            featureCollection.Add(geometry);

            Assert.AreEqual(1, features.Count);
            Assert.IsTrue(features[0].Geometry is ILineString);
        }
    }
}