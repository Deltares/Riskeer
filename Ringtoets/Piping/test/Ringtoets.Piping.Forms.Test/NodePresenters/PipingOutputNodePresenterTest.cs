using System;
using System.Drawing;
using System.Linq;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils.ContextMenu;
using Core.Common.TestUtils;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
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
            Assert.IsTrue(result);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_NullObject_ThrowsNullReferenceException()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, null);

            // Assert
            Assert.Throws<NullReferenceException>(removeAction);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_Object_ThrowsInvalidCastException()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, new object());

            // Assert
            Assert.Throws<InvalidCastException>(removeAction);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_NullParent_ThrowsNullReferenceException()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, new TestPipingOutput());

            // Assert
            Assert.Throws<NullReferenceException>(removeAction);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_WithParentContainingOutput_OutputCleared()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);

            var calculation = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());

            // Call
            nodePresenter.RemoveNodeData(pipingCalculationContext, calculation.Output);

            // Assert
            Assert.IsNull(calculation.Output);
            Assert.IsFalse(calculation.HasOutput);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
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

        [Test]
        public void GetContextMenu_ClearItemClicked_OutputCleared()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var treeMock = mockRepository.StrictMock<ITreeView>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            treeMock.Expect(t => t.TryDeleteSelectedNodeData());
            nodeMock.Expect(n => n.TreeView).Return(treeMock);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilder);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter(contextMenuBuilderProviderMock);
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, new TestPipingOutput());

            // Call
            contextMenu.Items[0].PerformClick();
            
            // Assert
            mockRepository.VerifyAll();
        }
    }
}