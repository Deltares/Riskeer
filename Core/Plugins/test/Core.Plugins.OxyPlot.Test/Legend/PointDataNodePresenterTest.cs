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
    public class PointDataNodePresenterTest
    {
        [Test]
        public void UpdateNode_ForPointData_SetsProperties()
        {
            // Setup
            var nodePresenter = new PointDataNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, new PointData(Enumerable.Empty<Tuple<double, double>>()));

            // Assert
            Assert.AreEqual("Punten", treeNode.Text);
            Assert.IsTrue(treeNode.ShowCheckBox);
            Assert.IsTrue(treeNode.Checked);
            TestHelper.AssertImagesAreEqual(Resources.PointsIcon, treeNode.Image);
        }
    }
}