// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;
        private const int contextMenuCalculateAllIndex = 3;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");

        private MockRepository mocks;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
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
                Assert.IsNull(info.CheckedState);
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
            const string name = "Hydraulische belastingen";
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                               assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(context);

                // Assert
                Assert.AreEqual(name, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                               assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddCustomImportItem(null, null, null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
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
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(context, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomImportItem()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var context = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                               assessmentSection);

            var applicationFeatureCommands = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            importCommandHandler.Stub(ich => ich.CanImportOn(null)).IgnoreArguments().Return(true);
            var exportCommandHandler = mocks.Stub<IExportCommandHandler>();
            var updateCommandHandler = mocks.Stub<IUpdateCommandHandler>();
            var viewCommands = mocks.Stub<IViewCommands>();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommands,
                                                     importCommandHandler,
                                                     exportCommandHandler,
                                                     updateCommandHandler,
                                                     viewCommands,
                                                     context,
                                                     treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(builder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, assessmentSection, treeViewControl))
                    {
                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuImportHydraulicBoundaryDatabaseIndex,
                                                                      "&Koppel aan database...",
                                                                      "Koppel aan hydraulische belastingendatabase.",
                                                                      RingtoetsCommonFormsResources.DatabaseIcon);
                    }
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseNotLinked_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseLinked_ReturnEnabledColor()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "databaseFile"
                }
            };
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseNotLinked_ReturnsEmpty()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDatabaseContext).ToArray();

                // Assert
                Assert.AreEqual(0, objects.Length);
            }
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseLinked_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "databaseFile"
                }
            };
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase,
                                                                                        assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDatabaseContext).ToArray();

                // Assert
                Assert.AreEqual(2, objects.Length);

                var designWaterLevelCalculationsGroupContext = (DesignWaterLevelCalculationsGroupContext) objects[0];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, designWaterLevelCalculationsGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, designWaterLevelCalculationsGroupContext.AssessmentSection);

                var waveHeightCalculationsGroupContext = (WaveHeightCalculationsGroupContext) objects[1];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase.Locations, waveHeightCalculationsGroupContext.WrappedData);
                Assert.AreSame(assessmentSection, waveHeightCalculationsGroupContext.AssessmentSection);
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Er is geen hydraulische belastingendatabase geïmporteerd.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseLinkedToInvalidFile_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = "invalidFilePath"
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenAllCalculationsScheduled()
        {
            // Given
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
            
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("locationName");
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new HydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            using (var plugin = new RingtoetsPlugin())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());
                gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false
                };
                var waveHeightCalculator = new TestWaveHeightCalculator
                {
                    Converged = false
                };

                HydraulicBoundaryCalculationSettings expectedSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         expectedSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(designWaterLevelCalculator)
                                 .Repeat
                                 .Times(4);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         expectedSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 }).Return(waveHeightCalculator)
                                 .Repeat
                                 .Times(4);
                mocks.ReplayAll();

                TreeNodeInfo info = GetInfo(plugin);
                plugin.Gui = gui;
                plugin.Activate();

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    Action call = () => contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(64, msgs.Length);

                        const string designWaterLevelCalculationTypeDisplayName = "Waterstand";
                        const string designWaterLevelCalculationDisplayName = "Waterstand berekening";

                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "A+", msgs, 0);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "A", msgs, 8);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "B", msgs, 16);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelCalculationTypeDisplayName, designWaterLevelCalculationDisplayName, "C", msgs, 24);

                        const string waveHeightCalculationTypeDisplayName = "Golfhoogte";
                        const string waveHeightCalculationDisplayName = "Golfhoogte berekening";

                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "A+", msgs, 32);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "A", msgs, 40);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "B", msgs, 48);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightCalculationTypeDisplayName, waveHeightCalculationDisplayName, "C", msgs, 56);
                    });
                }
            }

            mocks.VerifyAll();
        }

        public override void Setup()
        {
            mocks = new MockRepository();
        }

        private static TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }
    }
}