using System.Collections.ObjectModel;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Converters.WellKnownText;
using Core.GIS.SharpMap.Data;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Data.Providers
{
    [TestFixture]
    public class GeometryProviderTest
    {
        [Test]
        public void SetAttributes()
        {
            Collection<IGeometry> geometries = new Collection<IGeometry>();

            geometries.Add(GeometryFromWKT.Parse("LINESTRING (20 20, 20 30, 30 30, 30 20, 40 20)"));
            geometries.Add(GeometryFromWKT.Parse("LINESTRING (21 21, 21 31, 31 31, 31 21, 41 21)"));
            geometries.Add(GeometryFromWKT.Parse("LINESTRING (22 22, 22 32, 32 32, 32 22, 42 22)"));

            DataTableFeatureProvider dataTableFeatureFeatureProvider = new DataTableFeatureProvider(geometries);

            VectorLayer vectorLayer = new VectorLayer();
            vectorLayer.DataSource = dataTableFeatureFeatureProvider;

            // add column
            FeatureDataRow r = (FeatureDataRow) vectorLayer.DataSource.GetFeature(0);
            r.Table.Columns.Add("Value", typeof(float));

            // set value attribute
            for (int i = 0; i < dataTableFeatureFeatureProvider.GetFeatureCount(); i++)
            {
                r = (FeatureDataRow) dataTableFeatureFeatureProvider.GetFeature(i);
                r[0] = i;
            }

            FeatureDataRow row = (FeatureDataRow) dataTableFeatureFeatureProvider.GetFeature(2);
            Assert.AreEqual(2, row[0], "Attribute 0 in the second feature must be set to 2");
        }

        [Test]
        public void InitializationTest()
        {
            DataTableFeatureProvider gp = new DataTableFeatureProvider("LINESTRING(20 20,40 40)");
            Assert.AreEqual(1, gp.Geometries.Count);
        }
    }
}