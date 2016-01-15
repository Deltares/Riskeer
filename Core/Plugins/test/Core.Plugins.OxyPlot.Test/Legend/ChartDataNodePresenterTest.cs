using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartDataNodePresenterTest
    {
        private readonly Collection<Tuple<double, double>> emptyCollection = new Collection<Tuple<double,double>>();

        [Test]
        public void DefaultConstructor_ReturnsTreeViewNodePresenterBase()
        {
            // Call
            var nodePresenter = new ChartDataNodePresenter();

            // Assert
            Assert.IsInstanceOf<TreeViewNodePresenterBase<ChartData>>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(ChartData), nodePresenter.NodeTagType);
        }

        [Test]
        public void CanDrag_Always_ReturnsMove()
        {
            // Setup
            var nodePresenter = new ChartDataNodePresenter();

            // Call
            var operation = nodePresenter.CanDrag(null);

            // Assert
            Assert.AreEqual(DragOperations.Move, operation);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenTreeViewWithChartWithData_WhenOnNodeChecked_SetIsVisibleForData(bool isVisible)
        {
            // Given
            var lineData = new LineData(new List<Tuple<double,double>>());
            var pointData = new PointData(new List<Tuple<double,double>>());
            var areaData = new AreaData(new List<Tuple<double,double>>());

            var legendTreeView = new LegendTreeView
            {
                Chart = new BaseChart
                {
                    Data = new ChartData[]{lineData, pointData, areaData}
                }
            };
            var nodePresenter = new ChartDataNodePresenter
            {
                TreeView = legendTreeView
            };
            var lineNode = new TreeNode(legendTreeView)
            {
                Tag = lineData,
                Checked = isVisible
            };
            var pointNode = new TreeNode(legendTreeView)
            {
                Tag = pointData,
                Checked = isVisible
            };
            var areaNode = new TreeNode(legendTreeView)
            {
                Tag = areaData,
                Checked = isVisible
            };

            // When
            nodePresenter.OnNodeChecked(lineNode);
            nodePresenter.OnNodeChecked(pointNode);
            nodePresenter.OnNodeChecked(areaNode);

            // Then
            Assert.AreEqual(isVisible, lineData.IsVisible);
            Assert.AreEqual(isVisible, pointData.IsVisible);
            Assert.AreEqual(isVisible, areaData.IsVisible);
        }

        [Test]
        public void UpdateNode_ForPointData_SetsProperties()
        {
            // Setup
            var nodePresenter = new ChartDataNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, new PointData(emptyCollection));

            // Assert
            Assert.AreEqual("Punten", treeNode.Text);
            Assert.IsTrue(treeNode.ShowCheckBox);
            Assert.IsTrue(treeNode.Checked);
            TestHelper.AssertImagesAreEqual(Resources.PointsIcon, treeNode.Image);
        }

        [Test]
        public void UpdateNode_ForLineData_SetsTextAndIcon()
        {
            // Setup
            var nodePresenter = new ChartDataNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, new LineData(emptyCollection));

            // Assert
            Assert.AreEqual("Lijn", treeNode.Text);
            TestHelper.AssertImagesAreEqual(Resources.LineIcon, treeNode.Image);
        }

        [Test]
        public void UpdateNode_ForAreaData_SetsTextAndIcon()
        {
            // Setup
            var nodePresenter = new ChartDataNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, new AreaData(emptyCollection));

            // Assert
            Assert.AreEqual("Gebied", treeNode.Text);
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, treeNode.Image);
        }

        [Test]
        public void UpdateNode_ForOtherChartData_ThrowsNotSupportedException()
        {
            // Setup
            var nodePresenter = new ChartDataNodePresenter();
            var treeNode = new TreeNode(null);
            var data = new TestChartData();

            // Call
            TestDelegate test = () => nodePresenter.UpdateNode(null, treeNode, data);

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }
    }
}