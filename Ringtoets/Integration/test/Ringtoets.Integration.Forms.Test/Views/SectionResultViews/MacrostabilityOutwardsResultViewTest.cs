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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class MacroStabilityOutwardsResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerThreeIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            // Call
            using (var view = new MacroStabilityOutwardsResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<MacroStabilityOutwardsFailureMechanismSectionResult,
                    MacroStabilityOutwardsFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacroStabilityOutwardsResultView(new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>(),
                                                                   new MacroStabilityOutwardsFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultView_FailureMechanismSectionResultAssigned_SectionsAddedAsRows()
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
            var section4 = new FailureMechanismSection("Section 4", new[]
            {
                new Point2D(0, 0)
            });

            var random = new Random(21);
            var result1 = new MacroStabilityOutwardsFailureMechanismSectionResult(section1)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.None,
                AssessmentLayerTwoA = random.NextRoundedDouble(),
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result2 = new MacroStabilityOutwardsFailureMechanismSectionResult(section2)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                AssessmentLayerTwoA = random.NextRoundedDouble(),
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result3 = new MacroStabilityOutwardsFailureMechanismSectionResult(section3)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible,
                AssessmentLayerTwoA = random.NextRoundedDouble(),
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result4 = new MacroStabilityOutwardsFailureMechanismSectionResult(section4)
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                AssessmentLayerTwoA = random.NextRoundedDouble(),
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result1,
                result2,
                result3,
                result4
            };

            // Call
            using (var form = new Form())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, new MacroStabilityOutwardsFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(4, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedAssessmentLayer2AString1 = ProbabilityFormattingHelper.Format(result1.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString1, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedAssessmentLayer2AString2 = ProbabilityFormattingHelper.Format(result2.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString2, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedAssessmentLayer2AString3 = ProbabilityFormattingHelper.Format(result3.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString3, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[3].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 4", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result4.SimpleAssessmentResult, cells[simpleAssessmentIndex].Value);
                string expectedAssessmentLayer2AString4 = ProbabilityFormattingHelper.Format(result4.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString4, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result4.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, TestName = "FormWithFailureMechanismResultView_SectionPassesLevel0AndListenersNotified_RowsForSectionDisabled(notAssessed)")]
        [TestCase(SimpleAssessmentResultType.AssessFurther, TestName = "FormWithFailureMechanismResultView_SectionPassesLevel0AndListenersNotified_RowsForSectionDisabled(noVerdict)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionPassesSimpleAssessmentAndListenersNotified_ThenRowsForSectionDisabled(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Given
            var random = new Random(21);
            var result = new MacroStabilityOutwardsFailureMechanismSectionResult(new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            }))
            {
                SimpleAssessmentResult = simpleAssessmentResult,
                AssessmentLayerTwoA = random.NextRoundedDouble(),
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var sectionResults = new ObservableList<MacroStabilityOutwardsFailureMechanismSectionResult>
            {
                result
            };

            using (var form = new Form())
            using (var view = new MacroStabilityOutwardsResultView(sectionResults, new MacroStabilityOutwardsFailureMechanism()))
            {
                form.Controls.Add(view);
                form.Show();

                // When
                result.SimpleAssessmentResult = SimpleAssessmentResultType.ProbabilityNegligible;
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
    }
}