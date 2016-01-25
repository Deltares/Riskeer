using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Forms;
using NUnit.Framework;

namespace Core.Plugins.DotSpatial.Test.Forms
{
    [TestFixture]
    public class MapDataViewTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var mapView = new MapDataView();

            // Assert
            Assert.IsInstanceOf<UserControl>(mapView);
            var interfaceImplementation = mapView as IView;
            Assert.IsNotNull(interfaceImplementation);
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
        public void Data_SetToNull_ThrowsArgumentNullException()
        {
            // Setup
            var mapView = new MapDataView();

            // Call
            TestDelegate testDelegate = () => mapView.Data = null;

            // Assert
            Assert.Throws<ArgumentNullException>(testDelegate);
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
            var filePath = string.Format("{0}\\Resources\\DR10_dijkvakgebieden.shp", Environment.CurrentDirectory);
            mapData.AddShapeFile(filePath);
            mapView.Data = mapData;

            // Call
            var data = mapView.Data;

            // Assert
            Assert.IsInstanceOf<MapData>(data);
        }
    }
}