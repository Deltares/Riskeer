﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Forms.PresentationObjects.RegistrationState;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.DuneErosion.Forms.PresentationObjects.RegistrationState;
using Riskeer.GrassCoverErosionInwards.Forms.PresentationObjects.RegistrationState;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects.RegistrationState;
using Riskeer.HeightStructures.Forms.PresentationObjects.RegistrationState;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects.RegistrationState;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.StabilityPointStructures.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GenericFailureMechanismsContextTreeNodeInfoTest
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
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNotNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Generieke faalmechanismen", text);
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
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
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
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
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
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new GenericFailureMechanismsContext(assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(context).ToArray();

                // Assert
                Assert.AreEqual(15, objects.Length);
                var pipingFailurePathContext = (PipingFailurePathContext) objects[0];
                Assert.AreSame(assessmentSection.Piping, pipingFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingFailurePathContext.Parent);

                var grassCoverErosionInwardsFailureMechanismContext = (GrassCoverErosionInwardsFailureMechanismContext) objects[1];
                Assert.AreSame(assessmentSection.GrassCoverErosionInwards, grassCoverErosionInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionInwardsFailureMechanismContext.Parent);

                var macroStabilityInwardsFailureMechanismContext = (MacroStabilityInwardsFailureMechanismContext) objects[2];
                Assert.AreSame(assessmentSection.MacroStabilityInwards, macroStabilityInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macroStabilityInwardsFailureMechanismContext.Parent);

                var microStabilityFailurePathContext = (MicrostabilityFailurePathContext) objects[3];
                Assert.AreSame(assessmentSection.Microstability, microStabilityFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, microStabilityFailurePathContext.Parent);

                var stabilityStoneCoverFailurePathContext = (StabilityStoneCoverFailurePathContext) objects[4];
                Assert.AreSame(assessmentSection.StabilityStoneCover, stabilityStoneCoverFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailurePathContext.Parent);

                var waveImpactAsphaltCoverFailurePathContext = (WaveImpactAsphaltCoverFailurePathContext) objects[5];
                Assert.AreSame(assessmentSection.WaveImpactAsphaltCover, waveImpactAsphaltCoverFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailurePathContext.Parent);

                var waterPressureAsphaltCoverFailurePathContext = (WaterPressureAsphaltCoverFailurePathContext) objects[6];
                Assert.AreSame(assessmentSection.WaterPressureAsphaltCover, waterPressureAsphaltCoverFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, waterPressureAsphaltCoverFailurePathContext.Parent);

                var grassCoverErosionOutwardsFailureMechanismContext = (GrassCoverErosionOutwardsFailureMechanismContext) objects[7];
                Assert.AreSame(assessmentSection.GrassCoverErosionOutwards, grassCoverErosionOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailureMechanismContext.Parent);

                var grassCoverSlipOffOutwardsFailurePathContext = (GrassCoverSlipOffOutwardsFailurePathContext) objects[8];
                Assert.AreSame(assessmentSection.GrassCoverSlipOffOutwards, grassCoverSlipOffOutwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffOutwardsFailurePathContext.Parent);

                var grassCoverSlipOffInwardsFailurePathContext = (GrassCoverSlipOffInwardsFailurePathContext) objects[9];
                Assert.AreSame(assessmentSection.GrassCoverSlipOffInwards, grassCoverSlipOffInwardsFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffInwardsFailurePathContext.Parent);

                var heightStructuresFailureMechanismContext = (HeightStructuresFailureMechanismContext) objects[10];
                Assert.AreSame(assessmentSection.HeightStructures, heightStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, heightStructuresFailureMechanismContext.Parent);

                var closingStructuresFailureMechanismContext = (ClosingStructuresFailureMechanismContext) objects[11];
                Assert.AreSame(assessmentSection.ClosingStructures, closingStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, closingStructuresFailureMechanismContext.Parent);

                var pipingStructureFailurePathContext = (PipingStructureFailurePathContext) objects[12];
                Assert.AreSame(assessmentSection.PipingStructure, pipingStructureFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingStructureFailurePathContext.Parent);

                var stabilityPointStructuresFailurePathContext = (StabilityPointStructuresFailurePathContext) objects[13];
                Assert.AreSame(assessmentSection.StabilityPointStructures, stabilityPointStructuresFailurePathContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityPointStructuresFailurePathContext.Parent);

                var duneErosionFailureMechanismContext = (DuneErosionFailureMechanismContext) objects[14];
                Assert.AreSame(assessmentSection.DuneErosion, duneErosionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailureMechanismContext.Parent);
            }
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GenericFailureMechanismsContext));
        }
    }
}