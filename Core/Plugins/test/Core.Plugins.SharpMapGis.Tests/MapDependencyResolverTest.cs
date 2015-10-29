using Core.GIS.SharpMap.Map;
using NUnit.Framework;

namespace Core.Plugins.SharpMapGis.Tests
{
    [TestFixture]
    public class MapDependencyResolverTest
    {
        [Test]
        public void RemoveBackgroundMapFromMap()
        {
            var backgroundMap = new Map();
            var backGroundMapLayer = new BackGroundMapLayer(backgroundMap);
            var map = new Map
            {
                Layers =
                {
                    backGroundMapLayer
                }
            };

            MapDependencyResolver.RemoveItemsFromMap(map, backgroundMap);

            Assert.AreEqual(0, map.Layers.Count);
        }
    }
}