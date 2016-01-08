using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class TreeNodeListTest
    {
        [Test]
        public void Clear_ChildrenSubscribed_ChildrenUnsubscribe()
        {
            // Setup
            var mocks = new MockRepository();
            var treeView = mocks.Stub<ITreeView>();
            var someTreeNodeCollectionContainer = new System.Windows.Forms.TreeNode();
            var parentNode = new TreeNode(treeView);
            var childNode = new TreeNode(treeView);
            var childChildNode = new TreeNode(treeView);

            var parentData = mocks.Stub<IObservable>();
            var childData = mocks.Stub<IObservable>();
            var childChildData = mocks.Stub<IObservable>();

            parentData.Expect(p => p.Attach(parentNode));
            childData.Expect(p => p.Attach(childNode));
            childChildData.Expect(p => p.Attach(childChildNode));

            parentData.Expect(p => p.Detach(parentNode));
            childData.Expect(p => p.Detach(childNode));
            childChildData.Expect(p => p.Detach(childChildNode));

            mocks.ReplayAll();

            parentNode.Tag = parentData;
            childNode.Tag = childData;
            childChildNode.Tag = childChildData;

            var treeNodeList = new TreeNodeList(someTreeNodeCollectionContainer.Nodes);
            treeNodeList.Add(parentNode);
            treeNodeList.Add(childNode);
            treeNodeList.Add(childChildNode);

            // Call
            treeNodeList.Clear();

            // Assert
            mocks.VerifyAll();
        } 
    }
}