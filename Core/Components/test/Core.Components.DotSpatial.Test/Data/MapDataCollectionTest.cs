using System;
using System.Linq;
using Core.Components.DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapDataCollectionTest
    {
        [Test]
        public void Constructor_NullList_ThrowsArgumentNullException()
        {
            // Setup
            TestDelegate test = () => new MapDataCollection(null);

            // Call
            var message = Assert.Throws<ArgumentNullException>(test).Message;

            // Assert
            StringAssert.Contains("A list collection is required when creating MapDataCollection.", message);
        }

        [Test]
        public void Constructor_ListSet_InstanceWithListSet()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();

            // Call
            var collection = new MapDataCollection(list);

            // Assert
            Assert.IsInstanceOf<MapData>(collection);
            Assert.AreSame(list, collection.List);
        }
    }
}
