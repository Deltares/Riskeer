﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for determining the colors belonging to various assembly groups.
    /// </summary>
    public static class AssemblyGroupColorHelper
    {
        /// <summary>
        /// Gets the color for a failure mechanism section assembly group.
        /// </summary>
        /// <param name="assemblyGroup">The assembly group to get the color for.</param>
        /// <returns>The <see cref="Color"/> corresponding to the given assembly group.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// has an invalid value for <see cref="FailureMechanismSectionAssemblyGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is not supported.</exception>
        public static Color GetFailureMechanismSectionAssemblyCategoryGroupColor(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyGroup), assemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyGroup),
                                                       (int) assemblyGroup,
                                                       typeof(FailureMechanismSectionAssemblyGroup));
            }

            switch (assemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.ND:
                    return Color.FromArgb(192, 192, 192);
                case FailureMechanismSectionAssemblyGroup.III:
                    return Color.FromArgb(34, 139, 34);
                case FailureMechanismSectionAssemblyGroup.II:
                    return Color.FromArgb(146, 208, 80);
                case FailureMechanismSectionAssemblyGroup.I:
                    return Color.FromArgb(198, 224, 180);
                case FailureMechanismSectionAssemblyGroup.ZeroPlus:
                    return Color.FromArgb(255, 255, 0);
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return Color.FromArgb(255, 165, 0);
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return Color.FromArgb(255, 0, 0);
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return Color.FromArgb(178, 34, 34);
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return Color.FromArgb(128, 0, 0);
                case FailureMechanismSectionAssemblyGroup.D:
                    return Color.FromArgb(255, 90, 172);
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return Color.FromArgb(255, 255, 255);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}