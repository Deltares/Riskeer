using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.NodePresenters
{
    [TestFixture]
    public class CategoryTreeFolderNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new CategoryTreeFolderNodePresenter();

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<CategoryTreeFolder>>(nodePresenter);
            Assert.AreEqual(typeof(CategoryTreeFolder), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
        }

        [Test]
        [TestCase(TreeFolderCategory.General)]
        [TestCase(TreeFolderCategory.Input)]
        [TestCase(TreeFolderCategory.Output)]
        public void UpdateNode_ForCategory_InitializeNode(TreeFolderCategory category)
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var currentNode = mocks.Stub<ITreeNode>();
            currentNode.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter();
            var folder = new CategoryTreeFolder("Cool name", new object[0], category);

            // Call
            nodePresenter.UpdateNode(parentNode, currentNode, folder);

            // Assert
            Assert.AreEqual(folder.Name, currentNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), currentNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(GetExpectedIconForCategory(category), currentNode.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenamedNode_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new CategoryTreeFolderNodePresenter();

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRenamedNodeTo_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new CategoryTreeFolderNodePresenter();

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNodeTo(null, "");

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new CategoryTreeFolderNodePresenter();

            // Call
            var isRenamingAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void GetChildNodeObjects_FolderHasContents_ReturnContents()
        {
            // Setup
            var mocks = new MockRepository();
            var node = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var folder = new CategoryTreeFolder("", new[]
            {
                new object(),
                new object()
            });
            
            var nodePresenter = new CategoryTreeFolderNodePresenter();

            // Call
            IEnumerable children = nodePresenter.GetChildNodeObjects(folder);

            // Assert
            CollectionAssert.AreEqual(folder.Contents, children);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ContextMenuBuilderProviderSet_HaveImportSurfaceLinesItemInContextMenu()
        {
            // Setup
            var folder = new CategoryTreeFolder("", new object[0]);
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var menuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            menuBuilderProviderMock.Expect(mbp => mbp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(null, nodeMock));

            mocks.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter
            {
                ContextMenuBuilderProvider = menuBuilderProviderMock
            };

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, folder);

            // Assert
            Assert.AreEqual(2, returnedContextMenu.Items.Count);
            var expandAllItem = returnedContextMenu.Items[0];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Expand_all, expandAllItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Expand_all_ToolTip, expandAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExpandAllIcon, expandAllItem.Image);
            Assert.IsTrue(expandAllItem.Enabled);

            var collapseAllItem = returnedContextMenu.Items[1];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Collapse_all, collapseAllItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Collapse_all_ToolTip, collapseAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.CollapseAllIcon, collapseAllItem.Image);
            Assert.IsTrue(collapseAllItem.Enabled);

            mocks.VerifyAll();
        }

        private Image GetExpectedIconForCategory(TreeFolderCategory category)
        {
            switch (category)
            {
                case TreeFolderCategory.General:
                    return RingtoetsFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsFormsResources.OutputFolderIcon;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}