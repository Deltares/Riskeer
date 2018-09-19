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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableAssessmentSection"/>
    /// with assembly results.
    /// </summary>
    public static class ExportableAssessmentSectionFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableAssessmentSection"/> with assembly results
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to create
        /// an <see cref="ExportableAssessmentSection"/> for.</param>
        /// <returns>An <see cref="ExportableAssessmentSection"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when no reference line is set for <paramref name="assessmentSection"/>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        public static ExportableAssessmentSection CreateExportableAssessmentSection(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (assessmentSection.ReferenceLine == null)
            {
                throw new ArgumentException("reference line of assessment section cannot be null.");
            }

            return new ExportableAssessmentSection(assessmentSection.Name,
                                                   assessmentSection.Id,
                                                   assessmentSection.ReferenceLine.Points,
                                                   CreateExportableAssessmentSectionAssemblyResult(assessmentSection),
                                                   CreateExportableFailureMechanismAssemblyResultWithProbability(assessmentSection),
                                                   CreateExportableFailureMechanismAssemblyResultWithoutProbability(assessmentSection),
                                                   CreateExportableFailureMechanismsWithProbability(assessmentSection),
                                                   CreateExportableFailureMechanismsWithoutProbability(assessmentSection),
                                                   CreateExportableCombinedSectionAssemblyCollection(assessmentSection));
        }

        /// <summary>
        /// Creates an <see cref="ExportableAssessmentSectionAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create an <see cref="ExportableAssessmentSectionAssemblyResult"/> for.</param>
        /// <returns>An <see cref="ExportableAssessmentSectionAssemblyResult"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableAssessmentSectionAssemblyResult CreateExportableAssessmentSectionAssemblyResult(AssessmentSection assessmentSection)
        {
            return new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI2C1,
                                                                 AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection, false));
        }

        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create an <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> for.</param>
        /// <returns>An <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableFailureMechanismAssemblyResultWithProbability CreateExportableFailureMechanismAssemblyResultWithProbability(AssessmentSection assessmentSection)
        {
            FailureMechanismAssembly assemblyResult = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection, false);
            return new ExportableFailureMechanismAssemblyResultWithProbability(ExportableAssemblyMethod.WBI2B1,
                                                                               assemblyResult.Group,
                                                                               assemblyResult.Probability);
        }

        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanismAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create an <see cref="ExportableFailureMechanismAssemblyResult"/> for.</param>
        /// <returns>An <see cref="ExportableFailureMechanismAssemblyResult"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableFailureMechanismAssemblyResult CreateExportableFailureMechanismAssemblyResultWithoutProbability(AssessmentSection assessmentSection)
        {
            return new ExportableFailureMechanismAssemblyResult(ExportableAssemblyMethod.WBI2A1,
                                                                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection, true));
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// for failure mechanisms with an assembly result with a probability based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with probability for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> based on failure
        /// mechanisms with assembly results with a probability.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created  for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> CreateExportableFailureMechanismsWithProbability(AssessmentSection assessmentSection)
        {
            return new[]
            {
                ExportablePipingFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.Piping, assessmentSection),
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection),
                ExportableGrassCoverErosionInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                ExportableHeightStructuresFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.HeightStructures, assessmentSection),
                ExportableClosingStructuresFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.ClosingStructures, assessmentSection),
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection)
            };
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// for failure mechanisms with an assembly result without a probability based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with probability for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> based on failure
        /// mechanisms with assembly results without a probability.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> CreateExportableFailureMechanismsWithoutProbability(AssessmentSection assessmentSection)
        {
            return new[]
            {
                ExportableStabilityStoneCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StabilityStoneCover, assessmentSection),
                ExportableWaveImpactAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                ExportableGrassCoverErosionOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                ExportableDuneErosionFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.DuneErosion, assessmentSection),
                ExportableMacroStabilityOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.MacroStabilityOutwards, assessmentSection),
                ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.Microstability, assessmentSection),
                ExportableGrassCoverSlipOffOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, assessmentSection),
                ExportableGrassCoverSlipOffInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, assessmentSection),
                ExportablePipingStructureFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.PipingStructure, assessmentSection),
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.WaterPressureAsphaltCover, assessmentSection),
                ExportableStrengthStabilityLengthwiseConstructionFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction, assessmentSection),
                ExportableTechnicalInnovationFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.TechnicalInnovation, assessmentSection)
            };
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableCombinedSectionAssembly"/> based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of <see cref="ExportableCombinedSectionAssembly"/> for.</param>
        /// <returns>A <see cref="CreateExportableCombinedSectionAssemblyCollection"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableCombinedSectionAssembly> CreateExportableCombinedSectionAssemblyCollection(AssessmentSection assessmentSection)
        {
            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, false);
            return ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(assemblyResults, assessmentSection.ReferenceLine);
        }
    }
}