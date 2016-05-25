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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;
using CoreCommonResources = Core.Common.Base.Properties.Resources;
using CoreCommonControlsResources = Core.Common.Controls.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    public abstract partial class FailureMechanismResultView<T> : UserControl, IView where T : FailureMechanismSectionResult
    {
        private readonly IList<Observer> failureMechanismSectionResultObservers;
        private readonly Observer failureMechanismObserver;
        private const int assessmentLayerOneColumnIndex = 1;

        private IEnumerable<T> failureMechanismSectionResult;
        private IFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView{T}"/>.
        /// </summary>
        protected FailureMechanismResultView()
        {
            InitializeComponent();
            InitializeDataGridView();

            failureMechanismObserver = new Observer(UpdataDataGridViewDataSource);
            failureMechanismSectionResultObservers = new List<Observer>();
        }

        /// <summary>
        /// Sets the failure mechanism.
        /// </summary>
        public virtual IFailureMechanism FailureMechanism
        {
            set
            {
                failureMechanism = value;
                failureMechanismObserver.Observable = failureMechanism;
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
            if (failureMechanismObserver != null)
            {
                failureMechanismObserver.Dispose();
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds out whether the assessment section which is represented by the row at index 
        /// <paramref name="rowIndex"/> has passed the level 0 assessment.
        /// </summary>
        /// <param name="rowIndex">The index of the row which has a section attached.</param>
        /// <returns><c>false</c> if assessment level 0 has passed, <c>true</c> otherwise.</returns>
        protected bool HasPassedLevelZero(int rowIndex)
        {
            var row = dataGridView.Rows[rowIndex];
            return (bool) row.Cells[assessmentLayerOneColumnIndex].Value;
        }

        /// <summary>
        /// Add a handler for the <see cref="DataGridView.CellFormatting"/> event.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        protected void AddCellFormattingHandler(DataGridViewCellFormattingEventHandler handler)
        {
            dataGridView.CellFormatting += handler;
        }

        /// <summary>
        /// Restore the initial style of the cell at <paramref name="rowIndex"/>, <paramref name="columnIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        protected void RestoreCell(int rowIndex, int columnIndex)
        {
            var cell = dataGridView.Rows[rowIndex].Cells[columnIndex];
            cell.ReadOnly = GetDataGridColumns().ElementAt(columnIndex).ReadOnly;
            SetCellStyle(cell, CellStyle.Enabled);
        }

        /// <summary>
        /// Gives the cell at <paramref name="rowIndex"/>, <paramref name="columnIndex"/> a
        /// disabled style.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        protected void DisableCell(int rowIndex, int columnIndex)
        {
            var cell = GetCell(rowIndex, columnIndex);
            cell.ReadOnly = true;
            SetCellStyle(cell, CellStyle.Disabled);
        }

        protected DataGridViewCell GetCell(int rowIndex, int columnIndex)
        {
            return dataGridView.Rows[rowIndex].Cells[columnIndex];
        }

        /// <summary>
        /// Gets all the columns that should be added to the <see cref="DataGridView"/> on the
        /// <see cref="FailureMechanismResultView{T}"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DataGridViewColumn"/>.</returns>
        protected virtual IEnumerable<DataGridViewColumn> GetDataGridColumns()
        {
            yield return new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                Name = "column_Name",
                ReadOnly = true
            };

            yield return new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "AssessmentLayerOne",
                HeaderText = Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                Name = "column_AssessmentLayerOne"
            };
        }

        /// <summary>
        /// Updates the data source of the data grid view with the current known failure mechanism section results.
        /// </summary>
        protected void UpdataDataGridViewDataSource()
        {
            EndEdit();
            dataGridView.DataSource = failureMechanismSectionResult.Select(CreateFailureMechanismSectionResultRow).ToList();
        }

        /// <summary>
        /// Gets data that is visualized on the row a the given <paramref name="rowIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The position of the row in the data source.</param>
        /// <returns>The data bound to the row at index <paramref name="rowIndex"/>.</returns>
        protected object GetDataAtRow(int rowIndex)
        {
            return dataGridView.Rows[rowIndex].DataBoundItem;
        }

        /// <summary>
        /// Creates a display object for <paramref name="sectionResult"/> which is added to the
        /// <see cref="DataGridView"/> on the <see cref="FailureMechanismResultView{T}"/>.
        /// </summary>
        /// <param name="sectionResult">The <typeparamref name="T"/> for which to create a
        /// display object.</param>
        /// <returns>A display object which can be added as a row to the <see cref="DataGridView"/>.</returns>
        protected abstract object CreateFailureMechanismSectionResultRow(T sectionResult);

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

        private void SetCellStyle(DataGridViewCell cell, CellStyle style)
        {
            cell.Style.BackColor = style.BackgroundColor;
            cell.Style.ForeColor = style.TextColor;
        }

        private void RefreshDataGridView()
        {
            dataGridView.Refresh();
            dataGridView.AutoResizeColumns();
        }

        private void AddSectionResultObservers()
        {
            foreach (var sectionResult in failureMechanismSectionResult)
            {
                failureMechanismSectionResultObservers.Add(new Observer(RefreshDataGridView)
                {
                    Observable = sectionResult
                });
            }
        }

        private void ClearSectionResultObservers()
        {
            foreach (var observer in failureMechanismSectionResultObservers)
            {
                observer.Dispose();
            }
            failureMechanismSectionResultObservers.Clear();
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

        private class CellStyle
        {
            public static readonly CellStyle Enabled = new CellStyle
            {
                TextColor = Color.FromKnownColor(KnownColor.ControlText),
                BackgroundColor = Color.FromKnownColor(KnownColor.White)
            };

            public static readonly CellStyle Disabled = new CellStyle
            {
                TextColor = Color.FromKnownColor(KnownColor.GrayText),
                BackgroundColor = Color.FromKnownColor(KnownColor.DarkGray)
            };

            public Color TextColor { get; private set; }
            public Color BackgroundColor { get; private set; }
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
                dataGridView.Rows[e.RowIndex].ErrorText = CoreCommonControlsResources.DataGridViewCellValidating_Text_may_not_be_empty;
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