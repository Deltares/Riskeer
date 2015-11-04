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
    [ResourcesDisplayName(typeof(Resources), "PipingCalculationInputsProperties_DisplayName")]
    public class PipingCalculationInputsProperties : ObjectProperties<PipingCalculationInputs>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_Name_Description")]
        public string Name
        {
            get
            {
                return data.PipingData.Name;
            }
            set
            {
                data.PipingData.Name = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_WaterVolumetricWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_WaterVolumetricWeight_Description")]
        public double WaterVolumetricWeight
        {
            get
            {
                return data.PipingData.WaterVolumetricWeight;
            }
            set
            {
                data.PipingData.WaterVolumetricWeight = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_UpliftModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_UpliftModelFactor_Description")]
        public double UpliftModelFactor
        {
            get
            {
                return data.PipingData.UpliftModelFactor;
            }
            set
            {
                data.PipingData.UpliftModelFactor = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_AssessmentLevel_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_AssessmentLevel_Description")]
        public double AssessmentLevel
        {
            get
            {
                return data.PipingData.AssessmentLevel;
            }
            set
            {
                data.PipingData.AssessmentLevel = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_PiezometricHeadExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_PiezometricHeadExit_Description")]
        public double PiezometricHeadExit
        {
            get
            {
                return data.PipingData.PiezometricHeadExit;
            }
            set
            {
                data.PipingData.PiezometricHeadExit = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_DampingFactorExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_DampingFactorExit_Description")]
        public DesignVariable DampingFactorExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(data.PipingData);
            }
            set
            {
                data.PipingData.DampingFactorExit = (LognormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(NormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_PhreaticLevelExit_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_PhreaticLevelExit_Description")]
        public DesignVariable PhreaticLevelExit
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(data.PipingData);
            }
            set
            {
                data.PipingData.PhreaticLevelExit = (NormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_PiezometricHeadPolder_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_PiezometricHeadPolder_Description")]
        public double PiezometricHeadPolder
        {
            get
            {
                return data.PipingData.PiezometricHeadPolder;
            }
            set
            {
                data.PipingData.PiezometricHeadPolder = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_CriticalHeaveGradient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_CriticalHeaveGradient_Description")]
        public double CriticalHeaveGradient
        {
            get
            {
                return data.PipingData.CriticalHeaveGradient;
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_ThicknessCoverageLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_ThicknessCoverageLayer_Description")]
        public DesignVariable ThicknessCoverageLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(data.PipingData);
            }
            set
            {
                data.PipingData.ThicknessCoverageLayer = (LognormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_SellmeijerModelFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_SellmeijerModelFactor_Description")]
        public double SellmeijerModelFactor
        {
            get
            {
                return data.PipingData.SellmeijerModelFactor;
            }
            set
            {
                data.PipingData.SellmeijerModelFactor = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_SellmeijerReductionFactor_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_SellmeijerReductionFactor_Description")]
        public double SellmeijerReductionFactor
        {
            get
            {
                return data.PipingData.SellmeijerReductionFactor;
            }
            set
            {
                data.PipingData.SellmeijerReductionFactor = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_SeepageLength_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_SeepageLength_Description")]
        public DesignVariable SeepageLength
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(data.PipingData);
            }
            set
            {
                data.PipingData.SeepageLength = (LognormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_SandParticlesVolumicWeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_SandParticlesVolumicWeight_Description")]
        public double SandParticlesVolumicWeight
        {
            get
            {
                return data.PipingData.SandParticlesVolumicWeight;
            }
            set
            {
                data.PipingData.SandParticlesVolumicWeight = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_WhitesDragCoefficient_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_WhitesDragCoefficient_Description")]
        public double WhitesDragCoefficient
        {
            get
            {
                return data.PipingData.WhitesDragCoefficient;
            }
            set
            {
                data.PipingData.WhitesDragCoefficient = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_Diameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_Diameter70_Description")]
        public DesignVariable Diameter70
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDiameter70(data.PipingData);
            }
            set
            {
                data.PipingData.Diameter70 = (LognormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_DarcyPermeability_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_DarcyPermeability_Description")]
        public DesignVariable DarcyPermeability
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(data.PipingData);
            }
            set
            {
                data.PipingData.DarcyPermeability = (LognormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_WaterKinematicViscosity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_WaterKinematicViscosity_Description")]
        public double WaterKinematicViscosity
        {
            get
            {
                return data.PipingData.WaterKinematicViscosity;
            }
            set
            {
                data.PipingData.WaterKinematicViscosity = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_Gravity_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_Gravity_Description")]
        public double Gravity
        {
            get
            {
                return data.PipingData.Gravity;
            }
            set
            {
                data.PipingData.Gravity = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionDesignVariableTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_ThicknessAquiferLayer_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_ThicknessAquiferLayer_Description")]
        public DesignVariable ThicknessAquiferLayer
        {
            get
            {
                return PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(data.PipingData);
            }
            set
            {
                data.PipingData.ThicknessAquiferLayer = (LognormalDistribution)value.Distribution;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_MeanDiameter70_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_MeanDiameter70_Description")]
        public double MeanDiameter70
        {
            get
            {
                return data.PipingData.MeanDiameter70;
            }
            set
            {
                data.PipingData.MeanDiameter70 = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_BeddingAngle_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_BeddingAngle_Description")]
        public double BeddingAngle
        {
            get
            {
                return data.PipingData.BeddingAngle;
            }
            set
            {
                data.PipingData.BeddingAngle = value;
                data.PipingData.NotifyObservers();
            }
        }

        [Editor(typeof(PipingCalculationInputsSurfaceLineSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_SurfaceLine_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_SurfaceLine_Description")]
        public RingtoetsPipingSurfaceLine SurfaceLine
        {
            get
            {
                return data.PipingData.SurfaceLine;
            }
            set
            {
                data.PipingData.SurfaceLine = value;
                data.PipingData.NotifyObservers();
            }
        }

        [Editor(typeof(PipingCalculationInputsSoilProfileSelectionEditor), typeof(UITypeEditor))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingData_SoilProfile_DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingData_SoilProfile_Description")]
        public PipingSoilProfile SoilProfile
        {
            get
            {
                return data.PipingData.SoilProfile;
            }
            set
            {
                data.PipingData.SoilProfile = value;
                data.PipingData.NotifyObservers();
            }
        }

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
    }
}