using System;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(null, treeViewControlMock);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);
        }

        [Test]
        public void Constructor_WithoutTreeViewControl_ThrowsArgumentNullException()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<TreeNode>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(treeNodeMock, null);

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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(treeNodeMock, treeViewControlMock);

            // Assert
            Assert.DoesNotThrow(test);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDeleteItem_DependingOnCanRemoveNodeForData_ItemWithDeleteFunctionWillBeEnabled(bool canDelete)
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

            treeViewControlMock.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(canDelete);

            if (canDelete)
            {
                treeViewControlMock.Expect(tvc => tvc.RemoveNodeForData(nodeData));
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock, treeViewControlMock);

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
        public void CreateRenameItem_DependingOnCanRenameNodeForData_ItemWithDeleteFunctionWillBeEnabled(bool canRename)
        {
            // Setup
            var dataObject = new object();
            var treeNodeMock = mocks.StrictMock<TreeNode>();
            var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            treeNodeMock.Expect(tn => tn.Tag).Return(dataObject);

            treeNodeInfoMock.CanRename = (data, parentData) =>
            {
                if (data == dataObject)
                {
                    return canRename;
                }

                return !canRename;
            };

            treeViewControlMock.Expect(tvc => tvc.CanRenameNodeForData(dataObject)).Return(canRename);

            if (canRename)
            {
                treeNodeMock.Expect(tv => tv.BeginEdit());
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock, treeViewControlMock);

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
            var dataObject = "string";
            var treeNodeInfo = new TreeNodeInfo<string>();
            var childTreeNodeInfo = new TreeNodeInfo<object>();
            var treeViewControl = new TreeViewControl();

            if (hasChildren)
            {
                treeNodeInfo.ChildNodeObjects = o => new object[]
                {
                    new TreeNode
                    {
                        Tag = 10.0
                    }
                };
            }

            var factory = new TreeViewContextMenuItemFactory(new TreeNode { Tag = dataObject }, treeViewControl);

            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            treeViewControl.Data = dataObject;

            // Call
            var item = factory.CreateExpandAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Expand_all, item.Text);
            Assert.AreEqual(Resources.Expand_all_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExpandAllIcon, item.Image);
            Assert.AreEqual(hasChildren, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCollapseAllItem_DependingOnChildNodes_ItemWithCollapseFunctionWillBeEnabled(bool hasChildren)
        {
            // Setup
            var dataObject = "string";
            var treeNodeInfo = new TreeNodeInfo<string>();
            var childTreeNodeInfo = new TreeNodeInfo<object>();
            var treeViewControl = new TreeViewControl();

            if (hasChildren)
            {
                treeNodeInfo.ChildNodeObjects = o => new object[]
                {
                    new TreeNode
                    {
                        Tag = 10.0
                    }
                };
            }

            var factory = new TreeViewContextMenuItemFactory(new TreeNode { Tag = dataObject }, treeViewControl);

            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            treeViewControl.Data = dataObject;

            // Call
            var item = factory.CreateCollapseAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Collapse_all, item.Text);
            Assert.AreEqual(Resources.Collapse_all_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.CollapseAllIcon, item.Image);
            Assert.AreEqual(hasChildren, item.Enabled);

            mocks.VerifyAll();
        }
    }
}