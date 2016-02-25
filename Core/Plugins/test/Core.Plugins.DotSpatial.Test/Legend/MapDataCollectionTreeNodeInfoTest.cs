﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
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
            CollectionAssert.AreEqual(new[]
            {
                mapData3,
                mapData2,
                mapData1
            }, objects);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_SourceNodeTagIsNoMapData_ReturnsFalse()
        {
            // Setup
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>());

            mocks.ReplayAll();

            // Call
            var canDrop = info.CanDrop(new object(), mapDataCollection);

            // Assert
            Assert.IsFalse(canDrop);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_SourceNodeTagIsMapData_ReturnsTrue()
        {
            // Setup
            var mapData = mocks.StrictMock<MapData>();
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>());

            mocks.ReplayAll();

            // Call
            var canDrop = info.CanDrop(mapData, mapDataCollection);

            // Assert
            Assert.IsTrue(canDrop);

            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_SourceNodeTagIsNoMapData_ReturnsFalse()
        {
            // Setup
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>());

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(new object(), mapDataCollection);

            // Assert
            Assert.IsFalse(canInsert);

            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_SourceNodeTagIsMapData_ReturnsTrue()
        {
            // Setup
            var mapData = mocks.StrictMock<MapData>();
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>());

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(mapData, mapDataCollection);

            // Assert
            Assert.IsTrue(canInsert);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDrop_MapDataMovedToPositionInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var mapData1 = mocks.StrictMock<MapData>();
            var mapData2 = mocks.StrictMock<MapData>();
            var mapData3 = mocks.StrictMock<MapData>();
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>
            {
                mapData1,
                mapData2,
                mapData3
            });

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            mapDataCollection.Attach(observer);

            // Call
            info.OnDrop(mapData1, mapDataCollection, mapDataCollection, position, treeViewControlMock);

            // Assert
            var reversedIndex = 2 - position;
            Assert.AreSame(mapData1, mapDataCollection.List.ElementAt(reversedIndex));

            mocks.VerifyAll(); // UpdateObserver should be called
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
            var mapData1 = mocks.StrictMock<MapData>();
            var mapData2 = mocks.StrictMock<MapData>();
            var mapData3 = mocks.StrictMock<MapData>();
            var mapDataCollection = mocks.StrictMock<MapDataCollection>(new List<MapData>
            {
                mapData1,
                mapData2,
                mapData3
            });

            mapDataCollection.Attach(observer);

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => info.OnDrop(mapData1, mapDataCollection, mapDataCollection, position, treeViewControlMock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);

            mocks.VerifyAll(); // UpdateObserver should be not called
        }
    }
}