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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Model;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="FailureMechanismSectionList"/> instances.
    /// </summary>
    internal static class FailureMechanismSectionListCreator
    {
        /// <summary>
        /// Creates a collection of <see cref="FailureMechanismSectionList"/> based on the
        /// given <paramref name="failureMechanismSectionsCollection"/>.
        /// </summary>
        /// <param name="failureMechanismSectionsCollection">The collection of failure mechanism
        /// section collections to create the failure mechanism section lists for.</param>
        /// <returns>A collection of <see cref="FailureMechanismSectionList"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionsCollection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static IEnumerable<FailureMechanismSectionList> Create(
            IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> failureMechanismSectionsCollection)
        {
            if (failureMechanismSectionsCollection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionsCollection));
            }

            return failureMechanismSectionsCollection.Select(sectionCollection => new FailureMechanismSectionList(
                                                                 string.Empty,
                                                                 sectionCollection.Select(s => new FmSectionWithDirectCategory(
                                                                                              s.SectionStart, s.SectionEnd,
                                                                                              AssemblyCalculatorInputCreator.CreateFailureMechanismSectionCategory(
                                                                                                  s.CategoryGroup)))))
                                                     .ToArray();
        }
    }
}