using Core.Common.Controls;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
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
            Assert.IsInstanceOf<PipingNodePresenterBase<RingtoetsPipingSurfaceLine>>(nodePresenter);
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
            var surfaceLine = new RingtoetsPipingSurfaceLine { Name = name };

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