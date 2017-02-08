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

using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundariesGroupContextTreeNodeInfoTest
    {
        private MockRepository mockRepository;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundariesGroupContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mockRepository.ReplayAll();

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

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            string nodeText = info.Text(context);

            // Assert
            Assert.AreEqual("Hydraulische randvoorwaarden", nodeText);
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GeneralFolderIcon, icon);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = mockRepository.Stub<IAssessmentSection>();
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

                var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
                using (mockRepository.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mockRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Done in tearDown
        }

        [Test]
        public void ChildNodeObjects_NoHydraulicBoundaryDatabaseSet_ReturnNoChildNodes()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(0, children.Length);
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseSet_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var waterLevelHydraulicBoundaryLocationsContext = (GrassCoverErosionOutwardsDesignWaterLevelLocationsContext) children[0];
            Assert.AreSame(failureMechanism.HydraulicBoundaryLocations, waterLevelHydraulicBoundaryLocationsContext.WrappedData);
            Assert.AreSame(assessmentSection, waterLevelHydraulicBoundaryLocationsContext.AssessmentSection);

            var waveHeightHydraulicBoundaryLocationsContext = (GrassCoverErosionOutwardsWaveHeightLocationsContext) children[1];
            Assert.AreSame(failureMechanism.HydraulicBoundaryLocations, waveHeightHydraulicBoundaryLocationsContext.WrappedData);
            Assert.AreSame(assessmentSection, waveHeightHydraulicBoundaryLocationsContext.AssessmentSection);

            var waveConditionsCalculationGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) children[2];
            Assert.AreSame(failureMechanism.WaveConditionsCalculationGroup, waveConditionsCalculationGroupContext.WrappedData);
        }

        [Test]
        public void ForeColor_NoHydraulicBoundaryDatabaseSet_ReturnGrayText()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseSet_ReturnControlColor()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, assessmentSection);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
        }
    }
}