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

namespace Core.Components.Stack.Data
{
    /// <summary>
    /// This class represents a column in the <see cref="StackChartData"/>.
    /// </summary>
    public class ColumnChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ColumnChartData"/>.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public ColumnChartData(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; }
    }
}