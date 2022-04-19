﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.DataGrid;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for updating states of a <see cref="DataGridViewColumnStateDefinition"/>
    /// in a <see cref="FailureMechanismSectionResultRow{T}"/>.
    /// </summary>
    public static class FailureMechanismSectionResultRowHelper
    {
        /// <summary>
        /// Helper method that sets the style of a <paramref name="columnStateDefinition"/> based on a
        /// <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="columnStateDefinition">The column state definition to set the style for.</param>
        /// <param name="assemblyGroup">The assembly group to base the style on.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// has an invalid value for <see cref="FailureMechanismSectionAssemblyGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is not supported.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnStateDefinition"/>
        /// is <c>null</c>.</exception>
        public static void SetAssemblyGroupStyle(DataGridViewColumnStateDefinition columnStateDefinition,
                                                 FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            if (columnStateDefinition == null)
            {
                throw new ArgumentNullException(nameof(columnStateDefinition));
            }

            columnStateDefinition.Style = new CellStyle(
                Color.FromKnownColor(KnownColor.ControlText),
                FailureMechanismSectionAssemblyGroupColorHelper.GetFailureMechanismSectionAssemblyGroupColor(assemblyGroup));
        }
    }
}