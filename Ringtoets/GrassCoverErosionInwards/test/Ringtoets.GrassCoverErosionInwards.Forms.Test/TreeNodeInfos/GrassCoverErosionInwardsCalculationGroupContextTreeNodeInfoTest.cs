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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTreeNodeInfoTest
    {
        private MockRepository mocks;
        private GrassCoverErosionInwardsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculationGroupContext), info.TagType);
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
            var group = new CalculationGroup
            {
                Name = testname
            };
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanismMock,
                                                                                   assessmentSectionMock);

            // Call
            var text = info.Text(groupContext);

            // Assert
            Assert.AreEqual(testname, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_FolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.GeneralFolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanismMock,
                                                                                   assessmentSectionMock);

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
            var group = new CalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                 pipingFailureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var group = new CalculationGroup();
            var childGroup = new CalculationGroup();
            var calculationItem = mocks.StrictMock<ICalculationBase>();
            var childCalculation = new GrassCoverErosionInwardsCalculation();

            group.Children.Add(childGroup);
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);

            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                 failureMechanismMock,
                                                                 assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(groupContext).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);            
            var calculationGroupContext = (GrassCoverErosionInwardsCalculationGroupContext) children[0];
            Assert.AreSame(childGroup, calculationGroupContext.WrappedData);
            Assert.AreSame(failureMechanismMock, calculationGroupContext.GrassCoverErosionInwardsFailureMechanism);
            Assert.AreSame(assessmentSectionMock, calculationGroupContext.AssessmentSection);
            Assert.AreSame(calculationItem, children[1]);
            var calculationContext = (GrassCoverErosionInwardsCalculationContext) children[2];
            Assert.AreSame(childCalculation, calculationContext.WrappedData);
            Assert.AreSame(assessmentSectionMock, calculationContext.AssessmentSection);
        }

        [Test]
        public void ContextmenuStrip_Always_ReturnContextWithItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new CalculationGroup();

            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var parentData = new object();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                               failureMechanismMock, 
                                                                               assessmentSectionMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(nodeData)).Repeat.Twice().Return(false);
            viewCommandsHandler.Expect(vc => vc.CanOpenViewFor(nodeData)).Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            Assert.AreEqual(12, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, 0,
                                                          CoreCommonGuiResources.Open,
                                                          CoreCommonGuiResources.Open_ToolTip,
                                                          CoreCommonGuiResources.OpenIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 2,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan dit faalmechanisme.",
                                                          RingtoetsFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          "Voeg een nieuwe grasbekleding erosie kruin en binnentalud berekening toe aan dit faalmechanisme.",
                                                          GrassCoverErosionInwardsFormResources.CalculationIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 5,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesHS,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[1],
                menu.Items[4],
                menu.Items[7],
                menu.Items[10]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var calculationItem = mocks.Stub<ICalculationBase>();
            calculationItem.Name = "Nieuwe map";

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
            Assert.IsInstanceOf<CalculationGroup>(newlyAddedItem);
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
            var group = new CalculationGroup();
            var failureMechanismMock = mocks.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var nodeData = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                             failureMechanismMock,
                                                             assessmentSectionMock);

            var calculationItem = mocks.Stub<ICalculationBase>();
            calculationItem.Name = "Nieuwe berekening";

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
            contextMenu.Items[contextMenuAddCalculationItemIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<GrassCoverErosionInwardsCalculation>(newlyAddedItem);
            Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                            "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            mocks.VerifyAll();
        }

        private const int contextMenuAddCalculationGroupIndex = 1;
        private const int contextMenuAddCalculationItemIndex = 2;
    }
}