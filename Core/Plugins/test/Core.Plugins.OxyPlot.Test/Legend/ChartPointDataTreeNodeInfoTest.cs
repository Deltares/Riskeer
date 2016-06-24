﻿using System;
using System.Collections.Generic;
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
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartPointDataTreeNodeInfoTest
    {
        private MockRepository mocks;
        private ChartLegendView chartLegendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            chartLegendView = new ChartLegendView();

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(ChartPointData)];
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
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(ChartPointData), info.TagType);
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
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var text = info.Text(pointData);

            // Assert
            Assert.AreEqual(pointData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var image = info.Image(pointData);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.PointsIcon, image);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrag = info.CanDrag(pointData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var canCheck = info.CanCheck(pointData);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfPointsData(bool isVisible)
        {
            // Setup
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            pointData.IsVisible = isVisible;

            mocks.ReplayAll();

            // Call
            var canCheck = info.IsChecked(pointData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PointDataNodeWithoutParent_SetsPointDataVisibility(bool initialVisibleState)
        {
            // Setup
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            pointData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(pointData, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, pointData.IsVisible);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_PointDataNodeWithObservableParent_SetsPointDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var pointData = mocks.StrictMock<ChartPointData>(Enumerable.Empty<Point2D>(), "test data");

            var observable = mocks.StrictMock<IObservable>();
            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            pointData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(pointData, observable);

            // Assert
            Assert.AreEqual(!initialVisibleState, pointData.IsVisible);
        }
    }
}