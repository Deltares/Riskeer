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
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.TestUtil;
using Core.Plugins.Map.Legend;
using Core.Plugins.Map.PresentationObjects;
using Core.Plugins.Map.Properties;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class MapDataContextTreeNodeInfoTest
    {
        private const int contextMenuZoomToAllIndex = 0;

        private MapLegendView mapLegendView;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;

        private static IEnumerable<TestCaseData> MapDataLegendImages
        {
            get
            {
                yield return new TestCaseData(new MapPointData("test"), Resources.PointsIcon);
                yield return new TestCaseData(new MapLineData("test"), Resources.LineIcon);
                yield return new TestCaseData(new MapPolygonData("test"), Resources.AreaIcon);
                yield return new TestCaseData(new MapDataCollection("test"), GuiResources.folder);
            }
        }

        private static IEnumerable<MapData> DragMapData
        {
            get
            {
                return new MapData[]
                {
                    new MapPointData("test"),
                    new MapLineData("test"),
                    new MapPolygonData("test"),
                    new MapDataCollection("test")
                };
            }
        }

        private static IEnumerable<MapData> NoMapDataCollection
        {
            get
            {
                return new MapData[]
                {
                    new MapPointData("test"),
                    new MapLineData("test"),
                    new MapPolygonData("test")
                };
            }
        }

        private static IEnumerable<TestCaseData> IsCheckedMapData
        {
            get
            {
                yield return new TestCaseData(new MapPointData("test"), false);
                yield return new TestCaseData(new MapPointData("test"), true);
                yield return new TestCaseData(new MapLineData("test"), false);
                yield return new TestCaseData(new MapLineData("test"), true);
                yield return new TestCaseData(new MapPolygonData("test"), false);
                yield return new TestCaseData(new MapPolygonData("test"), true);
            }
        }

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            mapLegendView = new MapLegendView(contextMenuBuilderProvider);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(mapLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(MapDataContext)];
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
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanCheck);
            Assert.IsNotNull(info.IsChecked);
            Assert.IsNotNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsNameFromWrappedMapData()
        {
            // Setup
            MapDataContext context = GetContext(new TestMapData());

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(context.WrappedData.Name, text);
        }

        [Test]
        [TestCaseSource(nameof(MapDataLegendImages))]
        public void Image_WrappedDataMapPointData_ReturnPointsIcon(MapData mapData, Image expectedImage)
        {
            // Setup            
            MapDataContext context = GetContext(mapData);

            // Call
            Image image = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(expectedImage, image);
        }

        [Test]
        public void ChildNodeObjects_MapDataCollection_ReturnsItemsFromMapDataCollectionList()
        {
            // Setup
            var mapData1 = new TestMapData();
            var mapData2 = new TestMapData();
            var mapData3 = new TestMapData();
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(mapData3);

            MapDataContext context = GetContext(mapDataCollection);

            // Call
            object[] objects = info.ChildNodeObjects(context);

            // Assert
            var expectedChildren = new[]
            {
                new MapDataContext(mapData3, new MapDataCollection("test")),
                new MapDataContext(mapData2, new MapDataCollection("test")),
                new MapDataContext(mapData1, new MapDataCollection("test"))
            };
            CollectionAssert.AreEqual(expectedChildren, objects);
        }

        [Test]
        [TestCaseSource(nameof(NoMapDataCollection))]
        public void ChildNodeObjects_OtherThanMapDataCollection_ReturnsEmptyArray(MapData mapData)
        {
            // Setup
            MapDataContext context = GetContext(mapData);

            // Call
            object[] objects = info.ChildNodeObjects(context);

            // Assert
            CollectionAssert.IsEmpty(objects);
        }

        [Test]
        [TestCaseSource(nameof(DragMapData))]
        public void CanDrag_Always_ReturnsTrue(MapData mapData)
        {
            // Setup
            MapDataContext context = GetContext(mapData);

            // Call
            bool canDrag = info.CanDrag(context, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanCheck_WrappedDataMapDataCollection_ReturnsFalse()
        {
            // Setup
            MapDataContext context = GetContext(new MapDataCollection("test"));

            // Call
            bool canCheck = info.CanCheck(context);

            // Assert
            Assert.IsFalse(canCheck);
        }

        [Test]
        [TestCaseSource(nameof(NoMapDataCollection))]
        public void CanCheck_WrappedDataOtherThanMapDataCollection_ReturnsTrue(MapData mapData)
        {
            // Setup
            MapDataContext context = GetContext(mapData);

            // Call
            bool canCheck = info.CanCheck(context);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [Test]
        [TestCaseSource(nameof(IsCheckedMapData))]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfMapData(MapData mapData, bool isVisible)
        {
            // Setup
            MapDataContext context = GetContext(mapData);
            context.WrappedData.IsVisible = isVisible;

            // Call
            bool isChecked = info.IsChecked(context);

            // Assert
            Assert.AreEqual(isVisible, isChecked);
        }

        [Test]
        [TestCaseSource(nameof(IsCheckedMapData))]
        public void OnNodeChecked_Always_SetsPointDataVisibilityAndNotifiesParentObservers(MapData mapData, bool initialVisibleState)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            MapDataContext context = GetContext(mapData);
            context.WrappedData.IsVisible = initialVisibleState;

            context.WrappedData.Attach(observer);

            // Call
            info.OnNodeChecked(context, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, context.WrappedData.IsVisible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_TargetIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            MapData mapData = new TestMapData();
            var mapDataCollection = new MapDataCollection("test");

            MapDataContext context = GetContext(mapData, mapDataCollection);
            MapDataContext targetContext = GetContext(mapDataCollection);

            // Call
            bool canDrop = info.CanDrop(context, targetContext);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanDrop_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            MapData mapData = new TestMapData();
            MapData mapData2 = new TestMapData();

            MapDataContext context = GetContext(mapData);
            MapDataContext targetContext = GetContext(mapData2);

            // Call
            bool canDrop = info.CanDrop(context, targetContext);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_TargetDataIsCollection_ReturnsFalse()
        {
            // Setup
            MapData mapData = new TestMapData();
            var rootCollection = new MapDataCollection("test");
            var targetCollection = new MapDataCollection("test");

            MapDataContext context = GetContext(mapData, rootCollection);
            MapDataContext targetContext = GetContext(targetCollection, rootCollection);

            // Call
            bool canDrop = info.CanDrop(context, targetContext);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanInsert_TargetIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            MapData mapData = new TestMapData();
            var mapDataCollection = new MapDataCollection("test");

            MapDataContext context = GetContext(mapData, mapDataCollection);
            MapDataContext targetContext = GetContext(mapDataCollection);

            // Call
            bool canInsert = info.CanInsert(context, targetContext);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        public void CanInsert_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            MapData mapData = new TestMapData();
            MapData mapData2 = new TestMapData();

            MapDataContext context = GetContext(mapData);
            MapDataContext targetContext = GetContext(mapData2);

            // Call
            bool canInsert = info.CanInsert(context, targetContext);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_TargetDataIsCollection_ReturnsFalse()
        {
            // Setup
            MapData mapData = new TestMapData();
            var rootCollection = new MapDataCollection("test");
            var targetCollection = new MapDataCollection("test");

            MapDataContext context = GetContext(mapData, rootCollection);
            MapDataContext targetContext = GetContext(targetCollection, rootCollection);

            // Call
            bool canDrop = info.CanInsert(context, targetContext);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDrop_MapDataMovedToPositionInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var mapData1 = new TestMapData();
            var mapData2 = new TestMapData();
            var mapData3 = new TestMapData();
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(mapData3);

            MapDataContext context1 = GetContext(mapData1);
            MapDataContext collectionContext = GetContext(mapDataCollection);

            mapDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                info.OnDrop(context1, collectionContext, collectionContext, position, treeViewControl);

                // Assert
                int reversedIndex = 2 - position;
                var wrappedCollectionData = (MapDataCollection) collectionContext.WrappedData;
                Assert.AreSame(context1.WrappedData, wrappedCollectionData.Collection.ElementAt(reversedIndex));

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(5)]
        [TestCase(50)]
        public void OnDrop_MapDataMovedToPositionOutsideRange_ThrowsException(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var mapData1 = new MapLineData("line");
            var mapData2 = new MapPolygonData("polygon");
            var mapData3 = new MapPointData("point");
            var mapDataCollection = new MapDataCollection("test");

            mapDataCollection.Add(mapData1);
            mapDataCollection.Add(mapData2);
            mapDataCollection.Add(mapData3);

            mapDataCollection.Attach(observer);
            mapLegendView.Data = mapDataCollection;

            MapDataContext context = GetContext(mapData1);
            MapDataContext collectionContext = GetContext(mapDataCollection);

            mapDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => info.OnDrop(context, collectionContext, collectionContext, position, treeViewControl);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(test);

                mocks.VerifyAll(); // Expect no update observer.
            }
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

            MapDataContext context = GetContext(removable);

            // Call
            bool canRemove = info.CanRemove(context, new MapDataCollection("collection"));

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

            MapDataContext context = GetContext(removable);

            // Call
            bool canRemove = info.CanRemove(context, null);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void CanRemove_WithNotRemovableData_ReturnFalse()
        {
            // Setup
            var notRemovable = mocks.StrictMock<MapLineData>("name");
            mocks.ReplayAll();

            MapDataContext context = GetContext(notRemovable);

            // Call
            bool canRemove = info.CanRemove(context, new MapDataCollection("collection"));

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

            MapDataContext context = GetContext(toRemove, collection);

            // Call
            info.OnNodeRemoved(context, collection);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                otherData
            }, collection.Collection);
        }

        [Test]
        [TestCaseSource(nameof(NoMapDataCollection))]
        public void ContextMenuStrip_DifferentTypesOfMapData_CallsBuilder(MapData mapData)
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

            contextMenuBuilderProvider.Expect(p => p.Get(mapData, null)).Return(builder);

            mocks.ReplayAll();

            // Call
            info.ContextMenuStrip(GetContext(mapData), null, null);

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

            var mapData = new TestFeatureBasedMapData("A")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
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

            var mapData = new TestFeatureBasedMapData("A")
            {
                IsVisible = false
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
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

            var mapData = new TestFeatureBasedMapData("A")
            {
                IsVisible = true
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
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
        public void ContextMenuStrip_EnabledZoomToAllContextMenuOnMapDataItemClicked_DoZoomToVisibleData()
        {
            // Setup
            var mapData = new TestFeatureBasedMapData("A")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            var mapControl = mocks.StrictMock<IMapControl>();
            mapControl.Expect(c => c.Data).Return(new MapDataCollection("name"));
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
        public void ContextMenuStrip_EnabledZoomToAllContextMenuOnMapDataCollectionItemClicked_DoZoomToVisibleData()
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
            var mapData = new TestFeatureBasedMapData("A")
            {
                IsVisible = true,
                Features = new[]
                {
                    new MapFeature(Enumerable.Empty<MapGeometry>())
                }
            };

            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
            {
                // Call
                TestDelegate call = () => contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        private static MapDataContext GetContext(MapData mapData, MapDataCollection mapDataCollection = null)
        {
            return new MapDataContext(mapData, mapDataCollection ?? new MapDataCollection("test"));
        }
    }
}