using Core.GIS.SharpMap.Extensions.Layers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Extensions.Test.Layers
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
    }
}