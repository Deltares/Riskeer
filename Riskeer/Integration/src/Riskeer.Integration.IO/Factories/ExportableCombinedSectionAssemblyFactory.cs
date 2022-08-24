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
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

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
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="combinedSectionAssemblyResults">A collection of combined section results to
        /// create a collection of <see cref="ExportableCombinedSectionAssembly"/> for.</param>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> the section results belong to.</param>
        /// <returns>A collection of <see cref="ExportableCombinedSectionAssembly"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <see cref="CombinedFailureMechanismSectionAssemblyResult.TotalResult"/>
        /// is invalid and cannot be exported.</exception>
        public static IEnumerable<ExportableCombinedSectionAssembly> CreateExportableCombinedSectionAssemblyCollection(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry,
            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> combinedSectionAssemblyResults,
            AssessmentSection assessmentSection)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

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
                if (assemblyResult.TotalResult == FailureMechanismSectionAssemblyGroup.NoResult
                    || assemblyResult.TotalResult == FailureMechanismSectionAssemblyGroup.Dominant)
                {
                    throw new AssemblyFactoryException("The assembly result is invalid and cannot be created.");
                }

                ExportableCombinedFailureMechanismSection exportableSection = registry.Get(assemblyResult);

                var exportableSectionResult = new ExportableCombinedSectionAssembly(
                    idGenerator.GetNewId(Resources.ExportableCombinedSectionAssembly_IdPrefix), exportableSection, assemblyResult.TotalResult,
                    ExportableAssemblyMethodConverter.ConvertTo(assemblyResult.CombinedSectionResultAssemblyMethod),
                    CreateFailureMechanismCombinedSectionAssemblyResults(assemblyResult, assessmentSection));

                sectionResults.Add(exportableSectionResult);
            }

            return sectionResults;
        }

        private static IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> CreateFailureMechanismCombinedSectionAssemblyResults(
            CombinedFailureMechanismSectionAssemblyResult assemblyResult, AssessmentSection assessmentSection)
        {
            Tuple<FailureMechanismSectionAssemblyGroup?, string, string>[] failureMechanisms =
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
                                 .Select(fm => CreateExportableFailureMechanismCombinedSectionAssemblyResult(
                                             fm.Item1.Value, assemblyResult.FailureMechanismResultsAssemblyMethod))
                                 .ToList();

            for (var i = 0; i < assessmentSection.SpecificFailureMechanisms.Count; i++)
            {
                FailureMechanismSectionAssemblyGroup? specificFailureMechanismAssemblyResult = assemblyResult.SpecificFailureMechanisms[i];

                if (specificFailureMechanismAssemblyResult.HasValue)
                {
                    SpecificFailureMechanism specificFailureMechanism = assessmentSection.SpecificFailureMechanisms.ElementAt(i);
                    exportableAssemblyResults.Add(CreateExportableFailureMechanismCombinedSectionAssemblyResult(
                                                      specificFailureMechanismAssemblyResult.Value, assemblyResult.FailureMechanismResultsAssemblyMethod));
                }
            }

            return exportableAssemblyResults;
        }

        private static Tuple<FailureMechanismSectionAssemblyGroup?, string, string> CreateTuple(
            FailureMechanismSectionAssemblyGroup? assemblyResultGroup, IFailureMechanism failureMechanism)
        {
            return new Tuple<FailureMechanismSectionAssemblyGroup?, string, string>(assemblyResultGroup, failureMechanism.Code, failureMechanism.Name);
        }

        private static ExportableFailureMechanismCombinedSectionAssemblyResult CreateExportableFailureMechanismCombinedSectionAssemblyResult(
            FailureMechanismSectionAssemblyGroup sectionAssemblyGroup, AssemblyMethod assemblyMethod)
        {
            return new ExportableFailureMechanismCombinedSectionAssemblyResult(
                sectionAssemblyGroup, ExportableAssemblyMethodConverter.ConvertTo(assemblyMethod),
                new ExportableFailureMechanismSectionAssemblyResult(
                    "id", new ExportableFailureMechanismSection("id", Array.Empty<Point2D>(), double.NaN, double.NaN), double.NaN,
                    FailureMechanismSectionAssemblyGroup.Zero, ExportableAssemblyMethod.Manual, ExportableAssemblyMethod.Manual));
        }
    }
}