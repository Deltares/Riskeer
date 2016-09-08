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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveHeightLocationsContextTreeNodeInfoTest
    {
        private const int contextMenuRunWaveHeightCalculationsIndex = 0;
        private MockRepository mockRepository;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext), info.TagType);

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
        }

        [Test]
        public void Text_Always_ReturnName()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                assessmentSectionMock);

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                string nodeText = info.Text(context);

                // Assert
                Assert.AreEqual("Golfhoogtes bij doorsnede-eis", nodeText);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnGenericInputOutputIcon()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                assessmentSectionMock);
            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image icon = info.Image(context);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, icon);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                        assessmentSectionMock);

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
                    using (info.ContextMenuStrip(context, null, treeViewControl)) {}
                }
            }

            // Assert
            mockRepository.VerifyAll();
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
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                        assessmentSectionMock);

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
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        Assert.AreEqual(3, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      0,
                                                                      RingtoetsCommonFormsResources.Calculate_all,
                                                                      Resources.GrassCoverErosionOutwards_WaveHeight_No_HRD_To_Calculate,
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon,
                                                                      false);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      2,
                                                                      CoreCommonGuiResources.Properties,
                                                                      CoreCommonGuiResources.Properties_ToolTip,
                                                                      CoreCommonGuiResources.PropertiesHS,
                                                                      false);

                        CollectionAssert.AllItemsAreInstancesOfType(new[]
                        {
                            menu.Items[1],
                        }, typeof(ToolStripSeparator));
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithHydraulicDatabase_CalculateAllAndTooltipEnabled()
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
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                        new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                        assessmentSectionMock);

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
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // Assert
                        Assert.AreEqual(3, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      0,
                                                                      RingtoetsCommonFormsResources.Calculate_all,
                                                                      Resources.GrassCoverErosionOutwards_WaveHeight_Calculate_All_ToolTip,
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon);
                        TestHelper.AssertContextMenuStripContainsItem(menu,
                                                                      2,
                                                                      CoreCommonGuiResources.Properties,
                                                                      CoreCommonGuiResources.Properties_ToolTip,
                                                                      CoreCommonGuiResources.PropertiesHS,
                                                                      false);

                        CollectionAssert.AllItemsAreInstancesOfType(new[]
                        {
                            menu.Items[1],
                        }, typeof(ToolStripSeparator));
                    }
                }
            }
            mockRepository.VerifyAll();
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
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(locations, assessmentSectionMock);

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoLocationsData_ReturnGrayText()
        {
            // Setup
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(
                new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>(),
                assessmentSectionMock);

            using (var plugin = new GrassCoverErosionOutwardsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                Color color = info.ForeColor(context);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void GivenHydraulicBoundaryLocationThatFails_CalculatingWaveHeightFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var guiMock = mockRepository.DynamicMock<IGui>();
            var location = new HydraulicBoundaryLocation(1, "HydraulicBoundaryLocation", 1.1, 2.2);
            var assessmentSectionMock = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionMock.Expect(a => a.Id).Return("Id");
            assessmentSectionMock.Expect(a => a.GetFailureMechanisms()).Return(new[]
            {
                new GrassCoverErosionOutwardsFailureMechanism
                {
                    Contribution = 1
                }
            });
            assessmentSectionMock.Expect(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 300));
            assessmentSectionMock.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "HRD ijsselmeer.sqlite"),
                Locations =
                {
                    location
                }
            };

            var grassCoverErosionOutwardsHydraulicBoundaryLocation = new GrassCoverErosionOutwardsHydraulicBoundaryLocation(location);
            var context = new GrassCoverErosionOutwardsWaveHeightLocationsContext(new ObservableList<GrassCoverErosionOutwardsHydraulicBoundaryLocation>
            {
                grassCoverErosionOutwardsHydraulicBoundaryLocation
            }, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                using (var plugin = new GrassCoverErosionOutwardsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    guiMock.Expect(g => g.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                    guiMock.Expect(g => g.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                    mockRepository.ReplayAll();

                    plugin.Gui = guiMock;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        using (new WaveHeightCalculationServiceConfig())
                        {
                            var testService = (TestHydraulicBoundaryLocationCalculationService) WaveHeightCalculationService.Instance;
                            testService.CalculationConvergenceOutput = CalculationConvergence.NotCalculated;

                            // When
                            contextMenuAdapter.Items[contextMenuRunWaveHeightCalculationsIndex].PerformClick();

                            // Then
                            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider>(testService.MessageProvider);
                            Assert.AreEqual(CalculationConvergence.NotCalculated, grassCoverErosionOutwardsHydraulicBoundaryLocation.WaveHeightCalculationConvergence);
                        }
                    }
                }
            }
            mockRepository.VerifyAll();
        }

        private static TreeNodeInfo GetInfo(GrassCoverErosionOutwardsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveHeightLocationsContext));
        }
    }
}