using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;
using DotSpatialResources = Core.Plugins.DotSpatial.Properties.Resources;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapPolygonDataTreeNodeInfoTest
    {
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mapLegendView = new MapLegendView();

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapPolygonData)];
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(MapPolygonData), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Call
            var text = info.Text(null);

            // Assert
            Assert.AreEqual(DotSpatialResources.MapData_Polygon_data_label, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(DotSpatialResources.AreaIcon, image);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var lineData = mocks.StrictMock<MapPolygonData>(Enumerable.Empty<Point2D>());

            mocks.ReplayAll();

            // Call
            var canCheck = info.CanCheck(lineData);

            // Assert
            Assert.IsTrue(canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfLineData(bool isVisible)
        {
            // Setup
            var mocks = new MockRepository();
            var lineData = mocks.StrictMock<MapPolygonData>(Enumerable.Empty<Point2D>());

            lineData.IsVisible = isVisible;

            mocks.ReplayAll();

            // Call
            var canCheck = info.IsChecked(lineData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_LineDataNodeWithoutParent_SetsLineDataVisibility(bool initialVisibleState)
        {
            // Setup
            var mocks = new MockRepository();
            var lineData = mocks.StrictMock<MapPolygonData>(Enumerable.Empty<Point2D>());

            mocks.ReplayAll();

            lineData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(lineData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, lineData.IsVisible);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_LineDataNodeWithObservableParent_SetsLineDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var mocks = new MockRepository();
            var observable = mocks.StrictMock<IObservable>();
            var lineData = mocks.StrictMock<MapPolygonData>(Enumerable.Empty<Point2D>());

            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            lineData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(lineData, observable);

            // Assert
            Assert.AreEqual(!initialVisibleState, lineData.IsVisible);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var polygonData = mocks.StrictMock<MapPolygonData>(Enumerable.Empty<Point2D>());

            mocks.ReplayAll();

            // Call
            var canDrag = info.CanDrag(polygonData, null);

            // Assert
            Assert.IsTrue(canDrag);

            mocks.VerifyAll();
        }
    }
}