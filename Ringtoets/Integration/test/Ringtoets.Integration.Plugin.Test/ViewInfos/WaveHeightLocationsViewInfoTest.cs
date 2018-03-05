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
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.ViewInfos
{
    [TestFixture]
    public class WaveHeightLocationsViewInfoTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int locationWaveHeightColumnIndex = 5;

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
        public void GetViewName_WithWaveHeightLocationsContext_ReturnsViewNameContainingCategoryBoundaryName()
        {
            // Setup
            const string categoryBoundaryName = "Category";

            var context = new WaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                         new ObservableTestAssessmentSectionStub(),
                                                         () => 0.01,
                                                         hbl => new HydraulicBoundaryLocationCalculation(hbl),
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
            Assert.AreEqual(typeof(IEnumerable<HydraulicBoundaryLocation>), viewDataType);
        }

        [Test]
        public void DataType_Always_ReturnsDataType()
        {
            // Call
            Type dataType = info.DataType;

            // Assert
            Assert.AreEqual(typeof(WaveHeightLocationsContext), dataType);
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
        public void GetViewData_Always_ReturnsHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            ObservableList<HydraulicBoundaryLocation> locations = assessmentSection.HydraulicBoundaryDatabase.Locations;

            var context = new WaveHeightLocationsContext(locations,
                                                         assessmentSection,
                                                         () => 0.01,
                                                         hbl => new HydraulicBoundaryLocationCalculation(hbl),
                                                         "Category");

            // Call
            object viewData = info.GetViewData(context);

            // Assert
            Assert.AreSame(locations, viewData);
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedViewProperties()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var context = new WaveHeightLocationsContext(new ObservableList<HydraulicBoundaryLocation>(),
                                                         assessmentSection,
                                                         () => 0.01,
                                                         hbl => new HydraulicBoundaryLocationCalculation(hbl),
                                                         "Category");

            // Call
            var view = (WaveHeightLocationsView) info.CreateInstance(context);

            // Assert
            Assert.AreSame(assessmentSection, view.AssessmentSection);
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedDataGridViewData()
        {
            // Setup
            var random = new Random();

            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            var hydraulicBoundaryLocationCalculations = new[]
            {
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocations[0])
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                },
                new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocations[1])
                {
                    Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble())
                }
            };

            var context = new WaveHeightLocationsContext(hydraulicBoundaryLocations,
                                                         new ObservableTestAssessmentSectionStub(),
                                                         () => 0.01,
                                                         hbl => hydraulicBoundaryLocationCalculations.First(hblc => ReferenceEquals(hblc.HydraulicBoundaryLocation, hbl)),
                                                         "Category");

            // Call
            var view = (WaveHeightLocationsView) info.CreateInstance(context);

            // Assert
            using (var testForm = new Form())
            {
                testForm.Controls.Add(view);
                testForm.Show();

                DataGridView locationsDataGridView = ControlTestHelper.GetDataGridView(view, "DataGridView");
                DataGridViewRowCollection rows = locationsDataGridView.Rows;
                Assert.AreEqual(2, rows.Count);
                Assert.AreEqual(hydraulicBoundaryLocationCalculations[0].Output.Result.ToString(), rows[0].Cells[locationWaveHeightColumnIndex].FormattedValue);
                Assert.AreEqual(hydraulicBoundaryLocationCalculations[1].Output.Result.ToString(), rows[1].Cells[locationWaveHeightColumnIndex].FormattedValue);
            }
        }

        [Test]
        public void CreateInstance_WithContext_SetsExpectedCalculationData()
        {
            // Setup
            Func<double> getNormFunc = () => 0.01;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);
            var hydraulicBoundaryLocations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc = hbl => hydraulicBoundaryLocationCalculation;

            var context = new WaveHeightLocationsContext(hydraulicBoundaryLocations,
                                                         new ObservableTestAssessmentSectionStub(),
                                                         getNormFunc,
                                                         getCalculationFunc,
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
            var view = (WaveHeightLocationsView) info.CreateInstance(context);

            // Assert
            using (var testForm = new Form())
            {
                view.CalculationGuiService = guiService;
                testForm.Controls.Add(view);
                testForm.Show();

                DataGridView locationsDataGridView = ControlTestHelper.GetDataGridView(view, "DataGridView");
                DataGridViewRowCollection rows = locationsDataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;

                view.CalculationGuiService = guiService;
                var button = new ButtonTester("CalculateForSelectedButton", testForm);

                button.Click();

                Assert.AreEqual(getNormFunc(), actualNormValue);
                Assert.AreSame(hydraulicBoundaryLocationCalculation, performedCalculations.First());
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
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var locations = new ObservableList<HydraulicBoundaryLocation>();

            const string categoryBoundaryName = "Category";

            Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc = hbl => new HydraulicBoundaryLocationCalculation(hbl);

            var context = new WaveHeightLocationsContext(locations,
                                                         assessmentSection,
                                                         getNormFunc,
                                                         getCalculationFunc,
                                                         categoryBoundaryName);

            using (var view = new WaveHeightLocationsView(locations,
                                                          getCalculationFunc,
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
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(hbl),
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
            var assessmentSectionA = new ObservableTestAssessmentSectionStub();
            var assessmentSectionB = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(hbl),
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
            var assessmentSectionA = new ObservableTestAssessmentSectionStub();

            using (var view = new WaveHeightLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                          hbl => new HydraulicBoundaryLocationCalculation(hbl),
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
            return plugin.GetViewInfos().First(tni => tni.ViewType == typeof(WaveHeightLocationsView));
        }
    }
}