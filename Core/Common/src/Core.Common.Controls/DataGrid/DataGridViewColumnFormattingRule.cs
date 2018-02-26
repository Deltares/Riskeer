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
using System.Collections.Generic;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Class to define formatting rule on a column.
    /// </summary>
    /// <typeparam name="T">The type of object to use for the formatting rule.</typeparam>
    public class DataGridViewColumnFormattingRule<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DataGridViewColumnFormattingRule{T}"/>
        /// </summary>
        /// <param name="columnIndices">The column indices that the rule apply to.</param>
        /// <param name="rule">The rule to evaluate.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DataGridViewColumnFormattingRule(IEnumerable<int> columnIndices,
                                                Func<T, bool> rule)
        {
            if (columnIndices == null)
            {
                throw new ArgumentNullException(nameof(columnIndices));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            ColumnIndices = columnIndices;
            Rule = rule;
        }

        /// <summary>
        /// Gets the column indices that the rule applies to. </summary>
        public IEnumerable<int> ColumnIndices { get; }

        /// <summary>
        /// Gets the rule to evaluate.
        /// </summary>
        public Func<T, bool> Rule { get; }
    }
}