using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtils;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingSoilProfileCollectionNodePresenterTest
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
            TestDelegate test = () => new PipingSoilProfileCollectionNodePresenter(null);

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
            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(IEnumerable<PipingSoilProfile>), nodePresenter.NodeTagType);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithEmptyCollection_InitializeNodeWithGreyedText()
        {
            // Setup
            var soilProfileCollectionNodeStub = mockRepository.Stub<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<PipingSoilProfile> soilProfilesCollection = new PipingSoilProfile[0];

            // Call
            nodePresenter.UpdateNode(null, soilProfileCollectionNodeStub, soilProfilesCollection);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.PipingSoilProfilesCollection_DisplayName, soilProfileCollectionNodeStub.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), soilProfileCollectionNodeStub.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FolderIcon, soilProfileCollectionNodeStub.Image);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var soilProfileCollectionNodeStub = mockRepository.Stub<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<PipingSoilProfile> soilProfilesCollection = new []
            {
                new TestPipingSoilProfile()
            };

            // Call
            nodePresenter.UpdateNode(null, soilProfileCollectionNodeStub, soilProfilesCollection);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.PipingSoilProfilesCollection_DisplayName, soilProfileCollectionNodeStub.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), soilProfileCollectionNodeStub.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FolderIcon, soilProfileCollectionNodeStub.Image);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnCollection()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<object> soilProfilesCollection = new []
            {
                new TestPipingSoilProfile(),
                new TestPipingSoilProfile()
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(soilProfilesCollection);

            // Assert
            CollectionAssert.AreEqual(soilProfilesCollection, children);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mockRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);
            mockRepository.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, new PipingSoilProfile[0]);

            // Assert
            mockRepository.VerifyAll();
        }
    }
}