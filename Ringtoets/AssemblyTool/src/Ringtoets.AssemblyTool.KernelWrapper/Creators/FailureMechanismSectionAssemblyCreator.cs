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
using Assembly.Kernel.Model.FmSectionTypes;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismSectionAssembly"/> instances and
    /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> values.
    /// </summary>
    internal static class FailureMechanismSectionAssemblyCreator
    {
        /// <summary>
        /// Creates <see cref="FailureMechanismSectionAssembly"/> from the given <see cref="FmSectionAssemblyDirectResult"/>.
        /// </summary>
        /// <param name="result">The result to create the assembly from.</param>
        /// <returns>The created assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="EFmSectionCategory"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="EFmSectionCategory"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssembly Create(FmSectionAssemblyDirectResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new FailureMechanismSectionAssembly(double.NaN,
                                                       CreateFailureMechanismSectionAssemblyCategoryGroup(result.Result));
        }

        /// <summary>
        /// Creates <see cref="FailureMechanismSectionAssembly"/> from the given <see cref="FmSectionAssemblyDirectResultWithProbability"/>.
        /// </summary>
        /// <param name="result">The result to create the assembly from.</param>
        /// <returns>The created assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="EFmSectionCategory"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="EFmSectionCategory"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssembly Create(FmSectionAssemblyDirectResultWithProbability result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new FailureMechanismSectionAssembly(result.FailureProbability,
                                                       CreateFailureMechanismSectionAssemblyCategoryGroup(result.Result));
        }

        /// <summary>
        /// Converts a <see cref="EFmSectionCategory"/> into a <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="EFmSectionCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssemblyCategoryGroup CreateFailureMechanismSectionAssemblyCategoryGroup(EFmSectionCategory category)
        {
            if (!Enum.IsDefined(typeof(EFmSectionCategory), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(EFmSectionCategory));
            }

            switch (category)
            {
                case EFmSectionCategory.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case EFmSectionCategory.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case EFmSectionCategory.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case EFmSectionCategory.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case EFmSectionCategory.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case EFmSectionCategory.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case EFmSectionCategory.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case EFmSectionCategory.Gr:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                case EFmSectionCategory.NotApplicable:
                    return FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}