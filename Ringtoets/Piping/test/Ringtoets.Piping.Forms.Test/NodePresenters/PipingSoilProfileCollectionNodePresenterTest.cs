using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.NodePresenters;

using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

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
        public void GetContextMenu_ContextMenuBuilderProviderSet_HaveImportSurfaceLinesItemInContextMenu(bool importExportEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<IEnumerable<PipingSoilProfile>>();

            var nodePresenter = new PipingSoilProfileCollectionNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock, importExportEnabled)
            };

            mocks.ReplayAll();

            // Call
            var returnedContextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(5, returnedContextMenu.Items.Count);
            var expandAllItem = returnedContextMenu.Items[0];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Expand_all, expandAllItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Expand_all_ToolTip, expandAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExpandAllIcon, expandAllItem.Image);
            Assert.IsTrue(expandAllItem.Enabled);

            var collapseAllItem = returnedContextMenu.Items[1];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Collapse_all, collapseAllItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Collapse_all_ToolTip, collapseAllItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.CollapseAllIcon, collapseAllItem.Image);
            Assert.IsTrue(collapseAllItem.Enabled);

            var importItem = returnedContextMenu.Items[3];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Import, importItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Import_ToolTip, importItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ImportIcon, importItem.Image);
            Assert.AreEqual(importExportEnabled, importItem.Enabled);

            var exportItem = returnedContextMenu.Items[4];
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export, exportItem.Text);
            Assert.AreEqual(Core.Common.Gui.Properties.Resources.Export_ToolTip, exportItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(Core.Common.Gui.Properties.Resources.ExportIcon, exportItem.Image);
            Assert.AreEqual(importExportEnabled, exportItem.Enabled);

            Assert.IsInstanceOf<ToolStripSeparator>(returnedContextMenu.Items[2]);

            mocks.VerifyAll();
        }
    }
}