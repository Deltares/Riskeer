using System;
using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtils;
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
        public void Constructor_WithTreeNode_NewInstance()
        {
            // Call
            var result = new TreeViewContextMenuItemFactory(mocks.StrictMock<ITreeNode>());

            // Assert
            Assert.IsInstanceOf<TreeViewContextMenuItemFactory>(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExpandAllItem_DependingOnChildNodes_ItemWithExpandFunctionWillBeEnabled(bool enabled)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            if (enabled)
            {
                treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
                treeViewMock.Expect(tv => tv.ExpandAll(treeNodeMock));
            }

            var children = new List<ITreeNode>();

            if (enabled)
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
            Assert.AreEqual(enabled, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCollapseAllItem_DependingOnChildNodes_ItemWithCollapseFunctionWillBeEnabled(bool enabled)
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            if (enabled)
            {
                treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
                treeViewMock.Expect(tv => tv.CollapseAll(treeNodeMock));
            }
            var children = new List<ITreeNode>();

            if (enabled)
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
            Assert.AreEqual(enabled, item.Enabled);

            mocks.VerifyAll();
        }
    }
}