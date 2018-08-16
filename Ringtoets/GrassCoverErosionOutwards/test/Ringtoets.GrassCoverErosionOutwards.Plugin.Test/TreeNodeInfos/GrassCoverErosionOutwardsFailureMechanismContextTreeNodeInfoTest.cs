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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Plugin.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuRelevancyIndexWhenRelevant = 2;
        private const int contextMenuRelevancyIndexWhenNotRelevant = 0;
        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsFailureMechanismContext));
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            string nodeText = info.Text(failureMechanismContext);

            // Assert
            Assert.AreEqual(failureMechanism.Name, nodeText);
        }

        [Test]
        public void ForeColor_FailureMechanismIsRelevant_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = true
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            Color foreColor = info.ForeColor(failureMechanismContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), foreColor);
        }

        [Test]
        public void ForeColor_FailureMechanismIsNotRelevant_ReturnGrayText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            Color foreColor = info.ForeColor(failureMechanismContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), foreColor);
        }

        [Test]
        public void Image_Always_ReturnFailureMechanismIcon()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            Image icon = info.Image(failureMechanismContext);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismIcon, icon);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = true
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(3, inputsFolder.Contents.Count());
            var failureMechanismSectionsContext = (GrassCoverErosionOutwardsFailureMechanismSectionsContext) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism, failureMechanismSectionsContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismSectionsContext.AssessmentSection);

            var foreshoreProfilesContext = (ForeshoreProfilesContext) inputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, foreshoreProfilesContext.WrappedData);
            Assert.AreSame(failureMechanism, foreshoreProfilesContext.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, foreshoreProfilesContext.ParentAssessmentSection);

            var inputComment = (Comment) inputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism.InputComments, inputComment);

            var hydraulicBoundaryDatabaseContext = (GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext) children[1];
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, hydraulicBoundaryDatabaseContext.WrappedData);
            Assert.AreSame(failureMechanism, hydraulicBoundaryDatabaseContext.FailureMechanism);
            Assert.AreSame(assessmentSection, hydraulicBoundaryDatabaseContext.AssessmentSection);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Oordeel", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);

            Assert.AreEqual(3, outputsFolder.Contents.Count());
            var failureMechanismAssemblyCategoriesContext = (FailureMechanismAssemblyCategoriesContext) outputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism, failureMechanismAssemblyCategoriesContext.WrappedData);
            Assert.AreSame(assessmentSection, failureMechanismAssemblyCategoriesContext.AssessmentSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                failureMechanismAssemblyCategoriesContext.GetFailureMechanismSectionAssemblyCategoriesFunc();
                Assert.AreEqual(failureMechanism.GeneralInput.N, calculator.AssemblyCategoriesInput.N);
            }

            var failureMechanismResultsContext = (FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>)
                outputsFolder.Contents.ElementAt(1);
            Assert.AreSame(failureMechanism, failureMechanismResultsContext.FailureMechanism);
            Assert.AreSame(failureMechanism.SectionResults, failureMechanismResultsContext.WrappedData);

            var outputComment = (Comment) outputsFolder.Contents.ElementAt(2);
            Assert.AreSame(failureMechanism.OutputComments, outputComment);
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnOnlyFailureMechanismNotRelevantComments()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                IsRelevant = false
            };
            var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                               assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(failureMechanismContext).ToArray();

            // Assert
            Assert.AreEqual(1, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(failureMechanism.NotRelevantComments, comment);
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = true
                };
                var assessmentSection = new AssessmentSectionStub();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                                   assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
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
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_CallsContextMenuBuilderMethods()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            {
                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = false
                };
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(failureMechanismContext, null, treeViewControl);
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

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            });

            var nodeData = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism, assessmentSection);
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
                                                                  RingtoetsCommonFormsResources.CalculateAllIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevantAndClickOnIsRelevantItem_MakeFailureMechanismNotRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = new AssessmentSectionStub();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                                   assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenRelevant].PerformClick();

                    // Assert
                    Assert.IsFalse(failureMechanism.IsRelevant);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevantAndClickOnIsRelevantItem_MakeFailureMechanismRelevantAndRemovesAllViewsForItem()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var failureMechanismObserver = mocks.Stub<IObserver>();
                failureMechanismObserver.Expect(o => o.UpdateObserver());

                var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
                {
                    IsRelevant = false
                };
                failureMechanism.Attach(failureMechanismObserver);

                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var failureMechanismContext = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                                                   assessmentSection);

                var viewCommands = mocks.StrictMock<IViewCommands>();
                viewCommands.Expect(vs => vs.RemoveAllViewsForItem(failureMechanismContext));

                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ViewCommands).Return(viewCommands);
                gui.Stub(g => g.Get(failureMechanismContext, treeViewControl)).Return(menuBuilder);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanismContext, null, treeViewControl))
                {
                    // Call
                    contextMenu.Items[contextMenuRelevancyIndexWhenNotRelevant].PerformClick();

                    // Assert
                    Assert.IsTrue(failureMechanism.IsRelevant);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Test");
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            failureMechanism.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            var context = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                               assessmentSection);

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
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenAllCalculationsScheduled()
        {
            // Given
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Test");
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(
                failureMechanism, assessmentSection, new[]
                {
                    hydraulicBoundaryLocation
                });

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = CreateValidCalculation(hydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);

            var context = new GrassCoverErosionOutwardsFailureMechanismContext(failureMechanism,
                                                                               assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                var designWaterLevelCalculator = new TestDesignWaterLevelCalculator
                {
                    Converged = false,
                    DesignWaterLevel = 2.0
                };
                var waveHeightCalculator = new TestWaveHeightCalculator
                {
                    Converged = false
                };
                var waveConditionsCalculator = new TestWaveConditionsCosineCalculator
                {
                    Converged = false
                };

                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(testDataPath, string.Empty)).Return(designWaterLevelCalculator).Repeat.Times(5);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, string.Empty)).Return(waveHeightCalculator).Repeat.Times(5);
                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath, string.Empty)).Return(waveConditionsCalculator).Repeat.Times(3);
                mocks.ReplayAll();

                plugin.Gui = gui;
                plugin.Activate();

                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    // When
                    Action call = () => contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, messages =>
                    {
                        string[] msgs = messages.ToArray();
                        Assert.AreEqual(95, msgs.Length);

                        const string designWaterLevelName = "Waterstand";
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "Iv->IIv", msgs, 0);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "IIv->IIIv", msgs, 8);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "IIIv->IVv", msgs, 16);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "IVv->Vv", msgs, 24);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, designWaterLevelName, "Vv->VIv", msgs, 32);

                        const string waveHeightName = "Golfhoogte";
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "Iv->IIv", msgs, 40);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "IIv->IIIv", msgs, 48);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "IIIv->IVv", msgs, 56);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "IVv->Vv", msgs, 64);
                        HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                            hydraulicBoundaryLocation.Name, waveHeightName, "Vv->VIv", msgs, 72);

                        Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gestart.", msgs.ElementAt(80));
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs.ElementAt(81));
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs.ElementAt(82));
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs.ElementAt(83));

                        IEnumerable<RoundedDouble> waterLevels = calculation.InputParameters.GetWaterLevels(
                            failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.Single().Output.Result);
                        Assert.AreEqual(3, waterLevels.Count());
                        AssertWaveConditionsCalculationMessages(msgs, 84, waterLevels.First());
                        AssertWaveConditionsCalculationMessages(msgs, 87, waterLevels.ElementAt(1));
                        AssertWaveConditionsCalculationMessages(msgs, 90, waterLevels.ElementAt(2));
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs.ElementAt(93));
                        Assert.AreEqual($"Golfcondities berekenen voor '{calculation.Name}' is gelukt.", msgs.ElementAt(94));
                    });
                }
            }
        }

        private static void AssertWaveConditionsCalculationMessages(IEnumerable<string> messages, int startIndex, RoundedDouble waterLevel)
        {
            Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is gestart.", messages.ElementAt(startIndex));
            Assert.AreEqual("Golfcondities berekening is uitgevoerd op de tijdelijke locatie ''. " +
                            "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.",
                            messages.ElementAt(startIndex + 1));
            Assert.AreEqual($"Berekening voor waterstand '{waterLevel}' is beëindigd.", messages.ElementAt(startIndex + 2));
        }

        private static GrassCoverErosionOutwardsWaveConditionsCalculation CreateValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
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