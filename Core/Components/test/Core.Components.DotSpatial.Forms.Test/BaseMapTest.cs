using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
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
        public void Data_KnownMapData_MapFeatureAddedAndZoomedToExtents()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var mapView = map.Controls.OfType<Map>().First();

            // Call
            map.Data = testData;

            // Assert
            Assert.AreEqual(1, mapView.Layers.Count);
            Assert.AreEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
        }

        [Test]
        public void Data_SetToNull_BaseMapDetachedFeaturesCleared()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var observers = TypeUtils.GetField<ICollection<IObserver>>(testData, "observers");
            var mapView = map.Controls.OfType<Map>().First();

            map.Data = testData;

            // Precondition
            Assert.AreEqual(1, mapView.Layers.Count);

            // Call
            map.Data = null;

            // Assert
            Assert.IsNull(map.Data);
            CollectionAssert.IsEmpty(observers);
            CollectionAssert.IsEmpty(mapView.Layers);
        }

        [Test]
        public void Data_SetPointData_MapPointLayerAdded()
        {
            // Setup
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var mapView = map.Controls.OfType<Map>().First();

            // Call
            map.Data = testData;            

            // Assert
            Assert.IsInstanceOf<MapPointData>(map.Data);
            Assert.AreEqual(1, mapView.Layers.Count);
            Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);
        }

        [Test]
        public void UpdateObserver_UpdateData_UpdateMap()
        {
            // Setup
            var map = new BaseMap();
            var mapView = map.Controls.OfType<Map>().First();
            var testData = new MapDataCollection(new List<MapData>
            {
                new MapPointData(Enumerable.Empty<Point2D>())
            });

            map.Data = testData;

            // Precondition
            Assert.AreEqual(1, mapView.Layers.Count);
            Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);

            testData.List.Add(new MapLineData(Enumerable.Empty<Point2D>()));

            // Call
            map.UpdateObserver();

            // Assert
            Assert.AreEqual(2, mapView.Layers.Count);
            Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);
            Assert.IsInstanceOf<MapLineLayer>(mapView.Layers[1]);
        }

        [Test]
        public void Data__SetToNull_DetachObserver()
        {
            // Setup
            var map = new BaseMap();
            var mapView = map.Controls.OfType<Map>().First();
            var testData = new MapDataCollection(new List<MapData>
            {
                new MapPointData(Enumerable.Empty<Point2D>())
            });

            map.Data = testData;

            // Precondition
            Assert.AreEqual(1, mapView.Layers.Count);
            Assert.IsInstanceOf<MapPointLayer>(mapView.Layers[0]);

            map.Data = null;

            testData.List.Add(new MapPointData(Enumerable.Empty<Point2D>()));

            // Call
            map.UpdateObserver();

            // Assert
            Assert.IsNull(map.Data);
            Assert.AreEqual(0, mapView.Layers.Count);
        }	

        [Test]
        [RequiresSTA]
        public void UpdateObserver_MapInForm_MapLayersRenewed()
        {
            // Setup
            var form = new Form();
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var view = map.Controls.OfType<Map>().First();

            map.Data = testData;
            var layers = view.Layers.ToList();

            form.Controls.Add(map);

            form.Show();

            // Call
            map.UpdateObserver();

            // Assert
            Assert.AreEqual(1, view.Layers.Count);
            Assert.AreNotSame(layers[0], view.Layers[0]);
        }
    }
}