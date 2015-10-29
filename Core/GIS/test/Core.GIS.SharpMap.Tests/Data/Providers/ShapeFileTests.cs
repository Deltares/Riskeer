using Core.Common.TestUtils;
using Core.Gis.GeoApi.Extensions.Feature;
using Core.GIS.SharpMap.Api;
using Core.GIS.SharpMap.Data.Providers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class ShapeFileTests
    {
        // TODO: write tests for shapefile

        [Test]
        public void ContainsShouldWorkForShapeFile()
        {
            string path = TestHelper.GetTestDataPath(TestDataPath.DeltaShell.DeltaShellDeltaShellPluginsSharpMapGisTests, "Europe_Lakes.shp");
            var s = new ShapeFile(path);
            var feature = s.Features[0];
            s.Contains((IFeature) feature); // -> should not throw an exception
        }

        [Test]
        [Ignore("Should we drop the ShapeFile class and continue with the OgrFeatureProvider?")]
        public void GetFeatureShouldWorkForShapeFile()
        {
            string path = @"..\..\..\..\data\Europe_Lakes.shp";
            var s = new ShapeFile(path);
            var feature = s.Features[1];
            Assert.LessOrEqual(0, s.IndexOf((IFeature) feature));
        }

        [Test]
        [Ignore("Should we drop the ShapeFile class and continue with the OgrFeatureProvider?")]
        public void GetFeatureShouldWorkForShapeFileWithoutObjectID()
        {
            string path = @"..\..\..\..\data\gemeenten.shp";
            var s = new ShapeFile(path);
            var feature = s.Features[0];
            Assert.LessOrEqual(0, s.IndexOf((IFeature) feature));
        }

        [Test]
        public void FeatureCount()
        {
            string path = TestHelper.GetTestDataPath(TestDataPath.DeltaShell.DeltaShellDeltaShellPluginsSharpMapGisTests, "Europe_Lakes.shp");
            IFeatureProvider dataSource = new ShapeFile(path);
            Assert.AreEqual(37, dataSource.Features.Count);
        }
    }
}