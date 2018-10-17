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
using Core.Components.Gis.TestUtil;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.PresentationObjects;
using Core.Plugins.Map.Properties;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class MapDataCollectionContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddMapLayerIndex = 0;
        private const int contextMenuZoomToAllIndex = 2;

        private MockRepository mocks;
        private MapLegendView mapLegendView;
        private TreeNodeInfo info;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        public override void Setup()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapDataCollectionContext)];
        }

        public override void TearDown()
        {
            mapLegendView.Dispose();

            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
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
            Assert.IsNotNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void Text_WithContext_ReturnsNameFromMapData()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("Collectie");

            // Call
            string text = info.Text(GetContext(mapDataCollection));

            // Assert
            Assert.AreEqual(mapDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsImageFromResource()
        {
            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenWithContextAndDataReversed()
        {
            // Setup
            var mapData1 = new TestFeatureBasedMapData();
            var mapData2 = new TestFeatureBasedMapData();
            var nestedCollection = new MapDataCollection("nested");
            var mapData3 = new TestFeatureBasedMapData();
            var mapDataCollection = new MapDataCollection("test data");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(nestedCollection);
            mapDataCollection.Add(mapData3);

            MapDataCollectionContext parentCollectionContext = GetContext(mapDataCollection);

            // Call
            object[] objects = info.ChildNodeObjects(GetContext(mapDataCollection));

            // Assert
            CollectionAssert.AreEqual(new MapDataContext[]
            {
                new FeatureBasedMapDataContext(mapData3, parentCollectionContext),
                GetContext(nestedCollection, parentCollectionContext), 
                new FeatureBasedMapDataContext(mapData2, parentCollectionContext),
                new FeatureBasedMapDataContext(mapData1, parentCollectionContext)
            }, objects);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Call
            bool canCheck = info.CanCheck(null);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [Test]
        public void CanDrag_ParentContextNotNull_ReturnsTrue()
        {
            // Setup
            MapDataCollectionContext context = GetContext(new MapDataCollection("test"));

            // Call
            bool canDrag = info.CanDrag(context, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanDrag_ParentContextNull_ReturnsFalse()
        {
            // Setup
            var context = new MapDataCollectionContext(new MapDataCollection("test"), null);

            // Call
            bool canDrag = info.CanDrag(context, null);

            // Assert
            Assert.IsFalse(canDrag);
        }

        [Test]
        public void CanDrop_TargetIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test 1");
            MapDataCollectionContext context1 = GetContext(mapDataCollection);
            MapDataCollectionContext context2 = GetContext(new MapDataCollection("test 2"), context1);

            // Call
            bool canDrop = info.CanDrop(context2, context1);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanDrop_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            MapDataCollectionContext context = GetContext(new MapDataCollection("test"));

            // Call
            bool canDrop = info.CanDrop(context, GetContext(new MapDataCollection("parent")));

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanInsert_TargetParentIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test 1");
            MapDataCollectionContext context1 = GetContext(mapDataCollection);
            MapDataCollectionContext context2 = GetContext(new MapDataCollection("test 2"), context1);

            // Call
            bool canInsert = info.CanInsert(context2, context1);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        public void CanInsert_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            MapDataCollectionContext context = GetContext(new MapDataCollection("test"));

            // Call
            bool canInsert = info.CanInsert(context, GetContext(new MapDataCollection("parent")));

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDrop_MapDataCollectionContextMovedToPositionInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var mapDataCollection1 = new MapDataCollection("Collection 1");
            var mapDataCollection2 = new MapDataCollection("Collection 2");
            var mapDataCollection3 = new MapDataCollection("Collection 3");
            var parentMapDataCollection = new MapDataCollection("test data");

            parentMapDataCollection.Add(mapDataCollection1);
            parentMapDataCollection.Add(mapDataCollection2);
            parentMapDataCollection.Add(mapDataCollection3);

            MapDataCollectionContext parentContext = GetContext(parentMapDataCollection);
            MapDataCollectionContext context = GetContext(mapDataCollection1);
            
            parentMapDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                info.OnDrop(context, parentContext, parentContext, position, treeViewControl);

                // Assert
                int reversedIndex = 2 - position;
                Assert.AreSame(context.WrappedData, parentMapDataCollection.Collection.ElementAt(reversedIndex));
            }
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(50)]
        public void OnDrop_MapDataCollectionContextMovedToPositionOutsideRange_ThrowsException(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var mapDataCollection1 = new MapDataCollection("Collection 1");
            var mapDataCollection2 = new MapDataCollection("Collection 2");
            var mapDataCollection3 = new MapDataCollection("Collection 3");
            var parentMapDataCollection = new MapDataCollection("test data");

            parentMapDataCollection.Add(mapDataCollection1);
            parentMapDataCollection.Add(mapDataCollection2);
            parentMapDataCollection.Add(mapDataCollection3);

            parentMapDataCollection.Attach(observer);

            MapDataCollectionContext parentContext = GetContext(parentMapDataCollection);
            MapDataCollectionContext context = GetContext(mapDataCollection1);

            parentMapDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => info.OnDrop(context, parentContext, parentContext, position, treeViewControl);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(test);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mapData = new MapDataCollection("test data");
            MapDataCollectionContext context = GetContext(mapData);

            var builder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                builder.Expect(mb => mb.AddCustomImportItem(null, null, null)).IgnoreArguments().Return(builder);
                builder.Expect(mb => mb.AddSeparator()).Return(builder);
                builder.Expect(mb => mb.AddCustomItem(Arg<StrictContextMenuItem>.Is.NotNull)).Return(builder);
                builder.Expect(mb => mb.AddSeparator()).Return(builder);
                builder.Expect(mb => mb.AddPropertiesItem()).Return(builder);
                builder.Expect(mb => mb.Build()).Return(null);
            }

            contextMenuBuilderProvider.Expect(p => p.Get(context, null)).Return(builder);

            mocks.ReplayAll();

            // Call
            info.ContextMenuStrip(context, null, null);

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_Always_CustomImportItemEnabled()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test data");
            MapDataCollectionContext context = GetContext(mapDataCollection);

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

                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(context, treeViewControl)).Return(builder);
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuAddMapLayerIndex,
                                                                  "&Voeg kaartlaag toe...",
                                                                  "Importeer een nieuwe kaartlaag en voeg deze toe.",
                                                                  Resources.MapPlusIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_VisibleFeatureBasedMapDataWithFeaturesInMapDataCollection_ZoomToAllItemEnabled()
        {
            // Setup
            var featureBasedMapData = new TestFeatureBasedMapData
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(featureBasedMapData);

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(null, null)).IgnoreArguments().Return(builder);
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapDataCollection), null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                                  "&Zoom naar alles",
                                                                  "Zet het zoomniveau van de kaart dusdanig dat alle zichtbare kaartlagen in deze map met kaartlagen precies in het beeld passen.",
                                                                  Resources.ZoomToAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_InvisibleFeatureBasedMapDataInMapDataCollection_ZoomToAllItemDisabled()
        {
            // Setup
            var featureBasedMapData = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(featureBasedMapData);

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(null, null)).IgnoreArguments().Return(builder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapDataCollection), null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                                  "&Zoom naar alles",
                                                                  "Om het zoomniveau aan te passen moet er minstens één kaartlaag in deze map met kaartlagen zichtbaar zijn.",
                                                                  Resources.ZoomToAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_VisibleFeatureBasedMapDataWithoutFeaturesInMapDataCollection_ZoomToAllItemDisabled()
        {
            // Setup
            var featureBasedMapData = new TestFeatureBasedMapData
            {
                IsVisible = true
            };
            var mapDataCollection = new MapDataCollection("test data");
            mapDataCollection.Add(featureBasedMapData);
            
            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new CustomItemsOnlyContextMenuBuilder();
                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(null, null)).IgnoreArguments().Return(builder);
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapDataCollection), null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                                  "&Zoom naar alles",
                                                                  "Om het zoomniveau aan te passen moet minstens één van de zichtbare kaartlagen in deze map met kaartlagen elementen bevatten.",
                                                                  Resources.ZoomToAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_EnabledZoomToAllContextMenuItemClicked_DoZoomToVisibleData()
        {
            // Setup
            var mapData = new MapDataCollection("A");
            var featureBasedMapData = new TestFeatureBasedMapData
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };
            mapData.Add(featureBasedMapData);

            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Stub(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            var mapControl = mocks.StrictMock<IMapControl>();
            mapControl.Expect(c => c.Data).Return(mapData);
            mapControl.Expect(c => c.ZoomToAllVisibleLayers(mapData));
            mocks.ReplayAll();

            mapLegendView.MapControl = mapControl;

            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
            {
                // Call
                contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                // Assert expectancies are called in TearDown()
            }
        }

        [Test]
        public void ContextMenuStrip_NoMapControlAndEnabledZoomToAllContextMenuItemClicked_DoesNotThrow()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Stub(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            var mapData = new MapDataCollection("A")
            {
                IsVisible = true
            };

            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
            {
                // Call
                TestDelegate call = () => contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        private static MapDataCollectionContext GetContext(MapDataCollection mapDataCollection, MapDataCollectionContext parentMapDataCollectionContext = null)
        {
            return new MapDataCollectionContext(mapDataCollection, parentMapDataCollectionContext ?? new MapDataCollectionContext(new MapDataCollection("test"), null));
        }
    }
}