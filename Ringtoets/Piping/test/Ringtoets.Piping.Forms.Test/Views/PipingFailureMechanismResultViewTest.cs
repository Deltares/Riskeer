// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismResultViewTest
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
            // Call
            using (var view = new PipingFailureMechanismResultView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
            }
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
            using (var view = ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var points = new[]
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                };

                var section = new FailureMechanismSection("test", points);
                var sectionResult = new PipingFailureMechanismSectionResult(section);
                var testData = new List<PipingFailureMechanismSectionResult>
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
            using (var view = ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
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
            using (ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                var rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                var cells = rows[0].Cells;
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
            using (ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.Rows[0].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Assert
                var rows = dataGridView.Rows;

                var cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                var cellAssessmentLayerTwoA = cells[assessmentLayerTwoAIndex];
                var cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];

                Assert.AreEqual(assessmentLayerOneState, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cellAssessmentLayerTwoA.FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerThree.FormattedValue);

                if (assessmentLayerOneState == AssessmentLayerOneState.Sufficient)
                {
                    DataGridViewCellTestHelper.AssertCellIsDisabled(cellAssessmentLayerTwoA);
                    DataGridViewCellTestHelper.AssertCellIsDisabled(cellAssessmentLayerThree);

                    Assert.IsTrue(cellAssessmentLayerThree.ReadOnly);
                }
                else
                {
                    DataGridViewCellTestHelper.AssertCellIsEnabled(cellAssessmentLayerTwoA, true);
                    DataGridViewCellTestHelper.AssertCellIsEnabled(cellAssessmentLayerThree);

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
            using (ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

                // Assert
                Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [TestCase("1")]
        [TestCase("1e-6")]
        [TestCase("1e+6")]
        [TestCase("14.3")]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(string newValue)
        {
            // Setup
            using (var view = ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue;

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);

                var dataObject = view.Data as List<PipingFailureMechanismSectionResult>;
                Assert.IsNotNull(dataObject);
                var row = dataObject.First();

                const string propertyName = "AssessmentLayerThree";
                var propertyValue = row.GetType().GetProperty(propertyName).GetValue(row, null);

                Assert.AreEqual((RoundedDouble) double.Parse(newValue), propertyValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_TotalContributionNotHundred_ShowsErrorTooltip(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            var rowIndex = 0;

            var pipingFailureMechanism = new PipingFailureMechanism();
            using (PipingFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(pipingFailureMechanism))
            {
                PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(
                    1.0/1000.0,
                    pipingFailureMechanism.Sections.First());
                calculationScenario.Contribution = (RoundedDouble) 0.3;
                pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);
                view.Data = pipingFailureMechanism.SectionResults;

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
            var rowIndex = 0;

            var pipingFailureMechanism = new PipingFailureMechanism();
            using (PipingFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(pipingFailureMechanism))
            {
                PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(
                    (RoundedDouble) 1e-3,
                    pipingFailureMechanism.Sections.First());
                pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);
                view.Data = pipingFailureMechanism.SectionResults;

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Call
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.IsEmpty(dataGridViewCell.ErrorText);
                Assert.AreEqual(string.Format("1/{0:N0}", 1/calculationScenario.Probability),
                                formattedValue);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void FailureMechanismResultView_AssessmentLayerTwoANull_ShowsErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            var rowIndex = 0;

            var pipingFailureMechanism = new PipingFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(pipingFailureMechanism))
            {
                PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(
                    pipingFailureMechanism.Sections.First());
                pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

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
            var rowIndex = 0;

            var pipingFailureMechanism = new PipingFailureMechanism();
            using (PipingFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView(pipingFailureMechanism))
            {
                PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(
                    pipingFailureMechanism.Sections.First());
                pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);
                view.Data = pipingFailureMechanism.SectionResults;

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
            var rowIndex = 0;

            using (ShowFullyConfiguredFailureMechanismResultsView(new PipingFailureMechanism()))
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
            var rowIndex = 0;

            var pipingFailureMechanism = new PipingFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(pipingFailureMechanism))
            {
                PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreateIrreleveantPipingCalculationScenario(
                    pipingFailureMechanism.Sections.First());
                pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

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
        public void FailureMechanismResultView_AssessmentLayerOneStateSufficientAndAssessmentLayerTwoAHasError_DoesNotShowError()
        {
            // Setup
            var rowIndex = 0;

            var pipingFailureMechanism = new PipingFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(pipingFailureMechanism))
            {
                PipingCalculationScenario calculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(
                    pipingFailureMechanism.Sections.First());
                pipingFailureMechanism.CalculationsGroup.Children.Add(calculationScenario);

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

        private PipingFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView(PipingFailureMechanism failureMechanism)
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

            var failureMechanismResultView = ShowFailureMechanismResultsView();
            failureMechanismResultView.Data = failureMechanism.SectionResults;
            failureMechanismResultView.FailureMechanism = failureMechanism;

            return failureMechanismResultView;
        }

        private PipingFailureMechanismResultView ShowFailureMechanismResultsView()
        {
            PipingFailureMechanismResultView failureMechanismResultView = new PipingFailureMechanismResultView();
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}