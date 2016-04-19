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
    public class MapPolygonDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapPolygonData(null, "test data");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, string.Format("A feature collection is required when creating a subclass of {0}.", typeof(FeatureBasedMapData)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
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
            TestDelegate test = () => new MapPolygonData(features, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(5)]
        public void Constructor_InvalidGeometryConfiguration_ThrowArgumentException(int numberOfPointCollections)
        {
            // Setup
            var invalidPointsCollections = new IEnumerable<Point2D>[numberOfPointCollections];
            for (int i = 0; i < numberOfPointCollections; i++)
            {
                invalidPointsCollections[i] = CreateOuterRingPoint2Ds();
            }
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(invalidPointsCollections),
                })
            };

            // Call
            TestDelegate call = () => new MapPolygonData(features, "Some invalid map data");

            // Assert
            string expectedMessage = "MapPolygonData only accept MapFeature instances whose MapGeometries contain a single point-collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewMapPolygonData()
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
            var data = new MapPolygonData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapPolygonData()
        {
            // Setup
            var features = new Collection<MapFeature> 
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(CreateTestPointsCollections())
                })
            };

            // Call
            var data = new MapPolygonData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
            CollectionAssert.AreEqual(CreateTestPointsCollections(), data.Features.First().MapGeometries.First().PointCollections);
        }

        [Test]
        public void Constructor_WithName_SetsName()
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
            var name = "Some name";

            // Call
            var data = new MapPolygonData(features, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        private static IEnumerable<IEnumerable<Point2D>> CreateTestPointsCollections()
        {
            return new[]
            {
                CreateOuterRingPoint2Ds()
            };
        }

        private static Point2D[] CreateOuterRingPoint2Ds()
        {
            return new []
            {
                new Point2D(0.0, 1.1),
                new Point2D(1.0, 2.1),
                new Point2D(1.6, 1.6)
            };
        }
    }
}