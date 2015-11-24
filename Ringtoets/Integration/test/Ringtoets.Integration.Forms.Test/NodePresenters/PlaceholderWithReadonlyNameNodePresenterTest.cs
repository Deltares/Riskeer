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
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PlaceholderWithReadonlyName>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidNodeData_UpdateTreeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new PlaceholderWithReadonlyName("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

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
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new InputPlaceholder("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

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
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new OutputPlaceholder("test");

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

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
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRenamceTo_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNodeTo(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnEmptyEnumerable()
        {
            // Setup
            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();

            // Call
            IEnumerable children = nodePresenter.GetChildNodeObjects(null, null);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void GetContextMenu_PlaceholderWithReadonlyName_ReturnsContextMenuWithItems()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();
            var placeholderData = new PlaceholderWithReadonlyName("test");

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(0, menu.Items.Count);
        }

        [Test]
        public void GetContextMenu_InputPlaceHolder_ReturnsContextMenuWithItems()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();
            var placeholderData = new InputPlaceholder("test");

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(8, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open_ToolTip, RingtoetsCommonFormsResources.OpenIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 2, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Import, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Import_ToolTip, RingtoetsCommonFormsResources.ImportIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 5, RingtoetsCommonFormsResources.FailureMechanism_Export, RingtoetsCommonFormsResources.FailureMechanism_Export_ToolTip, RingtoetsCommonFormsResources.ExportIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 7, RingtoetsCommonFormsResources.FailureMechanism_Properties, RingtoetsCommonFormsResources.FailureMechanism_Properties_ToolTip, RingtoetsCommonFormsResources.PropertiesIcon);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[1], menu.Items[3], menu.Items[6] }, typeof(ToolStripSeparator));
        }

        [Test]
        public void GetContextMenu_OutputPlaceHolder_ReturnsContextMenuWithItems()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();
            var placeholderData = new OutputPlaceholder("test");

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(7, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Open_ToolTip, RingtoetsCommonFormsResources.OpenIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 2, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, RingtoetsCommonFormsResources.FailureMechanism_Export, RingtoetsCommonFormsResources.FailureMechanism_Export_ToolTip, RingtoetsCommonFormsResources.ExportIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6, RingtoetsCommonFormsResources.FailureMechanism_Properties, RingtoetsCommonFormsResources.FailureMechanism_Properties_ToolTip, RingtoetsCommonFormsResources.PropertiesIcon);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[1], menu.Items[3], menu.Items[5] }, typeof(ToolStripSeparator));
        }

        [Test]
        public void GetContextMenu_OutputPlaceholderShowPropertiesClickedWithHandler_CallsShowProperties()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.ShowProperties());

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(commandHandlerMock);
            var placeholderData = new OutputPlaceholder("test");

            mocks.ReplayAll();

            var showPropertiesMenuItem = nodePresenter.GetContextMenu(nodeMock, placeholderData).Items[6];

            // Call
            showPropertiesMenuItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_OutputPlaceholderShowPropertiesClickedWithoutHandler_NoExceptions()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter();
            var placeholderData = new OutputPlaceholder("test");

            var showPropertiesMenuItem = nodePresenter.GetContextMenu(nodeMock, placeholderData).Items[6];

            // Call & Assert
            showPropertiesMenuItem.PerformClick();
        }
    }
}