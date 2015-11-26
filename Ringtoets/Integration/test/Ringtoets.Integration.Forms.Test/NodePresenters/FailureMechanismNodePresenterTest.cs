﻿using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CommonResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class FailureMechanismNodePresenterTest
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
            // Call
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<FailureMechanismPlaceholder>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidOutputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var nodeToUpdate = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var dataObject = new FailureMechanismPlaceholder("test");

            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);

            // Call
            nodePresenter.UpdateNode(parentNode, nodeToUpdate, dataObject);

            // Assert
            Assert.AreEqual(dataObject.Name, nodeToUpdate.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), nodeToUpdate.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FailureMechanismIcon, nodeToUpdate.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void CanRenameTo_Always_ReturnFalse()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);

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
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(null, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnInputAndOutput()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);
            var failureMechanism = new FailureMechanismPlaceholder("test");

            // Call
            object[] children = nodePresenter.GetChildNodeObjects(failureMechanism, null).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);
            var inputFolder = (CategoryTreeFolder)children[0];
            Assert.AreEqual("Invoer", inputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputFolder.Category);
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.SectionDivisions,
                failureMechanism.Locations,
                failureMechanism.BoundaryConditions
            }, inputFolder.Contents);

            var outputFolder = (CategoryTreeFolder)children[1];
            Assert.AreEqual("Uitvoer", outputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputFolder.Category);
            Assert.AreEqual(new[]{failureMechanism.AssessmentResult}, outputFolder.Contents);
        }

        [Test]
        public void GetContextMenu_Always_ReturnsContextMenuWithItems()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);
            var failureMechanism = new FailureMechanismPlaceholder("test");

            mocks.ReplayAll();

            // Call
            var menu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);

            // Assert
            Assert.AreEqual(8, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.Calculate_all, RingtoetsCommonFormsResources.Calculate_all_ToolTip, RingtoetsCommonFormsResources.CalculateAllIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 1, RingtoetsCommonFormsResources.Clear_all_output, RingtoetsCommonFormsResources.Clear_all_output_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3, RingtoetsCommonFormsResources.FailureMechanism_Expand_all, RingtoetsCommonFormsResources.FailureMechanism_Expand_all_ToolTip, RingtoetsCommonFormsResources.ExpandAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, RingtoetsCommonFormsResources.FailureMechanism_Collapse_all, RingtoetsCommonFormsResources.FailureMechanism_Collapse_all_ToolTip, RingtoetsCommonFormsResources.CollapseAllIcon);

            CollectionAssert.AllItemsAreInstancesOfType(new []{menu.Items[2], menu.Items[5],menu.Items[6]}, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ShowPropertiesClickedWithHandler_CallsShowProperties()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var commandHandlerMock = mocks.StrictMock<IGuiCommandHandler>();
            commandHandlerMock.Expect(ch => ch.ShowProperties());

            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider, commandHandlerMock);
            var failureMechanism = new FailureMechanismPlaceholder("test");

            mocks.ReplayAll();

            var showPropertiesMenuItem = nodePresenter.GetContextMenu(nodeMock, failureMechanism).Items[7];

            // Call
            showPropertiesMenuItem.PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ShowPropertiesClickedWithoutHandler_NoExceptions()
        {
            // Setup
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var contextMenuProvider = mocks.StrictMock<IContextMenuProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(new object())).IgnoreArguments().Return(new ContextMenuStrip());

            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);
            var failureMechanism = new FailureMechanismPlaceholder("test");

            mocks.ReplayAll();

            var showPropertiesMenuItem = nodePresenter.GetContextMenu(nodeMock, failureMechanism).Items[7];

            // Call & Assert
            showPropertiesMenuItem.PerformClick();

            mocks.VerifyAll();
        }
    }
}