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

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 2;

        private const double failureMechanismSpecificNormFactor = 2.15;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "HydraulicBoundaryDatabase");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

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
            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Hydraulische belastingen", text);
            }
        }

        [Test]
        public void ForeColor_LocationsEmpty_ReturnGrayText()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();
            var locations = new ObservableList<DuneLocation>();

            var calculationsGroupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(locations, failureMechanism, assessmentSection);

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

            var calculationsGroupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(locations, failureMechanism, assessmentSection);

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
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mockRepository);

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

            var nodeData = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(new ObservableList<DuneLocation>(),
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
            var calculationsGroupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(locations, failureMechanism, assessmentSection);

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

                Assert.AreEqual("Iv", calculationsContexts[0].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, calculationsContexts[0].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, signalingNorm / 30), calculationsContexts[0].GetNormFunc(), 1e-6);

                Assert.AreEqual("IIv", calculationsContexts[1].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, calculationsContexts[1].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, signalingNorm), calculationsContexts[1].GetNormFunc(), 1e-6);

                Assert.AreEqual("IIIv", calculationsContexts[2].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, calculationsContexts[2].WrappedData);
                Assert.AreEqual(GetExpectedNorm(failureMechanism, lowerLimitNorm), calculationsContexts[2].GetNormFunc(), 1e-6);

                Assert.AreEqual("IVv", calculationsContexts[3].CategoryBoundaryName);
                Assert.AreSame(failureMechanism.CalculationsForLowerLimitNorm, calculationsContexts[3].WrappedData);
                Assert.AreEqual(failureMechanismSpecificNormFactor * lowerLimitNorm, calculationsContexts[3].GetNormFunc(), 1e-6);

                Assert.AreEqual("Vv", calculationsContexts[4].CategoryBoundaryName);
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
            var calculationsGroupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(locations, new DuneErosionFailureMechanism(), assessmentSection);

            using (var plugin = new DuneErosionPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] childNodeObjects = info.ChildNodeObjects(calculationsGroupContext);

                // Assert
                CollectionAssert.IsEmpty(childNodeObjects);
            }
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseNotLinked_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var duneLocation = new TestDuneLocation("Test");
            var failureMechanism = new DuneErosionFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation
            });

            var groupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(new ObservableList<DuneLocation>(),
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);

            var mocks = new MockRepository();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new DuneErosionPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
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

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoDuneLocationsPresent_ContextMenuItemCalculateAllDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var groupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(new ObservableList<DuneLocation>(),
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);

            var mocks = new MockRepository();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new DuneErosionPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(groupContext, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateAllIndex];

                        Assert.AreEqual("Alles be&rekenen", contextMenuItem.Text);
                        StringAssert.Contains("Geen van de locaties is geschikt voor een hydraulische belastingenberekening.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.CalculateAllIcon, contextMenuItem.Image);
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mocks.VerifyAll();
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
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 5
            };

            var duneLocation = new TestDuneLocation("Test");
            failureMechanism.SetDuneLocations(new[]
            {
                duneLocation
            });

            var groupContext = new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(new ObservableList<DuneLocation>(),
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);

            var mocks = new MockRepository();

            using (var treeViewControl = new TreeViewControl())
            {
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(groupContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator
                {
                    Converged = false
                };

                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(dunesBoundaryConditionsCalculator).Repeat.Times(5);
                mocks.ReplayAll();

                using (var plugin = new DuneErosionPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;
                    plugin.Activate();

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(groupContext, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    {
                        // When
                        Action call = () => contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                        // Then
                        TestHelper.AssertLogMessages(call, messages =>
                        {
                            string[] msgs = messages.ToArray();
                            Assert.AreEqual(40, msgs.Length);

                            const string calculationTypeDisplayName = "Hydraulische belastingen";
                            const string calculationDisplayName = "Hydraulische belastingenberekening";

                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                duneLocation.Name, calculationTypeDisplayName, calculationDisplayName, "Iv", msgs, 0);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                duneLocation.Name, calculationTypeDisplayName, calculationDisplayName, "IIv", msgs, 8);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                duneLocation.Name, calculationTypeDisplayName, calculationDisplayName, "IIIv", msgs, 16);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                duneLocation.Name, calculationTypeDisplayName, calculationDisplayName, "IVv", msgs, 24);
                            HydraulicBoundaryLocationCalculationActivityLogTestHelper.AssertHydraulicBoundaryLocationCalculationMessages(
                                duneLocation.Name, calculationTypeDisplayName, calculationDisplayName, "Vv", msgs, 32);
                        });
                    }
                }
            }

            mocks.VerifyAll();
        }

        private static double GetExpectedNorm(DuneErosionFailureMechanism failureMechanism, double norm)
        {
            return failureMechanismSpecificNormFactor * norm * (failureMechanism.Contribution / 100) / failureMechanism.GeneralInput.N;
        }

        private static TreeNodeInfo GetInfo(DuneErosionPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext));
        }
    }
}