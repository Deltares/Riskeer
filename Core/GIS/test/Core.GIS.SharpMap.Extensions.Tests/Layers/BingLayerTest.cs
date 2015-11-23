using Core.GIS.SharpMap.Extensions.Layers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Extensions.Tests.Layers
{
    [TestFixture]
    public class BingLayerTest
    {
        [Test]
        public void Clone()
        {
            var map = new Map.Map();

            var layer = new BingLayer
            {
                MapType = "Aerial"
            };
            map.Layers.Add(layer);

            var clone = (BingLayer) layer.Clone();
            Assert.IsNull(clone.Map);
        }

        [Test]
        public void CacheDirectoryIsDefined()
        {
            StringAssert.EndsWith(@"cache_bing_aerial", new BingLayer
            {
                MapType = "Aerial"
            }.CacheLocation, "Cache directory is defined");
        }
    }
}