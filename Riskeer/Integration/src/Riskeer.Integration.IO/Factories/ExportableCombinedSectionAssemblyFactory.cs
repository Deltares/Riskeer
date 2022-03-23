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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Integration.Data;
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
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> the section results belong to.</param>
        /// <returns>A collection of <see cref="ExportableCombinedSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<ExportableCombinedSectionAssembly> CreateExportableCombinedSectionAssemblyCollection(
            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> combinedSectionAssemblyResults,
            AssessmentSection assessmentSection)
        {
            if (combinedSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblyResults));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            var sectionResults = new List<ExportableCombinedSectionAssembly>();
            foreach (CombinedFailureMechanismSectionAssemblyResult assemblyResult in combinedSectionAssemblyResults)
            {
                var exportableSection = new ExportableCombinedFailureMechanismSection(
                    FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(
                        assessmentSection.ReferenceLine, assemblyResult.SectionStart, assemblyResult.SectionEnd),
                    assemblyResult.SectionStart,
                    assemblyResult.SectionEnd,
                    ExportableAssemblyMethod.WBI3A1);

                var exportableSectionResult = new ExportableCombinedSectionAssembly(
                    exportableSection, new ExportableFailureMechanismSectionAssemblyResult(
                        exportableSection, assemblyResult.TotalResult, ExportableAssemblyMethod.WBI3C1),
                    CreateFailureMechanismCombinedSectionAssemblyResults(assemblyResult, assessmentSection));

                sectionResults.Add(exportableSectionResult);
            }

            return sectionResults;
        }

        private static IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> CreateFailureMechanismCombinedSectionAssemblyResults(
            CombinedFailureMechanismSectionAssemblyResult assemblyResult, AssessmentSection assessmentSection)
        {
            Tuple<FailureMechanismSectionAssemblyGroup?, string>[] failureMechanisms =
            {
                CreateTuple(assemblyResult.Piping, assessmentSection.Piping),
                CreateTuple(assemblyResult.GrassCoverErosionInwards, assessmentSection.GrassCoverErosionInwards),
                CreateTuple(assemblyResult.MacroStabilityInwards, assessmentSection.MacroStabilityInwards),
                CreateTuple(assemblyResult.Microstability, assessmentSection.Microstability),
                CreateTuple(assemblyResult.StabilityStoneCover, assessmentSection.StabilityStoneCover),
                CreateTuple(assemblyResult.WaveImpactAsphaltCover, assessmentSection.WaveImpactAsphaltCover),
                CreateTuple(assemblyResult.WaterPressureAsphaltCover, assessmentSection.WaterPressureAsphaltCover),
                CreateTuple(assemblyResult.GrassCoverErosionOutwards, assessmentSection.GrassCoverErosionOutwards),
                CreateTuple(assemblyResult.GrassCoverSlipOffOutwards, assessmentSection.GrassCoverSlipOffOutwards),
                CreateTuple(assemblyResult.GrassCoverSlipOffInwards, assessmentSection.GrassCoverSlipOffInwards),
                CreateTuple(assemblyResult.HeightStructures, assessmentSection.HeightStructures),
                CreateTuple(assemblyResult.ClosingStructures, assessmentSection.ClosingStructures),
                CreateTuple(assemblyResult.PipingStructure, assessmentSection.PipingStructure),
                CreateTuple(assemblyResult.StabilityPointStructures, assessmentSection.StabilityPointStructures),
                CreateTuple(assemblyResult.DuneErosion, assessmentSection.DuneErosion)
            };

            List<ExportableFailureMechanismCombinedSectionAssemblyResult> exportableAssemblyResults =
                failureMechanisms.Where(fm => fm.Item1.HasValue)
                                 .Select(fm => CreateExportableFailureMechanismCombinedSectionAssemblyResult(fm.Item1.Value, ExportableFailureMechanismType.Generic, fm.Item2))
                                 .ToList();

            for (var i = 0; i < assessmentSection.SpecificFailurePaths.Count; i++)
            {
                FailureMechanismSectionAssemblyGroup? specificFailurePathAssemblyResult = assemblyResult.SpecificFailurePaths[i];

                if (specificFailurePathAssemblyResult.HasValue)
                {
                    SpecificFailurePath specificFailurePath = assessmentSection.SpecificFailurePaths.ElementAt(i);
                    exportableAssemblyResults.Add(CreateExportableFailureMechanismCombinedSectionAssemblyResult(
                                                      specificFailurePathAssemblyResult.Value, ExportableFailureMechanismType.Specific,
                                                      specificFailurePath.Code));
                }
            }

            return exportableAssemblyResults;
        }

        private static Tuple<FailureMechanismSectionAssemblyGroup?, string> CreateTuple(
            FailureMechanismSectionAssemblyGroup? assemblyResultGroup, IFailureMechanism failureMechanism)
        {
            return new Tuple<FailureMechanismSectionAssemblyGroup?, string>(assemblyResultGroup, failureMechanism.Code);
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateExportableFailureMechanismCombinedSectionAssemblyResult(
            FailureMechanismSectionAssemblyGroup sectionAssemblyGroup, ExportableFailureMechanismType failureMechanismType,
            string failureMechanismCode)
        {
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(
                new ExportableFailureMechanismSubSectionAssemblyResult(sectionAssemblyGroup, ExportableAssemblyMethod.WBI3B1),
                failureMechanismType, failureMechanismCode);
        }
    }
}