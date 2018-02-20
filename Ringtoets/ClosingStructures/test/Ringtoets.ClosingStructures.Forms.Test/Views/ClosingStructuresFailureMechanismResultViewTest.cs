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
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;

namespace Ringtoets.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentIndex = 1;
        private const int detailedAssessmentIndex = 2;
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

            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            using (var view = new ClosingStructuresFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<ClosingStructuresFailureMechanismSectionResult,
                    ClosingStructuresFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            TestDelegate call = () => new ClosingStructuresFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenAllDataSet_ThenDataGridViewCorrectlyInitialized()
        {
            // Given & When
            using (CreateConfiguredFailureMechanismResultsView())
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        public void FailureMechanismResultsView_ChangeSimpleAssessmentResult_DataGridViewCorrectlySyncedAndStylingSet(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            using (CreateConfiguredFailureMechanismResultsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[simpleAssessmentIndex].Value = simpleAssessmentResult;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                DataGridViewCell cellDetailedAssessment = cells[detailedAssessmentIndex];
                DataGridViewCell cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];
                DataGridViewCell dataGridViewCell = cells[simpleAssessmentIndex];

                Assert.AreEqual(simpleAssessmentResult, dataGridViewCell.Value);
                Assert.AreEqual("-", cellDetailedAssessment.FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerThree.FormattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);

                if (simpleAssessmentResult == SimpleAssessmentResultType.NotApplicable
                    || simpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible)
                {
                    DataGridViewTestHelper.AssertCellIsDisabled(cellDetailedAssessment);
                    DataGridViewTestHelper.AssertCellIsDisabled(cellAssessmentLayerThree);

                    Assert.IsTrue(cellAssessmentLayerThree.ReadOnly);
                }
                else
                {
                    DataGridViewTestHelper.AssertCellIsEnabled(cellDetailedAssessment, true);
                    DataGridViewTestHelper.AssertCellIsEnabled(cellAssessmentLayerThree);

                    Assert.IsFalse(cellAssessmentLayerThree.ReadOnly);
                }
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowFailureMechanismResultsView(
                new ObservableList<ClosingStructuresFailureMechanismSectionResult>()))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[detailedAssessmentIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultView_WithClosingStructuresFailureMechanismSectionResultAssigned_SectionsAddedAsRows()
        {
            // Setup
            var random = new Random(21);
            var result1 = new ClosingStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result2 = new ClosingStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 2"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result3 = new ClosingStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 3"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.None,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result4 = new ClosingStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 4"))
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                AssessmentLayerThree = random.NextRoundedDouble()
            };

            // Call
            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                result1,
                result2,
                result3,
                result4
            }))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(4, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);

                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result1.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result1.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result3.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[detailedAssessmentIndex], true);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[3].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 4", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result4.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result4.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[detailedAssessmentIndex], true);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, TestName = "FormWithFailureMechanismResultView_WhenSectionBecomesNotApplicableAndListenersNotified_RowsForSectionDisabled(None)")]
        [TestCase(SimpleAssessmentResultType.AssessFurther, TestName = "FormWithFailureMechanismResultView_WhenSectionBecomesNotApplicableAndListenersNotified_RowsForSectionDisabled(AssessFurther)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionBecomesNotApplicableAndListenersNotified_ThenRowsForSectionDisabled(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var random = new Random(21);
            var result = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                AssessmentLayerThree = random.NextRoundedDouble()
            };

            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                result
            }))
            {
                // When
                result.SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, TestName = "FormWithFailureMechanismResultView_WhenSectionBecomesProbabilityNegligibleAndListenersNotified_RowsForSectionDisabled(None)")]
        [TestCase(SimpleAssessmentResultType.AssessFurther, TestName = "FormWithFailureMechanismResultView_WhenSectionBecomesProbabilityNegligibleAndListenersNotified_RowsForSectionDisabled(AssessFurther)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionBecomesProbabilityNegligibleAndListenersNotified_ThenRowsForSectionDisabled(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var random = new Random(21);
            var result = new ClosingStructuresFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                AssessmentLayerThree = random.NextRoundedDouble()
            };

            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                result
            }))
            {
                // When
                result.SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void GivenSectionResultWithoutCalculation_ThenLayerTwoAErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<ClosingStructuresFailureMechanismSectionResult>
                {
                    sectionResult
                }))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[detailedAssessmentIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("Er moet een maatgevende berekening voor dit vak worden geselecteerd.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void GivenSectionResultAndCalculationNotCalculated_ThenDetailedAssessmentErrorTooltip(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>(),
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                sectionResult
            }))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[detailedAssessmentIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet nog worden uitgevoerd.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void GivenSectionResultAndFailedCalculation_ThenDetailedAssessmentErrorTooltip(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                },
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<ClosingStructuresFailureMechanismSectionResult>
                {
                    sectionResult
                }))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[detailedAssessmentIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void GivenSectionResultAndSuccessfulCalculation_ThenDetailedAssessmentNoError(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                },
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                sectionResult
            }))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[detailedAssessmentIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual(ProbabilityFormattingHelper.Format(0.25), formattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetVariousSimpleAssessmentResultConfigurationsWithoutErrorMessage))]
        public void GivenSectionResultAndAssessmentLayerOneStateSufficient_ThenDetailedAssessmentNoError(
            ClosingStructuresFailureMechanismSectionResult sectionResult, string expectedValue)
        {
            // Given
            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                sectionResult
            }))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[detailedAssessmentIndex];

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
        public void GivenSectionResultAndSuccessfulCalculation_WhenChangingCalculationToFailed_ThenDetailedAssessmentHasError(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                },
                AssessmentLayerOne = assessmentLayerOneState
            };
            using (ShowFailureMechanismResultsView(
                new ObservableList<ClosingStructuresFailureMechanismSectionResult>
                {
                    sectionResult
                }))
            {
                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[detailedAssessmentIndex];

                // Precondition
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(ProbabilityFormattingHelper.Format(0.25), formattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);

                // When
                sectionResult.Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                };
                formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.", dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCase("test", assessmentLayerThreeIndex)]
        [TestCase(";/[].,~!@#$%^&*()_-+={}|?", assessmentLayerThreeIndex)]
        public void FailureMechanismResultView_EditValueInvalid_ShowsErrorTooltip(string newValue, int cellIndex)
        {
            // Setup
            using (CreateConfiguredFailureMechanismResultsView())
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
            using (CreateConfiguredFailureMechanismResultsView())
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
            var result = new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (ShowFailureMechanismResultsView(new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                result
            }))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
                Assert.AreEqual(newValue, result.AssessmentLayerThree);
            }
        }

        private static IEnumerable GetVariousSimpleAssessmentResultConfigurationsWithoutErrorMessage()
        {
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible
            }, "-").SetName("SectionWithoutCalculationAndSimpleAssessmentResultProbabilityNegligible");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible,
                Calculation = new StructuresCalculation<ClosingStructuresInput>()
            }, "-").SetName("SectionWithCalculationNoOutputAndSimpleAssessmentResultProbabilityNegligible");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible,
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                }
            }, "-").SetName("SectionWithInvalidCalculationOutputAndSimpleAssessmentResultProbabilityNegligible");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible,
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                }
            }, ProbabilityFormattingHelper.Format(0.25)).SetName("SectionWithValidCalculationOutputAndSimpleAssessmentResultProbabilityNegligible");

            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable
            }, "-").SetName("SectionWithoutCalculationAndSimpleAssessmentResultNotApplicable");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                Calculation = new StructuresCalculation<ClosingStructuresInput>()
            }, "-").SetName("SectionWithCalculationNoOutputAndSimpleAssessmentResultNotApplicable");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                }
            }, "-").SetName("SectionWithInvalidCalculationOutputAndSimpleAssessmentResultNotApplicable");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                }
            }, ProbabilityFormattingHelper.Format(0.25)).SetName("SectionWithValidCalculationOutputAndSimpleAssessmentResultNotApplicable");
        }

        private ClosingStructuresFailureMechanismResultView CreateConfiguredFailureMechanismResultsView()
        {
            var results = new ObservableList<ClosingStructuresFailureMechanismSectionResult>
            {
                new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1")),
                new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 2"))
            };

            ClosingStructuresFailureMechanismResultView failureMechanismResultView = ShowFailureMechanismResultsView(results);

            return failureMechanismResultView;
        }

        private ClosingStructuresFailureMechanismResultView ShowFailureMechanismResultsView(
            IObservableEnumerable<ClosingStructuresFailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new ClosingStructuresFailureMechanismResultView(sectionResults,
                                                                                             new ClosingStructuresFailureMechanism(),
                                                                                             new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}