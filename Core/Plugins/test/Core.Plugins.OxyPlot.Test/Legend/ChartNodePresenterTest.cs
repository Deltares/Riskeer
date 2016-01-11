using Core.Common.Controls.TreeView;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using Core.Components.OxyPlot.Data;
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
            Assert.IsInstanceOf<TreeViewNodePresenterBase<BaseChart>>(nodePresenter);
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
        public void GetChildNodeObjects_Always_ReturnsSeries()
        {
            // Setup
            var nodePresenter = new ChartNodePresenter();
            var treeNode = new TreeNode(null);
            var chart = new BaseChart();

            // Call
            var result = nodePresenter.GetChildNodeObjects(chart);

            // Assert
            Assert.AreSame(chart.Model.Series, result);
        } 
    }
}