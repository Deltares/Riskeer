// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Plugin;
using Ringtoets.HeightStructures.Plugin.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using HeightStructuresFormsResources = Ringtoets.HeightStructures.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HeightStructuresFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private MockRepository mocksRepository;
        private HeightStructuresGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new HeightStructuresGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HeightStructuresFailureMechanismContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HeightStructuresFailureMechanismContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
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
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);
            Assert.AreEqual(2, inputsFolder.Contents.Count);
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
            CollectionAssert.AreEqual(failureMechanism.Sections, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSectionMock, failureMechanismSectionsContext.ParentAssessmentSection);
            var commentContext = (CommentContext<ICommentable>) inputsFolder.Contents[1];
            Assert.AreSame(failureMechanism, commentContext.CommentContainer);

            var calculationsFolder = (HeightStructuresCalculationGroupContext) children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
            Assert.AreSame(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.AreSame(failureMechanism, calculationsFolder.FailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            var failureMechanismResultsContext = (FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>) outputsFolder.Contents[0];
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.SectionResults);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismComments()
        {
            // Setup
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var commentContext = (CommentContext<ICommentable>) children[0];
            Assert.AreSame(failureMechanism, commentContext.CommentContainer);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
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
            guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilderMock);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilderMock);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
                var failureMechanism = new HeightStructuresFailureMechanism();
                var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                var menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView);

                // Assert
                Assert.AreEqual(8, menu.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                              RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                                                              RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                                                              RingtoetsCommonFormsResources.Checkbox_ticked);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                              RingtoetsCommonFormsResources.Calculate_all,
                                                              RingtoetsCommonFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run,
                                                              RingtoetsCommonFormsResources.CalculateAllIcon,
                                                              false);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndex,
                                                              RingtoetsCommonFormsResources.Clear_all_output,
                                                              RingtoetsCommonFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                              RingtoetsCommonFormsResources.ClearIcon,
                                                              false);

                mocksRepository.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
                var failureMechanism = new HeightStructuresFailureMechanism
                {
                    IsRelevant = false
                };
                var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                var menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView);

                // Assert
                Assert.AreEqual(2, menu.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                              RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                                                              RingtoetsCommonFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                                                              RingtoetsCommonFormsResources.Checkbox_ticked);

                mocksRepository.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_OnChangeActionRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var viewCommandsMock = mocksRepository.StrictMock<IViewCommands>();
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            viewCommandsMock.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));
            guiMock.Stub(g => g.ViewCommands).Return(viewCommandsMock);
            guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            // Call
            contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_NoOnChangeAction()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            // Call
            contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

            // Assert
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                                                            new ProbabilityAssessmentInput()));
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();

            var nodeData = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                          RingtoetsFormsResources.CalculateIcon,
                                                          false);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                                                            new ProbabilityAssessmentInput()));

            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(null);

            var nodeData = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                          RingtoetsFormsResources.CalculateAllIcon,
                                                          false);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                                                            new ProbabilityAssessmentInput()));

            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());

            var nodeData = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

            Assert.AreEqual(RingtoetsFormsResources.Calculate_all, contextMenuItem.Text);
            StringAssert.Contains(string.Format(RingtoetsFormsResources.GuiPlugin_VerifyHydraulicBoundaryDatabasePath_Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.CalculateAllIcon, contextMenuItem.Image);
            Assert.IsFalse(contextMenuItem.Enabled);

            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                                                            new ProbabilityAssessmentInput()));

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            guiMock.Expect(g => g.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.Calculate_all_ToolTip,
                                                          RingtoetsFormsResources.CalculateAllIcon);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var mainWindowStub = mocksRepository.Stub<IMainWindow>();
            var treeViewControlMock = mocksRepository.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new HeightStructuresFailureMechanism();

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0)
            });
            failureMechanism.AddSection(section);

            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation(new GeneralHeightStructuresInput(), new ProbabilityAssessmentInput())
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation(new GeneralHeightStructuresInput(), new ProbabilityAssessmentInput())
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseStub = mocksRepository.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;
            var failureMechanismContext = new HeightStructuresFailureMechanismContext(failureMechanism, assessmentSection);

            guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControlMock)).Return(menuBuilder);
            guiMock.Expect(g => g.MainWindow).Return(mainWindowStub);

            mocksRepository.ReplayAll();

            plugin.Gui = guiMock;
            var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControlMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // Call
            TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick(), messages =>
            {
                var messageList = messages.ToList();

                // Assert
                Assert.AreEqual(4, messageList.Count);
                Assert.AreEqual("Er is een fout opgetreden tijdens de berekening.", messageList[0]);
                Assert.AreEqual("Uitvoeren van 'A' is mislukt.", messageList[1]);
                Assert.AreEqual("Er is een fout opgetreden tijdens de berekening.", messageList[2]);
                Assert.AreEqual("Uitvoeren van 'B' is mislukt.", messageList[3]);
            });

            mocksRepository.VerifyAll();
        }

        private const int contextMenuRelevancyIndexWhenRelevant = 1;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearAllIndex = 4;
    }
}