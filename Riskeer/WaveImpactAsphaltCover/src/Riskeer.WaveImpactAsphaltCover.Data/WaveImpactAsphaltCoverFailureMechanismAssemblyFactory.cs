using System;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.WaveImpactAsphaltCover.Data
{
    /// <summary>
    /// Factory for assembling assembly results for a wave impact asphalt cover failure mechanism.
    /// </summary>
    public static class WaveImpactAsphaltCoverFailureMechanismAssemblyFactory
    {
        /// <summary>
        /// Assembles the section based on the input arguments.
        /// </summary>
        /// <param name="sectionResult">The section result to assemble.</param>
        /// <param name="failureMechanism">The <see cref="WaveImpactAsphaltCoverFailureMechanism"/> the section result belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> the section belongs to.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when the section could not be assembled.</exception>
        public static FailureMechanismSectionAssemblyResult AssembleSection(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                            WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                            IAssessmentSection assessmentSection)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, failureMechanism.GeneralWaveImpactAsphaltCoverInput.ApplyLengthEffectInSection);
        }
    }
}