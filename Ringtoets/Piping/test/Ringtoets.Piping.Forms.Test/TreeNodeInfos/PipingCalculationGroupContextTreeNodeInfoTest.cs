using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin;
using Ringtoets.Piping.Primitives;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        private const int contextMenuAddGenerateCalculationsIndex = 1;

        private const int contextMenuAddCalculationGroupIndex = 0;
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearOutputIndex = 5;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingCalculationGroupContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void Text_Always_ReturnsWrappedDataName()
        {
            // Setup
            var testname = "testName";
            var group = new PipingCalculationGroup { Name = testname };
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var text = info.Text(groupContext);

            // Assert
            Assert.AreEqual(testname, text);
        }

        [Test]
        public void Image_Always_FolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.FolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            var result = info.EnsureVisibleOnCreate(groupContext);

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var calculationItem = mocks.StrictMock<IPipingCalculationItem>();

            var childCalculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var childGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationContext) children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            Assert.AreSame(pipingFailureMechanismMock, returnedCalculationContext.PipingFailureMechanism);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext)children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(pipingFailureMechanismMock, returnedCalculationGroupContext.PipingFailureMechanism);
            Assert.AreSame(assessmentSectionMock, returnedCalculationGroupContext.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ChildOfGroupValidDataWithCalculationOutput_ReturnContextMenuWithAllItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();

            var parentGroup = new PipingCalculationGroup();
            var group = new PipingCalculationGroup();

            parentGroup.Children.Add(group);
            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new PipingCalculationGroupContext(parentGroup,
                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                               Enumerable.Empty<StochasticSoilModel>(),
                                                               pipingFailureMechanismMock,
                                                               assessmentSectionMock);

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            Assert.AreEqual(17, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Delete,
                                                          CoreCommonGuiResources.Delete_ToolTip,
                                                          CoreCommonGuiResources.DeleteIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 14,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 16,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[9],
                menu.Items[12],
                menu.Items[15]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NotValidDataWithCalculationOutput_ReturnContextWithItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new PipingCalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new object();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            Assert.AreEqual(16, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 15,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[8],
                menu.Items[11],
                menu.Items[14]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithFailureMechanismContextParent_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new PipingCalculationGroup();

            group.Children.Add(new PipingCalculationScenario(new GeneralPipingInput(), new SemiProbabilisticPipingInput())
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new PipingFailureMechanismContext(pipingFailureMechanismMock, assessmentSectionMock);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            viewCommandsHandler.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            var mainCalculationGroupContextMenuItemOffset = 4;
            Assert.AreEqual(18, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex + mainCalculationGroupContextMenuItemOffset,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex + mainCalculationGroupContextMenuItemOffset,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex + mainCalculationGroupContextMenuItemOffset,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex + mainCalculationGroupContextMenuItemOffset,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex + mainCalculationGroupContextMenuItemOffset,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 14,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 15,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 17,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[1],
                menu.Items[3],
                menu.Items[6],
                menu.Items[10],
                menu.Items[13],
                menu.Items[16]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismAsParentWithoutAvailableSurfaceLines_GenerateCalculationsDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var group = new PipingCalculationGroup();
            
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new PipingFailureMechanismContext(pipingFailureMechanismMock, assessmentSectionMock);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             new []
                                                             {
                                                                 new TestStochasticSoilModel()
                                                             },
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);


            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip,
                                                          PipingFormsResources.GeneratePipingCalculationsIcon,
                                                          false);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismAsParentWithoutAvailableSoilModels_GenerateCalculationsDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var group = new PipingCalculationGroup();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new PipingFailureMechanismContext(pipingFailureMechanismMock, assessmentSectionMock);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             new[]
                                                             {
                                                                 new RingtoetsPipingSurfaceLine()
                                                             },
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);


            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip,
                                                          PipingFormsResources.GeneratePipingCalculationsIcon,
                                                          false);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismAsParentWithAvailableSurfaceLinesAndSoilModels_GenerateCalculationsEnabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var group = new PipingCalculationGroup();
            
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new PipingFailureMechanismContext(pipingFailureMechanismMock, assessmentSectionMock);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             new []
                                                             {
                                                                 new RingtoetsPipingSurfaceLine()
                                                             },
                                                             new[]
                                                             {
                                                                 new TestStochasticSoilModel()
                                                             },
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);


            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, 1,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations,
                                                          PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_ToolTip,
                                                          PipingFormsResources.GeneratePipingCalculationsIcon);
        }

        [Test]
        public void ContextMenuStrip_GroupWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new PipingCalculationGroup();
            var parentData = new PipingFailureMechanism();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndex];
            ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndex];
            Assert.IsFalse(validateItem.Enabled);
            Assert.IsFalse(calculateItem.Enabled);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var calculationItem = mocks.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe map");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationGroupIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculationGroup>(newlyAddedItem);
            Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name,
                            "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var calculationItem = mocks.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe berekening");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculation>(newlyAddedItem);
            Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                            "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(validCalculation);

            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Call
            Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validCalculation.Name), msgs[1]);

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidCalculation.Name), msgs[2]);
                // Some validation error from validation service
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidCalculation.Name), msgs[5]);
            });
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(validCalculation);

            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            DialogBoxHandler = (name, wnd) =>
            {
                // Don't care about dialogs in this test.
            };

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ContextMenuStrip_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();

            if (confirm)
            {
                calculation1Observer.Expect(o => o.UpdateObserver());
                calculation2Observer.Expect(o => o.UpdateObserver());
            }

            var calculation1 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation1.Name = "A";
            calculation1.Output = new TestPipingOutput();
            calculation1.Attach(calculation1Observer);
            var calculation2 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation2.Name = "B";
            calculation2.Output = new TestPipingOutput();
            calculation1.Attach(calculation2Observer);

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(calculation1);

            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                if (confirm)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };

            // Precondition
            Assert.IsTrue(group.HasOutput);

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuClearOutputIndex].PerformClick();

            // Assert
            Assert.AreNotEqual(confirm, group.HasOutput);
            Assert.AreNotEqual(confirm, calculation1.HasOutput);
            Assert.AreNotEqual(confirm, calculation2.HasOutput);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnGenerateCalculationsItemWithSurfaceLinesAndSoilModels_ShowSurfaceLineSelectionView()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new PipingCalculationGroup();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var mainWindow = mocks.Stub<IMainWindow>();

            var parentData = new PipingFailureMechanismContext(pipingFailureMechanismMock, assessmentSectionMock);
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name= "surfaceLine1"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name= "surfaceLine2"
                }
            };
            var nodeData = new PipingCalculationGroupContext(group,
                                                             surfaceLines,
                                                             new []
                                                             {
                                                                 new TestStochasticSoilModel()
                                                             },
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            PipingSurfaceLineSelectionDialog selectionDialog = null;
            DataGridView grid = null;
            DialogBoxHandler = (name, wnd) =>
            {
                selectionDialog = new FormTester(name).TheObject as PipingSurfaceLineSelectionDialog;
                grid = new ControlTester("SurfaceLineDataGrid", selectionDialog).TheObject as DataGridView;

                new ButtonTester("CustomCancelButton", selectionDialog).Click();
            };

            var contextMenu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Call
            contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

            // Assert
            Assert.NotNull(selectionDialog);
            Assert.NotNull(grid);
            Assert.AreEqual(2, grid.RowCount);

            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_ParentIsPipingFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismContextMock = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            bool isRenamingAllowed = info.CanRename(null, pipingFailureMechanismContextMock);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_EverythingElse_ReturnTrue()
        {
            // Call
            bool isRenamingAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_RenameGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);
            nodeData.Attach(observer);

            // Call
            const string newName = "new name";
            info.OnNodeRenamed(nodeData, newName);

            // Assert
            Assert.AreEqual(newName, group.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsFailureMechanism_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var parentNodeData = new PipingFailureMechanism();
            parentNodeData.CalculationsGroup.Children.Add(group);

            mocks.ReplayAll();

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_ReturnTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            mocks.ReplayAll();

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsTrue(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupNotContainingGroup_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new PipingCalculationGroup();
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);

            // Precondition
            CollectionAssert.DoesNotContain(parentGroup.Children, group);

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void OnNodeRemoved_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<StochasticSoilModel>(),
                                                             pipingFailureMechanismMock,
                                                             assessmentSectionMock);

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<StochasticSoilModel>(),
                                                                   pipingFailureMechanismMock,
                                                                   assessmentSectionMock);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_WithParentNodeDefaultBehavior_ReturnTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var canDrag = info.CanDrag(groupContext, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanDrag_ParentIsPipingFailureMechanismContext_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismContextMock = mocks.StrictMock<PipingFailureMechanismContext>(pipingFailureMechanismMock, assessmentSectionMock);

            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<StochasticSoilModel>(),
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var canDrag = info.CanDrag(groupContext, pipingFailureMechanismContextMock);

            // Assert
            Assert.IsFalse(canDrag);
        }

        [Test]
        [Combinatorial]
        public void CanDropOrCanInsert_DraggingPipingCalculationItemContextOntoGroupNotContainingItem_ReturnTrue(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;

            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanism, assessmentSection);

            PipingCalculationGroup targetGroup;
            PipingCalculationGroupContext targetGroupContext;
            CreatePipingCalculationGroupAndContext(out targetGroup, out targetGroupContext, failureMechanism, assessmentSection);

            failureMechanism.CalculationsGroup.Children.Add(draggedItem);
            failureMechanism.CalculationsGroup.Children.Add(targetGroup);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = info.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = info.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CanDropOrInsert_DraggingCalculationItemContextOntoGroupNotContainingItemOtherFailureMechanism_ReturnFalse(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;

            var targetFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, targetFailureMechanism, assessmentSection);

            var sourceFailureMechanism = new PipingFailureMechanism();
            sourceFailureMechanism.CalculationsGroup.Children.Add(draggedItem);

            PipingCalculationGroup targetGroup;
            PipingCalculationGroupContext targetGroupContext;
            CreatePipingCalculationGroupAndContext(out targetGroup, out targetGroupContext, sourceFailureMechanism, assessmentSection);

            targetFailureMechanism.CalculationsGroup.Children.Add(targetGroup);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = info.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = info.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_DraggingPipingCalculationItemContextOntoGroupEnd_MoveCalculationItemInstanceToNewGroup(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection);

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(draggedItem);

            PipingCalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var newOwnerObserver = mocks.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);

            // Call
            info.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControlMock);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
                "Dragging node at the end of the target PipingCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_InsertingPipingCalculationItemContextAtDifferentLocationWithinSameGroup_ChangeItemIndexOfCalculationItem(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType,
            [Values(0, 2)] int newIndex)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            const string name = "Very cool name";
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection, name);

            var existingItemStub = mocks.Stub<IPipingCalculationItem>();
            existingItemStub.Stub(i => i.Name).Return("");

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(existingItemStub);
            originalOwnerGroup.Children.Add(draggedItem);
            originalOwnerGroup.Children.Add(existingItemStub);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);

            // Call
            info.OnDrop(draggedItemContext, originalOwnerGroupContext, originalOwnerGroupContext, newIndex, treeViewControlMock);

            // Assert
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            Assert.AreNotSame(draggedItem, originalOwnerGroup.Children[1],
                "Should have removed 'draggedItem' from its original location in the collection.");
            Assert.AreSame(draggedItem, originalOwnerGroup.Children[newIndex],
                "Dragging node to specific location within owning PipingCalculationGroup should put the dragged data at that index.");
            Assert.AreEqual(name, draggedItem.Name,
                "No renaming should occur when dragging within the same PipingCalculationGroup.");

            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_DraggingPipingCalculationItemContextOntoGroupWithSameNamedItem_MoveCalculationItemInstanceToNewGroupAndRename(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, pipingFailureMechanismMock, assessmentSection);

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);
            originalOwnerGroup.Children.Add(draggedItem);

            PipingCalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, pipingFailureMechanismMock, assessmentSection);

            var sameNamedItem = mocks.Stub<IPipingCalculationItem>();
            sameNamedItem.Stub(i => i.Name).Return(draggedItem.Name);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var newOwnerObserver = mocks.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver());

            treeViewControlMock.Expect(tvc => tvc.TryRenameNodeForData(draggedItemContext));

            mocks.ReplayAll();

            newOwnerGroup.Children.Add(sameNamedItem);

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children.Select(c => c.Name), draggedItem.Name,
                "Name of the dragged item should already exist in new owner.");

            // Call
            info.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControlMock);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.First(),
                "Dragging to insert node at start of newOwnerGroup should place the node at the start of the list.");
            switch (draggedItemType)
            {
                case PipingCalculationItemType.Calculation:
                    Assert.AreEqual("Nieuwe berekening", draggedItem.Name);
                    break;
                case PipingCalculationItemType.Group:
                    Assert.AreEqual("Nieuwe map", draggedItem.Name);
                    break;
            }

            mocks.VerifyAll();
        }

        /// <summary>
        /// Creates an instance of <see cref="PipingCalculationGroup"/> and the corresponding
        /// <see cref="PipingCalculationGroupContext"/>.
        /// </summary>
        /// <param name="data">The created group without any children.</param>
        /// <param name="dataContext">The context object for <paramref name="data"/>, without any other data.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism the item and context belong to.</param>
        /// <param name="assessmentSection">The assessment section the item and context belong to.</param>
        private void CreatePipingCalculationGroupAndContext(out PipingCalculationGroup data, out PipingCalculationGroupContext dataContext, PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
        {
            data = new PipingCalculationGroup();

            dataContext = new PipingCalculationGroupContext(data,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            pipingFailureMechanism,
                                                            assessmentSection);
        }

        /// <summary>
        /// Creates an instance of <see cref="IPipingCalculationItem"/> and the corresponding context.
        /// </summary>
        /// <param name="type">Defines the implementation of <see cref="IPipingCalculationItem"/> to be constructed.</param>
        /// <param name="data">Output: The concrete create class based on <paramref name="type"/>.</param>
        /// <param name="dataContext">Output: The <see cref="PipingContext{T}"/> corresponding with <paramref name="data"/>.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism the item and context belong to.</param>
        /// <param name="assessmentSection">The assessment section the item and context belong to.</param>
        /// <param name="initialName">Optional: The name of <paramref name="data"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        private void CreatePipingCalculationItemAndContext(PipingCalculationItemType type, out IPipingCalculationItem data, out object dataContext, PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection, string initialName = null)
        {
            switch (type)
            {
                case PipingCalculationItemType.Calculation:
                    var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
                    if (initialName != null)
                    {
                        calculation.Name = initialName;
                    }
                    data = calculation;
                    dataContext = new PipingCalculationContext(calculation,
                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                               Enumerable.Empty<StochasticSoilModel>(),
                                                               pipingFailureMechanism,
                                                               assessmentSection);
                    break;
                case PipingCalculationItemType.Group:
                    var group = new PipingCalculationGroup();
                    if (initialName != null)
                    {
                        group.Name = initialName;
                    }
                    data = group;
                    dataContext = new PipingCalculationGroupContext(group,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Type indicator for testing methods on <see cref="TreeNodeInfo"/>.
        /// </summary>
        public enum DragDropTestMethod
        {
            /// <summary>
            /// Indicates <see cref="TreeNodeInfo.CanDrop"/>.
            /// </summary>
            CanDrop,

            /// <summary>
            /// Indicates <see cref="TreeNodeInfo.CanInsert"/>.
            /// </summary>
            CanInsert
        }

        /// <summary>
        /// Type indicator for implementations of <see cref="IPipingCalculationItem"/> to be created in a test.
        /// </summary>
        public enum PipingCalculationItemType
        {
            /// <summary>
            /// Indicates <see cref="PipingCalculation"/>.
            /// </summary>
            Calculation,

            /// <summary>
            /// Indicates <see cref="PipingCalculationGroup"/>.
            /// </summary>
            Group
        }
    }
}