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
using Assembly.Kernel.Model;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismAssembly"/> instances and
    /// <see cref="FailureMechanismAssemblyCategoryGroup"/> values.
    /// </summary>
    internal static class FailureMechanismAssemblyCreator
    {
        /// <summary>
        /// Creates <see cref="FailureMechanismAssembly"/> from the given <see cref="FailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="result">The result to create the assembly from.</param>
        /// <returns>The created assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="EFailureMechanismCategory"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="EFailureMechanismCategory"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismAssembly Create(FailureMechanismAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new FailureMechanismAssembly(result.FailureProbability,
                                                CreateFailureMechanismAssemblyCategoryGroup(result.Category));
        }

        /// <summary>
        /// Converts a <see cref="EFailureMechanismCategory"/> into a <see cref="FailureMechanismAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="EFailureMechanismCategory"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismAssemblyCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismAssemblyCategoryGroup CreateFailureMechanismAssemblyCategoryGroup(EFailureMechanismCategory category)
        {
            if (!Enum.IsDefined(typeof(EFailureMechanismCategory), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(EFailureMechanismCategory));
            }

            switch (category)
            {
                case EFailureMechanismCategory.It:
                    return FailureMechanismAssemblyCategoryGroup.It;
                case EFailureMechanismCategory.IIt:
                    return FailureMechanismAssemblyCategoryGroup.IIt;
                case EFailureMechanismCategory.IIIt:
                    return FailureMechanismAssemblyCategoryGroup.IIIt;
                case EFailureMechanismCategory.IVt:
                    return FailureMechanismAssemblyCategoryGroup.IVt;
                case EFailureMechanismCategory.Vt:
                    return FailureMechanismAssemblyCategoryGroup.Vt;
                case EFailureMechanismCategory.VIt:
                    return FailureMechanismAssemblyCategoryGroup.VIt;
                case EFailureMechanismCategory.VIIt:
                    return FailureMechanismAssemblyCategoryGroup.VIIt;
                case EFailureMechanismCategory.Gr:
                    return FailureMechanismAssemblyCategoryGroup.None;
                case EFailureMechanismCategory.Nvt:
                    return FailureMechanismAssemblyCategoryGroup.NotApplicable;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}