using System;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingInputContextNodePresenterTest
    {
        private MockRepository mockRepository;
        private IContextMenuBuilderProvider contextMenuBuilderProviderMock;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingInputContextNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingInputContextNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PipingInputContext>>(nodePresenter);
            Assert.AreEqual(typeof(PipingInputContext), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
        }

        [Test]
        public void UpdateNode_WithInputParametersData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var currentNode = mocks.Stub<ITreeNode>();
            currentNode.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();

            var nodeData = new PipingInputContext();

            var nodePresenter = new PipingInputContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentNode, currentNode, nodeData);

            // Assert
            Assert.AreEqual("Invoer", currentNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), currentNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingInputIcon, currentNode.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var currentNode = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingInputContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenameAllowed = nodePresenter.CanRenameNode(currentNode);

            // Assert
            Assert.IsFalse(isRenameAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var currentNode = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingInputContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenameAllowed = nodePresenter.CanRenameNodeTo(currentNode, "new name");

            // Assert
            Assert.IsFalse(isRenameAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var currentNode = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodeData = new PipingInputContext();

            var nodePresenter = new PipingInputContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRemoveAllowed = nodePresenter.CanRemove(currentNode, nodeData);

            // Assert
            Assert.IsFalse(isRemoveAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void GetContextMenu_MenuBuilderProvider_ReturnsFourItems()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var nodePresenter = new PipingInputContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, nodeMock));

            mockRepository.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new PipingInputContext());

            // Assert
            Assert.AreEqual(4, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, CommonGuiResources.Import, CommonGuiResources.Import_ToolTip, CommonGuiResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(result, 1, CommonGuiResources.Export, CommonGuiResources.Export_ToolTip, CommonGuiResources.ExportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(result, 3, CommonGuiResources.Properties, CommonGuiResources.Properties_ToolTip, CommonGuiResources.PropertiesIcon, false);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[2]);
        }
    }
}