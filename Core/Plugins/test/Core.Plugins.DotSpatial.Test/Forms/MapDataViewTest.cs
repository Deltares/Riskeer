using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial;
using Core.Plugins.DotSpatial.Forms;
using DotSpatial.Controls;
using NUnit.Framework;

namespace Core.Plugins.DotSpatial.Test.Forms
{
    [TestFixture]
    public class MapDataViewTest
    {
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
        public void Data_SetToNull_BaseMapNoData()
        {
            // Setup
            var mapView = new MapDataView();
            var map = (BaseMap) mapView.Controls[0];

            // Call
            mapView.Data = null;

            // Assert
            Assert.IsNull(map.Data);
        }

        [Test]
        public void Data_SetToNull_MapNotUpdated()
        {
            // Setup
            var mapView = new MapDataView();
            var baseMap = (BaseMap)mapView.Controls[0];

            var map = (Map) TypeUtils.GetField(baseMap, "map");

            //Pre-condition
            var preLayerCount = map.GetLayers().Count;

            // Call
            baseMap.Data = null;

            //Assert
            Assert.AreEqual(preLayerCount, map.GetLayers().Count);
        }
    }
}