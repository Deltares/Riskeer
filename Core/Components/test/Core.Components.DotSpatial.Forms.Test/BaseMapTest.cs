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
            var mapView = TypeUtils.GetField<Map>(map, "map");

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
            var mapView = TypeUtils.GetField<Map>(map, "map");

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
        public void Data_NewDataSet_BaseMapDetachedFromOldAttachedToNewFeaturesUpdated()
        {
            // Setup
            var map = new BaseMap();
            var testDataOld = new MapPointData(Enumerable.Empty<Point2D>());
            var testDataNew = new MapPointData(Enumerable.Empty<Point2D>());

            var observersOld = TypeUtils.GetField<ICollection<IObserver>>(testDataOld, "observers");
            var observersNew = TypeUtils.GetField<ICollection<IObserver>>(testDataNew, "observers");
            var view = TypeUtils.GetField<Map>(map, "map");

            // Call
            map.Data = testDataOld;
            map.Data = testDataNew;

            // Assert
            CollectionAssert.IsEmpty(observersOld);
            CollectionAssert.AreEqual(new[] { map }, observersNew);
            Assert.AreEqual(1, view.Layers.Count);
        }

        [Test]
        [RequiresSTA]
        public void UpdateObserver_MapInForm_MapLayersRenewed()
        {
            // Setup
            var form = new Form();
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var view = TypeUtils.GetField<Map>(map, "map");

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