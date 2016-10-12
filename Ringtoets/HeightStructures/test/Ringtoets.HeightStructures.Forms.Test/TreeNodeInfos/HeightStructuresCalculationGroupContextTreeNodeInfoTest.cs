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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Plugin;
using Ringtoets.HydraRing.Data;

using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using HeighStructuresPluginResources = Ringtoets.HeightStructures.Plugin.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HeightStructuresCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextGenerateCalculationsIndexRootGroup = 0;
        private const int contextMenuAddCalculationGroupIndexRootGroup = 2;
        private const int contextMenuAddCalculationIndexRootGroup = 3;
        private const int contextMenuValidateAllIndexRootGroup = 7;
        private const int contextMenuCalculateAllIndexRootGroup = 8;
        private const int contextMenuClearAllIndexRootGroup = 9;

        private const int contextMenuAddCalculationGroupIndexNestedGroup = 0;
        private const int contextMenuAddCalculationIndexNestedGroup = 1;
        private const int contextMenuValidateAllIndexNestedGroup = 3;
        private const int contextMenuCalculateAllIndexNestedGroup = 4;
        private const int contextMenuClearAllIndexNestedGroup = 5;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private IGui guiStub;
        private TreeNodeInfo info;
        private MockRepository mocks;
        private HeightStructuresPlugin plugin;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            guiStub = mocks.Stub<IGui>();
            plugin = new HeightStructuresPlugin
            {
                Gui = guiStub
            };

            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HeightStructuresCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(HeightStructuresCalculationGroupContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.OnDrop);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void ChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var calculationItemMock = mocks.Stub<ICalculationBase>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var childCalculation = new HeightStructuresCalculation();

            group.Children.Add(childGroup);
            group.Children.Add(calculationItemMock);
            group.Children.Add(childCalculation);

            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);

            // Call
            var children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            var calculationGroupContext = (HeightStructuresCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSectionStub, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItemMock, children[1]);
            var calculationContext = (HeightStructuresCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(assessmentSectionStub, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithoutParentNodeDefaultBehavior_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);
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
                guiStub.Expect(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(14, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Er zijn geen kunstwerken beschikbaar om de berekeningen te genereren.",
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.Add(new TestHeightStructure());
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(14, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextGenerateCalculationsIndexRootGroup,
                                                                  "Genereer &berekeningen...",
                                                                  "Genereer berekeningen op basis van geselecteerde kunstwerken.",
                                                                  RingtoetsCommonFormsResources.GenerateScenariosIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NestedCalculationGroup_CallsContextMenuBuilderMethods()
        {
            // Setup
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);
            var parentGroupContext = new HeightStructuresCalculationGroupContext(parentGroup,
                                                                                 failureMechanism,
                                                                                 assessmentSectionStub);
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

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(cmp => cmp.Get(groupContext, treeViewControl)).Return(menuBuilderMock);

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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);
            var parentGroupContext = new HeightStructuresCalculationGroupContext(parentGroup,
                                                                                 failureMechanism,
                                                                                 assessmentSectionStub);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(groupContext, parentGroupContext, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(13, menu.Items.Count);

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
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();

            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen vakindeling geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();

            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

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
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndexRootGroup];

                    Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                    StringAssert.Contains(string.Format("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. {0}", ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndexRootGroup,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen deze berekeningsmap uit.",
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();

            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er is geen vakindeling geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();

            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemValidateAllDisabledAndTooltipSet()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuValidateAllIndexRootGroup];

                    Assert.AreEqual("Alles &valideren", contextMenuItem.Text);
                    StringAssert.Contains(string.Format("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. {0}", ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicDatabaseSet_ContextMenuItemValidateAllEnabled()
        {
            // Setup
            var group = new CalculationGroup
            {
                Children =
                {
                    new HeightStructuresCalculation()
                }
            };

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));
            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation());

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndexRootGroup,
                                                                  "Alles &valideren",
                                                                  "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                                  RingtoetsCommonFormsResources.ValidateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var mainWindowStub = mocks.Stub<IMainWindow>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new HeightStructuresFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2)
            }));

            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    Structure = new TestHeightStructure()
                }
            });

            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2),
                    Structure = new TestHeightStructure()
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var groupContext = new HeightStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiStub.Expect(g => g.MainWindow).Return(mainWindowStub);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;

                plugin.Gui = guiStub;

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                {
                    // Call
                    TestHelper.AssertLogMessages(() => contextMenu.Items[contextMenuCalculateAllIndexRootGroup].PerformClick(), messages =>
                    {
                        var messageList = messages.ToList();

                        // Assert
                        Assert.AreEqual(14, messageList.Count);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messageList[0]);
                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messageList[1]);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messageList[2]);
                        Assert.AreEqual("De berekening voor hoogte kunstwerk 'A' is niet gelukt.", messageList[3]);
                        StringAssert.StartsWith("Hoogte kunstwerken berekeningsverslag. Klik op details voor meer informatie.", messageList[4]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messageList[5]);
                        StringAssert.StartsWith("Validatie van 'B' gestart om: ", messageList[6]);
                        StringAssert.StartsWith("Validatie van 'B' beëindigd om: ", messageList[7]);
                        StringAssert.StartsWith("Berekening van 'B' gestart om: ", messageList[8]);
                        Assert.AreEqual("De berekening voor hoogte kunstwerk 'B' is niet gelukt.", messageList[9]);
                        StringAssert.StartsWith("Hoogte kunstwerken berekeningsverslag. Klik op details voor meer informatie.", messageList[10]);
                        StringAssert.StartsWith("Berekening van 'B' beëindigd om: ", messageList[11]);
                        Assert.AreEqual("Uitvoeren van 'A' is mislukt.", messageList[12]);
                        Assert.AreEqual("Uitvoeren van 'B' is mislukt.", messageList[13]);
                    });
                }
            }
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new HeightStructuresFailureMechanism();

            failureMechanism.AddSection(new FailureMechanismSection("A", new[]
            {
                new Point2D(0, 0)
            }));

            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Name = "A",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });

            failureMechanism.CalculationsGroup.Children.Add(new HeightStructuresCalculation
            {
                Name = "B",
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "nonExisting", 1, 2)
                }
            });

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabaseStub = mocks.Stub<HydraulicBoundaryDatabase>();
            hydraulicBoundaryDatabaseStub.FilePath = validFilePath;

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var groupContext = new HeightStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(g => g.Get(groupContext, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                assessmentSectionStub.HydraulicBoundaryDatabase = hydraulicBoundaryDatabaseStub;

                plugin.Gui = guiStub;

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
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);
            var calculationGroup = new CalculationGroup
            {
                Name = "Nieuwe map"
            };

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                group.Children.Add(calculationGroup);
                nodeData.Attach(observerMock);

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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);
            var calculation = new HeightStructuresCalculation
            {
                Name = "Nieuwe berekening"
            };
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            using (var treeViewControl = new TreeViewControl())
            {
                guiStub.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                group.Children.Add(calculation);
                nodeData.Attach(observerMock);

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Precondition
                    Assert.AreEqual(1, group.Children.Count);

                    // Call
                    contextMenu.Items[contextMenuAddCalculationIndexRootGroup].PerformClick();

                    // Assert
                    Assert.AreEqual(2, group.Children.Count);
                    var newlyAddedItem = group.Children.Last();
                    Assert.IsInstanceOf<HeightStructuresCalculation>(newlyAddedItem);
                    Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                                    "An item with the same name default name already exists, therefore '(1)' needs to be appended.");
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenDikeProfileSelectedAndDialogClosed_ThenCalculationsAddedWithProfileAssigned()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                HeightStructure structure1 = new TestHeightStructure("Structure 1");
                HeightStructure structure2 = new TestHeightStructure("Structure 2");

                var failureMechanism = new HeightStructuresFailureMechanism
                {
                    HeightStructures =
                    {
                        structure1,
                        structure2
                    }
                };

                var nodeData = new HeightStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                           failureMechanism,
                                                                           assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                guiStub.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Expect(g => g.MainWindow).Return(mainWindow);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiStub;

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
                    contextMenu.Items[contextGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    var HeightStructuresCalculations = failureMechanism.Calculations.OfType<HeightStructuresCalculation>().ToArray();
                    Assert.AreEqual(1, HeightStructuresCalculations.Length);
                }
            }
        }

        [Test]
        public void GivenCalculationsViewGenerateScenariosButtonClicked_WhenCancelButtonClickedAndDialogClosed_ThenCalculationsNotUpdated()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                HeightStructure structure1 = new TestHeightStructure("Structure 1");
                HeightStructure structure2 = new TestHeightStructure("Structure 2");

                var failureMechanism = new HeightStructuresFailureMechanism
                {
                    HeightStructures =
                    {
                        structure1,
                        structure2
                    }
                };

                var nodeData = new HeightStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                           failureMechanism,
                                                                           assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                guiStub.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Expect(g => g.MainWindow).Return(mainWindow);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiStub;

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
                    contextMenu.Items[contextGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    Assert.AreEqual(0, failureMechanism.Calculations.OfType<HeightStructuresCalculation>().Count());
                }
            }
        }

        [Test]
        public void GivenScenariosWithExistingCalculationWithSameName_WhenOkButtonClickedAndDialogClosed_ThenCalculationWithUniqueNameAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

                var existingCalculationName = "Height structure";
                HeightStructure heightStructure = new TestHeightStructure(existingCalculationName);

                var failureMechanism = new HeightStructuresFailureMechanism
                {
                    HeightStructures =
                    {
                        heightStructure
                    },
                    CalculationsGroup =
                    {
                        Children =
                        {
                            new HeightStructuresCalculation
                            {
                                Name = existingCalculationName
                            }
                        }
                    }
                };

                var nodeData = new HeightStructuresCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                           failureMechanism,
                                                                           assessmentSectionMock);

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                var mainWindow = mocks.Stub<IMainWindow>();

                guiStub.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                guiStub.Expect(g => g.MainWindow).Return(mainWindow);
                guiStub.Stub(cmp => cmp.ViewCommands).Return(mocks.Stub<IViewCommands>());

                mocks.ReplayAll();

                plugin.Gui = guiStub;

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
                    contextMenu.Items[contextGenerateCalculationsIndexRootGroup].PerformClick();

                    // Then
                    var heightStructuresCalculations = failureMechanism.Calculations.OfType<HeightStructuresCalculation>().ToArray();
                    Assert.AreEqual(2, heightStructuresCalculations.Length);
                    Assert.AreEqual(expectedNewName, heightStructuresCalculations[1].Name);
                }
            }
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);
            var parentNodeData = new HeightStructuresCalculationGroupContext(parentGroup,
                                                                             failureMechanism,
                                                                             assessmentSectionStub);

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            parentGroup.Children.Add(group);
            parentNodeData.Attach(observerMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
        }

        [Test]
        public void OnNodeRemoved_NestedCalculationGroupContainingCalculations_RemoveGroupAndCalculationsAndNotifyObservers()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var group = new CalculationGroup();
            var parentGroup = new CalculationGroup();
            var nodeData = new HeightStructuresCalculationGroupContext(group,
                                                                       failureMechanism,
                                                                       assessmentSectionStub);
            var parentNodeData = new HeightStructuresCalculationGroupContext(parentGroup,
                                                                             failureMechanism,
                                                                             assessmentSectionStub);
            var calculation = new HeightStructuresCalculation();

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(calculation);
            parentGroup.Children.Add(group);
            parentNodeData.Attach(observerMock);

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