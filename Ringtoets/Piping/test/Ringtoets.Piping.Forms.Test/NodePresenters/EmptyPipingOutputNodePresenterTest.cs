using System.Drawing;

using Core.Common.Controls;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class EmptyPipingOutputNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new EmptyPipingOutputNodePresenter();

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<EmptyPipingOutput>>(nodePresenter);
            Assert.AreEqual(typeof(EmptyPipingOutput), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
        }

        [Test]
        public void UpdateNode_Always_InitializeNodeSimilarlyToPipingOutputNodePresenterWithGreyedText()
        {
            // Setup
            var mocks = new MockRepository();
            var parentTreeNode = mocks.StrictMock<ITreeNode>();
            var outputTreeNode = mocks.Stub<ITreeNode>();
            var expectedOutputTreeNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentTreeNode, outputTreeNode, new EmptyPipingOutput());

            // Assert
            new PipingOutputNodePresenter().UpdateNode(parentTreeNode, expectedOutputTreeNode, new TestPipingOutput());
            Assert.AreEqual(expectedOutputTreeNode.Text, outputTreeNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), outputTreeNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(expectedOutputTreeNode.Image, outputTreeNode.Image);

            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_MimickBehaviorOfPipingOutputNodePresenter()
        {
            // Setup
            var mocks = new MockRepository();
            var outputTreeNode = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter();

            var referenceNodePresenter = new PipingOutputNodePresenter();

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(outputTreeNode);

            // Assert
            var expectedValue = referenceNodePresenter.CanRenameNode(outputTreeNode);
            Assert.AreEqual(expectedValue, isRenamingAllowed);

            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var parentDataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter();

            // Call
            var isRemovalAllowed = nodePresenter.CanRemove(parentDataMock, new EmptyPipingOutput());

            // Assert
            Assert.IsFalse(isRemovalAllowed, "EmptyPipingOutput represents no output, therefore there's nothing to remove.");

            mocks.VerifyAll();
        }
    }
}