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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.Views;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using CoreCommonResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    public partial class FailureMechanismResultView : UserControl, IView
    {
        private readonly Observer failureMechanismObserver;
        private readonly RecursiveObserver<IFailureMechanism, FailureMechanismSectionResult> failureMechanismSectionResultObserver;

        private IEnumerable<FailureMechanismSectionResult> failureMechanismSectionResult;
        private IFailureMechanism failureMechanism;
        private DataGridViewTextBoxColumn assessmentLayerTwoA;
        private DataGridViewTextBoxColumn assessmentLayerTwoB;
        private DataGridViewTextBoxColumn assessmentLayerThree;
        private const double tolerance = 1e-6;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView"/>.
        /// </summary>
        public FailureMechanismResultView()
        {
            InitializeComponent();
            InitializeDataGridView();

            failureMechanismObserver = new Observer(UpdataDataGridViewDataSource);
            failureMechanismSectionResultObserver = new RecursiveObserver<IFailureMechanism, FailureMechanismSectionResult>(RefreshDataGridView, mechanism => mechanism.SectionResults);

            Load += OnLoad;
        }

        /// <summary>
        /// Gets or sets the failure mechanism.
        /// </summary>
        public IFailureMechanism FailureMechanism
        {
            get
            {
                return failureMechanism;
            }
            set
            {
                failureMechanism = value;

                failureMechanismObserver.Observable = failureMechanism;
                failureMechanismSectionResultObserver.Observable = failureMechanism;
            }
        }

        public object Data
        {
            get
            {
                return failureMechanismSectionResult;
            }
            set
            {
                failureMechanismSectionResult = value as IEnumerable<FailureMechanismSectionResult>;

                if (failureMechanismSectionResult != null)
                {
                    UpdataDataGridViewDataSource();
                }
                else
                {
                    dataGridView.DataSource = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            FailureMechanism = null;

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (failureMechanismSectionResult != null && failureMechanismSectionResult.Any())
            {
                SetRowStyling();
            }
        }

        private void InitializeDataGridView()
        {
            dataGridView.CurrentCellDirtyStateChanged += DataGridViewCurrentCellDirtyStateChanged;
            dataGridView.CellValidating += DataGridViewCellValidating;
            dataGridView.DataError += DataGridViewDataError;
            dataGridView.CellFormatting += DataGridViewCellFormatting;

            var sectionName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                Name = "column_Name"
            };

            var assessmentLayerOne = new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "AssessmentLayerOne",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                Name = "column_AssessmentLayerOne"
            };

            assessmentLayerTwoA = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoA",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_a,
                Name = "column_AssessmentLayerTwoA",
                ReadOnly = true
            };

            assessmentLayerTwoB = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerTwoB",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_two_b,
                Name = "column_AssessmentLayerTwoB"
            };

            assessmentLayerThree = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AssessmentLayerThree",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_three,
                Name = "column_AssessmentLayerThree"
            };

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(sectionName, assessmentLayerOne, assessmentLayerTwoA, assessmentLayerTwoB, assessmentLayerThree);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void UpdataDataGridViewDataSource()
        {
            dataGridView.DataSource = failureMechanismSectionResult.Select(sr => new FailureMechanismSectionResultRow(sr)).ToList();
            SetRowStyling();
        }

        private void RefreshDataGridView()
        {
            dataGridView.Refresh();
            dataGridView.AutoResizeColumns();

            SetRowStyling();
        }

        private void SetRowStyling()
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var checkboxSelected = (bool) row.Cells[1].Value;

                SetRowEditMode(row, checkboxSelected);

                SetRowStyle(checkboxSelected, row);
            }
        }

        private void SetRowEditMode(DataGridViewRow row, bool checkboxSelected)
        {
            row.Cells[assessmentLayerTwoB.Index].ReadOnly = checkboxSelected;
            row.Cells[assessmentLayerThree.Index].ReadOnly = checkboxSelected;
        }

        private void SetRowStyle(bool checkboxSelected, DataGridViewRow row)
        {
            if (checkboxSelected)
            {
                SetCellStyle(row.Cells[assessmentLayerTwoA.Index], Color.FromKnownColor(KnownColor.DarkGray), Color.FromKnownColor(KnownColor.GrayText));
                SetCellStyle(row.Cells[assessmentLayerTwoB.Index], Color.FromKnownColor(KnownColor.DarkGray), Color.FromKnownColor(KnownColor.GrayText));
                SetCellStyle(row.Cells[assessmentLayerThree.Index], Color.FromKnownColor(KnownColor.DarkGray), Color.FromKnownColor(KnownColor.GrayText));
            }
            else
            {
                SetCellStyle(row.Cells[assessmentLayerTwoA.Index], Color.FromKnownColor(KnownColor.White), Color.FromKnownColor(KnownColor.ControlText));
                SetCellStyle(row.Cells[assessmentLayerTwoB.Index], Color.FromKnownColor(KnownColor.White), Color.FromKnownColor(KnownColor.ControlText));
                SetCellStyle(row.Cells[assessmentLayerThree.Index], Color.FromKnownColor(KnownColor.White), Color.FromKnownColor(KnownColor.ControlText));
            }
        }

        private void SetCellStyle(DataGridViewCell cell, Color backgroundColor, Color textColor)
        {
            cell.Style.BackColor = backgroundColor;
            cell.Style.ForeColor = textColor;
        }

        #region Nested types

        private class FailureMechanismSectionResultRow
        {
            public readonly FailureMechanismSectionResult failureMechanismSectionResult;

            public FailureMechanismSectionResultRow(FailureMechanismSectionResult failureMechanismSectionResult)
            {
                this.failureMechanismSectionResult = failureMechanismSectionResult;
            }

            public string Name
            {
                get
                {
                    return failureMechanismSectionResult.Section.Name;
                }
            }

            public bool AssessmentLayerOne
            {
                get
                {
                    return failureMechanismSectionResult.AssessmentLayerOne;
                }
                set
                {
                    failureMechanismSectionResult.AssessmentLayerOne = value;
                    failureMechanismSectionResult.NotifyObservers();
                }
            }

            public string AssessmentLayerTwoA
            {
                get
                {
                    if (failureMechanismSectionResult.CalculationScenarios.Any() && Math.Abs(failureMechanismSectionResult.TotalContribution - 1.0) > tolerance)
                    {
                        return double.NaN.ToString(CultureInfo.InvariantCulture);
                    }

                    var layerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

                    if (!failureMechanismSectionResult.CalculationScenarios.Any() || !layerTwoA.HasValue || double.IsNaN(layerTwoA.Value))
                    {
                        return Resources.FailureMechanismSectionResultRow_AssessmentLayerTwoA_No_result_dash;
                    }

                    return string.Format(CoreCommonResources.ProbabilityPerYearFormat, layerTwoA.Value);
                }
            }

            public RoundedDouble AssessmentLayerTwoB
            {
                get
                {
                    return failureMechanismSectionResult.AssessmentLayerTwoB;
                }
                set
                {
                    failureMechanismSectionResult.AssessmentLayerTwoB = value;
                }
            }

            public RoundedDouble AssessmentLayerThree
            {
                get
                {
                    return failureMechanismSectionResult.AssessmentLayerThree;
                }
                set
                {
                    failureMechanismSectionResult.AssessmentLayerThree = value;
                }
            }
        }

        #endregion

        #region Event handling

        private void DataGridViewCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Ensure checkbox values are directly committed
            DataGridViewColumn currentColumn = dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex];
            if (currentColumn is DataGridViewCheckBoxColumn)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DataGridViewCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].ErrorText = String.Empty;

            var cellEditValue = e.FormattedValue.ToString();
            if (string.IsNullOrWhiteSpace(cellEditValue))
            {
                dataGridView.Rows[e.RowIndex].ErrorText = Resources.DataGridViewCellValidating_Text_may_not_be_empty;
            }
        }

        private void DataGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;

            if (string.IsNullOrWhiteSpace(dataGridView.Rows[e.RowIndex].ErrorText) && e.Exception != null)
            {
                dataGridView.Rows[e.RowIndex].ErrorText = e.Exception.Message;
            }
        }

        private void DataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            FailureMechanismSectionResultRow resultRow = dataGridView.Rows[e.RowIndex].DataBoundItem as FailureMechanismSectionResultRow;

            if (resultRow != null && e.ColumnIndex == assessmentLayerTwoA.Index)
            {
                FailureMechanismSectionResult rowObject = resultRow.failureMechanismSectionResult;

                if (rowObject.AssessmentLayerOne)
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
                    return;
                }

                if (rowObject.CalculationScenarios.Any() && Math.Abs(rowObject.TotalContribution - 1.0) > tolerance)
                {
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Scenario_contribution_for_this_section_not_100;
                    return;
                }

                var layerTwoA = rowObject.AssessmentLayerTwoA;

                if (!layerTwoA.HasValue)
                {
                    // Calculation not done.
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Not_all_calculations_are_executed;
                    return;
                }
                
                if (double.IsNaN(layerTwoA.Value))
                {
                    // Calculation output not valid.
                    dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = Resources.FailureMechanismResultView_DataGridViewCellFormatting_Not_all_calculations_have_valid_output;
                    return;
                }

                dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = string.Empty;
            }
        }

        #endregion
    }
}