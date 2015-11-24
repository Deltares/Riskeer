using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

using Core.Common.Gui;
using Core.Common.Utils;

using Ringtoets.Piping.Calculation.Piping;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    public class PipingInputParametersContextProperties : ObjectProperties<PipingInputParametersContext>
    {
        #region General

        [Editor(typeof(PipingInputParametersContextSurfaceLineSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_SurfaceLine_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_SurfaceLine_Description")]
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return data.WrappedPipingInputParameters.SurfaceLine;
            }
            set
            {
                data.WrappedPipingInputParameters.SurfaceLine = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [Editor(typeof(PipingInputParametersContextSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_SoilProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_SoilProfile_Description")]
        public PipingSoilProfile SoilProfile
        {
            get
            {
                return data.WrappedPipingInputParameters.SoilProfile;
            }
            set
            {
                data.WrappedPipingInputParameters.SoilProfile = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        #endregion

        #region Model Factors

        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.WrappedPipingInputParameters.UpliftModelFactor;
            }
            set
            {
                data.WrappedPipingInputParameters.UpliftModelFactor = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_ModelFactors")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.WrappedPipingInputParameters.SellmeijerModelFactor;
            }
            set
            {
                data.WrappedPipingInputParameters.SellmeijerModelFactor = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        #endregion

        #region Heave

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.WrappedPipingInputParameters.CriticalHeaveGradient;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Heave")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PiezometricHeadExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_ThicknessCoverageLayer_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PiezometricHeadPolder_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_DampingFactorExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PhreaticLevelExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_WaterVolumetricWeight_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_AssessmentLevel_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PiezometricHeadExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_DampingFactorExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PhreaticLevelExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PiezometricHeadPolder_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_AssessmentLevel_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_PhreaticLevelExit_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.WrappedPipingInputParameters.SellmeijerReductionFactor;
            }
            set
            {
                data.WrappedPipingInputParameters.SellmeijerReductionFactor = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_ThicknessCoverageLayer_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_SeepageLength_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_SeepageLength_Description")]
        public DesignVariable<LognormalDistribution> SeepageLength
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.SeepageLength = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_SandParticlesVolumicWeight_Description")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.WrappedPipingInputParameters.SandParticlesVolumicWeight;
            }
            set
            {
                data.WrappedPipingInputParameters.SandParticlesVolumicWeight = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.WrappedPipingInputParameters.WhitesDragCoefficient;
            }
            set
            {
                data.WrappedPipingInputParameters.WhitesDragCoefficient = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_Diameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_Diameter70_Description")]
        public DesignVariable<LognormalDistribution> Diameter70
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDiameter70(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.Diameter70 = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_WaterVolumetricWeight_Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_DarcyPermeability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_DarcyPermeability_Description")]
        public DesignVariable<LognormalDistribution> DarcyPermeability
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.DarcyPermeability = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.WrappedPipingInputParameters.WaterKinematicViscosity;
            }
            set
            {
                data.WrappedPipingInputParameters.WaterKinematicViscosity = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_Gravity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_Gravity_Description")]
        public double Gravity
        {
            get
            {
                return data.WrappedPipingInputParameters.Gravity;
            }
            set
            {
                data.WrappedPipingInputParameters.Gravity = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_ThicknessAquiferLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_ThicknessAquiferLayer_Description")]
        public DesignVariable<LognormalDistribution> ThicknessAquiferLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.ThicknessAquiferLayer = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.WrappedPipingInputParameters.MeanDiameter70;
            }
            set
            {
                data.WrappedPipingInputParameters.MeanDiameter70 = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Sellmeijer")]
        [ResourcesDisplayName(typeof(Resources), "PipingInputParameters_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingInputParameters_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.WrappedPipingInputParameters.BeddingAngle;
            }
            set
            {
                data.WrappedPipingInputParameters.BeddingAngle = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

#endregion

        /// <summary>
        /// Gets the available surface lines on <see cref="PipingCalculationInputs"/>.
        /// </summary>
        public IEnumerable<RingtoetsPipingSurfaceLine> GetAvailableSurfaceLines()
        {
            return data.AvailablePipingSurfaceLines;
        }

        /// <summary>
        /// Gets the available soil profiles on <see cref="PipingCalculationInputs"/>.
        /// </summary>
        public IEnumerable<PipingSoilProfile> GetAvailableSoilProfiles()
        {
            return data.AvailablePipingSoilProfiles;
        }

        private double WaterVolumetricWeight
        {
            get
            {
                return data.WrappedPipingInputParameters.WaterVolumetricWeight;
            }
            set
            {
                data.WrappedPipingInputParameters.WaterVolumetricWeight = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        private double PiezometricHeadPolder
        {
            get
            {
                return data.WrappedPipingInputParameters.PiezometricHeadPolder;
            }
            set
            {
                data.WrappedPipingInputParameters.PiezometricHeadPolder = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        private double AssessmentLevel
        {
            get
            {
                return data.WrappedPipingInputParameters.AssessmentLevel;
            }
            set
            {
                data.WrappedPipingInputParameters.AssessmentLevel = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        private double PiezometricHeadExit
        {
            get
            {
                return data.WrappedPipingInputParameters.PiezometricHeadExit;
            }
            set
            {
                data.WrappedPipingInputParameters.PiezometricHeadExit = value;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        private DesignVariable<LognormalDistribution> DampingFactorExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.DampingFactorExit = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        private DesignVariable<NormalDistribution> PhreaticLevelExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.PhreaticLevelExit = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }

        private DesignVariable<LognormalDistribution> ThicknessCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(data.WrappedPipingInputParameters);
            }
            set
            {
                data.WrappedPipingInputParameters.ThicknessCoverageLayer = value.Distribution;
                data.WrappedPipingInputParameters.NotifyObservers();
            }
        }
    }
}