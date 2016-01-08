using System.Collections;

using Core.Common.Controls.TreeView;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui.NodePresenters;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Plugins.SharpMapGis.Test.Forms.NodePresenters
{
    [TestFixture]
    public class MapProjectTreeViewNodePresenterTest
    {
        [Test]
        public void CanInsert_TreeViewHasSorter_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceNode = mocks.Stub<TreeNode>();
            var targetNode = mocks.Stub<TreeNode>();

            var treeView = mocks.Stub<TreeView>();
            treeView.TreeViewNodeSorter = mocks.Stub<IComparer>();

            mocks.ReplayAll();
            
            var map = new Map();

            var nodePresenter = new MapProjectTreeViewNodePresenter { TreeView = treeView };

            // Call
            var insertionAllowed = nodePresenter.CanInsert(map, sourceNode, targetNode);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_TreeViewDoesNotHaveSorter_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var sourceNode = mocks.Stub<TreeNode>();
            var targetNode = mocks.Stub<TreeNode>();

            var treeView = mocks.Stub<TreeView>();
            mocks.ReplayAll();

            var map = new Map();

            var nodePresenter = new MapProjectTreeViewNodePresenter { TreeView = treeView };

            // Precondition
            Assert.IsNull(nodePresenter.TreeView.TreeViewNodeSorter);

            // Call
            var insertionAllowed = nodePresenter.CanInsert(map, sourceNode, targetNode);

            // Assert
            Assert.IsTrue(insertionAllowed);
            mocks.VerifyAll();
        }
    }
}