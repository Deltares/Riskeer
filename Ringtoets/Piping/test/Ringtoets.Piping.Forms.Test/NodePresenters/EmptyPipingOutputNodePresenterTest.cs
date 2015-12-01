using System;
using System.Drawing;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class EmptyPipingOutputNodePresenterTest
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
            TestDelegate test = () => new EmptyPipingOutputNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Core.Common.Gui.Properties.Resources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Call
            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

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

            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentTreeNode, outputTreeNode, new EmptyPipingOutput());

            // Assert
            new PipingOutputNodePresenter(contextMenuBuilderProviderMock).UpdateNode(parentTreeNode, expectedOutputTreeNode, new TestPipingOutput());
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
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            var referenceNodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

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

            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRemovalAllowed = nodePresenter.CanRemove(parentDataMock, new EmptyPipingOutput());

            // Assert
            Assert.IsFalse(isRemovalAllowed, "EmptyPipingOutput represents no output, therefore there's nothing to remove.");

            mocks.VerifyAll();
        }
    }
}