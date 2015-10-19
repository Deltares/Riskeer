using DelftTools.Controls;

using NUnit.Framework;

using Rhino.Mocks;

using Wti.Data;
using Wti.Forms.NodePresenters;

namespace Wti.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSurfaceLineNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingSurfaceLineNodePresenter();

            // Assert
            Assert.IsInstanceOf<PipingNodePresenterBase<PipingSurfaceLine>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_NodeWithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNodeMock = mocks.StrictMock<ITreeNode>();
            var dataNodeMock = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            const string name = "<insert name here>";
            var surfaceLine = new PipingSurfaceLine { Name = name };

            var nodePresenter = new PipingSurfaceLineNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNodeMock, dataNodeMock, surfaceLine);

            // Assert
            Assert.AreEqual(name, dataNodeMock.Text);
            Assert.AreEqual(16, dataNodeMock.Image.Height);
            Assert.AreEqual(16, dataNodeMock.Image.Width);
            mocks.VerifyAll();
        }
    }
}