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
    public class GrassCoverErosionOutwardsWaveHeightLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationWaveHeightColumnIndex = 5;

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
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            using (var view = new GrassCoverErosionOutwardsWaveHeightLocationsView(assessmentSection))
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
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            ShowWaveHeightLocationsView(assessmentSection, testForm);

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

            var locationWaveHeightColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationWaveHeightColumnIndex];
            Assert.AreEqual("Golfhoogte bij doorsnede-eis [m]", locationWaveHeightColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void WaveHeightLocationsView_WithNonIObservableList_ThrowsInvalidCastException()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowWaveHeightLocationsView(assessmentSection, testForm);

            var locations = new List<HydraulicBoundaryLocation>
            {
                new TestHydraulicBoundaryLocation()
            };

            // Call
            TestDelegate action = () => view.Data = locations;

            // Assert
            Assert.Throws<InvalidCastException>(action);
        }

        [Test]
        public void WaveHeightLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);

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
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[4].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(5, 5).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[locationWaveHeightColumnIndex].FormattedValue);
        }

        [Test]
        public void WaveHeightLocationsView_HydraulicBoundaryLocationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            object dataGridViewSource = locationsDataGridView.DataSource;
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(5, rows.Count);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10, 10)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(10)
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
            Assert.AreEqual(hydraulicBoundaryLocation.WaveHeight, cells[locationWaveHeightColumnIndex].Value);
            Assert.AreNotSame(dataGridViewSource, locationsDataGridView.DataSource);
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void WaveHeightLocationsView_HydraulicBoundaryLocationsUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);
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

            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Call
            locations[3].WaveHeightCalculation.Output = output;
            locations.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateWaveHeightsSelectionNotChanged(bool isSuccessful)
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(mockRepository);
            assessmentSection.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = mockRepository.StrictMock<IObserver>();

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            HydraulicBoundaryLocation[] calculatedLocations = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation => { calculatedLocations = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray(); }).Return(isSuccessful);

            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            object dataGridViewSource = locationsDataGridView.DataSource;
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            locations.Attach(observer);

            view.CalculationGuiService = guiService;
            view.FailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.AreEqual(1, calculatedLocations.Length);
            HydraulicBoundaryLocation expectedLocation = locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocations.First());
            Assert.AreSame(dataGridViewSource, locationsDataGridView.DataSource);
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);

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
        public void GivenWaveHeightLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                                    bool contributionNotZero,
                                                                                                                                                                    string expectedErrorMessage)
        {
            // Given
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);

            // When
            if (rowSelected)
            {
                DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
                DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            if (contributionNotZero)
            {
                failureMechanism.Contribution = 5;
            }
            view.FailureMechanism = failureMechanism;

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && contributionNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CalculateWaveHeightsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocation[] calculatedLocationsValue = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    calculatedLocationsValue = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                }).Return(true);

            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            view.FailureMechanism = failureMechanism;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                assessmentSection.FailureMechanismContribution.Norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual("", preprocessorDirectoryValue);
            Assert.AreEqual(expectedNorm, normValue);
            Assert.AreEqual(1, calculatedLocationsValue.Length);
            HydraulicBoundaryLocation expectedLocation = locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocationsValue.First());
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_CalculateWaveHeightsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";
            string preprocessorDirectory = TestHelper.GetScratchPadPath();

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = preprocessorDirectory
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocation[] calculatedLocationsValue = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    calculatedLocationsValue = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                }).Return(true);

            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            view.FailureMechanism = failureMechanism;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                assessmentSection.FailureMechanismContribution.Norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual(preprocessorDirectory, preprocessorDirectoryValue);
            Assert.AreEqual(expectedNorm, normValue);
            Assert.AreEqual(1, calculatedLocationsValue.Length);
            HydraulicBoundaryLocation expectedLocation = locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocationsValue.First());
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_CalculateWaveHeightsCalledAsExpected()
        {
            // Setup
            const string databaseFilePath = "DatabaseFilePath";

            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = databaseFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = false,
                PreprocessorDirectory = "InvalidPreprocessorDirectory"
            };
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var hydraulicBoundaryDatabaseFilePathValue = "";
            var preprocessorDirectoryValue = "";
            HydraulicBoundaryLocation[] calculatedLocationsValue = null;
            double normValue = double.NaN;
            ICalculationMessageProvider messageProviderValue = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    hydraulicBoundaryDatabaseFilePathValue = invocation.Arguments[0].ToString();
                    preprocessorDirectoryValue = invocation.Arguments[1].ToString();
                    calculatedLocationsValue = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[2]).ToArray();
                    normValue = (double) invocation.Arguments[3];
                    messageProviderValue = (ICalculationMessageProvider) invocation.Arguments[4];
                }).Return(true);

            mockRepository.ReplayAll();

            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView(assessmentSection, testForm);
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection rows = locationsDataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            view.CalculationGuiService = guiService;
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            view.FailureMechanism = failureMechanism;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            double expectedNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                assessmentSection.FailureMechanismContribution.Norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);

            Assert.IsInstanceOf<GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider>(messageProviderValue);
            Assert.AreEqual(databaseFilePath, hydraulicBoundaryDatabaseFilePathValue);
            Assert.AreEqual("", preprocessorDirectoryValue);
            Assert.AreEqual(expectedNorm, normValue);
            Assert.AreEqual(1, calculatedLocationsValue.Length);
            HydraulicBoundaryLocation expectedLocation = locations.First();
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

        private static GrassCoverErosionOutwardsWaveHeightLocationsView ShowWaveHeightLocationsView(IAssessmentSection assessmentSection, Form form)
        {
            var view = new GrassCoverErosionOutwardsWaveHeightLocationsView(assessmentSection);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static GrassCoverErosionOutwardsWaveHeightLocationsView ShowFullyConfiguredWaveHeightLocationsView(IAssessmentSection assessmentSection, Form form)
        {
            GrassCoverErosionOutwardsWaveHeightLocationsView view = ShowWaveHeightLocationsView(assessmentSection, form);

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

            view.Data = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                },
                new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                },
                new HydraulicBoundaryLocation(5, "5", 5.0, 5.0)
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = output
                    }
                }
            };

            return view;
        }

        [TestFixture]
        private class ViewSynchronizationTest : LocationsViewSynchronizationTester<HydraulicBoundaryLocation>
        {
            protected override int OutputColumnIndex
            {
                get
                {
                    return locationWaveHeightColumnIndex;
                }
            }

            protected override object GetLocationSelection(LocationsView<HydraulicBoundaryLocation> view, object selectedRowObject)
            {
                return new GrassCoverErosionOutwardsWaveHeightLocationContext(((HydraulicBoundaryLocationRow) selectedRowObject).CalculatableObject,
                                                                              (ObservableList<HydraulicBoundaryLocation>) view.Data);
            }

            protected override LocationsView<HydraulicBoundaryLocation> ShowFullyConfiguredLocationsView(Form form)
            {
                return ShowFullyConfiguredWaveHeightLocationsView(new ObservableTestAssessmentSectionStub(), form);
            }

            protected override void ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
            {
                var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
                var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0);

                locations.Clear();
                locations.Add(hydraulicBoundaryLocation);
                locations.NotifyObservers();
            }

            protected override void ClearLocationOutputAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
            {
                var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

                locations.ForEach(loc => loc.WaveHeightCalculation.Output = null);
                locations.NotifyObservers();
            }

            protected override void AddLocationOutputAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
            {
                var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

                locations.First().WaveHeightCalculation.Output = new TestHydraulicBoundaryLocationOutput(new TestGeneralResultSubMechanismIllustrationPoint());
                locations.NotifyObservers();
            }
        }
    }
}