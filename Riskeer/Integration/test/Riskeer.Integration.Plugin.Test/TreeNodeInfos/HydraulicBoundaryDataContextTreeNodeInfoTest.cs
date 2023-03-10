// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.Plugin;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Plugin.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDataContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuImportHydraulicBoundaryDatabaseIndex = 0;
        private const int contextMenuSelectDifferentFolderIndex = 1;
        private const int contextMenuClearIllustrationPointsIndexForNotLinkedHydraulicBoundaryData = 2;

        private MockRepository mocks;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
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
        public void Text_Always_ReturnsName()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(null);

                // Assert
                Assert.AreEqual("Hydraulische belastingen", text);
            }
        }

        [Test]
        public void Image_Always_ReturnsGeneralFolderIcon()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GeneralFolderIcon, image);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ContextMenuStrip_WithContext_CallsContextMenuBuilderMethods(bool hydraulicBoundaryDataLinked)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            if (hydraulicBoundaryDataLinked)
            {
                assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath = "hlcd.sqlite";
            }

            var context = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection);

            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddImportItem(null, null, null)).IgnoreArguments().Return(menuBuilder);
                if (hydraulicBoundaryDataLinked)
                {
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                }

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
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (var plugin = new RiskeerPlugin())
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
        [TestCase(true)]
        [TestCase(false)]
        public void ContextMenuStrip_WithContext_AddImportItem(bool hydraulicBoundaryDataLinked)
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            if (hydraulicBoundaryDataLinked)
            {
                assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath = "hlcd.sqlite";
            }

            var context = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData,
                                                           assessmentSection);

            var applicationFeatureCommands = mocks.Stub<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.Stub<IImportCommandHandler>();
            importCommandHandler.Stub(ich => ich.GetSupportedImportInfos(null)).IgnoreArguments().Return(new[]
            {
                new ImportInfo()
            });
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

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(g => g.Get(context, treeViewControl)).Return(builder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, assessmentSection, treeViewControl))
                    {
                        TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuImportHydraulicBoundaryDatabaseIndex,
                                                                      hydraulicBoundaryDataLinked
                                                                          ? "&Selecteer ander HLCD bestand..."
                                                                          : "&Selecteer HLCD bestand...",
                                                                      hydraulicBoundaryDataLinked
                                                                          ? "Selecteer een ander HLCD bestand."
                                                                          : "Selecteer een HLCD bestand.",
                                                                      RiskeerCommonFormsResources.DatabaseIcon);
                    }
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_WithLinkedHydraulicBoundaryData_AddItemSelectDifferentFolder()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            assessmentSection.HydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath = "hlcd.sqlite";

            var context = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        Assert.AreEqual(9, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuSelectDifferentFolderIndex,
                                                                      "Selecteer andere bestandsmap...",
                                                                      "Selecteer een andere bestandsmap.",
                                                                      RiskeerCommonFormsResources.GeneralFolderIcon);
                    }
                }
            }
        }

        [Test]
        public void ForeColor_HydraulicBoundaryDatabaseNotLinked_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDataContext = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData,
                                                                                assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(hydraulicBoundaryDataContext);

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
                HydraulicBoundaryData =
                {
                    FilePath = "databaseFile"
                }
            };
            var hydraulicBoundaryDataContext = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData,
                                                                                assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(hydraulicBoundaryDataContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
        }

        [Test]
        public void ChildNodeObjects_HydraulicBoundaryDatabaseNotLinked_ReturnsEmpty()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var hydraulicBoundaryDataContext = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData,
                                                                                assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDataContext).ToArray();

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
                HydraulicBoundaryData =
                {
                    FilePath = "databaseFile"
                }
            };
            var hydraulicBoundaryDataContext = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData,
                                                                                assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                object[] objects = info.ChildNodeObjects(hydraulicBoundaryDataContext).ToArray();

                // Assert
                Assert.AreEqual(1, objects.Length);

                var hydraulicBoundaryDatabasesContext = (HydraulicBoundaryDatabasesContext) objects[0];
                Assert.AreSame(assessmentSection.HydraulicBoundaryData, hydraulicBoundaryDatabasesContext.WrappedData);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithIllustrationPoints_ContextMenuItemClearAllIllustrationPointsEnabledAndTooltipSet(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getHydraulicBoundaryLocationCalculationFunc)
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            SetHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(assessmentSection);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            HydraulicBoundaryLocationCalculation calculation = getHydraulicBoundaryLocationCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            var nodeData = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuClearIllustrationPointsIndexForNotLinkedHydraulicBoundaryData];

                        Assert.AreEqual("Wis alle illustratiepunten...", contextMenuItem.Text);
                        Assert.AreEqual("Wis alle berekende illustratiepunten.", contextMenuItem.ToolTipText);
                        TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ClearIllustrationPointsIcon, contextMenuItem.Image);
                        Assert.IsTrue(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryLocationCalculationsWithoutIllustrationPoints_ContextMenuItemClearAllIllustrationPointsDisabled()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation()
            });

            var nodeData = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                mockRepository.ReplayAll();

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);

                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                    {
                        // Assert
                        ToolStripItem contextMenuItem = contextMenu.Items[contextMenuClearIllustrationPointsIndexForNotLinkedHydraulicBoundaryData];
                        Assert.IsFalse(contextMenuItem.Enabled);
                    }
                }
            }

            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndDoNotContinue_ThenInquiryAndIllustrationPointsNotCleared(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Given
            var random = new Random(21);
            AssessmentSection assessmentSection = GetAssessmentSectionWithHydraulicBoundaryLocationCalculationOutputs();
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllHydraulicLocationCalculationsWithOutput(assessmentSection).ToArray();

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickCancel();
            };

            var context = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var affectedCalculationObserver = mockRepository.StrictMock<IObserver>();

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(affectedCalculationObserver);

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndexForNotLinkedHydraulicBoundaryData].PerformClick();

                        // Then
                        const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Hydraulische belastingen' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                        Assert.IsTrue(calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetHydraulicBoundaryLocationCalculations))]
        [Apartment(ApartmentState.STA)]
        public void GivenCalculationsWithIllustrationPoints_WhenClearIllustrationPointsClickedAndContinue_ThenInquiryAndIllustrationPointsCleared(
            Func<IAssessmentSection, HydraulicBoundaryLocationCalculation> getHydraulicLocationCalculationFunc)
        {
            // Given
            var random = new Random(21);
            AssessmentSection assessmentSection = GetAssessmentSectionWithHydraulicBoundaryLocationCalculationOutputs();
            HydraulicBoundaryLocationCalculation calculation = getHydraulicLocationCalculationFunc(assessmentSection);
            calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble(), new TestGeneralResultSubMechanismIllustrationPoint());

            HydraulicBoundaryLocationCalculation[] calculationsWithOutput = GetAllHydraulicLocationCalculationsWithOutput(assessmentSection).ToArray();
            HydraulicBoundaryLocationCalculation unaffectedCalculation = calculationsWithOutput.First(calc => !calc.Output.HasGeneralResult);

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                helper.ClickOk();
            };

            var context = new HydraulicBoundaryDataContext(assessmentSection.HydraulicBoundaryData, assessmentSection);

            var mockRepository = new MockRepository();
            using (var treeViewControl = new TreeViewControl())
            {
                var affectedCalculationObserver = mockRepository.StrictMock<IObserver>();
                affectedCalculationObserver.Expect(o => o.UpdateObserver());
                var unAffectedCalculationObserver = mockRepository.StrictMock<IObserver>();

                IGui gui = StubFactory.CreateGuiStub(mocks);
                gui.Stub(cmp => cmp.MainWindow).Return(mockRepository.Stub<IMainWindow>());
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mockRepository.ReplayAll();

                calculation.Attach(affectedCalculationObserver);
                unaffectedCalculation.Attach(unAffectedCalculationObserver);

                using (var plugin = new RiskeerPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(context, null, treeViewControl))
                    {
                        // When
                        contextMenuAdapter.Items[contextMenuClearIllustrationPointsIndexForNotLinkedHydraulicBoundaryData].PerformClick();

                        // Then
                        const string expectedMessage = "Weet u zeker dat u alle berekende illustratiepunten bij 'Hydraulische belastingen' wilt wissen?";
                        Assert.AreEqual(expectedMessage, messageBoxText);

                        Assert.IsTrue(calculationsWithOutput.All(calc => calc.HasOutput));
                        Assert.IsFalse(calculation.Output.HasGeneralResult);
                    }
                }
            }

            mockRepository.VerifyAll();
        }

        private static void SetHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(IAssessmentSection assessmentSection)
        {
            assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.AddRange(
                new[]
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
                });

            assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.AddRange(
                new[]
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.025),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(0.0025)
                });
        }

        private static AssessmentSection GetAssessmentSectionWithHydraulicBoundaryLocationCalculationOutputs()
        {
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            SetHydraulicBoundaryLocationCalculationsForUserDefinedTargetProbabilities(assessmentSection);

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            });

            SetHydraulicBoundaryLocationOutput(assessmentSection.WaterLevelCalculationsForSignalFloodingProbability);
            SetHydraulicBoundaryLocationOutput(assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability);

            SetHydraulicBoundaryLocationOutput(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations);
            SetHydraulicBoundaryLocationOutput(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations);

            SetHydraulicBoundaryLocationOutput(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations);
            SetHydraulicBoundaryLocationOutput(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations);

            return assessmentSection;
        }

        private static void SetHydraulicBoundaryLocationOutput(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            var random = new Random(21);
            foreach (HydraulicBoundaryLocationCalculation calculation in calculations)
            {
                calculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
        }

        private static IEnumerable<HydraulicBoundaryLocationCalculation> GetAllHydraulicLocationCalculationsWithOutput(IAssessmentSection assessmentSection)
        {
            return assessmentSection.WaterLevelCalculationsForSignalFloodingProbability
                                    .Concat(assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability)
                                    .Concat(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.SelectMany(tp => tp.HydraulicBoundaryLocationCalculations))
                                    .Concat(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.SelectMany(tp => tp.HydraulicBoundaryLocationCalculations))
                                    .Where(calc => calc.HasOutput);
        }

        private static IEnumerable<TestCaseData> GetHydraulicBoundaryLocationCalculations()
        {
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaterLevelCalculationsForSignalFloodingProbability.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaterLevelCalculationsForMaximumAllowableFloodingProbability.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaterLevelCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaterLevelCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaveHeightCalculationsForUserDefinedTargetProbabilities[0].HydraulicBoundaryLocationCalculations.First()));
            yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                              section => section.WaveHeightCalculationsForUserDefinedTargetProbabilities[1].HydraulicBoundaryLocationCalculations.First()));
        }

        private static TreeNodeInfo GetInfo(RiskeerPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDataContext));
        }
    }
}