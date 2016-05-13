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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using CoreCommonResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="Ringtoets.Piping.Data.PipingFailureMechanismSectionResult"/>.
    /// </summary>
    public partial class FailureMechanismResultView : UserControl, IView
    {
        private const double tolerance = 1e-6;
        private readonly Observer failureMechanismObserver;
        private readonly RecursiveObserver<FailureMechanismBase<FailureMechanismSectionResult>, FailureMechanismSectionResult> failureMechanismSectionResultObserver;

        private IEnumerable<FailureMechanismSectionResult> failureMechanismSectionResult;
        private FailureMechanismBase<FailureMechanismSectionResult> failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="Ringtoets.Piping.Forms.Views.PipingFailureMechanismResultView"/>.
        /// </summary>
        public FailureMechanismResultView()
        {
            InitializeComponent();
            InitializeDataGridView();

            failureMechanismObserver = new Observer(UpdataDataGridViewDataSource);
            failureMechanismSectionResultObserver = new RecursiveObserver<FailureMechanismBase<FailureMechanismSectionResult>, FailureMechanismSectionResult>(RefreshDataGridView, mechanism => mechanism.SectionResults);
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
                failureMechanism = value as FailureMechanismBase<FailureMechanismSectionResult>;

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

            failureMechanismObserver.Dispose();
            failureMechanismSectionResultObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridView.CurrentCellDirtyStateChanged += DataGridViewCurrentCellDirtyStateChanged;
            dataGridView.CellValidating += DataGridViewCellValidating;
            dataGridView.DataError += DataGridViewDataError;
            dataGridView.GotFocus += DataGridViewGotFocus;

            var sectionName = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                Name = "column_Name"
            };

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(sectionName);

            foreach (var column in dataGridView.Columns.OfType<DataGridViewColumn>())
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void UpdataDataGridViewDataSource()
        {
            EndEdit();

            dataGridView.DataSource = failureMechanismSectionResult.Select(sr => new FailureMechanismSectionResultRow(sr)).ToList();
        }

        private void EndEdit()
        {
            if (dataGridView.IsCurrentCellInEditMode)
            {
                dataGridView.CancelEdit();
                dataGridView.EndEdit(); 
                dataGridView.CurrentCell = null;
            }
        }

        private void RefreshDataGridView()
        {
            dataGridView.Refresh();
            dataGridView.AutoResizeColumns();
        }

        #region Nested types

        private class FailureMechanismSectionResultRow
        {
            public FailureMechanismSectionResultRow(FailureMechanismSectionResult failureMechanismSectionResult)
            {
                FailureMechanismSectionResult = failureMechanismSectionResult;
            }

            public string Name
            {
                get
                {
                    return FailureMechanismSectionResult.Section.Name;
                }
            }

            public FailureMechanismSectionResult FailureMechanismSectionResult { get; private set; }
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

        private void DataGridViewGotFocus(object sender, EventArgs eventArgs)
        {
            if (dataGridView.CurrentCell != null)
            {
                dataGridView.BeginEdit(true); // Always start editing after setting the focus (otherwise data grid view cell dirty events are no longer fired when using the keyboard...)
            }
        }

        #endregion
    }
}