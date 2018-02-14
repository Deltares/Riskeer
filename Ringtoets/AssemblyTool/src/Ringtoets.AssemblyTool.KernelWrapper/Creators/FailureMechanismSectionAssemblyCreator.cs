// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismSectionAssembly"/> instances.
    /// </summary>
    public static class FailureMechanismSectionAssemblyCreator
    {
        /// <summary>
        /// Creates <see cref="FailureMechanismSectionAssembly"/> from the given <see cref="FailureMechanismSectionAssemblyCategoryResult"/>.
        /// </summary>
        /// <param name="result">The result to create the assembly from.</param>
        /// <returns>The created assembly.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionAssembly Create(FailureMechanismSectionAssemblyCategoryResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            FailureMechanismSectionAssemblyCategoryGroup group;
            FailureMechanismSectionCategoryGroup originalGroup = result.CategoryGroup;

            if (!Enum.IsDefined(typeof(FailureMechanismSectionCategoryGroup), originalGroup))
            {
                throw new InvalidEnumArgumentException(nameof(originalGroup),
                                                       (int)originalGroup,
                                                       typeof(FailureMechanismSectionCategoryGroup));
            }

            switch (originalGroup)
            {
                case FailureMechanismSectionCategoryGroup.Iv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.Iv;
                    break;
                case FailureMechanismSectionCategoryGroup.IIv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.IIv;
                    break;
                case FailureMechanismSectionCategoryGroup.IIIv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                    break;
                case FailureMechanismSectionCategoryGroup.IVv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.IVv;
                    break;
                case FailureMechanismSectionCategoryGroup.Vv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.Vv;
                    break;
                case FailureMechanismSectionCategoryGroup.VIv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.VIv;
                    break;
                case FailureMechanismSectionCategoryGroup.VIIv:
                    group = FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                    break;
                case FailureMechanismSectionCategoryGroup.None:
                    group = FailureMechanismSectionAssemblyCategoryGroup.None;
                    break;
                case FailureMechanismSectionCategoryGroup.NotApplicable:
                    group = FailureMechanismSectionAssemblyCategoryGroup.NotApplicable;
                    break;
                default:
                    throw new NotSupportedException();
            }

            return new FailureMechanismSectionAssembly(result.EstimatedProbabilityOfFailure.Value, group);
        }
    }
}