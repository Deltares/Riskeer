using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created
        /// for <paramref name="assessmentSection"/>.</exception>
        public static ExportableAssessmentSection CreateExportableAssessmentSection(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return new ExportableAssessmentSection(assessmentSection.Name,
                                                   assessmentSection.ReferenceLine.Points,
                                                   CreateExportableAssessmentSectionAssemblyResult(assessmentSection),
                                                   CreateExportableFailureMechanismsWithProbability(assessmentSection),
                                                   CreateExportableFailureMechanismsWithoutProbability(assessmentSection),
                                                   new ExportableCombinedSectionAssemblyCollection(Enumerable.Empty<ExportableCombinedFailureMechanismSection>(),
                                                                                                   Enumerable.Empty<ExportableCombinedSectionAssembly>()));
        }

        /// <summary>
        /// Creates a <see cref="ExportableAssessmentSectionAssemblyResult"/> with the assembly result
        /// based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a
        /// <see cref="ExportableAssessmentSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="ExportableAssessmentSectionAssemblyResult"/> with assembly results.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created
        /// for <paramref name="assessmentSection"/>.</exception>
        private static ExportableAssessmentSectionAssemblyResult CreateExportableAssessmentSectionAssemblyResult(AssessmentSection assessmentSection)
        {
            return new ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod.WBI2C1,
                                                                 AssessmentSectionAssemblyFactory.AssembleAssessmentSection(assessmentSection));
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// for failure mechanisms with an assembly result with a probability based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with probability for.</param>
        /// <returns>A a collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> based on failure
        /// mechanisms with assembly results with a probability.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created
        /// for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> CreateExportableFailureMechanismsWithProbability(AssessmentSection assessmentSection)
        {
            return new[]
            {
                ExportablePipingFailureMechanismFactory.CreateExportablePipingFailureMechanism(assessmentSection.Piping, assessmentSection),
                ExportableMacroStabilityInwardsFailureMechanismFactory.CreateExportableMacroStabilityInwardsFailureMechanism(assessmentSection.MacroStabilityInwards, assessmentSection),
                ExportableGrassCoverErosionInwardsFailureMechanismFactory.CreateExportableGrassCoverErosionInwardsFailureMechanism(assessmentSection.GrassCoverErosionInwards, assessmentSection),
                ExportableHeightStructuresFailureMechanismFactory.CreateExportableHeightStructuresFailureMechanism(assessmentSection.HeightStructures, assessmentSection),
                ExportableClosingStructuresFailureMechanismFactory.CreateExportableClosingStructuresFailureMechanism(assessmentSection.ClosingStructures, assessmentSection),
                ExportableStabilityPointStructuresFailureMechanismFactory.CreateExportableStabilityPointStructuresFailureMechanism(assessmentSection.StabilityPointStructures, assessmentSection)
            };
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// for failure mechanisms with an assembly result without a probability based on <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section to create a collection of
        /// <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> with probability for.</param>
        /// <returns>A a collection of <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> based on failure
        /// mechanisms with assembly results without a probability.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created
        /// for <paramref name="assessmentSection"/>.</exception>
        private static IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> CreateExportableFailureMechanismsWithoutProbability(AssessmentSection assessmentSection)
        {
            return new[]
            {
                ExportableStabilityStoneCoverFailureMechanismFactory.CreateExportableStabilityStoneCoverFailureMechanism(assessmentSection.StabilityStoneCover),
                ExportableWaveImpactAsphaltCoverFailureMechanismFactory.CreateExportableWaveImpactAsphaltCoverFailureMechanism(assessmentSection.WaveImpactAsphaltCover)
            };
        }
    }
}