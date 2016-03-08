using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
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
            using (var mapView = new MapDataView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(mapView);
                Assert.IsInstanceOf<IView>(mapView);
            }
        }

        [Test]
        public void DefaultConstructor_Always_AddsMapControl()
        {
            // Call
            using (var mapView = new MapDataView())
            {
                // Assert
                Assert.AreEqual(1, mapView.Controls.Count);
                object mapObject = mapView.Controls[0];
                Assert.IsInstanceOf<MapControl>(mapObject);

                var map = (MapControl)mapObject;
                Assert.AreEqual(DockStyle.Fill, map.Dock);
                Assert.NotNull(mapView.Map);
            }
        }

        [Test]
        public void Data_SetToNull_MapControlNoFeatures()
        {
            // Setup
            using (var mapView = new MapDataView())
            {
                var map = (MapControl)mapView.Controls[0];

                // Call
                TestDelegate testDelegate = () => mapView.Data = null;

                // Assert
                Assert.DoesNotThrow(testDelegate);
                Assert.IsNull(map.Data);
            }
        }

        [Test]
        public void Data_SetToObject_ThrowsInvalidCastException()
        {
            // Setup
            using (var mapView = new MapDataView())
            {
                // Call
                TestDelegate testDelegate = () => mapView.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(testDelegate);
            }
        }

        [Test]
        public void Data_SetToMapPointData_MapDataSet()
        {
            // Setup
            var features = new Collection<MapFeature> 
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(Enumerable.Empty<Point2D>())
                })
            };

            using (var mapView = new MapDataView())
            {
                var map = (MapControl)mapView.Controls[0];
                var pointData = new MapPointData(features, "test data");

                // Call
                mapView.Data = pointData;

                // Assert
                Assert.AreSame(pointData, map.Data);
                Assert.AreSame(pointData, mapView.Data);
            }
        }
    }
}