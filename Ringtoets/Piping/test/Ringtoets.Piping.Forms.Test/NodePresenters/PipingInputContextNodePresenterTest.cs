using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingInputContextNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingInputContextNodePresenter();

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

            var nodePresenter = new PipingInputContextNodePresenter();

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

            var nodePresenter = new PipingInputContextNodePresenter();

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

            var nodePresenter = new PipingInputContextNodePresenter();

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

            var nodePresenter = new PipingInputContextNodePresenter();

            // Call
            var isRemoveAllowed = nodePresenter.CanRemove(currentNode, nodeData);

            // Assert
            Assert.IsFalse(isRemoveAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void GetContextMenu_NoMenuBuilderProvider_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new PipingInputContextNodePresenter();

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new PipingInputContext());

            // Assert
            Assert.IsNull(result);

            mocks.ReplayAll();
        }

        [Test]
        public void GetContextMenu_MenuBuilderProvider_ReturnsFourItems()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new PipingInputContextNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock)
            };

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new PipingInputContext());

            // Assert
            Assert.AreEqual(4, result.Items.Count);

            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export, result.Items[0].Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export_ToolTip, result.Items[0].ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExportIcon, result.Items[0].Image);

            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Import, result.Items[1].Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Import_ToolTip, result.Items[1].ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ImportIcon, result.Items[1].Image);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[2]);

            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Properties, result.Items[3].Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Properties_ToolTip, result.Items[3].ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.PropertiesIcon, result.Items[3].Image);
        }
    }
}