using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Data;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Data
{
    [TestFixture]
    public class FeatureDataSetTest
    {
        [Test]
        public void GetSetAttributesViaIFeature()
        {
            var featureTable = new FeatureDataTable();
            featureTable.Columns.Add("name", typeof(string));
            featureTable.Columns.Add("attribute1", typeof(int));

            var feature1 = featureTable.NewRow();
            feature1.Geometry = new Point(0, 0);
            feature1["name"] = "feature1";
            feature1["attribute1"] = 1;
            featureTable.Rows.Add(feature1);

            // now access it using IFeature iterfaces
            IFeature f1 = feature1;

            Assert.AreEqual(2, f1.Attributes.Count);

            CollectionAssert.AreEqual(new[]
              {
                  "name",
                  "attribute1"
              },f1.Attributes.Keys);

            Assert.AreEqual("feature1", f1.Attributes["name"]);
            Assert.AreEqual(1, f1.Attributes["attribute1"]);
        }
    }
}