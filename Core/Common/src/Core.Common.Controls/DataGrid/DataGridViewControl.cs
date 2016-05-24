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

        private void SubscribeEvents()
        {
            dataGridView.CurrentCellDirtyStateChanged += DataGridViewCurrentCellDirtyStateChanged;
        }

        /// <summary>
        /// Adds a new <see cref="DataGridViewTextBoxColumn"/> to the <see cref="DataGridView"/> with the given data.
        /// </summary>
        /// <param name="dataPropertyName">The <see cref="DataGridViewColumn.DataPropertyName"/> of the column.</param>
        /// <param name="headerText">The <see cref="DataGridViewColumn.HeaderText"/> of the column.</param>
        /// <param name="readOnly">Indicates wether the column is read-only or not.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddTextBoxColumn(string dataPropertyName, string headerText, bool readOnly = false)
        {
            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format("column_{0}", dataPropertyName),
                ReadOnly = readOnly
            });
        }

        /// <summary>
        /// Adds a new <see cref="DataGridViewCheckBoxColumn"/> to the <see cref="DataGridView"/> with the given data.
        /// </summary>
        /// <param name="dataPropertyName">The <see cref="DataGridViewColumn.DataPropertyName"/> of the column.</param>
        /// <param name="headerText">The <see cref="DataGridViewColumn.HeaderText"/> of the column.</param>
        /// <remarks><paramref name="dataPropertyName"/> is also used to create the <see cref="DataGridViewColumn.Name"/>.
        /// The format is "column_<paramref name="dataPropertyName"/>.</remarks>
        public void AddCheckBoxColumn(string dataPropertyName, string headerText)
        {
            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                Name = string.Format("column_{0}", dataPropertyName)
            });
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

        #endregion
    }
}
