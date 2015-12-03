using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CommonResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PlaceholderWithReadonlyNameNodePresenterTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PlaceholderWithReadonlyNameNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CommonResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_ParamsSet_ExpectedValues()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();

            // Call
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidNodeData_UpdateTreeNode()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new PlaceholderWithReadonlyName("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.PlaceholderIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateNode_ValidInputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new InputPlaceholder("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateNode_ValidOutputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new OutputPlaceholder("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRenamceTo_Always_ReturnFalse()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNodeTo(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnEmptyEnumerable()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            IEnumerable children = nodePresenter.GetChildNodeObjects(null);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void GetContextMenu_OutputPlaceHolder_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new OutputPlaceholder("test"));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_InputPlaceHolder_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new InputPlaceholder("test"));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PlaceHolder_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new PlaceholderWithReadonlyName("test"));

            // Assert
            mocks.VerifyAll();
        }
    }
}