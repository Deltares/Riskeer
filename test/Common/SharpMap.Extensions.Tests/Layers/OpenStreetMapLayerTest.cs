using NUnit.Framework;
using SharpMap.Extensions.Layers;
using SharpTestsEx;

namespace SharpMap.Extensions.Tests.Layers
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