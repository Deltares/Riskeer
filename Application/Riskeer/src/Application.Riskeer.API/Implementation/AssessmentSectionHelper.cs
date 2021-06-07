using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.IO;
using Riskeer.Integration.Data;

namespace Application.Riskeer.API.Implementation
{
    internal static class AssessmentSectionHelper {

        internal static AssessmentSection CreateAssessmentSection(ReferenceLineMeta meta,
                                                          double lowerLimitNorm,
                                                          double signalingNorm,
                                                          NormType normativeNorm)
        {
            AssessmentSection assessmentSection;
            var reader = new AssessmentSectionSettingsReader();
            var settings = reader.ReadSettings();
            AssessmentSectionSettings settingOfSelectedAssessmentSection = settings.FirstOrDefault(s => s.AssessmentSectionId == meta.AssessmentSectionId);
            if (settingOfSelectedAssessmentSection == null)
            {
                //log.Warn(Resources.AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_No_settings_found_for_AssessmentSection);
                assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike, lowerLimitNorm, signalingNorm);
            }
            else
            {
                assessmentSection = settingOfSelectedAssessmentSection.IsDune
                                        ? CreateDuneAssessmentSection(lowerLimitNorm,
                                                                      signalingNorm,
                                                                      settingOfSelectedAssessmentSection.N)
                                        : CreateDikeAssessmentSection(lowerLimitNorm,
                                                                      signalingNorm,
                                                                      settingOfSelectedAssessmentSection.N);
            }

            assessmentSection.Name = string.Format(global::Riskeer.Integration.Data.Properties.Resources.AssessmentSection_Id_0, meta.AssessmentSectionId);
            assessmentSection.Id = meta.AssessmentSectionId;

            if (!meta.ReferenceLine.Points.Any())
            {
                //log.Warn(Resources.AssessmentSectionFromFileCommandHandler_CreateAssessmentSection_Importing_ReferenceLineFailed);
            }
            else
            {
                assessmentSection.ReferenceLine.SetGeometry(meta.ReferenceLine.Points);
            }

            assessmentSection.FailureMechanismContribution.NormativeNorm = normativeNorm;

            return assessmentSection;
        }

        private static AssessmentSection CreateDikeAssessmentSection(double lowerLimitNorm, double signalingNorm, int n)
        {
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike, lowerLimitNorm, signalingNorm);
            SetFailureMechanismsValueN(assessmentSection, n);
            return assessmentSection;
        }

        private static AssessmentSection CreateDuneAssessmentSection(double lowerLimitNorm, double signalingNorm, int n)
        {
            var duneAssessmentSection = new AssessmentSection(AssessmentSectionComposition.Dune,
                                                              lowerLimitNorm,
                                                              signalingNorm);
            SetFailureMechanismsValueN(duneAssessmentSection, n);
            return duneAssessmentSection;
        }

        private static void SetFailureMechanismsValueN(AssessmentSection assessmentSection, int n)
        {
            var roundedN = (RoundedDouble)n;
            assessmentSection.GrassCoverErosionInwards.GeneralInput.N = roundedN;
            assessmentSection.GrassCoverErosionOutwards.GeneralInput.N = roundedN;
            assessmentSection.HeightStructures.GeneralInput.N = roundedN;
        }
    }
}