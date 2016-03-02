using System;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapDataTest
    {
        [Test]
        public void Constructor_Values()
        {
            // Call
            var mapData = new MapDataChild("test data");

            // Assert
            Assert.IsInstanceOf<Observable>(mapData);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapDataChild(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "A name must be set to map data");
        }

        [Test]
        public void Constructor_NameEmpty_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapDataChild("");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "A name must be set to map data");
        }

        [Test]
        public void Constructor_WithName_SetsName()
        {
            // Setup
            var name = "Some name";

            // Call
            var data = new MapDataChild(name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        private class MapDataChild : MapData {
            public MapDataChild(string name) : base(name) {}
        }
    }
}
