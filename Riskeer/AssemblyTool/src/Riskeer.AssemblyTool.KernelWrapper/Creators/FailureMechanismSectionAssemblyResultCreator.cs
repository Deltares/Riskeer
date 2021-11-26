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
using Assembly.Kernel.Model.FailurePathSections;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismSectionAssemblyResult"/> instances and
    /// <see cref="FailureMechanismSectionAssemblyGroup"/> values.
    /// </summary>
    internal static class FailureMechanismSectionAssemblyResultCreator
    {
        /// <summary>
        /// Converts a <see cref="FailurePathSectionAssemblyResult"/> into a <see cref="FailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="FailurePathSectionAssemblyResult"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/> based on <paramref name="result"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="EInterpretationCategory"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="EInterpretationCategory"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyResult CreateFailureMechanismSectionAssemblyResult(FailurePathSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new FailureMechanismSectionAssemblyResult(result.ProbabilityProfile.Value,
                                                             result.ProbabilitySection.Value,
                                                             result.NSection,
                                                             CreateFailureMechanismSectionAssemblyGroup(result.InterpretationCategory));
        }

        /// <summary>
        /// Converts a <see cref="EInterpretationCategory"/> into a <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="EInterpretationCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyGroup CreateFailureMechanismSectionAssemblyGroup(EInterpretationCategory category)
        {
            if (!Enum.IsDefined(typeof(EInterpretationCategory), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(EInterpretationCategory));
            }

            switch (category)
            {
                case EInterpretationCategory.ND:
                    return FailureMechanismSectionAssemblyGroup.ND;
                case EInterpretationCategory.III:
                    return FailureMechanismSectionAssemblyGroup.III;
                case EInterpretationCategory.II:
                    return FailureMechanismSectionAssemblyGroup.II;
                case EInterpretationCategory.I:
                    return FailureMechanismSectionAssemblyGroup.I;
                case EInterpretationCategory.ZeroPlus:
                    return FailureMechanismSectionAssemblyGroup.ZeroPlus;
                case EInterpretationCategory.Zero:
                    return FailureMechanismSectionAssemblyGroup.Zero;
                case EInterpretationCategory.IMin:
                    return FailureMechanismSectionAssemblyGroup.IMin;
                case EInterpretationCategory.IIMin:
                    return FailureMechanismSectionAssemblyGroup.IIMin;
                case EInterpretationCategory.IIIMin:
                    return FailureMechanismSectionAssemblyGroup.IIIMin;
                case EInterpretationCategory.D:
                    return FailureMechanismSectionAssemblyGroup.D;
                case EInterpretationCategory.Gr:
                    return FailureMechanismSectionAssemblyGroup.Gr;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}