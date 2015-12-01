using System.Drawing;
using Core.Common.Controls.Swf.TreeViewControls;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Controls.Swf.Test.TreeViewControls
{
    [TestFixture]
    public class TreeNodeGraphicExtensionsTest
    {
        [Test]
        public void TestIsOnCheckbox()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<TreeNode>();

            treeNode.ShowCheckBox = false;

            mocks.ReplayAll();

            var pointOnCheckbox = new Point(25, 5);
            var bounds = new Rectangle(40, 0, 200, 16);

            Assert.IsFalse(TreeNodeGraphicExtensions.IsOnCheckBox(null, new Point()));
            Assert.IsFalse(treeNode.IsOnCheckBox(pointOnCheckbox));

            // Change node expectations to have a checkbox
            mocks.BackToRecord(treeNode, BackToRecordOptions.PropertyBehavior);
            treeNode.ShowCheckBox = true;
            Expect.Call(treeNode.Bounds).Return(bounds);
            Expect.Call(((ITreeNode) treeNode).Bounds).Return(bounds);
            treeNode.Replay();

            Assert.IsTrue(treeNode.IsOnCheckBox(pointOnCheckbox));
        }

        [Test]
        public void TestIsOnExpandButton()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<TreeNode>();

            treeNode.HasChildren = true;
            Expect.Call(treeNode.TreeView).Return(new TreeView());

            mocks.ReplayAll();

            var pointOnExpandButton = new Point(5, 5);
            var bounds = new Rectangle(40, 0, 200, 16);

            Assert.IsFalse(TreeNodeGraphicExtensions.IsOnExpandButton(null, new Point()));
            Assert.IsFalse(treeNode.IsOnExpandButton(pointOnExpandButton));

            // Change node expectations to have a checkbox
            mocks.BackToRecord(treeNode, BackToRecordOptions.PropertyBehavior);

            treeNode.HasChildren = true;
            Expect.Call(treeNode.Bounds).Return(bounds);
            Expect.Call(((ITreeNode) treeNode).Bounds).Return(bounds);
            treeNode.Replay();

            Assert.IsTrue(treeNode.IsOnExpandButton(pointOnExpandButton));
        }
    }
}