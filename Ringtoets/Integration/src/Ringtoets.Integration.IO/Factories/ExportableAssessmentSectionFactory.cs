﻿using System;
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
        /// a <see cref="ExportableAssessmentSection"/> for.</param>
        /// <returns>A <see cref="ExportableAssessmentSection"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        public static ExportableAssessmentSection CreateExportableAssessmentSection(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ExportableAssessmentSection(assessmentSection.Name,
                                                   assessmentSection.ReferenceLine.Points,
                                                   CreateExportableAssessmentSectionAssemblyResult(assessmentSection),
                                                   CreateExportableFailureMechanismAssemblyResultWithProbability(assessmentSection),
                                                   CreateExportableFailureMechanismAssemblyResultWithoutProbability(assessmentSection),
                                                   CreateExportableFailureMechanismsWithProbability(assessmentSection),
                                                   CreateExportableFailureMechanismsWithoutProbability(assessmentSection),
                                                   CreateExportableCombinedSectionAssemblyCollection(assessmentSection));
        }

        /// <summary>
        /// Creates a <see cref="ExportableAssessmentSectionAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a <see cref="ExportableAssessmentSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="ExportableAssessmentSectionAssemblyResult"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableAssessmentSectionAssemblyResult CreateExportableAssessmentSectionAssemblyResult(AssessmentSection assessmentSection)
        {
            return new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI2C1,
                                                                 AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection));
        }

        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a  <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> for.</param>
        /// <returns>A <see cref="ExportableFailureMechanismAssemblyResultWithProbability"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableFailureMechanismAssemblyResultWithProbability CreateExportableFailureMechanismAssemblyResultWithProbability(AssessmentSection assessmentSection)
        {
            FailureMechanismAssembly assemblyResult = AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithProbability(assessmentSection);
            return new ExportableFailureMechanismAssemblyResultWithProbability(ExportableAssemblyMethod.WBI2B1,
                                                                               assemblyResult.Group,
                                                                               assemblyResult.Probability);
        }

        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanismAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a  <see cref="ExportableFailureMechanismAssemblyResult"/> for.</param>
        /// <returns>A <see cref="ExportableFailureMechanismAssemblyResult"/> with assembly result.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly result cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableFailureMechanismAssemblyResult CreateExportableFailureMechanismAssemblyResultWithoutProbability(AssessmentSection assessmentSection)
        {
            return new ExportableFailureMechanismAssemblyResult(ExportableAssemblyMethod.WBI2A1,
                                                                AssessmentSectionAssemblyFactory.AssembleFailureMechanismsWithoutProbability(assessmentSection));
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
                ExportableStabilityStoneCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StabilityStoneCover),
                ExportableWaveImpactAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.WaveImpactAsphaltCover),
                ExportableGrassCoverErosionOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverErosionOutwards),
                ExportableDuneErosionFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.DuneErosion),
                ExportableMacroStabilityOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.MacroStabilityOutwards, assessmentSection),
                ExportableMicrostabilityFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.Microstability),
                ExportableGrassCoverSlipOffOutwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards),
                ExportableGrassCoverSlipOffInwardsFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.GrassCoverSlipOffInwards),
                ExportablePipingStructureFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.PipingStructure),
                ExportableWaterPressureAsphaltCoverFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.WaterPressureAsphaltCover),
                ExportableStrengthStabilityLengthwiseConstructionFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction),
                ExportableTechnicalInnovationFailureMechanismFactory.CreateExportableFailureMechanism(assessmentSection.TechnicalInnovation)
            };
        }

        /// <summary>
        /// Creates a <see cref="ExportableCombinedSectionAssemblyCollection"/> based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to a <see cref="ExportableCombinedSectionAssemblyCollection"/> for.</param>
        /// <returns>A <see cref="CreateExportableCombinedSectionAssemblyCollection"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created for <paramref name="assessmentSection"/>.</exception>
        private static ExportableCombinedSectionAssemblyCollection CreateExportableCombinedSectionAssemblyCollection(AssessmentSection assessmentSection)
        {
            IEnumerable<CombinedFailureMechanismSectionAssemblyResult> assemblyResults = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection);
            return ExportableCombinedSectionAssemblyCollectionFactory.CreateExportableCombinedSectionAssemblyCollection(assemblyResults);
        }
    }
}