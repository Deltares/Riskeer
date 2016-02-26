﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;

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
        public void ZoomToAll_MapInForm_ViewInvalidatedLayersSame()
        {
            // Setup
            var form = new Form();
            var map = new BaseMap();
            var testData = new MapPointData(Enumerable.Empty<Point2D>());
            var mapView = map.Controls.OfType<Map>().First();
            var invalidated = 0;

            map.Data = testData;
            form.Controls.Add(map);

            mapView.Invalidated += (sender, args) => { invalidated++; };

            form.Show();
            Assert.AreEqual(0, invalidated, "Precondition failed: mapView.Invalidated > 0");

            // Call
            map.ZoomToAll();

            // Assert
            Assert.AreEqual(2, invalidated);
            Assert.AreEqual(mapView.GetMaxExtent(), mapView.ViewExtents);
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