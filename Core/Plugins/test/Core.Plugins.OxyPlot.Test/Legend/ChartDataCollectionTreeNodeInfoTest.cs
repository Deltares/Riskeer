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
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartDataCollectionTreeNodeInfoTest
    {
        private ChartLegendView chartLegendView;
        private TreeNodeInfo info;
        private TreeViewControl treeViewControl;

        [SetUp]
        public void SetUp()
        {
            chartLegendView = new ChartLegendView();

            treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

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
            Assert.AreEqual(typeof(ChartDataCollection), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void Text_Always_ReturnsNameFromChartData()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var text = info.Text(chartDataCollection);

            // Assert
            Assert.AreEqual(chartDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var image = info.Image(chartDataCollection);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnDataReversed()
        {
            // Setup
            var chartData1 = new TestChartData();
            var chartData2 = new TestChartData();
            var chartData3 = new TestChartData();
            var chartDataCollection = new ChartDataCollection(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            }, "test data");

            // Call
            var objects = info.ChildNodeObjects(chartDataCollection);

            // Assert
            CollectionAssert.AreEqual(new[] { chartData3, chartData2, chartData1 }, objects);
        }

        [Test]
        public void CanDrop_SourceNodeTagIsNoChartData_ReturnsFalse()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var canDrop = info.CanDrop(new object(), chartDataCollection);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_SourceNodeTagIsChartData_ReturnsTrue()
        {
            // Setup
            var chartData = new TestChartData();
            var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var canDrop = info.CanDrop(chartData, chartDataCollection);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanInsert_SourceNodeTagIsNoChartData_ReturnsFalse()
        {
            // Setup
            var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var canInsert = info.CanInsert(new object(), chartDataCollection);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_SourceNodeTagIsChartData_ReturnsTrue()
        {
            // Setup
            var chartData = new TestChartData();
            var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

            // Call
            var canInsert = info.CanInsert(chartData, chartDataCollection);

            // Assert
            Assert.IsTrue(canInsert);
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
            var chartData1 = new TestChartData();
            var chartData2 = new TestChartData();
            var chartData3 = new TestChartData();
            var chartDataCollection = new ChartDataCollection(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            }, "test data");

            var notify = 0;
            var observer = new Observer(() => notify++);

            chartDataCollection.Attach(observer);

            // Call
            info.OnDrop(chartData1, chartDataCollection, chartDataCollection, position, treeViewControl);

            // Assert
            var reversedIndex = 2 - position;
            Assert.AreSame(chartData1, chartDataCollection.List.ElementAt(reversedIndex));

            Assert.AreEqual(1, notify);
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(50)]
        public void OnDrop_ChartDataMovedToPositionOutsideRange_ThrowsException(int position)
        {
            // Setup
            var chartData1 = new ChartLineData(Enumerable.Empty<Point2D>(), "line");
            var chartData2 = new ChartAreaData(Enumerable.Empty<Point2D>(), "area");
            var chartData3 = new ChartPointData(Enumerable.Empty<Point2D>(), "point");
            var chartData4 = new ChartMultipleAreaData(Enumerable.Empty<IEnumerable<Point2D>>(), "multiple area");
            var chartDataCollection = new ChartDataCollection(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3,
                chartData4
            }, "test data");

            var notify = 0;
            var observer = new Observer(() => notify++);
            chartDataCollection.Attach(observer);
            chartLegendView.Data = chartDataCollection;

            // Call
            TestDelegate test = () => info.OnDrop(chartData1, chartDataCollection, chartDataCollection, position, treeViewControl);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
            Assert.AreEqual(0, notify);
        }
    }
}