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
        public double UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftReliability_Description")]
        public double UpliftReliability
        {
            get
            {
                return data.UpliftReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_UpliftProbability_Description")]
        public double UpliftProbability
        {
            get
            {
                return data.UpliftProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveFactorOfSafety_Description")]
        public double HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveReliability_Description")]
        public double HeaveReliability
        {
            get
            {
                return data.HeaveReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_HeaveProbability_Description")]
        public double HeaveProbability
        {
            get
            {
                return data.HeaveProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerFactorOfSafety_Description")]
        public double SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerReliability_Description")]
        public double SellmeijerReliability
        {
            get
            {
                return data.SellmeijerReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_SellmeijerProbability_Description")]
        public double SellmeijerProbability
        {
            get
            {
                return data.SellmeijerProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredProbability_Description")]
        public double RequiredProbability
        {
            get
            {
                return data.RequiredProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_RequiredReliability_Description")]
        public double RequiredReliability
        {
            get
            {
                return data.RequiredReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingProbability_Description")]
        public double PipingProbability
        {
            get
            {
                return data.PipingProbability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingReliability_Description")]
        public double PipingReliability
        {
            get
            {
                return data.PipingReliability;
            }
        }

        [ResourcesCategory(typeof(Resources), "PipingSemiProbabilisticOutput_Categories_Piping")]
        [ResourcesDisplayName(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingSemiProbabilisticOutput_PipingFactorOfSafety_Description")]
        public double PipingFactorOfSafety
        {
            get
            {
                return data.PipingFactorOfSafety;
            }
        }
    }
}