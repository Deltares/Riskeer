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
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
    public class DuneErosionHydraulicLoadsContextTreeNodeInfoTest
    {
        private const int contextMenuCalculateAllIndex = 2;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Forms, "HydraulicBoundaryDatabase");

        private MockRepository mocksRepository;
        private DuneErosionPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocksRepository = new MockRepository();
            plugin = new DuneErosionPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DuneErosionHydraulicLoadsContext));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();

            mocksRepository.VerifyAll();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocksRepository.ReplayAll();

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
        public void ChildNodeObjects_WithContext_ReturnChildDataNodes()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            var failureMechanism = new DuneErosionFailureMechanism();
            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context).ToArray();

            // Assert
            Assert.AreEqual(2, children.Length);

            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);

            Assert.AreEqual(1, inputsFolder.Contents.Count());
            var inputComment = (Comment) inputsFolder.Contents.ElementAt(0);
            Assert.AreSame(failureMechanism.InputComments, inputComment);

            var duneLocationCalculationsGroupContext = (DuneLocationCalculationsGroupContext) children[1];
            Assert.AreSame(failureMechanism.DuneLocations, duneLocationCalculationsGroupContext.WrappedData);
            Assert.AreSame(failureMechanism, duneLocationCalculationsGroupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, duneLocationCalculationsGroupContext.AssessmentSection);
        }

        [Test]
        public void ContextMenuStrip_WithContext_CallsContextMenuBuilderMethods()
        {
            // Setup
            var failureMechanism = new DuneErosionFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocksRepository);
            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            var orderMocksRepository = new MockRepository();
            var menuBuilder = orderMocksRepository.StrictMock<IContextMenuBuilder>();
            using (orderMocksRepository.Ordered())
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

            orderMocksRepository.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            orderMocksRepository.VerifyAll();
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

            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);
                }
            }
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
                    FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite")
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);

            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            var mocks = new MockRepository();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocksRepository.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateAllIndex,
                                                                  "Alles be&rekenen",
                                                                  "Geen van de locaties is geschikt voor een hydraulische belastingenberekening.",
                                                                  RiskeerCommonFormsResources.CalculateAllIcon,
                                                                  false);
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
                    FilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite")
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

            var context = new DuneErosionHydraulicLoadsContext(failureMechanism, assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocksRepository.Stub<IGui>();
                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub(mocksRepository);
                gui.Stub(g => g.MainWindow).Return(mainWindow);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocksRepository.Stub<IDocumentViewController>());

                var calculatorFactory = mocksRepository.Stub<IHydraRingCalculatorFactory>();
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
                mocksRepository.ReplayAll();

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
    }
}