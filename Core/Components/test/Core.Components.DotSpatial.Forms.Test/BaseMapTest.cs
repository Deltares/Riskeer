using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.TestUtil;
using Core.Components.Gis.Data;

using DotSpatial.Controls;

using NUnit.Framework;

using IMap = Core.Components.Gis.IMap;

namespace Core.Components.DotSpatial.Forms.Test
{
    [TestFixture]
    public class BaseMapTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var map = new BaseMap();

            // Assert
            Assert.IsInstanceOf<Control>(map);
            Assert.IsInstanceOf<IMap>(map);
            Assert.IsNull(map.Data);
        }

        [Test]
        public void Data_UnknownMapData_ThrowsNotSupportedException()
        {
            // Setup
            var map = new BaseMap();
            var testData = new TestMapData();

            // Call
            TestDelegate test = () => map.Data = testData;

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void Data_Null_ReturnsNull()
        {
            // Setup
            var map = new BaseMap();

            // Call
            map.Data = null;

            // Assert
            Assert.IsNull(map.Data);
        }

        [Test]
        public void Data_NotNull_ReturnsData()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());

            // Call
            map.Data = testData;

            // Assert
            Assert.AreSame(testData, map.Data);
        }

        [Test]
        public void Data_KnownMapData_MapFeatureAdded()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var mapView = TypeUtils.GetField<Map>(map, "map");

            // Call
            map.Data = testData;

            // Assert
            Assert.AreEqual(1, mapView.Layers.Count);
        }

        [Test]
        public void Data_SetToNull_MapFeaturesCleared()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var mapView = TypeUtils.GetField<Map>(map, "map");

            map.Data = testData;

            // Precondition
            Assert.AreEqual(1, mapView.Layers.Count);

            // Call
            map.Data = null;

            // Assert
            Assert.IsNull(map.Data);
            Assert.AreEqual(0, mapView.Layers.Count);
        }
    }
}