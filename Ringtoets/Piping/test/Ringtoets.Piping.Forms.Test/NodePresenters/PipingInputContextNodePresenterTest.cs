using System.Drawing;

using Core.Common.Controls;
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
    }
}