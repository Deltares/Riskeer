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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.Forms
{
    /// <summary>
    /// Converter to convert <see cref="FailureMechanismSectionAssemblyGroup"/>
    /// into <see cref="DisplayFailureMechanismSectionAssemblyGroup"/>.
    /// </summary>
    public static class DisplayFailureMechanismSectionAssemblyGroupConverter
    {
        /// <summary>
        /// Converts <see cref="FailureMechanismSectionAssemblyGroup"/> into
        /// <see cref="DisplayFailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="assemblyGroup">The group to convert.</param>
        /// <returns>The converted <see cref="DisplayFailureMechanismSectionAssemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static DisplayFailureMechanismSectionAssemblyGroup Convert(FailureMechanismSectionAssemblyGroup assemblyGroup)
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
                    return DisplayFailureMechanismSectionAssemblyGroup.ND;
                case FailureMechanismSectionAssemblyGroup.III:
                    return DisplayFailureMechanismSectionAssemblyGroup.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return DisplayFailureMechanismSectionAssemblyGroup.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return DisplayFailureMechanismSectionAssemblyGroup.I;
                case FailureMechanismSectionAssemblyGroup.ZeroPlus:
                    return DisplayFailureMechanismSectionAssemblyGroup.ZeroPlus;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return DisplayFailureMechanismSectionAssemblyGroup.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return DisplayFailureMechanismSectionAssemblyGroup.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return DisplayFailureMechanismSectionAssemblyGroup.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return DisplayFailureMechanismSectionAssemblyGroup.IIIMin;
                case FailureMechanismSectionAssemblyGroup.D:
                    return DisplayFailureMechanismSectionAssemblyGroup.D;
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return DisplayFailureMechanismSectionAssemblyGroup.GR;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}