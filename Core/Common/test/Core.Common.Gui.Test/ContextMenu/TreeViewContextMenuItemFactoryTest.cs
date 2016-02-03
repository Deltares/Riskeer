using System;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class TreeViewContextMenuItemFactoryTest : NUnitFormTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutTreeNode_ThrowsArgumentNullException()
        {
            // Setup
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(null, treeNodeInfoMock, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);
        }

        [Test]
        public void Constructor_WithoutTreeNodeInfo_ThrowsArgumentNullException()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(treeNodeMock, null, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node_info, message);
            StringAssert.EndsWith("treeNodeInfo", message);
        }

        [Test]
        public void Constructor_WithoutTreeViewControl_ThrowsArgumentNullException()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(treeNodeMock, treeNodeInfoMock, null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_view_control, message);
            StringAssert.EndsWith("treeViewControl", message);
        }

        [Test]
        public void Constructor_WithTreeNode_DoesNotThrow()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(treeNodeMock, treeNodeInfoMock, treeViewControlMock);

            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDeleteItem_DependingOnCanDelete_ItemWithDeleteFunctionWillBeEnabled(bool canDelete)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeParentNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var parentNodeData = new object();
            var nodeData = new object();

            treeNodeMock.Stub(tn => tn.Text).Return("");
            treeNodeMock.Stub(tn => tn.Parent).Return(treeParentNodeMock);
            treeNodeMock.Stub(tn => tn.Tag).Return(nodeData);
            treeParentNodeMock.Stub(tn => tn.Tag).Return(parentNodeData);

            treeNodeInfoMock.CanRemove = (nd, pnd) =>
            {
                if (nd == nodeData && pnd == parentNodeData)
                {
                    return canDelete;
                }

                return !canDelete;
            };

            if (canDelete)
            {
                treeViewControlMock.Expect(tvc => tvc.DeleteNode(treeNodeMock, treeNodeInfoMock));
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock, treeNodeInfoMock, treeViewControlMock);

            // Call
            var item = factory.CreateDeleteItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Delete, item.Text);
            Assert.AreEqual(Resources.Delete_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.DeleteIcon, item.Image);
            Assert.AreEqual(canDelete, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateRenameItem_DependingOnCanRename_ItemWithDeleteFunctionWillBeEnabled(bool canRename)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            treeNodeInfoMock.CanRename = tn =>
            {
                if (tn == treeNodeMock)
                {
                    return canRename;
                }

                return !canRename;
            };

            if (canRename)
            {
                treeNodeMock.Expect(tv => tv.BeginEdit());
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock, treeNodeInfoMock, treeViewControlMock);

            // Call
            var item = factory.CreateRenameItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Rename, item.Text);
            Assert.AreEqual(Resources.Rename_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.RenameIcon, item.Image);
            Assert.AreEqual(canRename, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExpandAllItem_DependingOnChildNodes_ItemWithExpandFunctionWillBeEnabled(bool hasChildren)
        {
            // Setup
            var treeNode = new TreeNode();
            var treeView = new TreeView();
            var treeNodeInfo = new TreeNodeInfo();
            var treeViewControl = new TreeViewControl();

            treeView.Nodes.Add(treeNode);

            if (hasChildren)
            {
                treeNode.Nodes.Add(new TreeNode());
                treeNodeInfo.ChildNodeObjects = o => new object[] { new TreeNode() }; 
            }

            var factory = new TreeViewContextMenuItemFactory(treeNode, treeNodeInfo, treeViewControl);

            // Precondition
            Assert.IsFalse(treeNode.IsExpanded);

            // Call
            var item = factory.CreateExpandAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Expand_all, item.Text);
            Assert.AreEqual(Resources.Expand_all_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExpandAllIcon, item.Image);
            Assert.AreEqual(hasChildren, item.Enabled);
            Assert.AreEqual(hasChildren, treeNode.IsExpanded);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCollapseAllItem_DependingOnChildNodes_ItemWithCollapseFunctionWillBeEnabled(bool hasChildren)
        {
            // Setup
            var treeNode = new TreeNode();
            var treeView = new TreeView();
            var treeNodeInfo = new TreeNodeInfo();
            var treeViewControl = new TreeViewControl();

            treeView.Nodes.Add(treeNode);

            if (hasChildren)
            {
                treeNode.Expand();
                treeNode.Nodes.Add(new TreeNode());
                treeNodeInfo.ChildNodeObjects = o => new object[] { new TreeNode() };

                // Precondition
                Assert.IsTrue(treeNode.IsExpanded);
            }

            var factory = new TreeViewContextMenuItemFactory(treeNode, treeNodeInfo, treeViewControl);

            // Call
            var item = factory.CreateCollapseAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Collapse_all, item.Text);
            Assert.AreEqual(Resources.Collapse_all_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.CollapseAllIcon, item.Image);
            Assert.AreEqual(hasChildren, item.Enabled);
            Assert.IsFalse(treeNode.IsExpanded);

            mocks.VerifyAll();
        }
    }
}