using Core.Common.Gui;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingOutputPropertiesDisplayName")]
    public class PipingOutputProperties : ObjectProperties<PipingOutput>
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
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_HeaveZValue_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_HeaveZValue_Description")]
        public double HeaveZValue
        {
            get
            {
                return data.HeaveZValue;
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
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_UpliftZValue_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_UpliftZValue_Description")]
        public double UpliftZValue
        {
            get
            {
                return data.UpliftZValue;
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
        [ResourcesDisplayName(typeof(Resources), "PipingOutput_SellmeijerZValue_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutput_SellmeijerZValue_Description")]
        public double SellmeijerZValue
        {
            get
            {
                return data.SellmeijerZValue;
            }
        }
    }
}