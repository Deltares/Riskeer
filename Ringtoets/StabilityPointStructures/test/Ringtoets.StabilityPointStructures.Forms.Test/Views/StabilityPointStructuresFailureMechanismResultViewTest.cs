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
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.Views;

namespace Ringtoets.StabilityPointStructures.Forms.Test.Views
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
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

            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            using (var view = new StabilityPointStructuresFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<StabilityPointStructuresFailureMechanismSectionResult,
                    StabilityPointStructuresFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            TestDelegate call = () => new StabilityPointStructuresFailureMechanismResultView(failureMechanism.SectionResults,
                                                                                             failureMechanism,
                                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GivenFailureMechanismResultsView_WhenAllDataSet_ThenDataGridViewCorrectlyInitialized()
        {
            // Given
            using (CreateConfiguredFailureMechanismResultsView(new StabilityPointStructuresFailureMechanism()))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(AssessmentLayerOneState.NotAssessed, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(AssessmentLayerOneState.NotAssessed, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        [TestCase(AssessmentLayerOneState.Sufficient)]
        public void FailureMechanismResultsView_ChangeCheckBox_DataGridViewCorrectlySyncedAndStylingSet(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Setup
            using (CreateConfiguredFailureMechanismResultsView(new StabilityPointStructuresFailureMechanism()))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerOneIndex].Value = assessmentLayerOneState;

                // Assert
                DataGridViewRowCollection rows = dataGridView.Rows;

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                DataGridViewCell cellDetailedAssessment = cells[detailedAssessmentIndex];
                DataGridViewCell cellAssessmentLayerThree = cells[assessmentLayerThreeIndex];
                DataGridViewCell dataGridViewCell = cells[assessmentLayerOneIndex];

                Assert.AreEqual(assessmentLayerOneState, dataGridViewCell.Value);
                Assert.AreEqual("-", cellDetailedAssessment.FormattedValue);
                Assert.AreEqual("-", cellAssessmentLayerThree.FormattedValue);
                Assert.IsEmpty(dataGridViewCell.ErrorText);

                if (assessmentLayerOneState == AssessmentLayerOneState.Sufficient)
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
            using (ShowFailureMechanismResultsView(new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>()))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[detailedAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[detailedAssessmentIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultView_WithFailureMechanismSectionResultAssigned_SectionsAddedAsRows()
        {
            // Setup
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
            var result1 = new StabilityPointStructuresFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result2 = new StabilityPointStructuresFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NotAssessed,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result3 = new StabilityPointStructuresFailureMechanismSectionResult(section3)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NoVerdict,
                AssessmentLayerThree = random.NextRoundedDouble()
            };

            // Call
            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
                {
                    result1,
                    result2,
                    result3
                }))
            {
                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(3, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);

                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result1.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[detailedAssessmentIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result2.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[detailedAssessmentIndex], true);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual(ProbabilityFormattingHelper.Format(result3.AssessmentLayerThree),
                                cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[detailedAssessmentIndex], true);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed, TestName = "ResultView_SectionPassesLevel0AndListenersNotified_RowsDisabled(notAssessed)")]
        [TestCase(AssessmentLayerOneState.NoVerdict, TestName = "ResultView_SectionPassesLevel0AndListenersNotified_RowsDisabled(noVerdict)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionPassesLevel0AndListenersNotified_ThenRowsForSectionDisabled(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var section = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            });
            var random = new Random(21);
            var result = new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = assessmentLayerOneState,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
                {
                    result
                }))
            {
                // When
                result.AssessmentLayerOne = AssessmentLayerOneState.Sufficient;
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
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultWithoutCalculation_ThenDetailedAssessmentErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(CreateSimpleFailureMechanismSection())
            {
                AssessmentLayerOne = assessmentLayerOneState
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
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
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultAndCalculationNotCalculated_ThenDetailedAssessmentErrorTooltip(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(CreateSimpleFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>(),
                AssessmentLayerOne = assessmentLayerOneState
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
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
        [TestCase(AssessmentLayerOneState.NotAssessed)]
        [TestCase(AssessmentLayerOneState.NoVerdict)]
        public void GivenSectionResultAndFailedCalculation_ThenDetailedAssessmentErrorTooltip(AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(CreateSimpleFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                },
                AssessmentLayerOne = assessmentLayerOneState
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
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
        public void GivenSectionResultAndSuccessfulCalculation_ThenDetailedAssessmentNoError()
        {
            // Given
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(CreateSimpleFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                }
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
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
        [TestCaseSource(nameof(AssessmentLayerOneStateIsSufficientVariousSections))]
        public void GivenSectionResultAndAssessmentLayerOneStateSufficient_ThenDetailedAssessmentNoError(
            StabilityPointStructuresFailureMechanismSectionResult sectionResult, string expectedValue)
        {
            // Given
            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
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
        [TestCase(AssessmentLayerOneState.NotAssessed, TestName = "SectionResultSuccessfulCalculation_CalculationToFailed_DetailedAssessmentHasError(notAssessed)")]
        [TestCase(AssessmentLayerOneState.NoVerdict, TestName = "SectionResultSuccessfulCalculation_CalculationToFailed_DetailedAssessmentHasError(noVerdict)")]
        public void GivenSectionResultAndSuccessfulCalculation_WhenChangingCalculationToFailed_ThenDetailedAssessmentHasError(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var sectionResult = new StabilityPointStructuresFailureMechanismSectionResult(CreateSimpleFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    Output = new TestStructuresOutput(0.56789)
                },
                AssessmentLayerOne = assessmentLayerOneState
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<StabilityPointStructuresFailureMechanismSectionResult>
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
                sectionResult.Calculation = new StructuresCalculation<StabilityPointStructuresInput>
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
            using (CreateConfiguredFailureMechanismResultsView(new StabilityPointStructuresFailureMechanism()))
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
            using (CreateConfiguredFailureMechanismResultsView(new StabilityPointStructuresFailureMechanism()))
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
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            using (CreateConfiguredFailureMechanismResultsView(failureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
                Assert.AreEqual(newValue, failureMechanism.SectionResults.First().AssessmentLayerThree);
            }
        }

        private static IEnumerable AssessmentLayerOneStateIsSufficientVariousSections()
        {
            FailureMechanismSection section = CreateSimpleFailureMechanismSection();

            yield return new TestCaseData(new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient
            }, "-").SetName("SectionWithoutCalculation");
            yield return new TestCaseData(new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>()
            }, "-").SetName("SectionWithCalculationNoOutput");
            yield return new TestCaseData(new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    Output = new TestStructuresOutput(double.NaN)
                }
            }, "-").SetName("SectionWithInvalidCalculationOutput");
            yield return new TestCaseData(new StabilityPointStructuresFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>
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

        private StabilityPointStructuresFailureMechanismResultView CreateConfiguredFailureMechanismResultsView(
            StabilityPointStructuresFailureMechanism failureMechanism)
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

            StabilityPointStructuresFailureMechanismResultView failureMechanismResultView = ShowFailureMechanismResultsView(failureMechanism.SectionResults);

            return failureMechanismResultView;
        }

        private StabilityPointStructuresFailureMechanismResultView ShowFailureMechanismResultsView(
            IObservableEnumerable<StabilityPointStructuresFailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new StabilityPointStructuresFailureMechanismResultView(sectionResults,
                                                                                                    new StabilityPointStructuresFailureMechanism(),
                                                                                                    new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}