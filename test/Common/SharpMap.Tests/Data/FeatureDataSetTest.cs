using GeoAPI.Extensions.Feature;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Data;
using SharpTestsEx;

namespace SharpMap.Tests.Data
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

            f1.Attributes.Count
              .Should().Be.EqualTo(2);

            f1.Attributes.Keys
              .Should().Have.SameSequenceAs(new[]
              {
                  "name",
                  "attribute1"
              });

            f1.Attributes["name"]
                .Should().Be.EqualTo("feature1");

            f1.Attributes["attribute1"]
                .Should().Be.EqualTo(1);
        }
    }
}