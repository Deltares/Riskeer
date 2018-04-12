﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaveHeightCalculationsViewInfoTest
    {
        private const int calculateColumnIndex = 0;
        private const int waveHeightColumnIndex = 5;

        private RingtoetsPlugin plugin;
        private ViewInfo info;

        [SetUp]
        public void SetUp()
        {
            plugin = new RingtoetsPlugin();
            info = GetViewInfo(plugin);
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void GetViewName_WithWaveHeightCalculationsContext_ReturnsViewNameContainingCategoryBoundaryName()
        {
            // Setup
            const string categoryBoundaryName = "Category";

            var context = new WaveHeightCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                            new AssessmentSectionStub(),
                                                            () => 0.01,
                                                            categoryBoundaryName);

            // Call
            string viewName = info.GetViewName(null, context);

            // Assert
            Assert.AreEqual($"Golfhoogtes - {categoryBoundaryName}", viewName);
        }

        [Test]
        public void ViewDataType_Always_ReturnsViewDataType()
        {
            // Call
            Type viewDataType = info.ViewDataType;

            // Assert
            Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocationCalculation>), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(WaveHeightCalculationsContext), dataType);
        }

        [Test]
        public void Image_Always_ReturnsGenericInputOutputIcon()
        {
            // Call
            Image image = info.Image;

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void GetViewData_Always_ReturnsHydraulicBoundaryLocationCalculations()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            var context = new WaveHeightCalculationsContext(calculations,
                                                            assessmentSection,
                                                            () => 0.01,
                                                            "Category");

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(calculations, viewData);
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedViewProperties()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var context = new WaveHeightCalculationsContext(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                            assessmentSection,
                                                            () => 0.01,
                                                            "Category");

            // Call
            var view = (WaveHeightCalculationsView) info.CreateInstance(context);

            // Assert
            Assert.AreSame(assessmentSection, view.AssessmentSection);
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
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                },
                new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                }
            };

            var context = new WaveHeightCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                            new AssessmentSectionStub(),
                                                            () => 0.01,
                                                            "Category");

            // Call
            var view = (WaveHeightCalculationsView) info.CreateInstance(context);

            // Assert
            using (var testForm = new Form())
            {
                testForm.Controls.Add(view);
                testForm.Show();

                DataGridView calculationsDataGridView = ControlTestHelper.GetDataGridView(view, "DataGridView");
                DataGridViewRowCollection rows = calculationsDataGridView.Rows;
                Assert.AreEqual(2, rows.Count);
                Assert.AreEqual(hydraulicBoundaryLocationCalculations[0].Output.Result.ToString(), rows[0].Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual(hydraulicBoundaryLocationCalculations[1].Output.Result.ToString(), rows[1].Cells[waveHeightColumnIndex].FormattedValue);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedCalculationData()
        {
            // Setup
            Func<double> getNormFunc = () => 0.01;

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());
            var hydraulicBoundaryLocationCalculations = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                hydraulicBoundaryLocationCalculation
            };

            var context = new WaveHeightCalculationsContext(hydraulicBoundaryLocationCalculations,
                                                            new AssessmentSectionStub(),
                                                            getNormFunc,
                                                            "Category");

            var mockRepository = new MockRepository();
            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            double actualNormValue = double.NaN;
            IEnumerable<HydraulicBoundaryLocationCalculation> performedCalculations = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    performedCalculations = (IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[2];
                    actualNormValue = (double) invocation.Arguments[3];
                });

            mockRepository.ReplayAll();

            // Call
            var view = (WaveHeightCalculationsView) info.CreateInstance(context);

            // Assert
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

                Assert.AreEqual(getNormFunc(), actualNormValue);
                Assert.AreSame(hydraulicBoundaryLocationCalculation, performedCalculations.Single());
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void AfterCreate_WithGuiSet_SetsSpecificPropertiesToView()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.ViewCommands).Return(mocks.Stub<IViewCommands>());
            gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
            gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

            mocks.ReplayAll();

            Func<double> getNormFunc = () => 0.01;
            var assessmentSection = new AssessmentSectionStub();
            var calculations = new ObservableList<HydraulicBoundaryLocationCalculation>();

            const string categoryBoundaryName = "Category";

            var context = new WaveHeightCalculationsContext(calculations,
                                                            assessmentSection,
                                                            getNormFunc,
                                                            categoryBoundaryName);

            using (var view = new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                             assessmentSection,
                                                             getNormFunc,
                                                             categoryBoundaryName))

            using (var ringtoetsPlugin = new RingtoetsPlugin())
            {
                ViewInfo viewInfo = GetViewInfo(ringtoetsPlugin);
                ringtoetsPlugin.Gui = gui;
                ringtoetsPlugin.Activate();

                // Call
                viewInfo.AfterCreate(view, context);

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

            using (var view = new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                             assessmentSection,
                                                             () => 0.01,
                                                             "Category"))
            {
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

            using (var view = new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                             assessmentSectionA,
                                                             () => 0.01,
                                                             "Category"))
            {
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

            using (var view = new WaveHeightCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                             assessmentSectionA,
                                                             () => 0.01,
                                                             "Category"))
            {
                // Call
                bool closeForData = info.CloseForData(view, new object());

                // Assert
                Assert.IsFalse(closeForData);
            }
        }

        private static ViewInfo GetViewInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveHeightCalculationsView));
        }
    }
}