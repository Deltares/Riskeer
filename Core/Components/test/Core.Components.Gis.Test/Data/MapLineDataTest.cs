using System;
using System.Collections.ObjectModel;

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
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new MapLineData(points);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(points, data.Points);
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
        }

        private static Collection<Tuple<double, double>> CreateTestPoints()
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