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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Storage;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Plugin.TestUtil;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaterLevelCalculationsForNormTargetProbabilityContextViewInfoTest
    {
        private const int calculateColumnIndex = 0;
        private const int waterLevelColumnIndex = 5;

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                Type viewDataType = info.ViewDataType;

                // Assert
                Assert.AreEqual(typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>), viewDataType);
            }
        }

        [Test]
        public void ViewType_Always_ReturnsViewType()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                Type viewType = info.ViewType;

                // Assert
                Assert.AreEqual(typeof(DesignWaterLevelCalculationsView), viewType);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetWaterLevelForNormTargetProbabilityCalculationDisplayTextCases))]
        public void GetViewName_WithContext_ReturnsExpectedViewName(
            Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc,
            double lowerLimitNorm, double signalingNorm, string expectedProbabilityText)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub()
            {
                FailureMechanismContribution =
                {
                    LowerLimitNorm = lowerLimitNorm,
                    SignalingNorm = signalingNorm
                }
            };
            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(getCalculationsFunc(assessmentSection),
                                                                                    assessmentSection,
                                                                                    () => 0.01);
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                string viewName = info.GetViewName(null, context);

                // Assert
                Assert.AreEqual($"Waterstanden bij norm - {expectedProbabilityText}", viewName);
            }
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                Image image = info.Image;

                // Assert
                TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void GetViewData_Always_ReturnsHydraulicBoundaryLocationCalculations()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                    assessmentSection,
                                                                                    () => 0.01);

            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                object viewData = info.GetViewData(context);

                // Assert
                Assert.AreSame(calculations, viewData);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedViewProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                    assessmentSection,
                                                                                    () => 0.01);

            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                var view = (DesignWaterLevelCalculationsView) info.CreateInstance(context);

                // Assert
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedDataGridViewData()
        {
            // Setup
            var random = new Random();

            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
                },
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
                }
            };

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(hydraulicBoundaryLocationCalculations,
                                                                                    new AssessmentSectionStub(),
                                                                                    () => 0.01);

            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                var view = (DesignWaterLevelCalculationsView) info.CreateInstance(context);

                using (var testForm = new Form())
                {
                    testForm.Controls.Add(view);
                    testForm.Show();

                    // Assert
                    DataGridView calculationsDataGridView = ControlTestHelper.GetDataGridView(view, "DataGridView");
                    DataGridViewRowCollection rows = calculationsDataGridView.Rows;
                    Assert.AreEqual(2, rows.Count);
                    Assert.AreEqual(hydraulicBoundaryLocationCalculations[0].Output.Result.ToString(), rows[0].Cells[waterLevelColumnIndex].FormattedValue);
                    Assert.AreEqual(hydraulicBoundaryLocationCalculations[1].Output.Result.ToString(), rows[1].Cells[waterLevelColumnIndex].FormattedValue);
                }
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedCalculationData()
        {
            // Setup
            double GetNormFunc() => 0.01;

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(hydraulicBoundaryLocationCalculations,
                                                                                    new AssessmentSectionStub(),
                                                                                    GetNormFunc);

            var mockRepository = new MockRepository();
            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            double actualNormValue = double.NaN;
            IEnumerable<HydraulicBoundaryLocationCalculation> performedCalculations = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    performedCalculations = (IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[0];
                    actualNormValue = (double) invocation.Arguments[2];
                });

            mockRepository.ReplayAll();

            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                var view = (DesignWaterLevelCalculationsView) info.CreateInstance(context);

                using (var testForm = new Form())
                {
                    view.CalculationGuiService = guiService;
                    testForm.Controls.Add(view);
                    testForm.Show();

                    DataGridView calculationsDataGridView = ControlTestHelper.GetDataGridView(view, "DataGridView");
                    DataGridViewRowCollection rows = calculationsDataGridView.Rows;
                    rows[0].Cells[calculateColumnIndex].Value = true;

                    view.CalculationGuiService = guiService;
                    var button = new ButtonTester("CalculateForSelectedButton", testForm);

                    button.Click();

                    // Assert
                    Assert.AreEqual(GetNormFunc(), actualNormValue);
                    Assert.AreSame(hydraulicBoundaryLocationCalculation, performedCalculations.Single());
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AfterCreate_WithGuiSet_SetsSpecificPropertiesToView()
        {
            // Setup
            var mocks = new MockRepository();
            IGui gui = StubFactory.CreateGuiStub(mocks);
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
            gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());
            gui.Stub(g => g.ProjectStore).Return(mocks.Stub<IStoreProject>());
            mocks.ReplayAll();

            double GetNormFunc() => 0.01;
            var assessmentSection = new AssessmentSectionStub();
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            var context = new WaterLevelCalculationsForNormTargetProbabilityContext(calculations,
                                                                                    assessmentSection,
                                                                                    GetNormFunc);

            using (var view = new DesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   new AssessmentSectionStub(),
                                                                   GetNormFunc,
                                                                   () => "1/100"))

            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);
                plugin.Gui = gui;
                plugin.Activate();

                // Call
                info.AfterCreate(view, context);

                // Assert
                Assert.IsInstanceOf<IHydraulicBoundaryLocationCalculationGuiService>(view.CalculationGuiService);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CloseViewForData_ForMatchingAssessmentSection_ReturnsTrue()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();

            using (var view = new DesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   () => "1/100"))
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSection);

                // Assert
                Assert.IsTrue(closeForData);
            }
        }

        [Test]
        public void CloseViewForData_ForNonMatchingAssessmentSection_ReturnsFalse()
        {
            // Setup
            var assessmentSectionA = new AssessmentSectionStub();
            var assessmentSectionB = new AssessmentSectionStub();

            using (var view = new DesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSectionA,
                                                                   () => 0.01,
                                                                   () => "1/100"))
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, assessmentSectionB);

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        [Test]
        public void CloseViewForData_ForOtherObjectType_ReturnsFalse()
        {
            // Setup
            var assessmentSectionA = new AssessmentSectionStub();

            using (var view = new DesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                   assessmentSectionA,
                                                                   () => 0.01,
                                                                   () => "1/100"))
            using (var plugin = new RiskeerPlugin())
            {
                ViewInfo info = GetViewInfo(plugin);

                // Call
                bool closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }
        
        private static IEnumerable<TestCaseData> GetWaterLevelForNormTargetProbabilityCalculationDisplayTextCases()
        {
            const double lowerLimitNorm = 0.1;
            const double signalingNorm = 0.01;

            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm),
                                          signalingNorm, signalingNorm, "1/100 (1)")
                .SetName("SignalingNormCalculationsNormsSame");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForSignalingNorm),
                                          lowerLimitNorm, signalingNorm, "1/100")
                .SetName("SignalingNormCalculationsNormsDifferent");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm),
                                          lowerLimitNorm, lowerLimitNorm, "1/10")
                .SetName("LowerLimitNormCalculationsNormsSame");
            yield return new TestCaseData(new Func<IAssessmentSection, IObservableEnumerable<HydraulicBoundaryLocationCalculation>>(a => a.WaterLevelCalculationsForLowerLimitNorm),
                                          lowerLimitNorm, signalingNorm, "1/10")
                .SetName("LowerLimitNormCalculationsNormsDifferent");
        }

        private static ViewInfo GetViewInfo(RiskeerPlugin plugin)
        {
            return plugin.GetViewInfos().First(tni => tni.DataType == typeof(WaterLevelCalculationsForNormTargetProbabilityContext));
        }
    }
}