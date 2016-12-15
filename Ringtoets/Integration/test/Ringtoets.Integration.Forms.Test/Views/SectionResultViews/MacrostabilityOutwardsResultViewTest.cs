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
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class MacrostabilityOutwardsResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerThreeIndex = 3;

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (var form = new Form())
            using (var view = new MacrostabilityOutwardsResultView())
            {
                form.Controls.Add(view);
                form.Show();

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
        public void GivenFormWithFailureMechanismResultView_WhenDataSourceWithFailureMechanismSectionResultAssigned_ThenSectionsAddedAsRows()
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

            Random random = new Random(21);
            var result1 = new MacrostabilityOutwardsFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                AssessmentLayerTwoA = (RoundedDouble) random.NextDouble(),
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result2 = new MacrostabilityOutwardsFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NotAssessed,
                AssessmentLayerTwoA = (RoundedDouble) random.NextDouble(),
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result3 = new MacrostabilityOutwardsFailureMechanismSectionResult(section3)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NoVerdict,
                AssessmentLayerTwoA = (RoundedDouble) random.NextDouble(),
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };

            using (var form = new Form())
            using (var view = new MacrostabilityOutwardsResultView())
            {
                form.Controls.Add(view);
                form.Show();

                // When
                view.Data = new[]
                {
                    result1,
                    result2,
                    result3
                };

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var rows = dataGridView.Rows;
                Assert.AreEqual(3, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                var expectedAssessmentLayer2AString1 = ProbabilityFormattingHelper.Format(result1.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString1, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewCellTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewCellTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                var expectedAssessmentLayer2AString2 = ProbabilityFormattingHelper.Format(result2.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString2, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewCellTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewCellTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                var expectedAssessmentLayer2AString3 = ProbabilityFormattingHelper.Format(result3.AssessmentLayerTwoA);
                Assert.AreEqual(expectedAssessmentLayer2AString3, cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewCellTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewCellTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
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
            Random random = new Random(21);
            var result = new MacrostabilityOutwardsFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = assessmentLayerOneState,
                AssessmentLayerTwoA = (RoundedDouble) random.NextDouble(),
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            using (var form = new Form())
            using (var view = new MacrostabilityOutwardsResultView())
            {
                form.Controls.Add(view);
                form.Show();

                view.Data = new[]
                {
                    result
                };

                // When
                result.AssessmentLayerOne = AssessmentLayerOneState.Sufficient;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);

                DataGridViewCellTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewCellTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_WhenDataSourceWithOtherFailureMechanismSectionResultAssigned_ThenSectionsNotAdded()
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
            var result1 = new TestFailureMechanismSectionResult(section1);
            var result2 = new TestFailureMechanismSectionResult(section2);

            using (var form = new Form())
            using (var view = new MacrostabilityOutwardsResultView())
            {
                form.Controls.Add(view);
                form.Show();

                // When
                view.Data = new[]
                {
                    result1,
                    result2
                };

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var rows = dataGridView.Rows;
                Assert.AreEqual(0, rows.Count);
            }
        }
    }
}