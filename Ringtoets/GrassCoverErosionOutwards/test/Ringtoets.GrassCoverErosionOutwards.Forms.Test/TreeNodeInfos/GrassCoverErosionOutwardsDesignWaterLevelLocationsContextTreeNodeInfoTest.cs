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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsContextTreeNodeInfoTest
    {
        private MockRepository mockRepository;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext));
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext), info.TagType);

            Assert.IsNull(info.EnsureVisibleOnCreate);
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
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                assessmentSectionMock,
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>());

            // Call
            string nodeText = info.Text(context);

            // Assert
            Assert.AreEqual("Waterstand bij doorsnede-eis", nodeText);
        }

        [Test]
        public void Image_Always_ReturnGenericInputOutputIcon()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                assessmentSectionMock,
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>());

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, icon);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            using (var treeViewControl = new TreeViewControl())
            {
                var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                    assessmentSectionMock,
                    new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>());

                var menuBuilder = mockRepository.StrictMock<IContextMenuBuilder>();
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);

                var gui = mockRepository.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Part of TearDown
        }

        [Test]
        public void ContextMenuStrip_WithoutHydraulicDatabase_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                    assessmentSectionMock,
                    new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>());

                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         viewCommandsHandler,
                                                         context,
                                                         treeViewControl);

                var gui = mockRepository.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl);

                // Assert
                Assert.AreEqual(3, menu.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(menu, 0, Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                                                              Resources.GrassCoverErosionOutwardsWaterLevelLocation_No_HRD_To_Calculate, RingtoetsCommonFormsResources.CalculateAllIcon, false);
                TestHelper.AssertContextMenuStripContainsItem(menu, 2, CoreCommonGuiResources.Properties,
                                                              CoreCommonGuiResources.Properties_ToolTip, CoreCommonGuiResources.PropertiesHS, false);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[1],
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ContextMenuStrip_WithHydraulicDatabase_EnabledCalculateAllAndTooltip()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var applicationFeatureCommandHandler = mockRepository.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mockRepository.Stub<IImportCommandHandler>();
            var exportCommandHandler = mockRepository.Stub<IExportCommandHandler>();
            var viewCommandsHandler = mockRepository.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(
                    assessmentSectionMock,
                    new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>());

                var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler,
                                                         importCommandHandler,
                                                         exportCommandHandler,
                                                         viewCommandsHandler,
                                                         context,
                                                         treeViewControl);

                var gui = mockRepository.StrictMock<IGui>();
                gui.Expect(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);

                mockRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl);

                // Assert
                Assert.AreEqual(3, menu.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(menu, 0, Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                                                              Resources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All_ToolTip, RingtoetsCommonFormsResources.CalculateAllIcon);

                CollectionAssert.AllItemsAreInstancesOfType(new[]
                {
                    menu.Items[1],
                }, typeof(ToolStripSeparator));
            }
        }

        [Test]
        public void ForeColor_ContextHasLocationsData_ReturnControlText()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 0, 0);
            HydraulicBoundaryDatabase database = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };
            assessmentSectionMock.HydraulicBoundaryDatabase = database;
            mockRepository.ReplayAll();
            var locations = new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                new GrassCoverErosionOutwardsHydraulicBoundaryLocation(hydraulicBoundaryLocation)
            };
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(assessmentSectionMock, locations);

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
        }

        [Test]
        public void ForeColor_ContextHasNoLocationsData_ReturnGrayText()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(assessmentSectionMock, new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>());

            // Call
            Color color = info.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
        }
    }
}