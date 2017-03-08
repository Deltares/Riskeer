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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Components.Gis;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class AssessmentSectionTreeNodeInfoTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

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
                Assert.IsNotNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
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
            var testName = "ttt";

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = testName
            };

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(assessmentSection);

                // Assert
                Assert.AreEqual(testName, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(assessmentSection);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, image);
            }
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var result = info.EnsureVisibleOnCreate(null, null);

                // Assert
                Assert.IsTrue(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ExpandOnCreate_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var result = info.ExpandOnCreate(null);

                // Assert
                Assert.IsTrue(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                var objects = info.ChildNodeObjects(assessmentSection).ToArray();

                // Assert
                Assert.AreEqual(23, objects.Length);
                var backgroundMapData = (BackgroundMapData) objects[0];
                Assert.AreSame(assessmentSection.BackgroundMapData2, backgroundMapData);

                var referenceLineContext = (ReferenceLineContext) objects[1];
                Assert.AreSame(assessmentSection, referenceLineContext.WrappedData);

                var contributionContext = (FailureMechanismContributionContext) objects[2];
                Assert.AreSame(assessmentSection.FailureMechanismContribution, contributionContext.WrappedData);
                Assert.AreSame(assessmentSection, contributionContext.Parent);

                var context = (HydraulicBoundaryDatabaseContext) objects[3];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, context.WrappedData.HydraulicBoundaryDatabase);
                Assert.AreSame(assessmentSection, context.WrappedData);

                var comment = (Comment) objects[4];
                Assert.AreSame(assessmentSection.Comments, comment);

                var pipingFailureMechanismContext = (PipingFailureMechanismContext) objects[5];
                Assert.AreSame(assessmentSection.PipingFailureMechanism, pipingFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingFailureMechanismContext.Parent);

                var grassCoverErosionInwardsFailureMechanismContext = (GrassCoverErosionInwardsFailureMechanismContext) objects[6];
                Assert.AreSame(assessmentSection.GrassCoverErosionInwards, grassCoverErosionInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionInwardsFailureMechanismContext.Parent);

                var macrostabilityInwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[7];
                Assert.AreSame(assessmentSection.MacrostabilityInwards, macrostabilityInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macrostabilityInwardsFailureMechanismContext.Parent);
                var macrostabilityOutwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[8];
                Assert.AreSame(assessmentSection.MacrostabilityOutwards, macrostabilityOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macrostabilityOutwardsFailureMechanismContext.Parent);
                var microstabilityFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[9];
                Assert.AreSame(assessmentSection.Microstability, microstabilityFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, microstabilityFailureMechanismContext.Parent);

                var stabilityStoneCoverFailureMechanismContext = (StabilityStoneCoverFailureMechanismContext) objects[10];
                Assert.AreSame(assessmentSection.StabilityStoneCover, stabilityStoneCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailureMechanismContext.Parent);

                var waveImpactAsphaltCoverFailureMechanismContext = (WaveImpactAsphaltCoverFailureMechanismContext) objects[11];
                Assert.AreSame(assessmentSection.WaveImpactAsphaltCover, waveImpactAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailureMechanismContext.Parent);

                var waterPressureAsphaltCoverFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[12];
                Assert.AreSame(assessmentSection.WaterPressureAsphaltCover, waterPressureAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waterPressureAsphaltCoverFailureMechanismContext.Parent);

                var grassCoverErosionOutwardsFailureMechanismContext = (GrassCoverErosionOutwardsFailureMechanismContext) objects[13];
                Assert.AreSame(assessmentSection.GrassCoverErosionOutwards, grassCoverErosionOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailureMechanismContext.Parent);

                var grassCoverSlipOffOutwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[14];
                Assert.AreSame(assessmentSection.GrassCoverSlipOffOutwards, grassCoverSlipOffOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffOutwardsFailureMechanismContext.Parent);
                var grassCoverSlipOffInwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[15];
                Assert.AreSame(assessmentSection.GrassCoverSlipOffInwards, grassCoverSlipOffInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffInwardsFailureMechanismContext.Parent);

                var heightStructuresFailureMechanismContext = (HeightStructuresFailureMechanismContext) objects[16];
                Assert.AreSame(assessmentSection.HeightStructures, heightStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, heightStructuresFailureMechanismContext.Parent);

                var closingStructuresFailureMechanismContext = (ClosingStructuresFailureMechanismContext) objects[17];
                Assert.AreSame(assessmentSection.ClosingStructures, closingStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, closingStructuresFailureMechanismContext.Parent);
                var pipingStructureFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[18];
                Assert.AreSame(assessmentSection.PipingStructure, pipingStructureFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingStructureFailureMechanismContext.Parent);
                var stabilityPointStructuresFailureMechanismContext = (StabilityPointStructuresFailureMechanismContext) objects[19];
                Assert.AreSame(assessmentSection.StabilityPointStructures, stabilityPointStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityPointStructuresFailureMechanismContext.Parent);
                var strengthStabilityLengthwiseConstructionFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[20];
                Assert.AreSame(assessmentSection.StrengthStabilityLengthwiseConstruction, strengthStabilityLengthwiseConstructionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, strengthStabilityLengthwiseConstructionFailureMechanismContext.Parent);
                var duneErosionFailureMechanismContext = (DuneErosionFailureMechanismContext) objects[21];
                Assert.AreSame(assessmentSection.DuneErosion, duneErosionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailureMechanismContext.Parent);
                var technicalInnovationFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[22];
                Assert.AreSame(assessmentSection.TechnicalInnovation, technicalInnovationFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, technicalInnovationFailureMechanismContext.Parent);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);
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
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var canRename = info.CanRename(null, null);

                // Assert
                Assert.IsTrue(canRename);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.Attach(observer);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                const string newName = "New Name";
                info.OnNodeRenamed(assessmentSection, newName);

                // Assert
                Assert.AreEqual(newName, assessmentSection.Name);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                var canRemove = info.CanRemove(null, null);

                // Assert
                Assert.IsTrue(canRemove);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void RemoveNodeData_ProjectWithAssessmentSection_ReturnTrueAndRemoveAssessmentSection()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var project = new RingtoetsProject();
            project.AssessmentSections.Add(assessmentSection);
            project.Attach(observerMock);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                info.OnNodeRemoved(assessmentSection, project);

                // Assert
                CollectionAssert.DoesNotContain(project.AssessmentSections, assessmentSection);
            }
            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(AssessmentSection));
        }
    }
}