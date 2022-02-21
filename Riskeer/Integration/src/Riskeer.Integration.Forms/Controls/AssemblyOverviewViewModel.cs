using System;
using System.Collections.Generic;
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Forms.Controls
{
    public class AssemblyOverviewViewModel
    {
        public AssemblyOverviewViewModel(AssessmentSection assessmentSection)
        {
            AssessmentSection = assessmentSection;
            try
            {
                CombinedAssemblyResults = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection)
                                                                          .ToArray();
            }
            catch (Exception)
            {
                CombinedAssemblyResults = Array.Empty<CombinedFailureMechanismSectionAssemblyResult>();
            }
            FailureMechanisms = new List<Tuple<string, Dictionary<FailureMechanismSection, FailureMechanismSectionAssemblyResult>>>();
            CreateRows();
        }

        private void CreateRows()
        {
            foreach (IFailureMechanism failureMechanism in AssessmentSection.GetFailureMechanisms().Where(fm => fm.InAssembly))
            {
                if (failureMechanism is PipingFailureMechanism piping)
                {
                    FailureMechanisms.Add(CreateResults(piping, PipingAssemblyFunc));
                }

                if (failureMechanism is GrassCoverErosionInwardsFailureMechanism gekb)
                {
                    FailureMechanisms.Add(CreateResults(gekb, GrassCoverErosionInwardsAssemblyFunc));
                }
            }
        }

        private Tuple<string, Dictionary<FailureMechanismSection, FailureMechanismSectionAssemblyResult>> CreateResults<TFailureMechanism, TSectionResult>(
            TFailureMechanism failureMechanism, Func<TSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> performAssemblyFunc)
            where TFailureMechanism : IHasSectionResults<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            Dictionary<FailureMechanismSection, FailureMechanismSectionAssemblyResult> failureMechanismSectionAssemblyResults = failureMechanism.SectionResults.ToDictionary(
                sectionResult => sectionResult.Section,
                sectionResult => AssemblyToolHelper.AssembleFailureMechanismSection(sectionResult, sr => performAssemblyFunc(sr, AssessmentSection)));

            return new Tuple<string, Dictionary<FailureMechanismSection, FailureMechanismSectionAssemblyResult>>(
                                      failureMechanism.Name, failureMechanismSectionAssemblyResults);
        }

        public AssessmentSection AssessmentSection { get; }

        public CombinedFailureMechanismSectionAssemblyResult[] CombinedAssemblyResults { get; }

        public List<Tuple<string, Dictionary<FailureMechanismSection, FailureMechanismSectionAssemblyResult>>> FailureMechanisms { get; }

        #region Assembly Funcs

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> PipingAssemblyFunc =>
            (sectionResult, assessmentSection) => PipingFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.Piping, assessmentSection);

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverErosionInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverErosionInwards, assessmentSection);

        private static Func<AdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> MacroStabilityInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => MacroStabilityInwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.MacroStabilityInwards, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> MicrostabilityAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.Microstability, assessmentSection);

        private static Func<AdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> HeightStructuresAssemblyFunc =>
            (sectionResult, assessmentSection) => StructuresFailureMechanismAssemblyFactory.AssembleSection<HeightStructuresInput>(
                sectionResult, assessmentSection.HeightStructures, assessmentSection);

        private static Func<AdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> ClosingStructuresAssemblyFunc =>
            (sectionResult, assessmentSection) => StructuresFailureMechanismAssemblyFactory.AssembleSection<ClosingStructuresInput>(
                sectionResult, assessmentSection.HeightStructures, assessmentSection);

        private static Func<AdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> StabilityPointStructuresAssemblyFunc =>
            (sectionResult, assessmentSection) => StructuresFailureMechanismAssemblyFactory.AssembleSection<StabilityPointStructuresInput>(
                sectionResult, assessmentSection.HeightStructures, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverErosionOutwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => GrassCoverErosionOutwardsFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverErosionOutwards, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> StabilityStoneCoverAssemblyFunc =>
            (sectionResult, assessmentSection) => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.StabilityStoneCover, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> WaveImpactAsphaltCoverAssemblyFunc =>
            (sectionResult, assessmentSection) => WaveImpactAsphaltCoverFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.WaveImpactAsphaltCover, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> WaterPressureAsphaltCoverAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.WaterPressureAsphaltCover, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverSlipOffOutwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverSlipOffOutwards, assessmentSection);

        private static Func<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> GrassCoverSlipOffInwardsAssemblyFunc =>
            (sectionResult, assessmentSection) => StandAloneFailureMechanismAssemblyFactory.AssembleSection(
                sectionResult, assessmentSection.GrassCoverSlipOffInwards, assessmentSection);

        private static Func<NonAdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> PipingStructureAssemblyFunc =>
            FailureMechanismSectionAssemblyResultFactory.AssembleSection;

        private static Func<NonAdoptableFailureMechanismSectionResult, AssessmentSection, FailureMechanismSectionAssemblyResult> DuneErosionAssemblyFunc =>
            FailureMechanismSectionAssemblyResultFactory.AssembleSection;

        #endregion
    }
}