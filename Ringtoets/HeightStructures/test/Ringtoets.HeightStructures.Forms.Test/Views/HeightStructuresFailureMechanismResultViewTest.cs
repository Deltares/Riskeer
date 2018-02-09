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

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.Views;

namespace Ringtoets.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresFailureMechanismResultViewTest
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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanismSectionResults = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>();

            // Call
            using (var view = new HeightStructuresFailureMechanismResultView(assessmentSection, failureMechanismSectionResults))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<StructuresFailureMechanismSectionResult<HeightStructuresInput>>>(view);
                Assert.AreSame(failureMechanismSectionResults, view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenFormWithHeightStructuresFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Call
            using (ShowFailureMechanismResultsView(new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>()))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[assessmentLayerTwoAIndex].ReadOnly);

                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void Data_DataAlreadySetNewDataSet_DataSetAndDataGridViewUpdated()
        {
            // Setup
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                var points = new[]
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                };

                var section = new FailureMechanismSection("test", points);
                var sectionResult = new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section);
                var testData = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
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
        public void Data_SetOtherThanFailureMechanismSectionResultListData_DataNullAndDataGridViewEmpty()
        {
            // Setup
            var testData = new object();
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
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
            using (ShowFullyConfiguredFailureMechanismResultsView())
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
        public void FailureMechanismResultsView_ChangeAssessmentLayerOneState_DataGridViewCorrectlySyncedAndStylingSet(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                DataGridViewCell cellAssessmentLayerTwoA = cells[assessmentLayerTwoAIndex];
                DataGridViewCell cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];
                DataGridViewCell dataGridViewCell = cells[assessmentLayerOneIndex];

                Assert.AreEqual(assessmentLayerOneState, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cellAssessmentLayerTwoA.FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerThree.FormattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);

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
            using (ShowFullyConfiguredFailureMechanismResultsView())
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
            using (ShowFullyConfiguredFailureMechanismResultsView())
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
        [TestCase(0)]
        [TestCase(0.5)]
        [TestCase(1e-6)]
        [TestCase(double.NaN)]
        public void FailureMechanismResultView_EditValueAssessmentLayerThreeValid_DoNotShowErrorToolTipAndEditValue(double newValue)
        {
            // Setup
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);

                var dataObject = view.Data as List<StructuresFailureMechanismSectionResult<HeightStructuresInput>>;
                Assert.IsNotNull(dataObject);
                StructuresFailureMechanismSectionResult<HeightStructuresInput> row = dataObject.First();
                Assert.AreEqual(newValue, row.AssessmentLayerThree);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultWithoutCalculation_ThenLayerTwoAErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
                {
                    AssessmentLayerOne = assessmentLayerOneState
                };
                view.Data = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("Er moet een maatgevende berekening voor dit vak worden geselecteerd.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultAndCalculationNotCalculated_ThenLayerTwoAErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var calculation = new StructuresCalculation<HeightStructuresInput>();
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
                {
                    Calculation = calculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet nog worden uitgevoerd.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultAndFailedCalculation_ThenLayerTwoAErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var calculation = new StructuresCalculation<HeightStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                };
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
                {
                    Calculation = calculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultAndSuccessfulCalculation_ThenLayerTwoANoError(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var calculation = new StructuresCalculation<HeightStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                };
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
                {
                    Calculation = calculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual(ProbabilityFormattingHelper.Format(0.25), formattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCaseSource(nameof(AssessmentLayerOneStateIsSufficientVariousSections))]
        public void GivenSectionResultAndAssessmentLayerOneStateSufficient_ThenLayerTwoANoError(
            StructuresFailureMechanismSectionResult<HeightStructuresInput> sectionResult, string expectedValue)
        {
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                view.Data = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual(expectedValue, formattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultAndSuccessfulCalculation_WhenChangingCalculationToFailed_ThenLayerTwoAHasError(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (HeightStructuresFailureMechanismResultView view = ShowFullyConfiguredFailureMechanismResultsView())
            {
                var successfulCalculation = new StructuresCalculation<HeightStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                };

                var failedCalculation = new StructuresCalculation<HeightStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                };
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
                {
                    Calculation = successfulCalculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new ObservableList<StructuresFailureMechanismSectionResult<HeightStructuresInput>>
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // Precondition
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(ProbabilityFormattingHelper.Format(0.25), formattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);

                // When
                sectionResult.Calculation = failedCalculation;
                formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.", dataGridViewCell.ErrorText);
            }
        }

        private static IEnumerable AssessmentLayerOneStateIsSufficientVariousSections()
        {
            FailureMechanismSection section = CreateSimpleFailureMechanismSection();

            yield return new TestCaseData(new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient
            }, "-").SetName("SectionWithoutCalculation");
            yield return new TestCaseData(new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<HeightStructuresInput>()
            }, "-").SetName("SectionWithCalculationNoOutput");
            yield return new TestCaseData(new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<HeightStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                }
            }, "-").SetName("SectionWithInvalidCalculationOutput");
            yield return new TestCaseData(new StructuresFailureMechanismSectionResult<HeightStructuresInput>(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<HeightStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                }
            }, ProbabilityFormattingHelper.Format(0.25)).SetName("SectionWithValidCalculationOutput");
        }

        private static FailureMechanismSection CreateSimpleFailureMechanismSection()
        {
            var section = new FailureMechanismSection("A",
                                                      new[]
                                                      {
                                                          new Point2D(1, 2),
                                                          new Point2D(3, 4)
                                                      });
            return section;
        }

        private HeightStructuresFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView()
        {
            var failureMechanism = new HeightStructuresFailureMechanism();

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

            HeightStructuresFailureMechanismResultView failureMechanismResultView = ShowFailureMechanismResultsView(failureMechanism.SectionResults);
            failureMechanismResultView.Data = failureMechanism.SectionResults;
            failureMechanismResultView.FailureMechanism = failureMechanism;

            return failureMechanismResultView;
        }

        private HeightStructuresFailureMechanismResultView ShowFailureMechanismResultsView(
            IObservableEnumerable<StructuresFailureMechanismSectionResult<HeightStructuresInput>> sectionResults)
        {
            var failureMechanismResultView = new HeightStructuresFailureMechanismResultView(new ObservableTestAssessmentSectionStub(), sectionResults);
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}