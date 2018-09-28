// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances to be used in the <see cref="IAssessmentGradeAssembler"/>.
    /// </summary>
    internal static class AssessmentSectionAssemblyInputCreator
    {
        /// <summary>
        /// Creates <see cref="FailureMechanismAssemblyResult"/> based on the given parameters.
        /// </summary>
        /// <param name="input">The assembly to create a <see cref="FailureMechanismAssemblyResult"/> for.</param>
        /// <returns>The created <see cref="FailureMechanismAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="input"/> contains
        /// an invalid <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="input"/> contains
        /// a valid but unsupported <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="AssemblyException">Thrown when <paramref name="input"/> has an
        /// invalid value.</exception>
        public static FailureMechanismAssemblyResult CreateFailureMechanismAssemblyResult(FailureMechanismAssembly input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return new FailureMechanismAssemblyResult(CreateFailureMechanismCategory(input.Group),
                                                      input.Probability);
        }

        /// <summary>
        /// Creates <see cref="FailureMechanismAssemblyResult"/> based on the given parameters.
        /// </summary>
        /// <param name="input">The assembly to create a <see cref="FailureMechanismAssemblyResult"/> for.</param>
        /// <returns>The created <see cref="FailureMechanismAssemblyResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="input"/> contains
        /// an invalid <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="input"/> contains
        /// a valid but unsupported <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        public static FailureMechanismAssemblyResult CreateFailureMechanismAssemblyResult(FailureMechanismAssemblyCategoryGroup input)
        {
            return new FailureMechanismAssemblyResult(CreateFailureMechanismCategory(input), double.NaN);
        }

        /// <summary>
        /// Creates a <see cref="EFailureMechanismCategory"/> based on the <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="FailureMechanismAssemblyCategoryGroup"/>
        /// to create a <see cref="EFailureMechanismCategory"/> for.</param>
        /// <returns>The created <see cref="EFailureMechanismCategory"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="input"/> contains
        /// an invalid <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="input"/> contains
        /// a valid but unsupported <see cref="FailureMechanismAssemblyCategoryGroup"/>.</exception>
        public static EFailureMechanismCategory CreateFailureMechanismCategory(FailureMechanismAssemblyCategoryGroup input)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismAssemblyCategoryGroup), input))
            {
                throw new InvalidEnumArgumentException(nameof(input),
                                                       (int) input,
                                                       typeof(FailureMechanismAssemblyCategoryGroup));
            }

            switch (input)
            {
                case FailureMechanismAssemblyCategoryGroup.None:
                    return EFailureMechanismCategory.Gr;
                case FailureMechanismAssemblyCategoryGroup.NotApplicable:
                    return EFailureMechanismCategory.Nvt;
                case FailureMechanismAssemblyCategoryGroup.It:
                    return EFailureMechanismCategory.It;
                case FailureMechanismAssemblyCategoryGroup.IIt:
                    return EFailureMechanismCategory.IIt;
                case FailureMechanismAssemblyCategoryGroup.IIIt:
                    return EFailureMechanismCategory.IIIt;
                case FailureMechanismAssemblyCategoryGroup.IVt:
                    return EFailureMechanismCategory.IVt;
                case FailureMechanismAssemblyCategoryGroup.Vt:
                    return EFailureMechanismCategory.Vt;
                case FailureMechanismAssemblyCategoryGroup.VIt:
                    return EFailureMechanismCategory.VIt;
                case FailureMechanismAssemblyCategoryGroup.VIIt:
                    return EFailureMechanismCategory.VIIt;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}