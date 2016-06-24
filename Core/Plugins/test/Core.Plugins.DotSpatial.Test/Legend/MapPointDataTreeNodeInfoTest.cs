using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;
using DotSpatialResources = Core.Plugins.DotSpatial.Properties.Resources;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapPointDataTreeNodeInfoTest
    {
        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapPointData)];
        }

        [TearDown]
        public void TearDown()
        {
            mapLegendView.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(MapPointData), info.TagType);
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
        public void Text_Always_ReturnsNameFromMapData()
        {
            // Setup
            var mapPointData = mocks.StrictMock<MapPointData>(Enumerable.Empty<MapFeature>(), "MapPointData");
            mocks.ReplayAll();

            // Call
            var text = info.Text(mapPointData);

            // Assert
            Assert.AreEqual(mapPointData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(DotSpatialResources.PointsIcon, image);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var lineData = mocks.StrictMock<MapPointData>(Enumerable.Empty<MapFeature>(), "test data");

            mocks.ReplayAll();

            // Call
            var canCheck = info.CanCheck(lineData);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfLineData(bool isVisible)
        {
            // Setup
            var lineData = mocks.StrictMock<MapPointData>(Enumerable.Empty<MapFeature>(), "test data");
            lineData.IsVisible = isVisible;

            mocks.ReplayAll();

            // Call
            var canCheck = info.IsChecked(lineData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_LineDataNodeWithoutParent_SetsLineDataVisibility(bool initialVisibleState)
        {
            // Setup
            var lineData = mocks.StrictMock<MapPointData>(Enumerable.Empty<MapFeature>(), "test data");

            mocks.ReplayAll();

            lineData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(lineData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, lineData.IsVisible);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_LineDataNodeWithObservableParent_SetsLineDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var lineData = mocks.StrictMock<MapPointData>(Enumerable.Empty<MapFeature>(), "test data");

            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            lineData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(lineData, observable);

            // Assert
            Assert.AreEqual(!initialVisibleState, lineData.IsVisible);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            var pointData = mocks.StrictMock<MapPointData>(Enumerable.Empty<MapFeature>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrag = info.CanDrag(pointData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }
    }
}