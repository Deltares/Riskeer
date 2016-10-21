﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.Views.SectionResultViews
{
    [TestFixture]
    public class DuneErosionResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int assessmentLayerOneIndex = 1;
        private const int assessmentLayerTwoAIndex = 2;
        private const int assessmentLayerThreeIndex = 3;

        [Test]
        public void GivenFormWithDuneErosionFailureMechanismResultView_WhenShown_ThenExpectedColumnsAreVisible()
        {
            // Given
            using (var form = new Form())
            using (var view = new DuneErosionResultView())
            {
                form.Controls.Add(view);

                // When
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(4, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewCheckBoxColumn>(dataGridView.Columns[assessmentLayerOneIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[assessmentLayerTwoAIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one, dataGridView.Columns[assessmentLayerOneIndex].HeaderText);
                Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a, dataGridView.Columns[assessmentLayerTwoAIndex].HeaderText);
                Assert.AreEqual(RingtoetsCommonFormsResources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three, dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void GivenFormWithDuneErosionFailureMechanismResultView_WhenDataSourceWithDuneErosionFailureMechanismSectionResultAssigned_ThenSectionsAddedAsRows()
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
            var result1 = new DuneErosionFailureMechanismSectionResult(section1)
            {
                AssessmentLayerOne = true,
                AssessmentLayerTwoA = AssessmentLayerTwoAResult.Failed,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };
            var result2 = new DuneErosionFailureMechanismSectionResult(section2)
            {
                AssessmentLayerOne = true,
                AssessmentLayerTwoA = AssessmentLayerTwoAResult.Successful,
                AssessmentLayerThree = (RoundedDouble) random.NextDouble()
            };

            using (var form = new Form())
            using (var view = new DuneErosionResultView())
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
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.IsTrue((bool)cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result1.AssessmentLayerTwoA, cells[assessmentLayerTwoAIndex].Value);
                Assert.AreEqual(result1.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);

                cells = rows[1].Cells;
                Assert.AreEqual(4, cells.Count);
                Assert.AreEqual("Section 2", cells[nameColumnIndex].FormattedValue);
                Assert.IsTrue((bool) cells[assessmentLayerOneIndex].Value);
                Assert.AreEqual(result2.AssessmentLayerTwoA, cells[assessmentLayerTwoAIndex].Value);
                Assert.AreEqual(result2.AssessmentLayerThree.ToString(), cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }

        [Test]
        public void GivenFormWithDuneErosionFailureMechanismResultView_WhenDataSourceWithOtherFailureMechanismSectionResultAssigned_ThenSectionsNotAdded()
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
            using (var view = new DuneErosionResultView())
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