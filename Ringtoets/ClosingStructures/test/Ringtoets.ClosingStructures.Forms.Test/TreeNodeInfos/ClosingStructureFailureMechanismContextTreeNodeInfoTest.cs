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

using System;
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServicesResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.ClosingStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class ClosingStructureFailureMechanismContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuRelevancyIndexWhenRelevant = 0;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;

        private const int contextMenuValidateAllIndex = 2;
        private const int contextMenuCalculateAllIndex = 3;
        private const int contextMenuClearAllIndex = 4;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private MockRepository mocksRepository;
        private ClosingStructuresPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new ClosingStructuresPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ClosingStructuresFailureMechanismContext));
        }

        [TearDown]
        public override void TearDown()
        {
            plugin.Dispose();

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocksRepository.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(ClosingStructuresFailureMechanismContext), info.TagType);
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
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(4, inputsFolder.Contents.Count);
            var failureMechanismSectionsContext = (FailureMechanismSectionsContext) inputsFolder.Contents[0];
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(assessmentSectionMock, failureMechanismSectionsContext.ParentAssessmentSection);

            var profilesContext = (ForeshoreProfilesContext) inputsFolder.Contents[1];
            Assert.AreSame(failureMechanism.ForeshoreProfiles, profilesContext.WrappedData);
            Assert.AreSame(assessmentSectionMock, profilesContext.ParentAssessmentSection);

            var closingStructuresContext = (ClosingStructuresContext) inputsFolder.Contents[2];
            Assert.AreSame(failureMechanism.ClosingStructures, closingStructuresContext.WrappedData);
            Assert.AreSame(assessmentSectionMock, closingStructuresContext.AssessmentSection);

            var commentContext = (CommentContext<ICommentable>) inputsFolder.Contents[3];
            Assert.AreSame(failureMechanism, commentContext.WrappedData);

            var calculationsFolder = (ClosingStructuresCalculationGroupContext) children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
            Assert.AreEqual(failureMechanism.CalculationsGroup, calculationsFolder.WrappedData);
            Assert.AreEqual(failureMechanism, calculationsFolder.FailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Oordeel", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            Assert.AreEqual(2, outputsFolder.Contents.Count);

            var scenariosContext = (ClosingStructuresScenariosContext) outputsFolder.Contents[0];
            Assert.AreSame(failureMechanism.CalculationsGroup, scenariosContext.WrappedData);
            Assert.AreSame(failureMechanism, scenariosContext.ParentFailureMechanism);

            var failureMechanismResultsContext = (FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>) outputsFolder.Contents[1];
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismComments()
        {
            // Setup
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var commentContext = (CommentContext<ICommentable>) children[0];
            Assert.AreSame(failureMechanism, commentContext.WrappedData);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var guiMock = mocksRepository.StrictMock<IGui>();
            var menuBuilderMock = mocksRepository.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilderMock);
                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new ClosingStructuresFailureMechanism();
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView))
                {
                    // Assert
                    Assert.AreEqual(10, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RingtoetsCommonFormsResources.Checkbox_ticked);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndex,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
                var failureMechanism = new ClosingStructuresFailureMechanism
                {
                    IsRelevant = false
                };
                var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.StrictMock<IGui>();

                guiMock.Expect(cmp => cmp.Get(failureMechanismContext, treeView)).Return(menuBuilder);
                guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(failureMechanismContext, assessmentSectionMock, treeView))
                {
                    // Assert
                    Assert.AreEqual(4, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRelevancyIndexWhenNotRelevant,
                                                                  "I&s relevant",
                                                                  "Geeft aan of dit toetsspoor relevant is of niet.",
                                                                  RingtoetsCommonFormsResources.Checkbox_empty);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_OnChangeActionRemovesAllViewsForItem()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var viewCommandsMock = mocksRepository.StrictMock<IViewCommands>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            viewCommandsMock.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));
            guiMock.Stub(g => g.ViewCommands).Return(viewCommandsMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();
                }
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevant()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                IsRelevant = false
            };
            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var guiMock = mocksRepository.StrictMock<IGui>();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();
                plugin.Gui = guiMock;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();
                }
            }

            // Assert
            // Assert is done in TearDown
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());
            var assessmentSectionMock = mocksRepository.StrictMock<IAssessmentSection>();

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen vakindeling geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er is geen vakindeling geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculateAllContextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", calculateAllContextMenuItem.Text);
                    StringAssert.Contains(string.Format("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. {0}", ""), calculateAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, calculateAllContextMenuItem.Image);
                    Assert.IsFalse(calculateAllContextMenuItem.Enabled);

                    ToolStripItem validateAllContextMenuItem = contextMenu.Items[contextMenuValidateAllIndex];

                    Assert.AreEqual("Alles &valideren", validateAllContextMenuItem.Text);
                    StringAssert.Contains(string.Format("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. {0}", ""), validateAllContextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, validateAllContextMenuItem.Image);
                    Assert.IsFalse(validateAllContextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<ClosingStructuresInput>());

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var nodeData = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen dit toetsspoor.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var guiMock = mocksRepository.StrictMock<IGui>();
            var mainWindowStub = mocksRepository.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            });
            failureMechanism.AddSection(section);

            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocksRepository.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSectionStub = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;
            assessmentSectionStub.Stub(a => a.Id).Return(string.Empty);
            assessmentSectionStub.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionStub);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                guiMock.Expect(g => g.MainWindow).Return(mainWindowStub);

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (var contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig())
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndex].PerformClick(), messages =>
                    {
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(12, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", messageList[3]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[4]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[5]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[6]);
                        StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[7]);
                        StringAssert.StartsWith("Betrouwbaarheid sluiting kunstwerk berekening is uitgevoerd op de tijdelijke locatie:", messageList[8]);
                        StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[9]);
                        Assert.AreEqual("Uitvoeren van 'A' is gelukt.", messageList[10]);
                        Assert.AreEqual("Uitvoeren van 'B' is gelukt.", messageList[11]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0)
            });
            failureMechanism.AddSection(section);
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocksRepository.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSectionMock = mocksRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;

            var failureMechanismContext = new ClosingStructuresFailureMechanismContext(failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var guiMock = mocksRepository.Stub<IGui>();
                guiMock.Expect(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);

                mocksRepository.ReplayAll();

                plugin.Gui = guiMock;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        var messageList = messages.ToArray();

                        Assert.AreEqual(4, messageList.Length);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[3]);
                    });
                }
            }
        }
    }
}