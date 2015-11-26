using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Placeholder;
using Ringtoets.Integration.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

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
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();

            // Call
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidNodeData_UpdateTreeNode()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);

            // Call
            IEnumerable children = nodePresenter.GetChildNodeObjects(null, null);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void GetContextMenu_PlaceholderWithReadonlyName_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            mocks.ReplayAll();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new PlaceholderWithReadonlyName("test");

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(0, menu.Items.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_InputPlaceHolder_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new InputPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(6, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open_ToolTip, RingtoetsCommonFormsResources.OpenIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 2, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 5, RingtoetsCommonFormsResources.FailureMechanism_Properties, RingtoetsCommonFormsResources.FailureMechanism_Properties_ToolTip, RingtoetsCommonFormsResources.PropertiesIcon);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[1], menu.Items[3], menu.Items[4] }, typeof(ToolStripSeparator));
            
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_OutputPlaceHolder_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new OutputPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(6, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open_ToolTip, RingtoetsCommonFormsResources.OpenIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 2, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 5, RingtoetsCommonFormsResources.FailureMechanism_Properties, RingtoetsCommonFormsResources.FailureMechanism_Properties_ToolTip, RingtoetsCommonFormsResources.PropertiesIcon);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[1], menu.Items[3], menu.Items[4] }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_OutputPlaceholderShowPropertiesClickedWithHandler_CallsShowProperties()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();

            commandHandlerMock.Expect(ch => ch.ShowProperties());
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider, commandHandlerMock);
            var placeholderData = new OutputPlaceholder("test");

            mocks.ReplayAll();

            var showPropertiesMenuItem = nodePresenter.GetContextMenu(nodeMock, placeholderData).Items[5];

            // Call
            showPropertiesMenuItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_OutputPlaceholderShowPropertiesClickedWithoutHandler_NoExceptions()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();

            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new OutputPlaceholder("test");

            mocks.ReplayAll();

            var showPropertiesMenuItem = nodePresenter.GetContextMenu(nodeMock, placeholderData).Items[5];

            // Call & Assert
            showPropertiesMenuItem.PerformClick();

            mocks.VerifyAll();
        }
    }
}