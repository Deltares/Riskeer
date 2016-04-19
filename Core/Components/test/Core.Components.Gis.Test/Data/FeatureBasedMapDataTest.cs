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
    public class FeatureBasedMapDataTest
    {
        [Test]
        public void Constructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestFeatureBasedMapData(null, "some name");

            // Assert
            var expectedMessage = "A feature collection is required when creating a subclass of Core.Components.Gis.Data.FeatureBasedMapData.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("        ")]
        public void Constructor_InvalidName_ThrowsArgumentExcpetion(string invalidName)
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
            TestDelegate test = () => new TestFeatureBasedMapData(features, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_WithPoints_PropertiesSet()
        {
            // Setup
            var points = new[]
            {
                new Point2D(0.0, 1.0),
                new Point2D(2.5, 1.1)
            };

            var features = new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        points
                    })
                })
            };

            // Call
            var data = new TestFeatureBasedMapData(features, "some name");

            // Assert
            Assert.AreNotSame(features, data.Features);
            Assert.AreEqual(features.Length, data.Features.Count());
            Assert.AreEqual(features[0].MapGeometries.Count(), data.Features.First().MapGeometries.Count());
            CollectionAssert.AreEqual(points, data.Features.First().MapGeometries.First().PointCollections.First());
            Assert.IsTrue(data.IsVisible);
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
            var data = new TestFeatureBasedMapData(features, name);

            // Assert
            Assert.AreEqual(name, data.Name);
        }

        private class TestFeatureBasedMapData : FeatureBasedMapData
        {
            public TestFeatureBasedMapData(IEnumerable<MapFeature> features, string name) : base(features, name) { }
        }
    }
}