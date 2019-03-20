// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

namespace Core.Plugins.Map.Test.Legend
{
    [TestFixture]
    public class FeatureBasedMapDataContextTreeNodeInfoTest
    {
        private const int mapDataContextMenuZoomToAllIndex = 0;

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

            info = treeNodeInfoLookup[typeof(FeatureBasedMapDataContext)];
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
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanCheck);
            Assert.IsNotNull(info.CheckedState);
            Assert.IsNotNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_WithContext_ReturnsNameFromWrappedMapData()
        {
            // Setup
            FeatureBasedMapDataContext context = GetContext(new TestFeatureBasedMapData());

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(context.WrappedData.Name, text);
        }

        [Test]
        [TestCaseSource(nameof(MapDataLegendImages))]
        public void Image_WithContext_ReturnExpectedImage(FeatureBasedMapData mapData, Image expectedImage)
        {
            // Setup            
            FeatureBasedMapDataContext context = GetContext(mapData);

            // Call
            Image image = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(expectedImage, image);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
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
        [TestCase(true)]
        [TestCase(false)]
        public void CheckedState_WithContext_ReturnsAccordingToVisibleStateOfMapData(bool isVisible)
        {
            // Setup
            FeatureBasedMapDataContext context = GetContext(new TestFeatureBasedMapData());
            context.WrappedData.IsVisible = isVisible;

            // Call
            TreeNodeCheckedState checkedState = info.CheckedState(context);

            // Assert
            TreeNodeCheckedState expectedCheckedState = isVisible ? TreeNodeCheckedState.Checked : TreeNodeCheckedState.Unchecked;
            Assert.AreEqual(expectedCheckedState, checkedState);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_WithContext_SetMapDataVisibilityAndNotifyObservers(bool initialVisibleState)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FeatureBasedMapDataContext context = GetContext(new TestFeatureBasedMapData());
            context.WrappedData.IsVisible = initialVisibleState;

            context.WrappedData.Attach(observer);

            // Call
            info.OnNodeChecked(context, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, context.WrappedData.IsVisible);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeChecked_WithContext_NotifyObserversOfParentMapDataCollections()
        {
            // Setup
            var collectionObserver = mocks.StrictMock<IObserver>();
            collectionObserver.Expect(o => o.UpdateObserver());
            var parentCollectionObserver = mocks.StrictMock<IObserver>();
            parentCollectionObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var featureBasedMapData = new TestFeatureBasedMapData();
            var nestedMapDataCollection = new MapDataCollection("nested");
            nestedMapDataCollection.Add(featureBasedMapData);
            var mapDataCollection = new MapDataCollection("test");
            mapDataCollection.Add(nestedMapDataCollection);

            MapDataCollectionContext rootCollectionContext = GetContext(mapDataCollection);
            MapDataCollectionContext nestedCollectionContext = GetContext(nestedMapDataCollection, rootCollectionContext);
            FeatureBasedMapDataContext featureBasedMapDataContext = GetContext(featureBasedMapData, nestedCollectionContext);

            nestedMapDataCollection.Attach(collectionObserver);
            mapDataCollection.Attach(parentCollectionObserver);

            // Call
            info.OnNodeChecked(featureBasedMapDataContext, null);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_WithRemovableDataAndCollection_ReturnTrue()
        {
            // Setup
            var removable = mocks.StrictMultiMock<FeatureBasedMapData>(new[]
            {
                typeof(IRemovable)
            }, "name");
            mocks.ReplayAll();

            FeatureBasedMapDataContext context = GetContext(removable);

            // Call
            bool canRemove = info.CanRemove(context, context.ParentMapData);

            // Assert
            Assert.IsTrue(canRemove);
        }

        [Test]
        public void CanRemove_WithoutCollection_ReturnFalse()
        {
            // Setup
            var removable = mocks.StrictMultiMock<FeatureBasedMapData>(new[]
            {
                typeof(IRemovable)
            }, "name");
            mocks.ReplayAll();

            FeatureBasedMapDataContext context = GetContext(removable);

            // Call
            bool canRemove = info.CanRemove(context, null);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void CanRemove_WithNotRemovableData_ReturnFalse()
        {
            // Setup
            var notRemovable = mocks.StrictMock<FeatureBasedMapData>("name");
            mocks.ReplayAll();

            FeatureBasedMapDataContext context = GetContext(notRemovable);

            // Call
            bool canRemove = info.CanRemove(context, context.ParentMapData);

            // Assert
            Assert.IsFalse(canRemove);
        }

        [Test]
        public void OnNodeRemoved_WithRemovableDataToRemove_DataRemoved()
        {
            // Setup
            var toRemove = mocks.StrictMultiMock<FeatureBasedMapData>(new[]
            {
                typeof(IRemovable)
            }, "name");
            var otherData = mocks.Stub<MapLineData>("name");
            mocks.ReplayAll();

            var collection = new MapDataCollection("collection");
            collection.Add(toRemove);
            collection.Add(otherData);

            FeatureBasedMapDataContext context = GetContext(toRemove, GetContext(collection));

            // Call
            info.OnNodeRemoved(context, context.ParentMapData);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                otherData
            }, collection.Collection);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mapData = new TestFeatureBasedMapData();
            FeatureBasedMapDataContext context = GetContext(mapData);
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

            contextMenuBuilderProvider.Expect(p => p.Get(context, null)).Return(builder);

            mocks.ReplayAll();

            // Call
            info.ContextMenuStrip(context, null, null);

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_VisibleMapDataWithFeatures_ZoomToAllItemEnabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            var mapData = new TestFeatureBasedMapData
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
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, mapDataContextMenuZoomToAllIndex,
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

            var mapData = new TestFeatureBasedMapData
            {
                IsVisible = false
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, mapDataContextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Om het zoomniveau aan te passen moet de kaartlaag zichtbaar zijn.",
                                                              Resources.ZoomToAllIcon,
                                                              false);
            }
        }

        [Test]
        public void ContextMenuStrip_VisibleMapDataWithoutFeatures_ZoomToAllItemDisabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);

            mocks.ReplayAll();

            var mapData = new TestFeatureBasedMapData
            {
                IsVisible = true
            };

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(GetContext(mapData), null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, mapDataContextMenuZoomToAllIndex,
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
            var mapData = new TestFeatureBasedMapData
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
                contextMenu.Items[mapDataContextMenuZoomToAllIndex].PerformClick();

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
                TestDelegate call = () => contextMenu.Items[mapDataContextMenuZoomToAllIndex].PerformClick();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        private static FeatureBasedMapDataContext GetContext(FeatureBasedMapData mapData, MapDataCollectionContext parentMapDataCollectionContext = null)
        {
            return new FeatureBasedMapDataContext(mapData, parentMapDataCollectionContext ?? new MapDataCollectionContext(new MapDataCollection("test"), null));
        }

        private static MapDataCollectionContext GetContext(MapDataCollection mapDataCollection, MapDataCollectionContext parentMapDataCollectionContext = null)
        {
            return new MapDataCollectionContext(mapDataCollection, parentMapDataCollectionContext ?? new MapDataCollectionContext(new MapDataCollection("test"), null));
        }
    }
}