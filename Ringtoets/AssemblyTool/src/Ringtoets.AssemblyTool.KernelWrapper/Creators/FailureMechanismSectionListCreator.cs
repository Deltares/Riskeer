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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismSectionList"/> instances.
    /// </summary>
    internal static class FailureMechanismSectionListCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="FailureMechanismSectionList"/> based on the
        /// given <paramref name="failureMechanisms"/>.
        /// </summary>
        /// <param name="failureMechanisms">The failure mechanisms to create the failure mechanism
        /// section lists for.</param>
        /// <returns>A collection of <see cref="FailureMechanismSectionList"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<FailureMechanismSectionList> Create(IEnumerable<CombinedAssemblyFailureMechanismInput> failureMechanisms)
        {
            if (failureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(failureMechanisms));
            }

            return failureMechanisms.Select(fm => new FailureMechanismSectionList(
                                                new FailureMechanism(
                                                    fm.N, fm.FailureMechanismContribution),
                                                fm.Sections.Select(s => new FmSectionWithDirectCategory(
                                                                       s.SectionStart, s.SectionEnd,
                                                                       ConvertCategoryGroup(s.CategoryGroup)))))
                                    .ToArray();
        }

        private static EFmSectionCategory ConvertCategoryGroup(FailureMechanismSectionAssemblyCategoryGroup category)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (category)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return EFmSectionCategory.Gr;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return EFmSectionCategory.NotApplicable;
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
                default:
                    throw new NotSupportedException();
            }
        }
    }
}