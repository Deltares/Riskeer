using System;
using System.Linq;

using Core.Common.TestUtil;
using Core.Components.Gis.Data;

using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataCollectionTest
    {
        [Test]
        public void Constructor_NullList_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapDataCollection(null, "test data");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A list collection is required when creating MapDataCollection.");
        }

        [Test]
        public void Constructor_NullName_ThrowsArgumentNullExcpetion()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();

            // Call
            TestDelegate test = () => new MapDataCollection(list, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_EmptyName_ThrowsArgumentNullExcpetion()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();

            // Call
            TestDelegate test = () => new MapDataCollection(list, "");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_ListSet_InstanceWithListSet()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();

            // Call
            var collection = new MapDataCollection(list, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(collection);
            Assert.AreSame(list, collection.List);
        }

        [Test]
        public void Constructor_WithName_SetsName()
        {
            // Setup
            var list = Enumerable.Empty<MapData>().ToList();
            var name = "Some name";

            // Call
            var data = new MapDataCollection(list, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }
    }
}