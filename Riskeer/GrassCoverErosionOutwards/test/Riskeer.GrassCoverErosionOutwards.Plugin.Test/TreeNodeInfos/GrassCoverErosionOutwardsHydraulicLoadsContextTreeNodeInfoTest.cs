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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Service.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicLoadsContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 2;
        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "HydraulicBoundaryDatabase");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsHydraulicLoadsContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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

        [Test]
        public void Text_WithContext_ReturnName()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);

            // Call
            string nodeText = info.Text(context);

            // Assert
            Assert.AreEqual(failureMechanism.Name, nodeText);
        }

        [Test]
        public void ForeColor_Always_ReturnControlText()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Color foreColor = info.ForeColor(null);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);

            // Call
            Image icon = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.FailureMechanismIcon, icon);
        }

        [Test]
        public void ChildNodeObjects_WithContext_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(2, inputsFolder.Contents.Count());
            var foreshoreProfilesContext = (ForeshoreProfilesContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, foreshoreProfilesContext.WrappedData);
            Assert.AreSame(failureMechanism, foreshoreProfilesContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, foreshoreProfilesContext.ParentAssessmentSection);

            var inputComment = (Comment) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.InAssemblyInputComments, inputComment);

            var calculationGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) children[1];
            Assert.AreSame(failureMechanism.WaveConditionsCalculationGroup, calculationGroupContext.WrappedData);
            Assert.IsNull(calculationGroupContext.Parent);
            Assert.AreSame(failureMechanism, calculationGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, calculationGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithContext_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var context = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAllEnabled()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation());

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var nodeData = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            var context = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                    Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                    StringAssert.Contains("Er is geen hydraulische belastingendatabase geïmporteerd.", contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenAllCalculationsScheduled()
        {
            // Given
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Test");
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            assessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(0).Output =
                new TestHydraulicBoundaryLocationCalculationOutput(2.0);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var context = new GrassCoverErosionOutwardsHydraulicLoadsContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                var waveConditionsCalculator = new TestWaveConditionsCosineCalculator
                {
                    Converged = false
                };

                HydraulicBoundaryCalculationSettings expectedCalculationSettings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase);
                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         expectedCalculationSettings,
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(waveConditionsCalculator)
                                 .Repeat
                                 .Times(6);
                mocks.ReplayAll();

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    void Call() => contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(28, msgs.Length);

                        Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs.ElementAt(0));
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs.ElementAt(1));
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs.ElementAt(2));
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs.ElementAt(3));

                        IEnumerable<RoundedDouble> waterLevels = calculation.InputParameters.GetWaterLevels(
                            assessmentSection.WaterLevelCalculationsForSignalingNorm.Single().Output.Result);
                        Assert.AreEqual(3, waterLevels.Count());
                        AssertWaveConditionsCalculationMessages(msgs, waterLevels, "golfoploop", 4);
                        AssertWaveConditionsCalculationMessages(msgs, waterLevels, "golfklap", 15);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs.ElementAt(26));
                        Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gelukt.", msgs.ElementAt(27));
                    });
                }
            }
        }

        private static void AssertWaveConditionsCalculationMessages(string[] logMessages, IEnumerable<RoundedDouble> waterLevels, string calculationType, int index)
        {
            Assert.AreEqual($"Berekening voor {calculationType} is gestart.", logMessages[index++]);

            foreach (RoundedDouble waterLevel in waterLevels)
            {
                Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", logMessages[index++]);
                StringAssert.StartsWith("Golfcondities berekening is uitgevoerd op de tijdelijke locatie", logMessages[index++]);
                Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", logMessages[index++]);
            }

            Assert.AreEqual($"Berekening voor {calculationType} is beëindigd.", logMessages[index]);
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = WaveConditionsInputWaterLevelType.Signaling,
                    ForeshoreProfile = new TestForeshoreProfile(true)
                    {
                        BreakWater =
                        {
                            Height = new Random(39).NextRoundedDouble()
                        }
                    },
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 2,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 2
                }
            };
        }
    }
}