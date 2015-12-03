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

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
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
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<EmptyPipingOutput>>(nodePresenter);
            Assert.AreEqual(typeof(EmptyPipingOutput), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_Always_InitializeNodeSimilarlyToPipingOutputNodePresenterWithGreyedText()
        {
            // Setup
            var parentTreeNode = mockRepository.StrictMock<ITreeNode>();
            var outputTreeNode = mockRepository.Stub<ITreeNode>();
            var expectedOutputTreeNode = mockRepository.Stub<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentTreeNode, outputTreeNode, new EmptyPipingOutput());

            // Assert
            new PipingOutputNodePresenter(contextMenuBuilderProviderMock).UpdateNode(parentTreeNode, expectedOutputTreeNode, new TestPipingOutput());
            Assert.AreEqual(expectedOutputTreeNode.Text, outputTreeNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), outputTreeNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(expectedOutputTreeNode.Image, outputTreeNode.Image);

            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_MimickBehaviorOfPipingOutputNodePresenter()
        {
            // Setup
            var outputTreeNode = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            var referenceNodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(outputTreeNode);

            // Assert
            var expectedValue = referenceNodePresenter.CanRenameNode(outputTreeNode);
            Assert.AreEqual(expectedValue, isRenamingAllowed);

            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var parentDataMock = mockRepository.StrictMock<object>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRemovalAllowed = nodePresenter.CanRemove(parentDataMock, new EmptyPipingOutput());

            // Assert
            Assert.IsFalse(isRemovalAllowed, "EmptyPipingOutput represents no output, therefore there's nothing to remove.");

            mockRepository.VerifyAll();
        }
    }
}