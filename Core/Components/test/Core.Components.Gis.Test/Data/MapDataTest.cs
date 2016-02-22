using Core.Common.Base;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataTest
    {
        [Test]
        public void DefaultConstructor_Values()
        {
            // Call
            var mapData = new MapDataChild();

            // Assert
            Assert.IsInstanceOf<Observable>(mapData);
        }

        private class MapDataChild : MapData { }
    }
}
