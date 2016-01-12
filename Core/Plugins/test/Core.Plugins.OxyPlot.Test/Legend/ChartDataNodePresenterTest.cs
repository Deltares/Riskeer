using System;
using System.Collections.ObjectModel;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Components.OxyPlot.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;
using OxyPlot;

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
            Assert.IsInstanceOf<TreeViewNodePresenterBase<IChartData>>(nodePresenter);
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
            Assert.AreEqual("PointData", treeNode.Text);
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
            Assert.AreEqual("LineData", treeNode.Text);
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
            Assert.AreEqual("AreaData", treeNode.Text);
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, treeNode.Image);
        }

        [Test]
        public void UpdateNode_ForOtherIChartData_SetsText()
        {
            // Setup
            var nodePresenter = new ChartDataNodePresenter();
            var treeNode = new TreeNode(null);
            var data = new TestChartData();

            // Call
            nodePresenter.UpdateNode(null, treeNode, data);

            // Assert
            Assert.AreEqual("TestChartData", treeNode.Text);
            Assert.IsNull(treeNode.Image);
        }
    }

    public class TestChartData : IChartData {
        public void AddTo(PlotModel model)
        {
            throw new NotImplementedException();
        }
    }
}