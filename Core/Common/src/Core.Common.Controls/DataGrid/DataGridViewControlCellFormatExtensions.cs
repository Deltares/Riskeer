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
using System.Windows.Forms;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Extension methods for <see cref="DataGridViewControl"/> to format cells.
    /// </summary>
    public static class DataGridViewControlCellFormatExtensions
    {
        /// <summary>
        /// Formats a cell with the properties as defined in <see cref="DataGridViewColumnStateDefinition"/>.
        /// </summary>
        /// <param name="dataGridViewControl">The <see cref="DataGridViewControl"/> that needs to be formatted.</param>
        /// <param name="rowIndex">The row index of the cell that needs to be formatted.</param>
        /// <param name="columnIndex">The column index of the cell that should be formatted.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataGridViewControl"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="rowIndex"/>
        /// or the <paramref name="columnIndex"/> does not exist.</exception>
        public static void FormatCellWithColumnStateDefinition(this DataGridViewControl dataGridViewControl, int rowIndex, int columnIndex)
        {
            if (dataGridViewControl == null)
            {
                throw new ArgumentNullException(nameof(dataGridViewControl));
            }

            var row = (IHasColumnStateDefinitions) dataGridViewControl.GetRowFromIndex(rowIndex).DataBoundItem;
            if (row.ColumnStateDefinitions.ContainsKey(columnIndex))
            {
                DataGridViewColumnStateDefinition columnStateDefinition = row.ColumnStateDefinitions[columnIndex];
                DataGridViewCell cell = dataGridViewControl.GetCell(rowIndex, columnIndex);

                cell.ReadOnly = columnStateDefinition.ReadOnly;
                cell.ErrorText = columnStateDefinition.ErrorText;
                cell.Style.BackColor = columnStateDefinition.Style.BackgroundColor;
                cell.Style.ForeColor = columnStateDefinition.Style.TextColor;
            }
        }
    }
}