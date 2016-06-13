using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using CoreCommonResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingSemiProbabilisticOutputProperties : ObjectProperties<PipingSemiProbabilisticOutput>
    {
        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift", 1, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftFactorOfSafety_Description")]
        [PropertyOrder(1)]
        public RoundedDouble UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift", 1, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_Description")]
        [PropertyOrder(2)]
        public RoundedDouble UpliftReliability
        {
            get
            {
                return data.UpliftReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift", 1, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_Description")]
        [PropertyOrder(3)]
        public string UpliftProbability
        {
            get
            {
                return ToProbabilityFormat(data.UpliftProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave", 2, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_Description")]
        [PropertyOrder(11)]
        public RoundedDouble HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave", 2, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_Description")]
        [PropertyOrder(12)]
        public RoundedDouble HeaveReliability
        {
            get
            {
                return data.HeaveReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave", 2, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_Description")]
        [PropertyOrder(13)]
        public string HeaveProbability
        {
            get
            {
                return ToProbabilityFormat(data.HeaveProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer", 3, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_Description")]
        [PropertyOrder(21)]
        public RoundedDouble SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer", 3, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_Description")]
        [PropertyOrder(22)]
        public RoundedDouble SellmeijerReliability
        {
            get
            {
                return data.SellmeijerReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer", 3, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_Description")]
        [PropertyOrder(23)]
        public string SellmeijerProbability
        {
            get
            {
                return ToProbabilityFormat(data.SellmeijerProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_Description")]
        [PropertyOrder(31)]
        public string RequiredProbability
        {
            get
            {
                return ToProbabilityFormat(data.RequiredProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_Description")]
        [PropertyOrder(32)]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_Description")]
        [PropertyOrder(33)]
        public string PipingProbability
        {
            get
            {
                return ToProbabilityFormat(data.PipingProbability);
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_Description")]
        [PropertyOrder(34)]
        public RoundedDouble PipingReliability
        {
            get
            {
                return data.PipingReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping", 4, 4)]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_Description")]
        [PropertyOrder(35)]
        public RoundedDouble PipingFactorOfSafety
        {
            get
            {
                return data.PipingFactorOfSafety;
            }
        }

        private static string ToProbabilityFormat(RoundedDouble probability)
        {
            return string.Format(CoreCommonResources.ProbabilityPerYearFormat, (RoundedDouble) (1.0 / probability));
        }
    }
}