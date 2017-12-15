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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class DesignWaterLevelLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationDesignWaterlevelColumnIndex = 5;

        private Form testForm;
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            using (var view = new DesignWaterLevelLocationsView(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            ShowDesignWaterLevelLocationsView(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection, testForm);

            // Assert
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            Assert.AreEqual(6, locationsDataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) locationsDataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) locationsDataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var locationDesignWaterlevelColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationDesignWaterlevelColumnIndex];
            Assert.AreEqual("Toetspeil [m+NAP]", locationDesignWaterlevelColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Constructor_WithLocations_DataGridViewCorrectlyInitialized()
        {
            // Setup 
            var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();

            // Call
            ShowFullyConfiguredDesignWaterLevelLocationsView(testHydraulicBoundaryDatabase.Locations, testForm);

            // Assert
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            Assert.AreEqual(5, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[4].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(5, 5).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryDatabaseLocationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup 
            var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();
            ObservableList<HydraulicBoundaryLocation> locations = testHydraulicBoundaryDatabase.Locations;
            ShowFullyConfiguredDesignWaterLevelLocationsView(locations, testForm);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevelCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(10.23)
                }
            };

            // Precondition
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            Assert.AreEqual(5, rows.Count);

            locations.Clear();
            locations.Add(hydraulicBoundaryLocation);

            // Call
            locations.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(hydraulicBoundaryLocation.DesignWaterLevel, cells[locationDesignWaterlevelColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryLocationUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();
            ObservableList<HydraulicBoundaryLocation> locations = testHydraulicBoundaryDatabase.Locations;
            ShowFullyConfiguredDesignWaterLevelLocationsView(locations, testForm);

            // Precondition
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);

            HydraulicBoundaryLocation location = locations.First();

            // Call
            location.DesignWaterLevelCalculation.InputParameters.ShouldIllustrationPointsBeCalculated = true;
            location.NotifyObservers();

            // Assert
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryLocationUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();
            ObservableList<HydraulicBoundaryLocation> locations = testHydraulicBoundaryDatabase.Locations;
            ShowFullyConfiguredDesignWaterLevelLocationsView(locations, testForm);

            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();

            locationsDataGridViewControl.SetCurrentCell(locationsDataGridViewControl.GetCell(3, 0));

            // Precondition
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationOutput(generalResult);

            // Call
            HydraulicBoundaryLocation hydraulicBoundaryLocation = locations[3];
            hydraulicBoundaryLocation.DesignWaterLevelCalculation.Output = output;
            hydraulicBoundaryLocation.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevels()
        {
            // Setup
            var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(testHydraulicBoundaryDatabase.Locations, testForm);

            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            IEnumerable<HydraulicBoundaryLocation> locations = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation => { locations = (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]; });
            mockRepository.ReplayAll();

            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            HydraulicBoundaryLocation[] hydraulicBoundaryLocations = locations.ToArray();
            Assert.AreEqual(1, hydraulicBoundaryLocations.Length);
            HydraulicBoundaryLocation expectedLocation = testHydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, hydraulicBoundaryLocations.First());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();
            ShowFullyConfiguredDesignWaterLevelLocationsView(testHydraulicBoundaryDatabase.Locations, testForm);

            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocation[] calculatedLocationsValue = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    calculatedLocationsValue = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                });

            mockRepository.ReplayAll();

            DesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView(hydraulicBoundaryDatabase.Locations, assessmentSection, testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual("", preprocessorDirectoryValue);
            Assert.AreEqual(normValue, assessmentSection.FailureMechanismContribution.Norm);
            Assert.AreEqual(1, calculatedLocationsValue.Length);
            HydraulicBoundaryLocation expectedLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocationsValue.First());
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            const string preprocessorDirectory = "PreprocessorDirectory";

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = preprocessorDirectory
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocation[] calculatedLocationsValue = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    calculatedLocationsValue = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                });

            mockRepository.ReplayAll();

            DesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView(hydraulicBoundaryDatabase.Locations, assessmentSection, testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual(preprocessorDirectory, preprocessorDirectoryValue);
            Assert.AreEqual(normValue, assessmentSection.FailureMechanismContribution.Norm);
            Assert.AreEqual(1, calculatedLocationsValue.Length);
            HydraulicBoundaryLocation expectedLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocationsValue.First());
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = false,
                PreprocessorDirectory = "InvalidPreprocessorDirectory"
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocation[] calculatedLocationsValue = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    calculatedLocationsValue = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                });

            mockRepository.ReplayAll();

            DesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView(hydraulicBoundaryDatabase.Locations, assessmentSection, testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual(string.Empty, preprocessorDirectoryValue);
            Assert.AreEqual(normValue, assessmentSection.FailureMechanismContribution.Norm);
            Assert.AreEqual(1, calculatedLocationsValue.Length);
            HydraulicBoundaryLocation expectedLocation = hydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocationsValue.First());
        }

        private DataGridView GetLocationsDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "DataGridView");
        }

        private DataGridViewControl GetLocationsDataGridViewControl()
        {
            return ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return ControlTestHelper.GetControls<IllustrationPointsControl>(testForm, "IllustrationPointsControl").Single();
        }

        private static IEnumerable<IllustrationPointControlItem> CreateControlItems(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            return generalResult.TopLevelIllustrationPoints
                                .Select(topLevelIllustrationPoint =>
                                {
                                    SubMechanismIllustrationPoint illustrationPoint = topLevelIllustrationPoint.SubMechanismIllustrationPoint;
                                    return new IllustrationPointControlItem(topLevelIllustrationPoint,
                                                                            topLevelIllustrationPoint.WindDirection.Name,
                                                                            topLevelIllustrationPoint.ClosingSituation,
                                                                            illustrationPoint.Stochasts,
                                                                            illustrationPoint.Beta);
                                });
        }

        private static DesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView(ObservableList<HydraulicBoundaryLocation> locations,
                                                                                       IAssessmentSection assessmentSection, Form form)
        {
            var view = new DesignWaterLevelLocationsView(locations, assessmentSection);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static DesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView(ObservableList<HydraulicBoundaryLocation> locations,
                                                                                                      Form form)
        {
            var assessmentSection = new ObservableTestAssessmentSectionStub();

            return ShowDesignWaterLevelLocationsView(locations, assessmentSection, form);
        }

        private class TestHydraulicBoundaryDatabase : HydraulicBoundaryDatabase
        {
            public TestHydraulicBoundaryDatabase()
            {
                AddLocations();
            }

            private void AddLocations()
            {
                Locations.Add(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0));
                Locations.Add(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                });
                Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                });
                Locations.Add(new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                });

                var topLevelIllustrationPoints = new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "Regular",
                                                              new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "Test",
                                                              new TestSubMechanismIllustrationPoint())
                };

                var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
                var output = new TestHydraulicBoundaryLocationOutput(1.01, generalResult);

                Locations.Add(new HydraulicBoundaryLocation(5, "5", 5.0, 5.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = output
                    }
                });
            }
        }

        [TestFixture]
        private class ViewSynchronizationTest : LocationsViewSynchronizationTester<HydraulicBoundaryLocation>
        {
            private ObservableList<HydraulicBoundaryLocation> locations;

            protected override int OutputColumnIndex
            {
                get
                {
                    return locationDesignWaterlevelColumnIndex;
                }
            }

            [SetUp]
            public override void Setup()
            {
                var testHydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();
                locations = testHydraulicBoundaryDatabase.Locations;
                base.Setup();
            }

            protected override object GetLocationSelection(LocationsView<HydraulicBoundaryLocation> view, object selectedRowObject)
            {
                IAssessmentSection assessmentSection = view.AssessmentSection;

                return new DesignWaterLevelLocationContext(((HydraulicBoundaryLocationRow) selectedRowObject).CalculatableObject,
                                                           assessmentSection.HydraulicBoundaryDatabase);
            }

            protected override LocationsView<HydraulicBoundaryLocation> ShowFullyConfiguredLocationsView(Form form)
            {
                return ShowFullyConfiguredDesignWaterLevelLocationsView(locations, form);
            }

            protected override void ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
            {
                locations.Clear();
                locations.Add(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0));
                locations.NotifyObservers();
            }

            protected override void ClearLocationOutputAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
            {
                locations.ForEach(loc =>
                {
                    loc.DesignWaterLevelCalculation.Output = null;
                    loc.NotifyObservers();
                });
            }

            protected override void AddLocationOutputAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = locations.First();
                hydraulicBoundaryLocation.DesignWaterLevelCalculation.Output = new TestHydraulicBoundaryLocationOutput(new TestGeneralResultSubMechanismIllustrationPoint());
                hydraulicBoundaryLocation.NotifyObservers();
            }
        }
    }
}