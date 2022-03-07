﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Exceptions;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Factories
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

            return new ExportableAssessmentSection(assessmentSection.Name,
                                                   assessmentSection.Id,
                                                   assessmentSection.ReferenceLine.Points,
                                                   CreateExportableAssessmentSectionAssemblyResult(assessmentSection),
                                                   CreateExportableFailureMechanisms(assessmentSection),
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
            AssessmentSectionAssemblyResult assemblyResult = AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection);
            return new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI2C1, assemblyResult.AssemblyGroup,
                                                                 assemblyResult.Probability);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism"/>
        /// for failure mechanisms with an assembly result without a probability based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism"/> with probability for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanism"/> based on failure
        /// mechanisms with assembly results without a probability.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableFailureMechanism> CreateExportableFailureMechanisms(AssessmentSection assessmentSection)
        {
            return new[]
            {
                ExportablePipingFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.Piping, assessmentSection),
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection),
                ExportableGrassCoverErosionInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                ExportableHeightStructuresFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.HeightStructures, assessmentSection),
                ExportableClosingStructuresFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.ClosingStructures, assessmentSection),
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection),
                ExportableStabilityStoneCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StabilityStoneCover, assessmentSection),
                ExportableWaveImpactAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.WaveImpactAsphaltCover, assessmentSection),
                ExportableGrassCoverErosionOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverErosionOutwards, assessmentSection),
                ExportableDuneErosionFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.DuneErosion, assessmentSection),
                ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.Microstability, assessmentSection),
                ExportableGrassCoverSlipOffOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, assessmentSection),
                ExportableGrassCoverSlipOffInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, assessmentSection),
                ExportablePipingStructureFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.PipingStructure, assessmentSection),
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.WaterPressureAsphaltCover, assessmentSection)
            };
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableCombinedSectionAssembly"/> based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of <see cref="ExportableCombinedSectionAssembly"/> for.</param>
        /// <returns>A <see cref="CreateExportableCombinedSectionAssemblyCollection"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableCombinedSectionAssembly> CreateExportableCombinedSectionAssemblyCollection(IAssessmentSection assessmentSection)
        {
            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults = new List<CombinedFailureMechanismSectionAssemblyResult>();
            return ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(assemblyResults, assessmentSection.ReferenceLine);
        }
    }
}