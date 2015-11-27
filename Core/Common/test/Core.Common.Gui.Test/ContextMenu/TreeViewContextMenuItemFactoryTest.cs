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
        public void CreateExpandAllItem_Always_ItemWithExpandFunction()
        {
            // Setup
            var treeNodeMock = mocks.StrictMock<ITreeNode>();
            var treeViewMock = mocks.StrictMock<ITreeView>();
            treeNodeMock.Expect(tn => tn.TreeView).Return(treeViewMock);
            treeViewMock.Expect(tv => tv.ExpandAll(treeNodeMock));

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory();

            // Call
            var item = factory.CreateExpandAllItem(treeNodeMock);
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

            var factory = new TreeViewContextMenuItemFactory();

            // Call
            var item = factory.CreateCollapseAllItem(treeNodeMock);
            item.PerformClick();

            // Assert
            Assert.AreEqual(item.Text, Resources.Collapse_all);
            Assert.AreEqual(item.ToolTipText, Resources.Collapse_all_ToolTip);
            TestHelper.AssertImagesAreEqual(item.Image, Resources.CollapseAllIcon);

            mocks.VerifyAll();
        }
    }
}