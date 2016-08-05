using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="PipingFailureMechanismContext"/> properties panel.
    /// </summary>
    public class PipingFailureMechanismContextProperties : ObjectProperties<PipingFailureMechanismContext>
    {
        #region General

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Common.Data.Properties.Resources), "FailureMechanism_Name_DisplayName")]
        [ResourcesDescription(typeof(Common.Data.Properties.Resources), "FailureMechanism_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Common.Data.Properties.Resources), "FailureMechanism_Code_DisplayName")]
        [ResourcesDescription(typeof(Common.Data.Properties.Resources), "FailureMechanism_Code_Description")]
        public string Code
        {
            get
            {
                return data.WrappedData.Code;
            }
        }

        [PropertyOrder(3)]
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeight
        {
            get
            {
                return data.WrappedData.GeneralInput.WaterVolumetricWeight;
            }
        }

        #endregion

        #region Model Factors

        [PropertyOrder(11)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.WrappedData.GeneralInput.UpliftModelFactor;
            }
        }

        [PropertyOrder(12)]
        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.WrappedData.GeneralInput.SellmeijerModelFactor;
            }
        }

        #endregion

        #region Semi-probabilistic parameters

        [PropertyOrder(21)]
        [ResourcesCategory(typeof(Resources), "Categories_SemiProbabilisticParameters")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_A_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_A_Description")]
        public double A
        {
            get
            {
                return data.WrappedData.PipingProbabilityAssessmentInput.A;
            }
            set
            {
                data.WrappedData.PipingProbabilityAssessmentInput.A = value;
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(22)]
        [ResourcesCategory(typeof(Resources), "Categories_SemiProbabilisticParameters")]
        [ResourcesDisplayName(typeof(Resources), "GenerapPipingInput_B_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_B_Description")]
        public double B
        {
            get
            {
                return data.WrappedData.PipingProbabilityAssessmentInput.B;
            }
        }

        #endregion

        #region Heave

        [PropertyOrder(31)]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.WrappedData.GeneralInput.CriticalHeaveGradient;
            }
        }

        #endregion

        #region Uplift

        [PropertyOrder(41)]
        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_UpliftCriticalSafetyFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_UpliftCriticalSafetyFactor_Description")]
        public double UpliftCriticalSafetyFactor
        {
            get
            {
                return data.WrappedData.PipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor;
            }
            set
            {
                data.WrappedData.PipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor = value;
                data.WrappedData.NotifyObservers();
            }
        }

        #endregion

        #region Sellmeijer

        [PropertyOrder(51)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SandParticlesVolumicWeight_Description")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.WrappedData.GeneralInput.SandParticlesVolumicWeight;
            }
        }

        [PropertyOrder(52)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WrappedData.GeneralInput.WhitesDragCoefficient;
            }
        }

        [PropertyOrder(53)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.WrappedData.GeneralInput.BeddingAngle;
            }
        }

        [PropertyOrder(54)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WrappedData.GeneralInput.WaterKinematicViscosity;
            }
        }

        [PropertyOrder(55)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_Gravity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_Gravity_Description")]
        public double Gravity
        {
            get
            {
                return data.WrappedData.GeneralInput.Gravity;
            }
        }

        [PropertyOrder(56)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.WrappedData.GeneralInput.MeanDiameter70;
            }
        }

        [PropertyOrder(57)]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "GeneralPipingInput_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "GeneralPipingInput_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.WrappedData.GeneralInput.SellmeijerReductionFactor;
            }
        }

        #endregion
    }
}