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
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new MapDataChild(invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "A name must be set to map data");
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            var name = "Some name";

            // Call
            var data = new MapDataChild(name);

            // Assert
            Assert.IsInstanceOf<Observable>(data);
            Assert.AreEqual(name, data.Name);
        }

        [Test]
        public void Name_SetName_ReturnsNewName()
        {
            // setup
            var name = "Some name";
            var newName = "Something";
            var data = new MapDataChild(name);

            // Precondition
            Assert.AreEqual(name, data.Name);

            // Call
            data.Name = newName;

            // Assert
            Assert.AreNotEqual(name, data.Name);
            Assert.AreEqual(newName, data.Name);

        }

        private class MapDataChild : MapData {
            public MapDataChild(string name) : base(name) {}
        }
    }
}
