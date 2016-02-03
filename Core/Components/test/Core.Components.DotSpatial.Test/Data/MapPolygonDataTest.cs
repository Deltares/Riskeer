using System;
using System.Collections.ObjectModel;
using Core.Components.DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class MapPolygonDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPolygonData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewICharData()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new MapPolygonData(points);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(points, data.Points);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new MapPolygonData(points);

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