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

using System.Collections.Generic;
using Core.Common.Base;

namespace Core.Components.Stack.Data
{
    /// <summary>
    /// Class for data with the purpose of becoming visible in stack charting components.
    /// </summary>
    public class StackChartData : Observable
    {
        /// <summary>
        /// Creates a new instance of <see cref="StackChartData"/>.
        /// </summary>
        public StackChartData()
        {
            Columns = new List<ColumnChartData>();
            Rows = new List<RowChartData>();
        }

        /// <summary>
        /// Gets the columns of the <see cref="StackChartData"/>.
        /// </summary>
        public List<ColumnChartData> Columns { get; }

        /// <summary>
        /// Gets the rows of the <see cref="StackChartData"/>.
        /// </summary>
        public List<RowChartData> Rows { get; }

        /// <summary>
        /// Adds a column to the <see cref="StackChartData"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public void AddColumn(string name)
        {
            Columns.Add(new ColumnChartData(name));
        }

        /// <summary>
        /// Adds a row to the <see cref="StackChartData"/>.
        /// </summary>
        /// <param name="row">The row to add.</param>
        public void AddRow(RowChartData row)
        {
            // Check if the number of items is the same as the number of columns
            Rows.Add(row);
        }
    }
}