using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapPointDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPointData(null, "test data");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, string.Format("A feature collection is required when creating a subclass of {0}.", typeof(FeatureBasedMapData)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            // Call
            TestDelegate test = () => new MapPointData(features, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(7)]
        public void Constructor_InvalidGeometryConfiguration_ThrowArgumentException(int numberOfPointCollections)
        {
            // Setup
            var invalidPointsCollections = new IEnumerable<Point2D>[numberOfPointCollections];
            for (int i = 0; i < numberOfPointCollections; i++)
            {
                invalidPointsCollections[i] = CreateTestPoints();
            }
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(invalidPointsCollections),
                })
            };

            // Call
            TestDelegate call = () => new MapPointData(features, "Some invalid map data");

            // Assert
            string expectedMessage = "MapPointData only accept MapFeature instances whose MapGeometries contain a single point-collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewMapPointData()
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            // Call
            var data = new MapPointData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapPointData()
        {
            // Setup
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        CreateTestPoints()
                    })
                })
            };

            // Call
            var data = new MapPointData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
            CollectionAssert.AreEqual(CreateTestPoints(), data.Features.First().MapGeometries.First().PointCollections.First());
        }

        private Collection<Point2D> CreateTestPoints()
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