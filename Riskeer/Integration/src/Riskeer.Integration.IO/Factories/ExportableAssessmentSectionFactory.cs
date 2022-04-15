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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.IO.Assembly;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

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
            return new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI2B1, assemblyResult.AssemblyGroup,
                                                                 assemblyResult.Probability);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism"/>
        /// for failure mechanisms that are in assembly based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism"/> with probability for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanism"/> based on failure
        /// mechanisms that are in assembly.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableFailureMechanism> CreateExportableFailureMechanisms(AssessmentSection assessmentSection)
        {
            var exportableFailureMechanisms = new List<ExportableFailureMechanism>();

            AddFailureMechanismWhenInAssembly<PipingFailureMechanism, AdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.Piping, assessmentSection, PipingFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                PipingFailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<GrassCoverErosionInwardsFailureMechanism, AdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.GrassCoverErosionInwards, assessmentSection, GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<MacroStabilityInwardsFailureMechanism, AdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.MacroStabilityInwards, assessmentSection, MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<MicrostabilityFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.Microstability, assessmentSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                FailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<StabilityStoneCoverFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.StabilityStoneCover, assessmentSection, StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<WaveImpactAsphaltCoverFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.WaveImpactAsphaltCover, assessmentSection, WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<WaterPressureAsphaltCoverFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.WaterPressureAsphaltCover, assessmentSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                FailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<GrassCoverErosionOutwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.GrassCoverErosionOutwards, assessmentSection, GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<GrassCoverSlipOffOutwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.GrassCoverSlipOffOutwards, assessmentSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                FailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<GrassCoverSlipOffInwardsFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.GrassCoverSlipOffInwards, assessmentSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                FailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<HeightStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.HeightStructures, assessmentSection, HeightStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StructuresFailureMechanismAssemblyFactory.AssembleSection<HeightStructuresInput>, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<ClosingStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.ClosingStructures, assessmentSection, ClosingStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StructuresFailureMechanismAssemblyFactory.AssembleSection<ClosingStructuresInput>, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<PipingStructureFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.PipingStructure, assessmentSection, PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass), ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<StabilityPointStructuresFailureMechanism, AdoptableFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.StabilityPointStructures, assessmentSection, StabilityPointStructuresFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                StructuresFailureMechanismAssemblyFactory.AssembleSection<StabilityPointStructuresInput>, ExportableFailureMechanismType.Generic);

            AddFailureMechanismWhenInAssembly<DuneErosionFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                exportableFailureMechanisms, assessmentSection.DuneErosion, assessmentSection, DuneErosionFailureMechanismAssemblyFactory.AssembleFailureMechanism,
                (sr, fm, ass) => FailureMechanismSectionAssemblyResultFactory.AssembleSection(sr, ass), ExportableFailureMechanismType.Generic);

            foreach (SpecificFailureMechanism specificFailureMechanism in assessmentSection.SpecificFailureMechanisms)
            {
                AddFailureMechanismWhenInAssembly<SpecificFailureMechanism, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>(
                    exportableFailureMechanisms, specificFailureMechanism, assessmentSection, FailureMechanismAssemblyFactory.AssembleFailureMechanism,
                    FailureMechanismAssemblyFactory.AssembleSection, ExportableFailureMechanismType.Specific);
            }

            return exportableFailureMechanisms;
        }

        private static void AddFailureMechanismWhenInAssembly<TFailureMechanism, TSectionResult>(
            List<ExportableFailureMechanism> exportableFailureMechanisms, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> assembleFailureMechanismFunc,
            Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc,
            ExportableFailureMechanismType failureMechanismType)
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanism.InAssembly)
            {
                exportableFailureMechanisms.Add(
                    ExportableFailureMechanismFactory.CreateExportableFailureMechanism(
                        failureMechanism, assessmentSection, assembleFailureMechanismFunc, assembleFailureMechanismSectionFunc, failureMechanismType));
            }
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableCombinedSectionAssembly"/> based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of <see cref="ExportableCombinedSectionAssembly"/> for.</param>
        /// <returns>A <see cref="CreateExportableCombinedSectionAssemblyCollection"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableCombinedSectionAssembly> CreateExportableCombinedSectionAssemblyCollection(AssessmentSection assessmentSection)
        {
            return ExportableCombinedSectionAssemblyFactory.CreateExportableCombinedSectionAssemblyCollection(
                AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection),
                assessmentSection);
        }
    }
}