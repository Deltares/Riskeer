// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Map;
using Core.Gui.Plugin;
using Core.Gui.PresentationObjects.Map;
using Core.Gui.Properties;
using Core.Gui.TestUtil.ContextMenu;
using NSubstitute;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Gui.Test.Forms.Map
{
    [TestFixture]
    public class MapDataCollectionContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddMapLayerIndex = 0;
        private const int contextMenuZoomToAllIndex = 2;

        private MapLegendView mapLegendView;
        private TreeNodeInfo info;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNotNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanCheck);
            Assert.IsNotNull(info.CheckedState);
            Assert.IsNotNull(info.OnNodeChecked);
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
            TestHelper.AssertImagesAreEqual(Resources.folder, image);
        }

        [Test]
        public void ExpandOnCreate_Always_ReturnsTrue()
        {
            // Call
            bool expandOnCreate = info.ExpandOnCreate(null);

            // Assert
            Assert.IsTrue(expandOnCreate);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenWithContextAndDataReversed()
        {
            // Setup
            var mapPointData = new MapPointData("points");
            var mapLineData = new MapLineData("lines");
            var nestedCollection = new MapDataCollection("nested");
            var mapPolygonData = new MapPolygonData("polygons");
            var mapDataCollection = new MapDataCollection("test data");

            mapDataCollection.Add(mapPointData);
            mapDataCollection.Add(mapLineData);
            mapDataCollection.Add(nestedCollection);
            mapDataCollection.Add(mapPolygonData);

            MapDataCollectionContext parentCollectionContext = GetContext(mapDataCollection);

            // Call
            object[] objects = info.ChildNodeObjects(GetContext(mapDataCollection));

            // Assert
            CollectionAssert.AreEqual(new MapDataContext[]
            {
                new MapPolygonDataContext(mapPolygonData, parentCollectionContext),
                GetContext(nestedCollection, parentCollectionContext),
                new MapLineDataContext(mapLineData, parentCollectionContext),
                new MapPointDataContext(mapPointData, parentCollectionContext)
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
        public void CheckedState_WithContextAndMapDataCollectionVisibilityVisible_ReturnsStateChecked()
        {
            // Setup
            var featureBasedMapData = new TestFeatureBasedMapData();
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(featureBasedMapData);

            MapDataCollectionContext context = GetContext(mapDataCollection);

            // Call
            TreeNodeCheckedState checkedState = info.CheckedState(context);

            // Assert
            Assert.AreEqual(TreeNodeCheckedState.Checked, checkedState);
        }

        [Test]
        public void CheckedState_WithContextAndMapDataCollectionVisibilityNotVisible_ReturnsStateUnchecked()
        {
            // Setup
            var featureBasedMapData = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(featureBasedMapData);

            MapDataCollectionContext context = GetContext(mapDataCollection);

            // Call
            TreeNodeCheckedState checkedState = info.CheckedState(context);

            // Assert
            Assert.AreEqual(TreeNodeCheckedState.Unchecked, checkedState);
        }

        [Test]
        public void CheckedState_WithContextAndMapDataCollectionVisibilityMixed_ReturnsStateMixed()
        {
            // Setup
            var featureBasedMapData1 = new TestFeatureBasedMapData();
            var featureBasedMapData2 = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(featureBasedMapData1);
            mapDataCollection.Add(featureBasedMapData2);

            MapDataCollectionContext context = GetContext(mapDataCollection);

            // Call
            TreeNodeCheckedState checkedState = info.CheckedState(context);

            // Assert
            Assert.AreEqual(TreeNodeCheckedState.Mixed, checkedState);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_WithContext_SetMapDataVisibilityAndNotifyObservers(bool initialVisibleState)
        {
            // Setup
            var collectionObserver = Substitute.For<IObserver>();
            var featureBasedMapData = new TestFeatureBasedMapData();
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(featureBasedMapData);

            MapDataCollectionContext context = GetContext(mapDataCollection);
            context.WrappedData.IsVisible = initialVisibleState;

            context.WrappedData.Attach(collectionObserver);

            // Call
            info.OnNodeChecked(context, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, context.WrappedData.IsVisible);
            Assert.AreEqual(!initialVisibleState, featureBasedMapData.IsVisible);
            collectionObserver.Received().UpdateObserver();
        }

        [Test]
        public void OnNodeChecked_WithContextAndStateMixed_SetMapDataVisibilityAndNotifyObservers()
        {
            // Setup
            var observer1 = Substitute.For<IObserver>();
            var observer2 = Substitute.For<IObserver>();
            var featureBasedMapData1 = new TestFeatureBasedMapData();
            var featureBasedMapData2 = new TestFeatureBasedMapData
            {
                IsVisible = false
            };
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(featureBasedMapData1);
            mapDataCollection.Add(featureBasedMapData2);

            MapDataCollectionContext context = GetContext(mapDataCollection);

            featureBasedMapData1.Attach(observer1);
            featureBasedMapData2.Attach(observer2);

            // Call
            info.OnNodeChecked(context, null);

            // Assert
            Assert.IsTrue(context.WrappedData.IsVisible);
            Assert.IsTrue(featureBasedMapData1.IsVisible);
            Assert.IsTrue(featureBasedMapData2.IsVisible);
            observer2.Received().UpdateObserver();
        }

        [Test]
        [TestCase(true, 4)]
        [TestCase(false, 3)]
        public void OnNodeChecked_WithContext_NotifyObserversOfChangedChildrenOnly(bool initialVisibility, int expectedNotifications)
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var featureBasedMapData1 = new TestFeatureBasedMapData();
            var featureBasedMapData2 = new TestFeatureBasedMapData
            {
                IsVisible = initialVisibility
            };
            var featureBasedMapData3 = new TestFeatureBasedMapData
            {
                IsVisible = initialVisibility
            };
            var nestedMapDataCollection = new MapDataCollection("nested");
            nestedMapDataCollection.Add(featureBasedMapData1);
            nestedMapDataCollection.Add(featureBasedMapData3);

            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(nestedMapDataCollection);
            mapDataCollection.Add(featureBasedMapData2);

            MapDataCollectionContext context = GetContext(mapDataCollection);

            nestedMapDataCollection.Attach(observer);
            featureBasedMapData1.Attach(observer);
            featureBasedMapData2.Attach(observer);
            featureBasedMapData3.Attach(observer);

            // Call
            info.OnNodeChecked(context, null);

            // Assert
            observer.Received(expectedNotifications).UpdateObserver();
        }

        [Test]
        public void OnNodeChecked_WithContext_NotifyObserversOfParentMapDataCollections()
        {
            // Setup
            var collectionObserver = Substitute.For<IObserver>();
            var parentCollectionObserver = Substitute.For<IObserver>();
            var featureBasedMapData = new TestFeatureBasedMapData();
            var nestedMapDataCollection = new MapDataCollection("nested");
            nestedMapDataCollection.Add(featureBasedMapData);
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(nestedMapDataCollection);

            MapDataCollectionContext rootCollectionContext = GetContext(mapDataCollection);
            MapDataCollectionContext nestedCollectionContext = GetContext(nestedMapDataCollection, rootCollectionContext);

            nestedMapDataCollection.Attach(collectionObserver);
            mapDataCollection.Attach(parentCollectionObserver);

            // Call
            info.OnNodeChecked(nestedCollectionContext, null);

            // Assert
            collectionObserver.Received().UpdateObserver();
            parentCollectionObserver.Received().UpdateObserver();
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
            var observer = Substitute.For<IObserver>();
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

            observer.Received().UpdateObserver();
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(50)]
        public void OnDrop_MapDataCollectionContextMovedToPositionOutsideRange_ThrowsException(int position)
        {
            // Setup
            var observer = Substitute.For<IObserver>();
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

            var builder = Substitute.For<IContextMenuBuilder>();
            builder.AddImportItem(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Image>()).Returns(builder);
            builder.AddSeparator().Returns(builder);
            builder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(builder);
            builder.AddSeparator().Returns(builder);
            builder.AddPropertiesItem().Returns(builder);

            contextMenuBuilderProvider.Get(context, null).Returns(builder);
            // Call
            info.ContextMenuStrip(context, null, null);

            // Assert
            Received.InOrder(() =>
            {
                builder.AddImportItem(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Image>());
                builder.AddSeparator();
                builder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                builder.AddSeparator();
                builder.AddPropertiesItem();
                builder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_Always_ImportItemEnabled()
        {
            // Setup
            var mapDataCollection = new MapDataCollection("test data");
            MapDataCollectionContext context = GetContext(mapDataCollection);

            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            importCommandHandler.GetSupportedImportInfos(Arg.Any<object>()).Returns(new[]
            {
                new ImportInfo()
            });
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     mapDataCollection,
                                                     treeViewControl);

                contextMenuBuilderProvider.Get(context, treeViewControl).Returns(builder);
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
                contextMenuBuilderProvider.Get(Arg.Any<object>(), Arg.Any<ITreeViewControl>()).Returns(builder);
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
                contextMenuBuilderProvider.Get(Arg.Any<object>(), Arg.Any<ITreeViewControl>()).Returns(builder);
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
                contextMenuBuilderProvider.Get(Arg.Any<object>(), Arg.Any<ITreeViewControl>()).Returns(builder);
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
            contextMenuBuilderProvider.Get(Arg.Any<object>(), Arg.Any<ITreeViewControl>()).Returns(builder);
            var mapControl = Substitute.For<IMapControl>();
            mapControl.Data.Returns(mapData);
            mapControl.ZoomToVisibleLayers(mapData);
            mapLegendView.MapControl = mapControl;

            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
            {
                // Call
                contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                mapControl.Received().ZoomToVisibleLayers(mapData);
            }
        }

        [Test]
        public void ContextMenuStrip_NoMapControlAndEnabledZoomToAllContextMenuItemClicked_DoesNotThrow()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Get(Arg.Any<object>(), Arg.Any<ITreeViewControl>()).Returns(builder);
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

        public override void Setup()
        {
            contextMenuBuilderProvider = Substitute.For<IContextMenuBuilderProvider>();
            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapDataCollectionContext)];
        }

        public override void TearDown()
        {
            mapLegendView.Dispose();
        }

        private static MapDataCollectionContext GetContext(MapDataCollection mapDataCollection, MapDataCollectionContext parentMapDataCollectionContext = null)
        {
            return new MapDataCollectionContext(mapDataCollection, parentMapDataCollectionContext ?? new MapDataCollectionContext(new MapDataCollection("test"), null));
        }
    }
}