// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.DuneErosion.Forms.PresentationObjects.HydraulicLoadsState;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects.HydraulicLoadsState;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects.HydraulicLoadsState;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects.HydraulicLoadsState;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerIntegrationFormsResources = Riskeer.Integration.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicLoadsStateRootContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 4;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNotNull(info.EnsureVisibleOnCreate);
                Assert.IsNotNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNotNull(info.CanRename);
                Assert.IsNotNull(info.OnNodeRenamed);
                Assert.IsNotNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_WithContext_ReturnsName()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "ttt"
            };

            var context = new HydraulicLoadsStateRootContext(assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual(assessmentSection.Name, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerIntegrationFormsResources.AssessmentSectionFolderIcon, image);
            }
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool result = info.EnsureVisibleOnCreate(null, null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void ExpandOnCreate_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool result = info.ExpandOnCreate(null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void ChildNodeObjects_WithContext_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicLoadsStateRootContext(assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                object[] objects = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(8, objects.Length);

                var hydraulicBoundaryDataContext = (HydraulicBoundaryDataContext) objects[0];
                Assert.AreSame(assessmentSection.HydraulicBoundaryData, hydraulicBoundaryDataContext.WrappedData);
                Assert.AreSame(assessmentSection, hydraulicBoundaryDataContext.AssessmentSection);

                var waterLevelCalculationsForNormTargetProbabilitiesGroupContext = (WaterLevelCalculationsForNormTargetProbabilitiesGroupContext) objects[1];
                Assert.AreSame(assessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases, waterLevelCalculationsForNormTargetProbabilitiesGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, waterLevelCalculationsForNormTargetProbabilitiesGroupContext.AssessmentSection);

                var waterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext = (WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext) objects[2];
                Assert.AreSame(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities, waterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, waterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext.AssessmentSection);

                var waveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext = (WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext) objects[3];
                Assert.AreSame(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities, waveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, waveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext.AssessmentSection);

                var stabilityStoneCoverFailureMechanismContext = (StabilityStoneCoverFailureMechanismContext) objects[4];
                Assert.AreSame(assessmentSection.StabilityStoneCover, stabilityStoneCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailureMechanismContext.Parent);

                var waveImpactAsphaltCoverFailureMechanismContext = (WaveImpactAsphaltCoverFailureMechanismContext) objects[5];
                Assert.AreSame(assessmentSection.WaveImpactAsphaltCover, waveImpactAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailureMechanismContext.Parent);

                var grassCoverErosionOutwardsFailureMechanismContext = (GrassCoverErosionOutwardsFailureMechanismContext) objects[6];
                Assert.AreSame(assessmentSection.GrassCoverErosionOutwards, grassCoverErosionOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailureMechanismContext.Parent);

                var duneErosionFailureMechanismContext = (DuneErosionFailureMechanismContext) objects[7];
                Assert.AreSame(assessmentSection.DuneErosion, duneErosionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailureMechanismContext.Parent);
            }
        }

        [Test]
        public void ContextMenuStrip_WithContext_CallsBuilder()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicLoadsStateRootContext(assessmentSection);
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
                menuBuilder.AddOpenItem().Returns(menuBuilder);
                menuBuilder.AddSeparator().Returns(menuBuilder);
                menuBuilder.AddRenameItem().Returns(menuBuilder);
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
                menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
                menuBuilder.AddExpandAllItem().Returns(menuBuilder);
                menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            Received.InOrder(() =>
            {
                menuBuilder.AddOpenItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddRenameItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                var context = new HydraulicLoadsStateRootContext(assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
                IGui gui = StubFactory.CreateGuiStub();
                gui.Get(context, treeView).Returns(menuBuilder);
                using (var plugin = new RiskeerPlugin())
                {
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = GetInfo(plugin).ContextMenuStrip(context, null, treeView))
                    {
                        // Assert
                        Assert.AreEqual(10, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(
                            menu, contextMenuCalculateAllIndex,
                            "Alles be&rekenen",
                            "Alle hydraulische belastingen berekenen.",
                            RiskeerCommonFormsResources.CalculateAllIcon);
                    }
                }
            }
        }

        [Test]
        public void CanRename_Always_ReturnsTrue()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool canRename = info.CanRename(null, null);

                // Assert
                Assert.IsTrue(canRename);
            }
        }

        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var observer = Substitute.For<IObserver>();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicLoadsStateRootContext(assessmentSection);
            context.Attach(observer);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                const string newName = "New Name";
                info.OnNodeRenamed(context, newName);

                // Assert
                Assert.AreEqual(newName, assessmentSection.Name);
            }
            observer.Received().UpdateObserver();
        }

        [Test]
        public void CanRemove_Always_ReturnsFalse()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool canRemove = info.CanRemove(null, null);

                // Assert
                Assert.IsFalse(canRemove);
            }
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicLoadsStateRootContext));
        }
    }
}