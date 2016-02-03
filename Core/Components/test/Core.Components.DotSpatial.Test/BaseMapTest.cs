using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using Core.Components.DotSpatial.TestUtil;
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test
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
        public void Data_NotKnowMapData_ThrowsNotSupportedException()
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
            var testData = new MapPointData(Enumerable.Empty<Tuple<double, double>>());
            map.Data = testData;

            // Call
            var data = map.Data;

            // Assert
            Assert.AreSame(testData, data);
        }

        [Test]
        public void Data_KnownMapData_MapFeatureAdded()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Tuple<double, double>>());
            var mapView = TypeUtils.GetField<Map>(map, "map");

            // Call
            map.Data = testData;

            // Assert
            Assert.AreEqual(1, mapView.Layers.Count);
        }
    }
}