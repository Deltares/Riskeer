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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class WaterPressureAsphaltCoverResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerThreeIndex = 2;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanismSectionResults = new ObservableList<WaterPressureAsphaltCoverFailureMechanismSectionResult>();

            // Call
            using (var view = new WaterPressureAsphaltCoverResultView(failureMechanismSectionResults))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<WaterPressureAsphaltCoverFailureMechanismSectionResult>>(view);
                Assert.AreSame(failureMechanismSectionResults, view.Data);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (var form = new Form())
            {
                using (var view = new WaterPressureAsphaltCoverResultView(
                    new ObservableList<WaterPressureAsphaltCoverFailureMechanismSectionResult>()))
                {
                    form.Controls.Add(view);
                    form.Show();

                    // Then
                    var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                    Assert.AreEqual(3, dataGridView.ColumnCount);

                    Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                    Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                    Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                    Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                    Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                    Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
                }
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

            var random = new Random(21);
            var result1 = new WaterPressureAsphaltCoverFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = AssessmentLayerOneState.Sufficient,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result2 = new WaterPressureAsphaltCoverFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NotAssessed,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result3 = new WaterPressureAsphaltCoverFailureMechanismSectionResult(section3)
            {
                AssessmentLayerOne = AssessmentLayerOneState.NoVerdict,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var sectionResults = new ObservableList<WaterPressureAsphaltCoverFailureMechanismSectionResult>
            {
                result1,
                result2,
                result3
            };

            using (var form = new Form())
            {
                using (var view = new WaterPressureAsphaltCoverResultView(sectionResults))
                {
                    form.Controls.Add(view);
                    form.Show();

                    // When
                    view.Data = sectionResults;

                    // Then
                    var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                    DataGridViewRowCollection rows = dataGridView.Rows;
                    Assert.AreEqual(3, rows.Count);

                    DataGridViewCellCollection cells = rows[0].Cells;
                    Assert.AreEqual(3, cells.Count);
                    Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                    Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                    Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                    DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                    cells = rows[1].Cells;
                    Assert.AreEqual(3, cells.Count);
                    Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                    Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                    Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                    DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);

                    cells = rows[2].Cells;
                    Assert.AreEqual(3, cells.Count);
                    Assert.AreEqual("Section 3", cells[nameColumnIndex].FormattedValue);
                    Assert.AreEqual(result3.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                    Assert.AreEqual(result3.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                    DataGridViewTestHelper.AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
                }
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
            var result = new WaterPressureAsphaltCoverFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = assessmentLayerOneState,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var sectionResults = new ObservableList<WaterPressureAsphaltCoverFailureMechanismSectionResult>
            {
                result
            };

            using (var form = new Form())
            {
                using (var view = new WaterPressureAsphaltCoverResultView(sectionResults))
                {
                    form.Controls.Add(view);
                    form.Show();

                    view.Data = sectionResults;

                    // When
                    result.AssessmentLayerOne = AssessmentLayerOneState.Sufficient;
                    result.NotifyObservers();

                    // Then
                    var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                    DataGridViewRowCollection rows = dataGridView.Rows;
                    Assert.AreEqual(1, rows.Count);

                    DataGridViewCellCollection cells = rows[0].Cells;
                    Assert.AreEqual(3, cells.Count);

                    DataGridViewTestHelper.AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
                }
            }
        }
    }
}