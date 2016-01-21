using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ReturnsTreeViewNodePresenterBase()
        {
            // Call
            var nodePresenter = new ChartNodePresenter();

            // Assert
            Assert.IsInstanceOf<TreeViewNodePresenterBase<ChartDataCollection>>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(ChartDataCollection), nodePresenter.NodeTagType);
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void CanDrop_ForChartData_ReturnsGivenOperations(DragOperations operations)
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();

            // Call
            DragOperations result = nodePresenter.CanDrop(new TestChartData(), null, null, operations);

            // Assert
            Assert.AreEqual(operations, result);
        }

        [Test]
        [TestCase(DragOperations.Move)]
        [TestCase(DragOperations.None)]
        public void CanDrop_ForNoneChartData_ReturnsNone(DragOperations operations)
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();

            // Call
            DragOperations result = nodePresenter.CanDrop(null, null, null, operations);

            // Assert
            Assert.AreEqual(DragOperations.None, result);
        }

        [Test]
        public void CanInsert_ForChartData_ReturnsTrue()
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();

            // Call & Assert
            Assert.IsTrue(nodePresenter.CanInsert(new TestChartData(), null, null));
        }

        [Test]
        public void CanInsert_ForNoneChartData_ReturnsFalse()
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();

            // Call & Assert
            Assert.IsFalse(nodePresenter.CanInsert(null, null, null));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDragDrop_ChartDataAndBaseChartInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();
            ChartDataCollection chart = CreateTestBaseChart();


            ChartData testElement = chart.List.ElementAt(0);

            // Call
            nodePresenter.OnDragDrop(testElement, null, chart, 0, position);

            // Assert
            var reversedIndex = 2 - position;
            Assert.AreSame(testElement, chart.List.ElementAt(reversedIndex));
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(3)]
        [TestCase(50)]
        public void OnDragDrop_ChartDataAndBaseChartOutsideRange_ThrowsArgumentOutOfRangeException(int position)
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();
            ChartDataCollection chart = CreateTestBaseChart();

            ChartData testElement = chart.List.ElementAt(0);

            // Call
            TestDelegate test = () => nodePresenter.OnDragDrop(testElement, null, chart, 0, position);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        [Test]
        public void UpdateNode_Always_SetsTextAndIcon()
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, null);

            // Assert
            Assert.AreEqual("Grafiek", treeNode.Text);
            TestHelper.AssertImagesAreEqual(Resources.folder, treeNode.Image);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnsReverseSeries()
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();
            ChartDataCollection chart = CreateTestBaseChart();

            // Call
            IEnumerable result = nodePresenter.GetChildNodeObjects(chart);

            // Assert
            CollectionAssert.AreEqual(chart.List.Reverse(), result);
        }

        private static ChartDataCollection CreateTestBaseChart()
        {
            return new ChartDataCollection(new List<ChartData>
            {
                new LineData(new List<Tuple<double, double>>()),
                new PointData(new List<Tuple<double, double>>()),
                new AreaData(new List<Tuple<double, double>>())
            });
        }
    }
}