using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
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
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

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

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

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

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

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

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

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
        public void GetContextMenu_DefaultScenario_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_NoContextMenuBuilderProviderSet_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();
            mocks.ReplayAll();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter();

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNull(returnedContextMenu);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_ContextMenuBuilderProviderSet_HaveImportSurfaceLinesItemInContextMenu(bool commonItemsEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock, commonItemsEnabled)
            };

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