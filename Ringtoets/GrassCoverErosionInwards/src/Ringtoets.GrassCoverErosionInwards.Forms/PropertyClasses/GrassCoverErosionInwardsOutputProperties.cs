using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.GrassCoverErosionInwards.Data;
using CommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    public class GrassCoverErosionInwardsOutputProperties : ObjectProperties<GrassCoverErosionInwardsOutput>
    {
        [PropertyOrder(1)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_RequiredProbability_Displayname")]
        [ResourcesDescription(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_RequiredProbability_Description")]
        public string RequiredProbability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.RequiredProbability);
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_RequiredReliability_Displayname")]
        [ResourcesDescription(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_RequiredReliability_Description")]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_Probability_Displayname")]
        [ResourcesDescription(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_Probability_Description")]
        public string Probability
        {
            get
            {
                return ProbabilityFormattingHelper.Format(data.Probability);
            }
        }

        [PropertyOrder(4)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_Reliability_Displayname")]
        [ResourcesDescription(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_Reliability_Description")]
        public RoundedDouble Reliability
        {
            get
            {
                return data.Reliability;
            }
        }

        [PropertyOrder(5)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_FactorOfSafety_Displayname")]
        [ResourcesDescription(typeof(CommonFormsResources), "ProbabilityAssessmentOutput_FactorOfSafety_Description")]
        public RoundedDouble FactorOfSafety
        {
            get
            {
                return data.FactorOfSafety;
            }
        }

        [PropertyOrder(6)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_WaveHeight_Displayname")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_WaveHeight_Description")]
        public RoundedDouble WaveHeight
        {
            get
            {
                return data.WaveHeight;
            }
        }

        [PropertyOrder(7)]
        [ResourcesCategory(typeof(CommonFormsResources), "Categories_Result")]
        [ResourcesDisplayName(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_IsOvertoppingDominant_Displayname")]
        [ResourcesDescription(typeof(GrassCoverErosionInwardsFormsResources), "GrassCoverErosionInwardsOutput_IsOvertoppingDominant_Description")]
        public bool IsOvertoppingDominant
        {
            get
            {
                return data.IsOvertoppingDominant;
            }
        }
    }
}