using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial;
using Core.Components.Gis.Data;
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
            Assert.NotNull(mapView.Map);
        }

        [Test]
        public void Data_SetToNull_BaseMapNoFeatures()
        {
            // Setup
            var mapView = new MapDataView();
            var map = (BaseMap) mapView.Controls[0];

            // Call
            TestDelegate testDelegate = () => mapView.Data = null;

            // Assert
            Assert.DoesNotThrow(testDelegate);
            Assert.IsNull(map.Data);
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
        public void Data_SetToMapPointData_MapDataSet()
        {
            // Setup
            var mapView = new MapDataView();
            var map = (BaseMap) mapView.Controls[0];
            var pointData = new MapPointData(Enumerable.Empty<Tuple<double, double>>());

            // Call
            mapView.Data = pointData;

            // Assert
            Assert.AreSame(pointData, map.Data);
            Assert.AreSame(pointData, mapView.Data);
        }
    }
}