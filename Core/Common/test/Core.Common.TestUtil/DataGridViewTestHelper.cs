// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// This class defines helper methods for testing <see cref="DataGridView"/>.
    /// </summary>
    public static class DataGridViewTestHelper
    {
        /// <summary>
        /// Asserts that all columns match the expected header text.
        /// </summary>
        /// <param name="expectedHeaderNames">The expected header text.</param>
        /// <param name="actualDataGridView">The view.</param>
        /// <exception cref="AssertionException">Thrown when a column does not have the
        /// expected header text or there is a mismatch in the number of columns.</exception>
        public static void AssertExpectedHeaders(IList<string> expectedHeaderNames, DataGridView actualDataGridView)
        {
            Assert.AreEqual(expectedHeaderNames.Count, actualDataGridView.ColumnCount);
            for (var i = 0; i < expectedHeaderNames.Count; i++)
            {
                DataGridViewColumn column = actualDataGridView.Columns[i];
                Assert.AreEqual(expectedHeaderNames[i], column.HeaderText);
            }
        }

        /// <summary>
        /// Asserts that all columns are of the expected classes.
        /// </summary>
        /// <param name="expectedColumnTypes">The column types.</param>
        /// <param name="actualDataGridView">The view.</param>
        /// <exception cref="AssertionException">Thrown when a column is not of the
        /// expected class or there is a mismatch in the number of columns.</exception>
        public static void AssertColumnTypes(IList<Type> expectedColumnTypes, DataGridView actualDataGridView)
        {
            Assert.AreEqual(expectedColumnTypes.Count, actualDataGridView.ColumnCount);
            for (var i = 0; i < expectedColumnTypes.Count; i++)
            {
                DataGridViewColumn column = actualDataGridView.Columns[i];
                Assert.True(column.GetType().Implements(expectedColumnTypes[i]),
                            "Column type mismatch." + Environment.NewLine +
                            $"Expected: {expectedColumnTypes[i].FullName}" + Environment.NewLine +
                            $"Actual: {column.GetType().FullName}");
            }
        }

        /// <summary>
        /// Asserts that all cells in the given row match the expected formatted values.
        /// </summary>
        /// <param name="expectedFormattedValues">The expected formatted values.</param>
        /// <param name="actualRow">The row.</param>
        /// <exception cref="AssertionException">Thrown when a cell does not have the
        /// expected formatted value or there is a mismatch in the number of cells in the row.</exception>
        public static void AssertExpectedRowFormattedValues(IList expectedFormattedValues, DataGridViewRow actualRow)
        {
            DataGridViewCellCollection rowCells = actualRow.Cells;
            Assert.AreEqual(expectedFormattedValues.Count, rowCells.Count);
            for (var i = 0; i < expectedFormattedValues.Count; i++)
            {
                DataGridViewCell cell = rowCells[i];
                Assert.AreEqual(expectedFormattedValues[i], cell.FormattedValue);
            }
        }

        /// <summary>
        /// Asserts whether a <see cref="DataGridViewCell"/> is in a disabled state.
        /// </summary>
        /// <param name="dataGridViewCell">The cell that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The readonly property of the cell is <c>false</c>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.ForeColor"/> is not <see cref="KnownColor.GrayText"/>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.BackColor"/> is not <see cref="KnownColor.DarkGray"/>.</item>
        /// </list></exception>
        public static void AssertCellIsDisabled(DataGridViewCell dataGridViewCell)
        {
            Assert.AreEqual(true, dataGridViewCell.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), dataGridViewCell.Style.ForeColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.DarkGray), dataGridViewCell.Style.BackColor);
        }

        /// <summary>
        /// Asserts whether a <see cref="DataGridViewCell"/> is in an enabled state.
        /// </summary>
        /// <param name="dataGridViewCell">The cell that needs to be asserted.</param>
        /// <param name="isReadOnly">Flag indicating whether the cell is readonly</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The readonly property of the cell does not match with <paramref name="isReadOnly"/>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.ForeColor"/> is not <see cref="KnownColor.ControlText"/>.</item>
        /// <item>The <see cref="DataGridViewCellStyle.BackColor"/> is not <see cref="KnownColor.White"/>.</item>
        /// </list></exception>
        public static void AssertCellIsEnabled(DataGridViewCell dataGridViewCell, bool isReadOnly = false)
        {
            Assert.AreEqual(isReadOnly, dataGridViewCell.ReadOnly);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), dataGridViewCell.Style.ForeColor);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.White), dataGridViewCell.Style.BackColor);
        }
    }
}