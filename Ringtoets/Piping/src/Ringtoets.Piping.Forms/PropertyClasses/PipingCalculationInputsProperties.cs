using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Core.Common.Gui;
using Core.Common.Utils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.TypeConverters;
using Ringtoets.Piping.Forms.UITypeEditors;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    [ResourcesDisplayName(typeof(Resources), "PipingDataPropertiesDisplayName")]
    public class PipingCalculationInputsProperties : ObjectProperties<PipingCalculationInputs>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataNameDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataNameDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataWaterVolumetricWeightDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataWaterVolumetricWeightDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataUpliftModelFactorDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataUpliftModelFactorDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataAssessmentLevelDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataAssessmentLevelDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataPiezometricHeadExitDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataPiezometricHeadExitDescription")]
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

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataDampingFactorExitDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataDampingFactorExitDescription")]
        public LognormalDistribution DampingFactorExit
        {
            get
            {
                return data.PipingData.DampingFactorExit;
            }
            set
            {
                data.PipingData.DampingFactorExit = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(NormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataPhreaticLevelExitDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataPhreaticLevelExitDescription")]
        public NormalDistribution PhreaticLevelExit
        {
            get
            {
                return data.PipingData.PhreaticLevelExit;
            }
            set
            {
                data.PipingData.PhreaticLevelExit = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataPiezometricHeadPolderDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataPiezometricHeadPolderDescription")]
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

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataCriticalHeaveGradientDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataCriticalHeaveGradientDescription")]
        public LognormalDistribution CriticalHeaveGradient
        {
            get
            {
                return data.PipingData.CriticalHeaveGradient;
            }
            set
            {
                data.PipingData.CriticalHeaveGradient = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataThicknessCoverageLayerDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataThicknessCoverageLayerDescription")]
        public LognormalDistribution ThicknessCoverageLayer
        {
            get
            {
                return data.PipingData.ThicknessCoverageLayer;
            }
            set
            {
                data.PipingData.ThicknessCoverageLayer = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSellmeijerModelFactorDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSellmeijerModelFactorDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataSellmeijerReductionFactorDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSellmeijerReductionFactorDescription")]
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

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSeepageLengthDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSeepageLengthDescription")]
        public LognormalDistribution SeepageLength
        {
            get
            {
                return data.PipingData.SeepageLength;
            }
            set
            {
                data.PipingData.SeepageLength = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(ShiftedLognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataSandParticlesVolumicWeightDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSandParticlesVolumicWeightDescription")]
        public ShiftedLognormalDistribution SandParticlesVolumicWeight
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataWhitesDragCoefficientDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataWhitesDragCoefficientDescription")]
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

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataDiameter70DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataDiameter70Description")]
        public LognormalDistribution Diameter70
        {
            get
            {
                return data.PipingData.Diameter70;
            }
            set
            {
                data.PipingData.Diameter70 = value;
                data.PipingData.NotifyObservers();
            }
        }

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataDarcyPermeabilityDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataDarcyPermeabilityDescription")]
        public LognormalDistribution DarcyPermeability
        {
            get
            {
                return data.PipingData.DarcyPermeability;
            }
            set
            {
                data.PipingData.DarcyPermeability = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataWaterKinematicViscosityDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataWaterKinematicViscosityDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataGravityDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataGravityDescription")]
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

        [TypeConverter(typeof(LognormalDistributionTypeConverter))]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataThicknessAquiferLayerDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataThicknessAquiferLayerDescription")]
        public LognormalDistribution ThicknessAquiferLayer
        {
            get
            {
                return data.PipingData.ThicknessAquiferLayer;
            }
            set
            {
                data.PipingData.ThicknessAquiferLayer = value;
                data.PipingData.NotifyObservers();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "PipingDataMeanDiameter70DisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataMeanDiameter70Description")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataBeddingAngleDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataBeddingAngleDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataSurfaceLineDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSurfaceLineDescription")]
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
        [ResourcesDisplayName(typeof(Resources), "PipingDataSoilProfileDisplayName")]
        [ResourcesDescription(typeof(Resources), "PipingDataSoilProfileDescription")]
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