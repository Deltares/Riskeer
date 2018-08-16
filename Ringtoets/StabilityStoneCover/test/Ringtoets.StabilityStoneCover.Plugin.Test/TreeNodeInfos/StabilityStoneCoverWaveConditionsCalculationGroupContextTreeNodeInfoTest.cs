// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddGenerateCalculationsIndex = 3;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 5;
        private const int contextMenuAddCalculationIndexRootGroup = 6;
        private const int contextMenuUpdateForeshoreProfileIndexRootGroup = 8;
        private const int contextMenuValidateAllIndexRootGroup = 10;
        private const int contextMenuCalculateAllIndexRootGroup = 11;
        private const int contextMenuClearOutputIndexRootGroup = 13;
        private const int contextMenuRemoveAllChildrenIndexRootGroup = 14;
        private const int contextMenuCollapseAllIndexRootGroup = 16;
        private const int contextMenuExpandAllIndexRootGroup = 17;
        private const int contextMenuPropertiesIndexRootGroup = 19;

        private const int contextMenuDuplicateIndexNestedGroup = 3;
        private const int contextMenuAddCalculationGroupIndexNestedGroup = 5;
        private const int contextMenuAddCalculationIndexNestedGroup = 6;
        private const int contextMenuUpdateForeshoreProfileIndexNestedGroup = 9;
        private const int contextMenuValidateAllIndexNestedGroup = 11;
        private const int contextMenuCalculateAllIndexNestedGroup = 12;
        private const int contextMenuClearOutputIndexNestedGroup = 14;

        private IGui gui;
        private MockRepository mocks;
        private StabilityStoneCoverPlugin plugin;
        private TreeNodeInfo info;

        private readonly string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, Path.Combine("HydraRingCalculation", "HRD ijsselmeer.sqlite"));

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
        public void Text_Always_ReturnGroupName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische belastingen", text);
        }

        [Test]
        public void Image_Always_ReturnCalculationGroupIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var groupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                            null,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithChildren_ReturnChildren()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var childGroup = new CalculationGroup();
            var calculationItem = new StabilityStoneCoverWaveConditionsCalculation();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationItem);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(childGroup);

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                        null,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(failureMechanism.WaveConditionsCalculationGroup.Children.Count, children.Length);

            var returnedCalculationContext = (StabilityStoneCoverWaveConditionsCalculationContext) children[0];
            Assert.AreSame(calculationItem, returnedCalculationContext.WrappedData);
            Assert.AreSame(failureMechanism.WaveConditionsCalculationGroup, returnedCalculationContext.Parent);
            Assert.AreSame(failureMechanism, returnedCalculationContext.FailureMechanism);
            Assert.AreSame(assessmentSection, returnedCalculationContext.AssessmentSection);

            var returnedCalculationGroupContext = (StabilityStoneCoverWaveConditionsCalculationGroupContext) children[1];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism.WaveConditionsCalculationGroup, returnedCalculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, returnedCalculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, returnedCalculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var groupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            null,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

            var orderedMocks = new MockRepository();
            var menuBuilder = orderedMocks.StrictMock<IContextMenuBuilder>();
            using (orderedMocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteChildrenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();
                orderedMocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, null, treeViewControl);
            }

            // Assert
            orderedMocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var groupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            null,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(20, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de randvoorwaardenberekeningen te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            parentGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
            var parentGroupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(parentGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl);
            }

            // Assert
            // Assert expectancies called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_AddCustomItems()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var groupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            parentGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
            var parentGroupContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(parentGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(21, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDuplicateIndexNestedGroup,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RingtoetsCommonFormsResources.CopyHS);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexNestedGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexNestedGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndexNestedGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Er zijn geen berekeningen om bij te werken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexNestedGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithoutHydraulicLocationsDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                        null,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            importCommandHandler.Expect(ihm => ihm.CanImportOn(nodeData)).Return(true);
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            exportCommandHandler.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de randvoorwaardenberekeningen te genereren.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon, false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreCommonGuiResources.DeleteChildrenIcon,
                                                                  false);
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  "Alles ui&tklappen",
                                                                  "Klap dit element en alle onderliggende elementen uit.",
                                                                  CoreCommonGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  "Alles i&nklappen",
                                                                  "Klap dit element en alle onderliggende elementen in.",
                                                                  CoreCommonGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  CoreCommonGuiResources.PropertiesHS,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeWithHydraulicLocationsDefaultBehavior_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    new TestHydraulicBoundaryLocation()
                }
            });

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                        null,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            importCommandHandler.Expect(ihm => ihm.CanImportOn(nodeData)).Return(true);
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            exportCommandHandler.Expect(ehm => ehm.CanExportFrom(nodeData)).Return(true);
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         updateCommandHandler,
                                                         viewCommandsHandler,
                                                         nodeData,
                                                         treeViewControl);

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddGenerateCalculationsIndex,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer randvoorwaardenberekeningen.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndexRootGroup,
                                                                  "&Map toevoegen",
                                                                  "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.AddFolderIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuRemoveAllChildrenIndexRootGroup,
                                                                  "Ma&p leegmaken...",
                                                                  "Er zijn geen onderliggende elementen om te verwijderen.",
                                                                  CoreCommonGuiResources.DeleteChildrenIcon,
                                                                  false);
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
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndexRootGroup,
                                                                  "&Wis alle uitvoer...",
                                                                  "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuExpandAllIndexRootGroup,
                                                                  "Alles ui&tklappen",
                                                                  "Klap dit element en alle onderliggende elementen uit.",
                                                                  CoreCommonGuiResources.ExpandAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCollapseAllIndexRootGroup,
                                                                  "Alles i&nklappen",
                                                                  "Klap dit element en alle onderliggende elementen in.",
                                                                  CoreCommonGuiResources.CollapseAllIcon,
                                                                  false);
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuPropertiesIndexRootGroup,
                                                                  "Ei&genschappen",
                                                                  "Toon de eigenschappen in het Eigenschappenpaneel.",
                                                                  CoreCommonGuiResources.PropertiesHS,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroupWithoutCalculations_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
                    Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                group.Children.Add(new StabilityStoneCoverWaveConditionsCalculation());
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    Assert.AreEqual("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", calculateItem.ToolTipText);
                    Assert.AreEqual("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_CalculateAllAndValidateAllDisabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                group.Children.Add(new StabilityStoneCoverWaveConditionsCalculation());
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsFalse(validateItem.Enabled);
                    Assert.IsFalse(calculateItem.Enabled);
                    const string message = "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand 'invalidFilePath': het bestand bestaat niet.";
                    Assert.AreEqual(message, calculateItem.ToolTipText);
                    Assert.AreEqual(message, validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_CalculateAllAndValidateAllEnabled()
        {
            // Setup
            string validHydraulicBoundaryDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                                   Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                group.Children.Add(new StabilityStoneCoverWaveConditionsCalculation());
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    FilePath = validHydraulicBoundaryDatabasePath
                });

                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Assert
                    ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndexNestedGroup];
                    ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndexNestedGroup];
                    Assert.IsTrue(validateItem.Enabled);
                    Assert.IsTrue(calculateItem.Enabled);
                    Assert.AreEqual("Voer alle berekeningen binnen deze map met berekeningen uit.", calculateItem.ToolTipText);
                    Assert.AreEqual("Valideer alle berekeningen binnen deze map met berekeningen.", validateItem.ToolTipText);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnValidateAllInGroup_ValidationMessagesLogged()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculationA = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            StabilityStoneCoverWaveConditionsCalculation calculationB = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var group = new CalculationGroup();
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(4, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[2]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[3]);
                    });
                }
            }
        }

        [Test]
        public void ValidateAllViaContextMenuStrip_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_NoValidationErrorsLogged()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var group = new CalculationGroup();
            group.Children.Add(calculation);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(2, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                    });
                }
            }
        }

        [Test]
        public void ValidateAllViaContextMenuStrip_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_NoValidationErrorsLogged()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var group = new CalculationGroup();
            group.Children.Add(calculation);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(2, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                    });
                }
            }
        }

        [Test]
        public void ValidateAllViaContextMenuStrip_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_NoValidationErrorsLogged()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());

            var group = new CalculationGroup();
            group.Children.Add(calculation);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = TestHelper.GetScratchPadPath();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(2, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                    });
                }
            }
        }

        [Test]
        public void ValidateAllViaContextMenuStrip_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_ValidationErrorsLogged()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            var random = new Random(21);
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First(),
                    LowerBoundaryRevetment = (RoundedDouble) 1.0,
                    UpperBoundaryRevetment = (RoundedDouble) 10.0,
                    StepSize = WaveConditionsInputStepSize.One,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1.0,
                    UpperBoundaryWaterLevels = (RoundedDouble) 10.0,
                    Orientation = (RoundedDouble) 0,
                    CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>()
                }
            };

            var group = new CalculationGroup();
            group.Children.Add(calculation);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuValidateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(3, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", messages[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsClickOnCalculateAllInGroup_MessagesLogged()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            var group = new CalculationGroup();
            StabilityStoneCoverWaveConditionsCalculation calculationA = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculationA.Name = "A";
            StabilityStoneCoverWaveConditionsCalculation calculationB = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculationB.Name = "B";
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(Path.GetDirectoryName(validFilePath), string.Empty)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    Action test = () => contextMenu.Items[contextMenuCalculateAllIndexNestedGroup].PerformClick();

                    // Assert
                    TestHelper.AssertLogMessages(test, m =>
                    {
                        string[] messages = m.ToArray();
                        Assert.AreEqual(56, messages.Length);
                        Assert.AreEqual("Golfcondities voor blokken en zuilen berekenen voor 'A' is gestart.", messages[0]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messages[26]);
                        Assert.AreEqual("Golfcondities voor blokken en zuilen berekenen voor 'A' is gelukt.", messages[27]);

                        Assert.AreEqual("Golfcondities voor blokken en zuilen berekenen voor 'B' is gestart.", messages[28]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messages[31]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messages[54]);
                        Assert.AreEqual("Golfcondities voor blokken en zuilen berekenen voor 'B' is gelukt.", messages[55]);
                    });
                    Assert.AreEqual(3, calculationA.Output.BlocksOutput.Count());
                    Assert.AreEqual(3, calculationA.Output.ColumnsOutput.Count());
                    Assert.AreEqual(3, calculationB.Output.BlocksOutput.Count());
                    Assert.AreEqual(3, calculationB.Output.ColumnsOutput.Count());
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoCalculations_ClearAllOutputItemDisabled()
        {
            // Setup
            var group = new CalculationGroup();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                mocks.ReplayAll();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    ToolStripItem clearAllOutputItem = contextMenu.Items[contextMenuClearOutputIndexNestedGroup];

                    // Assert
                    Assert.IsFalse(clearAllOutputItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_TwoCalculationsWithoutOutput_ClearAllOutputItemDisabled()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);

            var group = new CalculationGroup();
            StabilityStoneCoverWaveConditionsCalculation calculationA = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            StabilityStoneCoverWaveConditionsCalculation calculationB = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(Path.GetDirectoryName(validFilePath), string.Empty)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    ToolStripItem clearAllOutputItem = contextMenu.Items[contextMenuClearOutputIndexNestedGroup];

                    // Assert
                    Assert.IsFalse(clearAllOutputItem.Enabled);
                }
            }
        }

        [Test]
        [TestCase(true, TestName = "Menu_TwoCalculationsAndOutputClickClearAllOutput_ClearOutputAfterConfirmation(true)")]
        [TestCase(false, TestName = "Menu_TwoCalculationsAndOutputClickClearAllOutput_ClearOutputAfterConfirmation(false)")]
        public void ContextMenuStrip_TwoCalculationsWithOutputClickOnClearAllOutput_OutputRemovedForCalculationsAfterConfirmation(bool confirm)
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);

            var observerA = mocks.StrictMock<IObserver>();
            var observerB = mocks.StrictMock<IObserver>();
            if (confirm)
            {
                observerA.Expect(o => o.UpdateObserver());
                observerB.Expect(o => o.UpdateObserver());
            }

            var group = new CalculationGroup();
            StabilityStoneCoverWaveConditionsCalculation calculationA = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculationA.Output = new StabilityStoneCoverWaveConditionsOutput(
                Enumerable.Empty<WaveConditionsOutput>(),
                Enumerable.Empty<WaveConditionsOutput>());
            StabilityStoneCoverWaveConditionsCalculation calculationB = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculationB.Output = new StabilityStoneCoverWaveConditionsOutput(
                Enumerable.Empty<WaveConditionsOutput>(),
                Enumerable.Empty<WaveConditionsOutput>());
            group.Children.Add(calculationA);
            group.Children.Add(calculationB);
            calculationA.Attach(observerA);
            calculationB.Attach(observerB);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            DialogBoxHandler = (name, wnd) =>
            {
                var dialog = new MessageBoxTester(wnd);
                if (confirm)
                {
                    dialog.ClickOk();
                }
                else
                {
                    dialog.ClickCancel();
                }
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var mainWindow = mocks.Stub<IMainWindow>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(Path.GetDirectoryName(validFilePath), string.Empty)).Return(new TestWaveConditionsCosineCalculator());
                mocks.ReplayAll();

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuClearOutputIndexNestedGroup].PerformClick();

                    // Assert
                    if (confirm)
                    {
                        Assert.IsNull(calculationA.Output);
                        Assert.IsNull(calculationB.Output);
                    }
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var group = new CalculationGroup();
                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);

                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            failureMechanism.WaveConditionsCalculationGroup,
                                                                                            failureMechanism,
                                                                                            assessmentSection);
                var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                  null,
                                                                                                  failureMechanism,
                                                                                                  assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                mocks.ReplayAll();

                var calculationItem = new CalculationGroup
                {
                    Name = "Nieuwe map"
                };
                group.Children.Add(calculationItem);

                nodeData.Attach(observer);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentNodeData, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationGroupIndexNestedGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<CalculationGroup>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        [TestCase(NormType.Signaling)]
        [TestCase(NormType.LowerLimit)]
        public void ContextMenuStrip_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers(
            NormType normType)
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    NormativeNorm = normType
                }
            };
            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        null,
                                                                                        failureMechanism,
                                                                                        assessmentSection);
            var calculationItem = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "Nieuwe berekening"
            };
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
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
                    ICalculationBase newlyAddedItem = group.Children.Last();
                    var newCalculationItem = newlyAddedItem as StabilityStoneCoverWaveConditionsCalculation;
                    Assert.IsNotNull(newCalculationItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                    Assert.AreEqual(GetCategoryTypeFromNormType(normType), newCalculationItem.InputParameters.CategoryType);
                }
            }
        }

        [Test]
        public void GivenDialogGenerateCalculationButtonClicked_WhenCalculationSelectedAndDialogClosed_ThenUpdateCalculationGroup()
        {
            // Given
            var random = new Random(21);
            var normType = random.NextEnumValue<NormType>();

            using (var treeViewControl = new TreeViewControl())
            {
                var existingGroup = new CalculationGroup();
                var existingcalculation = new StabilityStoneCoverWaveConditionsCalculation();
                var group = new CalculationGroup
                {
                    Children =
                    {
                        existingGroup,
                        existingcalculation
                    }
                };

                var failureMechanism = new StabilityStoneCoverFailureMechanism();
                var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
                var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

                var assessmentSection = new AssessmentSectionStub
                {
                    FailureMechanismContribution =
                    {
                        NormativeNorm = normType
                    }
                };
                assessmentSection.HydraulicBoundaryDatabase.Locations.AddRange(new[]
                {
                    hydraulicBoundaryLocation1,
                    hydraulicBoundaryLocation2
                });

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            null,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();
                var viewCommands = mocks.StrictMock<IViewCommands>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ViewCommands).Return(viewCommands);

                mocks.ReplayAll();

                nodeData.Attach(observer);

                HydraulicBoundaryLocationSelectionDialog dialog = null;
                DataGridViewControl grid = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    dialog = (HydraulicBoundaryLocationSelectionDialog) new FormTester(name).TheObject;
                    grid = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                    grid.Rows[0].Cells[0].Value = true;
                    grid.Rows[1].Cells[0].Value = true;
                    new ButtonTester("DoForSelectedButton", dialog).Click();
                };

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

                // Then
                Assert.AreEqual(4, group.Children.Count);
                Assert.AreSame(existingGroup, group.Children[0]);
                Assert.AreSame(existingcalculation, group.Children[1]);
                Assert.NotNull(dialog);
                Assert.NotNull(grid);

                AssessmentSectionCategoryType expectedAssessmentSectionCategoryType = GetCategoryTypeFromNormType(normType);
                var firstCalculation = group.Children[2] as StabilityStoneCoverWaveConditionsCalculation;
                Assert.IsNotNull(firstCalculation);
                AssessmentSectionCategoryWaveConditionsInput firstCalculationInputParameters = firstCalculation.InputParameters;
                Assert.AreSame(hydraulicBoundaryLocation1, firstCalculationInputParameters.HydraulicBoundaryLocation);
                Assert.AreEqual(expectedAssessmentSectionCategoryType, firstCalculationInputParameters.CategoryType);

                var secondCalculation = group.Children[3] as StabilityStoneCoverWaveConditionsCalculation;
                Assert.IsNotNull(secondCalculation);
                AssessmentSectionCategoryWaveConditionsInput secondCalculationInputParameters = secondCalculation.InputParameters;
                Assert.AreSame(hydraulicBoundaryLocation2, secondCalculationInputParameters.HydraulicBoundaryLocation);
                Assert.AreEqual(expectedAssessmentSectionCategoryType, secondCalculationInputParameters.CategoryType);
            }
        }

        [Test]
        public void GivenDialogGenerateCalculationButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationGroupNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var group = new CalculationGroup();
                var failureMechanism = new StabilityStoneCoverFailureMechanism();

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new TestHydraulicBoundaryLocation()
                    }
                });

                var observer = mocks.StrictMock<IObserver>();
                var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                            null,
                                                                                            failureMechanism,
                                                                                            assessmentSection);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();
                var viewCommands = mocks.StrictMock<IViewCommands>();

                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ViewCommands).Return(viewCommands);

                mocks.ReplayAll();

                nodeData.Attach(observer);

                HydraulicBoundaryLocationSelectionDialog dialog = null;
                DataGridViewControl grid = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    dialog = (HydraulicBoundaryLocationSelectionDialog) new FormTester(name).TheObject;
                    grid = (DataGridViewControl) new ControlTester("DataGridViewControl", dialog).TheObject;
                    grid.Rows[0].Cells[0].Value = true;
                    new ButtonTester("CustomCancelButton", dialog).Click();
                };

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    contextMenu.Items[contextMenuAddGenerateCalculationsIndex].PerformClick();

                // Then
                Assert.AreEqual(0, group.Children.Count);
                Assert.NotNull(dialog);
                Assert.NotNull(grid);
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsWaveConditionsCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var group = new CalculationGroup();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                        failureMechanism.WaveConditionsCalculationGroup,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(group);
            var parentNodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                              null,
                                                                                              failureMechanism,
                                                                                              assessmentSection);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.WaveConditionsCalculationGroup.Children, group);
        }

        [Test]
        public void GivenCalculationWithoutOutput_ThenClearOutputItemDisabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);

            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                // When
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuAddCalculationIndexRootGroup,
                                                                  "Berekening &toevoegen",
                                                                  "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                                  RingtoetsCommonFormsResources.FailureMechanismIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_WhenClearingOutput_ThenClearOutput()
        {
            // Given
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Attach(observer);

            var context = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       new AssessmentSectionStub());

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[contextMenuAddCalculationIndexRootGroup];
                    validateMenuItem.PerformClick();

                    // Then
                    Assert.AreEqual(1, failureMechanism.WaveConditionsCalculationGroup.Children.Count);
                    Assert.IsInstanceOf<StabilityStoneCoverWaveConditionsCalculation>(failureMechanism.WaveConditionsCalculationGroup.Children[0]);
                    // Check expectancies in TearDown()
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationGroupWithCalculationWithForeshoreProfileAndInputOutOfSync_ContextMenuItemUpdateForeshoreProfilesEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                null,
                failureMechanism,
                assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                calculation.InputParameters.UseBreakWater = true;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                  contextMenuUpdateForeshoreProfileIndexRootGroup,
                                                                  "&Bijwerken voorlandprofielen...",
                                                                  "Alle berekeningen met een voorlandprofiel bijwerken.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfilesClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            calculationInputObserver.Expect(o => o.UpdateObserver());

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile(true)
                }
            };

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationGroupContext(
                new CalculationGroup
                {
                    Children =
                    {
                        calculation
                    }
                },
                null,
                failureMechanism,
                assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                calculation.InputParameters.UseBreakWater = false;

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndexRootGroup].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                }
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            gui = mocks.Stub<IGui>();
            plugin = new StabilityStoneCoverPlugin
            {
                Gui = gui
            };
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationGroupContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        private IAssessmentSection CreateAssessmentSectionWithHydraulicBoundaryOutput()
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.LowerLimitNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }

        private static AssessmentSectionCategoryType GetCategoryTypeFromNormType(NormType normType)
        {
            switch (normType)
            {
                case NormType.LowerLimit:
                    return AssessmentSectionCategoryType.LowerLimitNorm;
                case NormType.Signaling:
                    return AssessmentSectionCategoryType.SignalingNorm;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}