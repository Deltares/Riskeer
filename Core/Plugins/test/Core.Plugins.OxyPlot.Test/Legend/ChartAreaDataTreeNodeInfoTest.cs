using System;
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
    public class ChartAreaDataTreeNodeInfoTest
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

            info = treeNodeInfoLookup[typeof(ChartAreaData)];
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
            Assert.AreEqual(typeof(ChartAreaData), info.TagType);
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
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var text = info.Text(areaData);

            // Assert
            Assert.AreEqual(areaData.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var image = info.Image(areaData);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, image);
        }

        [Test]
        public void CanDrag_Always_ReturnsTrue()
        {
            // Setup
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrag = info.CanDrag(areaData, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

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
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");
            areaData.IsVisible = isVisible;

            mocks.ReplayAll();

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
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");

            mocks.ReplayAll();

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
            var observable = mocks.StrictMock<IObservable>();
            var areaData = mocks.StrictMock<ChartAreaData>(Enumerable.Empty<Point2D>(), "test data");

            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            areaData.IsVisible = initialVisibleState;

            // Call
            info.OnNodeChecked(areaData, observable);

            // Assert
            Assert.AreEqual(!initialVisibleState, areaData.IsVisible);
        }
    }
}