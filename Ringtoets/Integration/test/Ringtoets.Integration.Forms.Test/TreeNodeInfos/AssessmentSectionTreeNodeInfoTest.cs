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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
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
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(IAssessmentSection), info.TagType);
                Assert.IsNull(info.ForeColor);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var testName = "ttt";

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Name = testName;

            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(assessmentSection);

                // Assert
                Assert.AreEqual(testName, text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(assessmentSection);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, image);
            }
            mocks.VerifyAll();
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
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var failureMechanisms = new IFailureMechanism[]
            {
                new PipingFailureMechanism(),
                new GrassCoverErosionInwardsFailureMechanism(),
                new HeightStructuresFailureMechanism(),
                new DuneErosionFailureMechanism(),
                new GrassCoverErosionOutwardsFailureMechanism(),
                new GrassCoverSlipOffInwardsFailureMechanism(),
                new GrassCoverSlipOffOutwardsFailureMechanism(),
                new MicrostabilityFailureMechanism(),
                new PipingStructureFailureMechanism(),
                new StabilityStoneCoverFailureMechanism(),
                new TechnicalInnovationFailureMechanism(),
                new StrengthStabilityLengthwiseConstructionFailureMechanism(),
                new WaterPressureAsphaltCoverFailureMechanism(),
                new WaveImpactAsphaltCoverFailureMechanism(),
                new ClosingStructureFailureMechanism(),
                new MacrostabilityInwardsFailureMechanism(),
                new MacrostabilityOutwardsFailureMechanism(),
                new StrengthStabilityPointConstructionFailureMechanism()
            };
            var contribution = new FailureMechanismContribution(failureMechanisms, 10.0, 2);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(contribution);
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                var objects = info.ChildNodeObjects(assessmentSection).ToArray();

                // Assert
                Assert.AreEqual(22, objects.Length);
                var referenceLineContext = (ReferenceLineContext) objects[0];
                Assert.AreSame(assessmentSection, referenceLineContext.WrappedData);

                var contributionContext = (FailureMechanismContributionContext) objects[1];
                Assert.AreSame(contribution, contributionContext.WrappedData);
                Assert.AreSame(assessmentSection, contributionContext.Parent);

                var context = (HydraulicBoundaryDatabaseContext) objects[2];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, context.WrappedData.HydraulicBoundaryDatabase);
                Assert.AreSame(assessmentSection, context.WrappedData);

                var commentContext = (CommentContext<ICommentable>) objects[3];
                Assert.AreSame(assessmentSection, commentContext.WrappedData);

                var pipingFailureMechanismContext = (PipingFailureMechanismContext) objects[4];
                Assert.AreSame(failureMechanisms[0], pipingFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingFailureMechanismContext.Parent);

                var grassCoverErosionInwardsFailureMechanismContext = (GrassCoverErosionInwardsFailureMechanismContext) objects[5];
                Assert.AreSame(failureMechanisms[1], grassCoverErosionInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionInwardsFailureMechanismContext.Parent);

                var heightStructuresFailureMechanismContext = (HeightStructuresFailureMechanismContext) objects[6];
                Assert.AreSame(failureMechanisms[2], heightStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, heightStructuresFailureMechanismContext.Parent);

                var duneErosionFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[7];
                Assert.AreSame(failureMechanisms[3], duneErosionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailureMechanismContext.Parent);
                var grassCoverErosionOutwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[8];
                Assert.AreSame(failureMechanisms[4], grassCoverErosionOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailureMechanismContext.Parent);
                var grassCoverSlipOffInwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[9];
                Assert.AreSame(failureMechanisms[5], grassCoverSlipOffInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffInwardsFailureMechanismContext.Parent);
                var grassCoverSlipOffOutwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[10];
                Assert.AreSame(failureMechanisms[6], grassCoverSlipOffOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffOutwardsFailureMechanismContext.Parent);
                var microstabilityFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[11];
                Assert.AreSame(failureMechanisms[7], microstabilityFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, microstabilityFailureMechanismContext.Parent);
                var pipingStructureFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[12];
                Assert.AreSame(failureMechanisms[8], pipingStructureFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingStructureFailureMechanismContext.Parent);

                var stabilityStoneCoverFailureMechanismContext = (StabilityStoneCoverFailureMechanismContext) objects[13];
                Assert.AreSame(failureMechanisms[9], stabilityStoneCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailureMechanismContext.Parent);

                var technicalInnovationFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[14];
                Assert.AreSame(failureMechanisms[10], technicalInnovationFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, technicalInnovationFailureMechanismContext.Parent);
                var strengthStabilityLengthwiseConstructionFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[15];
                Assert.AreSame(failureMechanisms[11], strengthStabilityLengthwiseConstructionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, strengthStabilityLengthwiseConstructionFailureMechanismContext.Parent);
                var waterPressureAsphaltCoverFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[16];
                Assert.AreSame(failureMechanisms[12], waterPressureAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waterPressureAsphaltCoverFailureMechanismContext.Parent);
                var waveImpactAsphaltCoverFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[17];
                Assert.AreSame(failureMechanisms[13], waveImpactAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailureMechanismContext.Parent);
                var closingStructureFailureMechnaismContext = (FailureMechanismContext<IFailureMechanism>) objects[18];
                Assert.AreSame(failureMechanisms[14], closingStructureFailureMechnaismContext.WrappedData);
                Assert.AreSame(assessmentSection, closingStructureFailureMechnaismContext.Parent);
                var macrostabilityInwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[19];
                Assert.AreSame(failureMechanisms[15], macrostabilityInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macrostabilityInwardsFailureMechanismContext.Parent);
                var macrostabilityOutwardsFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[20];
                Assert.AreSame(failureMechanisms[16], macrostabilityOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macrostabilityOutwardsFailureMechanismContext.Parent);
                var strengthStabilityPointConstructionFailureMechanismContext = (FailureMechanismContext<IFailureMechanism>) objects[21];
                Assert.AreSame(failureMechanisms[17], strengthStabilityPointConstructionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, strengthStabilityPointConstructionFailureMechanismContext.Parent);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
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
                gui.Expect(g => g.Get(null, treeViewControl)).Return(menuBuilderMock);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());

            mocks.ReplayAll();

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
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(IAssessmentSection));
        }
    }
}