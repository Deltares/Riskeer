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
using System.Drawing;
using System.Linq;
using Core.Common.Base;

namespace Core.Components.Stack.Data
{
    /// <summary>
    /// Class for data with the purpose of becoming visible in stack charting components.
    /// </summary>
    public class StackChartData : Observable
    {
        private readonly List<string> columns;
        private readonly List<RowChartData> rows;

        /// <summary>
        /// Creates a new instance of <see cref="StackChartData"/>.
        /// </summary>
        public StackChartData()
        {
            columns = new List<string>();
            rows = new List<RowChartData>();
        }

        /// <summary>
        /// Gets the columns of the <see cref="StackChartData"/>.
        /// </summary>
        public IEnumerable<string> Columns
        {
            get
            {
                return columns;
            }
        }

        /// <summary>
        /// Gets the rows of the <see cref="StackChartData"/>.
        /// </summary>
        public IEnumerable<RowChartData> Rows
        {
            get
            {
                return rows;
            }
        }

        /// <summary>
        /// Adds a column to the <see cref="StackChartData"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public void AddColumn(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            columns.Add(name);
        }

        /// <summary>
        /// Adds a columns to <see cref="StackChartData"/> with
        /// values to add to the rows that are already present.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="values">The values to add to the rows for this column.</param>
        /// <exception cref="InvalidOperationException">Thrown when no <see cref="Rows"/>
        /// are present.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the amount of
        /// <paramref name="values"/> is not equal to the amount of <see cref="Rows"/>.</exception>
        public void AddColumnWithValues(string name, List<double> values)
        {
            if (!Rows.Any())
            {
                throw new InvalidOperationException("Rows should be added before this method is called.");
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Count != rows.Count)
            {
                throw new ArgumentException("The number of value items must be the same as the number of rows.");
            }

            AddColumn(name);

            for (var i = 0; i < values.Count; i++)
            {
                rows[i].Values.Add(values[i]);
            }
        }

        /// <summary>
        /// Adds a row to the <see cref="StackChartData"/>.
        /// </summary>
        /// <param name="name">The name of the row.</param>
        /// <param name="values">The values of the row.</param>
        /// <param name="color">The color of the row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> 
        /// or <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the amount of 
        /// <paramref name="values"/> is not equal to the amount of <see cref="Columns"/>.</exception>
        public void AddRow(string name, List<double> values, Color? color = null)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Count != columns.Count)
            {
                throw new ArgumentException("The number of value items must be the same as the number of columns.");
            }

            rows.Add(new RowChartData(name, values, color));
        }

        /// <summary>
        /// Clears the rows and columns of the <see cref="StackChartData"/>.
        /// </summary>
        public void Clear()
        {
            columns.Clear();
            rows.Clear();
        }
    }
}