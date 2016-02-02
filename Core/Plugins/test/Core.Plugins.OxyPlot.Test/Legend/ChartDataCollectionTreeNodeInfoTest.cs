using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartDataCollectionTreeNodeInfoTest
    {
        private MockRepository mocks;
        private LegendView legendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            legendView = new LegendView();

            var treeView = TypeUtils.GetField<TreeView>(legendView, "treeView");

            info = treeView.TreeViewController.TreeNodeInfos.First(tni => tni.TagType == typeof(ChartDataCollection));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(ChartDataCollection), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
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
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>());

            mocks.ReplayAll();

            // Call
            var text = info.Text(chartDataCollection);

            // Assert
            Assert.AreEqual(Resources.General_Chart, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>());

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
            var chartData1 = mocks.StrictMock<ChartData>();
            var chartData2 = mocks.StrictMock<ChartData>();
            var chartData3 = mocks.StrictMock<ChartData>();
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            });

            mocks.ReplayAll();

            // Call
            var objects = info.ChildNodeObjects(chartDataCollection);

            // Assert
            CollectionAssert.AreEqual(new[] { chartData3, chartData2, chartData1 }, objects);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void CanDrop_SourceNodeTagIsNoChartData_ReturnsDragOperationsNone(DragOperations validDragOperations)
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>());
            var targetNode = new TreeNode
            {
                Tag = chartDataCollection
            };
            var sourceNode = new TreeNode { Tag = new object() };

            mocks.ReplayAll();

            // Call
            var validOperations = info.CanDrop(sourceNode, targetNode, validDragOperations);

            // Assert
            Assert.AreEqual(DragOperations.None, validOperations);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void CanDrop_SourceNodeTagIsChartData_ReturnsGivenDragOperations(DragOperations validDragOperations)
        {
            // Setup
            var chartData = mocks.StrictMock<ChartData>();
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>());
            var targetNode = new TreeNode
            {
                Tag = chartDataCollection
            };
            var sourceNode = new TreeNode { Tag = chartData };

            mocks.ReplayAll();

            // Call
            var validOperations = info.CanDrop(sourceNode, targetNode, validDragOperations);

            // Assert
            Assert.AreEqual(validDragOperations, validOperations);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_SourceNodeTagIsNoChartData_ReturnsFalse()
        {
            // Setup
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>());
            var targetNode = new TreeNode
            {
                Tag = chartDataCollection
            };
            var sourceNode = new TreeNode { Tag = new object() };

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(sourceNode, targetNode);

            // Assert
            Assert.IsFalse(canInsert);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_SourceNodeTagIsChartData_ReturnsTrue()
        {
            // Setup
            var chartData = mocks.StrictMock<ChartData>();
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>());
            var targetNode = new TreeNode
            {
                Tag = chartDataCollection
            };
            var sourceNode = new TreeNode { Tag = chartData };

            mocks.ReplayAll();

            // Call
            var canInsert = info.CanInsert(sourceNode, targetNode);

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
            var chartData1 = mocks.StrictMock<ChartData>();
            var chartData2 = mocks.StrictMock<ChartData>();
            var chartData3 = mocks.StrictMock<ChartData>();
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            });
            var targetNode = new TreeNode
            {
                Tag = chartDataCollection
            };
            var sourceNode = new TreeNode { Tag = chartData1 };

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            chartDataCollection.Attach(observer);

            // Call
            info.OnDrop(sourceNode, targetNode, DragOperations.Move, position);

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
            var chartData1 = mocks.StrictMock<ChartData>();
            var chartData2 = mocks.StrictMock<ChartData>();
            var chartData3 = mocks.StrictMock<ChartData>();
            var chartDataCollection = mocks.StrictMock<ChartDataCollection>(new List<ChartData>
            {
                chartData1,
                chartData2,
                chartData3
            });
            var targetNode = new TreeNode
            {
                Tag = chartDataCollection
            };
            var sourceNode = new TreeNode { Tag = chartData1 };

            chartDataCollection.Attach(observer);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => info.OnDrop(sourceNode, targetNode, DragOperations.Move, position);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);

            mocks.VerifyAll(); // UpdateObserver should be not called
        }
    }
}