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
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Converter to convert <see cref="EInterpretationCategory"/> into
    /// <see cref="FailureMechanismSectionAssemblyGroup"/> and back.
    /// </summary>
    public static class FailureMechanismSectionAssemblyGroupConverter
    {
        /// <summary>
        /// Converts an <see cref="EInterpretationCategory"/> into a <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="interpretationCategory">The <see cref="EInterpretationCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyGroup"/> based on <paramref name="interpretationCategory"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="interpretationCategory"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="interpretationCategory"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyGroup ConvertTo(EInterpretationCategory interpretationCategory)
        {
            if (!Enum.IsDefined(typeof(EInterpretationCategory), interpretationCategory))
            {
                throw new InvalidEnumArgumentException(nameof(interpretationCategory),
                                                       (int) interpretationCategory,
                                                       typeof(EInterpretationCategory));
            }

            switch (interpretationCategory)
            {
                case EInterpretationCategory.NotDominant:
                    return FailureMechanismSectionAssemblyGroup.NotDominant;
                case EInterpretationCategory.III:
                    return FailureMechanismSectionAssemblyGroup.III;
                case EInterpretationCategory.II:
                    return FailureMechanismSectionAssemblyGroup.II;
                case EInterpretationCategory.I:
                    return FailureMechanismSectionAssemblyGroup.I;
                case EInterpretationCategory.Zero:
                    return FailureMechanismSectionAssemblyGroup.Zero;
                case EInterpretationCategory.IMin:
                    return FailureMechanismSectionAssemblyGroup.IMin;
                case EInterpretationCategory.IIMin:
                    return FailureMechanismSectionAssemblyGroup.IIMin;
                case EInterpretationCategory.IIIMin:
                    return FailureMechanismSectionAssemblyGroup.IIIMin;
                case EInterpretationCategory.Dominant:
                    return FailureMechanismSectionAssemblyGroup.Dominant;
                case EInterpretationCategory.Gr:
                    return FailureMechanismSectionAssemblyGroup.Gr;
                case EInterpretationCategory.NotRelevant:
                    return FailureMechanismSectionAssemblyGroup.Nr;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="FailureMechanismSectionAssemblyGroup"/> into an <see cref="EInterpretationCategory"/>.
        /// </summary>
        /// <param name="assemblyGroup">The <see cref="FailureMechanismSectionAssemblyGroup"/> to convert.</param>
        /// <returns>A <see cref="EInterpretationCategory"/> based on <paramref name="assemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static EInterpretationCategory ConvertFrom(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyGroup), assemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyGroup),
                                                       (int) assemblyGroup,
                                                       typeof(FailureMechanismSectionAssemblyGroup));
            }

            switch (assemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.NotDominant:
                    return EInterpretationCategory.NotDominant;
                case FailureMechanismSectionAssemblyGroup.III:
                    return EInterpretationCategory.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return EInterpretationCategory.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return EInterpretationCategory.I;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return EInterpretationCategory.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return EInterpretationCategory.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return EInterpretationCategory.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return EInterpretationCategory.IIIMin;
                case FailureMechanismSectionAssemblyGroup.Dominant:
                    return EInterpretationCategory.Dominant;
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return EInterpretationCategory.Gr;
                case FailureMechanismSectionAssemblyGroup.Nr:
                    return EInterpretationCategory.NotRelevant;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}