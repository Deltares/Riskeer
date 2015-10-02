using DelftTools.Shell.Gui;
using DelftTools.Utils;
using Wti.Data;
using Wti.Forms.Properties;

namespace Wti.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingOutputPropertiesDisplayName")]
    public class PipingOutputProperties : ObjectProperties<PipingOuput>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutputHeaveFactorOfSafetyDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutputHeaveFactorOfSafetyDescription")]
        public double HeaveFactorOfSafety
        {
            get
            {
                return data.HeaveFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutputHeaveZValueDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutputHeaveZValueDescription")]
        public double HeaveZValue
        {
            get
            {
                return data.HeaveZValue;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutputUpliftFactorOfSafetyDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutputUpliftFactorOfSafetyDescription")]
        public double UpliftFactorOfSafety
        {
            get
            {
                return data.UpliftFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutputUpliftZValueDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutputUpliftZValueDescription")]
        public double UpliftZValue
        {
            get
            {
                return data.UpliftZValue;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutputSellmeijerFactorOfSafetyDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutputSellmeijerFactorOfSafetyDescription")]
        public double SellmeijerFactorOfSafety
        {
            get
            {
                return data.SellmeijerFactorOfSafety;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingOutputSellmeijerZValueDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingOutputSellmeijerZValueDescription")]
        public double SellmeijerZValue
        {
            get
            {
                return data.SellmeijerZValue;
            }
        }
    }
}