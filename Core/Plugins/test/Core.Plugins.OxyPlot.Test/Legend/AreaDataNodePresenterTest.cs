using System;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class AreaDataNodePresenterTest
    {
        [Test]
        public void UpdateNode_ForAreaData_SetsTextAndIcon()
        {
            // Setup
            var nodePresenter = new AreaDataNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, new AreaData(Enumerable.Empty<Tuple<double, double>>()));

            // Assert
            Assert.AreEqual("Gebied", treeNode.Text);
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, treeNode.Image);
        }
    }
}