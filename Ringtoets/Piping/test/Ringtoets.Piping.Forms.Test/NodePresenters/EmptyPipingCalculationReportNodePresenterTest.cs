using System;
using System.Drawing;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
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

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
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
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<EmptyPipingCalculationReport>>(nodePresenter);
            Assert.AreEqual(typeof(EmptyPipingCalculationReport), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_Always_MimickSameBehaviorAsPipingCalculationReportNodePresenterWithGreyedText()
        {
            // Setup
            var parentNode = mockRepository.StrictMock<ITreeNode>();
            var reportNode = mockRepository.Stub<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentNode, reportNode, new EmptyPipingCalculationReport());

            // Assert
            Assert.AreEqual("Berekeningsverslag", reportNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), reportNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingCalculationReportIcon, reportNode.Image);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_MimickSameBehaviorAsPipingCalculationReportNodePresenter()
        {
            // Setup
            var reportNode = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(reportNode);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var reportNode = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var isRenamingAllowed = nodePresenter.CanRenameNode(reportNode);

            // Assert
            Assert.IsFalse(isRenamingAllowed, "EmptyCalculationReport instance represents an absent report, therefore removal should be impossible.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mockRepository.ReplayAll();

            var nodePresenter = new EmptyPipingCalculationReportNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new EmptyPipingCalculationReport());

            // Assert
            mockRepository.VerifyAll();
        }
    }
}