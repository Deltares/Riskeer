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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PresentationObjects;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Chart.Test.Legend
{
    [TestFixture]
    public class ChartDataCollectionTreeNodeInfoTest
    {
        private ChartLegendView chartLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            chartLegendView = new ChartLegendView();

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(ChartDataCollection)];
        }

        [TearDown]
        public void TearDown()
        {
            chartLegendView.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNull(info.ContextMenuStrip);
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
            var text = info.Text(chartDataCollection);

            // Assert
            Assert.AreEqual(chartDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection("test data");

            // Call
            var image = info.Image(chartDataCollection);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenWithContextAndDataReversed()
        {
            // Setup
            TestChartData chartData1 = new TestChartData();
            TestChartData chartData2 = new TestChartData();
            TestChartData chartData3 = new TestChartData();
            ChartDataCollection chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);

            // Call
            var objects = info.ChildNodeObjects(chartDataCollection);

            // Assert
            var expectedChildren = new[]
            {
                new ChartDataContext(chartData3, new ChartDataCollection("test")),
                new ChartDataContext(chartData2, new ChartDataCollection("test")),
                new ChartDataContext(chartData1, new ChartDataCollection("test"))
            };
            CollectionAssert.AreEqual(expectedChildren, objects);
        }

        [Test]
        public void CanDrop_TargetParentIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartDataCollection chartDataCollection = new ChartDataCollection("test");

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
            ChartDataCollection chartDataCollection = new ChartDataCollection("test");

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
            ChartDataCollection chartDataCollection = new ChartDataCollection("test");

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
            ChartDataCollection chartDataCollection = new ChartDataCollection("test");

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
            var canInsert = info.CanDrag(null, null);

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
            var mocks = new MockRepository();
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

            var context1 = GetContext(chartData1);

            chartDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                info.OnDrop(context1, chartDataCollection, chartDataCollection, position, treeViewControl);

                // Assert
                var reversedIndex = 2 - position;
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
            var mocks = new MockRepository();
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

            var context1 = GetContext(chartData1);

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

        private static ChartDataContext GetContext(ChartData chartData, ChartDataCollection chartDataCollection = null)
        {
            return new ChartDataContext(chartData, chartDataCollection ?? new ChartDataCollection("test"));
        }
    }
}