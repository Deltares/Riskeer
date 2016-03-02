using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;

using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class PointsBasedMapDataTest
    {
        [Test]
        public void Constructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestPointBasedMapData(null, "some name");

            // Assert
            var expectedMessage = "A point collection is required when creating a subclass of Core.Components.Gis.Data.PointBasedMapData.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_NullName_ThrowsArgumentNullExcpetion()
        {
            // Setup
            var points = new Collection<Point2D>();

            // Call
            TestDelegate test = () => new TestPointBasedMapData(points, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_EmptyName_ThrowsArgumentNullExcpetion()
        {
            // Setup
            var points = new Collection<Point2D>();

            // Call
            TestDelegate test = () => new TestPointBasedMapData(points, "");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_WithPoints_PropertiesSet()
        {
            // Setup
            var points = new Collection<Point2D>
            {
                new Point2D(0.0, 1.0),
                new Point2D(2.5, 1.1)
            };

            // Call
            var data = new TestPointBasedMapData(points, "some name");

            // Assert
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
            Assert.IsTrue(data.IsVisible);
        }

        [Test]
        public void Constructor_WithName_SetsName()
        {
            // Setup
            var points = new Collection<Point2D>();
            var name = "Some name";

            // Call
            var data = new TestPointBasedMapData(points, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        private class TestPointBasedMapData : PointBasedMapData
        {
            public TestPointBasedMapData(IEnumerable<Point2D> points, string name) : base(points, name) { }
        }
    }
}