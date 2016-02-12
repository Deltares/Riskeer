using System;
using System.Collections.Generic;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.DotSpatial.Data;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;
using DotSpatialResources = Core.Plugins.DotSpatial.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapDataCollectionTreeNodeInfoTest
    {
        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mapLegendView = new MapLegendView();

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapDataCollection)];
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(MapDataCollection), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
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
            Assert.AreEqual(DotSpatialResources.General_Map, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnDataReversed()
        {
            // Setup
            var mapData1 = mocks.StrictMock<MapData>();
            var mapData2 = mocks.StrictMock<MapData>();
            var mapData3 = mocks.StrictMock<MapData>();
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>
            {
                mapData1,
                mapData2,
                mapData3
            });

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(mapDataCollection);

            // Assert
            CollectionAssert.AreEqual(new[] { mapData3, mapData2, mapData1 }, objects);

            mocks.VerifyAll();
        }
    }
}
