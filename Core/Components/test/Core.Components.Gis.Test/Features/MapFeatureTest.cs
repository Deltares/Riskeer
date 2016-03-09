using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Features
{
    [TestFixture]
    public class MapFeatureTest
    {
        [Test]
        public void ParameteredConstructor_WithMapGeometries_MapGeometriesAndDefaultValuesSet()
        {
            // Setup
            var mapGeometries = new List<MapGeometry>
            {
                new MapGeometry(Enumerable.Empty<Point2D>())
            };

            // Call
            var mapFeature = new MapFeature(mapGeometries);

            // Assert
            CollectionAssert.IsEmpty(mapFeature.MetaData);
            CollectionAssert.AreEqual(mapGeometries, mapFeature.MapGeometries);
        }

        [Test]
        public void ParameteredConstructor_WithoutMapGeometries_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapFeature(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "MapFeature cannot be created without map geometries.");
        }

        [Test]
        public void MetaData_Always_ReturnsMetaData()
        {
            // Setup
            var mapFeature = new MapFeature(Enumerable.Empty<MapGeometry>());
            var testMetaData = new KeyValuePair<string, object>("test", new object());
            mapFeature.MetaData.Add(testMetaData);

            // Call
            var featureMetaData = mapFeature.MetaData;

            // Assert
            Assert.AreEqual(1, featureMetaData.Count);
            Assert.IsTrue(featureMetaData.ContainsKey(testMetaData.Key));
            Assert.AreEqual(testMetaData.Value, featureMetaData[testMetaData.Key]);
        }
    }
}
