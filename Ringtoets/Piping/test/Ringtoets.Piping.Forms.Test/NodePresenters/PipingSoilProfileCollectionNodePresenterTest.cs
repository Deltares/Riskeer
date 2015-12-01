using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.TestUtils;
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
            TestDelegate test = () => new PipingSoilProfileCollectionNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Call
            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(mockRepository.StrictMock<IContextMenuBuilderProvider>());

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(IEnumerable<PipingSoilProfile>), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithEmptyCollection_InitializeNodeWithGreyedText()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfileCollectionNodeStub = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(contextMenuBuilderProviderMock);

            IEnumerable<PipingSoilProfile> soilProfilesCollection = new PipingSoilProfile[0];

            // Call
            nodePresenter.UpdateNode(null, soilProfileCollectionNodeStub, soilProfilesCollection);

            // Assert
            Assert.AreEqual(RingtoetsFormsResources.PipingSoilProfilesCollection_DisplayName, soilProfileCollectionNodeStub.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), soilProfileCollectionNodeStub.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FolderIcon, soilProfileCollectionNodeStub.Image);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfileCollectionNodeStub = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

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
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnCollection()
        {
            // Setup
            var mocks = new MockRepository();
            mocks.ReplayAll();

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
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_Always_ContextMenuWithFiveItems(bool commonItemsEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter(TestContextMenuBuilderProvider.Create(mocks, nodeMock, commonItemsEnabled));

            mocks.ReplayAll();

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(5, returnedContextMenu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(returnedContextMenu, 0, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(returnedContextMenu, 1, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(returnedContextMenu, 3, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(returnedContextMenu, 4, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon, commonItemsEnabled);

            Assert.IsInstanceOf<ToolStripSeparator>(returnedContextMenu.Items[2]);

            mocks.VerifyAll();
        }
    }
}