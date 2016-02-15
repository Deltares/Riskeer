using System;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;

using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapDataCollectionTest
    {
        [Test]
        public void Constructor_NullList_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapDataCollection(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A list collection is required when creating MapDataCollection.");
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