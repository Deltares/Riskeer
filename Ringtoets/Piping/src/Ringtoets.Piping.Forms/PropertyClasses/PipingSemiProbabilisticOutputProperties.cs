using Core.Common.Base.Data;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingSemiProbabilisticOutputProperties : ObjectProperties<PipingSemiProbabilisticOutput>
    {
        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftFactorOfSafety_Description")]
        public RoundedDouble UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_Description")]
        public RoundedDouble UpliftReliability
        {
            get
            {
                return data.UpliftReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_Description")]
        public RoundedDouble UpliftProbability
        {
            get
            {
                return data.UpliftProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_Description")]
        public RoundedDouble HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_Description")]
        public RoundedDouble HeaveReliability
        {
            get
            {
                return data.HeaveReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_Description")]
        public RoundedDouble HeaveProbability
        {
            get
            {
                return data.HeaveProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_Description")]
        public RoundedDouble SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_Description")]
        public RoundedDouble SellmeijerReliability
        {
            get
            {
                return data.SellmeijerReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_Description")]
        public RoundedDouble SellmeijerProbability
        {
            get
            {
                return data.SellmeijerProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_Description")]
        public RoundedDouble RequiredProbability
        {
            get
            {
                return data.RequiredProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_Description")]
        public RoundedDouble RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_Description")]
        public RoundedDouble PipingProbability
        {
            get
            {
                return data.PipingProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_Description")]
        public RoundedDouble PipingReliability
        {
            get
            {
                return data.PipingReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_Description")]
        public RoundedDouble PipingFactorOfSafety
        {
            get
            {
                return data.PipingFactorOfSafety;
            }
        }
    }
}