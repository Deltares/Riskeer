using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.WaveImpactAsphaltCover.Data
{
    public static class WaveImpactAsphaltCoverFailureMechanismAssemblyFactory
    {
        public static FailureMechanismSectionAssemblyResult AssembleSection(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                            WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                            IAssessmentSection assessmentSection)
        {
            return FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, failureMechanism.GeneralWaveImpactAsphaltCoverInput.ApplyLengthEffectInSection);
        }
    }
}