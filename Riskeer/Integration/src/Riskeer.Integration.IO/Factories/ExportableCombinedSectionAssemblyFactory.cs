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
using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.Util;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableCombinedSectionAssembly"/>
    /// </summary>
    public static class ExportableCombinedSectionAssemblyFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="ExportableCombinedSectionAssembly"/>
        /// based on <paramref name="combinedSectionAssemblyResults"/>.
        /// </summary>
        /// <param name="combinedSectionAssemblyResults">A collection of combined section results to
        /// create a collection of <see cref="ExportableCombinedSectionAssembly"/> for.</param>
        /// <param name="referenceLine">The reference line to map the sections to.</param>
        /// <returns>A collection of <see cref="ExportableCombinedSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<ExportableCombinedSectionAssembly> CreateExportableCombinedSectionAssemblyCollection(
            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> combinedSectionAssemblyResults,
            ReferenceLine referenceLine)
        {
            if (combinedSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblyResults));
            }

            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            var sectionResults = new List<ExportableCombinedSectionAssembly>();
            foreach (CombinedFailureMechanismSectionAssemblyResult assemblyResult in combinedSectionAssemblyResults)
            {
                var exportableSection = new ExportableCombinedFailureMechanismSection(
                    FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                        referenceLine, assemblyResult.SectionStart, assemblyResult.SectionEnd),
                    assemblyResult.SectionStart,
                    assemblyResult.SectionEnd,
                    ExportableAssemblyMethod.WBI3A1);

                var exportableSectionResult = new ExportableCombinedSectionAssembly(
                    exportableSection, new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI3C1, FailureMechanismSectionAssemblyCategoryGroup.None),
                    CreateFailureMechanismCombinedSectionAssemblyResults());

                sectionResults.Add(exportableSectionResult);
            }

            return sectionResults;
        }

        private static IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> CreateFailureMechanismCombinedSectionAssemblyResults()
        {
            return new[]
            {
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.STPH),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.GEKB),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.STBI),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.STMI),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.ZST),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.AGK),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.AWO),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.GEBU),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.GABU),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.GABI),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.HTKW),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.BSKW),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.PKW),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.STKWp),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup.None, ExportableFailureMechanismType.DA)
            };
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateExportableFailureMechanismCombinedSectionAssemblyResult(
            FailureMechanismSectionAssemblyCategoryGroup sectionAssemblyResult,
            ExportableFailureMechanismType failureMechanismCode)
        {
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(
                new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI3B1, sectionAssemblyResult),
                failureMechanismCode);
        }
    }
}