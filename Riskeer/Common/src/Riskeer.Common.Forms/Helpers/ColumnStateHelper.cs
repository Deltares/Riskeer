// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Controls.DataGrid;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for updating states of a <see cref="DataGridViewColumnStateDefinition"/>.
    /// </summary>
    public static class ColumnStateHelper
    {
        /// <summary>
        /// Helper method that sets the state of the <paramref name="columnStateDefinition"/>
        /// based on <paramref name="shouldDisable"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to set the state for.</param>
        /// <param name="shouldDisable">Indicator whether the column should be disabled or not.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void SetColumnState(DataGridViewColumnStateDefinition columnStateDefinition, bool shouldDisable)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            if (shouldDisable)
            {
                DisableColumn(columnStateDefinition);
            }
            else
            {
                EnableColumn(columnStateDefinition);
            }
        }

        /// <summary>
        /// Helper method that enables the <paramref name="columnStateDefinition"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to enable.</param>
        /// <param name="readOnly">Indicator whether the column should be read-only or not.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void EnableColumn(DataGridViewColumnStateDefinition columnStateDefinition, bool readOnly = false)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            columnStateDefinition.ReadOnly = readOnly;
            columnStateDefinition.Style = CellStyle.Enabled;
        }

        /// <summary>
        /// Helper method that disables the <paramref name="columnStateDefinition"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to enable.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void DisableColumn(DataGridViewColumnStateDefinition columnStateDefinition)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            columnStateDefinition.ReadOnly = true;
            columnStateDefinition.Style = CellStyle.Disabled;
        }

    }
}
