using System;
using Core.Common.Controls;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Tests.ContextMenu
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
        public void CreateExpandAllItem_Always_ItemWithExpandFunction()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
            treeViewMock.Expect(tv => tv.ExpandAll(treeNodeMock));

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock);

            // Call
            var item = factory.CreateExpandAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(item.Text, Resources.Expand_all);
            Assert.AreEqual(item.ToolTipText, Resources.Expand_all_ToolTip);
            TestHelper.AssertImagesAreEqual(item.Image, Resources.ExpandAllIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void CreateCollapseAllItem_Always_ItemWithCollapseFunction()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
            treeViewMock.Expect(tv => tv.CollapseAll(treeNodeMock));

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(treeNodeMock);

            // Call
            var item = factory.CreateCollapseAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(item.Text, Resources.Collapse_all);
            Assert.AreEqual(item.ToolTipText, Resources.Collapse_all_ToolTip);
            TestHelper.AssertImagesAreEqual(item.Image, Resources.CollapseAllIcon);

            mocks.VerifyAll();
        }
    }
}