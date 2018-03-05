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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
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
using Ringtoets.Common.Service;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationDesignWaterLevelColumnIndex = 5;

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
            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                         hbl => new HydraulicBoundaryLocationCalculation(hbl),
                                                                                         assessmentSection,
                                                                                         () => 0.01))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(null,
                                                                                                 hbl => new HydraulicBoundaryLocationCalculation(hbl),
                                                                                                 assessmentSection,
                                                                                                 () => 0.01);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void Constructor_GetNormFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                                 hbl => new HydraulicBoundaryLocationCalculation(hbl),
                                                                                                 assessmentSection,
                                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getNormFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            ShowDesignWaterLevelLocationsView(new ObservableList<HydraulicBoundaryLocation>(), assessmentSection, 0.01, testForm);

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

            var locationDesignWaterLevelColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationDesignWaterLevelColumnIndex];
            Assert.AreEqual("Waterstand bij doorsnede-eis [m+NAP]", locationDesignWaterLevelColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void DesignWaterLevelLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);

            // Assert
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            Assert.AreEqual(4, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterLevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterLevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterLevelColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterLevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryLocationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);
            ObservableList<HydraulicBoundaryLocation> locations = view.FailureMechanism.HydraulicBoundaryLocations;

            // Precondition
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            object dataGridViewSource = locationsDataGridView.DataSource;
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(4, rows.Count);

            const double designWaterLevel = 10.23;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevelCalculation1 =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(designWaterLevel)
                }
            };

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
            Assert.AreEqual(designWaterLevel, cells[locationDesignWaterLevelColumnIndex].Value);
            Assert.AreNotSame(dataGridViewSource, locationsDataGridView.DataSource);
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryLocationUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();

            locationsDataGridViewControl.SetCurrentCell(locationsDataGridViewControl.GetCell(2, 0));

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

            ObservableList<HydraulicBoundaryLocation> locations = view.FailureMechanism.HydraulicBoundaryLocations;

            // Call
            locations[2].DesignWaterLevelCalculation1.Output = output;
            locations[2].NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevelsSelectionNotChanged()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation => { performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[2]).ToArray(); });

            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            object dataGridViewSource = locationsDataGridView.DataSource;
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(view.FailureMechanism.HydraulicBoundaryLocations.First(), performedCalculations.First().HydraulicBoundaryLocation);
            Assert.AreSame(dataGridViewSource, locationsDataGridView.DataSource);
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);

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
        [TestCase(false, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, false, message)")]
        [TestCase(true, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, false, message)")]
        [TestCase(false, true, "Er zijn geen berekeningen geselecteerd.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, true, message)")]
        [TestCase(true, true, "", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, true, message)")]
        public void GivenDesignWaterLevelLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                                          bool contributionNotZero,
                                                                                                                                                                          string expectedErrorMessage)
        {
            // Given
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);
            if (!contributionNotZero)
            {
                view.FailureMechanism.Contribution = 0;
                view.FailureMechanism.NotifyObservers();
            }

            // When
            if (rowSelected)
            {
                DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
                DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && contributionNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            const double norm = 0.01;

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                });

            mockRepository.ReplayAll();

            assessmentSection.HydraulicBoundaryDatabase.FilePath = databaseFilePath;

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, norm, testForm);
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = view.FailureMechanism;
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            Assert.IsInstanceOf<GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual("", preprocessorDirectoryValue);
            Assert.AreEqual(expectedNorm, normValue);
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(failureMechanism.HydraulicBoundaryLocations.First(), performedCalculations.First().HydraulicBoundaryLocation);
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            string preprocessorDirectory = TestHelper.GetScratchPadPath();
            const double norm = 0.01;

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                });

            mockRepository.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.FilePath = databaseFilePath;
            hydraulicBoundaryDatabase.CanUsePreprocessor = true;
            hydraulicBoundaryDatabase.UsePreprocessor = true;
            hydraulicBoundaryDatabase.PreprocessorDirectory = preprocessorDirectory;

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, norm, testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = view.FailureMechanism;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            Assert.IsInstanceOf<GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual(preprocessorDirectory, preprocessorDirectoryValue);
            Assert.AreEqual(expectedNorm, normValue);
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(failureMechanism.HydraulicBoundaryLocations.First(), performedCalculations.First().HydraulicBoundaryLocation);
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_CalculateDesignWaterLevelsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            const double norm = 0.01;

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            assessmentSection.Stub(a => a.Id).Return(string.Empty);
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocationCalculation[] performedCalculations = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, int.MinValue, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    performedCalculations = ((IEnumerable<HydraulicBoundaryLocationCalculation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                });

            mockRepository.ReplayAll();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            hydraulicBoundaryDatabase.FilePath = databaseFilePath;
            hydraulicBoundaryDatabase.CanUsePreprocessor = true;
            hydraulicBoundaryDatabase.UsePreprocessor = false;
            hydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, norm, testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = view.FailureMechanism;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            Assert.IsInstanceOf<GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual("", preprocessorDirectoryValue);
            Assert.AreEqual(expectedNorm, normValue);
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(failureMechanism.HydraulicBoundaryLocations.First(), performedCalculations.First().HydraulicBoundaryLocation);
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

        private static GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView(ObservableList<HydraulicBoundaryLocation> locations,
                                                                                                                IAssessmentSection assessmentSection,
                                                                                                                double norm,
                                                                                                                Form form)
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            failureMechanism.HydraulicBoundaryLocations.AddRange(locations);

            var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView(failureMechanism,
                                                                                  hbl => hbl.DesignWaterLevelCalculation1,
                                                                                  assessmentSection,
                                                                                  () => norm);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView(IAssessmentSection assessmentSection,
                                                                                                                               Form form)
        {
            return ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, 0.01, form);
        }

        private static GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView(IAssessmentSection assessmentSection,
                                                                                                                               double norm,
                                                                                                                               Form form)
        {
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

            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevelCalculation1 =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    DesignWaterLevelCalculation1 =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                },
                new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    DesignWaterLevelCalculation1 =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = output
                    }
                }
            };

            return ShowDesignWaterLevelLocationsView(locations, assessmentSection, norm, form);
        }

        [TestFixture]
        private class ViewSynchronizationTest : LocationsViewSynchronizationTester<HydraulicBoundaryLocation>
        {
            protected override int OutputColumnIndex
            {
                get
                {
                    return locationDesignWaterLevelColumnIndex;
                }
            }

            protected override object GetLocationSelection(LocationsView<HydraulicBoundaryLocation> view, object selectedRowObject)
            {
                return new GrassCoverErosionOutwardsDesignWaterLevelLocationContext(((HydraulicBoundaryLocationRow) selectedRowObject).CalculatableObject);
            }

            protected override LocationsView<HydraulicBoundaryLocation> ShowFullyConfiguredLocationsView(Form form)
            {
                return ShowFullyConfiguredDesignWaterLevelLocationsView(new ObservableTestAssessmentSectionStub(), form);
            }

            protected override ObservableList<HydraulicBoundaryLocation> GetLocationsInView(LocationsView<HydraulicBoundaryLocation> view)
            {
                return ((GrassCoverErosionOutwardsDesignWaterLevelLocationsView) view).FailureMechanism.HydraulicBoundaryLocations;
            }

            protected override HydraulicBoundaryLocationCalculation GetCalculationForLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
            {
                return hydraulicBoundaryLocation.DesignWaterLevelCalculation1;
            }
        }
    }
}