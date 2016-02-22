using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;

using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapLineDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLineData(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, string.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedMapData)));
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewMapLineData()
        {
            // Setup
            var points = new Collection<Point2D>();

            // Call
            var data = new MapLineData(points);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.IsEmpty(data.MetaData);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapLineData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new MapLineData(points);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
            CollectionAssert.IsEmpty(data.MetaData);
        }

        [Test]
        public void MetaData_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var data = new MapLineData(Enumerable.Empty<Point2D>());

            const string key = "<some key>";
            var newValue = new object();

            // Call
            data.MetaData[key] = newValue;

            // Assert
            Assert.AreEqual(newValue, data.MetaData[key]);
        }

        private static Collection<Point2D> CreateTestPoints()
        {
            return new Collection<Point2D>
            {
                new Point2D(0.0, 1.1),
                new Point2D(1.0, 2.1),
                new Point2D(1.6, 1.6)
            };
        }
    }
}