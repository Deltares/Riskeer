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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerThreeIndex = 3;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView(failureMechanism, failureMechanism.SectionResults))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<GrassCoverErosionOutwardsFailureMechanism,
                    GrassCoverErosionOutwardsFailureMechanismSectionResult>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithGrassCoverErosionOutwardsFailureMechanismResultView_WhenShown_ThenExpectedColumnsAreVisible()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            using (var form = new Form())
            {
                using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView(failureMechanism, failureMechanism.SectionResults))
                {
                    form.Controls.Add(view);

                    // Call
                    form.Show();

                    // Then
                    var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                    Assert.AreEqual(4, dataGridView.ColumnCount);

                    Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                    Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                    Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                    Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                    Assert.AreEqual("Gedetailleerde toets per vak", dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                    Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                    Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                    Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                }
            }
        }

        [Test]
        public void GrassCoverErosionOutwardsFailureMechanismResultView_WithFailureMechanismSectionResultAssigned_SectionsAddedAsRows()
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
            var result1 = new GrassCoverErosionOutwardsFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                AssessmentLayerTwoA = AssessmentLayerTwoAResult.Failed,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result2 = new GrassCoverErosionOutwardsFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NotAssessed,
                AssessmentLayerTwoA = AssessmentLayerTwoAResult.Successful,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var result3 = new GrassCoverErosionOutwardsFailureMechanismSectionResult(section3)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NoVerdict,
                AssessmentLayerTwoA = AssessmentLayerTwoAResult.Successful,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var sectionResults = new ObservableList<GrassCoverErosionOutwardsFailureMechanismSectionResult>
            {
                result1,
                result2,
                result3
            };

            // Call
            using (var form = new Form())
            using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                      sectionResults))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(3, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result1.AssessmentLayerTwoA, cells[assessmentLayerTwoAIndex].Value);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result2.AssessmentLayerTwoA, cells[assessmentLayerTwoAIndex].Value);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                cells = rows[2].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result3.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result3.AssessmentLayerTwoA, cells[assessmentLayerTwoAIndex].Value);
                Assert.AreEqual(result3.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerTwoAIndex]);
                DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        [TestCase(AssessmentLayerOneState.NotAssessed, TestName = "ResultView_SectionPassesLevel0_RowsDisabled(notAssessed)")]
        [TestCase(AssessmentLayerOneState.NoVerdict, TestName = "ResultView_SectionPassesLevel0_RowsDisabled(noVerdict)")]
        public void GivenFormWithFailureMechanismResultView_WhenSectionPassesLevel0AndListenersNotified_ThenRowsForSectionBecomesDisabled(
            AssessmentLayerOneState assessmentLayerOneState)
        {
            // Given
            var random = new Random(21);
            var result = new GrassCoverErosionOutwardsFailureMechanismSectionResult(new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            }))
            {
                AssessmentLayerOne = assessmentLayerOneState,
                AssessmentLayerTwoA = AssessmentLayerTwoAResult.Failed,
                AssessmentLayerThree = random.NextRoundedDouble()
            };
            var sectionResults = new ObservableList<GrassCoverErosionOutwardsFailureMechanismSectionResult>
            {
                result
            };

            using (var form = new Form())
            {
                using (var view = new GrassCoverErosionOutwardsFailureMechanismResultView(new GrassCoverErosionOutwardsFailureMechanism(),
                                                                                          sectionResults))
                {
                    form.Controls.Add(view);
                    form.Show();

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
        }
    }
}