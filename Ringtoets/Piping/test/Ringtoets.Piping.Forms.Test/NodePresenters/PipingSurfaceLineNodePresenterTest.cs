using System.Drawing;

using Core.Common.Controls;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

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
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<RingtoetsPipingSurfaceLine>>(nodePresenter);
        }

        [Test]
        public void UpdateNode_NodeWithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNodeMock = mocks.StrictMock<ITreeNode>();
            var dataNodeMock = mocks.Stub<ITreeNode>();
            dataNodeMock.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();

            const string name = "<insert name here>";
            var surfaceLine = new RingtoetsPipingSurfaceLine { Name = name };

            var nodePresenter = new PipingSurfaceLineNodePresenter();

            // Call
            nodePresenter.UpdateNode(parentNodeMock, dataNodeMock, surfaceLine);

            // Assert
            Assert.AreEqual(name, dataNodeMock.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataNodeMock.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingSurfaceLineIcon, dataNodeMock.Image);
            mocks.VerifyAll();
        }
    }
}