using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Plugins.DotSpatial.Legend;
using Core.Plugins.DotSpatial.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapLegendViewTest
    {
        private MockRepository mocks;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;
        private IWin32Window parentWindow;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ContextMenuBuilderProviderAndWindowNotNull_CreatesUserControlAndTreeViewControl()
        {
            // Call
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            var treeView = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");

            // Assert
            Assert.IsInstanceOf<UserControl>(view);
            Assert.IsInstanceOf<IView>(view);
            Assert.IsNull(view.Data);
            Assert.AreEqual(Resources.General_Map, view.Text);
            Assert.IsNotNull(treeView);
            Assert.IsInstanceOf<TreeViewControl>(treeView);
        }

        [Test]
        public void Constructor_ContextMenuBuilderProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLegendView(null, parentWindow);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot create a MapLegendView when the context menu builder provider is null");
        }

        [Test]
        public void Constructor_ParentWindowNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapLegendView(contextMenuBuilderProvider, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot create a MapLegendView when the parent window is null");
        }

        [Test]
        public void Data_MapDataCollection_DataSet()
        {
            // Setup
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);
            var mapData = new MapDataCollection(new List<MapData>(), "test data");

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
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);
            var mapData = new MapPointData(Enumerable.Empty<Point2D>(), "test data");

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
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);
            var mapData = new MapLineData(Enumerable.Empty<Point2D>(), "test data");

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
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);
            var mapData = new MapPolygonData(Enumerable.Empty<Point2D>(), "test data");

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
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);
            var mapData = new MapMultiLineData(Enumerable.Empty<IEnumerable<Point2D>>(), "test data");

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
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            // Call
            view.Data = null;

            // Assert
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            var view = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            // Call
            TestDelegate test = () => view.Data = new object();

            // Assert
            Assert.Throws<InvalidCastException>(test);
        }
    }
}