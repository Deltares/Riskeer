// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for displaying <see cref="FailureMechanismSectionAssemblyGroup"/>.
    /// </summary>
    public static class FailureMechanismSectionAssemblyGroupHelper
    {
        /// <summary>
        /// Gets the display name of the given <paramref name="assemblyGroup"/>.
        /// </summary>
        /// <param name="assemblyGroup">The assembly group to get the display name for.</param>
        /// <returns>The display name.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static string GetAssemblyGroupDisplayName(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyGroup), assemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyGroup),
                                                       (int) assemblyGroup,
                                                       typeof(FailureMechanismSectionAssemblyGroup));
            }

            DisplayFailureMechanismSectionAssemblyGroup displayAssemblyGroup = DisplayFailureMechanismSectionAssemblyGroupConverter.Convert(
                assemblyGroup);

            return new EnumDisplayWrapper<DisplayFailureMechanismSectionAssemblyGroup>(displayAssemblyGroup).DisplayName;
        }
    }
}