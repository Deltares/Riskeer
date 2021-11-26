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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailurePathSections;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in the failure path assembly calculator.
    /// </summary>
    internal static class FailurePathAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates a <see cref="FailurePathSectionAssemblyResult"/> based on <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="FailureMechanismSectionAssemblyResult"/> to create the
        /// <see cref="FailurePathSectionAssemblyResult"/> with.</param>
        /// <returns>A <see cref="FailurePathSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static FailurePathSectionAssemblyResult CreateFailurePathSectionAssemblyResult(FailureMechanismSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new FailurePathSectionAssemblyResult(new Probability(result.ProfileProbability),
                                                        new Probability(result.SectionProbability),
                                                        CreateInterpretationCategory(result.AssemblyGroup));
        }

        /// <summary>
        /// Converts a <see cref="EInterpretationCategory"/> into a <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="assemblyGroup">The <see cref="FailureMechanismSectionAssemblyGroup"/> to convert.</param>
        /// <returns>A <see cref="EInterpretationCategory"/> based on <paramref name="assemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        private static EInterpretationCategory CreateInterpretationCategory(FailureMechanismSectionAssemblyGroup assemblyGroup)
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
                    return EInterpretationCategory.ND;
                case FailureMechanismSectionAssemblyGroup.III:
                    return EInterpretationCategory.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return EInterpretationCategory.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return EInterpretationCategory.I;
                case FailureMechanismSectionAssemblyGroup.ZeroPlus:
                    return EInterpretationCategory.ZeroPlus;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return EInterpretationCategory.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return EInterpretationCategory.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return EInterpretationCategory.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return EInterpretationCategory.IIIMin;
                case FailureMechanismSectionAssemblyGroup.D:
                    return EInterpretationCategory.D;
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return EInterpretationCategory.Gr;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}