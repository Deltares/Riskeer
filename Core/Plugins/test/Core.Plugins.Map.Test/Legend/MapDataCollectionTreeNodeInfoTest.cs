﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
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
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.Properties;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class MapDataCollectionTreeNodeInfoTest : NUnitFormTest
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
            mocks.ReplayAll();
            var mapDataCollection = new MapDataCollection("Collectie");

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
        public void ChildNodeObjects_Always_ReturnsChildrenOfDataReversed()
        {
            // Setup
            var mapData1 = mocks.StrictMock<MapData>("test data");
            var mapData2 = mocks.StrictMock<MapData>("test data");
            var mapData3 = mocks.StrictMock<MapData>("test data");
            var mapDataCollection = new MapDataCollection("test data");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(mapData3);

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
            mocks.ReplayAll();
            var mapDataCollection = new MapDataCollection("test data");

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
            var mapDataCollection = new MapDataCollection("test data");

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
            mocks.ReplayAll();
            var mapDataCollection = new MapDataCollection("test data");

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
            var mapDataCollection = new MapDataCollection("test data");

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
            var mapDataCollection = new MapDataCollection("test data");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(mapData3);

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
                Assert.AreSame(mapData1, mapDataCollection.Collection.ElementAt(reversedIndex));
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
            var mapDataCollection = new MapDataCollection("test data");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(mapData3);

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
            var mapDataCollection = new MapDataCollection("test data");

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
            var mapDataCollection = new MapDataCollection("test data");

            using (var treeViewControl = new TreeViewControl())
            {
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, Resources.MapPlusIcon);
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void GivenFilePathIsSet_WhenShapeFileIsCorrupt_ThenNoMapDataViewIsAdded()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "CorruptFile.shp");

            DialogBoxHandler = (name, wnd) =>
            {
                OpenFileDialogTester tester = new OpenFileDialogTester(wnd);
                tester.OpenFile(testFilePath);
            };

            // When
            var mapDataCollection = new MapDataCollection("test data");
            using (var treeViewControl = new TreeViewControl())
            {
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[0].PerformClick();

                    // Then
                    var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.",
                                                        testFilePath);
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);
                }
            }

            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GivenFilePathIsSet_WhenShapeFileIsEmpty_ThenNoMapDataViewIsAdded()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "EmptyFile.shp");

            DialogBoxHandler = (name, wnd) =>
            {
                OpenFileDialogTester tester = new OpenFileDialogTester(wnd);
                tester.OpenFile(testFilePath);
            };

            // When
            var mapDataCollection = new MapDataCollection("test data");
            using (var treeViewControl = new TreeViewControl())
            {
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[0].PerformClick();

                    // Then
                    var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': kon geen geometrieën vinden in dit bestand.",
                                                        testFilePath);
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);
                }
            }
        }

        [Test]
        [RequiresSTA]
        public void GivenFilePathIsSet_WhenShapeFileIsValid_ThenMapDataViewIsAdded()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, "Single_Point_with_ID.shp");

            DialogBoxHandler = (name, wnd) =>
            {
                OpenFileDialogTester tester = new OpenFileDialogTester(wnd);
                tester.OpenFile(testFilePath);
            };

            // When
            var mapDataCollection = new MapDataCollection("test data");
            using (var treeViewControl = new TreeViewControl())
            {
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[0].PerformClick();

                    // Then
                    string expectedMessage = "Het shapebestand is geïmporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(action, expectedMessage);
                }
            }
        }

        public override void TearDown()
        {
            mapLegendView.Dispose();

            mocks.VerifyAll();
        }
    }
}