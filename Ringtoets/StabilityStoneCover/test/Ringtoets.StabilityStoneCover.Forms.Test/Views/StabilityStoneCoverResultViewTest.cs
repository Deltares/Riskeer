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

using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.Views;

namespace Ringtoets.StabilityStoneCover.Forms.Test.Views
{
    [TestFixture]
    public class StabilityStoneCoverResultViewTest
    {
        private const int nameColumnIndex = 0;
        private const int simpleAssessmentResultIndex = 1;
        private const int detailedAssessmentResultForFactorizedSignalingNormIndex = 2;
        private const int detailedAssessmentResultForSignalingNormIndex = 3;
        private const int detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex = 4;
        private const int detailedAssessmentResultForLowerLimitNormIndex = 5;
        private const int detailedAssessmentResultForFactorizedLowerLimitNormIndex = 6;
        private const int assessmentLayerThreeIndex = 7;
        private const int columnCount = 8;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            using (var view = new StabilityStoneCoverResultView(failureMechanism.SectionResults, failureMechanism))
            {
                // Assert
                Assert.IsInstanceOf<FailureMechanismResultView<StabilityStoneCoverFailureMechanismSectionResult,
                    StabilityStoneCoverSectionResultRow, StabilityStoneCoverFailureMechanism>>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
            }
        }

        [Test]
        public void GivenFormWithFailureMechanismResultView_ThenExpectedColumnsAreVisible()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            using (var form = new Form())
            using (var view = new StabilityStoneCoverResultView(failureMechanism.SectionResults, failureMechanism))
            {
                form.Controls.Add(view);

                // When
                form.Show();

                // Then
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                Assert.AreEqual(columnCount, dataGridView.ColumnCount);

                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[nameColumnIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[simpleAssessmentResultIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForFactorizedSignalingNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForSignalingNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForLowerLimitNormIndex]);
                Assert.IsInstanceOf<DataGridViewComboBoxColumn>(dataGridView.Columns[detailedAssessmentResultForFactorizedLowerLimitNormIndex]);
                Assert.IsInstanceOf<DataGridViewTextBoxColumn>(dataGridView.Columns[assessmentLayerThreeIndex]);

                Assert.AreEqual("Eenvoudige toets", dataGridView.Columns[simpleAssessmentResultIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\nCat. Iv - IIv", dataGridView.Columns[detailedAssessmentResultForFactorizedSignalingNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\nCat. IIv - IIIv", dataGridView.Columns[detailedAssessmentResultForSignalingNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\nCat. IIIv - IVv", dataGridView.Columns[detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\nCat. IVv - Vv", dataGridView.Columns[detailedAssessmentResultForLowerLimitNormIndex].HeaderText);
                Assert.AreEqual("Gedetailleerde toets\r\nper vak\r\nCat. Vv - VIv", dataGridView.Columns[detailedAssessmentResultForFactorizedLowerLimitNormIndex].HeaderText);
                Assert.AreEqual("Toets op maat", dataGridView.Columns[assessmentLayerThreeIndex].HeaderText);

                Assert.AreEqual(DataGridViewAutoSizeColumnsMode.AllCells, dataGridView.AutoSizeColumnsMode);
                Assert.AreEqual(DataGridViewContentAlignment.MiddleCenter, dataGridView.ColumnHeadersDefaultCellStyle.Alignment);
            }
        }

        [Test]
        public void FailureMechanismResultsView_AllDataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.AddSection(FailureMechanismSectionTestFactory.CreateFailureMechanismSection("Section 1"));

            // Call
            using (var form = new Form())
            using (var view = new StabilityStoneCoverResultView(failureMechanism.SectionResults, failureMechanism))
            {
                form.Controls.Add(view);
                form.Show();

                // Assert
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(1, rows.Count);

                DataGridViewCellCollection cells = rows[0].Cells;
                Assert.AreEqual(columnCount, cells.Count);
                Assert.AreEqual("Section 1", cells[nameColumnIndex].FormattedValue);
                Assert.AreEqual(SimpleAssessmentValidityOnlyResultType.None, cells[simpleAssessmentResultIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForFactorizedSignalingNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForSignalingNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForMechanismSpecificLowerLimitNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForLowerLimitNormIndex].Value);
                Assert.AreEqual(DetailedAssessmentResultType.None, cells[detailedAssessmentResultForFactorizedLowerLimitNormIndex].Value);
                Assert.AreEqual("-", cells[assessmentLayerThreeIndex].FormattedValue);
            }
        }
    }
}