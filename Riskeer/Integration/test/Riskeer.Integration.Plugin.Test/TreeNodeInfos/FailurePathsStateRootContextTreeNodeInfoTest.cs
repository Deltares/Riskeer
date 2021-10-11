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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Forms.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HeightStructures.Forms.PresentationObjects;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RiskeerIntegrationFormsResources = Riskeer.Integration.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class FailurePathsStateRootContextTreeNodeInfoTest
    {
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

            var context = new FailurePathsStateRootContext(assessmentSection);

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
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new FailurePathsStateRootContext(assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(3, objects.Length);

                var genericFailurePathsFolder = (CategoryTreeFolder) objects[0];
                Assert.AreEqual("Generieke faalpaden", genericFailurePathsFolder.Name);
                Assert.AreEqual(TreeFolderCategory.General, genericFailurePathsFolder.Category);

                Assert.AreEqual(18, genericFailurePathsFolder.Contents.Count());
                var pipingFailurePathContext = (PipingFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(0);
                Assert.AreSame(assessmentSection.Piping, pipingFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingFailurePathContext.Parent);

                var grassCoverErosionInwardsFailurePathContext = (GrassCoverErosionInwardsFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(1);
                Assert.AreSame(assessmentSection.GrassCoverErosionInwards, grassCoverErosionInwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionInwardsFailurePathContext.Parent);

                var macroStabilityInwardsFailurePathContext = (MacroStabilityInwardsFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(2);
                Assert.AreSame(assessmentSection.MacroStabilityInwards, macroStabilityInwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, macroStabilityInwardsFailurePathContext.Parent);

                var macroStabilityOutwardsFailurePathContext = (MacroStabilityOutwardsFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(3);
                Assert.AreSame(assessmentSection.MacroStabilityOutwards, macroStabilityOutwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, macroStabilityOutwardsFailurePathContext.Parent);

                var microStabilityFailurePathContext = (MicrostabilityFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(4);
                Assert.AreSame(assessmentSection.Microstability, microStabilityFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, microStabilityFailurePathContext.Parent);

                var stabilityStoneCoverFailurePathContext = (StabilityStoneCoverFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(5);
                Assert.AreSame(assessmentSection.StabilityStoneCover, stabilityStoneCoverFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailurePathContext.Parent);

                var waveImpactAsphaltCoverFailurePathContext = (WaveImpactAsphaltCoverFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(6);
                Assert.AreSame(assessmentSection.WaveImpactAsphaltCover, waveImpactAsphaltCoverFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailurePathContext.Parent);

                var waterPressureAsphaltCoverFailurePathContext = (WaterPressureAsphaltCoverFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(7);
                Assert.AreSame(assessmentSection.WaterPressureAsphaltCover, waterPressureAsphaltCoverFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, waterPressureAsphaltCoverFailurePathContext.Parent);

                var grassCoverErosionOutwardsFailurePathContext = (GrassCoverErosionOutwardsFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(8);
                Assert.AreSame(assessmentSection.GrassCoverErosionOutwards, grassCoverErosionOutwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailurePathContext.Parent);

                var grassCoverSlipOffOutwardsFailurePathContext = (GrassCoverSlipOffOutwardsFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(9);
                Assert.AreSame(assessmentSection.GrassCoverSlipOffOutwards, grassCoverSlipOffOutwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffOutwardsFailurePathContext.Parent);

                var grassCoverSlipOffInwardsFailurePathContext = (GrassCoverSlipOffInwardsFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(10);
                Assert.AreSame(assessmentSection.GrassCoverSlipOffInwards, grassCoverSlipOffInwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffInwardsFailurePathContext.Parent);

                var heightStructuresFailurePathContext = (HeightStructuresFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(11);
                Assert.AreSame(assessmentSection.HeightStructures, heightStructuresFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, heightStructuresFailurePathContext.Parent);

                var closingStructuresFailurePathContext = (ClosingStructuresFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(12);
                Assert.AreSame(assessmentSection.ClosingStructures, closingStructuresFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, closingStructuresFailurePathContext.Parent);

                var pipingStructureFailurePathContext = (PipingStructureFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(13);
                Assert.AreSame(assessmentSection.PipingStructure, pipingStructureFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingStructureFailurePathContext.Parent);

                var stabilityPointStructuresFailurePathContext = (StabilityPointStructuresFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(14);
                Assert.AreSame(assessmentSection.StabilityPointStructures, stabilityPointStructuresFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityPointStructuresFailurePathContext.Parent);

                var strengthStabilityLengthwiseConstructionFailurePathContext = (StrengthStabilityLengthwiseConstructionFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(15);
                Assert.AreSame(assessmentSection.StrengthStabilityLengthwiseConstruction, strengthStabilityLengthwiseConstructionFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, strengthStabilityLengthwiseConstructionFailurePathContext.Parent);

                var duneErosionFailurePathContext = (DuneErosionFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(16);
                Assert.AreSame(assessmentSection.DuneErosion, duneErosionFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailurePathContext.Parent);

                var technicalInnovationFailurePathContext = (TechnicalInnovationFailurePathContext) genericFailurePathsFolder.Contents.ElementAt(17);
                Assert.AreSame(assessmentSection.TechnicalInnovation, technicalInnovationFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, technicalInnovationFailurePathContext.Parent);

                var specificFailurePathsFolder = (CategoryTreeFolder) objects[1];
                Assert.AreEqual("Specifieke faalpaden", specificFailurePathsFolder.Name);
                CollectionAssert.IsEmpty(specificFailurePathsFolder.Contents);
                
                var assemblyResultsContext = (AssemblyResultsContext) objects[2];
                Assert.AreSame(assessmentSection, assemblyResultsContext.WrappedData);
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
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilder);
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
            var context = new FailurePathsStateRootContext(assessmentSection);
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
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(FailurePathsStateRootContext));
        }
    }
}