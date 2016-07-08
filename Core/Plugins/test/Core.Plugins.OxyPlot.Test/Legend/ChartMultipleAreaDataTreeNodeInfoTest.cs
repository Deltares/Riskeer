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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartMultipleAreaDataTreeNodeInfoTest
    {
        private ChartLegendView chartLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            chartLegendView = new ChartLegendView();

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(ChartMultipleAreaData)];
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
            Assert.AreEqual(typeof(ChartMultipleAreaData), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsNameFromChartData()
        {
            // Setup
            var areaData = CreateChartMultipleAreaData();

            // Call
            var text = info.Text(areaData);

            // Assert
            Assert.AreEqual(areaData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var areaData = CreateChartMultipleAreaData();

            // Call
            var image = info.Image(areaData);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, image);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            var areaData = CreateChartMultipleAreaData();

            // Call
            var canDrag = info.CanDrag(areaData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var areaData = CreateChartMultipleAreaData();

            // Call
            var canCheck = info.CanCheck(areaData);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfAreaData(bool isVisible)
        {
            // Setup
            var areaData = CreateChartMultipleAreaData();
            areaData.IsVisible = isVisible;

            // Call
            var canCheck = info.IsChecked(areaData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_AreaDataNodeWithoutParent_SetsAreaDataVisibility(bool initialVisibleState)
        {
            // Setup
            var areaData = CreateChartMultipleAreaData();

            areaData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(areaData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, areaData.IsVisible);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_AreaDataNodeWithObservableParent_SetsAreaDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var notified = 0;
            var observer = new Observer(() => notified++);
            var chartData = new ChartDataCollection(new List<ChartData>(), "Collection");
            observer.Observable = chartData;
            chartLegendView.Data = chartData;

            var areaData = CreateChartMultipleAreaData();

            areaData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(areaData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, areaData.IsVisible);
            Assert.AreEqual(1, notified);
        }

        private ChartMultipleAreaData CreateChartMultipleAreaData()
        {
            return new ChartMultipleAreaData(Enumerable.Empty<Collection<Point2D>>(), "test data");
        }
    }
}