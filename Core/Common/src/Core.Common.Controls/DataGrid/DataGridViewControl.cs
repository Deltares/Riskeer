// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Wrapper for the <see cref="DataGridView"/>.
    /// </summary>
    public partial class DataGridViewControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="DataGridViewControl"/>.
        /// </summary>
        public DataGridViewControl()
        {
            InitializeComponent();

            SubscribeEvents();
        }

        /// <summary>
        /// Returns <c>true</c> when the <see cref="DataGridView.CurrentCell"/> is in edit mode.
        /// <c>False</c> otherwise.
        /// </summary>
        public bool IsCurrentCellInEditMode 
        {
            get
            {
                return dataGridView.IsCurrentCellInEditMode;
            }
        }

        /// <summary>
        /// Returns the <see cref="DataGridView.DataSource"/>.
        /// </summary>
        public object DataSource
        {
            get
            {
                return dataGridView.DataSource;
            }
        }

        /// <summary>
        /// Adds a new <see cref="DataGridViewTextBoxColumn"/> to the <see cref="DataGridView"/> with the given data.
        /// </summary>
        /// <param name="dataPropertyName">The <see cref="DataGridViewColumn.DataPropertyName"/> of the column.</param>
        /// <param name="headerText">The <see cref="DataGridViewColumn.HeaderText"/> of the column.</param>
        /// <param name="readOnly">Indicates whether the column is read-only or not.</param>
        /// <param name="autoSizeMode">The <see cref="DataGridViewColumn.AutoSizeMode"/> of the column.</param>
        /// <param name="minimumWidth">The minimum width of the column.</param>
        /// <param name="format">The text format of the column.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddTextBoxColumn(string dataPropertyName, string headerText, bool readOnly = false, DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, int minimumWidth = 0, string format = null)
        {
            var dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format("column_{0}", dataPropertyName),
                ReadOnly = readOnly,
                DefaultCellStyle =
                {                    
                    DataSourceNullValue = string.Empty
                },
                AutoSizeMode = autoSizeMode
            };

            if (minimumWidth > 0)
            {
                dataGridViewTextBoxColumn.MinimumWidth = minimumWidth;
            }

            if (!string.IsNullOrEmpty(format))
            {
                dataGridViewTextBoxColumn.DefaultCellStyle.Format = format;
            }

            dataGridView.Columns.Add(dataGridViewTextBoxColumn);
        }

        /// <summary>
        /// Adds a new <see cref="DataGridViewCheckBoxColumn"/> to the <see cref="DataGridView"/> with the given data.
        /// </summary>
        /// <param name="dataPropertyName">The <see cref="DataGridViewColumn.DataPropertyName"/> of the column.</param>
        /// <param name="headerText">The <see cref="DataGridViewColumn.HeaderText"/> of the column.</param>
        /// <param name="autoSizeMode">The <see cref="DataGridViewColumn.AutoSizeMode"/> of the column.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddCheckBoxColumn(string dataPropertyName, string headerText, DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.AllCells)
        {
            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format("column_{0}", dataPropertyName),
                AutoSizeMode = autoSizeMode
            });
        }

        /// <summary>
        /// Adds a new <see cref="DataGridViewComboBoxColumn"/> to the <see cref="DataGridView"/> with the given data.
        /// </summary>
        /// <param name="dataPropertyName">The <see cref="DataGridViewColumn.DataPropertyName"/> of the column.</param>
        /// <param name="headerText">The <see cref="DataGridViewColumn.HeaderText"/> of the column.</param>
        /// <param name="dataSource">The datasource that is set on the column.</param>
        /// <param name="valueMember">The <see cref="DataGridViewComboBoxColumn.ValueMember"/> of the column.</param>
        /// <param name="displayMember">The <see cref="DataGridViewComboBoxColumn.DisplayMember"/> of the column.</param>
        /// <param name="autoSizeMode">The <see cref="DataGridViewColumn.AutoSizeMode"/> of the column.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddComboBoxColumn<T>(string dataPropertyName, string headerText, IEnumerable<T> dataSource, string valueMember, string displayMember, DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.AllCells)
        {
            var dataGridViewComboBoxColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format("column_{0}", dataPropertyName),
                AutoSizeMode = autoSizeMode
            };

            if (dataSource != null)
            {
                dataGridViewComboBoxColumn.DataSource = dataSource;
            }

            if (valueMember != null)
            {
                dataGridViewComboBoxColumn.ValueMember = valueMember;
            }

            if (displayMember != null)
            {
                dataGridViewComboBoxColumn.DisplayMember = displayMember;
            }

            dataGridView.Columns.Add(dataGridViewComboBoxColumn);
        }

        /// <summary>
        /// Sets the datasource on the <see cref="DataGridView"/>.
        /// </summary>
        /// <param name="dataSource">The datasource to set.</param>
        public void SetDataSource(object dataSource)
        {
            dataGridView.DataSource = dataSource;
        }

        /// <summary>
        /// Refreshes the <see cref="DataGridView"/> and performs an <see cref="DataGridView.AutoResizeColumns()"/>.
        /// </summary>
        public void RefreshDataGridView()
        {
            dataGridView.Refresh();
            AutoResizeColumns();
        }

        /// <summary>
        /// Adjusts the width of all columns to fit the contents of all their cells, including the header cells.
        /// </summary>
        public void AutoResizeColumns()
        {
            dataGridView.AutoResizeColumns();
        }

        /// <summary>
        /// Ends the editing when the current cell is in edit mode.
        /// Sets the current cell to <c>null</c>.
        /// </summary>
        public void EndEdit()
        {
            if (IsCurrentCellInEditMode)
            {
                dataGridView.CancelEdit();
                dataGridView.EndEdit();
                dataGridView.CurrentCell = null;
            }
        }

        /// <summary>
        /// Gets all the rows of the <see cref="DataGridView"/>.
        /// </summary>
        /// <returns>A <see cref="DataGridViewRowCollection"/> with all the rows of the <see cref="DataGridView"/>.</returns>
        public DataGridViewRowCollection GetRows()
        {
            return dataGridView.Rows;
        }

        /// <summary>
        /// Gets the <see cref="DataGridViewRow"/> on the given index.
        /// </summary>
        /// <param name="rowIndex">The index of the row.</param>
        /// <returns>A <see cref="DataGridViewRow"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index of the row does not exist.</exception>
        public DataGridViewRow GetRowFromIndex(int rowIndex)
        {
            return dataGridView.Rows[rowIndex];
        }

        /// <summary>
        /// Gets the row containing the current cell.
        /// </summary>
        /// <returns>The <see cref="DataGridViewRow"/> that represents the row containing the current cell, 
        /// or <c>null</c> if there is no current cell.</returns>
        public DataGridViewRow GetCurrentRow()
        {
            return dataGridView.CurrentRow;
        }

        /// <summary>
        /// Gets the <see cref="DataGridViewCell"/> on the given row and column index.
        /// </summary>
        /// <param name="rowIndex">The index of the row the cell is on.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>A <see cref="DataGridViewCell"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index of the row or column does not exist.</exception>
        public DataGridViewCell GetCell(int rowIndex, int columnIndex)
        {
            return GetRowFromIndex(rowIndex).Cells[columnIndex];
        }

        /// <summary>
        /// Gets the <see cref="DataGridViewColumn"/> on the given index.
        /// </summary>
        /// <param name="columnIndex">The index of the column.</param>
        /// <returns>A <see cref="DataGridViewColumn"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index of the column does not exist.</exception>
        public DataGridViewColumn GetColumnFromIndex(int columnIndex)
        {
            return dataGridView.Columns[columnIndex];
        }

        #region Styling

        /// <summary>
        /// Restore the initial style of the cell at <paramref name="rowIndex"/>, <paramref name="columnIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        /// <param name="readOnly">Indicates wether the column should be read-only.</param>
        public void RestoreCell(int rowIndex, int columnIndex, bool readOnly = false)
        {
            var cell = GetCell(rowIndex, columnIndex);
            cell.ReadOnly = readOnly;
            SetCellStyle(cell, CellStyle.Enabled);
        }

        /// <summary>
        /// Gives the cell at <paramref name="rowIndex"/>, <paramref name="columnIndex"/> a
        /// disabled style.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        public void DisableCell(int rowIndex, int columnIndex)
        {
            var cell = GetCell(rowIndex, columnIndex);
            cell.ReadOnly = true;
            SetCellStyle(cell, CellStyle.Disabled);
        }

        private void SetCellStyle(DataGridViewCell cell, CellStyle style)
        {
            cell.Style.BackColor = style.BackgroundColor;
            cell.Style.ForeColor = style.TextColor;
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Add a handler for the <see cref="DataGridView.CellFormatting"/> event.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void AddCellFormattingHandler(DataGridViewCellFormattingEventHandler handler)
        {
            dataGridView.CellFormatting += handler;
        }

        /// <summary>
        /// Remove a handler from the <see cref="DataGridView.CellFormatting"/> event.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        public void RemoveCellFormattingHandler(DataGridViewCellFormattingEventHandler handler)
        {
            dataGridView.CellFormatting -= handler;
        }

        /// <summary>
        /// Add a handler for the <see cref="DataGridView.CellClick"/> event.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void AddCellClickHandler(DataGridViewCellEventHandler handler)
        {
            dataGridView.CellClick += handler;
        }

        /// <summary>
        /// Remove a handler from the <see cref="DataGridView.CellClick"/> event.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        public void RemoveCellClickHandler(DataGridViewCellEventHandler handler)
        {
            dataGridView.CellClick -= handler;
        }

        private void SubscribeEvents()
        {
            dataGridView.ColumnAdded += DataGridViewOnColumnAdded;
            dataGridView.CurrentCellDirtyStateChanged += DataGridViewOnCurrentCellDirtyStateChanged;
            dataGridView.GotFocus += DataGridViewOnGotFocus;
            dataGridView.CellEndEdit += DataGridViewOnCellEndEdit;
            dataGridView.DataError += DataGridViewOnDataError;
        }

        private void DataGridViewOnColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void DataGridViewOnCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Ensure checkbox values are directly committed
            DataGridViewColumn currentColumn = dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex];
            if (currentColumn is DataGridViewCheckBoxColumn || currentColumn is DataGridViewComboBoxColumn)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DataGridViewOnGotFocus(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell != null)
            {
                dataGridView.BeginEdit(true); // Always start editing after setting the focus (otherwise data grid view cell dirty events are no longer fired when using the keyboard...)
            }
        }

        private void DataGridViewOnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void DataGridViewOnDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;

            if (e.Exception != null)
            {
                dataGridView.Rows[e.RowIndex].ErrorText = e.Exception.Message;
            }
        }

        #endregion
    }
}