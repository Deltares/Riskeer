using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Test.Helper;

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
            mocks.ReplayAll();

            var nodePresenter = new PipingOutputNodePresenter();

            var project = PipingOutputCreator.Create();

            // Call
            nodePresenter.UpdateNode(null, pipingNode, project);

            // Assert
            Assert.AreEqual(outputName, pipingNode.Text);
            Assert.AreEqual(16, pipingNode.Image.Height);
            Assert.AreEqual(16, pipingNode.Image.Width);
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
    }
}