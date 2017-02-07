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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityPointStructuresCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuGenerateCalculationsIndexRootGroup = 0;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 2;
        private const int contextMenuAddCalculationIndexRootGroup = 3;
        private const int contextMenuValidateAllIndexRootGroup = 5;
        private const int contextMenuCalculateAllIndexRootGroup = 6;
        private const int contextMenuClearAllIndexRootGroup = 8;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuAddCalculationIndexNestedGroup = 1;
        private const int contextMenuValidateAllIndexNestedGroup = 4;
        private const int contextMenuCalculateAllIndexNestedGroup = 5;
        private const int contextMenuClearAllIndexNestedGroup = 7;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private IGui gui;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private StabilityPointStructuresPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new StabilityPointStructuresPlugin
            {
                Gui = gui
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityPointStructuresCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var childCalculation = new StructuresCalculation<StabilityPointStructuresInput>();

            group.Children.Add(childGroup);
            group.Children.Add(childCalculation);

            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (StabilityPointStructuresCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, calculationGroupContext.AssessmentSection);
            var calculationContext = (StabilityPointStructuresCalculationContext) children[1];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(assessmentSection, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilderMock);
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
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, null, treeViewControl);
            }

            // Assert
            // Assert expectancies called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(15, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Er zijn geen kunstwerken beschikbaar om berekeningen voor te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithStructuresImported_GenerateItemEnabledWithTooltip()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.StabilityPointStructures.Add(new TestStabilityPointStructure());
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(15, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer berekeningen op basis van geselecteerde kunstwerken.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<StabilityPointStructuresInput>()
                }
            };

            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = null;

            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<StabilityPointStructuresInput>()
                }
            };

            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem calculateAllMenuItem = contextMenu.Items[contextMenuCalculateAllIndexRootGroup];

                    Assert.AreEqual("Alles be&rekenen", calculateAllMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", calculateAllMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, calculateAllMenuItem.Image);
                    Assert.IsFalse(calculateAllMenuItem.Enabled);

                    ToolStripItem validateAllMenuItem = contextMenu.Items[contextMenuValidateAllIndexRootGroup];

                    Assert.AreEqual("Alles &valideren", validateAllMenuItem.Text);
                    StringAssert.Contains("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt.", validateAllMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, validateAllMenuItem.Image);
                    Assert.IsFalse(validateAllMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismContributionZero_ContextMenuItemCalculateAllAndValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<StabilityPointStructuresInput>()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllAndValidateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new StructuresCalculation<StabilityPointStructuresInput>()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new StructuresCalculation<StabilityPointStructuresInput>());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze berekeningsmap uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);

                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new StabilityPointStructuresCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSection);
            var parentGroupContext = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                         failureMechanism,
                                                                                         assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(14, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexNestedGroup,
                                                                  "Alles &valideren",
                                                                  "Er zijn geen berekeningen om te valideren.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexNestedGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er zijn geen berekeningen om uit te voeren.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearAllIndexNestedGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationGroup = new CalculationGroup
            {
                Name = "Nieuwe map"
            };
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                group.Children.Add(calculationGroup);
                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationGroupIndexRootGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    var newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<CalculationGroup>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var calculationItem = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = "Nieuwe berekening"
            };
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                group.Children.Add(calculationItem);
                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationIndexRootGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    var newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<StructuresCalculation<StabilityPointStructuresInput>>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindow = mocks.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var groupContext = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig())
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(12, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", messageList[3]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[4]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[5]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[6]);
                        StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[7]);
                        StringAssert.StartsWith("Puntconstructies berekening is uitgevoerd op de tijdelijke locatie", messageList[8]);
                        StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[9]);
                        Assert.AreEqual("Uitvoeren van 'A' is gelukt.", messageList[10]);
                        Assert.AreEqual("Uitvoeren van 'B' is gelukt.", messageList[11]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new TestStabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });
            failureMechanism.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    InflowModelType = StabilityPointStructureInflowModelType.LowSill,
                    LoadSchematizationType = LoadSchematizationType.Linear
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabase.FilePath = validFilePath;

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var groupContext = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuValidateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(4, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[2]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[3]);
                    });
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenStabilityPointStructureSelectedAndDialogClosed_ThenCalculationsAddedWithStabilityPointStructureAssigned()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                StabilityPointStructure structure1 = new TestStabilityPointStructure("Structure 1");
                StabilityPointStructure structure2 = new TestStabilityPointStructure("Structure 2");

                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    StabilityPointStructures =
                    {
                        structure1,
                        structure2
                    }
                };

                var nodeData = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (StructureSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    var stabilityPointStructuresCalculations = failureMechanism.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>().ToArray();
                    Assert.AreEqual(1, stabilityPointStructuresCalculations.Length);
                    Assert.AreSame(structure1, stabilityPointStructuresCalculations[0].InputParameters.Structure);
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationsNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                StabilityPointStructure structure1 = new TestStabilityPointStructure("Structure 1");
                StabilityPointStructure structure2 = new TestStabilityPointStructure("Structure 2");

                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    StabilityPointStructures =
                    {
                        structure1,
                        structure2
                    }
                };

                var nodeData = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (StructureSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("CustomCancelButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual(0, failureMechanism.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>().Count());
                }
            }
        }

        [Test]
        public void GivenScenariosWithExistingCalculationWithSameName_WhenOkButtonClickedAndDialogClosed_ThenCalculationWithUniqueNameAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var existingCalculationName = "StabilityPoint structure";
                StabilityPointStructure stabilityPointStructure = new TestStabilityPointStructure(existingCalculationName);

                var failureMechanism = new StabilityPointStructuresFailureMechanism
                {
                    StabilityPointStructures =
                    {
                        stabilityPointStructure
                    },
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new StructuresCalculation<StabilityPointStructuresInput>
                            {
                                Name = existingCalculationName
                            }
                        }
                    }
                };

                var nodeData = new StabilityPointStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var selectionDialog = (StructureSelectionDialog) new FormTester(name).TheObject;
                    var grid = (DataGridViewControl) new ControlTester("DataGridViewControl", selectionDialog).TheObject;

                    grid.Rows[0].Cells[0].Value = true;

                    new ButtonTester("DoForSelectedButton", selectionDialog).Click();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    string expectedNewName = NamingHelper.GetUniqueName(failureMechanism.CalculationsGroup.Children, existingCalculationName, c => c.Name);

                    // When
                    contextMenu.Items[contextMenuGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    var stabilityPointStructuresCalculations = failureMechanism.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>().ToArray();
                    Assert.AreEqual(2, stabilityPointStructuresCalculations.Length);
                    Assert.AreEqual(expectedNewName, stabilityPointStructuresCalculations[1].Name);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            parentGroup.Children.Add(group);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        [Test]
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            mocks.ReplayAll();

            parentGroup.Children.Add(group);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            group.Children.Add(calculation);

            StabilityPointStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.First();
            result.Calculation = calculation;

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            Assert.IsNull(result.Calculation);
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new StabilityPointStructuresCalculationGroupContext(group,
                                                                               failureMechanism,
                                                                               assessmentSection);
            var parentNodeData = new StabilityPointStructuresCalculationGroupContext(parentGroup,
                                                                                     failureMechanism,
                                                                                     assessmentSection);
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(calculation);
            parentGroup.Children.Add(group);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}