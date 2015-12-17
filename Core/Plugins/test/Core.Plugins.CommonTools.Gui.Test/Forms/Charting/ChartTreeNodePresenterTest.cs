using System.Collections;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Plugins.CommonTools.Gui.Forms.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test.Forms.Charting
{
    [TestFixture]
    public class ChartTreeNodePresenterTest
    {
        [Test]
        public void CanInsert_TreeViewHasSorter_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceNode = mocks.Stub<ITreeNode>();
            var targetNode = mocks.Stub<ITreeNode>();

            var treeView = mocks.Stub<ITreeView>();

            var chart = mocks.Stub<IChart>();

            var guiPlugin = mocks.Stub<GuiPlugin>();
            treeView.TreeViewNodeSorter = mocks.Stub<IComparer>();

            mocks.ReplayAll();

            var nodePresenter = new ChartTreeNodePresenter() { TreeView = treeView };

            // Call
            var insertionAllowed = nodePresenter.CanInsert(chart, sourceNode, targetNode);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_TreeViewDoesNotHaveSorter_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceNode = mocks.Stub<ITreeNode>();
            var targetNode = mocks.Stub<ITreeNode>();

            var treeView = mocks.Stub<ITreeView>();

            var chart = mocks.Stub<IChart>();

            var guiPlugin = mocks.Stub<GuiPlugin>();
            mocks.ReplayAll();

            // Precondition
            Assert.IsNull(treeView.TreeViewNodeSorter);

            var nodePresenter = new ChartTreeNodePresenter() { TreeView = treeView };

            // Call
            var insertionAllowed = nodePresenter.CanInsert(chart, sourceNode, targetNode);

            // Assert
            Assert.IsTrue(insertionAllowed);
            mocks.VerifyAll();
        }
    }
}