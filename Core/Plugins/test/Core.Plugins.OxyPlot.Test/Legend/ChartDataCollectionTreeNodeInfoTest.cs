using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartDataCollectionTreeNodeInfoTest
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
            // Setup
            mocks.ReplayAll();

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
            Assert.IsNull(info.CanDrag);
        }

        [Test]
        public void Text_Always_ReturnsNameFromChartData()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>(), "test data");

            mocks.ReplayAll();

            // Call
            var text = info.Text(chartDataCollection);

            // Assert
            Assert.AreEqual(chartDataCollection.Name, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>(), "test data");

            mocks.ReplayAll();

            // Call
            var image = info.Image(chartDataCollection);

            // Assert
            TestHelper.AssertImagesAreEqual(GuiResources.folder, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnDataReversed()
        {
            // Setup
            var chartData1 = mocks.StrictMock<ChartData>("test data");
            var chartData2 = mocks.StrictMock<ChartData>("test data");
            var chartData3 = mocks.StrictMock<ChartData>("test data");
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            }, "test data");

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(chartDataCollection);

            // Assert
            CollectionAssert.AreEqual(new[] { chartData3, chartData2, chartData1 }, objects);
        }

        [Test]
        public void CanDrop_SourceNodeTagIsNoChartData_ReturnsFalse()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrop = info.CanDrop(new object(), chartDataCollection);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_SourceNodeTagIsChartData_ReturnsTrue()
        {
            // Setup
            var chartData = mocks.StrictMock<ChartData>("test data");
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canDrop = info.CanDrop(chartData, chartDataCollection);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanInsert_SourceNodeTagIsNoChartData_ReturnsFalse()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(new object(), chartDataCollection);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_SourceNodeTagIsChartData_ReturnsTrue()
        {
            // Setup
            var chartData = mocks.StrictMock<ChartData>("test data");
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>(), "test data");

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(chartData, chartDataCollection);

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
            var chartData1 = mocks.StrictMock<ChartData>("test data");
            var chartData2 = mocks.StrictMock<ChartData>("test data");
            var chartData3 = mocks.StrictMock<ChartData>("test data");
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            }, "test data");


            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            chartDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                info.OnDrop(chartData1, chartDataCollection, chartDataCollection, position, treeViewControl);

                // Assert
                var reversedIndex = 2 - position;
                Assert.AreSame(chartData1, chartDataCollection.List.ElementAt(reversedIndex));
            }
            // Assert observer is notified in TearDown()
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(50)]
        public void OnDrop_ChartDataMovedToPositionOutsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var chartData1 = mocks.StrictMock<ChartData>("test data");
            var chartData2 = mocks.StrictMock<ChartData>("test data");
            var chartData3 = mocks.StrictMock<ChartData>("test data");
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            }, "test data");

            chartDataCollection.Attach(observer);
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => info.OnDrop(chartData1, chartDataCollection, chartDataCollection, position, treeViewControl);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(test);
            }
            // Assert UpdateObserver be not called by TearDown()
        }
    }
}