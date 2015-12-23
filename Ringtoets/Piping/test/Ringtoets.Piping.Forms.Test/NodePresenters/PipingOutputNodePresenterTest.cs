using System;
using System.Drawing;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.Test.Helper;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingOutputNodePresenterTest 
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
            TestDelegate test = () => new PipingOutputNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingOutput), nodePresenter.NodeTagType);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string outputName = "Piping resultaat";

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var pipingNode = mockRepository.Stub<ITreeNode>();
            mockRepository.ReplayAll();

            pipingNode.ForegroundColor = Color.AliceBlue;

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            var project = PipingOutputCreator.Create();

            // Call
            nodePresenter.UpdateNode(null, pipingNode, project);

            // Assert
            Assert.AreEqual(outputName, pipingNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), pipingNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingOutputIcon, pipingNode.Image);
            mockRepository.ReplayAll();
        }

        [Test]
        public void CanRemove_NotPipingOutput_ThrowsInvalidCastException()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var dataMock = mockRepository.StrictMock<object>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate test = () => nodePresenter.CanRemove(null, dataMock);

            // Assert
            Assert.Throws<InvalidCastException>(test);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_PipingOutput_ReturnsTrue()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var result = nodePresenter.CanRemove(null, new TestPipingOutput());

            // Assert
            Assert.IsFalse(result);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new TestPipingOutput());

            // Assert
            mockRepository.VerifyAll();
        }
    }
}