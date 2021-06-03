// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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
using Rhino.Mocks;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerIntegrationFormsResources = Riskeer.Integration.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class CalculationsStateRootContextTreeNodeInfoTest
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

            var context = new CalculationsStateRootContext(assessmentSection);

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
            var context = new CalculationsStateRootContext(assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                object[] objects = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(11, objects.Length);

                var hydraulicBoundaryDatabaseContext = (HydraulicBoundaryDatabaseContext) objects[0];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, hydraulicBoundaryDatabaseContext.WrappedData);
                Assert.AreSame(assessmentSection, hydraulicBoundaryDatabaseContext.AssessmentSection);
                
                var pipingCalculationsContext = (PipingCalculationsContext) objects[1];
                Assert.AreSame(assessmentSection.Piping, pipingCalculationsContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingCalculationsContext.Parent);

                var grassCoverErosionInwardsCalculationsContext = (GrassCoverErosionInwardsCalculationsContext) objects[2];
                Assert.AreSame(assessmentSection.GrassCoverErosionInwards, grassCoverErosionInwardsCalculationsContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionInwardsCalculationsContext.Parent);

                var macroStabilityInwardsCalculationsContext = (MacroStabilityInwardsCalculationsContext) objects[3];
                Assert.AreSame(assessmentSection.MacroStabilityInwards, macroStabilityInwardsCalculationsContext.WrappedData);
                Assert.AreSame(assessmentSection, macroStabilityInwardsCalculationsContext.Parent);

                var stabilityStoneCoverFailureMechanismContext = (StabilityStoneCoverFailureMechanismContext) objects[4];
                Assert.AreSame(assessmentSection.StabilityStoneCover, stabilityStoneCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailureMechanismContext.Parent);

                var waveImpactAsphaltCoverFailureMechanismContext = (WaveImpactAsphaltCoverFailureMechanismContext) objects[5];
                Assert.AreSame(assessmentSection.WaveImpactAsphaltCover, waveImpactAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailureMechanismContext.Parent);

                var grassCoverErosionOutwardsFailureMechanismContext = (GrassCoverErosionOutwardsFailureMechanismContext) objects[6];
                Assert.AreSame(assessmentSection.GrassCoverErosionOutwards, grassCoverErosionOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailureMechanismContext.Parent);

                var heightStructuresFailureMechanismContext = (HeightStructuresFailureMechanismContext) objects[7];
                Assert.AreSame(assessmentSection.HeightStructures, heightStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, heightStructuresFailureMechanismContext.Parent);

                var closingStructuresFailureMechanismContext = (ClosingStructuresFailureMechanismContext) objects[8];
                Assert.AreSame(assessmentSection.ClosingStructures, closingStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, closingStructuresFailureMechanismContext.Parent);

                var stabilityPointStructuresFailureMechanismContext = (StabilityPointStructuresFailureMechanismContext) objects[9];
                Assert.AreSame(assessmentSection.StabilityPointStructures, stabilityPointStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityPointStructuresFailureMechanismContext.Parent);

                var duneErosionFailureMechanismContext = (DuneErosionFailureMechanismContext) objects[10];
                Assert.AreSame(assessmentSection.DuneErosion, duneErosionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailureMechanismContext.Parent);

            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                var context = new CalculationsStateRootContext(assessmentSection);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var mocks = new MockRepository();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = GetInfo(plugin).ContextMenuStrip(context, null, treeView))
                    {
                        // Assert
                        Assert.AreEqual(10, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                      "Alles be&rekenen",
                                                                      "Voer alle berekeningen binnen dit traject uit.",
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
            var mocks = new MockRepository();
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new CalculationsStateRootContext(assessmentSection);
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

            mocks.VerifyAll();
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
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(CalculationsStateRootContext));
        }
    }
}