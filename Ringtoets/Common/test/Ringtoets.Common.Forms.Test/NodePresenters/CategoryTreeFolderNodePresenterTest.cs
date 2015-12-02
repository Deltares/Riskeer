using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.NodePresenters
{
    [TestFixture]
    public class CategoryTreeFolderNodePresenterTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new CategoryTreeFolderNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<CategoryTreeFolder>>(nodePresenter);
            Assert.AreEqual(typeof(CategoryTreeFolder), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(TreeFolderCategory.General)]
        [TestCase(TreeFolderCategory.Input)]
        [TestCase(TreeFolderCategory.Output)]
        public void UpdateNode_ForCategory_InitializeNode(TreeFolderCategory category)
        {
            // Setup
            var parentNode = mockRepository.StrictMock<ITreeNode>();
            var currentNode = mockRepository.Stub<ITreeNode>();
            currentNode.ForegroundColor = Color.AliceBlue;
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);
            var folder = new CategoryTreeFolder("Cool name", new object[0], category);

            // Call
            nodePresenter.UpdateNode(parentNode, currentNode, folder);

            // Assert
            Assert.AreEqual(folder.Name, currentNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), currentNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(GetExpectedIconForCategory(category), currentNode.Image);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenamedNode_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenamedNodeTo_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNodeTo(null, "");

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_FolderHasContents_ReturnContents()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var folder = new CategoryTreeFolder("", new[]
            {
                new object(),
                new object()
            });

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);

            // Call
            IEnumerable children = nodePresenter.GetChildNodeObjects(folder);

            // Assert
            CollectionAssert.AreEqual(folder.Contents, children);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_ContextMenuBuilderProviderSet_HaveImportSurfaceLinesItemInContextMenu(bool commonItemsEnabled)
        {
            // Setup
            var folder = new CategoryTreeFolder("", new object[0]);
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mockRepository.ReplayAll();

            var nodePresenter = new CategoryTreeFolderNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, folder);

            // Assert
            mockRepository.VerifyAll();
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