using System;
using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapPointDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPointData(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, string.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedMapData)));
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewMapPointData()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new MapPointData(points);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(points, data.Points);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapPointData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new MapPointData(points);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
        }

        private Collection<Tuple<double, double>> CreateTestPoints()
        {
            return new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6)
            };
        }
    }
}