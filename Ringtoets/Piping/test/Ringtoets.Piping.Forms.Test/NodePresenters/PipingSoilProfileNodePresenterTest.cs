using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
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
    public class PipingSoilProfileNodePresenterTest
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
            TestDelegate test = () => new PipingCalculationContextNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new PipingSoilProfileNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PipingSoilProfile>>(nodePresenter);
            Assert.IsInstanceOf<PipingSoilProfileNodePresenter>(nodePresenter);
        }

        [Test]
        public void UpdateNode_NodeWithData_InitializeNode()
        {
            // Setup
            var parentNodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataNodeMock = mockRepository.Stub<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            dataNodeMock.ForegroundColor = Color.AliceBlue;

            const string name = "<insert name here>";
            var random = new Random(22);
            double bottom = random.NextDouble();
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                } 
            };
            var soilProfile = new PipingSoilProfile(name, bottom, layers);

            var nodePresenter = new PipingSoilProfileNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentNodeMock, dataNodeMock, soilProfile);

            // Assert
            Assert.AreEqual(name, dataNodeMock.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataNodeMock.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingSoilProfileIcon, dataNodeMock.Image);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            IEnumerable<PipingSoilLayer> layers = new[]
            {
                new PipingSoilLayer(1)
                {
                    IsAquifer = true
                } 
            };

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var assessmentSection = mockRepository.Stub<PipingSoilProfile>(string.Empty, 0, layers);
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingSoilProfileNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}