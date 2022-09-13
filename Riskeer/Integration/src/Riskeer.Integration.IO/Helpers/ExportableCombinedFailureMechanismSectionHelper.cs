// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Exceptions;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Helper class for the <see cref="ExportableCombinedFailureMechanismSection"/>.
    /// </summary>
    public static class ExportableCombinedFailureMechanismSectionHelper
    {
        private const double tolerance = 1e-6;

        /// <summary>
        /// Gets the matching <see cref="ExportableFailureMechanismAssemblyResult"/> that contains a section corresponding with
        /// the <paramref name="exportableCombinedFailureMechanismSection"/>.
        /// </summary>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> that keeps track of the created
        /// <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="sectionResults">The collection of <see cref="FailureMechanismSectionResult"/>.</param>
        /// <param name="exportableCombinedFailureMechanismSection">The <see cref="ExportableCombinedFailureMechanismSection"/> to
        /// find the corresponding <see cref="ExportableFailureMechanismAssemblyResult"/> for.</param>
        /// <returns>The <see cref="ExportableFailureMechanismSectionAssemblyResult"/> with a section that corresponds with
        /// <paramref name="exportableCombinedFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when no matching <see cref="ExportableFailureMechanismSectionAssemblyResult"/>
        /// could be found.</exception>
        public static ExportableFailureMechanismSectionAssemblyResult GetExportableFailureMechanismSectionAssemblyResult(
            ExportableModelRegistry registry, IEnumerable<FailureMechanismSectionResult> sectionResults,
            ExportableCombinedFailureMechanismSection exportableCombinedFailureMechanismSection)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (sectionResults == null)
            {
                throw new ArgumentNullException(nameof(sectionResults));
            }

            if (exportableCombinedFailureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(exportableCombinedFailureMechanismSection));
            }

            foreach (FailureMechanismSectionResult sectionResult in sectionResults)
            {
                ExportableFailureMechanismSectionAssemblyResult exportableSectionResult = registry.Get(sectionResult);
                ExportableFailureMechanismSection exportableFailureMechanismSection = exportableSectionResult.FailureMechanismSection;

                if (IsMatchingSection(exportableFailureMechanismSection, exportableCombinedFailureMechanismSection))
                {
                    return exportableSectionResult;
                }
            }

            throw new AssemblyFactoryException($"No matching {typeof(ExportableFailureMechanismSectionAssemblyResult)} was found for the {nameof(exportableCombinedFailureMechanismSection)}.");
        }

        private static bool IsMatchingSection(ExportableFailureMechanismSection exportableFailureMechanismSection,
                                              ExportableFailureMechanismSection exportableCombinedFailureMechanismSection)
        {
            return exportableCombinedFailureMechanismSection.StartDistance >= exportableFailureMechanismSection.StartDistance - tolerance
                   && exportableCombinedFailureMechanismSection.EndDistance <= exportableFailureMechanismSection.EndDistance + tolerance;
        }
    }
}