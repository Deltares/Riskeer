// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Util.Reflection;
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
        public static void AssertExpectedHeaders(IEnumerable<string> expectedHeaderNames, DataGridView actualDataGridView)
        {
            int expectedHeaderNamesCount = expectedHeaderNames.Count();
            Assert.AreEqual(expectedHeaderNamesCount, actualDataGridView.ColumnCount);
            for (var i = 0; i < expectedHeaderNamesCount; i++)
            {
                DataGridViewColumn column = actualDataGridView.Columns[i];
                Assert.AreEqual(expectedHeaderNames.ElementAt(i), column.HeaderText);
            }
        }

        /// <summary>
        /// Asserts that all columns are of the expected classes.
        /// </summary>
        /// <param name="expectedColumnTypes">The column types.</param>
        /// <param name="actualDataGridView">The view.</param>
        /// <exception cref="AssertionException">Thrown when a column is not of the
        /// expected class or there is a mismatch in the number of columns.</exception>
        public static void AssertColumnTypes(IEnumerable<Type> expectedColumnTypes, DataGridView actualDataGridView)
        {
            int expectedColumnTypesCount = expectedColumnTypes.Count();
            Assert.AreEqual(expectedColumnTypesCount, actualDataGridView.ColumnCount);
            for (var i = 0; i < expectedColumnTypesCount; i++)
            {
                DataGridViewColumn column = actualDataGridView.Columns[i];
                Type expectedColumnType = expectedColumnTypes.ElementAt(i);
                Assert.True(column.GetType().Implements(expectedColumnType),
                            "Column type mismatch." + Environment.NewLine +
                            $"Expected: {expectedColumnType.FullName}" + Environment.NewLine +
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
        public static void AssertExpectedRowFormattedValues(IEnumerable<object> expectedFormattedValues, DataGridViewRow actualRow)
        {
            DataGridViewCellCollection rowCells = actualRow.Cells;
            int expectedFormattedValuesCount = expectedFormattedValues.Count();
            Assert.AreEqual(expectedFormattedValuesCount, rowCells.Count);
            for (var i = 0; i < expectedFormattedValuesCount; i++)
            {
                DataGridViewCell cell = rowCells[i];
                Assert.AreEqual(expectedFormattedValues.ElementAt(i), cell.FormattedValue);
            }
        }
    }
}