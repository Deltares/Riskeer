using Core.Common.TestUtil;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Data.Providers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Data.Providers
{
    [TestFixture]
    public class ShapeFileTests
    {
        [Test]
        public void ContainsShouldWorkForShapeFile()
        {
            var path = TestHelper.GetDataDir() + @"\Europe_Lakes.shp";
            var s = new ShapeFile(path);
            var feature = s.Features[0];
            s.Contains((IFeature) feature); // -> should not throw an exception
        }

        [Test]
        public void GetFeatureShouldWorkForShapeFile()
        {
            var path = TestHelper.GetDataDir() + @"\Europe_Lakes.shp";
            var s = new ShapeFile(path);
            var feature = s.Features[1];
            Assert.LessOrEqual(0, s.IndexOf((IFeature) feature));
        }

        [Test]
        public void GetFeatureShouldWorkForShapeFileWithoutObjectID()
        {
            var path = TestHelper.GetDataDir() + @"\gemeenten.shp";
            var s = new ShapeFile(path);
            var feature = s.Features[0];
            Assert.LessOrEqual(0, s.IndexOf((IFeature) feature));
        }

        [Test]
        public void FeatureCount()
        {
            var path = TestHelper.GetDataDir() + @"\Europe_Lakes.shp";
            IFeatureProvider dataSource = new ShapeFile(path);
            Assert.AreEqual(37, dataSource.Features.Count);
        }
    }
}