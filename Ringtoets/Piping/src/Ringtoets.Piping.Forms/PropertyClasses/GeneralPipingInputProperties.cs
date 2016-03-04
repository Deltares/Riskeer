using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GeneralPipingInput"/> for properties panel.
    /// </summary>
    public class GeneralPipingInputProperties : ObjectProperties<GeneralPipingInput>
    {
        #region Model Factors

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.UpliftModelFactor;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.SellmeijerModelFactor;
            }
        }

        #endregion

        #region General

        [PropertyOrder(11)]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeight
        {
            get
            {
                return data.WaterVolumetricWeight;
            }
        }

        #endregion

        #region Heave

        [PropertyOrder(21)]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.CriticalHeaveGradient;
            }
        }

        #endregion

        #region Sellmeijer

        [PropertyOrder(31)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SandParticlesVolumicWeight_Description")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.SandParticlesVolumicWeight;
            }
        }

        [PropertyOrder(32)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WhitesDragCoefficient;
            }
        }

        [PropertyOrder(33)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.BeddingAngle;
            }
        }

        [PropertyOrder(34)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WaterKinematicViscosity;
            }
        }

        [PropertyOrder(35)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_Gravity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_Gravity_Description")]
        public double Gravity
        {
            get
            {
                return data.Gravity;
            }
        }

        [PropertyOrder(36)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.MeanDiameter70;
            }
        }

        [PropertyOrder(37)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.SellmeijerReductionFactor;
            }
        }

        #endregion

        #region Semi-probabilistic parameters

        [PropertyOrder(41)]
        [ResourcesCategory(typeof(Resources), "Categories_SemiProbabilisticParameters")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_A_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_A_Description")]
        public double A
        {
            get
            {
                return data.A;
            }
        }

        [PropertyOrder(42)]
        [ResourcesCategory(typeof(Resources), "Categories_SemiProbabilisticParameters")]
        [ResourcesDisplayName(typeof(Resources), "GenerapPipingInput_B_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_B_Description")]
        public double B
        {
            get
            {
                return data.B;
            }
        }

        #endregion
    }
}