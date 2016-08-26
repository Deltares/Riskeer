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
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class StrengthStabilityLengthwiseConstructionResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerThreeIndex = 2;

        [Test]
        public void GivenFormWithStrengthStabilityLengthwiseConstructionResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (var form = new Form())
            using (var view = new StrengthStabilityLengthwiseConstructionResultView())
            {
                form.Controls.Add(view);
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(3, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one, dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three, dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void GivenFormWithStrengthStabilityLengthwiseConstructionResultView_WhenDataSourceWithStrengthStabilityLengthwiseConstructionFailureMechanismSectionResultAssigned_ThenSectionsAddedAsRows()
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
            Random random = new Random(21);
            var result1 = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = true,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result2 = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = false,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };

            using (var form = new Form())
            using (var view = new StrengthStabilityLengthwiseConstructionResultView())
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
                Assert.AreEqual(2, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(3, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);

                cells = rows[1].Cells;
                Assert.AreEqual(3, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                AssertCellIsEnabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        public void GivenFormWithStrengthStabilityLengthwiseConstructionFailureMechanismResultView_WhenSectionPassesLevel0AndListenersNotified_ThenRowsForSectionBecomesDisabled()
        {
            // Given
            var section = new FailureMechanismSection("Section 1", new[]
            {
                new Point2D(0, 0)
            });
            Random random = new Random(21);
            var result = new StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult(section)
            {
                AssessmentLayerOne = false,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            using (var form = new Form())
            using (var view = new StrengthStabilityLengthwiseConstructionResultView())
            {
                form.Controls.Add(view);
                form.Show();

                view.Data = new[]
                {
                    result
                };

                // When
                result.AssessmentLayerOne = true;
                result.NotifyObservers();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(3, cells.Count);

                AssertCellIsDisabled(cells[assessmentLayerThreeIndex]);
            }
        }

        [Test]
        public void GivenFormWithStrengthStabilityLengthwiseConstructionResultView_WhenDataSourceWithOtherFailureMechanismSectionResultAssigned_ThenSectionsNotAdded()
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
            using (var view = new StrengthStabilityLengthwiseConstructionResultView())
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

        private void AssertCellIsDisabled(DataGridViewCell dataGridViewCell)
        {
            Assert.AreEqual(true, dataGridViewCell.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
        }

        private void AssertCellIsEnabled(DataGridViewCell dataGridViewCell)
        {
            Assert.AreEqual(false, dataGridViewCell.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
        }
    }
}