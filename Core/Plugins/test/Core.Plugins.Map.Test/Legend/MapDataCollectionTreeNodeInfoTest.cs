// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
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

        public override void Setup()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapDataCollection)];
        }

        public override void TearDown()
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
            string text = info.Text(mapDataCollection);

            // Assert
            Assert.AreEqual(mapDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

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
            object[] objects = info.ChildNodeObjects(mapDataCollection);

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
            bool canDrop = info.CanDrop(new object(), mapDataCollection);

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
            bool canDrop = info.CanDrop(mapData, mapDataCollection);

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
            bool canInsert = info.CanInsert(new object(), mapDataCollection);

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
            bool canInsert = info.CanInsert(mapData, mapDataCollection);

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
                int reversedIndex = 2 - position;
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

            var applicationFeatureCommands = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            importCommandHandler.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
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
                                                                  "Zet het zoomniveau van de kaart dusdanig dat alle zichtbare kaartlagen in deze map met kaartlagen precies in het beeld passen.",
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

            var applicationFeatureCommands = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            importCommandHandler.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
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
                                                                  "Om het zoomniveau aan te passen moet er minstens één kaartlaag in deze map met kaartlagen zichtbaar zijn.",
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
        public void ContextMenuStrip_VisibleFeatureBasedMapDataWithoutFeaturesInMapDataCollection_CallsContextMenuBuilderMethods()
        {
            // Setup
            var lineData = new MapLineData("test line")
            {
                IsVisible = false,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };
            var pointData = new MapPointData("test data")
            {
                IsVisible = true
            };
            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(pointData);
            mapDataCollection.Add(lineData);

            var applicationFeatureCommands = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            importCommandHandler.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
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
                                                                  "Om het zoomniveau aan te passen moet minstens één van de zichtbare kaartlagen in deze map met kaartlagen elementen bevatten.",
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
        public void ContextMenuStrip_EnabledZoomToAllContextMenuItemClicked_DoZoomToVisibleData()
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
            var mapControl = mocks.StrictMock<IMapControl>();
            mapControl.Expect(c => c.Data).Return(new MapDataCollection("name"));
            mapControl.Expect(c => c.ZoomToAllVisibleLayers(mapData));
            mocks.ReplayAll();

            mapLegendView.MapControl = mapControl;

            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Call
                contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                // Assert expectancies are called in TearDown()
            }
        }

        [Test]
        public void ContextMenuStrip_NoChartControlAndEnabledZoomToAllContextMenuItemClicked_DoesNotThrow()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Stub(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            var mapData = new MapDataCollection("A")
            {
                IsVisible = true
            };

            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Call
                TestDelegate call = () => contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }
    }
}