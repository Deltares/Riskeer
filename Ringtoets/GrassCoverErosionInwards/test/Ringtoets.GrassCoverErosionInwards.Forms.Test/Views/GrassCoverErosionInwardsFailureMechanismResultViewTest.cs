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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismResultViewTest
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

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            using (var view = new GrassCoverErosionInwardsFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<GrassCoverErosionInwardsFailureMechanismSectionResult,
                    GrassCoverErosionInwardsFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsFailureMechanismResultView(failureMechanism.SectionResults, failureMechanism, null);
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GivenFormWithGrassCoverErosionInwardsFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (ShowFailureMechanismResultsView(new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>()))
            {
                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);
                Assert.IsTrue(dataGridView.Columns[detailedAssessmentIndex].ReadOnly);

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
                Assert.AreEqual(SimpleAssessmentResultValidityOnlyType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentResultValidityOnlyType.None, cells[simpleAssessmentIndex].Value);
                Assert.AreEqual("-", cells[detailedAssessmentIndex].FormattedValue);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.NotApplicable)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void FailureMechanismResultsView_ChangeComboBox_DataGridViewCorrectlySyncedAndStylingSet(
            SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Setup
            using (ShowFullyConfiguredFailureMechanismResultsView())
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

                if (simpleAssessmentResult == SimpleAssessmentResultValidityOnlyType.NotApplicable)
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            using (ShowFullyConfiguredFailureMechanismResultsView(failureMechanism))
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                // Call
                dataGridView.Rows[0].Cells[assessmentLayerThreeIndex].Value = newValue.ToString(CultureInfo.CurrentCulture);

                // Assert
                Assert.IsEmpty(dataGridView.Rows[0].ErrorText);
                Assert.AreEqual(newValue, failureMechanism.SectionResults.First().AssessmentLayerThree);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void GivenSectionResultWithoutCalculation_ThenDetailedAssessmentErrorTooltip(SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>
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
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void GivenSectionResultAndCalculationNotCalculated_ThenDetailedAssessmentErrorTooltip(
            SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new GrassCoverErosionInwardsCalculation(),
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>
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
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void GivenSectionResultAndFailedCalculation_ThenDetailedAssessmentErrorTooltip(SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Given
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                            new TestDikeHeightOutput(double.NaN),
                                                            new TestOvertoppingRateOutput(double.NaN))
            };
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation,
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>
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
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void GivenSectionResultAndSuccessfulCalculation_ThenDetailedAssessmentNoError(SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.56789),
                                                                new TestDikeHeightOutput(0),
                                                                new TestOvertoppingRateOutput(0))
                }
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>
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
        [TestCaseSource(nameof(SimpleAssessmentResultIsSufficientVariousSectionResults))]
        public void GivenSectionResultAndAssessmentSimpleAssessmentNotApplicable_ThenDetailedAssessmentNoError(
            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult, string expectedValue)
        {
            // Given
            using (ShowFailureMechanismResultsView(
                new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>
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
        [TestCase(SimpleAssessmentResultValidityOnlyType.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable)]
        public void GivenSectionResultAndSuccessfulCalculation_WhenChangingCalculationToFailed_ThenDetailedAssessmentHasError(
            SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            // Given
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.56789),
                                                                new TestDikeHeightOutput(0),
                                                                new TestOvertoppingRateOutput(0))
                },
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (ShowFailureMechanismResultsView(
                new ObservableList<GrassCoverErosionInwardsFailureMechanismSectionResult>
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
                sectionResult.Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                                new TestDikeHeightOutput(double.NaN),
                                                                new TestOvertoppingRateOutput(double.NaN))
                };
                formattedValue = dataGridViewCell.FormattedValue; // Need to do this to fire the CellFormatting event.

                // Then
                Assert.AreEqual("-", formattedValue);
                Assert.AreEqual("De maatgevende berekening voor dit vak moet een geldige uitkomst hebben.", dataGridViewCell.ErrorText);
            }
        }

        private static IEnumerable SimpleAssessmentResultIsSufficientVariousSectionResults()
        {
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            const double reliability = 0.56789;

            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultValidityOnlyType.NotApplicable
            }, "-").SetName("SectionWithoutCalculation");
            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultValidityOnlyType.NotApplicable,
                Calculation = new GrassCoverErosionInwardsCalculation()
            }, "-").SetName("SectionWithCalculationNoOutput");
            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultValidityOnlyType.NotApplicable,
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                                new TestDikeHeightOutput(double.NaN),
                                                                new TestOvertoppingRateOutput(double.NaN))
                }
            }, "-").SetName("SectionWithInvalidCalculationOutput");
            yield return new TestCaseData(new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                SimpleAssessmentResult = SimpleAssessmentResultValidityOnlyType.NotApplicable,
                Calculation = new GrassCoverErosionInwardsCalculation
                {
                    Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(reliability),
                                                                new TestDikeHeightOutput(0),
                                                                new TestOvertoppingRateOutput(0))
                }
            }, ProbabilityFormattingHelper.Format(0.25)).SetName("SectionWithValidCalculationOutput");
        }

        private GrassCoverErosionInwardsFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView()
        {
            return ShowFullyConfiguredFailureMechanismResultsView(new GrassCoverErosionInwardsFailureMechanism());
        }

        private GrassCoverErosionInwardsFailureMechanismResultView ShowFullyConfiguredFailureMechanismResultsView(
            GrassCoverErosionInwardsFailureMechanism failureMechanism)
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

            GrassCoverErosionInwardsFailureMechanismResultView failureMechanismResultView = ShowFailureMechanismResultsView(failureMechanism.SectionResults);

            return failureMechanismResultView;
        }

        private GrassCoverErosionInwardsFailureMechanismResultView ShowFailureMechanismResultsView(
            IObservableEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults)
        {
            var failureMechanismResultView = new GrassCoverErosionInwardsFailureMechanismResultView(sectionResults,
                                                                                                    new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                    new ObservableTestAssessmentSectionStub());
            testForm.Controls.Add(failureMechanismResultView);
            testForm.Show();

            return failureMechanismResultView;
        }
    }
}