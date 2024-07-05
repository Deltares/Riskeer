// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.Integration.IO.Converters
{
    /// <summary>
    /// Converter for converting <see cref="FailureMechanismSectionAssemblyGroup"/> into <see cref="ExportableFailureMechanismSectionAssemblyGroup"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionAssemblyGroupConverter
    {
        /// <summary>
        /// Converts a <see cref="FailureMechanismSectionAssemblyGroup"/> into an <see cref="ExportableFailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="failureMechanismSectionAssemblyGroup">The <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// to convert into an <see cref="ExportableFailureMechanismSectionAssemblyGroup"/>.</param>
        /// <returns>An <see cref="ExportableFailureMechanismSectionAssemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="failureMechanismSectionAssemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="failureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static ExportableFailureMechanismSectionAssemblyGroup ConvertTo(
            FailureMechanismSectionAssemblyGroup failureMechanismSectionAssemblyGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyGroup), failureMechanismSectionAssemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(failureMechanismSectionAssemblyGroup),
                                                       (int) failureMechanismSectionAssemblyGroup,
                                                       typeof(FailureMechanismSectionAssemblyGroup));
            }

            switch (failureMechanismSectionAssemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.NotDominant:
                    return ExportableFailureMechanismSectionAssemblyGroup.NotDominant;
                case FailureMechanismSectionAssemblyGroup.III:
                    return ExportableFailureMechanismSectionAssemblyGroup.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return ExportableFailureMechanismSectionAssemblyGroup.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return ExportableFailureMechanismSectionAssemblyGroup.I;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return ExportableFailureMechanismSectionAssemblyGroup.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return ExportableFailureMechanismSectionAssemblyGroup.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return ExportableFailureMechanismSectionAssemblyGroup.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return ExportableFailureMechanismSectionAssemblyGroup.IIIMin;
                case FailureMechanismSectionAssemblyGroup.Dominant:
                    return ExportableFailureMechanismSectionAssemblyGroup.Dominant;
                case FailureMechanismSectionAssemblyGroup.NoResult:
                    return ExportableFailureMechanismSectionAssemblyGroup.NoResult;
                case FailureMechanismSectionAssemblyGroup.NotRelevant:
                    return ExportableFailureMechanismSectionAssemblyGroup.NotRelevant;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}