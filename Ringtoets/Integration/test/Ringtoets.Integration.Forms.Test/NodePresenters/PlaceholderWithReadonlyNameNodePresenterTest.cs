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
        public void DefaultConstructor_ExpectedValues()
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
        public void GetContextMenu_PlaceholderWithReadonlyNameNoGui_ReturnsEmptyContextMenu()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(null, nodeMock)); ;

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new PlaceholderWithReadonlyName("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(0, menu.Items.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_InputPlaceHolderNoGui_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();

            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(null, nodeMock)); ;

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new InputPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(2, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[1] }, typeof(ToolStripSeparator));
            
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_OutputPlaceHolderNoGui_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();

            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(null, nodeMock));

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new OutputPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(2, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[1] }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_InputPlaceHolderWithGui_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.Stub<ITreeNode>();
            var guiHandlerMock = mocks.DynamicMock<IGuiCommandHandler>();

            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(guiHandlerMock, nodeMock));

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new InputPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(7, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, CommonResources.Open, CommonResources.Open_ToolTip, CommonResources.OpenIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 1, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3, CommonResources.Import, CommonResources.Import_ToolTip, CommonResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, CommonResources.Export, CommonResources.Export_ToolTip, CommonResources.ExportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6, CommonResources.Properties, CommonResources.Properties_ToolTip, CommonResources.PropertiesIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2], menu.Items[5] }, typeof(ToolStripSeparator));
            
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_OutputPlaceHolderWithGui_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.Stub<ITreeNode>();
            var guiHandlerMock = mocks.DynamicMock<IGuiCommandHandler>();

            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(guiHandlerMock, nodeMock));

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new OutputPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(7, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, CommonResources.Open, CommonResources.Open_ToolTip, CommonResources.OpenIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 1, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase, RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3, CommonResources.Import, CommonResources.Import_ToolTip, CommonResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, CommonResources.Export, CommonResources.Export_ToolTip, CommonResources.ExportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6, CommonResources.Properties, CommonResources.Properties_ToolTip, CommonResources.PropertiesIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2], menu.Items[5] }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PlaceholderWithReadonlyNameWithGui_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.Stub<ITreeNode>();
            var guiHandlerMock = mocks.DynamicMock<IGuiCommandHandler>();

            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(guiHandlerMock, nodeMock));

            var nodePresenter = new PlaceholderWithReadonlyNameNodePresenter(contextMenuProvider);
            var placeholderData = new PlaceholderWithReadonlyName("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, placeholderData);

            // Assert
            Assert.AreEqual(4, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, CommonResources.Import, CommonResources.Import_ToolTip, CommonResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 1, CommonResources.Export, CommonResources.Export_ToolTip, CommonResources.ExportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3, CommonResources.Properties, CommonResources.Properties_ToolTip, CommonResources.PropertiesIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2] }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }
    }
}