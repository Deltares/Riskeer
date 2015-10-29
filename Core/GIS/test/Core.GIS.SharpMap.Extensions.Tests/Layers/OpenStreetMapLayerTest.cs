using Core.GIS.SharpMap.Extensions.Layers;
using NUnit.Framework;
using SharpTestsEx;

namespace Core.GIS.SharpMap.Extensions.Tests.Layers
{
    [TestFixture]
    public class OpenStreetMapLayerTest
    {
        [Test]
        public void CacheDirectoryIsDefined()
        {
            OpenStreetMapLayer.CacheLocation
                              .Should("Cache directory is defined")
                              .EndWith(@"cache_open_street_map");
        }
    }
}