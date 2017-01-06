// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
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
        private const int contextMenuAddMapLayerIndex = 0;
        private const int contextMenuZoomToAllIndex = 2;
        private const int contextMenuPropertiesIndex = 4;

        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

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
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
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
        public void ContextMenuStrip_MapDataCollectionWithVisibleFeatureBasedmapData_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mapPointData = new MapPointData("test")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };
            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(mapPointData);

            var applicationFeatureCommandsStub = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.Stub<IImportCommandHandler>();
            importCommandHandlerMock.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandlerMock = mocks.Stub<IExportCommandHandler>();
            var viewCommandsMock = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                var builder = new ContextMenuBuilder(applicationFeatureCommandsStub,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     mapDataCollection,
                                                     treeViewControl);

                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(5, contextMenu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuAddMapLayerIndex,
                                                                  "&Voeg kaartlaag toe...",
                                                                  "Importeer een nieuwe kaartlaag en voeg deze toe.",
                                                                  Resources.MapPlusIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                                  "&Zoom naar alles",
                                                                  "Zet het zoomniveau van de kaart dusdanig dat alle zichtbare elementen van deze kaartlagenmap precies in het beeld passen.",
                                                                  Resources.ZoomToAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuPropertiesIndex,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  GuiResources.PropertiesHS,
                                                                  false);

                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                                                                {
                                                                    contextMenu.Items[1],
                                                                    contextMenu.Items[3]
                                                                }, typeof(ToolStripSeparator));
                }
            }
        }

        [Test]
        public void ContextMenuStrip_InvisibleFeatureBasedMapDataInMapDataCollection_CallsContextMenuBuilderMethods()
        {
            // Setup
            var pointData = new MapPointData("test data")
            {
                IsVisible = false
            };
            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(pointData);

            var applicationFeatureCommandsStub = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.Stub<IImportCommandHandler>();
            importCommandHandlerMock.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandlerMock = mocks.Stub<IExportCommandHandler>();
            var viewCommandsMock = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                var builder = new ContextMenuBuilder(applicationFeatureCommandsStub,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     mapDataCollection,
                                                     treeViewControl);

                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(5, contextMenu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuAddMapLayerIndex,
                                                                  "&Voeg kaartlaag toe...",
                                                                  "Importeer een nieuwe kaartlaag en voeg deze toe.",
                                                                  Resources.MapPlusIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                                  "&Zoom naar alles",
                                                                  "Om het zoomniveau aan te passen moet er minstens één kaartlaag in deze kaartlagenmap zichtbaar zijn.",
                                                                  Resources.ZoomToAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuPropertiesIndex,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  GuiResources.PropertiesHS,
                                                                  false);

                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                                                                {
                                                                    contextMenu.Items[1],
                                                                    contextMenu.Items[3]
                                                                }, typeof(ToolStripSeparator));
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FeatureBasedMapDataWithoutFeaturesInMapDataCollection_CallsContextMenuBuilderMethods()
        {
            // Setup
            var pointData = new MapPointData("test data")
            {
                IsVisible = true
            };
            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(pointData);

            var applicationFeatureCommandsStub = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.Stub<IImportCommandHandler>();
            importCommandHandlerMock.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandlerMock = mocks.Stub<IExportCommandHandler>();
            var viewCommandsMock = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                var builder = new ContextMenuBuilder(applicationFeatureCommandsStub,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     mapDataCollection,
                                                     treeViewControl);

                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(mapDataCollection, treeViewControl)).Return(builder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapDataCollection, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(5, contextMenu.Items.Count);
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuAddMapLayerIndex,
                                                                  "&Voeg kaartlaag toe...",
                                                                  "Importeer een nieuwe kaartlaag en voeg deze toe.",
                                                                  Resources.MapPlusIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                                  "&Zoom naar alles",
                                                                  "Om het zoomniveau aan te passen moeten alle zichtbare kaartlagen in deze kaartlagenmap elementen bevatten.",
                                                                  Resources.ZoomToAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuPropertiesIndex,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  GuiResources.PropertiesHS,
                                                                  false);

                    CollectionAssert.AllItemsAreInstancesOfType(new[]
                                                                {
                                                                    contextMenu.Items[1],
                                                                    contextMenu.Items[3]
                                                                }, typeof(ToolStripSeparator));
                }
            }
        }

        [Test]
        public void ContextMenuStrip_MapLegendViewHasMapControlAndClickZoomToAll_DoZoomToAllVisibleLayersForMapData()
        {
            // Setup
            var mapData = new MapDataCollection("A");
            var pointData = new MapPointData("B")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };
            mapData.Add(pointData);

            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Stub(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            var mapControl = mocks.Stub<IMapControl>();
            mapControl.Expect(c => c.ZoomToAllVisibleLayers(mapData));
            mocks.ReplayAll();

            mapLegendView.MapControl = mapControl;

            using (var contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Call
                contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                // Assert expectancies are called in TearDown()
            }
        }

        [Test]
        public void ContextMenuStrip_MapLegendViewWithoutMapControlAndClickZoomToAll_DoNothing()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Stub(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            mapLegendView.MapControl = null;

            var mapData = new MapDataCollection("A")
            {
                IsVisible = true
            };

            using (var contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Call
                TestDelegate call = () => contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        public override void TearDown()
        {
            mapLegendView.Dispose();

            mocks.VerifyAll();
        }
    }
}