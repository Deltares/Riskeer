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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismResultViewTest
    {
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
                Assert.IsNull(view.FailureMechanism);
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

                Assert.AreEqual(5, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[assessmentLayerTwoAIndex].ReadOnly);

                foreach (var column in dataGridView.Columns.OfType<DataGridViewComboBoxColumn>())
                {
                    Assert.AreEqual("This", column.ValueMember);
                    Assert.AreEqual("DisplayName", column.DisplayMember);
                }

                foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
                {
                    Assert.AreEqual(DataGridViewAutoSizeColumnMode.AllCells, column.AutoSizeMode);
                    Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, column.HeaderCell.Style.Alignment);
                }
            }
        }

        [Test]
        public void Data_DataAlreadySetNewDataSet_DataSetAndDataGridViewUpdated()
        {
            // Setup
            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
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
        public void Data_SetOtherThanFailureMechanismSectionResultListData_DataNullAndDataGridViewOneEmtpyRow()
        {
            // Setup
            var testData = new object();
            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                view.Data = testData;

                // Assert
                Assert.IsNull(view.Data);

                Assert.AreEqual(1, dataGridView.RowCount);
                foreach (DataGridViewCell cell in dataGridView.Rows[0].Cells)
                {
                    Assert.IsNull(cell.Value);
                }
            }
        }

        [Test]
        public void Dispose_FailureMechanismResultViewWithAdditionalPropertiesSet_AdditionalPropertiesSetToNull()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new SimpleFailureMechanism();
            mocks.ReplayAll();
            using (var view = new PipingFailureMechanismResultView
            {
                FailureMechanism = failureMechanism
            })
            {
                // Precondition
                Assert.IsNotNull(view.FailureMechanism);

                // Call
                view.Dispose();

                // Assert
                Assert.IsNull(view.FailureMechanism);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Assert
                var rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(5, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.IsFalse((bool) cells[assessmentLayerOneIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", 0), cells[assessmentLayerTwoBIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", 0), cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(5, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.IsFalse((bool) cells[assessmentLayerOneIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", 0), cells[assessmentLayerTwoBIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", 0), cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void FailureMechanismResultsView_ChangeCheckBox_DataGridViewCorrectlySyncedAndStylingSet(bool checkBoxSelected)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                dataGridView.Rows[0].Cells[assessmentLayerOneIndex].Value = checkBoxSelected;

                // Assert
                var rows = dataGridView.Rows;

                var cells = rows[0].Cells;
                Assert.AreEqual(5, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                var cellAssessmentLayerTwoA = cells[assessmentLayerTwoAIndex];
                var cellAssessmentLayerTwoB = cells[assessmentLayerTwoBIndex];
                var cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];

                Assert.AreEqual(checkBoxSelected, (bool) cells[assessmentLayerOneIndex].FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerTwoA.FormattedValue);
                Assert.AreEqual(string.Format("{0}", 0), cellAssessmentLayerTwoB.FormattedValue);
                Assert.AreEqual(string.Format("{0}", 0), cellAssessmentLayerThree.FormattedValue);

                var cellAssessmentLayerTwoABackColor = cellAssessmentLayerTwoA.Style.BackColor;
                var cellAssessmentLayerTwoAForeColor = cellAssessmentLayerTwoA.Style.ForeColor;
                var cellAssessmentLayerTwoBBackColor = cellAssessmentLayerTwoB.Style.BackColor;
                var cellAssessmentLayerTwoBForeColor = cellAssessmentLayerTwoB.Style.ForeColor;
                var cellAssessmentLayerThreeBackColor = cellAssessmentLayerThree.Style.BackColor;
                var cellAssessmentLayerThreeForeColor = cellAssessmentLayerThree.Style.ForeColor;

                if (checkBoxSelected)
                {
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), cellAssessmentLayerTwoABackColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), cellAssessmentLayerTwoAForeColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), cellAssessmentLayerTwoBBackColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), cellAssessmentLayerTwoBForeColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), cellAssessmentLayerThreeBackColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), cellAssessmentLayerThreeForeColor);
                }
                else
                {
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.White), cellAssessmentLayerTwoABackColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), cellAssessmentLayerTwoAForeColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.White), cellAssessmentLayerTwoBBackColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), cellAssessmentLayerTwoBForeColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.White), cellAssessmentLayerThreeBackColor);
                    Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), cellAssessmentLayerThreeForeColor);
                }

                Assert.AreEqual(checkBoxSelected, cellAssessmentLayerTwoB.ReadOnly);
                Assert.AreEqual(checkBoxSelected, cellAssessmentLayerThree.ReadOnly);
            }
        }

        [Test]
        [TestCase("test", assessmentLayerTwoBIndex)]
        [TestCase("test", assessmentLayerThreeIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", assessmentLayerTwoBIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", assessmentLayerThreeIndex)]
        public void FailureMechanismResultView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

                // Assert
                Assert.AreEqual("De tekst moet een getal zijn.", dataGridView.Rows[0].ErrorText);
            }
        }

        [Test]
        [TestCase("1", assessmentLayerTwoBIndex, "AssessmentLayerTwoB")]
        [TestCase("1e-6", assessmentLayerTwoBIndex, "AssessmentLayerTwoB")]
        [TestCase("1e+6", assessmentLayerTwoBIndex, "AssessmentLayerTwoB")]
        [TestCase("14.3", assessmentLayerTwoBIndex, "AssessmentLayerTwoB")]
        [TestCase("1", assessmentLayerThreeIndex, "AssessmentLayerThree")]
        [TestCase("1e-6", assessmentLayerThreeIndex, "AssessmentLayerThree")]
        [TestCase("1e+6", assessmentLayerThreeIndex, "AssessmentLayerThree")]
        [TestCase("14.3", assessmentLayerThreeIndex, "AssessmentLayerThree")]
        public void FailureMechanismResultView_EditValueValid_DoNotShowErrorToolTipAndEditValue(string newValue, int cellIndex, string propertyName)
        {
            // Setup
            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[cellIndex].Value = newValue;

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);

                var dataObject = view.Data as List<PipingFailureMechanismSectionResult>;
                Assert.IsNotNull(dataObject);
                var row = dataObject.First();

                var propertyValue = row.GetType().GetProperty(propertyName).GetValue(row, null);

                Assert.AreEqual((RoundedDouble) double.Parse(newValue), propertyValue);
            }
        }

        [Test]
        public void FailureMechanismResultView_EditValueDirtyStateChangedEventFired_ValueCommittedCellInEditMode()
        {
            // Setup
            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].AssessmentLayerOne = false;

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;
                var dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerOneIndex];

                dataGridView.CurrentCell = dataGridViewCell;
                dataGridView.BeginEdit(false);
                gridTester.FireEvent("KeyUp", new KeyEventArgs(Keys.Space));

                // Call
                gridTester.FireEvent("CurrentCellDirtyStateChanged", EventArgs.Empty);

                // Assert
                Assert.IsTrue(dataGridViewCell.IsInEditMode);
                Assert.IsTrue(sections[0].AssessmentLayerOne);
            }
        }

        [Test]
        public void FailureMechanismResultView_TotalContributionNotHundred_ShowsErrorTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 0.3);
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Probability).Return((RoundedDouble) 1000);

            mocks.ReplayAll();

            var rowIndex = 0;

            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].CalculationScenarios.Add(calculationScenarioMock);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Bijdrage van de geselecteerde scenario's voor dit vak is opgeteld niet gelijk aan 100%.", dataGridViewCell.ErrorText);
                Assert.AreEqual(double.NaN.ToString(CultureInfo.InvariantCulture), formattedValue);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void FailureMechanismResultView_AssessmentLayerTwoAHasValue_DoesNotShowsErrorTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Probability).Return((RoundedDouble) 1000);
            calculationScenarioMock.Stub(cs => cs.CalculationScenarioStatus).Return(CalculationScenarioStatus.Done);

            mocks.ReplayAll();

            var rowIndex = 0;

            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].CalculationScenarios.Add(calculationScenarioMock);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual(string.Empty, dataGridViewCell.ErrorText);
                Assert.AreEqual(string.Format("1/{0:N0}", calculationScenarioMock.Probability), formattedValue);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void FailureMechanismResultView_AssessmentLayerTwoANull_ShowsErrorTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.CalculationScenarioStatus).Return(CalculationScenarioStatus.NotCalculated);

            mocks.ReplayAll();

            var rowIndex = 0;

            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].CalculationScenarios.Add(calculationScenarioMock);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Niet alle berekeningen voor dit vak zijn uitgevoerd.", dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void FailureMechanismResultView_AssessmentLayerTwoANaN_ShowsErrorTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.CalculationScenarioStatus).Return(CalculationScenarioStatus.Failed);

            mocks.ReplayAll();

            var rowIndex = 0;

            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].CalculationScenarios.Add(calculationScenarioMock);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual("Niet alle berekeningen voor dit vak hebben een geldige uitkomst.", dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void FailureMechanismResultView_NoCalculationScenarios_DoesNotShowErrorTooltip()
        {
            // Setup
            var rowIndex = 0;

            using (ShowFullyConfiguredFailureMechanismResultsView())
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual(string.Empty, dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultView_NoCalculationScenariosRelevant_DoesNotShowErrorTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(false);
            calculationScenarioMock.Stub(cs => cs.Probability).Return((RoundedDouble) double.NaN);

            mocks.ReplayAll();

            var rowIndex = 0;

            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].CalculationScenarios.Add(calculationScenarioMock);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual(string.Empty, dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
            }
        }

        [Test]
        public void FailureMechanismResultView_AssessmentLayerOneTrueAndAssessmentLayerTwoAHasError_DoesNotShowError()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Probability).Return((RoundedDouble) double.NaN);
            calculationScenarioMock.Stub(cs => cs.CalculationScenarioStatus).Return(CalculationScenarioStatus.Failed);

            mocks.ReplayAll();

            var rowIndex = 0;

            using (var view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var sections = (List<PipingFailureMechanismSectionResult>) view.Data;
                sections[0].CalculationScenarios.Add(calculationScenarioMock);

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[rowIndex].Cells[assessmentLayerTwoAIndex];

                // Call
                dataGridView.Rows[rowIndex].Cells[assessmentLayerOneIndex].Value = true;
                var formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Assert
                Assert.AreEqual(string.Empty, dataGridViewCell.ErrorText);
                Assert.AreEqual("-", formattedValue);
                mocks.VerifyAll();
            }
        }

        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerTwoBIndex = 3;
        private const int assessmentLayerThreeIndex = 4;

        private PipingFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView()
        {
            var failureMechanism = new SimpleFailureMechanism();

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

        private class SimpleFailureMechanism : FailureMechanismBase<PipingFailureMechanismSectionResult>
        {
            public SimpleFailureMechanism() : base("Stubbed name", "Stubbed code") {}

            public override IEnumerable<ICalculation> Calculations
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            protected override PipingFailureMechanismSectionResult CreateFailureMechanismSectionResult(FailureMechanismSection section)
            {
                return new PipingFailureMechanismSectionResult(section);
            }
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