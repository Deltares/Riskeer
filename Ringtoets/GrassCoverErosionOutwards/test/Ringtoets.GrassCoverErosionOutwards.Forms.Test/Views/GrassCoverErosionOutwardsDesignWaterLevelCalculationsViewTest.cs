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
    public class GrassCoverErosionOutwardsDesignWaterLevelCalculationsViewTest
    {
        private const int calculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int designWaterLevelColumnIndex = 5;

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
            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                            new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                            assessmentSection,
                                                                                            () => 0.01))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryCalculationsView>(view);
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
            TestDelegate call = () => new GrassCoverErosionOutwardsDesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                                    null,
                                                                                                    assessmentSection,
                                                                                                    () => 0.01);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_GetNormFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionOutwardsDesignWaterLevelCalculationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(),
                                                                                                    new GrassCoverErosionOutwardsFailureMechanism(),
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
            ShowDesignWaterLevelLocationsView(new ObservableList<HydraulicBoundaryLocationCalculation>(), assessmentSection, 0.01, testForm);

            // Assert
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            Assert.AreEqual(6, calculationsDataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn) calculationsDataGridView.Columns[calculateColumnIndex];
            Assert.AreEqual("Berekenen", calculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) calculationsDataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var designWaterLevelColumn = (DataGridViewTextBoxColumn) calculationsDataGridView.Columns[designWaterLevelColumnIndex];
            Assert.AreEqual("Waterstand bij doorsnede-eis [m+NAP]", designWaterLevelColumn.HeaderText);

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
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            Assert.AreEqual(4, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[designWaterLevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[designWaterLevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[designWaterLevelColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[designWaterLevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryLocationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowDesignWaterLevelLocationsView(calculations, assessmentSection, 0.01, testForm);

            // Precondition
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            object dataGridViewSource = calculationsDataGridView.DataSource;
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;
            Assert.AreEqual(4, rows.Count);

            const double designWaterLevel = 10.23;

            calculations.Clear();
            calculations.Add(new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0))
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationOutput(designWaterLevel)
            });

            // Call
            calculations.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[calculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(designWaterLevel, cells[designWaterLevelColumnIndex].Value);
            Assert.AreNotSame(dataGridViewSource, calculationsDataGridView.DataSource);
            Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryLocationUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetTestHydraulicBoundaryLocationCalculations();

            ShowDesignWaterLevelLocationsView(calculations, assessmentSection, 0.01, testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();

            calculationsDataGridViewControl.SetCurrentCell(calculationsDataGridViewControl.GetCell(2, 0));

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
            calculations[2].Output = output;
            calculations[2].NotifyObservers();

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

            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetTestHydraulicBoundaryLocationCalculations();

            GrassCoverErosionOutwardsDesignWaterLevelCalculationsView view = ShowDesignWaterLevelLocationsView(calculations, assessmentSection, 0.01, testForm);
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            object dataGridViewSource = calculationsDataGridView.DataSource;
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, performedCalculations.Length);
            Assert.AreSame(calculations.First(), performedCalculations.First());
            Assert.AreSame(dataGridViewSource, calculationsDataGridView.DataSource);
            Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            mockRepository.ReplayAll();

            ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);

            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
            DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

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

            GrassCoverErosionOutwardsDesignWaterLevelCalculationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(assessmentSection, testForm);
            if (!contributionNotZero)
            {
                view.FailureMechanism.Contribution = 0;
                view.FailureMechanism.NotifyObservers();
            }

            // When
            if (rowSelected)
            {
                DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();
                DataGridViewRowCollection rows = calculationsDataGridViewControl.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;
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

            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetTestHydraulicBoundaryLocationCalculations();

            GrassCoverErosionOutwardsDesignWaterLevelCalculationsView view = ShowDesignWaterLevelLocationsView(calculations, assessmentSection, norm, testForm);
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = view.FailureMechanism;
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

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
            Assert.AreSame(calculations.First(), performedCalculations.First());
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

            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetTestHydraulicBoundaryLocationCalculations();

            GrassCoverErosionOutwardsDesignWaterLevelCalculationsView view = ShowDesignWaterLevelLocationsView(calculations, assessmentSection, norm, testForm);
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

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
            Assert.AreSame(calculations.First(), performedCalculations.First());
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

            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetTestHydraulicBoundaryLocationCalculations();

            GrassCoverErosionOutwardsDesignWaterLevelCalculationsView view = ShowDesignWaterLevelLocationsView(calculations, assessmentSection, norm, testForm);
            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            DataGridViewRowCollection rows = calculationsDataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

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
            Assert.AreSame(calculations.First(), performedCalculations.First());
        }

        private DataGridView GetCalculationsDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "DataGridView");
        }

        private DataGridViewControl GetCalculationsDataGridViewControl()
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

        private static GrassCoverErosionOutwardsDesignWaterLevelCalculationsView ShowDesignWaterLevelLocationsView(ObservableList<HydraulicBoundaryLocationCalculation> calculations,
                                                                                                                   IAssessmentSection assessmentSection,
                                                                                                                   double norm,
                                                                                                                   Form form)
        {
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };

            var view = new GrassCoverErosionOutwardsDesignWaterLevelCalculationsView(calculations,
                                                                                     failureMechanism,
                                                                                     assessmentSection,
                                                                                     () => norm);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static GrassCoverErosionOutwardsDesignWaterLevelCalculationsView ShowFullyConfiguredDesignWaterLevelLocationsView(IAssessmentSection assessmentSection,
                                                                                                                                  Form form)
        {
            return ShowDesignWaterLevelLocationsView(GetTestHydraulicBoundaryLocationCalculations(), assessmentSection, 0.01, form);
        }

        private static ObservableList<HydraulicBoundaryLocationCalculation> GetTestHydraulicBoundaryLocationCalculations()
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

            return new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0)),
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0))
                {
                    Output = new TestHydraulicBoundaryLocationOutput(1.23)
                },
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0))
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    }
                },
                new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(4, "4", 4.0, 4.0))
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(1.01, generalResult)
                }
            };
        }

        [TestFixture]
        private class ViewSynchronizationTest : CalculationsViewSynchronizationTester<HydraulicBoundaryLocationCalculation>
        {
            private ObservableList<HydraulicBoundaryLocationCalculation> calculations;

            protected override int OutputColumnIndex
            {
                get
                {
                    return designWaterLevelColumnIndex;
                }
            }

            public override void Setup()
            {
                calculations = GetTestHydraulicBoundaryLocationCalculations();

                base.Setup();
            }

            protected override object GetCalculationSelection(CalculationsView<HydraulicBoundaryLocationCalculation> view, object selectedRowObject)
            {
                return new GrassCoverErosionOutwardsDesignWaterLevelCalculationContext(((HydraulicBoundaryLocationRow) selectedRowObject).CalculatableObject);
            }

            protected override CalculationsView<HydraulicBoundaryLocationCalculation> ShowFullyConfiguredCalculationsView(Form form)
            {
                return ShowDesignWaterLevelLocationsView(calculations, new ObservableTestAssessmentSectionStub(), 0.01, form);
            }

            protected override ObservableList<HydraulicBoundaryLocationCalculation> GetCalculationsInView(CalculationsView<HydraulicBoundaryLocationCalculation> view)
            {
                return calculations;
            }
        }
    }
}