﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
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
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.Stub<IWin32Window>();
            mapLegendView = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapDataCollection)];
        }

        [TearDown]
        public void TearDown()
        {
            mapLegendView.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(MapDataCollection), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
        }

        [Test]
        public void Text_Always_ReturnsNameFromMapData()
        {
            // Setup
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "Collectie");

            mocks.ReplayAll();

            // Call
            var text = info.Text(mapDataCollection);

            // Assert
            Assert.AreEqual(mapDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnDataReversed()
        {
            // Setup
            var mapData1 = mocks.StrictMock<MapData>("test data");
            var mapData2 = mocks.StrictMock<MapData>("test data");
            var mapData3 = mocks.StrictMock<MapData>("test data");
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>
            {
                mapData1,
                mapData2,
                mapData3
            }, "test data");

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(mapDataCollection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                mapData3,
                mapData2,
                mapData1
            }, objects);
        }

        [Test]
        public void CanDrop_SourceNodeTagIsNoMapData_ReturnsFalse()
        {
            // Setup
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrop = info.CanDrop(new object(), mapDataCollection);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_SourceNodeTagIsMapData_ReturnsTrue()
        {
            // Setup
            var mapData = mocks.StrictMock<MapData>("test data");
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrop = info.CanDrop(mapData, mapDataCollection);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanInsert_SourceNodeTagIsNoMapData_ReturnsFalse()
        {
            // Setup
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(new object(), mapDataCollection);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_SourceNodeTagIsMapData_ReturnsTrue()
        {
            // Setup
            var mapData = mocks.StrictMock<MapData>("test data");
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(mapData, mapDataCollection);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDrop_MapDataMovedToPositionInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var mapData1 = mocks.StrictMock<MapData>("test data");
            var mapData2 = mocks.StrictMock<MapData>("test data");
            var mapData3 = mocks.StrictMock<MapData>("test data");
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>
            {
                mapData1,
                mapData2,
                mapData3
            }, "test data");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                mapDataCollection.Attach(observer);

                // Call
                info.OnDrop(mapData1, mapDataCollection, mapDataCollection, position, treeViewControl);

                // Assert
                var reversedIndex = 2 - position;
                Assert.AreSame(mapData1, mapDataCollection.List.ElementAt(reversedIndex));
            }
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(50)]
        public void OnDrop_MapDataMovedToPositionOutsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var mapData1 = mocks.StrictMock<MapData>("test data");
            var mapData2 = mocks.StrictMock<MapData>("test data");
            var mapData3 = mocks.StrictMock<MapData>("test data");
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>
            {
                mapData1,
                mapData2,
                mapData3
            }, "test data");

            mapDataCollection.Attach(observer);
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => info.OnDrop(mapData1, mapDataCollection, mapDataCollection, position, treeViewControl);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(test);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "test data");

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(mapDataCollection, null, treeViewControl);
            }

            // Assert
            // Expectancies will be asserted in TearDown()
        }

        [Test]
        public void ContextMenuStrip_Always_ContainsAddMapLayerMenuItem()
        {
            // Setup
            const string expectedItemText = "&Voeg kaartlaag toe...";
            const string expectedItemTooltip = "Importeer een nieuwe kaartlaag en voeg deze toe.";
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(Enumerable.Empty<MapData>(), "test data");

            using (var treeViewControl = new TreeViewControl())
            {
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                // Call
                var contextMenu = info.ContextMenuStrip(mapDataCollection, null, treeViewControl);

                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, DotSpatialResources.MapIcon);
            }
        }
    }
}