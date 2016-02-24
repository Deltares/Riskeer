using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsHydraringRormsResources = Ringtoets.HydraRing.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }

        [TearDown]
        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HydraulicBoundaryDatabaseContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            var name = "Hydraulische randvoorwaarden";
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            mocks.ReplayAll();

            var context = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            // Call
            var text = info.Text(context);

            // Assert
            Assert.AreEqual(name, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void CanRenameNode_Always_ReturnsFalse()
        {
            // Call
            var renameAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsFalse(renameAllowed);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabaseSet_ContextMenuItemCalculateToetspeilenDisabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, RingtoetsHydraringRormsResources.DesignWaterLevel_Calculate, RingtoetsHydraringRormsResources.DesignWaterLevel_No_HRD_To_Calculate, RingtoetsFormsResources.FailureMechanismIcon, false);
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseSet_ContextMenuItemCalculateToetspeilenEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);
            nodeData.Parent.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            nodeData.Parent.HydraulicBoundaryDatabase.FilePath = testDataPath;
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, RingtoetsHydraringRormsResources.DesignWaterLevel_Calculate, RingtoetsHydraringRormsResources.DesignWaterLevel_Calculate_ToolTip, RingtoetsFormsResources.FailureMechanismIcon);
        }
    }
}