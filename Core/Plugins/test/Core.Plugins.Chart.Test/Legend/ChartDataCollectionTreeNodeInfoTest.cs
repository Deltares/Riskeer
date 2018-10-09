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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Core.Components.Chart.TestUtil;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;
using ChartResources = Core.Plugins.Chart.Properties.Resources;

namespace Core.Plugins.Chart.Test.Legend
{
    [TestFixture]
    public class ChartDataCollectionTreeNodeInfoTest
    {
        private const int contextMenuZoomToAllIndex = 0;
        private ChartLegendView chartLegendView;
        private TreeNodeInfo info;
        private IContextMenuBuilderProvider contextMenuBuilderProvider;
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            chartLegendView = new ChartLegendView(contextMenuBuilderProvider);

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(ChartDataCollection)];
        }

        [TearDown]
        public void TearDown()
        {
            chartLegendView.Dispose();
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
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsNameFromChartData()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("test data");

            // Call
            string text = info.Text(chartDataCollection);

            // Assert
            Assert.AreEqual(chartDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("test data");

            // Call
            Image image = info.Image(chartDataCollection);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenWithContextAndDataReversed()
        {
            // Setup
            var chartData1 = new TestChartData();
            var chartData2 = new TestChartData();
            var chartData3 = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);

            // Call
            object[] objects = info.ChildNodeObjects(chartDataCollection);

            // Assert
            var expectedChildren = new[]
            {
                new ChartDataContext(chartData3, chartDataCollection),
                new ChartDataContext(chartData2, chartDataCollection),
                new ChartDataContext(chartData1, chartDataCollection)
            };
            CollectionAssert.AreEqual(expectedChildren, objects);
        }

        [Test]
        public void CanDrop_TargetParentIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            ChartData chartData = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, chartDataCollection);

            // Call
            bool canDrop = info.CanDrop(context, chartDataCollection);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanDrop_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            ChartData chartData = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, chartDataCollection);

            // Call
            bool canDrop = info.CanDrop(context, GetContext(new ChartDataCollection("test")));

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanInsert_TargetParentIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            ChartData chartData = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, chartDataCollection);

            // Call
            bool canInsert = info.CanInsert(context, chartDataCollection);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        public void CanInsert_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            ChartData chartData = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, chartDataCollection);

            // Call
            bool canInsert = info.CanInsert(context, GetContext(new ChartDataCollection("test")));

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Call
            bool canInsert = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDrop_ChartDataMovedToPositionInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var chartData1 = new TestChartData();
            var chartData2 = new TestChartData();
            var chartData3 = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);

            ChartDataContext context1 = GetContext(chartData1);

            chartDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                info.OnDrop(context1, chartDataCollection, chartDataCollection, position, treeViewControl);

                // Assert
                int reversedIndex = 2 - position;
                Assert.AreSame(context1.WrappedData, chartDataCollection.Collection.ElementAt(reversedIndex));

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(50)]
        public void OnDrop_ChartDataMovedToPositionOutsideRange_ThrowsException(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var chartData1 = new ChartLineData("line");
            var chartData2 = new ChartAreaData("area");
            var chartData3 = new ChartPointData("point");
            var chartData4 = new ChartMultipleAreaData("multiple area");
            var chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);
            chartDataCollection.Add(chartData4);

            chartDataCollection.Attach(observer);
            chartLegendView.Data = chartDataCollection;

            ChartDataContext context1 = GetContext(chartData1);

            chartDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => info.OnDrop(context1, chartDataCollection, chartDataCollection, position, treeViewControl);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(test);

                mocks.VerifyAll(); // Expect no update observer.
            }
        }

        [Test]
        public void ContextMenuStrip_WithChartDataCollection_CallsContextMenuBuilderMethods()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("test data");

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                contextMenuBuilderProvider.Expect(cmbp => cmbp.Get(null, null))
                                          .IgnoreArguments()
                                          .Return(menuBuilder);
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(chartDataCollection, null, treeViewControl);

                // Assert
                // Asserts done in TearDown
            }
        }

        [Test]
        public void ContextMenuStrip_InvisibleChartDataInChartDataCollection_ZoomToAllDisabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            var pointData = new ChartPointData("test data")
            {
                IsVisible = false
            };
            var chartDataCollection = new ChartDataCollection("test data");
            chartDataCollection.Add(pointData);

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(chartDataCollection, null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Om het zoomniveau aan te passen moet er minstens één gegevensreeks in deze map met gegevensreeksen zichtbaar zijn.",
                                                              ChartResources.ZoomToAllIcon,
                                                              false);
            }
        }

        [Test]
        public void ContextMenuStrip_VisibleChartDataWithoutFeaturesInChartDataCollection_ZoomToAllDisabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            var lineData = new ChartLineData("test line")
            {
                IsVisible = false,
                Points = new[]
                {
                    new Point2D(1, 5)
                }
            };
            var pointData = new ChartPointData("test data")
            {
                IsVisible = true
            };
            var chartDataCollection = new ChartDataCollection("test data");
            chartDataCollection.Add(pointData);
            chartDataCollection.Add(lineData);

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(chartDataCollection, null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Om het zoomniveau aan te passen moet minstens één van de zichtbare gegevensreeksen in deze map met gegevensreeksen elementen bevatten.",
                                                              ChartResources.ZoomToAllIcon,
                                                              false);
            }
        }

        [Test]
        public void ContextMenuStrip_ChartDataCollectionWithVisibleChartData_ZoomToAllEnabled()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            var chartPointData = new ChartPointData("test")
            {
                IsVisible = true,
                Points = new[]
                {
                    new Point2D(0, 1)
                }
            };
            var chartDataCollection = new ChartDataCollection("test data");
            chartDataCollection.Add(chartPointData);

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(chartDataCollection, null, null))
            {
                // Assert
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuZoomToAllIndex,
                                                              "&Zoom naar alles",
                                                              "Zet het zoomniveau van de grafiek dusdanig dat alle zichtbare gegevensreeksen in deze map met gegevensreeksen precies in het beeld passen.",
                                                              ChartResources.ZoomToAllIcon);
            }
        }

        [Test]
        public void ContextMenuStrip_EnabledZoomToAllContextMenuItemClicked_DoZoomToVisibleData()
        {
            // Setup
            var lineData = new ChartLineData("test line")
            {
                IsVisible = true,
                Points = new[]
                {
                    new Point2D(1, 5)
                }
            };
            var chartDataCollection = new ChartDataCollection("test data");
            chartDataCollection.Add(lineData);

            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            var chartControl = mocks.StrictMock<IChartControl>();
            chartControl.Expect(c => c.Data).Return(new ChartDataCollection("name"));
            chartControl.Expect(c => c.ZoomToAllVisibleLayers(chartDataCollection));

            mocks.ReplayAll();

            chartLegendView.ChartControl = chartControl;

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(chartDataCollection, null, null))
            {
                // Call
                contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                // Assert expectancies are called in TearDown()
            }
        }

        [Test]
        public void ContextMenuStrip_DisabledZoomToAllContextMenuItemClicked_DoesNotThrow()
        {
            // Setup
            var builder = new CustomItemsOnlyContextMenuBuilder();
            contextMenuBuilderProvider.Expect(p => p.Get(null, null)).IgnoreArguments().Return(builder);
            mocks.ReplayAll();

            var lineData = new ChartLineData("test line");
            var chartDataCollection = new ChartDataCollection("test data");
            chartDataCollection.Add(lineData);

            // Call
            using (ContextMenuStrip contextMenu = info.ContextMenuStrip(chartDataCollection, null, null))
            {
                // Call
                TestDelegate call = () => contextMenu.Items[contextMenuZoomToAllIndex].PerformClick();

                // Assert
                Assert.DoesNotThrow(call);
            }
        }

        private static ChartDataContext GetContext(ChartData chartData, ChartDataCollection chartDataCollection = null)
        {
            return new ChartDataContext(chartData, chartDataCollection ?? new ChartDataCollection("test"));
        }
    }
}