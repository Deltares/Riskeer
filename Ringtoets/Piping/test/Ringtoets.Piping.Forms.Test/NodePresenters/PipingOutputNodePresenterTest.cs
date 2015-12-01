using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Test.Helper;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingOutputNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingOutputNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingOutput), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string outputName = "Piping resultaat";

            var mocks = new MockRepository();
            var pipingNode = mocks.Stub<ITreeNode>();
            pipingNode.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter();

            var project = PipingOutputCreator.Create();

            // Call
            nodePresenter.UpdateNode(null, pipingNode, project);

            // Assert
            Assert.AreEqual(outputName, pipingNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), pipingNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingOutputIcon, pipingNode.Image);
        }

        [Test]
        public void GetContextMenu_PipingOutput_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, new TestPipingOutput());

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_NotPipingOutput_ThrowsInvalidCastException()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter();

            // Call
            TestDelegate test = () => nodePresenter.CanRemove(null, dataMock);

            // Assert
            Assert.Throws<InvalidCastException>(test);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_PipingOutput_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            mocks.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter();

            // Call
            var result = nodePresenter.CanRemove(null, new TestPipingOutput());

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_NullObject_ThrowsNullReferenceException()
        {
            // Setup
            var nodePresenter = new PipingOutputNodePresenter();

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, null);

            // Assert
            Assert.Throws<NullReferenceException>(removeAction);
        }

        [Test]
        public void RemoveNodeData_Object_ThrowsInvalidCastException()
        {
            // Setup
            var nodePresenter = new PipingOutputNodePresenter();

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, new object());

            // Assert
            Assert.Throws<InvalidCastException>(removeAction);
        }

        [Test]
        public void RemoveNodeData_NullParent_ThrowsNullReferenceException()
        {
            // Setup
            var nodePresenter = new PipingOutputNodePresenter();

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, new TestPipingOutput());

            // Assert
            Assert.Throws<NullReferenceException>(removeAction);
        }

        [Test]
        public void RemoveNodeData_WithParentContainingOutput_OutputCleared()
        {
            // Setup
            var nodePresenter = new PipingOutputNodePresenter();

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
        }

        [Test]
        public void GetContextMenu_NoMenuBuilderProvider_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new PipingOutputNodePresenter();

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new TestPipingOutput());

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
            var nodePresenter = new PipingOutputNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock)
            };

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new TestPipingOutput());

            // Assert
            Assert.AreEqual(5, result.Items.Count);

            Assert.AreEqual(Properties.Resources.Clear_output, result.Items[0].Text);
            Assert.IsNull(result.Items[0].ToolTipText);
            TestHelper.AssertImagesAreEqual(Common.Forms.Properties.Resources.ClearIcon, result.Items[0].Image);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[1]);

            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export, result.Items[2].Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export_ToolTip, result.Items[2].ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExportIcon, result.Items[2].Image);

            Assert.IsInstanceOf<ToolStripSeparator>(result.Items[3]);

            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Properties, result.Items[4].Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Properties_ToolTip, result.Items[4].ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.PropertiesIcon, result.Items[4].Image);
        }
        [Test]
        public void GetContextMenu_ClearItemClicked_OutputCleared()
        {
            // Setup
            var mocks = new MockRepository();
            var treeMock = mocks.StrictMock<ITreeView>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            treeMock.Expect(t => t.TryDeleteSelectedNodeData());
            nodeMock.Expect(n => n.TreeView).Return(treeMock);

            var nodePresenter = new PipingOutputNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock)
            };

            mocks.ReplayAll();

            var contextMenu = nodePresenter.GetContextMenu(nodeMock, new TestPipingOutput());

            // Call
            contextMenu.Items[0].PerformClick();
            
            // Assert
            mocks.VerifyAll();
        }
    }
}