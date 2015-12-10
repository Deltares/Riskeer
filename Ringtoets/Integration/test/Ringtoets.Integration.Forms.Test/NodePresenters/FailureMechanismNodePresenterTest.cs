using System;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
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
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new FailureMechanismNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CommonResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_ParamsSet_ExpectedValues()
        {
            // Call
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<FailureMechanismPlaceholder>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_ValidOutputPlaceholderData_UpdateTreeNode()
        {
            // Setup
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
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
            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var nodePresenter = new FailureMechanismNodePresenter(contextMenuProvider);
            var failureMechanism = new FailureMechanismPlaceholder("test");

            // Call
            object[] children = nodePresenter.GetChildNodeObjects(failureMechanism).OfType<object>().ToArray();

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
        public void GetContextMenu_NoGuiCommandHandler_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            var nodePresenter = new FailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new FailureMechanismPlaceholder("test"));

            // Assert
            mocks.VerifyAll();
        }
    }
}