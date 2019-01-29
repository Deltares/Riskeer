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

using System.Collections.Generic;

namespace Core.Common.Controls.DataGrid
{
    /// <summary>
    /// Interface for the row objects in a <see cref="DataGridViewControl"/> that have an <see cref="IDictionary{TKey,TValue}"/>
    /// of column indices with the belonging <see cref="DataGridViewColumnStateDefinition"/>.
    /// </summary>
    public interface IHasColumnStateDefinitions
    {
        /// <summary>
        /// Gets the column state definitions.
        /// </summary>
        IDictionary<int, DataGridViewColumnStateDefinition> ColumnStateDefinitions { get; }
    }
}