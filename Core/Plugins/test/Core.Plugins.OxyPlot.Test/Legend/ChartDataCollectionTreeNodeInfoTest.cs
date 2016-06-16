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
using Resources = Core.Plugins.OxyPlot.Properties.Resources;
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

            mocks.VerifyAll();
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

            mocks.VerifyAll();
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

            mocks.VerifyAll();
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

            mocks.VerifyAll();
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

            mocks.VerifyAll();
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

            mocks.VerifyAll();
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

            mocks.VerifyAll();
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

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            chartDataCollection.Attach(observer);

            // Call
            info.OnDrop(chartData1, chartDataCollection, chartDataCollection, position, treeViewControlMock);

            // Assert
            var reversedIndex = 2 - position;
            Assert.AreSame(chartData1, chartDataCollection.List.ElementAt(reversedIndex));

            mocks.VerifyAll(); // UpdateObserver should be called
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

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => info.OnDrop(chartData1, chartDataCollection, chartDataCollection, position, treeViewControlMock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);

            mocks.VerifyAll(); // UpdateObserver should be not called
        }
    }
}