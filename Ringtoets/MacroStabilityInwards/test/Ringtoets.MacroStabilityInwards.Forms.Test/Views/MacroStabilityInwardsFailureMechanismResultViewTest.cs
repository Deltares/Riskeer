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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerThreeIndex = 3;
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (var view = new MacroStabilityInwardsFailureMechanismResultView(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Call
            using (ShowFailureMechanismResultsView())
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[assessmentLayerTwoAIndex].ReadOnly);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Toetslaag 1", dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                Assert.AreEqual("Toetslaag 2a", dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                Assert.AreEqual("Toetslaag 3", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void Data_DataAlreadySetNewDataSet_DataSetAndDataGridViewUpdated()
        {
            // Setup
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var points = new[]
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                };

                var section = new FailureMechanismSection("test", points);
                var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);
                var testData = new List<MacroStabilityInwardsFailureMechanismSectionResult>
                {
                    sectionResult
                };

                // Precondition
                Assert.AreEqual(2, dataGridView.RowCount);

                // Call
                view.Data = testData;

                // Assert
                Assert.AreSame(testData, view.Data);

                Assert.AreEqual(testData.Count, dataGridView.RowCount);
                Assert.AreEqual(sectionResult.Section.Name, dataGridView.Rows[0].Cells[0].Value);
            }
        }

        [Test]
        public void Data_SetOtherThanFailureMechanismSectionResultListData_DataNullAndEmptyGrid()
        {
            // Setup
            var testData = new object();
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                view.Data = testData;

                // Assert
                Assert.IsNull(view.Data);

                Assert.AreEqual(0, dataGridView.RowCount);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(AssessmentLayerOneState.NotAssessed, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(AssessmentLayerOneState.NotAssessed, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        [TestCase(AssessmentLayerOneState.Sufficient)]
        public void FailureMechanismResultsView_ChangeCheckBox_DataGridViewCorrectlySyncedAndStylingSet(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.Rows[0].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                DataGridViewCell cellAssessmentLayerTwoA = cells[assessmentLayerTwoAIndex];
                DataGridViewCell cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];

                Assert.AreEqual(assessmentLayerOneState, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cellAssessmentLayerTwoA.FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerThree.FormattedValue);

                if (assessmentLayerOneState == AssessmentLayerOneState.Sufficient)
                {
                    DataGridViewTestHelper.AssertCellIsDisabled(cellAssessmentLayerTwoA);
                    DataGridViewTestHelper.AssertCellIsDisabled(cellAssessmentLayerThree);

                    Assert.IsTrue(cellAssessmentLayerThree.ReadOnly);
                }
                else
                {
                    DataGridViewTestHelper.AssertCellIsEnabled(cellAssessmentLayerTwoA, true);
                    DataGridViewTestHelper.AssertCellIsEnabled(cellAssessmentLayerThree);

                    Assert.IsFalse(cellAssessmentLayerThree.ReadOnly);
                }
            }
        }

        [Test]
        [TestCase("test", assessmentLayerThreeIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", assessmentLayerThreeIndex)]
        public void FailureMechanismResultView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

                // Assert
                Assert.AreEqual("De waarde kon niet geïnterpreteerd worden als een kans.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1.01)]
        [TestCase(-0.01)]
        [TestCase(5)]
        [TestCase(-10)]
        public void FailureMechanismResultView_EditValueAssessmentLayerThreeInvalid_ShowErrorToolTip(double newValue)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.AreEqual("Kans moet in het bereik [0,0, 1,0] liggen.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(1)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void FailureMechanismResultView_EditValueAssessmentLayerThreeValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);

                var dataObject = view.Data as List<MacroStabilityInwardsFailureMechanismSectionResult>;
                Assert.IsNotNull(dataObject);
                MacroStabilityInwardsFailureMechanismSectionResult row = dataObject.First();
                Assert.AreEqual(newValue, row.AssessmentLayerThree);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_TotalContributionNotHundred_ShowsErrorTooltip(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            const int rowIndex = 0;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                    1.0 / 1000.0,
                    failureMechanism.Sections.First());
                calculationScenario.Contribution = (RoundedDouble) 0.3;
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario);
                view.Data = failureMechanism.SectionResults;

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak moet opgeteld gelijk zijn aan 100%.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_AssessmentLayerTwoAHasValue_DoesNotShowsErrorTooltip(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            const int rowIndex = 0;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                    (RoundedDouble) 1e-3,
                    failureMechanism.Sections.First());
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario);
                view.Data = failureMechanism.SectionResults;

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.IsEmpty(dataGridViewCell.ErrorText);
                Assert.AreEqual($"1/{1 / 1}",
                                formattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_AssessmentLayerTwoANull_ShowsErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            const int rowIndex = 0;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(
                    failureMechanism.Sections.First());
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Alle berekeningen voor dit vak moeten uitgevoerd zijn.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_AssessmentLayerTwoANaN_ShowsErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            const int rowIndex = 0;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(
                    failureMechanism.Sections.First());
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario);
                view.Data = failureMechanism.SectionResults;

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Alle berekeningen voor dit vak moeten een geldige uitkomst hebben.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_NoCalculationScenarios_ShowsErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            const int rowIndex = 0;

            using (ShowFullyConfiguredFailureMechanismResultsView(new MacroStabilityInwardsFailureMechanism()))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_NoCalculationScenariosRelevant_ShowsErrorTooltip(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            const int rowIndex = 0;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(
                    failureMechanism.Sections.First());
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden geselecteerd.",
                                dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultView_AssessmentLayerOneStateSufficientAndAssessmentLayerTwoAHasNaNOutput_DoesNotShowError()
        {
            // Setup
            const int rowIndex = 0;

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(
                    failureMechanism.Sections.First());
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = AssessmentLayerOneState.Sufficient;
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.IsEmpty(dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void GivenFailureMechanismResultView_WhenFailureMechanismNotifiesObserver_ThenViewUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            using (MacroStabilityInwardsFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                MacroStabilityInwardsCalculationScenario calculationScenario1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                    0.1,
                    failureMechanism.Sections.First());
                calculationScenario1.Contribution = (RoundedDouble) 0.6;
                MacroStabilityInwardsCalculationScenario calculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(
                    0.2,
                    failureMechanism.Sections.First());
                calculationScenario2.Contribution = (RoundedDouble) 0.4;
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario1);
                failureMechanism.CalculationsGroup.Children.Add(calculationScenario2);

                view.Data = failureMechanism.SectionResults;

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                MacroStabilityInwardsFailureMechanismSectionResultRow[] sectionResultRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                                        .Select(r => r.DataBoundItem)
                                                                                                        .Cast<MacroStabilityInwardsFailureMechanismSectionResultRow>()
                                                                                                        .ToArray();

                // When
                failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput.A = 0.01;
                failureMechanism.NotifyObservers();

                // Then
                MacroStabilityInwardsFailureMechanismSectionResultRow[] updatedRows = dataGridView.Rows.Cast<DataGridViewRow>()
                                                                                                  .Select(r => r.DataBoundItem)
                                                                                                  .Cast<MacroStabilityInwardsFailureMechanismSectionResultRow>()
                                                                                                  .ToArray();

                CollectionAssert.AreNotEquivalent(sectionResultRows, updatedRows);
            }
        }

        private MacroStabilityInwardsFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            failureMechanism.AddSection(new FailureMechanismSection("Section 1", new List<Point2D>
            {
                new Point2D(0.0, 0.0),
                new Point2D(5.0, 0.0)
            }));

            failureMechanism.AddSection(new FailureMechanismSection("Section 2", new List<Point2D>
            {
                new Point2D(5.0, 0.0),
                new Point2D(10.0, 0.0)
            }));

            MacroStabilityInwardsFailureMechanismResultView failureMechanismResultView = ShowFailureMechanismResultsView();
            failureMechanismResultView.Data = failureMechanism.SectionResults;
            failureMechanismResultView.FailureMechanism = failureMechanism;

            return failureMechanismResultView;
        }

        private MacroStabilityInwardsFailureMechanismResultView ShowFailureMechanismResultsView()
        {
            var failureMechanismResultView = new MacroStabilityInwardsFailureMechanismResultView(new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}