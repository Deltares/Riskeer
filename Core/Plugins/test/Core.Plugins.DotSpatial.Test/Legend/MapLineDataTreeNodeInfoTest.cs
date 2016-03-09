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
    public class MapLineDataTreeNodeInfoTest
    {
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(MapLineData), info.TagType);
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
            // Setup
            var mocks = CreateDefaultMocks();
            var mapLineData = mocks.StrictMock<MapLineData>(Enumerable.Empty<MapFeature>(), "MapLineData");
            mocks.ReplayAll();

            // Call
            var text = info.Text(mapLineData);

            // Assert
            Assert.AreEqual(mapLineData.Name, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(DotSpatialResources.LineIcon, image);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var mocks = CreateDefaultMocks();
            var lineData = mocks.StrictMock<MapLineData>(Enumerable.Empty<MapFeature>(), "test data");

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
            var mocks = CreateDefaultMocks();
            var lineData = mocks.StrictMock<MapLineData>(Enumerable.Empty<MapFeature>(), "test data");

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
            var mocks = CreateDefaultMocks();
            var lineData = mocks.StrictMock<MapLineData>(Enumerable.Empty<MapFeature>(), "test data");

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
            var mocks = CreateDefaultMocks();
            var observable = mocks.StrictMock<IObservable>();
            var lineData = mocks.StrictMock<MapLineData>(Enumerable.Empty<MapFeature>(), "test data");

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
            var mocks = CreateDefaultMocks();
            var lineData = mocks.StrictMock<MapLineData>(Enumerable.Empty<MapFeature>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrag = info.CanDrag(lineData, null);

            // Assert
            Assert.IsTrue(canDrag);

            mocks.VerifyAll();
        }

        private MockRepository CreateDefaultMocks()
        {
            var mockRepository = new MockRepository();
            var contextMenuBuilderProvider = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mockRepository.StrictMock<IWin32Window>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapLineData)];

            return mockRepository;
        }
    }
}