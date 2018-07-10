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

using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneLocationCalculationsGroupContextTreeNodeInfoTest
    {
        private const double failureMechanismSpecificNormFactor = 2.15;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNotNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNull(info.EnsureVisibleOnCreate);
                Assert.IsNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNull(info.CanRename);
                Assert.IsNull(info.OnNodeRenamed);
                Assert.IsNull(info.CanRemove);
                Assert.IsNull(info.OnNodeRemoved);
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
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Hydraulische randvoorwaarden", text);
            }
        }

        [Test]
        public void ForeColor_LocationsEmpty_ReturnGrayText()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var locations = new ObservableList<DuneLocation>();

            var calculationsGroupContext = new DuneLocationCalculationsGroupContext(locations, failureMechanism, assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color textColor = info.ForeColor(calculationsGroupContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), textColor);
            }
        }

        [Test]
        public void ForeColor_WithLocations_ReturnControlText()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var locations = new ObservableList<DuneLocation>
            {
                new TestDuneLocation()
            };

            var calculationsGroupContext = new DuneLocationCalculationsGroupContext(locations, failureMechanism, assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color textColor = info.ForeColor(calculationsGroupContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), textColor);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);

            var orderedMockRepository = new MockRepository();
            var menuBuilder = orderedMockRepository.StrictMock<IContextMenuBuilder>();
            using (orderedMockRepository.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            orderedMockRepository.ReplayAll();

            var nodeData = new DuneLocationCalculationsGroupContext(new ObservableList<DuneLocation>(),
                                                                    new DuneErosionFailureMechanism(),
                                                                    assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mockRepository.ReplayAll();

                using (var plugin = new DuneErosionPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(nodeData, null, treeViewControl);
                }
            }

            // Assert
            orderedMockRepository.VerifyAll();
            mockRepository.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_LocationsNotEmpty_ReturnsExpectedChildData()
        {
            // Setup
            const double signalingNorm = 0.002;
            const double lowerLimitNorm = 0.005;

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.LowerLimitNorm = lowerLimitNorm;
            assessmentSection.FailureMechanismContribution.SignalingNorm = signalingNorm;

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 5
            };

            var locations = new ObservableList<DuneLocation>
            {
                new TestDuneLocation()
            };
            var calculationsGroupContext = new DuneLocationCalculationsGroupContext(locations, failureMechanism, assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(calculationsGroupContext);

                // Assert
                Assert.AreEqual(5, childNodeObjects.Length);

                DuneLocationCalculationsContext[] calculationsContexts = childNodeObjects.OfType<DuneLocationCalculationsContext>().ToArray();
                Assert.AreEqual(5, calculationsContexts.Length);

                Assert.IsTrue(calculationsContexts.All(c => ReferenceEquals(assessmentSection, c.AssessmentSection)));
                Assert.IsTrue(calculationsContexts.All(c => ReferenceEquals(failureMechanism, c.FailureMechanism)));

                Assert.AreEqual("Iv->IIv", calculationsContexts[0].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, calculationsContexts[0].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, signalingNorm / 30), calculationsContexts[0].GetNormFunc(), 1e-6);

                Assert.AreEqual("IIv->IIIv", calculationsContexts[1].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, calculationsContexts[1].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, signalingNorm), calculationsContexts[1].GetNormFunc(), 1e-6);

                Assert.AreEqual("IIIv->IVv", calculationsContexts[2].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, calculationsContexts[2].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, lowerLimitNorm), calculationsContexts[2].GetNormFunc(), 1e-6);

                Assert.AreEqual("IVv->Vv", calculationsContexts[3].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForLowerLimitNorm, calculationsContexts[3].WrappedData);
                Assert.AreEqual(failureMechanismSpecificNormFactor * lowerLimitNorm, calculationsContexts[3].GetNormFunc(), 1e-6);

                Assert.AreEqual("Vv->VIv", calculationsContexts[4].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForFactorizedLowerLimitNorm, calculationsContexts[4].WrappedData);
                Assert.AreEqual(failureMechanismSpecificNormFactor * lowerLimitNorm * 30, calculationsContexts[4].GetNormFunc(), 1e-6);
            }
        }

        [Test]
        public void ChildNodeObjects_LocationsEmpty_ReturnsExpectedChildData()
        {
            // Setup
            const double signalingNorm = 0.002;
            const double lowerLimitNorm = 0.005;

            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.FailureMechanismContribution.LowerLimitNorm = lowerLimitNorm;
            assessmentSection.FailureMechanismContribution.SignalingNorm = signalingNorm;

            var locations = new ObservableList<DuneLocation>();
            var calculationsGroupContext = new DuneLocationCalculationsGroupContext(locations, new DuneErosionFailureMechanism(), assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(calculationsGroupContext);

                // Assert
                CollectionAssert.IsEmpty(childNodeObjects);
            }
        }

        private static double GetExpectedNorm(DuneErosionFailureMechanism failureMechanism, double norm)
        {
            return failureMechanismSpecificNormFactor * norm * (failureMechanism.Contribution / 100) / failureMechanism.GeneralInput.N;
        }

        private static TreeNodeInfo GetInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationCalculationsGroupContext));
        }
    }
}