// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Wrapper for the <see cref="DataGridView"/>.
    /// </summary>
    public partial class DataGridViewControl : UserControl
    {
        private int lastSelectedRow;

        /// <summary>
        /// Occurs when it's the first selection or when a cell in a row different 
        /// from the previously selected row is selected.
        /// </summary>
        public event EventHandler CurrentRowChanged
        {
            add
            {
                RowChanged += value;
            }
            remove
            {
                RowChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="DataGrid.CurrentCell"/> property changes.
        /// </summary>
        public event EventHandler CurrentCellChanged
        {
            add
            {
                dataGridView.CurrentCellChanged += value;
            }
            remove
            {
                dataGridView.CurrentCellChanged -= value;
            }
        }

        /// <summary>
        /// Occurs when the contents of a cell need to be formatted for display.
        /// </summary>
        public event DataGridViewCellFormattingEventHandler CellFormatting
        {
            add
            {
                dataGridView.CellFormatting += value;
            }
            remove
            {
                dataGridView.CellFormatting -= value;
            }
        }

        /// <summary>
        /// Occurs when the value of a cell changes.
        /// </summary>
        public event DataGridViewCellEventHandler CellValueChanged
        {
            add
            {
                dataGridView.CellValueChanged += value;
            }
            remove
            {
                dataGridView.CellValueChanged -= value;
            }
        }

        private event EventHandler RowChanged;

        /// <summary>
        /// Creates a new instance of <see cref="DataGridViewControl"/>.
        /// </summary>
        public DataGridViewControl()
        {
            InitializeComponent();
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            SubscribeEvents();

            ResetLastSelectedRow();
        }

        /// <summary>
        /// Returns <c>true</c> when the <see cref="DataGridView.CurrentCell"/> is in edit mode,
        /// <c>false</c> otherwise.
        /// </summary>
        public bool IsCurrentCellInEditMode
        {
            get
            {
                return dataGridView.IsCurrentCellInEditMode;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to select more than 
        /// one cell, row, or column of the <see cref="DataGridViewControl"/> at a time.
        /// </summary>
        /// <remarks>See <see cref="DataGridView.MultiSelect"/>.</remarks>
        public bool MultiSelect
        {
            get
            {
                return dataGridView.MultiSelect;
            }
            set
            {
                dataGridView.MultiSelect = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how the cells of the <see cref="DataGridViewControl"/> can be selected.
        /// </summary>
        /// <remarks>See <see cref="DataGridViewSelectionMode"/>.</remarks>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the specified value to set is 
        /// not a valid <see cref="DataGridViewSelectionMode"/> value.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the specified value to set is 
        /// <see cref="DataGridViewSelectionMode.FullColumnSelect"/> or <see cref="DataGridViewSelectionMode.ColumnHeaderSelect"/> 
        /// and the SortMode property of one or more columns is set to <c>Automatic</c>.</exception>
        public DataGridViewSelectionMode SelectionMode
        {
            get
            {
                return dataGridView.SelectionMode;
            }
            set
            {
                dataGridView.SelectionMode = value;
            }
        }

        /// <summary>
        /// Gets a <see cref="DataGridViewRowCollection"/> with all the rows of the <see cref="DataGridView"/>.
        /// </summary>
        public DataGridViewRowCollection Rows
        {
            get
            {
                return dataGridView.Rows;
            }
        }

        /// <summary>
        /// Gets the <see cref="DataGridViewRow"/> that represents the row containing the current cell, 
        /// or <c>null</c> if there is no current cell.
        /// </summary>
        public DataGridViewRow CurrentRow
        {
            get
            {
                return dataGridView.CurrentRow;
            }
        }

        /// <summary>
        /// Clears the current cell and resets the last selected row.
        /// </summary>
        public void ClearCurrentCell()
        {
            dataGridView.CurrentCell = null;
            ResetLastSelectedRow();
        }

        /// <summary>
        /// Sets the currently active cell.
        /// </summary>
        /// <param name="cell">Sets the cell to be set active.</param>
        /// <exception cref="ArgumentException">Thrown when:<list type="bullet">
        /// <item><paramref name="cell"/> is not in the DataGridView;</item>
        /// <item><paramref name="cell"/> cannot be 
        /// set because changes to the current cell cannot be committed or canceled.</item>
        /// </list>
        /// </exception>
        public void SetCurrentCell(DataGridViewCell cell)
        {
            try
            {
                dataGridView.CurrentCell = cell;
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidOperationException)
            {
                throw new ArgumentException(@"Unable to set the cell active.", nameof(cell), e);
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
                Name = string.Format(CultureInfo.InvariantCulture,
                                     "column_{0}",
                                     dataPropertyName),
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
        /// <param name="readOnly">Indicates whether the column is read-only or not.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddCheckBoxColumn(string dataPropertyName, string headerText, bool readOnly = false)
        {
            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format(CultureInfo.InvariantCulture,
                                     "column_{0}",
                                     dataPropertyName),
                ReadOnly = readOnly,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
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
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddComboBoxColumn<T>(string dataPropertyName, string headerText, IEnumerable<T> dataSource, string valueMember, string displayMember)
        {
            var dataGridViewComboBoxColumn = new DataGridViewComboBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format(CultureInfo.InvariantCulture,
                                     "column_{0}",
                                     dataPropertyName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                FlatStyle = FlatStyle.Flat
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
        /// Adds a new read-only <see cref="DataGridViewColorColumn"/> to the <see cref="DataGridView"/> with the given data.
        /// </summary>
        /// <param name="dataPropertyName">The <see cref="DataGridViewColumn.DataPropertyName"/> of the column.</param>
        /// <param name="headerText">The <see cref="DataGridViewColumn.HeaderText"/> of the column.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddColorColumn(string dataPropertyName, string headerText)
        {
            var colorColumn = new DataGridViewColorColumn
            {
                Name = string.Format(CultureInfo.InvariantCulture,
                                     "column_{0}",
                                     dataPropertyName),
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                ReadOnly = true
            };

            dataGridView.Columns.Add(colorColumn);
        }

        /// <summary>
        /// Sets the data source on the <see cref="DataGridView"/>.
        /// </summary>
        /// <param name="dataSource">The data source to set.</param>
        /// <remarks>Providing a value of <c>null</c> for <paramref name="dataSource"/>
        /// will clear the grid view.</remarks>
        public void SetDataSource(IEnumerable dataSource)
        {
            dataGridView.DataSource = dataSource;
        }

        /// <summary>
        /// Refreshes the <see cref="DataGridView"/>.
        /// </summary>
        /// <param name="shouldAutoResizeColumns">Indicator whether the column widths in the control should automatically be resized.</param>
        public void RefreshDataGridView(bool shouldAutoResizeColumns = true)
        {
            dataGridView.Refresh();

            if (shouldAutoResizeColumns)
            {
                AutoResizeColumns();
            }
        }

        /// <summary>
        /// Adjusts the width of all columns to fit the contents of all their cells, including the header cells.
        /// </summary>
        public void AutoResizeColumns()
        {
            dataGridView.AutoResizeColumns();
        }

        /// <summary>
        /// Adjusts the width of the column on index <paramref name="columnIndex"/> to fit the contents of all its cells,
        /// including the header cell.
        /// </summary>
        public void AutoResizeColumn(int columnIndex)
        {
            dataGridView.AutoResizeColumn(columnIndex);
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
                EndCellEdit();
            }
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

        protected override void OnLoad(EventArgs e)
        {
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            base.OnLoad(e);
        }

        private void ResetLastSelectedRow()
        {
            lastSelectedRow = -1;
        }

        #region Styling

        /// <summary>
        /// Restore the initial style of the cell at <paramref name="rowIndex"/>, <paramref name="columnIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        /// <remarks>The read-only state of the cell is set according to the read-only state of the corresponding column.</remarks>
        public void RestoreCell(int rowIndex, int columnIndex)
        {
            RestoreCell(rowIndex, columnIndex, GetColumnFromIndex(columnIndex).ReadOnly);
        }

        /// <summary>
        /// Restore the initial style of the cell at <paramref name="rowIndex"/>, <paramref name="columnIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="columnIndex">The column index of the cell.</param>
        /// <param name="readOnly">Indicates whether the cell should be read-only.</param>
        public void RestoreCell(int rowIndex, int columnIndex, bool readOnly)
        {
            DataGridViewCell cell = GetCell(rowIndex, columnIndex);
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
            DataGridViewCell cell = GetCell(rowIndex, columnIndex);
            cell.ReadOnly = true;
            SetCellStyle(cell, CellStyle.Disabled);
        }

        /// <summary>
        /// Sets the visibility of the column at the given index.
        /// </summary>
        /// <param name="columnIndex">The index of the column.</param>
        /// <param name="isVisible">The visibility of the column.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the 
        /// index of the column does not exist.</exception>
        public void SetColumnVisibility(int columnIndex, bool isVisible)
        {
            GetColumnFromIndex(columnIndex).Visible = isVisible;
        }

        private static void SetCellStyle(DataGridViewCell cell, CellStyle style)
        {
            cell.Style.BackColor = style.BackgroundColor;
            cell.Style.ForeColor = style.TextColor;
        }

        #endregion

        #region Event handling

        private void DataGridViewOnCurrentCellChanged(object o, EventArgs eventArgs)
        {
            if (RowChanged == null)
            {
                return;
            }

            if (CurrentRow == null)
            {
                RowChanged.Invoke(o, eventArgs);
                ResetLastSelectedRow();
                return;
            }

            if (lastSelectedRow == CurrentRow.Index)
            {
                return;
            }

            RowChanged.Invoke(o, eventArgs);
            lastSelectedRow = CurrentRow.Index;
        }

        private void SubscribeEvents()
        {
            dataGridView.ColumnAdded += DataGridViewOnColumnAdded;
            dataGridView.CurrentCellDirtyStateChanged += DataGridViewOnCurrentCellDirtyStateChanged;
            dataGridView.CellEndEdit += DataGridViewOnCellEndEdit;
            dataGridView.DataError += DataGridViewOnDataError;
            dataGridView.Leave += DataGridViewOnLeave;
            dataGridView.CellClick += DataGridViewOnCellClick;
            dataGridView.CurrentCellChanged += DataGridViewOnCurrentCellChanged;
        }

        private void DataGridViewOnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
            {
                return;
            }

            dataGridView.BeginEdit(true);
            var combobox = dataGridView.EditingControl as ComboBox;
            if (combobox != null)
            {
                combobox.DroppedDown = true;
            }
        }

        private static void DataGridViewOnColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void DataGridViewOnCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // Ensure checkbox and combobox values are directly committed
            DataGridViewColumn currentColumn = dataGridView.Columns[dataGridView.CurrentCell.ColumnIndex];
            if (currentColumn is DataGridViewCheckBoxColumn || currentColumn is DataGridViewComboBoxColumn)
            {
                dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dataGridView.EndEdit();
                dataGridView.Refresh();
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

        private void DataGridViewOnLeave(object sender, EventArgs eventArgs)
        {
            if (!Disposing && IsCurrentCellInEditMode)
            {
                // Try to end the edit action
                if (!dataGridView.EndEdit())
                {
                    // Cancel the edit action on validation errors
                    dataGridView.CancelEdit();
                    dataGridView.CurrentCell.OwningRow.ErrorText = string.Empty;
                }

                EndCellEdit();
            }
        }

        private void EndCellEdit()
        {
            DataGridViewCell currentCell = dataGridView.CurrentCell;

            // End edits of current cell:
            ClearCurrentCell();

            // Restore selection highlight:
            dataGridView.CurrentCell = currentCell;
        }

        #endregion
    }
}