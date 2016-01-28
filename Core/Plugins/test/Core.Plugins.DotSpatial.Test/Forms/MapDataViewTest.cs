using System;
using System.IO;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Components.DotSpatial;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Forms;
using NUnit.Framework;

namespace Core.Plugins.DotSpatial.Test.Forms
{
    [TestFixture]
    public class MapDataViewTest
    {
        private readonly string dijkvakgebiedenFile = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Core.Plugins.DotSpatial, "ShapeFiles"), "DR10_dijkvakgebieden.shp");

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var mapView = new MapDataView();

            // Assert
            Assert.IsInstanceOf<UserControl>(mapView);
            Assert.IsInstanceOf<IView>(mapView);
        }

        [Test]
        public void DefaultConstructor_Always_AddsBaseMap()
        {
            // Call
            var mapView = new MapDataView();

            // Assert
            Assert.AreEqual(1, mapView.Controls.Count);
            object mapObject = mapView.Controls[0];
            Assert.IsInstanceOf<BaseMap>(mapObject);

            var map = (BaseMap) mapObject;
            Assert.AreEqual(DockStyle.Fill, map.Dock);
        }

        [Test]
        public void Data_SetToNull_DoesNotThrowException()
        {
            // Setup
            var mapView = new MapDataView();

            // Call
            TestDelegate testDelegate = () => mapView.Data = null;

            // Assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void Data_SetToObject_ThrowsInvalidCastException()
        {
            // Setup
            var mapView = new MapDataView();

            // Call
            TestDelegate testDelegate = () => mapView.Data = new object();

            // Assert
            Assert.Throws<InvalidCastException>(testDelegate);
        }

        [Test]
        public void Data_Always_IsMapData()
        {
            // Setup
            var mapData = new MapData();
            var mapView = new MapDataView();
            mapData.AddShapeFile(dijkvakgebiedenFile);
            mapView.Data = mapData;

            // Call
            var data = mapView.Data;

            // Assert
            Assert.IsInstanceOf<MapData>(data);
        }
    }
}