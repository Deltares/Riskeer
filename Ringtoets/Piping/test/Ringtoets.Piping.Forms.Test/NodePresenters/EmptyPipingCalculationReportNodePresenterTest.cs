using System;
using System.Drawing;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.Properties;
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
    public class EmptyPipingCalculationReportNodePresenterTest
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
            TestDelegate test = () => new EmptyPipingCalculationReportNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Call
            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<EmptyPipingCalculationReport>>(nodePresenter);
            Assert.AreEqual(typeof(EmptyPipingCalculationReport), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);
        }

        [Test]
        public void UpdateNode_Always_MimickSameBehaviorAsPipingCalculationReportNodePresenterWithGreyedText()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNode = mocks.StrictMock<ITreeNode>();
            var reportNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentNode, reportNode, new EmptyPipingCalculationReport());

            // Assert
            Assert.AreEqual("Berekeningsverslag", reportNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), reportNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingCalculationReportIcon, reportNode.Image);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_MimickSameBehaviorAsPipingCalculationReportNodePresenter()
        {
            // Setup
            var mocks = new MockRepository();
            var reportNode = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(reportNode);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var reportNode = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(reportNode);

            // Assert
            Assert.IsFalse(isRenamingAllowed, "EmptyCalculationReport instance represents an absent report, therefore removal should be impossible.");
            mocks.VerifyAll();
        }
    }
}