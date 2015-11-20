using System;
using System.Collections;
using System.Drawing;

using Core.Common.Controls;
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
            mocks.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter();
            var folder = new CategoryTreeFolder("Cool name", new object[0], category);

            // Call
            nodePresenter.UpdateNode(parentNode, currentNode, folder);

            // Assert
            Assert.AreEqual(folder.Name, currentNode.Text);
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
            IEnumerable children = nodePresenter.GetChildNodeObjects(folder, node);

            // Assert
            CollectionAssert.AreEqual(folder.Contents, children);
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