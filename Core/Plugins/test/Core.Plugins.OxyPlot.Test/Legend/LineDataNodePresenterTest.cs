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
    public class LineDataNodePresenterTest
    {
        [Test]
        public void UpdateNode_ForLineData_SetsTextAndIcon()
        {
            // Setup
            var nodePresenter = new LineDataNodePresenter();
            var treeNode = new TreeNode(null);

            // Call
            nodePresenter.UpdateNode(null, treeNode, new LineData(Enumerable.Empty<Tuple<double,double>>()));

            // Assert
            Assert.AreEqual("Lijn", treeNode.Text);
            TestHelper.AssertImagesAreEqual(Resources.LineIcon, treeNode.Image);
        } 
    }
}