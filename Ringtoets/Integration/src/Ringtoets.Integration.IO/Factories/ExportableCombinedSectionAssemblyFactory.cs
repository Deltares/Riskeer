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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;

namespace Ringtoets.Integration.IO.Factories
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
                    ExportableFailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                        referenceLine, assemblyResult.SectionStart, assemblyResult.SectionEnd),
                    assemblyResult.SectionStart,
                    assemblyResult.SectionEnd,
                    ExportableAssemblyMethod.WBI3A1);

                var exportableSectionResult = new ExportableCombinedSectionAssembly(exportableSection,
                                                                                    new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI3C1,
                                                                                                                        assemblyResult.TotalResult),
                                                                                    CreateFailureMechanismCombinedSectionAssemblyResults(assemblyResult));

                sectionResults.Add(exportableSectionResult);
            }

            return sectionResults;
        }

        private static IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> CreateFailureMechanismCombinedSectionAssemblyResults(
            CombinedFailureMechanismSectionAssemblyResult assemblyResult)
        {
            return new[]
            {
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.Piping, ExportableFailureMechanismType.STPH),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.GrassCoverErosionInwards, ExportableFailureMechanismType.GEKB),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.MacroStabilityInwards, ExportableFailureMechanismType.STBI),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.MacroStabilityOutwards, ExportableFailureMechanismType.STBU),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.Microstability, ExportableFailureMechanismType.STMI),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.StabilityStoneCover, ExportableFailureMechanismType.ZST),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.WaveImpactAsphaltCover, ExportableFailureMechanismType.AGK),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.WaterPressureAsphaltCover, ExportableFailureMechanismType.AWO),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.GrassCoverErosionOutwards, ExportableFailureMechanismType.GEBU),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.GrassCoverSlipOffOutwards, ExportableFailureMechanismType.GABU),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.GrassCoverSlipOffInwards, ExportableFailureMechanismType.GABI),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.HeightStructures, ExportableFailureMechanismType.HTKW),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.ClosingStructures, ExportableFailureMechanismType.BSKW),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.PipingStructure, ExportableFailureMechanismType.PKW),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.StabilityPointStructures, ExportableFailureMechanismType.STKWp),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.StrengthStabilityLengthwiseConstruction, ExportableFailureMechanismType.STKWl),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.DuneErosion, ExportableFailureMechanismType.DA),
                CreateExportableFailureMechanismCombinedSectionAssemblyResult(assemblyResult.TechnicalInnovation, ExportableFailureMechanismType.INN)
            };
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateExportableFailureMechanismCombinedSectionAssemblyResult(
            FailureMechanismSectionAssemblyCategoryGroup sectionAssemblyResult,
            ExportableFailureMechanismType failureMechanismCode)
        {
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(new ExportableSectionAssemblyResult(ExportableAssemblyMethod.WBI3B1,
                                                                                                                   sectionAssemblyResult),
                                                                               failureMechanismCode);
        }
    }
}