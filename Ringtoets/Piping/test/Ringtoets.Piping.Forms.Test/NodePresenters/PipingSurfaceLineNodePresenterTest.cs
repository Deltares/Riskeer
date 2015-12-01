using System;
using System.Drawing;

using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.TestUtils;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSurfaceLineNodePresenterTest
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
            TestDelegate test = () => new PipingCalculationContextNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Call
            var nodePresenter = new PipingSurfaceLineNodePresenter(contextMenuBuilderProviderMock);

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

            var nodePresenter = new PipingSurfaceLineNodePresenter(contextMenuBuilderProviderMock);

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