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
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in the assembly kernel.
    /// </summary>
    internal static class AssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates a <see cref="EFmSectionCategory"/> from a <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> to create
        /// an <see cref="EFmSectionCategory"/> for.</param>
        /// <returns>An <see cref="EFmSectionCategory"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static EFmSectionCategory CreateFailureMechanismSectionCategory(
            FailureMechanismSectionAssemblyCategoryGroup category)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (category)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return EFmSectionCategory.Iv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return EFmSectionCategory.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return EFmSectionCategory.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return EFmSectionCategory.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return EFmSectionCategory.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return EFmSectionCategory.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return EFmSectionCategory.VIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return EFmSectionCategory.NotApplicable;
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return EFmSectionCategory.Gr;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}