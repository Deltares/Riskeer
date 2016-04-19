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
    public class MapLineDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLineData(null, "test data");

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, string.Format("A feature collection is required when creating a subclass of {0}.", typeof(FeatureBasedMapData)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate test = () => new MapLineData(Enumerable.Empty<MapFeature>(), invalidName);

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
                invalidPointsCollections[i] = CreateSingleLinePoints();
            }
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(invalidPointsCollections),
                })
            };

            // Call
            TestDelegate call = () => new MapLineData(features, "Some invalid map data");

            // Assert
            string expectedMessage = "MapLineData only accept MapFeature instances whose MapGeometries contain a single point-collection.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_WithEmptyMapGeometryPoints_CreatesNewMapLineData()
        {
            // Setup
            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        Enumerable.Empty<Point2D>()
                    })
                })
            };

            // Call
            var data = new MapLineData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
            CollectionAssert.IsEmpty(data.Features.First().MapGeometries.First().PointCollections.First());
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapLineData()
        {
            // Setup
            var features = new Collection<MapFeature>
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(CreateTestPointCollections())
                })
            };

            // Call
            var data = new MapLineData(features, "test data");

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(features, data.Features);
            CollectionAssert.AreEqual(CreateTestPointCollections(), data.Features.First().MapGeometries.First().PointCollections);
            CollectionAssert.IsEmpty(data.Features.First().MetaData);
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
            var data = new MapLineData(features, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        [Test]
        public void MetaData_SetNewValue_GetNewlySetValue()
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

            var data = new MapLineData(features, "test data");

            const string key = "<some key>";
            var newValue = new object();

            // Call
            data.Features.First().MetaData[key] = newValue;

            // Assert
            Assert.AreEqual(newValue, data.Features.First().MetaData[key]);
        }

        private static IEnumerable<IEnumerable<Point2D>> CreateTestPointCollections()
        {
            return new[]
            {
                CreateSingleLinePoints()
            };
        }

        private static Point2D[] CreateSingleLinePoints()
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