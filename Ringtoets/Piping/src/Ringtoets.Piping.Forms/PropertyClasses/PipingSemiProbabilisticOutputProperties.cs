using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingSemiProbabilisticOutputProperties : ObjectProperties<PipingSemiProbabilisticOutput>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_HeaveFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_HeaveFactorOfSafety_Description")]
        public double HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_UpliftFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_UpliftFactorOfSafety_Description")]
        public double UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_SellmeijerFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_SellmeijerFactorOfSafety_Description")]
        public double SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_PipingFactorOfSafety_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_PipingFactorOfSafety_Description")]
        public double PipingFactorOfSafety
        {
            get
            {
                return data.PipingFactorOfSafety;
            }
        }
    }
}