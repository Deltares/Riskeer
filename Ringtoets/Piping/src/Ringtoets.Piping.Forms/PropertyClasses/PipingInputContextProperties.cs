using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

using Core.Common.Gui;
using Core.Common.Utils.Attributes;

using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingInputContextProperties : ObjectProperties<PipingInputContext>
    {
        #region General

        [Editor(typeof(PipingInputContextSurfaceLineSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SurfaceLine_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SurfaceLine_Description")]
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return data.WrappedPipingInput.SurfaceLine;
            }
            set
            {
                data.WrappedPipingInput.SurfaceLine = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [Editor(typeof(PipingInputContextSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SoilProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SoilProfile_Description")]
        public PipingSoilProfile SoilProfile
        {
            get
            {
                return data.WrappedPipingInput.SoilProfile;
            }
            set
            {
                data.WrappedPipingInput.SoilProfile = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        #endregion

        #region Model Factors

        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.WrappedPipingInput.UpliftModelFactor;
            }
            set
            {
                data.WrappedPipingInput.UpliftModelFactor = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.WrappedPipingInput.SellmeijerModelFactor;
            }
            set
            {
                data.WrappedPipingInput.SellmeijerModelFactor = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        #endregion

        #region Heave

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.WrappedPipingInput.CriticalHeaveGradient;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadExit_Description")]
        public double PiezometricHeadExitHeave
        {
            get
            {
                return PiezometricHeadExit;
            }
            set
            {
                PiezometricHeadExit = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessCoverageLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessCoverageLayerHeave
        {
            get
            {
                return ThicknessCoverageLayer;
            }
            set
            {
                ThicknessCoverageLayer = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadPolder_Description")]
        public double PiezometricHeadPolderHeave
        {
            get
            {
                return PiezometricHeadPolder;
            }
            set
            {
                PiezometricHeadPolder = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DampingFactorExit_Description")]
        public DesignVariable<LognormalDistribution> DampingFactorExitHeave
        {
            get
            {
                return DampingFactorExit;
            }
            set
            {
                DampingFactorExit = value;
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExitHeave
        {
            get
            {
                return PhreaticLevelExit;
            }
            set
            {
                PhreaticLevelExit = value;
            }
        }

        #endregion

        #region Uplift 

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeightUplift
        {
            get
            {
                return WaterVolumetricWeight;
            }
            set
            {
                WaterVolumetricWeight = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_AssessmentLevel_Description")]
        public double AssessmentLevelUplift
        {
            get
            {
                return AssessmentLevel;
            }
            set
            {
                AssessmentLevel = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadExit_Description")]
        public double PiezometricHeadExitUplift
        {
            get
            {
                return PiezometricHeadExit;
            }
            set
            {
                PiezometricHeadExit = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DampingFactorExit_Description")]
        public DesignVariable<LognormalDistribution> DampingFactorExitUplift
        {
            get
            {
                return DampingFactorExit;
            }
            set
            {
                DampingFactorExit = value;
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExitUplift
        {
            get
            {
                return PhreaticLevelExit;
            }
            set
            {
                PhreaticLevelExit = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Uplift")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PiezometricHeadPolder_Description")]
        public double PiezometricHeadPolderUplift
        {
            get
            {
                return PiezometricHeadPolder;
            }
            set
            {
                PiezometricHeadPolder = value;
            }
        }

        #endregion

        #region Sellmeijer

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_AssessmentLevel_Description")]
        public double AssessmentLevelSellmeijer
        {
            get
            {
                return AssessmentLevel;
            }
            set
            {
                AssessmentLevel = value;
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_PhreaticLevelExit_Description")]
        public DesignVariable<NormalDistribution> PhreaticLevelExitSellmeijer
        {
            get
            {
                return PhreaticLevelExit;
            }
            set
            {
                PhreaticLevelExit = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.WrappedPipingInput.SellmeijerReductionFactor;
            }
            set
            {
                data.WrappedPipingInput.SellmeijerReductionFactor = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessCoverageLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessCoverageLayerSellmeijer
        {
            get
            {
                return ThicknessCoverageLayer;
            }
            set
            {
                ThicknessCoverageLayer = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SeepageLength_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SeepageLength_Description")]
        public DesignVariable<LognormalDistribution> SeepageLength
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.SeepageLength = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_SandParticlesVolumicWeight_Description")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.WrappedPipingInput.SandParticlesVolumicWeight;
            }
            set
            {
                data.WrappedPipingInput.SandParticlesVolumicWeight = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WrappedPipingInput.WhitesDragCoefficient;
            }
            set
            {
                data.WrappedPipingInput.WhitesDragCoefficient = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_Diameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_Diameter70_Description")]
        public DesignVariable<LognormalDistribution> Diameter70
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDiameter70(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.Diameter70 = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeightSellmeijer
        {
            get
            {
                return WaterVolumetricWeight;
            }
            set
            {
                WaterVolumetricWeight = value;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_DarcyPermeability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_DarcyPermeability_Description")]
        public DesignVariable<LognormalDistribution> DarcyPermeability
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.DarcyPermeability = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WrappedPipingInput.WaterKinematicViscosity;
            }
            set
            {
                data.WrappedPipingInput.WaterKinematicViscosity = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_Gravity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_Gravity_Description")]
        public double Gravity
        {
            get
            {
                return data.WrappedPipingInput.Gravity;
            }
            set
            {
                data.WrappedPipingInput.Gravity = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_ThicknessAquiferLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_ThicknessAquiferLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessAquiferLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.ThicknessAquiferLayer = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.WrappedPipingInput.MeanDiameter70;
            }
            set
            {
                data.WrappedPipingInput.MeanDiameter70 = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInput_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInput_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.WrappedPipingInput.BeddingAngle;
            }
            set
            {
                data.WrappedPipingInput.BeddingAngle = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

#endregion

        /// <summary>
        /// Gets the available surface lines on <see cref="PipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> GetAvailableSurfaceLines()
        {
            return data.AvailablePipingSurfaceLines;
        }

        /// <summary>
        /// Gets the available soil profiles on <see cref="PipingCalculationContext"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> GetAvailableSoilProfiles()
        {
            return data.AvailablePipingSoilProfiles;
        }

        private double WaterVolumetricWeight
        {
            get
            {
                return data.WrappedPipingInput.WaterVolumetricWeight;
            }
            set
            {
                data.WrappedPipingInput.WaterVolumetricWeight = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        private double PiezometricHeadPolder
        {
            get
            {
                return data.WrappedPipingInput.PiezometricHeadPolder;
            }
            set
            {
                data.WrappedPipingInput.PiezometricHeadPolder = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        private double AssessmentLevel
        {
            get
            {
                return data.WrappedPipingInput.AssessmentLevel;
            }
            set
            {
                data.WrappedPipingInput.AssessmentLevel = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        private double PiezometricHeadExit
        {
            get
            {
                return data.WrappedPipingInput.PiezometricHeadExit;
            }
            set
            {
                data.WrappedPipingInput.PiezometricHeadExit = value;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        private DesignVariable<LognormalDistribution> DampingFactorExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.DampingFactorExit = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        private DesignVariable<NormalDistribution> PhreaticLevelExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.PhreaticLevelExit = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }

        private DesignVariable<LognormalDistribution> ThicknessCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(data.WrappedPipingInput);
            }
            set
            {
                data.WrappedPipingInput.ThicknessCoverageLayer = value.Distribution;
                data.WrappedPipingInput.NotifyObservers();
            }
        }
    }
}