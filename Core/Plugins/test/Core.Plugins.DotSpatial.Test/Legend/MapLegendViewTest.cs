using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Plugins.DotSpatial.Legend;
using Core.Plugins.DotSpatial.Properties;
using NUnit.Framework;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapLegendViewTest
    {
        [Test]
        public void DefaultConstructor_CreatesUserControl()
        {
            // Call
            var view = new MapLegendView();

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsNull(view.Data);
            Assert.AreEqual(Resources.General_Map, view.Text);
        }

        [Test]
        public void DefaultConstructor_CreatesTreeViewControl()
        {
            // Call
            var view = new MapLegendView();

            var treeView = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

            // Assert
            Assert.IsNotNull(treeView);
            Assert.IsInstanceOf<TreeViewControl>(treeView);
        }

        [Test]
        public void Data_MapDataCollection_DataSet()
        {
            // Setup
            var view = new MapLegendView();
            var mapData = new MapDataCollection(new List<MapData>());

            // Call
            view.Data = mapData;

            // Assert
            Assert.AreSame(mapData, view.Data);
            Assert.IsInstanceOf<MapData>(view.Data);
        }


        [Test]
        public void Data_MapPointData_DataSet()
        {
            // Setup
            var view = new MapLegendView();
            var mapData = new MapPointData(Enumerable.Empty<Point2D>());

            // Call
            view.Data = mapData;

            // Assert
            Assert.AreSame(mapData, view.Data);
            Assert.IsInstanceOf<MapData>(view.Data);
        }

        [Test]
        public void Data_MapLineData_DataSet()
        {
            // Setup
            var view = new MapLegendView();
            var mapData = new MapLineData(Enumerable.Empty<Point2D>());

            // Call
            view.Data = mapData;

            // Assert
            Assert.AreSame(mapData, view.Data);
            Assert.IsInstanceOf<MapData>(view.Data);
        }

        [Test]
        public void Data_MapPolygonData_DataSet()
        {
            // Setup
            var view = new MapLegendView();
            var mapData = new MapPolygonData(Enumerable.Empty<Point2D>());

            // Call
            view.Data = mapData;

            // Assert
            Assert.AreSame(mapData, view.Data);
            Assert.IsInstanceOf<MapData>(view.Data);
        }

        [Test]
        public void Data_MapMultiLineData_DataSet()
        {
            // Setup
            var view = new MapLegendView();
            var mapData = new MapMultiLineData(Enumerable.Empty<IEnumerable<Point2D>>());

            // Call
            view.Data = mapData;

            // Assert
            Assert.AreSame(mapData, view.Data);
            Assert.IsInstanceOf<MapData>(view.Data);
        }

        [Test]
        public void Data_ForNull_NullSet()
        {
            // Setup
            var view = new MapLegendView();

            // Call
            view.Data = null;

            // Assert
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            var view = new MapLegendView();

            // Call
            TestDelegate test = () => view.Data = new object();

            // Assert
            Assert.Throws<InvalidCastException>(test);
        }
    }
}