﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using log4net.Core;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Data.TestUtil.Probabilistic;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Primitives;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin.Test.TreeNodeInfos.Probabilistic
{
    [TestFixture]
    public class ProbabilisticPipingCalculationScenarioContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuUpdateEntryAndExitPointIndex = 5;
        private const int contextMenuValidateIndex = 7;
        private const int contextMenuCalculateIndex = 8;
        private const int contextMenuClearIndex = 10;
        private const int contextMenuClearIllustrationPointsIndex = 11;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

        private MockRepository mocks;
        private PipingPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Image_Always_ReturnsPipingIcon()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            Image image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.ProbabilisticCalculationIcon, image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ChildNodeObjects_Always_ReturnsCollectionWithOutputObjects(bool hasOutput)
        {
            // Setup
            var calculation = new ProbabilisticPipingCalculationScenario
            {
                Output = hasOutput
                             ? PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                             : null
            };

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                                     new CalculationGroup(),
                                                                                                     new[]
                                                                                                     {
                                                                                                         new PipingSurfaceLine(string.Empty)
                                                                                                     },
                                                                                                     new[]
                                                                                                     {
                                                                                                         PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel()
                                                                                                     },
                                                                                                     pipingFailureMechanism,
                                                                                                     assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(pipingCalculationScenarioContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var comment = (Comment) children[0];
            Assert.AreSame(pipingCalculationScenarioContext.WrappedData.Comments, comment);

            var pipingInputContext = (ProbabilisticPipingInputContext) children[1];
            Assert.AreSame(pipingCalculationScenarioContext.WrappedData.InputParameters, pipingInputContext.WrappedData);
            CollectionAssert.AreEqual(pipingCalculationScenarioContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationScenarioContext.AvailableStochasticSoilModels, pipingInputContext.AvailableStochasticSoilModels);

            var pipingOutputContext = (ProbabilisticPipingOutputContext) children[2];
            Assert.AreSame(pipingCalculationScenarioContext.WrappedData, pipingOutputContext.WrappedData);
        }

        [Test]
        public void ContextMenuStrip_CalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                };
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Wis de uitvoer van deze berekening.",
                                                                  RiskeerCommonFormsResources.ClearIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_AllRequiredInputSet_ContextMenuItemCalculateAndValidateEnabled()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RiskeerCommonFormsResources.CalculateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(new ProbabilisticPipingCalculationScenario(),
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    Assert.AreEqual(18, contextMenu.Items.Count);

                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RiskeerCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointIndex,
                                                                  "&Bijwerken intrede- en uittredepunt...",
                                                                  "Er moet een profielschematisatie geselecteerd zijn.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuValidateIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuCalculateIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RiskeerCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIllustrationPointsIndex,
                                                                  "Wis illustratiepunten...",
                                                                  "Deze berekening heeft geen illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(new ProbabilisticPipingCalculationScenario(),
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
                using (mocks.Ordered())
                {
                    menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                    menuBuilder.Expect(mb => mb.Build()).Return(null);
                }

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }

            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutSurfaceLine_ContextMenuItemUpdateEntryAndExitPointDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointIndex,
                                                                  "&Bijwerken intrede- en uittredepunt...",
                                                                  "Er moet een profielschematisatie geselecteerd zijn.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithSurfaceLineAndInputInSync_ContextMenuItemUpdateEntryAndExitPointDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var surfaceLine = new PipingSurfaceLine(string.Empty);
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointIndex,
                                                                  "&Bijwerken intrede- en uittredepunt...",
                                                                  "Er zijn geen wijzigingen om bij te werken.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithSurfaceLineAndInputOutOfSync_ContextMenuItemUpdateEntryAndExitPointEnabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var surfaceLine = new PipingSurfaceLine(string.Empty);
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(1, 2, 3),
                    new Point3D(4, 5, 6)
                });
                var calculation = new ProbabilisticPipingCalculationScenario
                {
                    InputParameters =
                    {
                        SurfaceLine = surfaceLine
                    }
                };
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeSurfaceLine(surfaceLine);

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuUpdateEntryAndExitPointIndex,
                                                                  "&Bijwerken intrede- en uittredepunt...",
                                                                  "Berekening bijwerken met de karakteristieke punten.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithOutputAndWithoutIllustrationPoints_ContextMenuItemClearIllustrationPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithoutIllustrationPoints()
                };
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuClearIllustrationPointsIndex,
                                                                  "Wis illustratiepunten...",
                                                                  "Deze berekening heeft geen illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutOutput_ContextMenuItemClearOutputDisabledAndTooltipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  contextMenuClearIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutOutput_ContextMenuItemClearIllustrationPointsDisabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuClearIllustrationPointsIndex,
                                                                  "Wis illustratiepunten...",
                                                                  "Deze berekening heeft geen illustratiepunten om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithOutputAndIllustrationPoints_ContextMenuItemClearIllustrationPointsEnabledAndToolTipSet()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario
                {
                    Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints()
                };
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuClearIllustrationPointsIndex,
                                                                  "Wis illustratiepunten...",
                                                                  "Wis de berekende illustratiepunten van deze berekening.",
                                                                  RiskeerCommonFormsResources.ClearIllustrationPointsIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateEntryAndExitPointClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                CreateCalculationWithSurfaceLine(out ProbabilisticPipingCalculationScenario calculation, out PipingSurfaceLine surfaceLine);

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                inputObserver.Expect(obs => obs.UpdateObserver());
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                ChangeSurfaceLine(surfaceLine);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsEntryAndExitPointInputSynchronized);

                    // Note: observer assertions are verified in TearDown
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateEntryAndExitPointClickedAndCancelled_ThenInquiryAndCalculationNotUpdatedAndObserversNotNotified()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                CreateCalculationWithSurfaceLine(out ProbabilisticPipingCalculationScenario calculation, out PipingSurfaceLine surfaceLine);
                calculation.Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickCancel();
                };

                ChangeSurfaceLine(surfaceLine);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.HasOutput);
                    Assert.IsFalse(calculation.InputParameters.IsEntryAndExitPointInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in TearDown
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutputAndInputOutOfSync_WhenUpdateEntryAndExitPointClickedAndContinued_ThenInquiryAndCalculationUpdatedAndObserversNotified()
        {
            using (var treeViewControl = new TreeViewControl())
            {
                // Given
                CreateCalculationWithSurfaceLine(out ProbabilisticPipingCalculationScenario calculation, out PipingSurfaceLine surfaceLine);
                calculation.Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints();

                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                var nodeData = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                 pipingFailureMechanism,
                                                                                 assessmentSection);

                var inputObserver = mocks.StrictMock<IObserver>();
                inputObserver.Expect(obs => obs.UpdateObserver());
                calculation.InputParameters.Attach(inputObserver);

                var calculationObserver = mocks.StrictMock<IObserver>();
                calculationObserver.Expect(obs => obs.UpdateObserver());
                calculation.Attach(calculationObserver);

                var mainWindow = mocks.Stub<IMainWindow>();
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                mocks.ReplayAll();

                plugin.Gui = gui;

                string textBoxMessage = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var helper = new MessageBoxTester(wnd);
                    textBoxMessage = helper.Text;
                    helper.ClickOk();
                };

                ChangeSurfaceLine(surfaceLine);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateEntryAndExitPointIndex].PerformClick();

                    // Then
                    Assert.IsFalse(calculation.HasOutput);
                    Assert.IsTrue(calculation.InputParameters.IsEntryAndExitPointInputSynchronized);

                    string expectedMessage = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                             $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";
                    Assert.AreEqual(expectedMessage, textBoxMessage);

                    // Note: observer assertions are verified in TearDown
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var elementToBeRemoved = new ProbabilisticPipingCalculationScenario();

            var group = new CalculationGroup();
            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new ProbabilisticPipingCalculationScenario());
            group.Attach(observer);

            var pipingFailureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationContext = new ProbabilisticPipingCalculationScenarioContext(elementToBeRemoved,
                                                                                       group,
                                                                                       Enumerable.Empty<PipingSurfaceLine>(),
                                                                                       Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                       pipingFailureMechanism,
                                                                                       assessmentSection);
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 null,
                                                                 Enumerable.Empty<PipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                 pipingFailureMechanism,
                                                                 assessmentSection);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
        }

        [Test]
        public void GivenInvalidCalculation_WhenCalculatingFromContextMenu_ThenCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var failureMechanism = new PipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();
                var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                                         new CalculationGroup(),
                                                                                                         Enumerable.Empty<PipingSurfaceLine>(),
                                                                                                         Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(cmp => cmp.Get(pipingCalculationScenarioContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationScenarioContext, null, treeViewControl))
                {
                    // When
                    void Call() => contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                    {
                        Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                        string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        Assert.AreEqual(Level.Error, tupleArray[2].Item2);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is mislukt.", msgs[4]);
                    });

                    Assert.IsNull(calculation.Output);
                }
            }
        }

        [Test]
        public void GivenInvalidCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = new AssessmentSectionStub();

                var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                                         new CalculationGroup(),
                                                                                                         Enumerable.Empty<PipingSurfaceLine>(),
                                                                                                         Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                                         pipingFailureMechanism,
                                                                                                         assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(pipingCalculationScenarioContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver()).Repeat.Never();
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationScenarioContext, null, treeViewControl))
                {
                    // When
                    void Call() => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                    {
                        Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();
                        string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();

                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                        Assert.AreEqual(Level.Error, tupleArray[1].Item2);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                    });
                }
            }
        }

        [Test]
        public void GivenValidCalculation_WhenCalculatingFromContextMenu_ThenCalculationNotifiesObservers()
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSectionStub();
                var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
                PipingFailureMechanism failureMechanism = TestPipingFailureMechanism.GetFailureMechanismWithSurfaceLinesAndStochasticSoilModels();

                assessmentSection.HydraulicBoundaryDatabase.FilePath = validHydraulicBoundaryDatabaseFilePath;
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
                {
                    hydraulicBoundaryLocation
                }, true);

                var calculation = ProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<ProbabilisticPipingCalculationScenario>(
                    hydraulicBoundaryLocation);

                var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                                         new CalculationGroup(),
                                                                                                         Enumerable.Empty<PipingSurfaceLine>(),
                                                                                                         Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                                         failureMechanism,
                                                                                                         assessmentSection);

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocks);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.Get(pipingCalculationScenarioContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreatePipingCalculator(null))
                                 .IgnoreArguments()
                                 .Return(new TestPipingCalculator());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observer);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(pipingCalculationScenarioContext, null, treeViewControl))
                {
                    // When
                    void Call() => contextMenuAdapter.Items[contextMenuCalculateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Call, messages =>
                    {
                        string[] msgs = messages.ToArray();

                        Assert.AreEqual(8, msgs.Length);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gestart.", msgs[0]);
                        CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[6]);
                        Assert.AreEqual($"Uitvoeren van berekening '{calculation.Name}' is gelukt.", msgs[7]);
                    });
                    Assert.IsNotNull(calculation.Output);
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenCalculationOutputClearedAndNotified(bool confirm)
        {
            // Given
            using (var treeViewControl = new TreeViewControl())
            {
                var calculation = new ProbabilisticPipingCalculationScenario();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var assessmentSection = mocks.Stub<IAssessmentSection>();

                var pipingCalculationScenarioContext = new ProbabilisticPipingCalculationScenarioContext(calculation,
                                                                                                         new CalculationGroup(),
                                                                                                         Enumerable.Empty<PipingSurfaceLine>(),
                                                                                                         Enumerable.Empty<PipingStochasticSoilModel>(),
                                                                                                         pipingFailureMechanism,
                                                                                                         assessmentSection);

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(pipingCalculationScenarioContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());

                var observer = mocks.StrictMock<IObserver>();
                if (confirm)
                {
                    observer.Expect(o => o.UpdateObserver());
                }

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Output = PipingTestDataGenerator.GetRandomProbabilisticPipingOutputWithIllustrationPoints();
                calculation.Attach(observer);

                string messageBoxText = null, messageBoxTitle = null;
                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBoxText = messageBox.Text;
                    messageBoxTitle = messageBox.Title;
                    if (confirm)
                    {
                        messageBox.ClickOk();
                    }
                    else
                    {
                        messageBox.ClickCancel();
                    }
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(pipingCalculationScenarioContext, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuClearIndex].PerformClick();

                    // Then
                    Assert.AreNotEqual(confirm, calculation.HasOutput);
                    Assert.AreEqual("Bevestigen", messageBoxTitle);
                    Assert.AreEqual("Weet u zeker dat u de uitvoer van deze berekening wilt wissen?", messageBoxText);
                }
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new PipingPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(ProbabilisticPipingCalculationScenarioContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }

        private static void ChangeSurfaceLine(PipingSurfaceLine surfaceLine)
        {
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(2, 0, 3));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(3, 0, 0));
        }

        private static void CreateCalculationWithSurfaceLine(out ProbabilisticPipingCalculationScenario calculation, out PipingSurfaceLine surfaceLine)
        {
            surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            calculation = new ProbabilisticPipingCalculationScenario
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 1
                }
            };
        }
    }
}