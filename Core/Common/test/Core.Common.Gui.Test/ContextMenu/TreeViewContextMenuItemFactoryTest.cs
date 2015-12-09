using System;
using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class TreeViewContextMenuItemFactoryTest
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
            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_node, message);
            StringAssert.EndsWith("treeNode", message);
        }

        [Test]
        public void Constructor_WithTreeNode_DoesNotThrow()
        {
            // Setup
            var strictMock = mocks.StrictMock<ITreeNode>();

            mocks.ReplayAll();
            
            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(strictMock);

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
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeParentNodeMock = mocks.StrictMock<ITreeNode>();
            var treeNodePresenterMock = mocks.StrictMock<ITreeNodePresenter>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            var arg1 = new object();
            var arg2 = new object();

            treeNodeMock.Expect(tn => tn.Presenter).Return(treeNodePresenterMock);
            treeNodeMock.Expect(tn => tn.Parent).Return(treeParentNodeMock);
            treeNodeMock.Expect(tn => tn.Tag).Return(arg2);
            treeParentNodeMock.Expect(tn => tn.Tag).Return(arg1);
            treeNodePresenterMock.Expect(tnp => tnp.CanRemove(arg1, arg2)).Return(canDelete);

            if (canDelete)
            {
                treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
                treeViewMock.Expect(tv => tv.TryDeleteNodeData(treeNodeMock));
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock);

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
        public void CreateExpandAllItem_DependingOnChildNodes_ItemWithExpandFunctionWillBeEnabled(bool hasChildren)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            if (hasChildren)
            {
                treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
                treeViewMock.Expect(tv => tv.ExpandAll(treeNodeMock));
            }

            var children = new List<ITreeNode>();

            if (hasChildren)
            {
                children.Add(mocks.StrictMock<ITreeNode>());
            }

            treeNodeMock.Expect(tn => tn.Nodes).Return(children);
            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock);

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
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            if (hasChildren)
            {
                treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
                treeViewMock.Expect(tv => tv.CollapseAll(treeNodeMock));
            }
            var children = new List<ITreeNode>();

            if (hasChildren)
            {
                children.Add(mocks.StrictMock<ITreeNode>());
            }

            treeNodeMock.Expect(tn => tn.Nodes).Return(children);

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock);

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