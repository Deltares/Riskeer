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
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    public abstract partial class FailureMechanismResultView<T> : UserControl, IView where T : FailureMechanismSectionResult
    {
        private IEnumerable<T> failureMechanismSectionResult;
        private IFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView"/>.
        /// </summary>
        protected FailureMechanismResultView()
        {
            InitializeComponent();
            InitializeDataGridView();

            FailureMechanismObserver = new Observer(UpdataDataGridViewDataSource);
            FailureMechanismSectionResultObservers = new List<Observer>();
        }

        /// <summary>
        /// Gets or sets the failure mechanism.
        /// </summary>
        public IFailureMechanism FailureMechanism
        {
            set
            {
                failureMechanism = value;
                FailureMechanismObserver.Observable = failureMechanism;
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
                FailureMechanismSectionResult = value as IEnumerable<T>;

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
            FailureMechanismSectionResult = null;
            if (FailureMechanismObserver != null)
            {
                FailureMechanismObserver.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual IEnumerable<DataGridViewColumn> GetDataGridColumns()
        {
            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                Name = "column_Name"
            };
        }

        protected void UpdataDataGridViewDataSource()
        {
            EndEdit();

            dataGridView.DataSource = failureMechanismSectionResult.Select(sr => CreateFailureMechanismSectionResultRow(sr)).ToList();
        }

        protected abstract object CreateFailureMechanismSectionResultRow(T sectionResult);

        protected void RefreshDataGridView()
        {
            dataGridView.Refresh();
            dataGridView.AutoResizeColumns();
        }

        private IList<Observer> FailureMechanismSectionResultObservers { get; set; }
        private Observer FailureMechanismObserver { get; set; }

        private IEnumerable<T> FailureMechanismSectionResult
        {
            set
            {
                ClearSectionResultObservers();
                failureMechanismSectionResult = value;

                if (failureMechanismSectionResult != null)
                {
                    AddSectionResultObservers();
                }
            }
        }

        private void AddSectionResultObservers()
        {
            foreach (var sectionResult in failureMechanismSectionResult)
            {
                FailureMechanismSectionResultObservers.Add(new Observer(RefreshDataGridView)
                {
                    Observable = sectionResult
                });
            }
        }

        private void ClearSectionResultObservers()
        {
            foreach (var observer in FailureMechanismSectionResultObservers)
            {
                observer.Dispose();
            }
            FailureMechanismSectionResultObservers.Clear();
        }

        private void InitializeDataGridView()
        {
            dataGridView.GotFocus += DataGridViewGotFocus;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.Columns.AddRange(GetDataGridColumns().ToArray());
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