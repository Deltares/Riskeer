using Core.GIS.SharpMap.Extensions.Layers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Extensions.Test.Layers
{
    [TestFixture]
    public class OpenStreetMapLayerTest
    {
        [Test]
        public void CacheDirectoryIsDefined()
        {
            StringAssert.EndsWith(@"cache_open_street_map", OpenStreetMapLayer.CacheLocation, "Cache directory is defined");
        }
    }
}