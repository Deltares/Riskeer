// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class MapLineDataTreeNodeInfoTest
    {
        private const int contextMenuZoomToAllIndex = 0;

        private TreeNodeInfo info;
        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapLineData)];
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
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanCheck);
            Assert.IsNotNull(info.IsChecked);
            Assert.IsNotNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsNameFromMapData()
        {
            // Setup
            mocks.ReplayAll();
            var mapLineData = new MapLineData("test data");

            // Call
            string text = info.Text(mapLineData);

            // Assert
            Assert.AreEqual(mapLineData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.LineIcon, image);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var builder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                builder.Expect(mb => mb.AddCustomItem(Arg<StrictContextMenuItem>.Is.NotNull)).Return(builder);
                builder.Expect(mb => mb.AddSeparator()).Return(builder);
                builder.Expect(mb => mb.AddDeleteItem()).Return(builder);
                builder.Expect(mb => mb.AddSeparator()).Return(builder);
                builder.Expect(mb => mb.AddPropertiesItem()).Return(builder);
                builder.Expect(mb => mb.Build()).Return(null);
            }

            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            var mapData = new MapLineData("A");

            // Call
            info.ContextMenuStrip(mapData, null, null);

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_VisibleMapData_ZoomToAllItemEnabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            var mapData = new MapLineData("A")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Zet het zoomniveau van de kaart dusdanig dat deze kaartlaag precies in het beeld past.",
                                                              Resources.ZoomToAllIcon);
            }
        }

        [Test]
        public void ContextMenuStrip_InvisibleMapData_ZoomToAllItemDisabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            var mapData = new MapLineData("A")
            {
                IsVisible = false
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Om het zoomniveau aan te passen moet de kaartlaag zichtbaar zijn.",
                                                              Resources.ZoomToAllIcon,
                                                              false);
            }
        }

        [Test]
        public void ContextMenuStrip_MapDataWithoutFeatures_ZoomToAllItemDisabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            var mapData = new MapLineData("A")
            {
                IsVisible = true
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(mapData, null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Om het zoomniveau aan te passen moet de kaartlaag elementen bevatten.",
                                                              Resources.ZoomToAllIcon,
                                                              false);
            }
        }

        [Test]
        public void ContextMenuStrip_EnabledZoomToAllContextMenuItemClicked_DoZoomToVisibleData()
        {
            // Setup
            var mapData = new MapLineData("A")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

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

            var mapData = new MapLineData("A")
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

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();
            var mapLineData = new MapLineData("test data");

            // Call
            bool canCheck = info.CanCheck(mapLineData);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();
            var mapLineData = new MapLineData("test data");

            // Call
            bool canDrag = info.CanDrag(mapLineData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanRemove_WithRemovableDataAndCollection_ReturnTrue()
        {
            // Setup
            var removable = mocks.StrictMultiMock<MapLineData>(new[]
            {
                typeof(IRemovable)
            }, "name");
            mocks.ReplayAll();

            // Call
            bool canRemove = info.CanRemove(removable, new MapDataCollection("collection"));

            // Assert
            Assert.IsTrue(canRemove);
        }

        [Test]
        public void CanRemove_WithoutCollection_ReturnFalse()
        {
            // Setup
            var removable = mocks.StrictMultiMock<MapLineData>(new[]
            {
                typeof(IRemovable)
            }, "name");
            mocks.ReplayAll();

            // Call
            bool canRemove = info.CanRemove(removable, null);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void CanRemove_WithNotRemovableData_ReturnFalse()
        {
            // Setup
            var notRemovable = mocks.StrictMock<MapLineData>("name");
            mocks.ReplayAll();

            // Call
            bool canRemove = info.CanRemove(notRemovable, new MapDataCollection("collection"));

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void OnNodeRemoved_WithRemovableDataToRemove_DataRemoved()
        {
            // Setup
            var toRemove = mocks.StrictMultiMock<MapLineData>(new[]
            {
                typeof(IRemovable)
            }, "name");
            var otherData = mocks.Stub<MapLineData>("name");
            mocks.ReplayAll();

            var collection = new MapDataCollection("collection");
            collection.Add(toRemove);
            collection.Add(otherData);

            // Call
            info.OnNodeRemoved(toRemove, collection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                otherData
            }, collection.Collection);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfLineData(bool isVisible)
        {
            // Setup
            mocks.ReplayAll();
            var mapLineData = new MapLineData("test data")
            {
                IsVisible = isVisible
            };

            // Call
            bool canCheck = info.IsChecked(mapLineData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_MapLineDataNode_SetsLineDataVisibilityAndNotifiesObservers(bool initialVisibleState)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var mapLineData = new MapLineData("test data")
            {
                IsVisible = initialVisibleState
            };

            mapLineData.Attach(observer);

            // Call
            info.OnNodeChecked(mapLineData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, mapLineData.IsVisible);
        }
    }
}