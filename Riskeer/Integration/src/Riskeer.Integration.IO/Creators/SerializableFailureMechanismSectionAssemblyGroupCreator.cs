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
using Riskeer.AssemblyTool.Data.Old;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create <see cref="SerializableFailureMechanismSectionAssemblyGroup"/>.
    /// </summary>
    public static class SerializableFailureMechanismSectionAssemblyGroupCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableFailureMechanismSectionAssemblyGroup"/> based on <paramref name="assemblyGroup"/>.
        /// </summary>
        /// <param name="assemblyGroup">The <see cref="FailureMechanismSectionAssemblyGroup"/> to
        /// create a <see cref="SerializableFailureMechanismSectionAssemblyGroup"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static SerializableFailureMechanismSectionAssemblyGroup Create(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), assemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyGroup),
                                                       (int) assemblyGroup,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (assemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.NotDominant:
                    return SerializableFailureMechanismSectionAssemblyGroup.NotDominant;
                case FailureMechanismSectionAssemblyGroup.III:
                    return SerializableFailureMechanismSectionAssemblyGroup.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return SerializableFailureMechanismSectionAssemblyGroup.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return SerializableFailureMechanismSectionAssemblyGroup.I;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return SerializableFailureMechanismSectionAssemblyGroup.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return SerializableFailureMechanismSectionAssemblyGroup.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return SerializableFailureMechanismSectionAssemblyGroup.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return SerializableFailureMechanismSectionAssemblyGroup.IIIMin;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}