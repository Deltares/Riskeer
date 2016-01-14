using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;

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
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(IChartData), nodePresenter.NodeTagType);
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
        public void UpdateNode_ForOtherIChartData_ThrowsNotSupportedException()
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

    public class TestChartData : IChartData {
        public bool IsVisible { get; set; }
        public IEnumerable<Tuple<double, double>> Points { get; private set; }
    }
}