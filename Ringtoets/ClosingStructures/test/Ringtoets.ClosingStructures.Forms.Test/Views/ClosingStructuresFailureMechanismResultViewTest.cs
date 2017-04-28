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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.ClosingStructures.Forms.Test.Views
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismResultViewTest
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
            using (var view = new ClosingStructuresFailureMechanismResultView())
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<ClosingStructuresFailureMechanismSectionResult>>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_DataAlreadySetNewDataSet_DataSetAndDataGridViewUpdated()
        {
            // Setup
            using (ClosingStructuresFailureMechanismResultView view = CreateConfiguredFailureMechanismResultsView())
            {
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);
                var testData = new List<ClosingStructuresFailureMechanismSectionResult>
                {
                    sectionResult
                };

                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

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
            using (ClosingStructuresFailureMechanismResultView view = CreateConfiguredFailureMechanismResultsView())
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
        public void GivenFailureMechanismResultsView_WhenAllDataSet_ThenDataGridViewCorrectlyInitialized()
        {
            // Given
            using (CreateConfiguredFailureMechanismResultsView())
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

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
        [SetCulture("nl-NL")]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        [TestCase(AssessmentLayerOneState.Sufficient)]
        public void FailureMechanismResultsView_ChangeAssessmentLayerOneState_DataGridViewCorrectlySyncedAndStylingSet(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            using (CreateConfiguredFailureMechanismResultsView())
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

                Assert.AreEqual(assessmentLayerOneState, dataGridViewCell.Value);
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
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowFailureMechanismResultsView())
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);

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
        public void GivenFormWithFailureMechanismResultView_WhenDataSourceWithClosingStructureFailureMechanismSectionResultAssigned_ThenSectionsAddedAsRows()
        {
            // Given
            var section1 = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            });
            var section2 = new FailureMechanismSection("Section 2", new[]
            {
                new Point2D(0, 0)
            });
            var section3 = new FailureMechanismSection("Section 3", new[]
            {
                new Point2D(0, 0)
            });

            var random = new Random(21);
            var result1 = new ClosingStructuresFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result2 = new ClosingStructuresFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NotAssessed,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result3 = new ClosingStructuresFailureMechanismSectionResult(section3)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NoVerdict,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };

            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                // When
                view.Data = new[]
                {
                    result1,
                    result2,
                    result3
                };

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(3, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);

                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex], true);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex], true);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed, TestName = "FormWithFailureMechanismResultView_SectionPassesLevel0AndListenersNotified_RowsForSectionDisabled(notAssessed)")]
        [TestCase(AssessmentLayerOneState.NoVerdict, TestName = "FormWithFailureMechanismResultView_SectionPassesLevel0AndListenersNotified_RowsForSectionDisabled(noVerdict)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionPassesLevel0AndListenersNotified_ThenRowsForSectionDisabled(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var section = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            });
            var random = new Random(21);
            var result = new ClosingStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = assessmentLayerOneState,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                view.Data = new[]
                {
                    result
                };

                // When
                result.AssessmentLayerOne = AssessmentLayerOneState.Sufficient;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_WhenDataSourceWithOtherFailureMechanismSectionResultAssigned_ThenSectionsNotAdded()
        {
            // Given
            FailureMechanismSection section1 = CreateSimpleFailureMechanismSection();
            FailureMechanismSection section2 = CreateSimpleFailureMechanismSection();
            var result1 = new TestFailureMechanismSectionResult(section1);
            var result2 = new TestFailureMechanismSectionResult(section2);

            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                // When
                view.Data = new[]
                {
                    result1,
                    result2
                };

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(0, rows.Count);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultWithoutCalculation_ThenLayerTwoAErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
                {
                    AssessmentLayerOne = assessmentLayerOneState
                };
                view.Data = new[]
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
        public void GivenSectionResultAndCalculationNotCalculated_ThenLayerTwoAErrorTooltip(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                var calculation = new StructuresCalculation<ClosingStructuresInput>();
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
                {
                    Calculation = calculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new[]
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
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                var calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, double.NaN, 1.0, 1.0)
                };
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
                {
                    Calculation = calculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new[]
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
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                const double probability = 0.56789;
                var calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, probability, 1.0, 1.0)
                };
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
                {
                    Calculation = calculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new[]
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // When
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual(ProbabilityFormattingHelper.Format(probability), formattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);
            }
        }

        [Test]
        [TestCaseSource("AssessmentLayerOneStateIsSufficientVariousSections")]
        public void GivenSectionResultAndAssessmentLayerOneStateSufficient_ThenLayerTwoANoError(
            ClosingStructuresFailureMechanismSectionResult sectionResult, string expectedValue)
        {
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                view.Data = new[]
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
            using (ClosingStructuresFailureMechanismResultView view = ShowFailureMechanismResultsView())
            {
                const double probability = 0.56789;
                var successfulCalculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, probability, 1.0, 1.0)
                };

                var failedCalculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, double.NaN, 1.0, 1.0)
                };
                FailureMechanismSection section = CreateSimpleFailureMechanismSection();
                var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section)
                {
                    Calculation = successfulCalculation,
                    AssessmentLayerOne = assessmentLayerOneState
                };

                view.Data = new[]
                {
                    sectionResult
                };

                var gridTester = new ControlTester("dataGridView");
                var dataGridView = (DataGridView) gridTester.TheObject;

                DataGridViewCell dataGridViewCell = dataGridView.Rows[0].Cells[assessmentLayerTwoAIndex];

                // Precondition
                object formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.
                Assert.AreEqual(ProbabilityFormattingHelper.Format(probability), formattedValue);
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
            const double probability = 0.56789;

            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient
            }, "-").SetName("SectionWithoutCalculation");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<ClosingStructuresInput>()
            }, "-").SetName("SectionWithCalculationNoOutput");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, double.NaN, 1.0, 1.0)
                }
            }, "-").SetName("SectionWithInvalidCalculationOutput");
            yield return new TestCaseData(new ClosingStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<ClosingStructuresInput>
                {
                    Output = new ProbabilityAssessmentOutput(1.0, 1.0, probability, 1.0, 1.0)
                }
            }, ProbabilityFormattingHelper.Format(probability)).SetName("SectionWithValidCalculationOutput");
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

        private ClosingStructuresFailureMechanismResultView CreateConfiguredFailureMechanismResultsView()
        {
            var failureMechanism = new ClosingStructuresFailureMechanism();

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

            ClosingStructuresFailureMechanismResultView failureMechanismResultView = ShowFailureMechanismResultsView();
            failureMechanismResultView.Data = failureMechanism.SectionResults;
            failureMechanismResultView.FailureMechanism = failureMechanism;

            return failureMechanismResultView;
        }

        private ClosingStructuresFailureMechanismResultView ShowFailureMechanismResultsView()
        {
            var failureMechanismResultView = new ClosingStructuresFailureMechanismResultView();
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}