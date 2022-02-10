using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Data.StandAlone.AssemblyFactories
{
    public static class StandAloneFailureMechanismAssemblyFactory
    {
        public static FailureMechanismSectionAssemblyResult AssembleSection(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult,
                                                                            IHasGeneralInput failureMechanism,
                                                                            IAssessmentSection assessmentSection)
        {
            return FailureMechanismSectionResultAssemblyFactory.AssembleSection(sectionResult, assessmentSection, failureMechanism.GeneralInput.ApplyLengthEffectInSection);
        }
    }
}