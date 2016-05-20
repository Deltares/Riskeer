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
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class CustomFailureMechanismResultViewTest
    {
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerTwoBIndex = 3;
        private const int assessmentLayerThreeIndex = 4;

        [Test]
        public void GivenFormWithCustomFailureMechanismResultView_WhenAlways_ThenExpectedColumnsAreVisible()
        {
            // Setup
            using (var form = new Form())
            using (var view = new CustomFailureMechanismResultView())
            {
                form.Controls.Add(view);
                // Call

                form.Show();

                // Assert
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(5, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerTwoBIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one, dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a, dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_b, dataGridView.Columns[assessmentLayerTwoBIndex].HeaderText);
                Assert.AreEqual(Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three, dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void GivenFormWithCustomFailureMechanismResultView_WhenDataSourceWithCustomFailureMechanismSectionResultAssigned_ThenSectionsAddedAsRows()
        {
            // Setup
            var section1 = new FailureMechanismSection("Section 1", new [] { new Point2D(0,0) });
            var section2 = new FailureMechanismSection("Section 2", new [] { new Point2D(0,0) });
            Random random = new Random(21);
            var result1 = new CustomFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = true,
                AssessmentLayerTwoA = (RoundedDouble)random.NextDouble(),
                AssessmentLayerTwoB = (RoundedDouble)random.NextDouble(),
                AssessmentLayerThree = (RoundedDouble)random.NextDouble()
            };
            var result2 = new CustomFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = false,
                AssessmentLayerTwoA = (RoundedDouble)random.NextDouble(),
                AssessmentLayerTwoB = (RoundedDouble)random.NextDouble(),
                AssessmentLayerThree = (RoundedDouble)random.NextDouble()
            };

            // Call
            using (var form = new Form())
            using (var view = new CustomFailureMechanismResultView())
            {
                form.Controls.Add(view);
                form.Show();

                view.Data = new[]
                {
                    result1,
                    result2
                };
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                // Assert
                var rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                var cells = rows[0].Cells;
                Assert.AreEqual(5, cells.Count);
                int nameColumnIndex = 0;
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result1.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(string.Format("{0}", result1.AssessmentLayerTwoA), cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", result1.AssessmentLayerTwoB), cells[assessmentLayerTwoBIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", result1.AssessmentLayerThree), cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(5, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(result2.AssessmentLayerOne, cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(string.Format("{0}", result2.AssessmentLayerTwoA), cells[assessmentLayerTwoAIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", result2.AssessmentLayerTwoB), cells[assessmentLayerTwoBIndex].FormattedValue);
                Assert.AreEqual(string.Format("{0}", result2.AssessmentLayerThree), cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        public void GivenFormWithCustomFailureMechanismResultView_WhenDataSourceWithOtherFailureMechanismSectionResultAssigned_ThenSectionsNotAdded()
        {
            // Setup
            var section1 = new FailureMechanismSection("Section 1", new [] { new Point2D(0,0) });
            var section2 = new FailureMechanismSection("Section 2", new [] { new Point2D(0,0) });
            var result1 = new FailureMechanismSectionResult(section1);
            var result2 = new FailureMechanismSectionResult(section2);

            // Call
            using (var form = new Form())
            using (var view = new CustomFailureMechanismResultView())
            {
                form.Controls.Add(view);
                form.Show();

                view.Data = new[]
                {
                    result1,
                    result2
                };
                var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

                // Assert
                var rows = dataGridView.Rows;
                Assert.AreEqual(0, rows.Count);
            }
        }
    }
}