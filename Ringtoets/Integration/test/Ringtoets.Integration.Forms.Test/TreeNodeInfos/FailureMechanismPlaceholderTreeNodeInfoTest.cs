using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Plugin;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class FailureMechanismPlaceholderTreeNodeInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailureMechanismPlaceholder));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(FailureMechanismPlaceholder), info.TagType);

            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var testName = "ttt";
            var placeholder = new FailureMechanismPlaceholder(testName);

            // Call
            var text = info.Text(placeholder);

            // Assert
            Assert.AreEqual(testName, text);
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void ForeColor_Always_ReturnsGrayText()
        {
            // Call
            var textColor = info.ForeColor(null);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), textColor);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnFoldersWithInputAndOutput()
        {
            // Setup
            var failureMechanism = new FailureMechanismPlaceholder("test");

            // Call
            object[] children = info.ChildNodeObjects(failureMechanism).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);
            var inputFolder = (CategoryTreeFolder)children[0];
            Assert.AreEqual("Invoer", inputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputFolder.Category);
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.SectionDivisions,
                failureMechanism.Locations,
                failureMechanism.BoundaryConditions
            }, inputFolder.Contents);

            var outputFolder = (CategoryTreeFolder)children[1];
            Assert.AreEqual("Uitvoer", outputFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputFolder.Category);
            Assert.AreEqual(new[]{failureMechanism.AssessmentResult}, outputFolder.Contents);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var nodeMock = mocks.StrictMock<TreeNode>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(nodeMock, info)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, nodeMock, info);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CalculateAllAndClearAllItemDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilderMock = new CustomItemsOnlyContextMenuBuilder();
            var nodeMock = mocks.StrictMock<TreeNode>();

            gui.Expect(cmp => cmp.Get(nodeMock, info)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            var menu = info.ContextMenuStrip(null, nodeMock, info);

            // Assert
            mocks.VerifyAll();

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, RingtoetsCommonFormsResources.Calculate_all, RingtoetsCommonFormsResources.Calculate_all_ToolTip, RingtoetsCommonFormsResources.CalculateAllIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 1, RingtoetsCommonFormsResources.Clear_all_output, RingtoetsCommonFormsResources.Clear_all_output_ToolTip, RingtoetsCommonFormsResources.ClearIcon, false);
        }
    }
}